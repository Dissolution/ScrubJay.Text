#pragma warning disable CA1034, CA1200, IDE0060

using static InlineIL.IL;

// ReSharper disable EntityNameCapturedOnly.Global

namespace ScrubJay.Text;

/// <summary>
/// Methods for efficiently working on textual types (<see cref="char"/>, <see cref="string"/>, <see cref="text">ReadOnlySpan&lt;char&gt;</see>
/// </summary>
[PublicAPI]
public static class TextHelper
{
#region Unsafe

    /// <summary>
    /// <c>unsafe</c> methods on textual types
    /// </summary>
    /// <remarks>
    /// <b>WARNING</b>: All methods in <see cref="Unsafe"/> lack bounds checking and can cause undefined behavior
    /// </remarks>
    public static class Unsafe
    {
        /// <summary>
        /// Efficiently copy <see cref="char">characters</see> from a
        /// <c>in char</c> <paramref name="source"/> pointer
        /// to a <c>ref char</c> <paramref name="destination"/> pointer
        /// <br/>
        /// <b>Warning:</b> behavior is undefined if <paramref name="source"/> and <paramref name="destination"/> overlap
        /// </summary>
        /// <param name="source">
        /// A readonly reference to the starting <see cref="char"/> in the source text
        /// </param>
        /// <param name="destination">
        /// A reference to the starting <see cref="char"/> in the destination buffer
        /// </param>
        /// <param name="count">
        /// The number of <see cref="char"/>s to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
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
            Emit.Ldarg(nameof(destination));    // dest ptr
            Emit.Ldarg(nameof(source));         // src ptr
            // Total byte count == (sizeof(char) * count) == 2*count 
            Emit.Ldarg(nameof(count));
            Emit.Ldc_I4_2();
            Emit.Mul();
            // Cpblk -> takes dest*, source*, uint byteCount
            Emit.Cpblk();
        }

        /* All the public methods for CopyBlock allow for the most efficient conversion of
         * source + dest to what CpBlk is expecting
         *
         * Source types: `in char`, `char[]`, `ReadOnlySpan<char>`, `string`
         * Destination types: `ref char`, `char[]`, `Span<char>`
         */

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// An <c>in char</c> reference to the start of some text
        /// </param>
        /// <param name="destination">
        /// A <c>ref char</c> reference to the start of a writeable text buffer
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(in char source, ref char destination, int count)
        {
            CpBlk(in source, ref destination, count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// An <c>in char</c> reference to the start of some text
        /// </param>
        /// <param name="destination">
        /// A character array (<c>char[]</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(in char source, char[] destination, int count)
        {
            CpBlk(in source, ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// An <c>in char</c> reference to the start of some text
        /// </param>
        /// <param name="destination">
        /// A character span (<c>Span&lt;char&gt;</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(in char source, Span<char> destination, int count)
        {
            CpBlk(in source, ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A character array (<c>char[]</c>) to be read from
        /// </param>
        /// <param name="destination">
        /// A <c>ref char</c> reference to the start of a writeable text buffer
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(char[] source, ref char destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A character array (<c>char[]</c>) to be read from
        /// </param>
        /// <param name="destination">
        /// A character array (<c>char[]</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(char[] source, char[] destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A character array (<c>char[]</c>) to be read from
        /// </param>
        /// <param name="destination">
        /// A character span (<c>Span&lt;char&gt;</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(char[] source, Span<char> destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <c>ReadOnlySpan&lt;char&gt;</c> to be read from
        /// </param>
        /// <param name="destination">
        /// A <c>ref char</c> reference to the start of a writeable text buffer
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        public static void CopyBlock(text source, ref char destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <c>ReadOnlySpan&lt;char&gt;</c> to be read from
        /// </param>
        /// <param name="destination">
        /// A character array (<c>char[]</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(text source, char[] destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <c>ReadOnlySpan&lt;char&gt;</c> to be read from
        /// </param>
        /// <param name="destination">
        /// A character span (<c>Span&lt;char&gt;</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(text source, Span<char> destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <see cref="string"/> to be read from
        /// </param>
        /// <param name="destination">
        /// A <c>ref char</c> reference to the start of a writeable text buffer
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(string source, ref char destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <see cref="string"/> to be read from
        /// </param>
        /// <param name="destination">
        /// A character array (<c>char[]</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(string source, char[] destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <see cref="string"/> to be read from
        /// </param>
        /// <param name="destination">
        /// A character span (<c>Span&lt;char&gt;</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(string source, Span<char> destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }
    }

#endregion

#region TryCopy

    /* All TryCopy methods are mirrors of the methods in Unsafe with nullability and bounds checking
     *
     * Source types: `char[]`, `ReadOnlySpan<char>`, `string`
     * Destination types: `char[]`, `Span<char>`
     */

    /// <summary>
    /// Try to copy a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
    /// </summary>
    /// <param name="source">
    /// A character array (<c>char[]</c>) to be read from
    /// </param>
    /// <param name="destination">
    /// A character array (<c>char[]</c>) to be written to
    /// </param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that is either:<br/>
    /// <c>Ok(int)</c> that contains the number of characters copied<br/>
    /// <c>Error(Exception)</c> that explains why the copy failed
    /// </returns>
    public static Result<int, Exception> TryCopy(char[]? source, char[]? destination)
    {
        if (source is null)
            return new ArgumentNullException(nameof(source));
        if (destination is null)
            return new ArgumentNullException(nameof(destination));
        int count = source.Length;
        if (count > destination.Length)
            return new ArgumentException($"Destination cannot contain {count} characters", nameof(destination));
        Unsafe.CopyBlock(source, destination, count);
        return count;
    }

    /// <summary>
    /// Try to copy a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
    /// </summary>
    /// <param name="source">
    /// A character array (<c>char[]</c>) to be read from
    /// </param>
    /// <param name="destination">
    /// A character span (<c>Span&lt;char&gt;</c>) to be written to
    /// </param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that is either:<br/>
    /// <c>Ok(int)</c> that contains the number of characters copied<br/>
    /// <c>Error(Exception)</c> that explains why the copy failed
    /// </returns>
    public static Result<int, Exception> TryCopy(char[]? source, Span<char> destination)
    {
        if (source is null)
            return new ArgumentNullException(nameof(source));
        int count = source.Length;
        if (count > destination.Length)
            return new ArgumentException($"Destination cannot contain {count} characters", nameof(destination));
        Unsafe.CopyBlock(source, destination, count);
        return count;
    }

    /// <summary>
    /// Try to copy a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
    /// </summary>
    /// <param name="source">
    /// A <c>ReadOnlySpan&lt;char&gt;</c> to be read from
    /// </param>
    /// <param name="destination">
    /// A character array (<c>char[]</c>) to be written to
    /// </param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that is either:<br/>
    /// <c>Ok(int)</c> that contains the number of characters copied<br/>
    /// <c>Error(Exception)</c> that explains why the copy failed
    /// </returns>
    public static Result<int, Exception> TryCopy(text source, char[]? destination)
    {
        if (destination is null)
            return new ArgumentNullException(nameof(destination));
        int count = source.Length;
        if (count > destination.Length)
            return new ArgumentException($"Destination cannot contain {count} characters", nameof(destination));
        Unsafe.CopyBlock(source, destination, count);
        return count;
    }

    /// <summary>
    /// Try to copy a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
    /// </summary>
    /// <param name="source">
    /// A <c>ReadOnlySpan&lt;char&gt;</c> to be read from
    /// </param>
    /// <param name="destination">
    /// A character span (<c>Span&lt;char&gt;</c>) to be written to
    /// </param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that is either:<br/>
    /// <c>Ok(int)</c> that contains the number of characters copied<br/>
    /// <c>Error(Exception)</c> that explains why the copy failed
    /// </returns>
    public static Result<int, Exception> TryCopy(text source, Span<char> destination)
    {
        int count = source.Length;
        if (count > destination.Length)
            return new ArgumentException($"Destination cannot contain {count} characters", nameof(destination));
        Unsafe.CopyBlock(source, destination, count);
        return count;
    }

    /// <summary>
    /// Try to copy a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
    /// </summary>
    /// <param name="source">
    /// A <see cref="string"/> to be read from
    /// </param>
    /// <param name="destination">
    /// A character array (<c>char[]</c>) to be written to
    /// </param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that is either:<br/>
    /// <c>Ok(int)</c> that contains the number of characters copied<br/>
    /// <c>Error(Exception)</c> that explains why the copy failed
    /// </returns>
    public static Result<int, Exception> TryCopy(string? source, char[]? destination)
    {
        if (source is null)
            return new ArgumentNullException(nameof(source));
        if (destination is null)
            return new ArgumentNullException(nameof(destination));
        int count = source.Length;
        if (count > destination.Length)
            return new ArgumentException($"Destination cannot contain {count} characters", nameof(destination));
        Unsafe.CopyBlock(source, destination, count);
        return count;
    }

    /// <summary>
    /// Try to copy a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
    /// </summary>
    /// <param name="source">
    /// A <see cref="string"/> to be read from
    /// </param>
    /// <param name="destination">
    /// A character span (<c>Span&lt;char&gt;</c>) to be written to
    /// </param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that is either:<br/>
    /// <c>Ok(int)</c> that contains the number of characters copied<br/>
    /// <c>Error(Exception)</c> that explains why the copy failed
    /// </returns>
    public static Result<int, Exception> TryCopy(string? source, Span<char> destination)
    {
        if (source is null)
            return new ArgumentNullException(nameof(source));
        int count = source.Length;
        if (count > destination.Length)
            return new ArgumentException($"Destination cannot contain {count} characters", nameof(destination));
        Unsafe.CopyBlock(source, destination, count);
        return count;
    }

#endregion

    
    public static string Repeat(int count, char ch)
    {
        if (count <= 0) 
            return string.Empty;
        return new string(ch, count);
    }
    
    public static string Repeat(int count, scoped text text)
    {
        int textLength = text.Length;
        int totalLength = count * textLength;
        if (totalLength <= 0)
            return string.Empty;
        Span<char> buffer = stackalloc char[totalLength];
        int i = 0;
        do
        {
            Unsafe.CopyBlock(text, buffer[i..], textLength);
            i += textLength;
        } while (i < totalLength);
        return buffer.AsString();
    }
}