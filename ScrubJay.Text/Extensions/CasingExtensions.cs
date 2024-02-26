using System.Globalization;

namespace ScrubJay.Text.Extensions;

public static class CasingExtensions
{
    public static string ToCase(this string? text, Casing casing, CultureInfo? culture = null)
    {
        if (text is null) return string.Empty;
        int textLen = text.Length;
        if (textLen == 0) return string.Empty;
        TextInfo textInfo = (culture ?? CultureInfo.CurrentCulture).TextInfo;
        switch (casing)
        {
            case Casing.Lower:
            {
                return textInfo.ToLower(text);
            }
            case Casing.Upper:
            {
                return textInfo.ToUpper(text);
            }
            case Casing.Camel:
            {
                Span<char> buffer = stackalloc char[textLen];
                buffer[0] = textInfo.ToLower(text[0]);
                TextHelper.CopyTo(text.AsSpan(1), buffer.Slice(1));
                return buffer.ToString();
            }
            case Casing.Pascal:
            {
                Span<char> buffer = stackalloc char[textLen];
                buffer[0] = textInfo.ToUpper(text[0]);
                TextHelper.CopyTo(text.AsSpan(1), buffer.Slice(1));
                return buffer.ToString();
            }
//            case Casing.Title:
//            {
//                return textInfo.ToTitleCase(text);
//            }
            default:
                return text;
        }
    }

    public static string ToCasedString(
        this text text,
        Casing casing,
        CultureInfo? culture = null
    )
    {
        int textLen = text.Length;
        if (textLen == 0)
            return string.Empty;
        TextInfo textInfo = (culture ?? CultureInfo.CurrentCulture).TextInfo;
        switch (casing)
        {
            case Casing.Lower:
            {
                Span<char> buffer = stackalloc char[textLen];
                for (var i = textLen - 1; i >= 0; i--)
                {
                    buffer[i] = textInfo.ToLower(text[i]);
                }
                return buffer.ToString();
            }
            case Casing.Upper:
            {
                Span<char> buffer = stackalloc char[textLen];
                for (var i = textLen - 1; i >= 0; i--)
                {
                    buffer[i] = textInfo.ToUpper(text[i]);
                }
                return buffer.ToString();
            }
            case Casing.Camel:
            {
                Span<char> buffer = stackalloc char[textLen];
                buffer[0] = textInfo.ToLower(text[0]);
                TextHelper.CopyTo(text.Slice(1), buffer.Slice(1));
                return buffer.ToString();
            }
            case Casing.Pascal:
            {
                Span<char> buffer = stackalloc char[textLen];
                buffer[0] = textInfo.ToUpper(text[0]);
                TextHelper.CopyTo(text.Slice(1), buffer.Slice(1));
                return buffer.ToString();
            }
//            case Casing.Title:
//            {
//                // Have to allocate a string
//                return textInfo.ToTitleCase(text.ToString());
//            }
            default:
                return text.ToString();
        }
    }
    
    public static void Cased(this Span<char> chars, Casing casing, CultureInfo? culture = null)
    {
        int charCount = chars.Length;
        if (charCount == 0)
            return;
        TextInfo textInfo = (culture ?? CultureInfo.CurrentCulture).TextInfo;
        switch (casing)
        {
            case Casing.Lower:
            {
                for (var i = charCount - 1; i >= 0; i--)
                {
                    chars[i] = textInfo.ToLower(chars[i]);
                }
                return;
            }
            case Casing.Upper:
            {
                for (var i = charCount - 1; i >= 0; i--)
                {
                    chars[i] = textInfo.ToUpper(chars[i]);
                }
                return;
            }
            case Casing.Camel:
            {
                chars[0] = textInfo.ToLower(chars[0]);
                return;
            }
            case Casing.Pascal:
            {
                chars[0] = textInfo.ToUpper(chars[0]);
                return;
            }
        }
    }
}