#pragma warning disable CA1715

using Polyfills;

namespace ScrubJay.Text.Builders;

public abstract class UberFluentTextBuilder<B> : FluentTextBuilder<B>
    where B : UberFluentTextBuilder<B>, new()
{
    protected readonly WhitespaceManager _whitespace;

    protected UberFluentTextBuilder() : base()
    {
        _whitespace = new();
    }

    protected UberFluentTextBuilder(int minCapacity) : base(minCapacity)
    {
        _whitespace = new();
    }

    public B Indented(string? indent, Action<B> indentedBuild) => Indented(indent.AsSpan(), indentedBuild);

    public B Indented(text indent, Action<B> indentedBuild)
    {
        using var _ = _whitespace.NewIndent(indent);
        indentedBuild(_builder);
        return _builder;
    }

    public override B NewLine()
    {
        _textBuffer.AddMany(_whitespace.NewLine.AsSpan());
        _textBuffer.AddMany(_whitespace.CurrentIndent);
        return _builder;
    }

    public override B Append(char ch)
    {
        if (_whitespace.HasOneCharNewLine.HasSome(out var nl) && nl == ch)
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
        if (_whitespace.NewLine.Length > 0)
        {
            var e = text.Split(_whitespace.NewLine.AsSpan());
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
                _textBuffer.AddMany(_whitespace.NewLine.AsSpan());
                _textBuffer.AddMany(_whitespace.CurrentIndent);
            }

            return _builder;
        }

        return base.Append(text);
    }

    public override B Append<T>([AllowNull] T value)
    {
        return base.Append(value);
    }

    internal override void InterpolatedExecute(Action<B> build)
    {
        // Capture our current position as an indent
        var i = _textBuffer.TryFindIndex(_whitespace.NewLine.AsSpan(), firstToLast: false);
        if (i.HasSome(out var index))
        {
            var newIndent = _textBuffer.Written.Slice(index + _whitespace.NewLine.Length);
            if (newIndent.Length == 0)
            {
                // No indent required
                build(_builder);
            }
            else
            {
                // Capture this indent and then build
                using (_whitespace.NewBlock())
                using (_whitespace.NewIndent(newIndent))
                {
                    build(_builder);
                }
            }
        }
        else
        {
            // No change in indent
            build(_builder);
        }
    }

    [HandlesResourceDisposal]
    public override void Dispose()
    {
        _whitespace.Dispose();
        base.Dispose();
    }
}