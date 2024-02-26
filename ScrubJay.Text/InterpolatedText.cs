using System.Diagnostics;
using ScrubJay.Text.Collections;

namespace ScrubJay.Text;

[InterpolatedStringHandler]
public ref struct InterpolatedText
{
    private char[]? _charArray;
    private Span<char> _chars;
    private int _position;

    private Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Slice(0, _position);
    }

    private Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Slice(_position);
    }

    public InterpolatedText(int literalLength, int formattedCount)
    {
        _chars = _charArray = TextPool.RentForInterpolation(literalLength, formattedCount);
        _position = 0;
    }

    // ReSharper disable UnusedParameter.Local
    public InterpolatedText(int literalLength, int formattedCount, Span<char> initialBuffer) 
    // ReSharper restore UnusedParameter.Local
    {
        _chars = initialBuffer;
        _charArray = null;
        _position = 0;
    }

#region Grow

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopyChar(char ch)
    {
        GrowBy(1);
        _chars[_position] = ch;
        _position += 1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopyString(string value)
    {
        GrowBy(value.Length);
        TextHelper.CopyTo(value, Available);
        _position += value.Length;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopySpan(scoped text value)
    {
        GrowBy(value.Length);
        TextHelper.CopyTo(value, Available);
        _position += value.Length;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowBy(int additionalChars)
    {
        Debug.Assert(additionalChars > _chars.Length - _position);
        char[] newArray = TextPool.RentGrowBy(_chars.Length, additionalChars);
        TextHelper.CopyTo(Written, newArray);
        char[]? toReturn = _charArray;
        _chars = _charArray = newArray;
        TextPool.Return(toReturn);
    }

#endregion


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string value)
    {
        if (TextHelper.TryCopyTo(value, Available))
        {
            _position += value.Length;
        }
        else
        {
            GrowThenCopyString(value);
        }
    }

    public void AppendFormatted(char ch)
    {
        if (Available.TrySetItem(0, ch))
        {
            _position += 1;
        }
        else
        {
            GrowThenCopyChar(ch);
        }
    }

    public void AppendFormatted(scoped text value)
    {
        if (TextHelper.TryCopyTo(value, Available))
        {
            _position += value.Length;
        }
        else
        {
            GrowThenCopySpan(value);
        }
    }

    public void AppendFormatted(string? value)
    {
        if (value is not null)
        {
            if (TextHelper.TryCopyTo(value, Available))
            {
                _position += value.Length;
            }
            else
            {
                GrowThenCopyString(value);
            }
        }
    }


    public void AppendFormatted<T>(T value)
    {
        string? str;
        // ReSharper disable once MergeCastWithTypeCheck
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so.
#if NET6_0_OR_GREATER
            // ReSharper disable once MergeCastWithTypeCheck
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, default, default))
                {
                    GrowBy(1);
                }

                _position += charsWritten;
                return;
            }
#endif
            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(default, default);
        }
        else
        {
            str = value?.ToString();
        }

        AppendFormatted(str);
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        string? str;
        // ReSharper disable once MergeCastWithTypeCheck
        if (value is IFormattable)
        {
            // If the value can format itself directly into our buffer, do so.
#if NET6_0_OR_GREATER
            // ReSharper disable once MergeCastWithTypeCheck
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(Available, out charsWritten, format, default))
                {
                    GrowBy(1);
                }

                _position += charsWritten;
                return;
            }
#endif

            // constrained call avoiding boxing for value types
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

    public void Dispose()
    {
        char[]? toReturn = _charArray;
        this = default;
        TextPool.Return(toReturn);
    }

    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }

    public override string ToString() => Written.ToString();
}