namespace ScrubJay.Text;

[PublicAPI]
[MustDisposeResource]
public abstract class FluentTextBuilder<B> : IDisposable
    where B : FluentTextBuilder<B>, new()
{
    public delegate void BuilderAction(B builder);

    public delegate void BuilderValueAction<in T>(B builder, T value);

    public delegate void BuilderValueIndexAction<in T>(B builder, T value, int index);


    public static B New => new();

    protected readonly B _builder;
    private readonly TextBuffer _textBuffer;

    protected FluentTextBuilder()
    {
        _textBuffer = new();
        _builder = (B)this;
    }

    protected FluentTextBuilder(int minCapacity)
    {
        _textBuffer = new(minCapacity);
        _builder = (B)this;
    }

    public virtual B Append(char ch)
    {
        _textBuffer.Write(ch);
        return _builder;
    }

    public virtual B Append(scoped text text)
    {
        _textBuffer.Write(text);
        return _builder;
    }

    public virtual B Append(string? str)
    {
        _textBuffer.Write(str);
        return _builder;
    }

    public virtual B Append<T>(T? value)
    {
        _textBuffer.Write<T>(value);
        return _builder;
    }

    public virtual B Append([InterpolatedStringHandlerArgument("")] ref InterpolatedTextBuilder<B> interpolatedTextBuilder)
    {
        // As soon as we've gotten here, the interpolation has occurred
        return _builder;
    }


    public virtual B Format<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        _textBuffer.WriteFormatted<T>(value, format, provider);
        return _builder;
    }

    public virtual B Format<T>(T? value, text format, IFormatProvider? provider = null)
    {
        _textBuffer.WriteFormatted<T>(value, format, provider);
        return _builder;
    }


    public virtual B NewLine()
    {
        _textBuffer.Write(Environment.NewLine.AsSpan());
        return _builder;
    }


    public B Enumerate<T>(ReadOnlySpan<T> values, BuilderValueAction<T> onBuilderValue)
    {
        foreach (var t in values)
        {
            onBuilderValue(_builder, t);
        }

        return _builder;
    }

    public B Enumerate<T>(IEnumerable<T>? values, BuilderValueAction<T> onBuilderValue)
    {
        if (values is null)
            return _builder;
                
        foreach (var value in values)
        {
            onBuilderValue(_builder, value);
        }

        return _builder;
    }

    public B EnumerateAppend<T>(ReadOnlySpan<T> values) => Enumerate<T>(values, static (tb, value) => tb.Append<T>(value));

    public B EnumerateAppend<T>(IEnumerable<T>? values) => Enumerate<T>(values, static (tb, value) => tb.Append<T>(value));


    public B Iterate<T>(ReadOnlySpan<T> values, BuilderValueIndexAction<T> onBuilderValueIndex)
    {
        for (var i = 0; i < values.Length; i++)
        {
            onBuilderValueIndex(_builder, values[i], i);
        }

        return _builder;
    }

    public B Iterate<T>(IEnumerable<T>? values, BuilderValueIndexAction<T> onBuilderValueIndex)
    {
        if (values is null)
            return _builder;
        
        int index = 0;
        foreach (var value in values)
        {
            onBuilderValueIndex(_builder, value, index);
            index++;
        }

        return _builder;
    }

#region Delimit

#region ReadOnlySpan

    public B Delimit<T>(char delimiter, ReadOnlySpan<T> values, BuilderValueAction<T> onBuilderValue)
    {
        int len = values.Length;
        if (len == 0)
            return _builder;
        onBuilderValue(_builder, values[0]);
        for (var i = 1; i < len; i++)
        {
            _textBuffer.Write(delimiter);
            onBuilderValue(_builder, values[i]);
        }

        return _builder;
    }


    public B Delimit<T>(string? delimiter, ReadOnlySpan<T> values, BuilderValueAction<T> onBuilderValue)
    {
        if (delimiter is null || delimiter.Length == 0)
            return Enumerate(values, onBuilderValue);
        // if (delimiter == Environment.NewLine)
        //     return Delimit(static b => b.NewLine(), values, onBuilderValue);
        int len = values.Length;
        if (len == 0)
            return _builder;
        onBuilderValue(_builder, values[0]);
        for (var i = 1; i < len; i++)
        {
            _textBuffer.Write(delimiter);
            onBuilderValue(_builder, values[i]);
        }

        return _builder;
    }

    public B Delimit<T>(scoped text delimiter, ReadOnlySpan<T> values, BuilderValueAction<T> onBuilderValue)
    {
        if (delimiter.Length == 0)
            return Enumerate(values, onBuilderValue);
        // if (delimiter == Environment.NewLine)
        //     return Delimit(static b => b.NewLine(), values, onBuilderValue);
        int len = values.Length;
        if (len == 0)
            return _builder;
        onBuilderValue(_builder, values[0]);
        for (var i = 1; i < len; i++)
        {
            _textBuffer.Write(delimiter);
            onBuilderValue(_builder, values[i]);
        }

        return _builder;
    }

    public B Delimit<T>(BuilderAction? onDelimit, ReadOnlySpan<T> values, BuilderValueAction<T> onBuilderValue)
    {
        if (onDelimit is null)
            return Enumerate<T>(values, onBuilderValue);
        int len = values.Length;
        if (len == 0)
            return _builder;
        onBuilderValue(_builder, values[0]);
        for (var i = 1; i < len; i++)
        {
            onDelimit(_builder);
            onBuilderValue(_builder, values[i]);
        }

        return _builder;
    }

    public B DelimitAppend<T>(char delimiter, ReadOnlySpan<T> values) => Delimit<T>(delimiter, values, static (tb, value) => tb.Append<T>(value));
    public B DelimitAppend<T>(string? delimiter, ReadOnlySpan<T> values) => Delimit<T>(delimiter, values, static (tb, value) => tb.Append<T>(value));
    public B DelimitAppend<T>(scoped text delimiter, ReadOnlySpan<T> values) => Delimit<T>(delimiter, values, static (tb, value) => tb.Append<T>(value));
    public B DelimitAppend<T>(BuilderAction? onDelimit, ReadOnlySpan<T> values) => Delimit<T>(onDelimit, values, static (tb, value) => tb.Append<T>(value));

#endregion

#region IEnumerable

    public B Delimit<T>(char delimiter, IEnumerable<T>? values, BuilderValueAction<T> onBuilderValue)
    {
        if (values is null)
            return _builder;
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return _builder;
        onBuilderValue(_builder, e.Current);
        while (e.MoveNext())
        {
            _textBuffer.Write(delimiter);
            onBuilderValue(_builder, e.Current);
        }

        return _builder;
    }


    public B Delimit<T>(string? delimiter, IEnumerable<T>? values, BuilderValueAction<T> onBuilderValue)
    {
        if (delimiter is null || delimiter.Length == 0)
            return Enumerate<T>(values, onBuilderValue);
        // if (delimiter == Environment.NewLine)
        //     return Delimit(static b => b.NewLine(), values, onBuilderValue);
        if (values is null)
            return _builder;
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return _builder;
        onBuilderValue(_builder, e.Current);
        while (e.MoveNext())
        {
            _textBuffer.Write(delimiter);
            onBuilderValue(_builder, e.Current);
        }

        return _builder;
    }

    public B Delimit<T>(scoped text delimiter, IEnumerable<T>? values, BuilderValueAction<T> onBuilderValue)
    {
        if (delimiter.Length == 0)
            return Enumerate<T>(values, onBuilderValue);
        // if (delimiter == Environment.NewLine)
        //     return Delimit(static b => b.NewLine(), values, onBuilderValue);
        if (values is null)
            return _builder;
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return _builder;
        onBuilderValue(_builder, e.Current);
        while (e.MoveNext())
        {
            _textBuffer.Write(delimiter);
            onBuilderValue(_builder, e.Current);
        }

        return _builder;
    }

    public B Delimit<T>(BuilderAction? onDelimit, IEnumerable<T>? values, BuilderValueAction<T> onBuilderValue)
    {
        if (values is null)
            return _builder;
        if (onDelimit is null)
            return Enumerate<T>(values, onBuilderValue);
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return _builder;
        onBuilderValue(_builder, e.Current);
        while (e.MoveNext())
        {
            onDelimit(_builder);
            onBuilderValue(_builder, e.Current);
        }

        return _builder;
    }

    public B DelimitAppend<T>(char delimiter, IEnumerable<T> values) => Delimit<T>(delimiter, values, static (tb, value) => tb.Append<T>(value));
    public B DelimitAppend<T>(string? delimiter, IEnumerable<T> values) => Delimit<T>(delimiter, values, static (tb, value) => tb.Append<T>(value));
    public B DelimitAppend<T>(scoped text delimiter, IEnumerable<T> values) => Delimit<T>(delimiter, values, static (tb, value) => tb.Append<T>(value));
    public B DelimitAppend<T>(BuilderAction? onDelimit, IEnumerable<T> values) => Delimit<T>(onDelimit, values, static (tb, value) => tb.Append<T>(value));

#endregion

#endregion


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public text AsText() => _textBuffer.AsText();

    public void Dispose()
    {
        _textBuffer.Dispose();
    }

    public string ToStringAndDispose()
    {
       return _textBuffer.ToStringAndDispose();
    }

    public override string ToString()
    {
        return _textBuffer.ToString();
    }
}