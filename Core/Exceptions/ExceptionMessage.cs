// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MergeCastWithTypeCheck
// ReSharper disable UnusedParameter.Local

namespace ScrubJay.Text.Exceptions;

using System.Buffers;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Extensions;
using ScrubJay.Text;

/// <summary>
/// A custom minimized implementation of an Interpolated String Handler
/// </summary>
/// <remarks>
/// Heavily based upon <c>DefaultInterpolatedStringHandler</c>
/// </remarks>
[InterpolatedStringHandler]
public ref struct InterpolatedText
{
    /// <summary>
    /// The <see cref="Array">char[]</see> rented from <see cref="TextPool"/> that will be returned when this is <see cref="Dispose">disposed</see>
    /// </summary>
    private char[]? _charArray;

    /// <summary>
    /// The <see cref="Span{T}">Span&lt;char&gt;</see> being written to
    /// </summary>
    private Span<char> _chars;

    /// <summary>
    /// The next position in <see cref="_chars"/> to write at
    /// </summary>
    private int _position;

    /// <summary>
    /// Gets a <see cref="Span{T}">Span&lt;char&gt;</see> of all written characters
    /// </summary>
    internal Span<char> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Slice(0, _position);
    }

    /// <summary>
    /// Gets a <see cref="Span{T}">Span&lt;char&gt;</see> of available characters that can be written to<br/>
    /// </summary>
    internal Span<char> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Slice(_position);
    }

    /// <summary>
    /// Gets the total number of characters written
    /// </summary>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _position = value < 0 ? 0 : value > Capacity ? Capacity : value;
    }

    /// <summary>
    /// Gets the current capacity to store characters<br/>
    /// <i>Note:</i> Will be increased as necessary
    /// </summary>
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chars.Length;
    }

    /// <summary>
    /// Construct a new <see cref="InterpolatedText"/>
    /// </summary>
    public InterpolatedText()
    {
        _chars = _charArray = TextPool.Rent();
        _position = 0;
    }

    /// <summary>
    /// Construct a new <see cref="InterpolatedText"/> that starts with an <paramref name="initialBuffer"/>
    /// </summary>
    /// <param name="initialBuffer">The initial <see cref="Span{T}">Span&lt;char&gt;</see> to write to</param>
    /// <remarks>
    /// If writing would exceed <paramref name="initialBuffer"/>'s length,
    /// a new buffer will be rented from <see cref="TextPool"/> and that will be used.
    /// No indication of such will be provided, but checking if the output of <see cref="ToString"/> is larger than <paramref name="initialBuffer"/>
    /// should indicate if a resize was performed.
    /// </remarks>
    public InterpolatedText(Span<char> initialBuffer)
    {
        _chars = initialBuffer;
        _charArray = null;
        _position = 0;
    }

    /// <summary>
    /// <see cref="InterpolatedStringHandlerAttribute"/> support
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public InterpolatedText(int literalLength, int formattedCount)
    {
        _chars = _charArray = TextPool.Rent(literalLength + (formattedCount * 16));
        _position = 0;
    }

    /// <summary>
    /// <see cref="InterpolatedStringHandlerAttribute"/> support
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public InterpolatedText(int literalLength, int formattedCount, Span<char> initialBuffer)
    {
        _chars = initialBuffer;
        _charArray = null;
        _position = 0;
    }

#region Grow
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBy(int growCount)
    {
        char[] newArray = TextPool.Rent((Capacity + growCount)*2);
        Written.CopyTo(newArray);

        char[]? toReturn = _charArray;
        _chars = _charArray = newArray;
        TextPool.Return(toReturn);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(char ch)
    {
        int index = _position;
        GrowBy(1);
        TextHelper.Unsafe.CopyBlock(
            in ch,
            ref Available.GetPinnableReference(),
            1);
        _position = index + 1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(scoped ReadOnlySpan<char> text)
    {
        int index = _position;
        int len = text.Length;
        GrowBy(len);
        TextHelper.Unsafe.CopyBlock(
            in text.GetPinnableReference(),
            ref Available.GetPinnableReference(),
            len);
        _position = index + len;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowThenCopy(string text)
    {
        int index = _position;
        int len = text.Length;
        GrowBy(len);
        TextHelper.Unsafe.CopyBlock(
            in text.GetPinnableReference(),
            ref Available.GetPinnableReference(),
            len);
        _position = index + len;
    }
#endregion

#region Interpolated String Handler implementations
    /// <summary>Writes the specified string to the handler.</summary>
    /// <param name="text">The string to write.</param>
    [EditorBrowsable(EditorBrowsableState.Never),MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral([DisallowNull, NotNull] string text)
    {
        int pos = _position;
        var chars = _chars;
        int textLen = text.Length;
        if (pos + textLen <= chars.Length)
        {
            TextHelper.Unsafe.CopyBlock(text, chars[pos..], textLen);
            _position = pos + textLen;
        }
        else
        {
            GrowThenCopy(text);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(char ch) => Write(ch);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(params char[]? chars) => Write(chars);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(scoped ReadOnlySpan<char> text) => Write(text);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted(string? str) => Write(str);
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T? value) => Write<T>(value);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void AppendFormatted<T>(T value, string? format) => Write<T>(value, format);
#endregion

#region Write
    public void Write(char ch)
    {
        int pos = _position;
        Span<char> chars = _chars;
        if (pos < chars.Length)
        {
            chars[pos] = ch;
            _position = pos + 1;
        }
        else
        {
            GrowThenCopy(ch);
        }
    }

    public void Write(scoped ReadOnlySpan<char> text)
    {
        int pos = _position;
        Span<char> chars = _chars;
        int textLen = text.Length;
        if (pos + textLen <= chars.Length)
        {
            TextHelper.Unsafe.CopyBlock(text, chars[pos..], textLen);
            _position = pos + textLen;
        }
        else
        {
            GrowThenCopy(text);
        }
    }

    public void Write(params char[]? characters)
    {
        if (characters is not null)
        {
            int pos = _position;
            var chars = _chars;
            int textLen = characters.Length;
            if (pos + textLen <= chars.Length)
            {
                TextHelper.Unsafe.CopyBlock(characters, chars[pos..], textLen);
                _position = pos + textLen;
            }
            else
            {
                GrowThenCopy(characters.AsSpan());
            }
        }
    }

    public void Write(string? str)
    {
        if (str is not null)
        {
            int pos = _position;
            var chars = _chars;
            int textLen = str.Length;
            if (pos + textLen <= chars.Length)
            {
                TextHelper.Unsafe.CopyBlock(str, chars[pos..], textLen);
                _position = pos + textLen;
            }
            else
            {
                GrowThenCopy(str);
            }
        }
    }

    public void Write<T>(T? value)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(
                    _chars.Slice(_position),
                    out charsWritten, default, default))
                {
                    GrowBy(16); // hole guess
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

        Write(str);
    }

    public void Write<T>(T? value, scoped ReadOnlySpan<char> format, IFormatProvider? provider = default)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(
                    _chars.Slice(_position),
                    out charsWritten, format, provider))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return;
            }
#endif

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format.ToString(), provider);
        }
        else
        {
            str = value?.ToString();
        }

        Write(str);
    }

    public void Write<T>(T? value, string? format, IFormatProvider? provider = default)
    {
        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            // If the value can format itself directly into our buffer, do so.
            if (value is ISpanFormattable)
            {
                int charsWritten;
                // constrained call avoiding boxing for value types
                while (!((ISpanFormattable)value).TryFormat(
                    _chars.Slice(_position),
                    out charsWritten, format, provider))
                {
                    GrowBy(16);
                }

                _position += charsWritten;
                return;
            }
#endif

            // constrained call avoiding boxing for value types
            str = ((IFormattable)value).ToString(format, provider);
        }
        else
        {
            str = value?.ToString();
        }

        Write(str);
    }
#endregion


    /// <summary>
    /// Returns any rented <see cref="Array">char[]</see> back to the <see cref="TextPool"/>
    /// </summary>
    public void Dispose()
    {
        char[]? toReturn = _charArray;
        this = default; // defensive clear
        TextPool.Return(toReturn);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => throw new NotSupportedException();

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => throw new NotSupportedException();

    public string ToStringAndDispose()
    {
        string result = ToString();
        Dispose();
        return result;
    }

    public override string ToString() => this.Written.ToString();
}