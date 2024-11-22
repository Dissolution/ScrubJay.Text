// using ScrubJay.Utilities;
//
// namespace ScrubJay.Text;
//
// [PublicAPI]
// [MustDisposeResource]
// public sealed class WhitespaceManager : IDisposable
// {
//     public const string DefaultIndent = "    "; // 4 spaces
//     public static readonly string DefaultNewLine = Environment.NewLine;
//
//     private string _newLine = DefaultNewLine;
//     private readonly PooledList<char> _indents = new();
//     private readonly Stack<int> _indentOffsets = new();
//     private readonly Stack<int> _blockOffsets = new();
//
//     /// <summary>
//     /// Gets or sets the current <see cref="string"/> that represents starting a new line
//     /// </summary>
//     /// <exception cref="ArgumentException"></exception>
//     [AllowNull, NotNull]
//     public string NewLine
//     {
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         get => _newLine;
//         set => _newLine = value ?? "";
//     }
//
//     public Option<char> HasOneCharNewLine => _newLine.Length == 1 ? Some(_newLine[0]) : None();
//
//     public text CurrentIndent
//     {
//         get
//         {
//             // If we're not in a block
//             if (!_blockOffsets.TryPeek(out var lastBlockStart))
//                 return _indents.Written;
//             return _indents.Written.Slice(lastBlockStart);
//         }
//     }
//
//     public string CurrentIndentString => CurrentIndent.ToString();
//
//     public Result<Unit, Exception> TryStartIndent(text indent)
//     {
//         var v = Validate.IsNotEmpty(indent);
//         if (!v)
//             return v;
//
//         int position = _indents.Count;
//         _indentOffsets.Push(position);
//         _indents.AddMany(indent);
//         return Unit();
//     }
//
//     public void StartIndent(text indent) => TryStartIndent(indent).OkOrThrow();
//
//     public void StartIndent(string indent) => TryStartIndent(indent.AsSpan()).OkOrThrow();
//
//     public Result<string, Exception> TryEndIndent()
//     {
//         if (_indentOffsets.Count == 0)
//             return new InvalidOperationException("There are no indents to remove");
//
//         var indentStart = _indentOffsets.Peek(); // will succeed
//         Debug.Assert(indentStart >= 0 && indentStart <= _indents.Count);
//
//         if (_blockOffsets.TryPeek(out var lastBlockStart) && indentStart < lastBlockStart)
//             return new InvalidOperationException("Remaining indents belong to previous block");
//
//         // Remove the indent
//         _indentOffsets.Pop(); // will succeed
//         // capture that indent to return
//         string str = _indents.Written.Slice(indentStart).ToString();
//         // set the indents length lower to remove the indent
//         _indents.Count = indentStart;
//         return str;
//     }
//
//     public void EndIndent() => TryEndIndent().OkOrThrow();
//
//     public IDisposable NewIndent(text indent)
//     {
//         TryStartIndent(indent).OkOrThrow();
//         return Disposable.Action(() => TryEndIndent().OkOrThrow());
//     }
//
//     public void StartBlock()
//     {
//         int position = _indents.Count;
//         _blockOffsets.Push(position);
//     }
//
//     public void EndBlock() => TryEndIndent().OkOrThrow();
//
//     public Result<Unit, Exception> TryEndBlock(bool allowIndentTrimming = false)
//     {
//         if (!_blockOffsets.TryPeek(out var blockStart))
//             return new InvalidOperationException("There are no blocks to remove");
//
//         Debug.Assert(blockStart >= 0 && blockStart <= _indents.Count);
//
//         // Do we have any indents to discard?
//         while (_indentOffsets.TryPeek(out int indentOffset))
//         {
//             if (indentOffset < blockStart)
//                 break;
//             if (allowIndentTrimming)
//             {
//                 _indentOffsets.Pop();
//             }
//             else
//             {
//                 return new InvalidOperationException("There are hanging indents");
//             }
//         }
//
//         _blockOffsets.Pop(); // will succeed
//         _indents.Count = blockStart;
//         return Unit();
//     }
//
//     public IDisposable NewBlock()
//     {
//         StartBlock();
//         return Disposable.Action(() => EndBlock());
//     }
//
//     public void Dispose()
//     {
//         _indentOffsets.Clear();
//         _indents.Dispose();
//     }
// }