using ScrubJay.Text.Builders;

namespace ScrubJay.Text.Tests;

public class UberFluentTextBuilderTests
{


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

        UberTextBuilder.BuilderAction ba = tb => tb.Append("public void DoThing() => { };");
        
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
        builder.Append("""
        return Console.WriteLine("Hello, World!");
        """);
    }

//     [Fact]
//     public void ComplexInterpolationWorks()
//     {
//         using var builder = new UberTextBuilder();
//         builder.Append($$"""
//         public class TestClass()
//         {
//             public void DoThing()
//             {
//                 {{WriteBody}}
//             }
//         }
//         """);
//     }
}