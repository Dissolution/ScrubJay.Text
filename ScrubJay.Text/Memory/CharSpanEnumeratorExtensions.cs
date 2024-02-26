using ScrubJay.Memory;

namespace ScrubJay.Text.Memory;

/// <summary>
/// Extensions on <see cref="SpanEnumerator{T}">SpanEnumerator&lt;char&gt;</see>
/// </summary>
/// <remarks>
/// Text has extra common ways to be consumed with char.IsXYZ() methods
/// </remarks>
public static class CharSpanEnumeratorExtensions
{
    public static void SkipWhiteSpace(this ref SpanEnumerator<char> textIterator)
        => textIterator.SkipWhile(static ch => char.IsWhiteSpace(ch));
    
    public static void SkipDigits(this ref SpanEnumerator<char> textIterator)
        => textIterator.SkipWhile(static ch => char.IsDigit(ch));

    public static void SkipLetters(this ref SpanEnumerator<char> textIterator)
        => textIterator.SkipWhile(static ch => char.IsLetter(ch));
    
    public static text TakeWhiteSpace(
        this ref SpanEnumerator<char> textIterator)
        => textIterator.TakeWhile(static ch => char.IsWhiteSpace(ch));
    
    public static text TakeDigits(
        this ref SpanEnumerator<char> textIterator)
        => textIterator.TakeWhile(static ch => char.IsDigit(ch));

    public static text TakeLetters(
        this ref SpanEnumerator<char> textIterator)
        => textIterator.TakeWhile(static ch => char.IsLetter(ch));

}

