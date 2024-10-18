using ScrubJay.Functional;

namespace ScrubJay.Text.Tests;

public class WhitespaceManagerTests
{


    [Fact]
    public void StartEndSlicesWork()
    {
        using var whitespace = new WhitespaceManager();
        
        string[] indents = ["\t", Environment.NewLine, "    "];

        foreach (var indent in indents)
        {
            var started = whitespace.TryStartIndent(indent.AsSpan());
            Assert.True(started.IsOk());
        }

        Result<string, Exception> ended;
        
        for (var i = 0; i < indents.Length; i++)
        {
            ended = whitespace.TryEndIndent();
            Assert.True(ended.IsOk(out var indent));
            Assert.Equal(indents[^(i + 1)], indent);
        }

        ended = whitespace.TryEndIndent();
        Assert.True(ended.IsError());
    }
}