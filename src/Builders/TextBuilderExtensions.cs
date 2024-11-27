using JetBrains.Annotations;

namespace ScrubJay.Text.Builders;

[PublicAPI]
public static class TextBuilderExtensions
{
    public static B AppendIf<B, T>(this B builder, bool condition, T trueValue)
        where B : FluentTextBuilder<B>
    {
        if (condition)
            return builder.Append<T>(trueValue);
        else
            return builder;
    }

    public static B AppendIf<B, T>(this B builder, bool condition, T trueValue, T falseValue)
        where B : FluentTextBuilder<B>
    {
        if (condition)
            return builder.Append<T>(trueValue);
        else
            return builder.Append<T>(falseValue);
    }

    public static B AppendIfOk<B, O, E>(this B builder, Result<O, E> result)
        where B : FluentTextBuilder<B>
    {
        return result.HasOk(out var ok) ? builder.Append<O>(ok) : builder;
    }

    public static B AppendIfSome<B, T>(this B builder, Option<T> option)
        where B : FluentTextBuilder<B>
    {
        return option.HasSome(out var some) ? builder.Append<T>(some) : builder;
    }
}