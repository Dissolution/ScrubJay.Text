using ScrubJay.Fluent;

#pragma warning disable S3247, CA1715, S4136

// ReSharper disable MergeCastWithTypeCheck
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
namespace ScrubJay.Text.Builders;

/// <summary>
/// A FluentTextBuilder uses fluent operations to build up complex <see cref="string">strings</see>
/// </summary>
/// <typeparam name="B">
/// The <see cref="Type"/> of <see cref="FluentTextBuilder{B}"/> that will be returned from all fluent operations
/// </typeparam>
[PublicAPI]
[MustDisposeResource]
public abstract class FluentTextBuilder<B> : FluentBuilder<B>, IDisposable
    where B : FluentTextBuilder<B>, new()
{
    /// <summary>
    /// An <see cref="Action{B}"/> that applies to a <see cref="FluentTextBuilder{B}"/> <paramref name="builder"/>
    /// </summary>
    public delegate void BuilderAction(B builder);

    /// <summary>
    /// An <see cref="Action{B,T}"/> that applies to a <see cref="FluentTextBuilder{B}"/> <paramref name="builder"/> and a <typeparamref name="T"/> <paramref name="value"/>
    /// </summary>
    public delegate void BuilderValueAction<in T>(B builder, T value);

    /// <summary>
    /// An <see cref="Action{B,T,I}"/> that applies to a <see cref="FluentTextBuilder{B}"/> <paramref name="builder"/>, a <typeparamref name="T"/> <paramref name="value"/>, and an <c>int</c> <paramref name="index"/>
    /// </summary>
    public delegate void BuilderValueIndexAction<in T>(B builder, T value, int index);
    
    protected readonly Buffer<char> _textBuffer;

    protected FluentTextBuilder() : base()
    {
        _textBuffer = new();
    }

    protected FluentTextBuilder(int minCapacity) : base()
    {
        _textBuffer = new(minCapacity);
    }

    /// <summary>
    /// Append a new <see cref="char"/> to this <typeparamref name="B"/>
    /// </summary>
    /// <param name="ch">The <see cref="char"/> to append</param>
    /// <returns>
    /// This <typeparamref name="B"/> builder instance
    /// </returns>
    public virtual B Append(char ch)
    {
        _textBuffer.Add(ch);
        return _builder;
    }

    public virtual B Append(scoped text text)
    {
        _textBuffer.AddMany(text);
        return _builder;
    }

    public virtual B Append(string? str) => Append(str.AsSpan());

    public virtual B Append<T>(T? value)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(_textBuffer.Available, out charsWritten, default, default))
            {
                _textBuffer.Grow();
            }

            _textBuffer.Count += charsWritten;
            return _builder;
        }
#endif

        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(default, default);
        }
        else
        {
            str = value?.ToString();
        }

        if (str is not null)
        {
            _textBuffer.AddMany(str.AsSpan());
        }
        return _builder;
    }

    public virtual B Append([InterpolatedStringHandlerArgument("")] ref InterpolatedTextBuilder<B> interpolatedTextBuilder)
    {
        // As soon as we've gotten here, the interpolation has occurred
        return _builder;
    }


    public virtual B Format<T>(T? value, string? format, IFormatProvider? provider = null)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(_textBuffer.Available, out charsWritten, format, provider))
            {
                _textBuffer.Grow();
            }

            _textBuffer.Count += charsWritten;
            return _builder;
        }
#endif

        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }

        if (str is not null)
        {
            _textBuffer.AddMany(str.AsSpan());
        }
        return _builder;
    }

    public virtual B Format<T>(T? value, scoped text format, IFormatProvider? provider = null)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(_textBuffer.Available, out charsWritten, format, provider))
            {
                _textBuffer.Grow();
            }

            _textBuffer.Count += charsWritten;
            return _builder;
        }
#endif

        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        if (str is not null)
        {
            _textBuffer.AddMany(str.AsSpan());
        }
        return _builder;
    }


    public virtual B NewLine() => Append(Environment.NewLine);


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
            _textBuffer.Add(delimiter);
            onBuilderValue(_builder, values[i]);
        }

        return _builder;
    }


    public B Delimit<T>(string? delimiter, ReadOnlySpan<T> values, BuilderValueAction<T> onBuilderValue)
    {
        text del = delimiter.AsSpan();
        
        if (del.Length == 0)
            return Enumerate(values, onBuilderValue);
        int len = values.Length;
        if (len == 0)
            return _builder;
        onBuilderValue(_builder, values[0]);
        for (var i = 1; i < len; i++)
        {
            _textBuffer.AddMany(del);
            onBuilderValue(_builder, values[i]);
        }

        return _builder;
    }

    public B Delimit<T>(scoped text delimiter, ReadOnlySpan<T> values, BuilderValueAction<T> onBuilderValue)
    {
        if (delimiter.Length == 0)
            return Enumerate(values, onBuilderValue);
        int len = values.Length;
        if (len == 0)
            return _builder;
        onBuilderValue(_builder, values[0]);
        for (var i = 1; i < len; i++)
        {
            _textBuffer.AddMany(delimiter);
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
            _textBuffer.Add(delimiter);
            onBuilderValue(_builder, e.Current);
        }

        return _builder;
    }


    public B Delimit<T>(string? delimiter, IEnumerable<T>? values, BuilderValueAction<T> onBuilderValue)
    {
        text del = delimiter.AsSpan();
        if (del.Length == 0)
            return Enumerate<T>(values, onBuilderValue);
        if (values is null)
            return _builder;
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return _builder;
        onBuilderValue(_builder, e.Current);
        while (e.MoveNext())
        {
            _textBuffer.AddMany(del);
            onBuilderValue(_builder, e.Current);
        }

        return _builder;
    }

    public B Delimit<T>(scoped text delimiter, IEnumerable<T>? values, BuilderValueAction<T> onBuilderValue)
    {
        if (delimiter.Length == 0)
            return Enumerate<T>(values, onBuilderValue);
        if (values is null)
            return _builder;
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return _builder;
        onBuilderValue(_builder, e.Current);
        while (e.MoveNext())
        {
            _textBuffer.AddMany(delimiter);
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
    public text AsText() => _textBuffer.Written;

    [HandlesResourceDisposal]
    public virtual void Dispose()
    {
        _textBuffer.Dispose();
    }

    [HandlesResourceDisposal]
    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }

    public override string ToString()
    {
        return _textBuffer.ToString();
    }
}