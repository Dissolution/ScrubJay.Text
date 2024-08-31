using static InlineIL.IL;
// ReSharper disable EntityNameCapturedOnly.Global

namespace ScrubJay.Text;

[PublicAPI]
public static class TextHelper
{
    /// <summary>
    /// Unsafe methods for textual types
    /// </summary>
    /// <remarks>
    /// <b>WARNING</b>: All methods in <see cref="Unsafe"/> lack bounds checking!
    /// </remarks>
    public static class Unsafe
    {
        /// <summary>
        /// Efficiently copy <paramref name="count"/> <see cref="char">characters</see> from a
        /// <c>in</c> <see cref="char"/> <paramref name="source"/>
        /// to a <c>ref</c> <see cref="char"/> <paramref name="destination"/>
        /// </summary>
        /// <param name="source">A readonly reference to the starting <see cref="char"/> in the source text</param>
        /// <param name="destination">A reference to the starting <see cref="char"/> in the destination buffer</param>
        /// <param name="count">The number of <see cref="char"/>s to copy from <paramref name="source"/> to <paramref name="destination"/></param>
        /// <remarks>
        /// <see cref="M:System.Runtime.CompilerServices.Unsafe.CopyBlock"/> has limitations:<br/>
        /// - It requires either <c>ref byte</c><br/>
        /// --- which requires a conversion from <see cref="char"/> to <see cref="byte"/><br/>
        /// --- and a conversion from <c>in byte</c> to <c>ref byte</c> (as a source <see cref="string"/> will only return <c>readonly</c> references)<br/>
        /// - Or <c>void*</c><br/>
        /// --- which requires <c>unsafe</c> and <c>fixed</c> to manipulate the pointers<br/>
        /// <br/>
        /// This version of CopyBlock restricts <paramref name="source"/> and <paramref name="destination"/> to <see cref="char"/><br/>
        /// and uses the <c>Cpblk</c> instruction directly<br/>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CpBlk(in char source, ref char destination, int count)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(count));
            Emit.Sizeof<char>(); // == 2
            Emit.Mul();

            // `*2` == `<< 1`
            //Emit.Ldc_I4_1();
            //Emit.Shl();

            Emit.Cpblk();
        }

        /* This is all the variations of CopyBlock:
         * Source types: `in char`, `char[]`, `ReadOnlySpan<char>`, `string`
         * Destination types: `ref char`, `char[]`, `Span<char>`
         */

        /// <summary>
        /// Copies the block of <paramref name="count"/> <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(in char source, ref char destination, int count)
        {
            CpBlk(in source, ref destination, count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/> <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(in char source, char[] destination, int count)
        {
            CpBlk(in source, ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/> <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(in char source, Span<char> destination, int count)
        {
            CpBlk(in source, ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/> <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(char[] source, ref char destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(char[] source, char[] destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(char[] source, Span<char> destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(text source, ref char destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(text source, char[] destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(text source, Span<char> destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(string source, ref char destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(string source, char[] destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(string source, Span<char> destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }
    }

    /* These methods mirror Unsafe.CopyBlock
     * Source types: `char[]`, `ReadOnlySpan<char>/text`, `string`
     * Destination types: `char[]`, `Span<char>`
     */

    public static bool TryCopy(char[]? source, char[]? destination)
    {
        if (source is null || destination is null)
            return false;
        int count = source.Length;
        if (count > destination.Length)
            return false;
        Unsafe.CopyBlock(source, destination, count);
        return true;
    }

    public static bool TryCopy(char[]? source, Span<char> destination)
    {
        if (source is null)
            return false;
        int count = source.Length;
        if (count > destination.Length)
            return false;
        Unsafe.CopyBlock(source, destination, count);
        return true;
    }

    public static bool TryCopy(text source, char[]? destination)
    {
        if (destination is null)
            return false;
        int count = source.Length;
        if (count > destination.Length)
            return false;
        Unsafe.CopyBlock(source, destination, count);
        return true;
    }

    public static bool TryCopy(text source, Span<char> destination)
    {
        int count = source.Length;
        if (count > destination.Length)
            return false;
        Unsafe.CopyBlock(source, destination, count);
        return true;
    }

    public static bool TryCopy(string? source, char[]? destination)
    {
        if (source is null || destination is null)
            return false;
        int count = source.Length;
        if (count > destination.Length)
            return false;
        Unsafe.CopyBlock(source, destination, count);
        return true;
    }

    public static bool TryCopy(string? source, Span<char> destination)
    {
        if (source is null)
            return false;
        int count = source.Length;
        if (count > destination.Length)
            return false;
        Unsafe.CopyBlock(source, destination, count);
        return true;
    }
}