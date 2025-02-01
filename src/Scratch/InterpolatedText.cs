using ScrubJay.Text.Utilities;

namespace ScrubJay.Text.Scratch;

[InterpolatedStringHandler]
public ref struct InterpolatedText
{
    private char[]? _charArray;
    private Span<char> _chars;
    private int _position;

    private Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Slice(_position);
    }

    private int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Length;
    }

    public InterpolatedText(int literalLength, int formattedCount)
    {
        _chars = _charArray = TextPool.Rent(literalLength + (formattedCount * 16));
        _position = 0;
    }

    public InterpolatedText(Span<char> initialBuffer)
    {
        _chars = initialBuffer;
        _charArray = null;
        _position = 0;
    }
    
      
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowBy(int additionalChars)
    {
        int newCapacity = ((_chars.Length + additionalChars) * 2).Clamp(TextPool.MIN_CAPACITY, TextPool.MAX_CAPACITY);
        char[] newArray = TextPool.Rent(newCapacity);
        TextHelper.Unsafe.CopyBlock(_chars, newArray, _position);

        char[]? toReturn = _charArray;
        _chars = _charArray = newArray;

        TextPool.Return(toReturn);
    }
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral([DisallowNull, NotNull] string str)
    {
        int pos = _position;
        int len = str.Length;
        int newPos = pos + len;
        if (newPos >= Capacity)
            GrowBy(len);

        TextHelper.Unsafe.CopyBlock(str, Available, len);
        _position = newPos;
    }
    
    public void AppendFormatted(char ch)
    {
        if (_position >= _chars.Length)
        {
            GrowBy(1);
        }

        Available[0] = ch;
        _position += 1;
    }
    
    public void AppendFormatted(text txt)
    {
        int pos = _position;
        int len = txt.Length;
        int newPos = pos + len;
        if (newPos >= Capacity)
            GrowBy(len);

        TextHelper.Unsafe.CopyBlock(txt, Available, len);
        _position = newPos;
    }
    
    public void AppendFormatted(string? str)
    {
        if (str is not null)
        {
            AppendLiteral(str);
        }
    }
    
    public void AppendFormatted<T>(T value)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, default))
                {
                    GrowBy(64);
                }

                _position += charsWritten;
                return;
            }
#endif

            str = ((IFormattable)value).ToString(default, default);
        }
        else
        {
            str = value?.ToString();
        }

        if (str is not null)
        {
            AppendLiteral(str);
        }
    }

    public void AppendFormatted<T>(T value, text format)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, default))
                {
                    GrowBy(64);
                }

                _position += charsWritten;
                return;
            }
#endif

            str = ((IFormattable)value).ToString(format.AsString(), default);
        }
        else
        {
            str = value?.ToString();
        }

        if (str is not null)
        {
            AppendLiteral(str);
        }
    }
    
    public void AppendFormatted<T>(T value, string? format)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, default))
                {
                    GrowBy(64);
                }

                _position += charsWritten;
                return;
            }
#endif

            str = ((IFormattable)value).ToString(format, default);
        }
        else
        {
            str = value?.ToString();
        }

        if (str is not null)
        {
            AppendLiteral(str);
        }
    }

    public text AsSpan() => _chars.Slice(0, _position);

    public void Dispose()
    {
        char[]? toReturn = _charArray;
        this = default; // defensive clear
        TextPool.Return(toReturn);
    }

    public string ToStringAndDispose()
    {
        string result = this.ToString();
        this.Dispose();
        return result;
    }

    public override string ToString() => AsSpan().AsString();
}