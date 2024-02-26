namespace ScrubJay.Text.Splitting;

public ref struct SplitTextEnumerable //: IEnumerable<ReadOnlySpan<char>>, IEnumerable
{
    /// <summary>
    /// The source <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> that is being split
    /// </summary>
    public readonly text SourceText;

    /// <summary>
    /// The <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> that is being split upon
    /// </summary>
    public readonly text Separator;

    /// <summary>
    /// Additional options applied to splitting
    /// </summary>
    public readonly TextSplitOptions SplitOptions;

    /// <summary>
    /// The <see cref="Comparison"/> used to determine if a <see cref="Separator"/> matches
    /// </summary>
    public readonly StringComparison Comparison;

    public SplitTextEnumerable(
        text sourceText,
        text separator,
        TextSplitOptions splitOptions = TextSplitOptions.None,
        StringComparison comparison = StringComparison.Ordinal)
    {
        this.SourceText = sourceText;
        this.Separator = separator;
        this.SplitOptions = splitOptions;
        this.Comparison = comparison;
    }

    public SplitTextEnumerator GetEnumerator()
    {
        return new SplitTextEnumerator(this);
    }

    public IReadOnlyList<string> ToStringList()
    {
        var strings = new List<string>();
        var e = GetEnumerator();
        while (e.MoveNext())
        {
            strings.Add(e.Text.ToString());
        }

        return strings;
    }

    public IReadOnlyList<Range> ToRangeList()
    {
        var ranges = new List<Range>();
        var e = GetEnumerator();
        while (e.MoveNext())
        {
            ranges.Add(e.Range);
        }

        return ranges;
    }
}