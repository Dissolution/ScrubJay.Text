namespace ScrubJay.Text.Extensions;

/// <summary>
/// Extensions on <see cref="char"/>
/// </summary>
public static class CharExtensions
{
    /// <summary>
    /// Converts this <see cref="char"/> into a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> containing it
    /// </summary>
    /// <param name="ch">
    /// The input character
    /// </param>
    /// <returns>
    /// A <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> containing the <see cref="char"/>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static text AsSpan(this in char ch)
    {
#if NET7_0_OR_GREATER
        return new text(in ch);
#else
        unsafe
        {
            fixed (char* pointer = &ch)
            {
                return new ReadOnlySpan<char>(pointer, 1);
            }
        }
#endif
    }
}