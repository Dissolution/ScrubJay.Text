namespace ScrubJay.Text.Extensions;

public static class CharArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref char GetPinnableReference(this char[] charArray)
    {
#if NET6_0_OR_GREATER
        return ref MemoryMarshal.GetArrayDataReference(charArray);
#else
        return ref charArray[0];
#endif
    }
}