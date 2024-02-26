using ScrubJay.Collections;

namespace ScrubJay.Text.Scratch;

public class DefaultValueFormatter : ValueFormatter, IValueFormatter
{
    public static DefaultValueFormatter Instance { get; } = new();
    
    public override bool CanFormat(Type type) => true;

    public override bool CanFormat<T>() => true;

    public override void WriteObject(object? obj, Buffer<char> buffer, text format = default, IFormatProvider? provider = default)
    {
        string? str;
        // ReSharper disable once MergeCastWithTypeCheck
        if (obj is IFormattable)
        {
#if NET6_0_OR_GREATER
            // ReSharper disable once MergeCastWithTypeCheck
            if (obj is ISpanFormattable)
            {
                do
                {
                    var dest = buffer.GetUnwrittenSpan();
                    if (((ISpanFormattable)obj).TryFormat(dest, out int charsWritten, format, provider))
                    {
                        buffer.Count += charsWritten;
                        return;
                    }
                    buffer.IncreaseCapacity();
                } while (true);
            }
#endif
            str = ((IFormattable)obj).ToString(format.ToString(), provider);
        }
        else
        {
            str = obj?.ToString();
        }

        buffer.AddMany(str.AsSpan());
    }
}