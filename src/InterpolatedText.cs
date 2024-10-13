#pragma warning disable S3247, CA2213

using ScrubJay.Collections;

namespace ScrubJay.Text;

[PublicAPI]
public delegate void AppendFormatted<in T>(ref InterpolatedText text, T value, text format = default);

[PublicAPI]
[InterpolatedStringHandler]
public ref struct InterpolatedText
{
    private static readonly ConcurrentTypeMap<Delegate> _formatters = [];

    public static void AddFormatter<T>(AppendFormatted<T> formatter)
    {
        _formatters.AddOrUpdate<T>(formatter);
    }


    private SpanBuffer<char> _buffer;

    public ref char this[Index index] => ref _buffer[index];

    public Span<char> this[Range range] => _buffer[range];

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _buffer.Count;
    }

    public InterpolatedText()
    {
        _buffer = new SpanBuffer<char>();
    }

    public InterpolatedText(int literalLength, int formattedCount)
    {
        _buffer = new SpanBuffer<char>(literalLength + (formattedCount * 16));
    }

    public InterpolatedText(Span<char> initialBuffer)
    {
        _buffer = new SpanBuffer<char>(initialBuffer, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(char ch) => _buffer.Add(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string str) => _buffer.Add(str);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(char ch) => _buffer.Add(ch);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(scoped text text) => _buffer.Add(text);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(string? str) => _buffer.Add(str);

    public void AppendFormatted<T>(T value) => AppendFormatted<T>(value, default(text));

    public void AppendFormatted<T>(T value, text format)
    {
        if (_formatters.TryGetValue<T>(out var del) && del.Is<AppendFormatted<T>>(out var appendFormatted))
        {
            appendFormatted(ref this, value, format);
            return;
        }

        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(_buffer.Available, out charsWritten, format, default))
                {
                    _buffer.Grow();
                }

                _buffer.Count += charsWritten;
                return;
            }
#endif

            str = ((IFormattable)value).ToString(format.AsString(), default);
        }
        else
        {
            str = value?.ToString();
        }

        _buffer.AddMany(str.AsSpan());
    }

    public void AppendFormatted<T>(T value, string? format)
    {
        if (_formatters.TryGetValue<T>(out var del) && del.Is<AppendFormatted<T>>(out var appendFormatted))
        {
            appendFormatted(ref this, value, format.AsSpan());
            return;
        }

        string? str;
        if (value is IFormattable)
        {
#if NET6_0_OR_GREATER
            if (value is ISpanFormattable)
            {
                int charsWritten;
                while (!((ISpanFormattable)value).TryFormat(_buffer.Available, out charsWritten, format, default))
                {
                    _buffer.Grow();
                }

                _buffer.Count += charsWritten;
                return;
            }
#endif

            str = ((IFormattable)value).ToString(format, default);
        }
        else
        {
            str = value?.ToString();
        }

        _buffer.Add(str);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AsSpan() => _buffer.AsSpan();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public text AsText() => _buffer.AsSpan();

    [HandlesResourceDisposal]
    public void Dispose() => _buffer.Dispose();

    [HandlesResourceDisposal]
    public string ToStringAndDispose()
    {
        string result = this.ToString();
        this.Dispose();
        return result;
    }

    public override string ToString()
    {
#if NETSTANDARD2_1 || NET6_0_OR_GREATER
        return new string(AsSpan());
#else
        return AsSpan().ToString();
#endif
    }
}