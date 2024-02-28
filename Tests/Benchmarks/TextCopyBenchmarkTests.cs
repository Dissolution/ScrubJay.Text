namespace ScrubJay.Text.Tests.Benchmarks;

using Text.Benchmarks;

public class TextCopyBenchmarkTests
{
    public static TheoryData<string, char[]> Args => new()
    {
        { "", new char[1024] },
        { Environment.NewLine, new char[1024] },
        { "EE0A1999-5F25-411B-AA2C-ACB40D6778C1", new char[1024] },
        { new string('x', 1000), new char[1024] },
    };

    public static TextCopyBenchmarks Benchmarks { get; } = new TextCopyBenchmarks();
    
    
    [Theory]
    [MemberData(nameof(Args))]
    public void StringCopyTo(string str, char[] array)
    {
        Assert.NotNull(str);
        Assert.NotNull(array);
        Assert.True(str.Length <= array.Length);
        Benchmarks.StringCopyTo(str, array);
        Assert.Equal(str, new string(array, 0, str.Length));
        if (array.Length > str.Length)
        {
            Assert.True(array[str.Length] == default);
        }
    }
    
    [Theory]
    [MemberData(nameof(Args))]
    public void SpanCopyTo(string str, char[] array)
    {
        Assert.NotNull(str);
        Assert.NotNull(array);
        Assert.True(str.Length <= array.Length);
        Benchmarks.SpanCopyTo(str, array);
        Assert.Equal(str, new string(array, 0, str.Length));
        if (array.Length > str.Length)
        {
            Assert.True(array[str.Length] == default);
        }
    }
    
    [Theory]
    [MemberData(nameof(Args))]
    public void UnsafeCopyBlockVoid(string str, char[] array)
    {
        Assert.NotNull(str);
        Assert.NotNull(array);
        Assert.True(str.Length <= array.Length);
        Benchmarks.UnsafeCopyBlockVoid(str, array);
        Assert.Equal(str, new string(array, 0, str.Length));
        if (array.Length > str.Length)
        {
            Assert.True(array[str.Length] == default);
        }
    }
    
    [Theory]
    [MemberData(nameof(Args))]
    public void EmitCopyBlock(string str, char[] array)
    {
        Assert.NotNull(str);
        Assert.NotNull(array);
        Assert.True(str.Length <= array.Length);
        Benchmarks.EmitCopyBlock(str, array);
        Assert.Equal(str, new string(array, 0, str.Length));
        if (array.Length > str.Length)
        {
            Assert.True(array[str.Length] == default);
        }
    }
}