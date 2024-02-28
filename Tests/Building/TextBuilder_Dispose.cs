namespace ScrubJay.Text.Tests.Building;

using Text.Building;

public class TextBuilder_Dispose
{
    [Fact]
    public void CanDispose()
    {
        TextBuilder? textBuilder = TextBuilder.Borrow();
        textBuilder.Dispose();
    }

    [Fact]
    public void CanDisposeMany()
    {
        using TextBuilder textBuilder = TextBuilder.Borrow();
        // ReSharper disable DisposeOnUsingVariable
        textBuilder.Dispose();
        textBuilder.Dispose();
        textBuilder.Dispose();
        // ReSharper restore DisposeOnUsingVariable
    }
}

public class TextBuilder_IsEnumerable
{
    
}

public class TextBuilder_IsReadOnlyCollection
{
    public static void HasCount()
    {
        using var text = TextBuilder.Borrow();
        int count = ((IReadOnlyCollection<char>)text).Count;
        Assert.Equal(0, count);
    }
}


public class TextBuilder_Append
{
    public static IEnumerable<object[]> AllCharacters()
    {
        for (int i = char.MinValue; i <= char.MaxValue; i++)
        {
            yield return new object[1] { (char)i };
        }
    }
    
    [Theory]
    [MemberData(nameof(AllCharacters))]
    public void CanAppendChar(char ch)
    {
        using var text = TextBuilder.Borrow();
        text.Append(ch);
        Assert.Equal(1, text.Length);
    }
}