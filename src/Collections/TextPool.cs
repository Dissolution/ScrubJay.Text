using System.Buffers;

namespace ScrubJay.Text.Collections;

/// <summary>
/// A TextPool is a wrapper around <see cref="ArrayPool{T}">ArrayPool&lt;char&gt;.</see><see cref="ArrayPool{T}.Shared"/>
/// that provides more methods for working specifically with pools of characters
/// </summary>
public static class TextPool
{
    /// <summary>
    /// The minimum possible capacity for any <c>char[]</c> rented from <see cref="TextPool"/>
    /// </summary>
    public const int MIN_CAPACITY = 1024;
    
    /// <summary>
    /// The maximum possible capacity for any <c>char[]</c> rented from <see cref="TextPool"/>
    /// </summary>
    /// <remarks>
    /// This is <c>== string.MaxLength</c>, which is <c>&lt; Array.MaxLength (2147483591)</c>
    /// </remarks>
    public const int MAX_CAPACITY = 1073741791;

    // Reference the standard shared pool
    private static readonly ArrayPool<char> _charArrayPool = ArrayPool<char>.Shared;

    /// <summary>
    /// Rents a new <c>char[]</c> from this <see cref="TextPool"/>
    /// </summary>
    /// <returns>
    /// An empty <c>char[]</c> with a capacity of at least <see cref="MIN_CAPACITY"/>
    /// </returns>
    public static char[] Rent() => _charArrayPool.Rent(MIN_CAPACITY);
    
    /// <summary>
    /// Rents a new <c>char[]</c> from this <see cref="TextPool"/> with at least the given minimum capacity
    /// </summary>
    /// <param name="minCapacity">
    /// The minimum allowed size of array to return
    /// </param>
    /// <returns>
    /// An empty <c>char[]</c> with a capacity of at least <paramref name="minCapacity"/>
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <see cref="minCapacity"/> is greater than <see cref="MAX_CAPACITY"/>
    /// </exception>
    public static char[] Rent(int minCapacity)
    {
        return minCapacity switch
        {
            < MIN_CAPACITY => _charArrayPool.Rent(MIN_CAPACITY),
            > MAX_CAPACITY => _charArrayPool.Rent(MAX_CAPACITY),
            _ => _charArrayPool.Rent(minCapacity),
        };
    }
    
    /// <summary>
    /// Returns a <c>char[]?</c> to this <see cref="TextPool"/>
    /// </summary>
    /// <param name="chars">
    /// The <c>char[]?</c> array to return
    /// </param>
    public static void Return(char[]? chars)
    {
        // Do nothing with null or empty arrays
        if (chars is not null && chars.Length > 0)
        {
            _charArrayPool.Return(chars, true);
        }
    }
}