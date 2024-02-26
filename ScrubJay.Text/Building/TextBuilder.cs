using System.Diagnostics;
using System.Globalization;
using ScrubJay.Collections;
using ScrubJay.Text.Splitting;

namespace ScrubJay.Text.Building;

public sealed class TextBuilder : FluentTextBuilder<TextBuilder>
{
    public static TextBuilder New => new();

    public ref char this[int index] => ref _textBuffer[index];
    public Span<char> this[Range range] => _textBuffer[range];
    
    public TextBuilder()
    {
    }

    public TextBuilder(int minCapacity) : base(minCapacity)
    {
    }

    public TextBuilder(int literalLength, int formattedCount) : base(literalLength, formattedCount)
    {
    }
}

public abstract class FluentTextBuilder<TBuilder> : IBuildingText
    where TBuilder : FluentTextBuilder<TBuilder>
{
    public delegate void Build(TBuilder builder);
    public delegate void BuildWithText(TBuilder builder, text text);
    public delegate void BuildWithValue<in T>(TBuilder builder, T? value);
    public delegate void BuildWithIndexedValue<in T>(TBuilder builder, T? value, int index);
    
    
    
    protected readonly Buffer<char> _textBuffer;
    protected readonly TBuilder _self;

    public int Length => _textBuffer.Count;
    
    protected FluentTextBuilder()
    {
        _textBuffer = new();
        _self = (TBuilder)this;
    }

    protected FluentTextBuilder(int minCapacity)
    {
        _textBuffer = new(minCapacity);
        _self = (TBuilder)this;
    }

    protected FluentTextBuilder(int literalLength, int formattedCount)
    {
        _textBuffer = new(literalLength + (formattedCount * 16));
        _self = (TBuilder)this;
    }

    
    
    public virtual TBuilder NewLine() => Append(NewLineAndIndentManager.DefaultNewLine);

    public TBuilder NewLines(int count)
    {
        for (var i = 0; i < count; i++)
        {
            this.NewLine();
        }
        return _self;
    }

#region Append
    public virtual TBuilder Append(char ch)
    {
        _textBuffer.Add(ch);
        return _self;
    }

    public TBuilder AppendLine(char ch) => this.Append(ch).NewLine();

    public virtual TBuilder Append(scoped text text)
    {
        _textBuffer.AddMany(text);
        return _self;
    }

    public TBuilder AppendLine(scoped text text) => this.Append(text).NewLine();

    public virtual TBuilder Append(params char[]? characters)
    {
        _textBuffer.AddMany(characters);
        return _self;
    }

    public TBuilder AppendLine(params char[]? characters) => this.Append(characters).NewLine();

    public virtual TBuilder Append(string? str)
    {
        _textBuffer.AddMany(str.AsSpan());
        return _self;
    }

    public TBuilder AppendLine(string? str) => this.Append(str).NewLine();

    public TBuilder Append([InterpolatedStringHandlerArgument("")] ref InterpolatedFluentTextBuilder<TBuilder> interpolatedText)
    {
        // It has already written to us!
        return _self;
    }

    public TBuilder AppendLine([InterpolatedStringHandlerArgument("")] ref InterpolatedFluentTextBuilder<TBuilder> interpolatedText)
        => this.Append(ref interpolatedText).NewLine();

    public virtual TBuilder Append<T>(T? value)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                do
                {
                    var dest = _textBuffer.GetUnwrittenSpan();
                    if (((ISpanFormattable)value).TryFormat(dest, out int charsWritten, default, default))
                    {
                        _textBuffer.Count += charsWritten;
                        return _self;
                    }
                    _textBuffer.IncreaseCapacity();
                } while (true);
            }
#endif
            str = ((IFormattable)value).ToString(default, default);
        }
        else
        {
            str = value?.ToString();
        }
        _textBuffer.AddMany(str.AsSpan());
        return _self;
    }

    public TBuilder AppendLine<T>(T? value) => this.Append(value).NewLine();

    public virtual TBuilder Append<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                do
                {
                    var dest = _textBuffer.GetUnwrittenSpan();
                    if (((ISpanFormattable)value).TryFormat(dest, out int charsWritten, format.AsSpan(), provider))
                    {
                        _textBuffer.Count += charsWritten;
                        return _self;
                    }
                    _textBuffer.IncreaseCapacity();
                } while (true);
            }
#endif
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }
        _textBuffer.AddMany(str.AsSpan());
        return _self;
    }

    public TBuilder AppendLine<T>(T? value, string? format, IFormatProvider? provider = null)
        => this.Append(value, format, provider).NewLine();

    public virtual TBuilder Append<T>(T? value, scoped text format, IFormatProvider? provider = null)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                do
                {
                    var dest = _textBuffer.GetUnwrittenSpan();
                    if (((ISpanFormattable)value).TryFormat(dest, out int charsWritten, format, provider))
                    {
                        _textBuffer.Count += charsWritten;
                        return _self;
                    }
                    _textBuffer.IncreaseCapacity();
                } while (true);
            }
#endif
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }
        _textBuffer.AddMany(str.AsSpan());
        return _self;
    }

    public TBuilder AppendLine<T>(T? value, scoped text format, IFormatProvider? provider = null)
        => this.Append(value, format, provider).NewLine();
#endregion

#region Casing

    public TBuilder Case(char ch, Casing casing, CultureInfo? culture = default)
    {
        TextInfo textInfo = (culture ?? CultureInfo.CurrentCulture).TextInfo;
        switch (casing)
        {
            case Casing.Lower:
            case Casing.Camel:
                ch = textInfo.ToLower(ch);
                break;
            case Casing.Upper:
            case Casing.Pascal:
                ch = textInfo.ToUpper(ch);
                break;
        }
        return Append(ch);
    }
    
    public TBuilder Case(scoped text text, Casing casing, CultureInfo? culture = default)
    {
        int textLen = text.Length;
        if (textLen == 0)
            return _self;
        if (casing == Casing.Default)
            return Append(text);
        
        var buffer = _textBuffer.AllocateMany(textLen);
        TextInfo textInfo = (culture ?? CultureInfo.CurrentCulture).TextInfo;
        switch (casing)
        {
            case Casing.Lower:
            {
                for (var i = textLen - 1; i >= 0; i--)
                {
                    buffer[i] = textInfo.ToLower(text[i]);
                }
                break;
            }
            case Casing.Upper:
            {
                for (var i = textLen - 1; i >= 0; i--)
                {
                    buffer[i] = textInfo.ToUpper(text[i]);
                }
                break;
            }
            case Casing.Camel:
            {
                buffer[0] = textInfo.ToLower(text[0]);
                TextHelper.CopyTo(text.Slice(1), buffer.Slice(1));
                break;
            }
            case Casing.Pascal:
            {
                buffer[0] = textInfo.ToUpper(text[0]);
                TextHelper.CopyTo(text.Slice(1), buffer.Slice(1));
                break;
            }
        }
        return _self;
    }

    public TBuilder Case(string? str, Casing casing, CultureInfo? culture = default)
        => Case(str.AsSpan(), casing, culture);
    
    
    public TBuilder Case<T>(T? value, Casing casing)
        => this.Append(value?.ToString().ToCase(casing));
#endregion

#region Align
    public TBuilder Align(char ch, int width, Alignment alignment)
    {
        if (width < 1)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be 1 or greater");

        var appendSpan = _textBuffer.AllocateMany(width);
        if (alignment == Alignment.Left)
        {
            appendSpan[0] = ch;
            appendSpan[1..]
                .Fill(' ');
        }
        else if (alignment == Alignment.Right)
        {
            appendSpan[..^1]
                .Fill(' ');
            appendSpan[^1] = ch;
        }
        else // Center
        {
            int padding;
            // Odd width?
            if (width % 2 == 1)
            {
                padding = width / 2;
            }
            else // even
            {
                if (alignment.HasFlag(Alignment.Right)) // Right|Center?
                {
                    padding = width / 2;
                }
                else // Left|Center / Default|Center
                {
                    padding = width / 2 - 1;
                }
            }
            appendSpan[..padding]
                .Fill(' ');
            appendSpan[padding] = ch;
            appendSpan[(padding + 1)..]
                .Fill(' ');
        }
        return _self;
    }

    public TBuilder Align(string? str, int width, Alignment alignment) => Align(str.AsSpan(), width, alignment);

    public TBuilder Align(scoped text text, int width, Alignment alignment)
    {
        int textLen = text.Length;
        if (textLen == 0)
        {
            _textBuffer.AllocateMany(width).Fill(' ');
            return _self;
        }
        int spaces = width - textLen;
        if (spaces < 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, $"Width must be {textLen} or greater");

        if (spaces == 0)
        {
            _textBuffer.AddMany(text);
            return _self;
        }
        var appendSpan = _textBuffer.AllocateMany(width);
        if (alignment == Alignment.Left)
        {
            TextHelper.Unsafe.CopyBlock(text, appendSpan, textLen);
            appendSpan[textLen..]
                .Fill(' ');
        }
        else if (alignment == Alignment.Right)
        {
            appendSpan[..spaces]
                .Fill(' ');
            TextHelper.Unsafe.CopyBlock(text, appendSpan[spaces..], textLen);
        }
        else // Center
        {
            int frontPadding;
            // Even spacing is easy split
            if (spaces % 2 == 0)
            {
                frontPadding = spaces / 2;
            }
            else // Odd spacing we have to align
            {
                if (alignment.HasFlag(Alignment.Right)) // Right|Center
                {
                    frontPadding = (int)Math.Ceiling(spaces / 2d);
                }
                else // Center or Left|Center 
                {
                    frontPadding = (int)Math.Floor(spaces / 2d);
                }
            }
            appendSpan[..frontPadding]
                .Fill(' ');
            TextHelper.Unsafe.CopyBlock(text, appendSpan[frontPadding..], textLen);
            appendSpan[(frontPadding + textLen)..]
                .Fill(' ');
        }
        return _self;
    }
#endregion

#region Format
    protected void WriteFormatLine(text format, object?[] args)
    {
        // Undocumented exclusive limits on the range for Argument Hole Index
        const int INDEX_LIMIT = 1_000_000; // Note:            0 <= ArgIndex < IndexLimit

        // Repeatedly find the next hole and process it.
        int pos = 0;
        char ch;
        while (true)
        {
            // Skip until either the end of the input or the first unescaped opening brace, whichever comes first.
            // Along the way we need to also unescape escaped closing braces.
            while (true)
            {
                // Find the next brace.  If there isn't one, the remainder of the input is text to be appended, and we're done.
                if (pos >= format.Length)
                {
                    return;
                }

                text remainder = format.Slice(pos);
                int countUntilNextBrace = remainder.IndexOfAny('{', '}');
                if (countUntilNextBrace < 0)
                {
                    _textBuffer.AddMany(remainder);
                    return;
                }

                // Append the text until the brace.
                _textBuffer.AddMany(remainder.Slice(0, countUntilNextBrace));
                pos += countUntilNextBrace;

                // Get the brace.
                // It must be followed by another character, either a copy of itself in the case of being escaped,
                // or an arbitrary character that's part of the hole in the case of an opening brace.
                char brace = format[pos];
                ch = moveNext(format, ref pos);
                if (brace == ch)
                {
                    _textBuffer.Add(ch);
                    pos++;
                    continue;
                }

                // This wasn't an escape, so it must be an opening brace.
                if (brace != '{')
                {
                    throw createFormatException(format, pos, "Missing opening brace");
                }

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
            if ((uint)index >= 10u)
            {
                throw createFormatException(format, pos, "Invalid character in index");
            }

            // Common case is a single digit index followed by a closing brace.  If it's not a closing brace,
            // proceed to finish parsing the full hole format.
            ch = moveNext(format, ref pos);
            if (ch != '}')
            {
                // Continue consuming optional additional digits.
                while (ch.IsAsciiDigit() && index < INDEX_LIMIT)
                {
                    // Shift by power of 10
                    index = index * 10 + (ch - '0');
                    ch = moveNext(format, ref pos);
                }

                // Consume optional whitespace.
                while (ch == ' ')
                {
                    ch = moveNext(format, ref pos);
                }

                // We do not support alignment
                if (ch == ',')
                {
                    throw createFormatException(format, pos, "Alignment is not supported");
                }

                // The next character needs to either be a closing brace for the end of the hole,
                // or a colon indicating the start of the format.
                if (ch != '}')
                {
                    if (ch != ':')
                    {
                        // Unexpected character
                        throw createFormatException(format, pos, "Unexpected character");
                    }

                    // Search for the closing brace; everything in between is the format,
                    // but opening braces aren't allowed.
                    int startingPos = pos;
                    while (true)
                    {
                        ch = moveNext(format, ref pos);

                        if (ch == '}')
                        {
                            // Argument hole closed
                            break;
                        }

                        if (ch == '{')
                        {
                            // Braces inside the argument hole are not supported
                            throw createFormatException(format, pos, "Braces inside the argument hole are not supported");
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
                throw createFormatException(format, pos, $"Invalid Format: Argument '{index}' does not exist");
            }

            string? itemFormat = null;
            if (itemFormatSpan.Length > 0)
                itemFormat = itemFormatSpan.ToString();

            object? arg = args[index];

            // Append this arg, allows for overridden behavior
            this.Append<object?>(arg, itemFormat);

            // Continue parsing the rest of the format string.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static char moveNext(text format, ref int pos)
        {
            pos++;
            if (pos < format.Length)
                return format[pos];

            throw createFormatException(format, pos, "Attempted to move past final character");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static FormatException createFormatException(text format, int pos, string? details = null)
        {
            var message = StringBuilderPool.Rent();
            message.Append("Invalid Format at position ");
            message.Append(pos);
            message.Append(Environment.NewLine);
            int start = pos - 16;
            if (start < 0)
                start = 0;
            int end = pos + 16;
            if (end > format.Length)
                end = format.Length - 1;
            message.Append(format[new Range(start, end)]);
            if (details is not null)
            {
                message.Append(Environment.NewLine);
                message.Append("Details: ");
                message.Append(details);
            }
            return new FormatException(message.ToStringAndReturn());
        }
    }

    public TBuilder Format(NonFormattableString format, params object?[] args)
    {
        WriteFormatLine(format.Text, args);
        return _self;
    }

    public TBuilder Format(FormattableString formattableString)
    {
        WriteFormatLine(formattableString.Format.AsSpan(), formattableString.GetArguments());
        return _self;
    }

    public TBuilder Format([InterpolatedStringHandlerArgument("")] ref InterpolatedTextBuilder interpolatedText)
    {
        // Exactly like Write, the work is already done
        return _self;
    }
#endregion


#region Enumerate


    
    public TBuilder Enumerate(SplitTextEnumerable enumerable, BuildWithText perSplitSection)
    {
        foreach (var splitSection in enumerable)
        {
            perSplitSection(_self, splitSection);
        }
        return _self;
    }

    public TBuilder Enumerate<T>(ReadOnlySpan<T> values, BuildWithValue<T> perValue)
    {
        for (var i = 0; i < values.Length; i++)
        {
            perValue(_self, values[i]);
        }
        return _self;
    }

    public TBuilder Enumerate<T>(IEnumerable<T> values, BuildWithValue<T> perValue)
    {
        foreach (var value in values)
        {
            perValue(_self, value);
        }
        return _self;
    }

    
    public TBuilder Iterate<T>(ReadOnlySpan<T> values, BuildWithIndexedValue<T> perValueIndex)
    {
        for (var i = 0; i < values.Length; i++)
        {
            perValueIndex(_self, values[i], i);
        }
        return _self;
    }

    public TBuilder Iterate<T>(IEnumerable<T> values, BuildWithIndexedValue<T> perValueIndex)
    {
        if (values is IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                perValueIndex(_self, list[i], i);
            }
        }
        else
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return _self;

            int i = 0;
            perValueIndex(_self, e.Current, i);
            while (e.MoveNext())
            {
                i++;
                perValueIndex(_self, e.Current, i);
            }
        }
        return _self;
    }
#endregion

#region Delimit
    public TBuilder Delimit(
        Build delimit,
        SplitTextEnumerable enumerable,
        BuildWithText perSplitSection)
    {
        var splitEnumerator = enumerable.GetEnumerator();
        if (!splitEnumerator.MoveNext()) return _self;
        perSplitSection(_self, splitEnumerator.Current);
        while (splitEnumerator.MoveNext())
        {
            delimit(_self);
            perSplitSection(_self, splitEnumerator.Current);
        }
        return _self;
    }

    public TBuilder Delimit<T>(Build delimit, ReadOnlySpan<T> values, BuildWithValue<T> perValue)
    {
        var count = values.Length;
        if (count == 0)
            return _self;

        perValue(_self, values[0]);
        for (var i = 1; i < count; i++)
        {
            delimit(_self);
            perValue(_self, values[i]);
        }
        return _self;
    }

    public TBuilder Delimit<T>(Build delimit, IEnumerable<T> values, BuildWithValue<T> perValue)
    {
        if (values is IList<T> list)
        {
            var count = list.Count;
            if (count == 0)
                return _self;

            perValue(_self, list[0]);
            for (var i = 1; i < count; i++)
            {
                delimit(_self);
                perValue(_self, list[i]);
            }
        }
        else
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
                return _self;

            perValue(_self, e.Current);
            while (e.MoveNext())
            {
                delimit(_self);
                perValue(_self, e.Current);
            }
        }

        return _self;
    }
#endregion


    public TBuilder If(
        bool predicateResult,
        Build? ifTrue,
        Build? ifFalse = null)
    {
        if (predicateResult)
        {
            ifTrue?.Invoke(_self);
        }
        else
        {
            ifFalse?.Invoke(_self);
        }
        return _self;
    }

    public TBuilder IfAppend<T>(T? value, Build? ifAppended, Build? ifNotAppended = null)
    {
        if (this.Wrote(b => b.Append<T>(value)))
            return Invoke(ifAppended);

        return _self;
    }

    public bool Wrote(Build? build)
    {
        int pos = _textBuffer.Count;
        build?.Invoke(_self);
        return _textBuffer.Count > pos;
    }


    public TBuilder GetWritten(Build build, out Span<char> written)
    {
        int start = _textBuffer.Count;
        build(_self);
        int end = _textBuffer.Count;
        written = _textBuffer[new Range(start: start, end: end)];
        return _self;
    }

    public TBuilder Invoke(Build? tba)
    {
        tba?.Invoke(_self);
        return _self;
    }


    public void Clear() => _textBuffer.Clear();

    public Span<char> AsSpan() => _textBuffer.AsSpan();
    

    public void Dispose()
    {
        _textBuffer.Dispose();
    }

    public string ToStringAndDispose()
    {
        string str = _textBuffer.ToString();
        _textBuffer.Dispose();
        return str;
    }

    public override string ToString()
    {
        return _textBuffer.ToString();
    }
}