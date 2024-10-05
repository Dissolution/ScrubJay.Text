using ScrubJay.Utilities;
using ScrubJay.Validation;

namespace ScrubJay.Text;

[PublicAPI]
public sealed class WhitespaceManager : IDisposable
{
    public const string DefaultIndent = "    "; // 4 spaces
    public static readonly string DefaultNewLine = Environment.NewLine;

    private string _newLine = DefaultNewLine;
    private readonly Buffer<char> _indents = new();
    private readonly Stack<int> _indentOffsets = new();
    private readonly Stack<int> _blockOffsets = new();

    public text NewLine
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _newLine.AsSpan();
        set
        {
            if (value.Length == 0)
                throw new ArgumentException($"NewLine cannot be empty", nameof(value));
            _newLine = value.ToString();
        }
    }
    
    public text CurrentIndent
    {
        get
        {
            if (!_blockOffsets.TryPeek(out var lastBlockStart))
                return _indents.Written;
            return _indents.Written.Slice(lastBlockStart);
        }
    }

    public Result<Unit, Exception> TryStartIndent(scoped text indent)
    {
        var v = Validate.IsNotEmpty(indent);
        if (!v)
            return v;
        
        int position = _indents.Count;
        _indentOffsets.Push(position);
        _indents.AddMany(indent);
        return Unit();
    }

    public void StartIndent(scoped text indent) => TryStartIndent(indent).OkOrThrow();
    
    public Result<string, Exception> TryEndIndent()
    {
        if (_indentOffsets.Count == 0)
            return new InvalidOperationException("There are no indents to remove");
        
        var lastIndentStart = _indentOffsets.Peek(); // will succeed
        Debug.Assert(lastIndentStart >= 0 && lastIndentStart <= _indents.Count);
        
        if (_blockOffsets.TryPeek(out var lastBlockStart))
        {
            if (lastIndentStart < lastBlockStart)
                return new InvalidOperationException("Remaining indents belong to previous block");
        }
        
        // Remove the indent
        _indentOffsets.Pop(); // will succeed
        // capture that indent to return
        string str = _indents.Written.Slice(lastIndentStart).ToString();
        // set the indents length lower to remove the indent
        _indents.Count = lastIndentStart;
        return str;
    }

    public void EndIndent() => TryEndIndent().OkOrThrow();
    
    public IDisposable NewIndent(scoped text indent)
    {
        TryStartIndent(indent).OkOrThrow();
        return Disposable.Action(() => TryEndIndent().OkOrThrow());
    }
    
    public void StartBlock()
    {
        int position = _indents.Count;
        _blockOffsets.Push(position);
    }

    public void EndBlock()
    {
        int blockStart = _blockOffsets.Pop();

        while (_indentOffsets.TryPeek(out int offset))
        {
            if (offset < blockStart)
                break;

            _indentOffsets.Pop();
            _indents.Count = offset;
        }
    }
    
    public IDisposable NewBlock()
    {
        StartBlock();
        return Disposable.Action(() => EndBlock());
    }
   
    public void Dispose()
    {
        _indentOffsets.Clear();
        _indents.Dispose();
    }
}