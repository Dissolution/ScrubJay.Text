namespace ScrubJay.Text.Benchmarks;

using BenchmarkDotNet.Attributes;

public class TextToStringBenchmarks
{
    public static IEnumerable<object[]> Args()
    {
        yield return new object[1] { "" };
        yield return new object[1] { Environment.NewLine };
        yield return new object[1] { "EE0A1999-5F25-411B-AA2C-ACB40D6778C1" };
        yield return new object[1] { new string('x', 1000) };
    }


    [Benchmark]
    [ArgumentsSource(nameof(Args))]
    public string ROSpanToString(ReadOnlySpan<char> text)
    {
        return text.ToString();
    }
}