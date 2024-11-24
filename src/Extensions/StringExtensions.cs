using ScrubJay.Enums;
using ScrubJay.Text.Matching;

namespace ScrubJay.Text.Extensions;

[PublicAPI]
public static class StringExtensions
{
#if NET481_OR_GREATER || NETSTANDARD2_0 || NETSTANDARD2_1
    /// <summary>
    /// Gets a pinned <c>ref readonly</c> to this <see cref="string"/>
    /// </summary>
    public static ref readonly char GetPinnableReference(this string str)
    {
        unsafe
        {
            fixed (char* strPtr = str)
            {
                return ref Unsafe.AsRef<char>(strPtr);
            }
        }
    }
#endif

    public static bool Equals(this string? str, string? value, StringMatch match, StringComparison comparisonType = StringComparison.Ordinal)
    {
        if (match == StringMatch.Exact)
        {
            return string.Equals(str!, value!, comparisonType);
        }

        // null values cannot match after this point
        if (str is null || value is null)
            return false;

        if (match.HasFlags(StringMatch.Contains))
        {
            if (str.AsSpan().Contains(value.AsSpan(), comparisonType))
                return true;
        }
        else
        {
            if (match.HasFlags(StringMatch.StartsWith) && str.StartsWith(value, comparisonType))
                return true;
            if (match.HasFlags(StringMatch.EndsWith) && str.EndsWith(value, comparisonType))
                return true;
        }

        // does not meet specifications
        return false;
    }
}