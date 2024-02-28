namespace ScrubJay.Text.Tests;

internal static class TestHelper
{
    public static IEnumerable<object[]> AsMemberData<T>(params T[] values)
        where T : notnull
        => values.Select(static t => new object[1] { t });
    
    public static IEnumerable<object[]> AsMemberData<T>(IEnumerable<T> values)
        where T : notnull
        => values.Select(static t => new object[1] { t });
}