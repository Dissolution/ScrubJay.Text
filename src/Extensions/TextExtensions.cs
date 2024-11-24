using ScrubJay.Comparison;
using ScrubJay.Enums;
using ScrubJay.Text.Matching;

namespace ScrubJay.Text.Extensions;

public static class TextExtensions
{
    /// <summary>
    /// Gets this <see cref="text"/> as a <see cref="string"/>
    /// </summary>
    public static string AsString(this text text)
    {
#if NET481_OR_GREATER || NETSTANDARD2_0
        unsafe
        {
            fixed (char* ptr = text)
            {
                return new string(ptr, 0, text.Length);
            }
        }
#else
        return new string(text);
#endif
    }

    public static bool Equals(this text text, string? value, StringMatch match, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (match == StringMatch.Exact)
        {
            return Equate.Text(text, value, comparisonType);
        }

        // null values cannot match after this point
        if (value is null)
            return false;

        if (match.HasFlags(StringMatch.Contains))
        {
            if (text.Contains(value.AsSpan(), comparisonType))
                return true;
        }
        else
        {
            if (match.HasFlags(StringMatch.StartsWith) && text.StartsWith(value.AsSpan(), comparisonType))
                return true;
            if (match.HasFlags(StringMatch.EndsWith) && text.EndsWith(value.AsSpan(), comparisonType))
                return true;
        }

        // does not meet specifications
        return false;
    }
}