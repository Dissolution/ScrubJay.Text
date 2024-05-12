namespace ScrubJay.Text.Scratch;

public delegate void TBA(TextBuilder buildText);
public delegate void TBVA<T>(TextBuilder buildText, T value);
public delegate void TBVIA<T>(TextBuilder buildText, T value, int index);

public class TextBuilder : IDisposable
{
    public static TextBuilder New => new();

    protected char[] _charArray;
    protected int _position;

    internal Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.AsSpan(_position);
    }

    internal int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.Length;
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    public TextBuilder()
    {
        _charArray = TextPool.Rent();
        _position = 0;
    }

    public TextBuilder(int minCapacity)
    {
        _charArray = TextPool.Rent(minCapacity);
        _position = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalChars)
    {
        int newCapacity = (_charArray.Length + additionalChars) * 2;
        newCapacity = newCapacity.Clamp(TextPool.MIN_CAPACITY, TextPool.MAX_CAPACITY);
        char[] newArray = TextPool.Rent(newCapacity);
        _charArray.AsSpan(0, _position).CopyTo(newArray);

        char[] toReturn = Interlocked.Exchange(ref _charArray, newArray);
        TextPool.Return(toReturn);
    }

    public TextBuilder Append(char ch)
    {
        int pos = _position;
        if (pos >= Capacity)
        {
            Grow(1);
        }

        _charArray[pos] = ch;
        _position = pos + 1;
        return this;
    }

    public TextBuilder Append(scoped text text)
    {
        int pos = _position;
        int len = text.Length;
        int newPos = pos + len;
        if (newPos >= Capacity)
        {
            Grow(len);
        }

        TextHelper.Unsafe.CopyBlock(text, Available, len);
        _position = newPos;
        return this;
    }

    public TextBuilder Append(string? str)
    {
        if (str is not null)
        {
            int pos = _position;
            int len = str.Length;
            int newPos = pos + len;
            if (newPos >= Capacity)
            {
                Grow(len);
            }

            TextHelper.Unsafe.CopyBlock(str, Available, len);
            _position = newPos;
        }
        return this;
    }

    public TextBuilder Append<T>(T? value)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, default))
                Grow(1);
            _position += charsWritten;
            return this;
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
        return Append(str);
    }

    public TextBuilder Append([InterpolatedStringHandlerArgument("")] ref InterpolatedTextBuilder interpolatedTextBuilder)
    {
        // As soon as we've gotten here, the interpolation has occurred
        return this;
    }
    

    public TextBuilder Format<T>(T? value, string? format, IFormatProvider? provider = null)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
                Grow(1);
            _position += charsWritten;
            return this;
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
        return Append(str);
    }

    public TextBuilder Format<T>(T? value, text format, IFormatProvider? provider = null)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
                Grow(1);
            _position += charsWritten;
            return this;
        }
#endif

        string? str;
        if (value is IFormattable)
        {
            str = ((IFormattable)value).ToString(format.AsString(), provider);
        }
        else
        {
            str = value?.ToString();
        }
        return Append(str);
    }


    public TextBuilder NewLine() => Append(Environment.NewLine);


    public TextBuilder Enumerate<T>(ReadOnlySpan<T> values, TBVA<T> buildValueText)
    {
        for (var i = 0; i < values.Length; i++)
        {
            buildValueText(this, values[i]);
        }
        return this;
    }
    public TextBuilder Enumerate<T>(IEnumerable<T> values, TBVA<T> buildValueText)
    {
        foreach (var value in values)
        {
            buildValueText(this, value);
        }
        return this;
    }
    public TextBuilder EnumerateAppend<T>(ReadOnlySpan<T> values) => Enumerate<T>(values, static (tb, value) => tb.Append<T>(value));
    public TextBuilder EnumerateAppend<T>(IEnumerable<T> values) => Enumerate<T>(values, static (tb, value) => tb.Append<T>(value));

    public TextBuilder Iterate<T>(ReadOnlySpan<T> values, TBVIA<T> buildValueIndexText)
    {
        for (var i = 0; i < values.Length; i++)
        {
            buildValueIndexText(this, values[i], i);
        }
        return this;
    }

    public TextBuilder Iterate<T>(IEnumerable<T> values, TBVIA<T> buildValueIndexText)
    {
        int index = 0;
        foreach (var value in values)
        {
            buildValueIndexText(this, value, index);
            index++;
        }
        return this;
    }

    public TextBuilder Delimit<T>(TBA delimit, ReadOnlySpan<T> values, TBVA<T> buildValueText)
    {
        int len = values.Length;
        if (len == 0) return this;
        buildValueText(this, values[0]);
        for (var i = 1; i < len; i++)
        {
            delimit(this);
            buildValueText(this, values[i]);
        }
        return this;
    }
    public TextBuilder Delimit<T>(char delimiter, ReadOnlySpan<T> values, TBVA<T> buildValueText)
        => Delimit<T>(tb => tb.Append(delimiter), values, buildValueText);
    public TextBuilder Delimit<T>(string? delimiter, ReadOnlySpan<T> values, TBVA<T> buildValueText)
        => Delimit<T>(tb => tb.Append(delimiter), values, buildValueText);

    public TextBuilder DelimitAppend<T>(TBA delimit, ReadOnlySpan<T> values)
        => Delimit<T>(delimit, values, static (tb, value) => tb.Append<T>(value));
    public TextBuilder DelimitAppend<T>(char delimiter, ReadOnlySpan<T> values)
        => Delimit<T>(tb => tb.Append(delimiter), values, static (tb, value) => tb.Append<T>(value));
    public TextBuilder DelimitAppend<T>(string? delimiter, ReadOnlySpan<T> values)
        => Delimit<T>(tb => tb.Append(delimiter), values, static (tb, value) => tb.Append<T>(value));


    public TextBuilder Delimit<T>(TBA delimit, IEnumerable<T> values, TBVA<T> buildValueText)
    {
        using var e = values.GetEnumerator();
        if (!e.MoveNext()) return this;
        buildValueText(this, e.Current);
        while (e.MoveNext())
        {
            delimit(this);
            buildValueText(this, e.Current);
        }
        return this;
    }
    public TextBuilder Delimit<T>(char delimiter, IEnumerable<T> values, TBVA<T> buildValueText)
        => Delimit<T>(tb => tb.Append(delimiter), values, buildValueText);
    public TextBuilder Delimit<T>(string? delimiter, IEnumerable<T> values, TBVA<T> buildValueText)
        => Delimit<T>(tb => tb.Append(delimiter), values, buildValueText);

    public TextBuilder DelimitAppend<T>(TBA delimit, IEnumerable<T> values)
        => Delimit<T>(delimit, values, static (tb, value) => tb.Append<T>(value));
    public TextBuilder DelimitAppend<T>(char delimiter, IEnumerable<T> values)
        => Delimit<T>(tb => tb.Append(delimiter), values, static (tb, value) => tb.Append<T>(value));
    public TextBuilder DelimitAppend<T>(string? delimiter, IEnumerable<T> values)
        => Delimit<T>(tb => tb.Append(delimiter), values, static (tb, value) => tb.Append<T>(value));



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public text AsText() => new text(_charArray, 0, _position);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AsSpan() => new Span<char>(_charArray, 0, _position);



    public void Dispose()
    {
        char[] toReturn = Interlocked.Exchange<char[]>(ref _charArray, Array.Empty<char>());
        TextPool.Return(toReturn);
    }

    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }

    public override string ToString()
    {
        return new string(_charArray, 0, _position);
    }
}