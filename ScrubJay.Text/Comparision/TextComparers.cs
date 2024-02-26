﻿namespace ScrubJay.Text.Comparision;

public abstract class TextComparers : ITextComparer, ITextEqualityComparer
{
    public static implicit operator TextComparers(StringComparison comparison)
        => FromComparison(comparison);

    public static TextComparers CurrentCulture { get; } = new StringComparisonTextComparers(StringComparison.CurrentCulture);
    public static TextComparers CurrentCultureIgnoreCase { get; } = new StringComparisonTextComparers(StringComparison.CurrentCultureIgnoreCase);
    public static TextComparers Ordinal { get; } = new StringComparisonTextComparers(StringComparison.Ordinal);
    public static TextComparers OrdinalIgnoreCase { get; } = new StringComparisonTextComparers(StringComparison.OrdinalIgnoreCase);
    public static TextComparers Invariant { get; } = new StringComparisonTextComparers(StringComparison.InvariantCulture);
    public static TextComparers InvariantIgnoreCase { get; } = new StringComparisonTextComparers(StringComparison.InvariantCultureIgnoreCase);
    
    public static TextComparers FromComparison(StringComparison comparison)
    {
        return comparison switch
        {
            StringComparison.CurrentCulture => CurrentCulture,
            StringComparison.CurrentCultureIgnoreCase => CurrentCultureIgnoreCase,
            StringComparison.InvariantCulture => Invariant,
            StringComparison.InvariantCultureIgnoreCase => InvariantIgnoreCase,
            StringComparison.Ordinal => Ordinal,
            StringComparison.OrdinalIgnoreCase => OrdinalIgnoreCase,
            _ => throw new InvalidOperationException(),
        };
    }
    
    public abstract int Compare(text x, text y);
    public abstract bool Equals(text x, text y);
    public abstract int GetHashCode(text span);
    
    
    public virtual int Compare(string? x, string? y) => Compare(x.AsSpan(), y.AsSpan());

    public virtual int Compare(char[]? x, char[]? y) => Compare(x.AsSpan(), y.AsSpan());

    public virtual int Compare(char x, char y) => Compare(x.AsSpan(), y.AsSpan());

    public int Compare(object? x, object? y)
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
    
    public virtual bool Equals(string? x, string? y) => Equals(x.AsSpan(), y.AsSpan());

    public virtual bool Equals(char[]? x, char[]? y) => Equals(x.AsSpan(), y.AsSpan());

    public virtual bool Equals(char x, char y) => Equals(x.AsSpan(), y.AsSpan());

    public new bool Equals(object? x, object? y)
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
    
    public virtual int GetHashCode(string? text) => GetHashCode(text.AsSpan());

    public virtual int GetHashCode(char[]? chars) => GetHashCode(chars.AsSpan());

    public virtual int GetHashCode(char ch) => GetHashCode(ch.AsSpan());

    public int GetHashCode(object? obj)
    {
        return obj switch
        {
            char ch => GetHashCode(ch),
            char[] chars => GetHashCode(chars.AsSpan()),
            string str => GetHashCode(str.AsSpan()),
            _ => 0,
        };
    }
}