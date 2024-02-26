namespace ScrubJay.Text;

/// <summary>
/// Textual alignment
/// </summary>
/// <remarks>
/// This has <see cref="FlagsAttribute"/> so that:<br/>
/// <c>Left|Center</c> indicates a left-bias for centering<br/>
/// <c>Right|Center</c> indicates a right-bias for centering
/// </remarks>
[Flags]
public enum Alignment
{
    None = 0,
    
    /// <summary>
    /// Align text to the left (filler to the right)
    /// </summary>
    Left = 1 << 0,
    
    /// <summary>
    /// Align text to the right (filler to the left)
    /// </summary>
    Right = 1 << 1,
    
    /// <summary>
    /// Align text in the center (filler to the sides)
    /// </summary>
    Center = 1 << 2,
}