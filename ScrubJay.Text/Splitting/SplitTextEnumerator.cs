using System.ComponentModel;

namespace ScrubJay.Text.Splitting;

public ref struct SplitTextEnumerator //: IEnumerator<ReadOnlySpan<char>>, IEnumerator
{
    private readonly SplitTextEnumerable _enumerable;

    private int _position = 0;
    private Range _currentRange = default;

    /// <inheritdoc cref="IEnumerator{T}"/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public text Current => _enumerable.SourceText[_currentRange];

    /// <summary>
    /// The current slice's <see cref="Range"/> in <see cref="SourceText"/>
    /// </summary>
    public Range Range => _currentRange;

    /// <summary>
    /// The current slice's <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> from <see cref="SourceText"/>
    /// </summary>
    public text Text => Current;

    internal SplitTextEnumerator(SplitTextEnumerable enumerable)
    {
        _enumerable = enumerable;
    }

    public void Deconstruct(out Range range)
    {
        range = _currentRange;
    }

    public void Deconstruct(out text text)
    {
        text = Current;
    }

    public void Deconstruct(out Range range, out text text)
    {
        range = _currentRange;
        text = Current;
    }

    public bool MoveNext()
    {
        text sourceText = _enumerable.SourceText;
        int sourceLength = sourceText.Length;
        // inclusive start index
        int sliceStart;
        // exclusive end index
        int sliceEnd;

        // scan
        while (true)
        {
            // slice starts where we left off
            sliceStart = _position;

            // After the end = done enumerating
            if (sliceStart > sourceLength)
            {
                // clear the range
                _currentRange = default;
                // but leave _position after the end in case MoveNext is called again
                return false;
            }

            // At end = might need to yield a last empty slice
            if (sliceStart == sourceLength)
            {
                // finished enumerating               
                _position = sourceLength + 1;

                // If we are not removing empty lines
                if (!_enumerable.SplitOptions.HasFlags(TextSplitOptions.RemoveEmptyLines))
                {
                    // return the empty
                    _currentRange = new Range(start: sliceStart, end: sliceStart);
                    return true;
                }

                // clear the range
                _currentRange = default;
                // but leave _position after the end in case MoveNext is called again
                return false;
            }

            // Scan for next separator
            var separator = _enumerable.Separator;
            var separatorLength = separator.Length;
            var separatorIndex = sourceText.NextIndexOf(
                separator,
                _position,
                _enumerable.Comparison);

            // None found or an empty separator
            if (separatorIndex == -1 || separatorLength == 0)
            {
                // end of slice is the rest of the text
                sliceEnd = sourceLength;
                // finished enumerating    
                _position = sliceEnd + 1;
            }
            else
            {
                // This slice ends where the separator starts
                sliceEnd = separatorIndex;
                // We'll start again where the separator ends
                _position = sliceEnd + separatorLength;
            }

            // Respect StringSplitOptions
            if (_enumerable.SplitOptions.HasFlags(TextSplitOptions.TrimLines))
            {
                // Copied from ReadOnlySpan<char>.Trim()
                for (; sliceStart < sliceEnd; sliceStart++)
                {
                    if (!char.IsWhiteSpace(sourceText[sliceStart]))
                    {
                        break;
                    }
                }

                for (; sliceEnd > sliceStart; sliceEnd--)
                {
                    if (!char.IsWhiteSpace(sourceText[sliceEnd - 1]))
                    {
                        break;
                    }
                }
            }

            _currentRange = new Range(
                /* inclusive */
                start: sliceStart,
                /* exclusive */
                end: sliceEnd);

            // Respect StringSplitOptions
            if ((sliceEnd - sliceStart) > 0 || !_enumerable.SplitOptions.HasFlags(TextSplitOptions.RemoveEmptyLines))
            {
                // This is a valid return slice
                return true;
            }

            // We're not going to return this slice
            // _position has been updated, start the next scan
        }
    }

    public void Reset()
    {
        _position = 0;
        _currentRange = default;
    }
}