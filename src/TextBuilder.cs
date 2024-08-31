namespace ScrubJay.Text;

[PublicAPI]
[MustDisposeResource]
public sealed class TextBuilder : FluentTextBuilder<TextBuilder>, IDisposable
{
    public TextBuilder() : base() { }
    public TextBuilder(int minCapacity) : base(minCapacity) { }
}