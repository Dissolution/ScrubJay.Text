using Polyfills;

namespace ScrubJay.Text.Builders;

public sealed class UberTextBuilder : UberFluentTextBuilder<UberTextBuilder>
{
}

public abstract class UberFluentTextBuilder<B> : FluentTextBuilder<B>
    where B : UberFluentTextBuilder<B>, new()
{
    protected readonly WhitespaceManager _whitespace;

    public UberFluentTextBuilder() : base()
    {
        _whitespace = new();
    }

    public B Indented(string? indent, BuilderAction indentedAction) => Indented(indent.AsSpan(), indentedAction);
    
    public B Indented(text indent, BuilderAction indentedAction)
    {
        _whitespace.StartIndent(indent);
        indentedAction(_builder);
#if DEBUG
        var removed = _whitespace.TryEndIndent();
        Debug.Assert(removed);
        Debug.Assert(removed.IsOk(out var formerIndent));
        Debug.Assert(formerIndent == indent.ToString());
#else
        _whitespace.EndIndent();
#endif
        return _builder;
    }

    public override B NewLine()
    {
        _textBuffer.AddMany(_whitespace.NewLine);
        _textBuffer.AddMany(_whitespace.CurrentIndent);
        return _builder;
    }

    public override B Append(char ch)
    {
        if (_whitespace.NewLine.Length == 1 && _whitespace.NewLine[0] == ch)
        {
            _textBuffer.Add(ch);
            _textBuffer.AddMany(_whitespace.CurrentIndent);
            return _builder;
        }
        return base.Append(ch);
    }

    public override B Append(scoped text text)
    {
        if (text.Length == 0)
            return _builder;
        var e = text.Split(_whitespace.NewLine);
        if (!e.MoveNext())
            return _builder;
        
        // our loop
        while (true)
        {
            // add this slice
            var sliceRange = e.Current;
            var slice = text[sliceRange];
            _textBuffer.AddMany(slice);
            
            // if we have no more slices we're done
            if (!e.MoveNext())
                break;
            
            // add our newline + indents
            _textBuffer.AddMany(_whitespace.NewLine);
            _textBuffer.AddMany(_whitespace.CurrentIndent);
        }

        return _builder;
    }

    public override B Append<T>([AllowNull] T value)
    {
        // For certain Ts, we behave slightly differently
        if (value is BuilderAction builderAction)
        {
            // Capture our current position as an indent
            var i = _textBuffer.TryFindIndex(_whitespace.NewLine, firstToLast: false);
            if (i.IsSome(out var index))
            {
                var newIndent = _textBuffer.Written.Slice(index + _whitespace.NewLine.Length);
                if (newIndent.Length == 0)
                {
                    // No indent required
                    builderAction(_builder);
                }
                else
                {
                    // Capture this indent and then build
                    using (_whitespace.NewBlock())
                    using (_whitespace.NewIndent(newIndent))
                    {
                        builderAction(_builder);
                    }
                }
            }
            else
            {
                // No change in indent
                builderAction(_builder);
            }
            
            return _builder;
        }
        else if (value is string str)
        {
            _textBuffer.AddMany(str.AsSpan());
            return _builder;
        }
        
        return base.Append(value);
    }

    public override void Dispose()
    {
        _whitespace.Dispose();
        base.Dispose();
    }
}