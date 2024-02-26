using System.Globalization;
using ScrubJay.Collections;

namespace ScrubJay.Text.Scratch;

public class TextFormatter : IDisposable
{
    internal readonly Buffer<char> _textBuffer;

    public TextFormatterOptions Options { get; set; }
    public ValueFormatters Formatters { get; set; }
    public IndentManager Indents { get; set; }


    internal TextFormatter(TextFormatterPool pool)
    {
        this.Options = pool.DefaultOptions.Clone();
        this.Formatters = pool.DefaultFormatters.Clone();
        this.Indents = new(this);
        _textBuffer = new Buffer<char>(1024);
    }

#region New Line(s)
    
    public TextFormatter NewLine()
    {
        this.Indents.WriteNewLine(_textBuffer);
        return this;
    }

    public TextFormatter NewLines(int count)
    {
        for (var i = 0; i < count; i++)
        {
            this.Indents.WriteNewLine(_textBuffer);
        }
        return this;
    }
#endregion
    
#region Append
    public TextFormatter Append(char ch)
    {
        this.Indents.IndentAwareWrite(in ch, _textBuffer);
        return this;
    }

    public TextFormatter Append(scoped text text)
    {
        this.Indents.IndentAwareWrite(text, _textBuffer);
        return this;
    }

    public TextFormatter Append(params char[]? characters)
    {
        this.Indents.IndentAwareWrite(characters, _textBuffer);
        return this;
    }

    public TextFormatter Append(string? str)
    {
        this.Indents.IndentAwareWrite(str.AsSpan(), _textBuffer);
        return this;
    }

    public TextFormatter Append([InterpolatedStringHandlerArgument("")] ref InterpolatedTextFormatter interpolated)
    {
        // We have already written
        return this;
    }
    
    public TextFormatter Append<T>(T? value, text format = default, IFormatProvider? provider = default)
    {
        if (value is Build build)
        {
            return Build(build);
        }
        var formatter = this.Formatters.GetFormatter<T>();
        formatter.WriteValue(value, _textBuffer, format, provider);
        return this;
    }

    public TextFormatter Append(object? obj, text format = default, IFormatProvider? provider = default)
    {
        if (obj is not null)
        {
            if (obj is Build build)
            {
                return Build(build);
            }
            var formatter = this.Formatters.GetFormatter(obj.GetType());
            formatter.WriteObject(obj, _textBuffer, format, provider);
        }
        return this;
    }

    public TextFormatter AppendLine(char ch) => Append(ch).NewLine();
    public TextFormatter AppendLine(scoped text text) => Append(text).NewLine();
    public TextFormatter AppendLine(params char[]? characters) => Append(characters).NewLine();
    public TextFormatter AppendLine(string? str) => Append(str).NewLine();
    public TextFormatter AppendLine([InterpolatedStringHandlerArgument("")] ref InterpolatedTextFormatter interpolated) => Append(ref interpolated).NewLine();
    public TextFormatter AppendLine<T>(T? value, text format = default, IFormatProvider? provider = default)
        => Append<T>(value, format, provider).NewLine();
#endregion
    
#region Casing

    public TextFormatter Case(char ch, Casing casing, CultureInfo? culture = default)
    {
        TextInfo textInfo = (culture ?? CultureInfo.CurrentCulture).TextInfo;
        switch (casing)
        {
            case Casing.Lower:
            case Casing.Camel:
                ch = textInfo.ToLower(ch);
                break;
            case Casing.Upper:
            case Casing.Pascal:
                ch = textInfo.ToUpper(ch);
                break;
        }
        return Append(ch);
    }
    
    public TextFormatter Case(scoped text text, Casing casing, CultureInfo? culture = default)
    {
        int textLen = text.Length;
        if (textLen == 0)
            return this;
        if (casing == Casing.Default)
            return Append(text);
        
        var buffer = _textBuffer.AllocateMany(textLen);
        TextInfo textInfo = (culture ?? CultureInfo.CurrentCulture).TextInfo;
        switch (casing)
        {
            case Casing.Lower:
            {
                for (var i = textLen - 1; i >= 0; i--)
                {
                    buffer[i] = textInfo.ToLower(text[i]);
                }
                break;
            }
            case Casing.Upper:
            {
                for (var i = textLen - 1; i >= 0; i--)
                {
                    buffer[i] = textInfo.ToUpper(text[i]);
                }
                break;
            }
            case Casing.Camel:
            {
                buffer[0] = textInfo.ToLower(text[0]);
                TextHelper.CopyTo(text.Slice(1), buffer.Slice(1));
                break;
            }
            case Casing.Pascal:
            {
                buffer[0] = textInfo.ToUpper(text[0]);
                TextHelper.CopyTo(text.Slice(1), buffer.Slice(1));
                break;
            }
        }
        return this;
    }

    public TextFormatter Case(string? str, Casing casing, CultureInfo? culture = default)
        => Case(str.AsSpan(), casing, culture);

    public TextFormatter Case<T>(T? value, Casing casing)
    {
        CaptureWritten(tb => tb.Append<T>(value), out var written);
        written.Cased(casing);
        return this;
    }
#endregion

#region Align
    public TextFormatter Align(char ch, int width, Alignment alignment)
    {
        if (width < 1)
            throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be 1 or greater");

        var buffer = _textBuffer.AllocateMany(width);
        if (alignment == Alignment.Left)
        {
            buffer[0] = ch;
            buffer[1..].Fill(' ');
        }
        else if (alignment == Alignment.Right)
        {
            buffer[..^1].Fill(' ');
            buffer[^1] = ch;
        }
        else // Center
        {
            int padding;
            // Odd width?
            if (width % 2 == 1)
            {
                padding = width / 2;
            }
            else // even
            {
                if (alignment.HasFlag(Alignment.Right)) // Right|Center?
                {
                    padding = width / 2;
                }
                else // Left|Center / Default|Center
                {
                    padding = (width / 2) - 1;
                }
            }
            buffer[..padding].Fill(' ');
            buffer[padding] = ch;
            buffer[(padding + 1)..].Fill(' ');
        }
        return this;
    }

    public TextFormatter Align(string? str, int width, Alignment alignment) 
        => Align(str.AsSpan(), width, alignment);

    public TextFormatter Align(scoped text text, int width, Alignment alignment)
    {
        int textLen = text.Length;
        if (textLen == 0)
        {
            _textBuffer.AllocateMany(width).Fill(' ');
            return this;
        }
        int spaces = width - textLen;
        if (spaces < 0)
            throw new ArgumentOutOfRangeException(nameof(width), width, $"Width must be {textLen} or greater");

        if (spaces == 0)
        {
            _textBuffer.AddMany(text);
            return this;
        }
        var appendSpan = _textBuffer.AllocateMany(width);
        if (alignment == Alignment.Left)
        {
            TextHelper.Unsafe.CopyBlock(text, appendSpan, textLen);
            appendSpan[textLen..].Fill(' ');
        }
        else if (alignment == Alignment.Right)
        {
            appendSpan[..spaces]
                .Fill(' ');
            TextHelper.Unsafe.CopyBlock(text, appendSpan[spaces..], textLen);
        }
        else // Center
        {
            int frontPadding;
            // Even spacing is easy split
            if (spaces % 2 == 0)
            {
                frontPadding = spaces / 2;
            }
            else // Odd spacing we have to align
            {
                if (alignment.HasFlag(Alignment.Right)) // Right|Center
                {
                    frontPadding = (int)Math.Ceiling(spaces / 2d);
                }
                else // Center or Left|Center 
                {
                    frontPadding = (int)Math.Floor(spaces / 2d);
                }
            }
            appendSpan[..frontPadding].Fill(' ');
            TextHelper.Unsafe.CopyBlock(text, appendSpan[frontPadding..], textLen);
            appendSpan[(frontPadding + textLen)..].Fill(' ');
        }
        return this;
    }
#endregion
    
    

    public TextFormatter CaptureWritten(Build build, out Span<char> written)
    {
        int pos = _textBuffer.Count;
        this.Build(build);
        written = _textBuffer[pos..];
        return this;
    }

    /// <summary>
    /// Context (indent) aware Build action!
    /// </summary>
    /// <param name="build"></param>
    /// <returns></returns>
    public TextFormatter Build(Build build)
    {
        if (!Indents.HasIndent)
        {
            build(this);
        }
        else
        {
            var oldIndents = this.Indents;
            using var newIndents = new IndentManager(this);
            this.Indents = new IndentManager(this);
            text newLine = Options.NewLine.AsSpan();
            int i = _textBuffer.AsSpan().PreviousIndexOf(newLine, _textBuffer.Count - 1, Options.StringComparison);
            if (i == -1)
            {
                this.Indents.AddIndent(_textBuffer.AsSpan());
            }
            else
            {
                this.Indents.AddIndent(_textBuffer[(i + newLine.Length)..]);
            }
            build(this);
            Interlocked.Exchange(ref oldIndents, this.Indents);
            oldIndents.Dispose();
        }
        return this;
    }
    
    
    
    public void Dispose()
    {
        _textBuffer.Dispose();
    }

    public string ToStringAndDispose()
    {
        string str = this.ToString();
        this.Dispose();
        return str;
    }

    public override string ToString()
    {
        return _textBuffer.AsSpan().ToString();
    }
}