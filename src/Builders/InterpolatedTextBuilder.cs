#pragma warning disable CA1715, IDE0250, IDE0251, MA0102, IDE0060, RCS1163

namespace ScrubJay.Text.Builders;

/// <summary>
/// An InterpolatedStringHandler that writes to a <see cref="FluentTextBuilder{B}"/>
/// </summary>
/// <typeparam name="B"></typeparam>
[PublicAPI]
[InterpolatedStringHandler]
public ref struct InterpolatedTextBuilder<B>
    where B : FluentTextBuilder<B>, new()
{
    private readonly B _builder;

    /// <summary>
    /// Construct a new <see cref="InterpolatedTextBuilder{B}"/> that writes to a <typeparamref name="B"/> <paramref name="builder"/>
    /// </summary>
    /// <param name="literalLength"></param>
    /// <param name="formattedCount"></param>
    /// <param name="builder"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InterpolatedTextBuilder(int literalLength, int formattedCount, B builder)
    {
        _builder = builder;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(string str) => _builder.Append(str);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(char ch) => _builder.Append(ch);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(text txt) => _builder.Append(txt);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(string? str) => _builder.Append(str);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value) => _builder.Append(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, scoped text format) => _builder.Format(value, format);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted<T>(T value, string? format) => _builder.Format(value, format);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(Action<B> build)
    {
        // pass-through to builder for execution
        _builder.InterpolatedExecute(build);
    }

    public override string ToString() => _builder.ToString();
}