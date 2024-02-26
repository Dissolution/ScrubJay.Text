namespace ScrubJay.Text.Comparision;

public abstract class TextComparer : ITextComparer
{
    public virtual int Compare(string? x, string? y) => Compare(x.AsSpan(), y.AsSpan());

    public virtual int Compare(char[]? x, char[]? y) => Compare(x.AsSpan(), y.AsSpan());

    public virtual int Compare(char x, char y) => Compare(x.AsSpan(), y.AsSpan());

    public int Compare(object? x, object? y)
    {
        text xSpan = x switch
        {
            string str => str.AsSpan(),
            char[] chars => chars,
            char ch => ch.AsSpan(),
            _ => default,
        };
        text ySpan = y switch
        {
            string str => str.AsSpan(),
            char[] chars => chars,
            char ch => ch.AsSpan(),
            _ => default,
        };
        return Compare(xSpan, ySpan);
    }

    public abstract int Compare(text x, text y);
}