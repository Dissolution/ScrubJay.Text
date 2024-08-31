namespace ScrubJay.Text;

[PublicAPI]
[InterpolatedStringHandler]
public ref struct InterpolatedTextBuffer
{
    private TextBuffer _textBuffer;
    
    public InterpolatedTextBuffer(TextBuffer textBuffer)
    {
        _textBuffer = textBuffer;
    }

    public void AppendLiteral(string? str) => _textBuffer.Write(str);
    public void AppendFormatted(char ch) => _textBuffer.Write(ch);
    public void AppendFormatted(text txt) => _textBuffer.Write(txt); 
    public void AppendFormatted(string? str) => _textBuffer.Write(str);
    public void AppendFormatted<T>(T value) => _textBuffer.Write<T>(value);
    public void AppendFormatted<T>(T value, text format) => _textBuffer.WriteFormatted<T>(value, format);
    public void AppendFormatted<T>(T value, string? format) => _textBuffer.WriteFormatted<T>(value, format);
    
    public void Dispose() => _textBuffer.Dispose();
    public string ToStringAndDispose() => _textBuffer.ToStringAndDispose();
    public override string ToString() => _textBuffer.ToString();
}