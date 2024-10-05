namespace ScrubJay.Text.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Gets a pinned <c>ref readonly</c> to this <see cref="string"/>
    /// </summary>
#if NET48_OR_GREATER || NETSTANDARD2_0 || NETSTANDARD2_1
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


