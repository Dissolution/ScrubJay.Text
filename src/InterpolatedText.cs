using ScrubJay.Text.Buffers;

namespace ScrubJay.Text;

[PublicAPI]
[InterpolatedStringHandler]
public ref struct InterpolatedText
{
    private const int DEFAULT_GROW = 64;
    
    private char[]? _charArray;
    private Span<char> _charSpan;
    private int _position;

    private Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charSpan.Slice(_position);
    }

    private int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _charSpan.Length;
    }

    public ref char this[Index index] => ref _charSpan.Slice(0, _position)[index];

    public Span<char> this[Range range] => _charSpan.Slice(0, _position)[range];
    
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    public InterpolatedText()
    {
        _charSpan = _charArray = TextPool.Rent();
        _position = 0;
    }
    
    public InterpolatedText(int literalLength, int formattedCount)
    {
        _charSpan = _charArray = TextPool.Rent(literalLength + (formattedCount * 16));
        _position = 0;
    }

    public InterpolatedText(Span<char> initialBuffer)
    {
        _charSpan = initialBuffer;
        _charArray = null;
        _position = 0;
    }
    
      
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowBy(int additionalChars)
    {
        char[] newArray = TextPool.Rent((_charSpan.Length + additionalChars) * 2);
        TextHelper.Unsafe.CopyBlock(_charSpan, newArray, _position);

        char[]? toReturn = _charArray;
        _charSpan = _charArray = newArray;

        TextPool.Return(toReturn);
    }
    
    
    public void AppendLiteral(char ch)
    {
        if (_position >= _charSpan.Length)
        {
            GrowBy(1);
        }

        Available[0] = ch;
        _position += 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string str)
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
        if (_position >= _charSpan.Length)
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
                    GrowBy(DEFAULT_GROW);
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
                    GrowBy(DEFAULT_GROW);
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
                    GrowBy(DEFAULT_GROW);
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

    public Span<char> AsSpan() => _charSpan.Slice(0, _position);

    public text AsText() => AsSpan();
    
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

    public override string ToString() => AsSpan().ToString();
}