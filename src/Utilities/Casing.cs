using System.Globalization;

namespace ScrubJay.Text.Utilities;

public enum Case
{
    None,
    Lower,
    Upper,
    Camel,
    Pascal,
    Title,
}

public static class Casing
{
    public static string ToCasedString(this scoped text text, Case @case)
    {
        int textLen = text.Length;
        if (textLen == 0) return string.Empty;
        switch (@case)
        {
            case Case.Lower:
            {
                Span<char> buffer = stackalloc char[textLen];
                for (var i = 0; i < textLen; i++)
                {
                    buffer[i] = char.ToLower(text[i]);
                }
                return buffer.AsString();
            }
            case Case.Upper:
            {
                Span<char> buffer = stackalloc char[textLen];
                for (var i = 0; i < textLen; i++)
                {
                    buffer[i] = char.ToUpper(text[i]);
                }
                return buffer.AsString();
            }
            case Case.Camel:
            {
                Span<char> buffer = stackalloc char[textLen];
                buffer[0] = char.ToLower(text[0]);
                TextHelper.Unsafe.CopyBlock(text[1..], buffer[1..], textLen - 1);
                return buffer.AsString();
            }
            case Case.Pascal:
            {
                Span<char> buffer = stackalloc char[textLen];
                buffer[0] = char.ToUpper(text[0]);
                TextHelper.Unsafe.CopyBlock(text[1..], buffer[1..], textLen - 1);
                return buffer.AsString();
            }
            case Case.Title:
            {
                var str = text.AsString();
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
            }
            case Case.None:
            default:
                return text.AsString();
        }
    }
    
    [return: NotNullIfNotNull(nameof(str))]
    public static string? ToCasedString(this string? str, Case @case)
    {
        if (str is null) return null;
        int strLen = str.Length;
        if (strLen == 0) return string.Empty;
        switch (@case)
        {
            case Case.Lower:
            {
                Span<char> buffer = stackalloc char[strLen];
                for (var i = 0; i < strLen; i++)
                {
                    buffer[i] = char.ToLower(str[i]);
                }
                return buffer.AsString();
            }
            case Case.Upper:
            {
                Span<char> buffer = stackalloc char[strLen];
                for (var i = 0; i < strLen; i++)
                {
                    buffer[i] = char.ToUpper(str[i]);
                }
                return buffer.AsString();
            }
            case Case.Camel:
            {
                Span<char> buffer = stackalloc char[strLen];
                buffer[0] = char.ToLower(str[0]);
                TextHelper.Unsafe.CopyBlock(str[1..], buffer[1..], strLen - 1);
                return buffer.AsString();
            }
            case Case.Pascal:
            {
                Span<char> buffer = stackalloc char[strLen];
                buffer[0] = char.ToUpper(str[0]);
                TextHelper.Unsafe.CopyBlock(str[1..], buffer[1..], strLen - 1);
                return buffer.AsString();
            }
            case Case.Title:
            {
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
            }
            case Case.None:
            default:
                return str;
        }
    }
}