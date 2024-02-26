namespace ScrubJay.Text.Comparision;

public abstract class TextEqualityComparer : ITextEqualityComparer
{
    public virtual bool Equals(string? x, string? y) => Equals(x.AsSpan(), y.AsSpan());

    public virtual bool Equals(char[]? x, char[]? y) => Equals(x.AsSpan(), y.AsSpan());

    public virtual bool Equals(char x, char y) => Equals(x.AsSpan(), y.AsSpan());

    public new bool Equals(object? x, object? y)
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
            char[] chars => chars.AsSpan(),
            char ch => ch.AsSpan(),
            _ => default,
        };
        return Equals(xSpan, ySpan);
    }

    public abstract bool Equals(text x, text y);

    public virtual int GetHashCode(string? text) => GetHashCode(text.AsSpan());

    public virtual int GetHashCode(char[]? chars) => GetHashCode(chars.AsSpan());

    public virtual int GetHashCode(char ch) => GetHashCode(ch.AsSpan());

    public int GetHashCode(object? obj)
    {
        text span = obj switch
        {
            string str => str.AsSpan(),
            char[] chars => chars.AsSpan(),
            char ch => ch.AsSpan(),
            _ => default,
        };
        return GetHashCode(span);
    }
    
    public abstract int GetHashCode(text span);
}