using ScrubJay.Collections;

namespace ScrubJay.Text.Scratch;

public abstract class ValueFormatter : IValueFormatter
{
    public abstract bool CanFormat(Type type);

    public virtual bool CanFormat<T>() => CanFormat(typeof(T));
    
    public abstract void WriteObject(
        object? obj, 
        Buffer<char> buffer, 
        text format = default, 
        IFormatProvider? provider = default);
}