namespace ScrubJay.Text.Splitting;

public static class TextSplitExtensions
{
    public static SplitTextEnumerable Split(this text text, text separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison comparison = StringComparison.Ordinal)
    {
        return new SplitTextEnumerable(text, separator, splitOptions, comparison);
    }

    public static SplitTextEnumerable Split(this text text, string? separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison comparison = StringComparison.Ordinal)
    {
        return new SplitTextEnumerable(text, separator.AsSpan(), splitOptions, comparison);
    }

    public static SplitTextEnumerable Split(this text text, in char separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison comparison = StringComparison.Ordinal)
    {
        return new SplitTextEnumerable(text, separator.AsSpan(), splitOptions, comparison);
    }

    public static SplitTextEnumerable Split(this string? str, text separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison comparison = StringComparison.Ordinal)
    {
        return new SplitTextEnumerable(str.AsSpan(), separator, splitOptions, comparison);
    }

    public static SplitTextEnumerable Split(this string? str, string? separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison comparison = StringComparison.Ordinal)
    {
        return new SplitTextEnumerable(str.AsSpan(), separator.AsSpan(), splitOptions, comparison);
    }

    public static SplitTextEnumerable Split(this string? str, in char separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison comparison = StringComparison.Ordinal)
    {
        return new SplitTextEnumerable(str.AsSpan(), separator.AsSpan(), splitOptions, comparison);
    }
}