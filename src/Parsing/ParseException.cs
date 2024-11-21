namespace ScrubJay.Text.Parsing;

[PublicAPI]
public class ParseException : InvalidOperationException
{
    public string? Input { get;}
    public Type? DestType { get; }

    public ParseException(string? input, Type? destType)
        : base($"Could not parse \"{input}\" into a {destType}")
    {
        this.Input = input;
        this.DestType = destType;
    }
}