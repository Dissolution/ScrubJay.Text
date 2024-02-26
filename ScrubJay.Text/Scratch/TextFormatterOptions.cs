using static ScrubJay.StaticImports;
using ScrubJay.Text.Collections;

namespace ScrubJay.Text.Scratch;

public sealed class TextFormatterOptions
{
    private string _newLine = Environment.NewLine;

    public string NewLine
    {
        get => _newLine;
        set
        {
            Throw.IfNullOrEmpty(value);
            _newLine = value;
        }
    }

    public IFormatProvider? FormatProvider { get; set; } = null;

    public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

    public TextFormatterOptions Clone() => new() { NewLine = this.NewLine, FormatProvider = this.FormatProvider, };
}