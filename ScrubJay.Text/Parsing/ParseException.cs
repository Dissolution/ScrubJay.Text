using ScrubJay.Text.Building;


namespace ScrubJay.Text.Parsing;

public sealed class ParseException : ArgumentException
{
    public static ParseException Create(
        text input, 
        Type? destType,
        Exception? innerException = null,
        string? extraInfo = null,
        [CallerArgumentExpression(nameof(input))]
        string? inputName = null)
    {
        var message = TextBuilder.New
            .Append($"Could not parse '{input}' to a '{destType?.Name}' value")
            .If(extraInfo.IsNonWhiteSpace(), tb => tb.Append($": {extraInfo}"))
            .ToStringAndDispose();
        return new ParseException(message, inputName, innerException);
    }
    
    public static ParseException Create(
        string? input, 
        Type? destType,
        Exception? innerException = null,
        string? extraInfo = null,
        [CallerArgumentExpression(nameof(input))]
        string? inputName = null)
    {
        var message = TextBuilder.New
            .Append($"Could not parse '{input}' to a '{destType?.Name}' value")
            .If(extraInfo.IsNonWhiteSpace(), tb => tb.Append($": {extraInfo}"))
            .ToStringAndDispose();
        return new ParseException(message, inputName, innerException);
    }

    public static ParseException Create<TDest>(
        text input,
        Exception? innerException = null,
        string? extraInfo = null,
        [CallerArgumentExpression(nameof(input))]
        string? inputName = null)
        => Create(input, typeof(TDest), innerException, extraInfo, inputName);
    
    public static ParseException Create<TDest>(
        string? input,
        Exception? innerException = null,
        string? extraInfo = null,
        [CallerArgumentExpression(nameof(input))]
        string? inputName = null)
        => Create(input, typeof(TDest), innerException, extraInfo, inputName);
   

    private ParseException(string? message, string? paramName, Exception? innerException) : 
        base(message, paramName, innerException)
    {
    }
}