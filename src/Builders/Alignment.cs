namespace ScrubJay.Text.Builders;

[Flags]
public enum Alignment
{
    None = 0,
    Left = 1 << 0,
    Right = 1 << 1,
    Center = 1 << 2,
}