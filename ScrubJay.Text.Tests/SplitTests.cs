﻿using ScrubJay.Text.Splitting;

namespace ScrubJay.Text.Tests;

public class SplitTests
{   
    public static IReadOnlyList<string?> TestStrings { get; } = new[]
    {
        (string?)null,
        "",
        "\r\n",
        "\t",
        "\tx ",
        "    ",
        "xxx",
        "  xxx  ",
        "\r\n\r\n",
        " \r\n \r\n ",
        " xx \r\n xx \r\n"
    };

    public static IReadOnlyList<string?> TestSeparators { get; } = new[]
    {
        (string?)null,
        "",
        " ",
        "x",
        "\r\n"
    };

    public static IReadOnlyList<TextSplitOptions> TestOptions { get; } = new[]
    {
        TextSplitOptions.None,
        TextSplitOptions.RemoveEmptyLines,
        TextSplitOptions.TrimLines,
        TextSplitOptions.RemoveEmptyLines | TextSplitOptions.TrimLines
    };

    public static IEnumerable<object?[]> CanSplitData()
    {
        foreach (var testSeparator in TestSeparators)
        foreach (var testString in TestStrings)
        foreach (var testOption in TestOptions)
        {
            yield return new object?[3] { testString, testSeparator, testOption };
        }
    }

    [Theory,MemberData(nameof(CanSplitData))]
    public void CanSplitText(string? input, string? separator, TextSplitOptions splitOptions)
    {
        // We're comparing to (string)input.Split(separator, splitOptions);
        // If you pass a `null` separator, it converts it to `""` (empty)
        // So I feel that letting a `null` input be treated as `""` is fair
        input ??= "";
        separator ??= "";

#if !NET6_0_OR_GREATER
        if ((int)splitOptions >= 2) return;
#endif
        
#if NETSTANDARD2_0 || NET48
        string[] stringSplit = input.Split(
            new string[1]{separator}, 
            (StringSplitOptions)splitOptions);
#else
        string[] stringSplit = input.Split(
            separator, 
            (StringSplitOptions)splitOptions);
#endif
        using var stringSplitEnumerator = ((IEnumerable<string>)stringSplit)
            .GetEnumerator();

        // Now our implementation
        var inputSpan = input.AsSpan();
        var separatorSpan = separator.AsSpan();

        // Have to be able to create an enumerator, _always_
        SplitTextEnumerable splitTextEnumerable = new(inputSpan, separatorSpan, splitOptions);
        SplitTextEnumerator splitTextEnumerator = splitTextEnumerable.GetEnumerator();
        // For debugging
        //var testSplitStrings = textSplitter.ListStrings();

        // They have to stay in sync
        while (true)
        {
            bool eStringMoved = stringSplitEnumerator.MoveNext();
            bool eTextMoved = splitTextEnumerator.MoveNext();
            Assert.Equal(eStringMoved, eTextMoved);

            // If they are different, test has failed, exit early
            // If they are false, we're done enumerating
            if (eTextMoved == false || eStringMoved == false) return;

            // Their values have to be exactly the same
            Assert.NotNull(stringSplitEnumerator.Current);
            string stringSplitString = stringSplitEnumerator.Current!;
            string textSplitString = splitTextEnumerator.Text.ToString()!;
            Assert.Equal(stringSplitString, textSplitString);
            if (stringSplitString != textSplitString) return;

            // The range has to be the correct range
            Range textSplitRange = splitTextEnumerator.Range;
            textSplitString = input[textSplitRange];
            Assert.Equal(stringSplitString, textSplitString);
        }
    }
}