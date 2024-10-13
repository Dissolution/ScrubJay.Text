namespace ScrubJay.Text.Builders;

/// <summary>
/// An InterpolatedStringHandler that writes to a <see cref="FluentTextBuilder{B}"/>
/// </summary>
/// <typeparam name="B"></typeparam>
[PublicAPI]
[MustDisposeResource]
[InterpolatedStringHandler]
public ref struct InterpolatedTextBuilder<B>
    where B : FluentTextBuilder<B>
{
    private readonly B _builder;
    
    /// <summary>
    /// Construct a new <see cref="InterpolatedTextBuilder{B}"/> that writes to a <typeparamref name="B"/> <paramref name="builder"/>
    /// </summary>
    public InterpolatedTextBuilder(int literalLength, int formattedCount, B builder)
    {
        _builder = builder;
    }
    
    public void AppendLiteral(string str) => _builder.Append(str);
    public void AppendFormatted(char ch) => _builder.Append(ch);
    public void AppendFormatted(text txt) => _builder.Append(txt); 
    public void AppendFormatted(string? str) => _builder.Append(str);
    public void AppendFormatted<T>(T value) => _builder.Append<T>(value);
    public void AppendFormatted<T>(T value, scoped text format) => _builder.Format<T>(value, format);
    public void AppendFormatted<T>(T value, string? format) => _builder.Format<T>(value, format);
    
    [HandlesResourceDisposal]
    public void Dispose() => _builder.Dispose();
    [HandlesResourceDisposal]
    public string ToStringAndDispose() => _builder.ToStringAndDispose();
    public override string ToString() => _builder.ToString();
}