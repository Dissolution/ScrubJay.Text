namespace ScrubJay.Text.Scratch;

[InterpolatedStringHandler]
public ref struct InterpolatedTextFormatter
{
    private readonly TextFormatter _formatter;
    
    public InterpolatedTextFormatter(TextFormatter formatter)
    {
        _formatter = formatter;
    }

    public InterpolatedTextFormatter(int literalLength, int formattedCount, TextFormatter formatter)
    {
        _formatter = formatter;
    }
}