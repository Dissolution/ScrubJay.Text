namespace ScrubJay.Text.Scratch;

[InterpolatedStringHandler]
public ref struct InterpolatedTextBuilder
{
    private TextBuilder _textBuilder;

    public InterpolatedTextBuilder(int literalLength, int formattedCount)
    {
        _textBuilder = new TextBuilder(literalLength + (formattedCount * 16));
    }

    public InterpolatedTextBuilder(TextBuilder textBuilder)
    {
        _textBuilder = textBuilder;
    }

    public void AppendLiteral(string? str) => _textBuilder.Append(str);
    public void AppendFormatted(char ch) => _textBuilder.Append(ch);
    public void AppendFormatted(text txt) => _textBuilder.Append(txt); 
    public void AppendFormatted(string? str) => _textBuilder.Append(str);
    public void AppendFormatted<T>(T value) => _textBuilder.Append<T>(value);
    public void AppendFormatted<T>(T value, text format) => _textBuilder.Format<T>(value, format);
    public void AppendFormatted<T>(T value, string? format) => _textBuilder.Format<T>(value, format);

    public text AsSpan() => _textBuilder.AsSpan();
    public void Dispose() => _textBuilder.Dispose();
    public string ToStringAndDispose() => _textBuilder.ToStringAndDispose();
    public override string ToString() => _textBuilder.ToString();
}