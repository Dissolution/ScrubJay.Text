namespace ScrubJay.Text.Extensions;

/// <summary>
/// Extensions on <see cref="Buffer{T}"/> where <c>T</c> is <see cref="char"/>
/// </summary>
public static class CharBufferExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddMany(this Buffer<char> buffer, string? str) => buffer.AddMany(str.AsSpan());
}