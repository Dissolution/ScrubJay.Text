using ScrubJay.Collections;

namespace ScrubJay.Text.Scratch;

public sealed class IndentManager : IDisposable
{
    private readonly TextFormatterOptions _options;
    private readonly Buffer<char> _indents;
    private readonly Stack<(int Start, int End)> _indentRanges;

    public bool HasIndent => Indent.Length > 0;

    private (int Start, int End) IndentRange
    {
        get
        {
            _indentRanges.TryPeek(out var range);
            return range;
        }
    }
    
    public text Indent
    {
        get
        {
            var range = this.IndentRange;
            return _indents[range.Start..range.End];
        }
    }

    internal IndentManager(TextFormatter formatter)
    {
        _options = formatter.Options;
        _indents = new Buffer<char>(1024);
        _indentRanges = new(0);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ProcessSpan(text input, Buffer<char> buffer)
    {
        var indent = this.Indent;
        var newLine = _options.NewLine.AsSpan();

        int s = 0;
        while (s < input.Length)
        {
            var i = input.NextIndexOf(newLine, s, _options.StringComparison);
            if (i == -1)
            {
                // All of this has no newline
                buffer.AddMany(input.Slice(s));
                return;
            }

            // All of this up to newline
            buffer.AddMany(input[s..i]);
            // newline
            buffer.AddMany(newLine);
            // indent
            buffer.AddMany(indent);

            // start scanning after the newline
            s = i + newLine.Length;
        }
    }

    public void AddIndent(text indent, bool addToExisting = true)
    {
        if (indent.Length > 0)
        {
            (int Start, int End) newRange;
            if (addToExisting)
            {
                _indents.AddMany(indent);
                var range = IndentRange;
                newRange = new(range.Start, _indents.Count);
                
            }
            else
            {
                int newStart = _indents.Count;
                _indents.AddMany(indent);
                newRange = new(newStart, _indents.Count);
            }
            _indentRanges.Push(newRange);
        }
    }

    public void RemoveIndent()
    {
        if (!_indentRanges.TryPop(out var range))
        {
            _indents.Count = 0;
        }
        else
        {
            _indents.Count = range.Start;
        }
    }

    public void RemoveIndent(out text lastIndent)
    {
        if (!_indentRanges.TryPop(out var range))
        {
            lastIndent = default;
            _indents.Count = 0;
        }
        else
        {
            lastIndent = _indents[range.Start..range.End];
            _indents.Count = range.Start;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IndentAwareWrite(in char input, Buffer<char> buffer)
    {
        if (HasIndent || _options.NewLine.Length > 1)
        {
            buffer.Add(input);
        }
        else
        {
            ProcessSpan(input.AsSpan(), buffer);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IndentAwareWrite(scoped text input, Buffer<char> buffer)
    {
        if (HasIndent || _options.NewLine.Length > input.Length)
        {
            buffer.AddMany(input);
        }
        else
        {
            ProcessSpan(input, buffer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteNewLine(Buffer<char> buffer)
    {
        buffer.AddMany(_options.NewLine.AsSpan());
        buffer.AddMany(_indents.AsSpan());
    }
    
    public void Dispose()
    {
        _indentRanges.Clear();
        _indents.Dispose();
    }
}