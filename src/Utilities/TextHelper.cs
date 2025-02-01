using static InlineIL.IL;
// ReSharper disable EntityNameCapturedOnly.Global

namespace ScrubJay.Text.Utilities;

public static class TextHelper
{
    /// <summary>
    /// <b>WARNING</b>: All methods in <see cref="Unsafe"/> lack bounds checking!
    /// </summary>
    /// <remarks>
    /// Input:<br/>
    /// <c>in char</c>, <c>char[]</c>, <c>ReadOnlySpan&lt;char&gt;</c>, <c>string</c><br/>
    /// Output:<br/>
    /// <c>ref char</c>, <c>char[]</c>, <c>Span&lt;char&gt;</c><br/>
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
        /// What we really want (and what this does) is to restrict the <paramref name="source"/> and <paramref name="destination"/> to <see cref="char"/><br/>
        /// and hide all the <see cref="char"/>&lt;-&gt;<see cref="byte"/> and <c>void*</c> in the IL<br/>
        /// which can use the <c>Cpblk</c> instruction directly<br/>
        /// We also swap the arguments, as <see cref="System.Runtime.CompilerServices.Unsafe"/> mirrors <c>Cpblk</c>> in that it takes the <i>destination</i> first<br/>
        /// rather than the <i>source</i> (which I believe is less intuitive than source first)<br/>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CopyBlock(
            in char source,
            ref char destination,
            int count)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(count));
            Emit.Sizeof<char>();
            Emit.Mul();
            Emit.Cpblk();
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            in char source,
            [DisallowNull, NotNull] char[] destination,
            int count)
        {
            CopyBlock(in source, ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            in char source,
            Span<char> destination,
            int count)
        {
            CopyBlock(in source, ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            [DisallowNull, NotNull] char[] source,
            ref char destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            [DisallowNull, NotNull] char[] source,
            [DisallowNull, NotNull] char[] destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            [DisallowNull, NotNull] char[] source,
            Span<char> destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            text source,
            ref char destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            text source,
            [DisallowNull, NotNull] char[] destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            text source,
            Span<char> destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            [DisallowNull, NotNull] string source,
            ref char destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            [DisallowNull, NotNull] string source,
            [DisallowNull, NotNull] char[] destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies the block of <paramref name="count"/>  <see cref="char">characters</see>
        /// from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">The source readable text</param>
        /// <param name="destination">The destination writable text</param>
        /// <param name="count">The number of characters to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(
            [DisallowNull, NotNull] string source,
            Span<char> destination,
            int count)
        {
            CopyBlock(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }
    }
    
    public static Result TryCopyTo(char source, [AllowNull, NotNullWhen(true)] char[]? dest)
    {
        if (dest is null)
            return new ArgumentNullException(nameof(dest));
        if (dest.Length == 0)
            return new ArgumentException("Destination must have a capacity of at least 1", nameof(dest));
        Unsafe.CopyBlock(
            in source,
            ref dest.GetPinnableReference(),
            1);
        return Ok();
    }
    
    public static Result TryCopyTo(char source, scoped Span<char> dest)
    {
        if (dest.Length == 0)
            return new ArgumentException("Destination must have a capacity of at least 1", nameof(dest));
        Unsafe.CopyBlock(
            in source,
            ref dest.GetPinnableReference(),
            1);
        return Ok();
    }
    
    public static Result TryCopyTo([AllowNull, NotNullWhen(true)] char[]? source, [AllowNull, NotNullWhen(true)] char[]? dest)
    {
        if (source is null)
            return new ArgumentNullException(nameof(source));
        if (dest is null)
            return new ArgumentNullException(nameof(dest));
        int sourceLen = source.Length;
        if (sourceLen == 0) return Ok();
        if (sourceLen > dest.Length)
            return new ArgumentException("Destination must be at least as large as Source", nameof(dest));
        Unsafe.CopyBlock(
            in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            sourceLen);
        return Ok();
    }
    
    public static Result TryCopyTo([AllowNull, NotNullWhen(true)] char[]? source, scoped Span<char> dest)
    {
        if (source is null)
            return new ArgumentNullException(nameof(source));
        int sourceLen = source.Length;
        if (sourceLen == 0) return Ok();
        if (sourceLen > dest.Length)
            return new ArgumentException("Destination must be at least as large as Source", nameof(dest));
        Unsafe.CopyBlock(
            in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            sourceLen);
        return Ok();
    }
    
    public static Result TryCopyTo(scoped text source, [AllowNull, NotNullWhen(true)] char[]? dest)
    {
        if (dest is null)
            return new ArgumentNullException(nameof(dest));
        int sourceLen = source.Length;
        if (sourceLen == 0) return Ok();
        if (sourceLen > dest.Length)
            return new ArgumentException("Destination must be at least as large as Source", nameof(dest));
        Unsafe.CopyBlock(
            in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            sourceLen);
        return Ok();
    }
    
    public static Result TryCopyTo(scoped text source, scoped Span<char> dest)
    {
        int sourceLen = source.Length;
        if (sourceLen == 0) return Ok();
        if (sourceLen > dest.Length)
            return new ArgumentException("Destination must be at least as large as Source", nameof(dest));
        Unsafe.CopyBlock(
            in source.GetPinnableReference(),
            ref dest.GetPinnableReference(),
            sourceLen);
        return Ok();
    }
}