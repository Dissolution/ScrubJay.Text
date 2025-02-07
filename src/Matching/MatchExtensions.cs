namespace ScrubJay.Text.Matching;

public static class MatchExtensions
{
    public static bool Matches(this text text, string? value, StringMatch match, StringComparison comparisonType = StringComparison.Ordinal)
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
    
    public static bool Matches(this string? str, string? value, StringMatch match, StringComparison comparisonType = StringComparison.Ordinal)
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