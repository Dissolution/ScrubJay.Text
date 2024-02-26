﻿namespace ScrubJay.Text.Comparision;

/// <inheritdoc />
/// <summary>
/// Compares two strings by using logical numeric and character order.
/// </summary>
public sealed class AlphanumericTextComparer : TextComparer
{
    private readonly StringComparison _stringComparison;
    private readonly TextDirection _textDirection;

    public AlphanumericTextComparer(StringComparison stringComparison = StringComparison.CurrentCulture,
        TextDirection textDirection = TextDirection.LeftToRight)
    {
        _stringComparison = stringComparison;
        _textDirection = textDirection;
    }

    private static int FrontToBlackCompare(text left,
        text right,
        StringComparison comparison)
    {
        int l = 0;
        int leftLength = left.Length;

        int r = 0;
        int rightLength = right.Length;

        int result = 0;

        while (l < leftLength || r < rightLength)
        {
            if (l >= leftLength) return -1; // Left ran out, right is longer
            if (r >= rightLength) return 1; // Right ran out, left is longer

            // Get left chunk (grouped based on Digits / NonDigits
            bool leftChunkDigit = char.IsDigit(left[l]);
            int leftChunkStart = l;
            do
            {
                l++;
            } while (l < leftLength && char.IsDigit(left[l]) == leftChunkDigit);
            text leftChunk = left.Slice(leftChunkStart, l - leftChunkStart);

            // Get right chunk (grouped based on Digits / NonDigits
            bool rightChunkDigit = char.IsDigit(right[r]);
            int rightChunkStart = r;
            do
            {
                r++;
            } while (r < rightLength && char.IsDigit(right[r]) == rightChunkDigit);
            text rightChunk = right.Slice(rightChunkStart, r - rightChunkStart);

            // If they are both digit chunks, compare them
            if (leftChunkDigit && rightChunkDigit &&
#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
                int.TryParse(leftChunk, out int leftChunkValue) &&
                int.TryParse(rightChunk, out int rightChunkValue))
#else
                int.TryParse(leftChunk.ToString(), out int leftChunkValue) &&
                int.TryParse(rightChunk.ToString(), out int rightChunkValue))
#endif
            {
                if (leftChunkValue < rightChunkValue)
                    result = -1;
                else if (leftChunkValue > rightChunkValue)
                    result = 1;
            }
            // Otherwise, use our string comparer
            else
            {
                result = leftChunk.CompareTo(rightChunk, comparison);
            }

            // Did we find a comparison?
            if (result != 0)
                return result;
        }

        // They are the same
        return 0;
    }

    private static int BackToFrontCompare(text left,
        text right,
        StringComparison comparison)
    {
        int l = left.Length - 1;

        int r = right.Length - 1;

        int result = 0;

        while (l >= 0 || r >= 0)
        {
            if (l < 0) return -1; // Left ran out, right is longer
            if (r < 0) return 1; // Right ran out, left is longer

            // Get left chunk (grouped based on Digits / NonDigits
            bool leftChunkDigit = char.IsDigit(left[l]);
            int leftChunkEnd = l + 1;
            do
            {
                l--;
            } while (l >= 0 && char.IsDigit(left[l]) == leftChunkDigit);
            text leftChunk = left[(l + 1)..leftChunkEnd];

            // Get right chunk (grouped based on Digits / NonDigits
            bool rightChunkDigit = char.IsDigit(right[r]);
            int rightChunkEnd = r + 1;
            do
            {
                r--;
            } while (r >= 0 && char.IsDigit(right[r]) == rightChunkDigit);
            text rightChunk = right[(r + 1)..rightChunkEnd];

            // If they are both digit chunks, compare them
            if (leftChunkDigit && rightChunkDigit &&
#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
                int.TryParse(leftChunk, out int leftChunkValue) &&
                int.TryParse(rightChunk, out int rightChunkValue))
#else
                int.TryParse(leftChunk.ToString(), out int leftChunkValue) &&
                int.TryParse(rightChunk.ToString(), out int rightChunkValue))
#endif
            {
                if (leftChunkValue < rightChunkValue)
                    result = -1;
                else if (leftChunkValue > rightChunkValue)
                    result = 1;
            }
            // Otherwise, use our string comparer
            else
            {
                result = leftChunk.CompareTo(rightChunk, comparison);
            }

            // Did we find a comparison?
            if (result != 0)
                return result;
        }

        // They are the same
        return 0;
    }

    public override int Compare(text x, text y)
    {
        if (_textDirection == TextDirection.LeftToRight)
            return FrontToBlackCompare(x, y, _stringComparison);
        return BackToFrontCompare(x, y, _stringComparison);
    }
}