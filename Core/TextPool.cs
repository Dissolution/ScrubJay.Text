namespace ScrubJay.Text;

using System.Buffers;

public static class TextPool
{
    private const int MIN_CAPACITY = 1024;
    private const int MAX_CAPACITY = 1073741791; // string.MaxLength < Array.MaxLength (0X7FFFFFC7)
    
    private static readonly ArrayPool<char> _charArrayPool = ArrayPool<char>.Shared;

    static TextPool()
    {
        
    }
    
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