using ScrubJay.Memory;

using static ScrubJay.StaticImports;

namespace ScrubJay.Text.Memory;

public static class CharSpanWriterExtensions
{
    public static Result TryWrite(
        this SpanWriter<char> textWriter,
        string? text)
    {
        if (text is null) return Ok();

        int index = textWriter.Position;
        int newIndex = index + text.Length;
        var span = textWriter.AvailableItems;
        if (newIndex <= span.Length)
        {
            TextHelper.CopyTo(text, span);
            textWriter.Position = newIndex;
            return Ok();
        }

        return new InvalidOperationException($"Cannot add '{text}': Only a capacity of {span.Length - index} characters remains");
    }

    public static Result TryWrite<T>(this SpanWriter<char> textWriter, T? value, scoped text format = default, IFormatProvider? provider = default)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                if (((ISpanFormattable)value).TryFormat(textWriter.AvailableItems, out int charsWritten, format, provider))
                {
                    textWriter.Position += charsWritten;
                    return Ok();
                }

                return new InvalidOperationException(
                    $"Cannot write '{value}': Only a capacity of {textWriter.AvailableItems.Length} remains");
            }
#endif
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        return TryWrite(textWriter, str);
    }

    public static Result TryWrite<T>(this SpanWriter<char> textWriter, T? value, string? format, IFormatProvider? provider = default)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                if (((ISpanFormattable)value).TryFormat(textWriter.AvailableItems, out int charsWritten, format.AsSpan(), provider))
                {
                    textWriter.Position += charsWritten;
                    return Ok();
                }

                return new InvalidOperationException(
                    $"Cannot write '{value}': Only a capacity of {textWriter.AvailableItems.Length} remains");
            }
#endif
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }

        return TryWrite(textWriter, str);
    }
}