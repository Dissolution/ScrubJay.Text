using System.Buffers;

namespace ScrubJay.Text.Collections;

public static class TextPool
{
    // We want to reserve large chunks
    internal const int MIN_CAPACITY = 1_024;
    
    /* Needs to be:
     * <=  Array.MaxLength (2_147_483_591)
     * <= string.MaxLength (1_073_741_791) (string.Length = char.count)
     */
    internal const int MAX_CAPACITY = 1_073_741_791;
    
    // Use the Shared pool
    private static readonly ArrayPool<char> _charArrayPool = ArrayPool<char>.Shared;

    
    public static char[] Rent() => _charArrayPool.Rent(MIN_CAPACITY);
    
    public static char[] Rent(int minCapacity)
    {
        if (minCapacity > MAX_CAPACITY)
            throw new ArgumentOutOfRangeException(nameof(minCapacity), minCapacity, $"The maximum possible capacity is {MAX_CAPACITY}");
        if (minCapacity < MIN_CAPACITY)
            minCapacity = MIN_CAPACITY;
        return _charArrayPool.Rent(minCapacity);
    }
    
    public static void Return(char[]? chars)
    {
        if (chars is not null && chars.Length > 0)
        {
            _charArrayPool.Return(chars, true);
        }
    }

}