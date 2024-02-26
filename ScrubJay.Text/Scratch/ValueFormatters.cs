using ScrubJay.Collections;
using static ScrubJay.StaticImports;

namespace ScrubJay.Text.Scratch;

internal sealed class CharFormatter : ValueFormatter<char>
{
    public override void WriteValue(char ch, Buffer<char> buffer, text format = default, IFormatProvider? provider = default)
    {
        buffer.Add(ch);
    }
}

internal sealed class StringFormatter : ValueFormatter<string>
{
    public override void WriteValue(string? str, Buffer<char> buffer, text format = default, IFormatProvider? provider = default)
    {
        buffer.AddMany(str.AsSpan());
    }
}

internal sealed class CharArrayFormatter : ValueFormatter<char[]>
{
    public override void WriteValue(char[]? value, Buffer<char> buffer, text format = default, IFormatProvider? provider = default)
    {
        buffer.AddMany(value);
    }
}

public sealed class ValueFormatters
{
    private readonly List<IValueFormatter> _formatters;

    private ValueFormatters(ValueFormatters parent) : this()
    {
        _formatters = new(parent._formatters);
    }

    public ValueFormatters()
    {
        _formatters =
        [
            new CharFormatter(),
            new StringFormatter(),
            new CharArrayFormatter(),
        ];
    }

    public IValueFormatter GetFormatter(Type type)
    {
        var formatter = _formatters.LastOrDefault(fmt => fmt.CanFormat(type));
        return formatter ?? DefaultValueFormatter.Instance;
    }

    public IValueFormatter<T> GetFormatter<T>()
    {
        var formatter = _formatters
            .SelectWhere(fmt => fmt.CanFormat<T>() && fmt is IValueFormatter<T> valueFormatter ? Some(valueFormatter) : default)
            .LastOrDefault();
        return formatter ?? DefaultValueFormatter<T>.Instance;
    }

    public void Add<T>(IValueFormatter<T> valueFormatter)
    {
        _formatters.Add(valueFormatter);
    }

    public void Clear()
    {
        _formatters.Clear();
    }

    public ValueFormatters Clone() => new ValueFormatters(this);
}