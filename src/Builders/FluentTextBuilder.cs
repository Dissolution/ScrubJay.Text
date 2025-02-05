using ScrubJay.Fluent;

#pragma warning disable S3247, CA1715, S4136, RCS1220, CA1033, CA1710

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
public abstract class FluentTextBuilder<B> : FluentBuilder<B>,
    IList<char>,
    IReadOnlyList<char>,
    ICollection<char>,
    IReadOnlyCollection<char>,
    IEnumerable<char>,
    IDisposable
    where B : FluentTextBuilder<B>
{
    // This manages all the actual writing
    protected readonly PooledList<char> _text;

    int ICollection<char>.Count => Length;
    bool ICollection<char>.IsReadOnly => false;
    int IReadOnlyCollection<char>.Count => Length;

    char IReadOnlyList<char>.this[int index] => this[index];

    char IList<char>.this[int index]
    {
        get => this[index];
        set => this[index] = value;
    }

    public ref char this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _text[index];
    }

    public ref char this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _text[index];
    }

    public Span<char> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _text[range];
    }

    /// <summary>
    /// Gets the total length of text written to this Builder
    /// </summary>
    public int Length => _text.Count;

    protected FluentTextBuilder() : base()
    {
        _text = new();
    }

    protected FluentTextBuilder(int minCapacity) : base()
    {
        _text = new(minCapacity);
    }

    /// <summary>
    /// Append a <see cref="char">character</see>
    /// </summary>
    /// <param name="ch">The <see cref="char"/> to append</param>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Append(char ch)
    {
        _text.Add(ch);
        return _builder;
    }
    void ICollection<char>.Add(char item) => Append(item);

    /// <summary>
    /// Append a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see>
    /// </summary>
    /// <param name="text">The <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> to append</param>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Append(scoped text text)
    {
        _text.AddMany(text);
        return _builder;
    }

    /// <summary>
    /// Append a <see cref="string"/>
    /// </summary>
    /// <param name="str">The <see cref="string"/> to append</param>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public B Append(string? str) => Append(str.AsSpan());

    /// <summary>
    /// Appends an <see cref="InterpolatedTextBuilder{B}"/>
    /// </summary>
    /// <param name="interpolatedTextBuilder"></param>
    /// <returns></returns>
    public virtual B Append([InterpolatedStringHandlerArgument("")] ref InterpolatedTextBuilder<B> interpolatedTextBuilder)
    {
        // As soon as we've gotten here, the interpolation has occurred
        return _builder;
    }

    protected internal virtual void InterpolatedExecute(Action<B> build)
    {
        build(_builder);
    }

    /// <summary>
    /// Append a <typeparamref name="T"/> <paramref name="value"/> with no formatting
    /// </summary>
    /// <param name="value">The value to append the textual representation of</param>
    /// <typeparam name="T">The <see cref="Type"/> of value to append</typeparam>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Append<T>(T? value)
    {
        if (value is null)
        {
            return _builder;
        }
        
        if (value is IWriteable)
        {
            ((IWriteable)value).WriteTo(_builder);
            return _builder;
        }
        
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(_text.Available, out charsWritten, default, default))
            {
                _text.Grow();
            }

            _text.Count += charsWritten;
            return _builder;
        }
#endif
        
        text str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(default, default).AsSpan();
        }
        else
        {
            str = value.ToString().AsSpan();
        }
        
        _text.AddMany(str);
        return _builder;
    }

    /// <summary>
    /// Append a <typeparamref name="T"/> <paramref name="value"/> with a format
    /// </summary>
    /// <param name="value">The value to append the textual representation of</param>
    /// <param name="format">The <see cref="string"/> format</param>
    /// <param name="provider">The optional <see cref="IFormatProvider"/></param>
    /// <typeparam name="T">The <see cref="Type"/> of value to format and append</typeparam>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Format<T>(T? value, string? format, IFormatProvider? provider = null)
    {
        if (value is null)
        {
            return _builder;
        }
        
        if (value is IWriteable)
        {
            ((IWriteable)value).WriteTo(_builder);
            return _builder;
        }
        
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(_text.Available, out charsWritten, format, provider))
            {
                _text.Grow();
            }

            _text.Count += charsWritten;
            return _builder;
        }
#endif
        
        text str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format, provider).AsSpan();
        }
        else
        {
            str = value.ToString().AsSpan();
        }
        
        _text.AddMany(str);
        return _builder;
    }

    /// <summary>
    /// Append a <typeparamref name="T"/> <paramref name="value"/> with a format
    /// </summary>
    /// <param name="value">The value to append the textual representation of</param>
    /// <param name="format">The <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> format</param>
    /// <param name="provider">The optional <see cref="IFormatProvider"/></param>
    /// <typeparam name="T">The <see cref="Type"/> of value to format and append</typeparam>
    /// <returns>
    /// This builder instance after operation has completed
    /// </returns>
    public virtual B Format<T>(T? value, scoped text format, IFormatProvider? provider = null)
    {
        if (value is null)
        {
            return _builder;
        }
        
        if (value is IWriteable)
        {
            ((IWriteable)value).WriteTo(_builder);
            return _builder;
        }
        
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(_text.Available, out charsWritten, format, provider))
            {
                _text.Grow();
            }

            _text.Count += charsWritten;
            return _builder;
        }
#endif
        
        text str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format.AsString(), provider).AsSpan();
        }
        else
        {
            str = value.ToString().AsSpan();
        }
        
        _text.AddMany(str);
        return _builder;
    }

    /// <summary>
    /// Appends a <see cref="Environment.NewLine"/>
    /// </summary>
    /// <returns></returns>
    public virtual B NewLine() => Append(Environment.NewLine);

    public B AppendLine(char ch) => Append(ch).NewLine();
    public B AppendLine(text text) => Append(text).NewLine();
    public B AppendLine(string? str) => Append(str).NewLine();
    public B AppendLine<T>(T? value) => Append(value).NewLine();

    #region Enumeration

    public B Enumerate<T>(ReadOnlySpan<T> values, Action<B, T> onBuilderValue)
    {
        foreach (var t in values)
        {
            onBuilderValue(_builder, t);
        }

        return _builder;
    }

    public B Enumerate<T>(IEnumerable<T> values, Action<B, T> onBuilderValue)
    {
        foreach (var value in values)
        {
            onBuilderValue(_builder, value);
        }

        return _builder;
    }

    public B EnumerateAppend<T>(ReadOnlySpan<T> values) 
        => Enumerate(values, static (tb, value) => tb.Append(value));

    public B EnumerateAppend<T>(IEnumerable<T> values) 
        => Enumerate(values, static (tb, value) => tb.Append(value));


    public B Iterate<T>(ReadOnlySpan<T> values, Action<B, T, int> onBuilderValueIndex)
    {
        for (var i = 0; i < values.Length; i++)
        {
            onBuilderValueIndex(_builder, values[i], i);
        }

        return _builder;
    }

    public B Iterate<T>(IEnumerable<T> values, Action<B, T, int> onBuilderValueIndex)
    {
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

    public B Delimit<T>(char delimiter, ReadOnlySpan<T> values, Action<B, T> onBuilderValue)
    {
        int len = values.Length;
        if (len == 0)
            return _builder;
        onBuilderValue(_builder, values[0]);
        for (var i = 1; i < len; i++)
        {
            Append(delimiter);
            onBuilderValue(_builder, values[i]);
        }

        return _builder;
    }


    public B Delimit<T>(string delimiter, ReadOnlySpan<T> values, Action<B, T> onBuilderValue) => Delimit(delimiter.AsSpan(), values, onBuilderValue);

    public B Delimit<T>(scoped text delimiter, ReadOnlySpan<T> values, Action<B, T> onBuilderValue)
    {
        if (delimiter.Length == 0)
            return Enumerate(values, onBuilderValue);
        int len = values.Length;
        if (len == 0)
            return _builder;
        onBuilderValue(_builder, values[0]);
        for (var i = 1; i < len; i++)
        {
            Append(delimiter);
            onBuilderValue(_builder, values[i]);
        }

        return _builder;
    }

    public B Delimit<T>(Action<B> onDelimit, ReadOnlySpan<T> values, Action<B, T> onBuilderValue)
    {
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

    public B DelimitAppend<T>(char delimiter, ReadOnlySpan<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(string delimiter, ReadOnlySpan<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(scoped text delimiter, ReadOnlySpan<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(Action<B> onDelimit, ReadOnlySpan<T> values) => Delimit(onDelimit, values, static (tb, value) => tb.Append(value));

    #endregion

    #region IEnumerable

    public B Delimit<T>(char delimiter, IEnumerable<T> values, Action<B, T> onBuilderValue)
    {
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return _builder;
        onBuilderValue(_builder, e.Current);
        while (e.MoveNext())
        {
            _text.Add(delimiter);
            onBuilderValue(_builder, e.Current);
        }

        return _builder;
    }


    public B Delimit<T>(string delimiter, IEnumerable<T> values, Action<B, T> onBuilderValue)
        => Delimit(delimiter.AsSpan(), values, onBuilderValue);

    public B Delimit<T>(scoped text delimiter, IEnumerable<T> values, Action<B, T> onBuilderValue)
    {
        if (delimiter.Length == 0)
            return Enumerate(values, onBuilderValue);
        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return _builder;
        onBuilderValue(_builder, e.Current);
        while (e.MoveNext())
        {
            _text.AddMany(delimiter);
            onBuilderValue(_builder, e.Current);
        }

        return _builder;
    }

    public B Delimit<T>(Action<B> onDelimit, IEnumerable<T> values, Action<B, T> onBuilderValue)
    {
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

    public B DelimitAppend<T>(char delimiter, IEnumerable<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(string delimiter, IEnumerable<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(scoped text delimiter, IEnumerable<T> values) => Delimit(delimiter, values, static (tb, value) => tb.Append(value));
    public B DelimitAppend<T>(Action<B> onDelimit, IEnumerable<T> values) => Delimit(onDelimit, values, static (tb, value) => tb.Append(value));

    #endregion

    #endregion

    #endregion

    #region Insertion

    public Result<int, Exception> TryInsert(Index index, char ch)
        => _text.TryInsert(index, ch);

    public Result<int, Exception> TryInsert(Index index, scoped text text)
        => _text.TryInsertMany(index, text);

    public Result<int, Exception> TryInsert(Index index, string? str)
        => _text.TryInsertMany(index, str.AsSpan());

    void IList<char>.Insert(int index, char item)
    {
        _text.TryInsert(index, item).ThrowIfError();
    }

    #endregion


    public Option<int> TryFindIndex(char ch, bool firstToLast = true, Index? offset = default, IEqualityComparer<char>? charComparer = null)
        => _text.TryFindIndex(ch, firstToLast, offset, charComparer);

    public Option<int> TryFindIndex(
        scoped text text,
        bool firstToLast = true,
        Index? offset = default,
        IEqualityComparer<char>? charComparer = null)
        => _text.TryFindIndex(text, firstToLast, offset, charComparer);

    bool ICollection<char>.Contains(char item) => _text.Contains(item);

    int IList<char>.IndexOf(char item) => _text.TryFindIndex(item).SomeOr(-1);


    public bool TryRemoveAt(Index index)
        => _text.TryRemoveAt(index);

    public bool TryRemoveMany(Range range)
        => _text.TryRemoveMany(range);

    bool ICollection<char>.Remove(char item)
    {
        if (TryFindIndex(item).HasSome(out var index))
        {
            return _text.TryRemoveAt(index);
        }
        return false;
    }

    void IList<char>.RemoveAt(int index)
    {
        if (!_text.TryRemoveAt(index))
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public B RemoveLast(int count)
    {
        _text.Count -= count;
        return _builder;
    }


    public B Clear()
    {
        _text.Clear();
        return _builder;
    }

    void ICollection<char>.Clear() => Clear();

    public bool TryCopyTo(Span<char> span) => _text.TryCopyTo(span);

    void ICollection<char>.CopyTo(char[] array, int arrayIndex)
    {
        Validate.CanCopyTo(array, arrayIndex, _text.Count).ThrowIfError();
        _ = _text.TryCopyTo(array.AsSpan(arrayIndex));
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public text AsText() => _text.Written;

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_text).GetEnumerator();
    IEnumerator<char> IEnumerable<char>.GetEnumerator() => ((IEnumerable<char>)_text).GetEnumerator();

    public Span<char>.Enumerator GetEnumerator() => _text.GetEnumerator();


    [HandlesResourceDisposal]
    public virtual void Dispose()
    {
        _text.Dispose();
    }

    [HandlesResourceDisposal]
    public string ToStringAndDispose()
    {
        string str = ToString();
        Dispose();
        return str;
    }

    public override string ToString()
    {
        return _text.ToString();
    }
}