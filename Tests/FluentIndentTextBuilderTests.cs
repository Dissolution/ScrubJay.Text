﻿using ScrubJay.Text.Builders;

namespace ScrubJay.Text.Tests;

public class FluentIndentTextBuilderTests
{
    [Fact]
    public void StartAndEndIndentWorks()
    {
        const string output = """
        L1
        L2L3
        ----L4
        ----L5L6
        L7
        L8
        """;

        var text = FluentIndentTextBuilder.New
            .Append("L1")
            .NewLine()
            .Append("L2")
            .AddIndent("----")
            .Append("L3")
            .NewLine()
            .Append("L4")
            .NewLine()
            .Append("L5")
            .RemoveIndent(out var indent)
            .Append("L6")
            .NewLine()
            .Append("L7")
            .NewLine()
            .Append("L8")
            .ToStringAndDispose();

        Assert.Equal("----", indent);
        Assert.Equal(output, text);
    }


    [Fact]
    public void IndentsWork()
    {
        using var builder = new FluentIndentTextBuilder();
        builder.Append("Start")
            .Indented(
                "   ", b =>
                {
                    b.NewLine()
                        .AppendLine("level 1, A")
                        .Append("level 1, B");
                })
            .NewLine()
            .Append("End");

        const string output = """
            Start
               level 1, A
               level 1, B
            End
            """;

        Assert.Equal(output, builder.ToString());
    }


    [Fact]
    public void NewLinesAreParsedInText()
    {
        const string sample = """
            Come with me,
            into the trees.
            We lay on the grass,
            and let hours pass.
            """;

        const string sample_indented = """
            Come with me,
                into the trees.
                We lay on the grass,
                and let hours pass.
            """;

        using var builder = new FluentIndentTextBuilder();

        builder.Indented(
            "    ", b => b
                .Append(sample));

        var output = builder.ToString();

        Assert.Equal(sample_indented, output);
    }

    [Fact]
    public void PlaceholdersAreParsed()
    {
        string className = "Test";

        Action<FluentIndentTextBuilder> ba = tb => tb.Append("public void DoThing() => { };");

        using var builder = new FluentIndentTextBuilder();
        builder.Append(
            $$"""
            public void {{className}}()
            {
                {{ba}}
            }
            """);

        string output = """
            public void Test()
            {
                public void DoThing() => { };
            }
            """;
        Assert.Equal(output, builder.ToString());
    }

    private static void WriteBody(FluentIndentTextBuilder builder)
    {
        builder.Delimit(b => b.NewLine(), Enumerable.Range(0, 10), (b, i) => b.Append(i));
    }

    [Fact]
    public void ComplexInterpolationWorks()
    {
        using var builder = new FluentIndentTextBuilder();
        builder.Append(
            $$"""
            public class TestClass()
            {
                public void DoThing()
                {
                    {{WriteBody}}
                }
            }
            """);

        const string output = $$"""
            public class TestClass()
            {
                public void DoThing()
                {
                    0
                    1
                    2
                    3
                    4
                    5
                    6
                    7
                    8
                    9
                }
            }
            """;

        Assert.Equal(output, builder.ToString());
    }
}