#pragma warning disable CS8981

// Alias `text` to `ReadOnlySpan<char>` to simplify usage
global using text = System.ReadOnlySpan<char>;
global using NotNull = System.Diagnostics.CodeAnalysis.NotNullAttribute;


/* When I find a use for another Text-based GlobalHelper, use this template
 * namespace ScrubJay.Text
 * public static class GlobalHelper
 */