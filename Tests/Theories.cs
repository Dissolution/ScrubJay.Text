namespace ScrubJay.Text.Tests;

public static class Theories
{
    public static TheoryData<string?> Strings { get; } = new()
    {
        "",
        ",",
        "\r\n",
        Guid.NewGuid().ToString(),
        new string('*', 1000),
    };
}