using ScrubJay.Text.Builders;

namespace ScrubJay.Text.Tests;

public class UberFluentTextBuilderTests
{

    [Fact]
    public void IndentsWork()
    {
        using var builder = new UberTextBuilder();
        builder.Append("Start")
            .Indented(
                "  ", b =>
                {
                    b.NewLine()
                        .AppendLine("level 1, A")
                        .Append("level 1, B");
                })
            .NewLine()
            .Append("End");

        const string output = """
            Start
               level1, A
               level1, B
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

        using var builder = new UberTextBuilder();

        builder.Indented("    ", b => b
            .Append(sample));

        var output = builder.ToString();

        Assert.Equal(sample_indented, output);
    }

    [Fact]
    public void PlaceholdersAreParsed()
    {
        string className = "Test";

        Action<UberTextBuilder> ba = tb => tb.Append("public void DoThing() => { };");

        using var builder = new UberTextBuilder();
        builder.Append($$"""
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

    private static void WriteBody(UberTextBuilder builder)
    {
        builder.Delimit(b => b.NewLine(), Enumerable.Range(0, 10), (b, i) => b.Append(i));
    }

     [Fact]
     public void ComplexInterpolationWorks()
     {
         using var builder = new UberTextBuilder();
         builder.Append($$"""
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