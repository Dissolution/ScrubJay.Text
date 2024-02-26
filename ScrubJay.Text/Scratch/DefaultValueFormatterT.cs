using ScrubJay.Collections;

namespace ScrubJay.Text.Scratch;

public class DefaultValueFormatter<T> : ValueFormatter<T>, IValueFormatter<T>, IValueFormatter
{
    public static DefaultValueFormatter<T> Instance { get; } = new();

    
    public override bool CanFormat(Type type) => true;

    public override bool CanFormat<T1>() => true;
    
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

    public override void WriteValue(T? value, Buffer<char> buffer, text format = default, IFormatProvider? provider = default)
    {
        string? str;
        // ReSharper disable once MergeCastWithTypeCheck
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            // ReSharper disable once MergeCastWithTypeCheck
            if (value is ISpanFormattable)
            {
                do
                {
                    var dest = buffer.GetUnwrittenSpan();
                    if (((ISpanFormattable)value).TryFormat(dest, out int charsWritten, format, provider))
                    {
                        buffer.Count += charsWritten;
                        return;
                    }

                    buffer.IncreaseCapacity();
                } while (true);
            }
#endif
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        buffer.AddMany(str.AsSpan());
    }

   
}