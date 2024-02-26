using System.Collections;

namespace ScrubJay.Text.Comparision;

/// <summary>
/// An <see cref="IEqualityComparer{T}"/> that works on all text types
/// </summary>
public interface ITextEqualityComparer : 
    IEqualityComparer<string?>,
    IEqualityComparer<char[]>,
    IEqualityComparer<char>,
    IEqualityComparer
{
    /// <inheritdoc cref="IEqualityComparer{T}"/>
    bool Equals(text x, text y);
    
    /// <inheritdoc cref="IEqualityComparer{T}"/>
    int GetHashCode(text span);
}