using ScrubJay.Text.Builders;
using ScrubJay.Utilities;

namespace ScrubJay.Text.Buffers;

[PublicAPI]
[MustDisposeResource]
public sealed class SpacingManager
{
    public const string DefaultIndent = "    "; // 4 spaces
    public static readonly string DefaultNewLine = Environment.NewLine;

    private string _newLine = DefaultNewLine;
    private readonly List<List<string>> _indents = [];

    private List<string> CurrentBlock
    {
        get
        {
            if (_indents.Count == 0)
            {
                List<string> block = [];
                _indents.Add(block);
                return block;
            }
            return _indents[^1];
        }
    }

    /// <summary>
    /// Gets or sets the current <see cref="string"/> that represents starting a new line
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    [AllowNull, NotNull]
    public string NewLine
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _newLine;
        set => _newLine = value ?? "";
    }

    public SpacingManager()
    {

    }

    internal B WriteIndented<B>(FluentTextBuilder<B> builder, text text)
        where B : FluentTextBuilder<B>
    {
        var currentBlock = CurrentBlock;
        if (currentBlock.Count == 0)
            return builder.Append(text);

        var e = text.Split(NewLine.AsSpan());
        foreach (Range range in e)
        {
            builder.Append(text[range]).Append(_newLine).EnumerateAppend(currentBlock);
        }

        return (B)builder;
    }

    public void StartIndent(string? indent)
    {
        CurrentBlock.Add(indent ?? "");
    }

    public void EndIndent()
    {
        int indentCount = CurrentBlock.Count;
        if (indentCount == 0)
            throw new InvalidOperationException();
        CurrentBlock.RemoveAt(indentCount - 1);
    }

    public IDisposable Indent(string? indent)
    {
        StartIndent(indent);
        return Disposable.Action(EndIndent);
    }

    public void StartBlock()
    {
        _indents.Add([]);
    }

    public void EndBlock()
    {
        int blockCount = _indents.Count;
        if (blockCount == 0)
            throw new InvalidOperationException();
        _indents.RemoveAt(blockCount - 1);
    }

    public IDisposable Block()
    {
        StartBlock();
        return Disposable.Action(EndBlock);
    }
}