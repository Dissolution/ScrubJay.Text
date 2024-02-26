using ScrubJay.Collections;

namespace ScrubJay.Text.Building;

public sealed class NewLineAndIndentManager : IDisposable
{
    public static string DefaultNewLine { get; set; } = Environment.NewLine;
    public static string DefaultIndent { get; set; } = "    "; // 4 spaces

    private readonly Buffer<char> _textBuffer;
    private readonly Stack<int> _indentOffsets;

    public text CurrentNewLineAndIndent => _textBuffer.AsSpan();
    
    public NewLineAndIndentManager()
    {
        _textBuffer = new();
        _textBuffer.AddMany(Environment.NewLine.AsSpan());
        _indentOffsets = new();
    }
    
    public void AddIndent(char indent)
    {
        _indentOffsets.Push(_textBuffer.Count);
        _textBuffer.Add(indent);
    }

    public void AddIndent(string? indent) => AddIndent(indent.AsSpan());
    

    public void AddIndent(scoped text indent)
    {
        _indentOffsets.Push(_textBuffer.Count);
        _textBuffer.AddMany(indent);
    }

    public void RemoveIndent()
    {
        if (_indentOffsets.Count > 0)
        {
            _textBuffer.Count = _indentOffsets.Pop();
        }
    }

    public void RemoveIndent(out text lastIndent)
    {
        if (_indentOffsets.Count > 0)
        {
            var lastIndentIndex = _indentOffsets.Pop();
            lastIndent = _textBuffer[lastIndentIndex..];
            _textBuffer.Count = lastIndentIndex;
        }
        else
        {
            lastIndent = default;
        }
    }

    
    public void Dispose()
    {
        _textBuffer.Dispose();
    }
}