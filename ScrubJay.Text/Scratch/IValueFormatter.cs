using ScrubJay.Collections;

namespace ScrubJay.Text.Scratch;

public interface IValueFormatter
{
    bool CanFormat(Type type);
    bool CanFormat<T>();

    void WriteObject(
        object? obj, 
        Buffer<char> buffer, 
        text format = default, 
        IFormatProvider? provider = default);
}

public interface IValueFormatter<in T> : IValueFormatter
{
    void WriteValue(
        T? value, 
        Buffer<char> buffer, 
        text format = default, 
        IFormatProvider? provider = default);
}