namespace ScrubJay.Text.Extensions;

[PublicAPI]
public static class StringExtensions
{
#if NETFRAMEWORK || NETSTANDARD
    /// <summary>
    /// Gets a pinned <c>ref readonly</c> to this <see cref="string"/>
    /// </summary>
    public static ref readonly char GetPinnableReference(this string str)
    {
        unsafe
        {
            fixed (char* strPtr = str)
            {
                return ref Unsafe.AsRef<char>(strPtr);
            }
        }
    }
#endif
}