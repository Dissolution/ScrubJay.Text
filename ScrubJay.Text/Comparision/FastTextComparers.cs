﻿using System.Collections;

// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Text.Comparision;

/// <summary>
/// The fastest way to Equal, GetHashCode, and Compare text
/// </summary>
public sealed class FastTextComparers : ITextComparer, ITextEqualityComparer
{
    public static FastTextComparers Default { get; } = new();
    
    private FastTextComparers() { }
    
#region Compare
    int IComparer<string?>.Compare(string? x, string? y) => Compare(x, y);
    int IComparer<char[]?>.Compare(char[]? x, char[]? y) => Compare(x, y);
    int IComparer<char>.Compare(char x, char y) => x.CompareTo(y);
    int IComparer.Compare(object? x, object? y)
    {
        text xSpan = x switch
        {
            string str => str.AsSpan(),
            char[] chars => chars.AsSpan(),
            char ch => ch.AsSpan(),
            _ => default,
        };
        text ySpan = y switch
        {
            string str => str.AsSpan(),
            char[] chars => chars.AsSpan(),
            char ch => ch.AsSpan(),
            _ => default,
        };
        return Compare(xSpan, ySpan);
    }
    int ITextComparer.Compare(text x, text y) => Compare(x, y);

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(text x, text y)
    {
        return MemoryExtensions.SequenceCompareTo(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(text x, char[]? y)
    {
        return MemoryExtensions.SequenceCompareTo(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(text x, string? y)
    {
        return MemoryExtensions.SequenceCompareTo(x, MemoryExtensions.AsSpan(y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(char[]? x, text y)
    {
        return MemoryExtensions.SequenceCompareTo(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(char[]? x, char[]? y)
    {
        return MemoryExtensions.SequenceCompareTo<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(char[]? x, string? y)
    {
        return MemoryExtensions.SequenceCompareTo(x, MemoryExtensions.AsSpan(y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(string? x, text y)
    {
        return MemoryExtensions.SequenceCompareTo(MemoryExtensions.AsSpan(x), y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(string? x, char[]? y)
    {
        return MemoryExtensions.SequenceCompareTo(MemoryExtensions.AsSpan(x), y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(string? x, string? y)
    {
        return string.CompareOrdinal(x, y);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(text x, text y, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(text x, char[]? y, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(text x, string? y, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(x, MemoryExtensions.AsSpan(y), comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(char[]? x, text y, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(char[]? x, char[]? y, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(char[]? x, string? y, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(x, MemoryExtensions.AsSpan(y), comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(string? x, text y, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(MemoryExtensions.AsSpan(x), y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(string? x, char[]? y, StringComparison comparison)
    {
        return MemoryExtensions.CompareTo(MemoryExtensions.AsSpan(x), y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Compare(string? x, string? y, StringComparison comparison)
    {
        return string.Compare(x, y, comparison);
    }
#endregion
    
#region Equals
    bool IEqualityComparer<string?>.Equals(string? x, string? y) => Equals(x, y);
    bool IEqualityComparer<char[]>.Equals(char[]? x, char[]? y) => Equals(x, y);
    bool IEqualityComparer<char>.Equals(char x, char y) => x == y;
    bool IEqualityComparer.Equals(object? x, object? y)
    {
        text xSpan = x switch
        {
            string str => str.AsSpan(),
            char[] chars => chars,
            char ch => ch.AsSpan(),
            _ => default,
        };
        text ySpan = y switch
        {
            string str => str.AsSpan(),
            char[] chars => chars.AsSpan(),
            char ch => ch.AsSpan(),
            _ => default,
        };
        return Equals(xSpan, ySpan);
    }
    bool ITextEqualityComparer.Equals(text x, text y) => Equals(x, y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(text x, text y)
    {
        return MemoryExtensions.SequenceEqual(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(text x, char[]? y)
    {
        return MemoryExtensions.SequenceEqual(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(text x, string? y)
    {
        return MemoryExtensions.SequenceEqual(x, MemoryExtensions.AsSpan(y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, text y)
    {
        return MemoryExtensions.SequenceEqual(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, char[]? y)
    {
        return MemoryExtensions.SequenceEqual<char>(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, string? y)
    {
        return MemoryExtensions.SequenceEqual(x, MemoryExtensions.AsSpan(y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, text y)
    {
        return MemoryExtensions.SequenceEqual(MemoryExtensions.AsSpan(x), y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, char[]? y)
    {
        return MemoryExtensions.SequenceEqual(MemoryExtensions.AsSpan(x), y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, string? y)
    {
        return string.Equals(x, y);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(text x, text y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(text x, char[]? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(text x, string? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, MemoryExtensions.AsSpan(y), comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, text y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, char[]? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(char[]? x, string? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(x, MemoryExtensions.AsSpan(y), comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, text y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(MemoryExtensions.AsSpan(x), y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, char[]? y, StringComparison comparison)
    {
        return MemoryExtensions.Equals(MemoryExtensions.AsSpan(x), y, comparison);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(string? x, string? y, StringComparison comparison)
    {
        return string.Equals(x, y, comparison);
    }

#endregion

#region GetHashCode
    int IEqualityComparer<string?>.GetHashCode(string? str) => GetHashCode(str);
    int IEqualityComparer<char[]>.GetHashCode(char[]? chars) => GetHashCode(chars);
    int IEqualityComparer<char>.GetHashCode(char ch) => (int)ch;
    int IEqualityComparer.GetHashCode(object? obj)
    {
        return obj switch
        {
            char ch => (int)ch,
            char[] chars => GetHashCode(chars.AsSpan()),
            string str => GetHashCode(str.AsSpan()),
            _ => 0,
        };
    }
    int ITextEqualityComparer.GetHashCode(text span) => GetHashCode(span);

#if NETCOREAPP3_1_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(string? str)
    {
        return string.GetHashCode(str);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(char[]? chars)
    {
        return string.GetHashCode(chars);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(text text)
    {
        return string.GetHashCode(text);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(string? str, StringComparison comparison)
    {
        return string.GetHashCode(str, comparison);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(char[]? chars, StringComparison comparison)
    {
        return string.GetHashCode(chars, comparison);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(text text, StringComparison comparison)
    {
        return string.GetHashCode(text, comparison);
    }
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(string? str)
    {
        return TextComparers.Ordinal.GetHashCode(str);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(char[]? chars)
    {
        return TextComparers.Ordinal.GetHashCode(chars);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<char> text)
    {
        return TextComparers.Ordinal.GetHashCode(text);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(string? str, StringComparison comparison)
    {
        return TextComparers.FromComparison(comparison).GetHashCode(str);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(char[]? chars, StringComparison comparison)
    {
        return TextComparers.FromComparison(comparison).GetHashCode(chars);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetHashCode(ReadOnlySpan<char> text, StringComparison comparison)
    {
        return TextComparers.FromComparison(comparison).GetHashCode(text);
    }
#endif
#endregion
}