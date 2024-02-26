using ScrubJay.Collections;

namespace ScrubJay.Text.Scratch;

public abstract class ValueFormatter<T> : ValueFormatter, IValueFormatter<T>
{
    public override bool CanFormat(Type type)
    {
        return type.Implements<T>();
    }

    public override void WriteObject(object? obj, Buffer<char> buffer, text format = default, IFormatProvider? provider = default)
    {
        if (obj is null)
        {
            if (typeof(T).CanContainNull())
            {
                WriteValue(default, buffer, format, provider);
            }
            else
            {
                throw new ArgumentNullException(nameof(obj));
            }
        }
        else if (obj is T value)
        {
            WriteValue(value, buffer, format, provider);
        }
        else
        {
            throw new ArgumentException($"Object does not implement {typeof(T).Name}", nameof(obj));
        }
    }

    public abstract void WriteValue(
        T? value, 
        Buffer<char> buffer, 
        text format = default, 
        IFormatProvider? provider = default);
}

