// namespace ScrubJay.Text;
//
// public sealed class Whitespace
// {
//     public static readonly string DefaultNewLine = Environment.NewLine;
//     public static readonly string DefaultIndent = "    "; // 4 spaces
//     
//     
//     public string NewLine { get; set; } = DefaultNewLine;
//     public bool MatchPartialNewLine { get; set; } = true;
//
//     public int NextNewLine(text text)
//     {
//         int index;
//         if (MatchPartialNewLine)
//         {
//             index = text.IndexOfAny(NewLine);
//         }
//         else
//         {
//             index = text.IndexOf(NewLine);
//         }
//
//         return index;
//     }
//     
// }