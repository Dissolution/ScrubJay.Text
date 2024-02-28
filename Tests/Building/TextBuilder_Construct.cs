using ScrubJay.Text.Building;

namespace ScrubJay.Text.Tests.Building;

using System.Reflection;

public class TextBuilder_Construct
{
    [Fact]
    public void CannotConstructWithoutMethods()
    {
        Assert.Throws<MissingMethodException>(static () => Activator.CreateInstance<TextBuilder>());
        var constructors = typeof(TextBuilder).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        Assert.Empty(constructors);
    }

    [Fact]
    public void CanBorrow()
    {
        TextBuilder? textBuilder = TextBuilder.Borrow();
        Assert.NotNull(textBuilder);
    }
}