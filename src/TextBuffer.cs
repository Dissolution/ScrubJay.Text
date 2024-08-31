// ReSharper disable MergeCastWithTypeCheck
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression

using System.Diagnostics;

namespace ScrubJay.Text;

public delegate int UseAvailable(Span<char> available);

[PublicAPI]
public sealed class TextBuffer : IDisposable
{
    private char[] _charArray;
    private int _position;

    private Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.AsSpan(_position);
    }

    /// <summary>
    /// Gets the current capacity for this <see cref="TextBuffer"/> to store characters
    /// </summary>
    /// <remarks>
    /// This will be increased as necessary during Write operations
    /// </remarks>
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charArray.Length;
    }

    /// <summary>
    /// Gets the total number of characters in this <see cref="TextBuffer"/>
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    public ref char this[int index] => ref AsSpan()[index];
    public ref char this[Index index] => ref AsSpan()[index];
    public Span<char> this[Range range] => AsSpan()[range];

    /// <summary>
    /// Creates a new <see cref="TextBuffer"/> with default starting <see cref="Capacity"/>
    /// </summary>
    public TextBuffer()
    {
        _charArray = TextPool.Rent();
        _position = 0;
    }

    /// <summary>
    /// Creates a new <see cref="TextBuffer"/> with specified starting <see cref="Capacity"/>
    /// </summary>
    /// <param name="minCapacity">
    /// The minimum starting <see cref="Capacity"/> for the <see cref="TextBuffer"/>
    /// </param>
    public TextBuffer(int minCapacity)
    {
        _charArray = TextPool.Rent(minCapacity);
        _position = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow()
    {
        char[] newArray = TextPool.Rent(_charArray.Length * 2);
        TextHelper.Unsafe.CopyBlock(_charArray, newArray, _position);

        char[] toReturn = Interlocked.Exchange(ref _charArray, newArray);
        TextPool.Return(toReturn);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowBy(int additionalChars)
    {
        Debug.Assert(additionalChars > 0);
        int capacity = Capacity;
        int newCapacity = Math.Max(capacity * 2, capacity + additionalChars);

        Debug.Assert(capacity + additionalChars <= newCapacity);
        
        char[] newArray = TextPool.Rent(newCapacity);
        TextHelper.Unsafe.CopyBlock(_charArray, newArray, _position);

        char[] toReturn = Interlocked.Exchange(ref _charArray, newArray);
        TextPool.Return(toReturn);
    }


    public void Write(char ch)
    {
        int pos = _position;
        if (pos >= Capacity)
        {
            GrowBy(1);
        }

        _charArray[pos] = ch;
        _position = pos + 1;
    }

    public void Write(scoped text text)
    {
        int pos = _position;
        int len = text.Length;
        int newPos = pos + len;
        if (newPos >= Capacity)
        {
            GrowBy(len);
        }

        TextHelper.Unsafe.CopyBlock(text, Available, len);
        _position = newPos;
    }

    public void Write(string? str)
    {
        if (str is not null)
        {
            Write(str.AsSpan());
        }
    }

    // ReSharper disable once UnusedParameter.Global
    public void Write([InterpolatedStringHandlerArgument("")] ref InterpolatedTextBuffer interpolatedTextBuffer)
    {
        // As soon as we've gotten here, the interpolation has occurred
    }

    public void Write<T>(T? value)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, default))
            {
                Grow();
            }

            _position += charsWritten;
            return;
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
            Write(str.AsSpan());
        }
    }

    public void WriteFormatted<T>(T? value, string? format, IFormatProvider? provider = null)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
            {
                Grow();
            }

            _position += charsWritten;
            return;
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
            Write(str.AsSpan());
        }
    }


    public void WriteFormatted<T>(T? value, text format, IFormatProvider? provider = null)
    {
#if NET6_0_OR_GREATER
        if (value is ISpanFormattable)
        {
            int charsWritten;
            while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, provider))
            {
                Grow();
            }

            _position += charsWritten;
            return;
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

        if (str is not null)
        {
            Write(str.AsSpan());
        }
    }

    public void UseAvailable(UseAvailable onAvailable)
    {
        int used = onAvailable(Available);
        if (used > 0)
        {
            _position += used;
        }
    }

    public void Reserve(int count, out Span<char> buffer, char? fillChar = null)
    {
        if (count <= 0)
        {
            buffer = [];
            return;
        }

        int pos = _position;
        int newPos = pos + count;
        if (newPos >= Capacity)
        {
            GrowBy(count);
        }

        buffer = _charArray.AsSpan(pos, count);
        if (fillChar.TryGetValue(out var ch))
        {
            buffer.Fill(ch);
        }

        _position = newPos;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public text AsText() => new text(_charArray, 0, _position);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AsSpan() => new Span<char>(_charArray, 0, _position);

    public char[] ToArray()
    {
        int len = _position;
        char[] array = new char[len];
        TextHelper.Unsafe.CopyBlock(_charArray, array, len);
        return array;
    }

    [HandlesResourceDisposal]
    public void Dispose()
    {
        char[] toReturn = Interlocked.Exchange<char[]>(ref _charArray, []);
        TextPool.Return(toReturn);
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
        return new string(_charArray, 0, _position);
    }
}