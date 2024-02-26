namespace ScrubJay.Text.Scratch;

public sealed class TextFormatterPool
{
    public TextFormatterOptions DefaultOptions { get; set; } = new();

    public ValueFormatters DefaultFormatters { get; set; } = new();
}