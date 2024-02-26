using System.Collections;

namespace ScrubJay.Text.Comparision;

/// <summary>
/// An <see cref="IComparer{T}"/> that works on all text types
/// </summary>
public interface ITextComparer :
    IComparer<string?>,
    IComparer<char[]?>,
    IComparer<char>,
    IComparer
{
    /// <inheritdoc cref="IComparer{T}"/>
    int Compare(text x, text y);
}
