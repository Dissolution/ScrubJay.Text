namespace ScrubJay.Text.Tests.Benchmarks;

using Text.Benchmarks;

public class TextToStringBenchmarkTests
{
    public static TheoryData<string> Args => new()
    {
        { "" },
        { Environment.NewLine },
        { "EE0A1999-5F25-411B-AA2C-ACB40D6778C1" },
        { new string('x', 1000) },
    };
    
    public static TextToStringBenchmarks Benchmarks { get; } = new();

    [Theory]
    [MemberData(nameof(Args))]
    public void ROSpanToString(string str)
    {
        Assert.NotNull(str);
        var text = str.AsSpan();
        var output = Benchmarks.ROSpanToString(str);
        Assert.NotNull(output);
        Assert.Equal(text.Length, output.Length);
        Assert.Equal(str, output);
    }
    
#if !(NET48 || NETSTANDARD2_0)
    [Theory]
    [MemberData(nameof(Args))]
    public void NewStringFromROSpan(string str)
    {
        Assert.NotNull(str);
        var text = str.AsSpan();
        var output = Benchmarks.NewStringFromROSpan(str);
        Assert.NotNull(output);
        Assert.Equal(text.Length, output.Length);
        Assert.Equal(str, output);
    }
#endif
    
    [Theory]
    [MemberData(nameof(Args))]
    public void NewStringFromPointer(string str)
    {
        Assert.NotNull(str);
        var text = str.AsSpan();
        var output = Benchmarks.NewStringFromPointer(str);
        Assert.NotNull(output);
        Assert.Equal(text.Length, output.Length);
        Assert.Equal(str, output);
    }
}