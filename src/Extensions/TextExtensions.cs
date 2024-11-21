namespace ScrubJay.Text.Extensions;

public static class TextExtensions
{
    /// <summary>
    /// Gets this <see cref="text"/> as a <see cref="string"/>
    /// </summary>
    public static string AsString(this text text)
    {
#if NET481_OR_GREATER || NETSTANDARD2_0
        unsafe
        {
            fixed (char* ptr = text)
            {
                return new string(ptr, 0, text.Length);
            }
        }
#else
        return new string(text);
#endif
    }
}