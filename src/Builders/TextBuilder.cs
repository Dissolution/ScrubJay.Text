namespace ScrubJay.Text.Builders;

/// <summary>
/// An instance of a <see cref="FluentTextBuilder{B}"/>
/// </summary>
[PublicAPI]
[MustDisposeResource]
public sealed class TextBuilder : FluentTextBuilder<TextBuilder>, IDisposable
{
    /// <summary>
    /// Construct a new, empty <see cref="TextBuilder"/>
    /// </summary>
    public TextBuilder() : base() { }

    /// <summary>
    /// Construct a new, empty <see cref="TextBuilder"/> with a minimum starting capacity
    /// </summary>
    /// <param name="minCapacity"></param>
    public TextBuilder(int minCapacity) : base(minCapacity)
    {

    }
}