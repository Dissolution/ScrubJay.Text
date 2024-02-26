using System.Diagnostics;
using ScrubJay.Memory;
using static ScrubJay.StaticImports;

using ScrubJay.Text.Building;

namespace ScrubJay.Text.Utilities;

public static class TextFormatter
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static FormatException CreateFormatException(text format, int pos, string? details = null)
    {
        using var message = TextBuilder.New.AppendLine($"Invalid Format at position {pos}");
        int start = (pos - 16).Clamp(0, pos);
        int end = (pos + 16).Clamp(pos, format.Length - 1);
        message.Append(format[new Range(start, end)])
            .If(details is not null, msg => msg.AppendLine().Append($"Details: {details}"));
        return new FormatException(message.ToString());
    }
    
    private static Result<char> TryMoveNext(text format, ref int pos)
    {
        pos += 1;
        if (pos < format.Length)
            return Ok(format[pos]);
        return Error<char>(CreateFormatException(format, pos, "Attempted to move past final character"));
    }


    private static Result<int> TryWriteFormatLine(Span<char> destination, text format, object?[] args, IFormatProvider? provider)
    {
        Exception? ex;
        int pos = 0;
        char ch;
        
        SpanWriter<char> buffer = stackalloc char[destination.Length];
        
        // Repeatedly find the next hole and process it.
        while (true)
        {
            // Skip until either the end of the input or the first unescaped opening brace, whichever comes first.
            // Along the way we need to also unescape escaped closing braces.
            while (true)
            {
                // End?   
                if (pos >= format.Length)
                {
                    buffer.WrittenItems.CopyTo(destination);
                    return Ok(buffer.Position);
                }

                // Find the next brace.
                text remainder = format.Slice(pos);
                int countUntilNextBrace = remainder.IndexOfAny('{', '}');
                
                // If there isn't one, the remainder of the input is text to be appended and we're done.
                if (countUntilNextBrace < 0)
                {
                    var r = buffer.TryWriteMany(remainder);
                    if (r.IsError(out ex))
                        return Error<int>(ex);
                    
                    buffer.WrittenItems.CopyTo(destination);
                    return Ok(buffer.Position);
                }

                // Append the text before the brace
                var result = buffer.TryWriteMany(remainder.Slice(0, countUntilNextBrace));
                if (result.IsError(out ex))
                    return Error<int>(ex);
                
                pos += countUntilNextBrace;

                // Get the brace.
                // It must be followed by another character, either a copy of itself in the case of being escaped,
                // or an arbitrary character that's part of the hole in the case of an opening brace.
                char brace = format[pos];

                var moveResult = TryMoveNext(format, ref pos);
                if (!moveResult.IsOk(out ch, out ex))
                    return Error<int>(ex);
                // If it's the same character, it's an escape, just append a single copy and continue
                if (brace == ch)
                {
                    result = buffer.TryWrite(ch);
                    if (result.IsError(out ex))
                        return Error<int>(ex);

                    pos++;
                    continue;
                }

                // This wasn't an escape, so it must be an opening brace.
                if (brace != '{')
                    return Error<int>(CreateFormatException(format, pos, "Missing opening brace"));

                // Proceed to parse the hole.
                break;
            }

            // We're now positioned just after the opening brace of an argument hole, which consists of
            // an opening brace, an index, and an optional format
            // preceded by a colon, with arbitrary amounts of spaces throughout.
            text itemFormatSpan = default; // used if itemFormat is null

            // First up is the index parameter, which is of the form:
            //     at least on digit
            //     optional any number of spaces
            // We've already read the first digit into ch.
            Debug.Assert(format[pos - 1] == '{');
            Debug.Assert(ch != '{');
            int index = ch - '0';
            // Has to be between 0 and 9
            if ((uint)index >= 10U)
                return Error<int>(CreateFormatException(format, pos, "Invalid character in index"));

            // Common case is a single digit index followed by a closing brace.
            if (!TryMoveNext(format, ref pos).IsOk(out ch, out ex))
                return Error<int>(ex);
            // If it's not a closing brace, proceed to finish parsing the full hole format.
            if (ch != '}')
            {
                // Continue consuming optional additional digits.
                while (ch.IsAsciiDigit())
                {
                    // Shift by power of 10
                    index = (index * 10) + (ch - '0');
                    if (!TryMoveNext(format, ref pos).IsOk(out ch, out ex))
                        return Error<int>(ex);
                }

                // Consume optional whitespace.
                while (ch == ' ')
                {
                    if (!TryMoveNext(format, ref pos).IsOk(out ch, out ex))
                        return Error<int>(ex);
                }

                // We do not support alignment
                if (ch == ',')
                {
                    return new NotSupportedException("Alignment is not yet supported");
                }

                // The next character needs to either be a closing brace for the end of the hole,
                // or a colon indicating the start of the format.
                if (ch != '}')
                {
                    if (ch != ':')
                    {
                        // Unexpected character
                        return CreateFormatException(format, pos, "Unexpected character");
                    }

                    // Search for the closing brace; everything in between is the format,
                    // but opening braces aren't allowed.
                    int startingPos = pos;
                    while (true)
                    {
                        if (!TryMoveNext(format, ref pos).IsOk(out ch, out ex))
                            return Error<int>(ex);

                        if (ch == '}')
                        {
                            // Argument hole closed
                            break;
                        }

                        if (ch == '{')
                        {
                            // Braces inside the argument hole are not supported
                            return CreateFormatException(format, pos, "Braces inside the argument hole are not supported");
                        }
                    }

                    startingPos++;
                    itemFormatSpan = format.Slice(startingPos, pos - startingPos);
                }
            }

            // Construct the output for this arg hole.
            Debug.Assert(format[pos] == '}');
            pos++;

            if ((uint)index >= (uint)args.Length)
            {
                return CreateFormatException(format, pos, $"Invalid Format: Argument '{index}' does not exist");
            }

            object? arg = args[index];

            // Append this arg
            var formatResult = TryFormat<object?>(buffer.AvailableItems, arg, itemFormatSpan, provider);
            if (formatResult.IsOk(out var charsWritten, out ex))
            {
                buffer.Position += charsWritten;
            }
            else
            {
                return ex;
            }

            // Continue parsing the rest of the format string.
        }
    }
    
    public static Result<int> TryFormat<T>(Span<char> destination,
        T? value,
        scoped text format = default,
        IFormatProvider? provider = default)
    {
        string? str;
        // ReSharper disable once MergeCastWithTypeCheck
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            // ReSharper disable once MergeCastWithTypeCheck
            if (value is ISpanFormattable)
            {
                if (((ISpanFormattable)value).TryFormat(destination, out int charsWritten, format, provider))
                {
                    return charsWritten;
                }
                return new ArgumentException("Destination buffer is not large enough", nameof(destination));
            }
#endif
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        return TextHelper.TryCopyTo(str, destination);
    }

    public static Result<int> TryFormat(Span<char> destination, scoped text format, params object?[] args)
    {
        return TryWriteFormatLine(destination, format, args, default);
    }
    
    public static Result<int> TryFormat(Span<char> destination, NonFormattableString format, params object?[] args)
    {
        return TryWriteFormatLine(destination, format.Text, args, default);
    }

    public static Result<int> TryFormat(Span<char> destination, FormattableString formatString, IFormatProvider? provider = default)
    {
        return TryWriteFormatLine(destination, formatString.Format.AsSpan(), formatString.GetArguments(), provider);
    }

//    public static Result<int> TryFormat<T>(Span<char> destination, ref DefaultInterpolatedStringHandler interpolatedString)
//    {
//        throw new NotImplementedException();
//    }
}