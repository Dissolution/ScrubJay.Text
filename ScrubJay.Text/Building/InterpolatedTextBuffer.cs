namespace ScrubJay.Text.Building;

[InterpolatedStringHandler]
public ref struct InterpolatedFluentTextBuilder<TBuilder>
    where TBuilder : FluentTextBuilder<TBuilder>
{
    private readonly TBuilder _textBuilder;

    // ReSharper disable UnusedParameter.Local
    public InterpolatedFluentTextBuilder(int literalLength, int formattedCount, TBuilder textBuilder)
    // ReSharper restore UnusedParameter.Local
    {
        _textBuilder = textBuilder;
    }

    public void AppendLiteral(string literal)
    {
        _textBuilder.Append(literal);
    }

    public void AppendFormatted(char ch)
    {
        _textBuilder.Append(ch);
    }

    public void AppendFormatted(scoped text text)
    {
        _textBuilder.Append(text);
    }

    public void AppendFormatted(string? str)
    {
        _textBuilder.Append(str.AsSpan());
    }

    public void AppendFormatted<T>(T? value)
    {
        _textBuilder.Append<T>(value);
    }

    public void AppendFormatted<T>(T? value, string? format)
    {
        _textBuilder.Append<T>(value, format);
    }

    public void AppendFormatted<T>(T? value, text format)
    {
        _textBuilder.Append<T>(value, format);
    }

    public override bool Equals(object? obj) => throw new NotSupportedException();

    public override int GetHashCode() => throw new NotSupportedException();

    public override string ToString()
    {
        return _textBuilder.ToString();
    }
}