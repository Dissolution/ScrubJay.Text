using System.Buffers;

namespace ScrubJay.Text.Buffers;

/// <summary>
/// Manages renting and returning <c>char[]</c> to <see cref="ArrayPool{T}">ArrayPool&lt;char&gt;</see>
/// </summary>
[PublicAPI]
public static class TextPool
{
    private static readonly ArrayPool<char> _charArrayPool = ArrayPool<char>.Shared;
    
    /// <summary>
    /// The minimum capacity of an array returned from <see cref="Rent()"/>
    /// </summary>
    public const int MinCapacity = 1024;

    /// <summary>
    /// The maximum capacity of an array returned from <see cref="Rent()"/>
    /// </summary>
    public const int MaxCapacity = 0x3FFFFFDF; // == string.MaxLength
    
    /// <summary>
    /// Rents a <c>char[]</c> with at least a <see cref="Array.Length"/> of <see cref="MinCapacity"/>
    /// from the <see cref="char"/> <see cref="ArrayPool{T}"/>
    /// that should be <see cref="Return">Returned</see>
    /// </summary>
    public static char[] Rent()
    {
        return ArrayPool<char>.Shared.Rent(MinCapacity);
    }

    /// <summary>
    /// Rents a <c>char[]</c> with at least a <see cref="Array.Length"/> of <paramref name="minLength"/>
    /// from the <see cref="char"/> <see cref="ArrayPool{T}"/>
    /// that should be <see cref="Return">Returned</see>
    /// </summary>
    /// <param name="minLength">
    /// The minimum <see cref="Array.Length"/> the returned array can have
    /// </param>
    public static char[] Rent(int minLength)
    {
        minLength = minLength switch
        {
            < MinCapacity => MinCapacity,
            > MaxCapacity => MaxCapacity,
            _ => minLength,
        };
        return _charArrayPool.Rent(minLength);
    }

    /// <summary>
    /// Returns a <see cref="Rent()">Rented</see> <c>char[]</c> back to the <see cref="char"/> <see cref="ArrayPool{T}"/>
    /// </summary>
    /// <param name="array">
    /// The <c>char[]</c> to return, may be <c>null</c>
    /// </param>
    public static void Return(char[]? array)
    {
        if (array is not null && array.Length > 0)
        {
            _charArrayPool.Return(array, true);
        }
    }
}