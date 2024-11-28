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

    public static B AppendIf<B, T>(this B builder, bool condition, Action<B>? trueBuild, Action<B>? falseBuild = null)
        where B : FluentTextBuilder<B>
    {
        if (condition)
            return builder.Execute(trueBuild);
        else
            return builder.Execute(falseBuild);
    }

    public static B AppendIfOk<B, O, E>(this B builder, Result<O, E> result)
        where B : FluentTextBuilder<B>
    {
        return result.HasOk(out var ok) ? builder.Append<O>(ok) : builder;
    }

    public static B AppendIf<B, O, E>(this B builder, Result<O, E> result, Action<B, O>? okBuild, Action<B,E>? errorBuild = null)
        where B : FluentTextBuilder<B>
    {
        result.Match(
            ok => okBuild?.Invoke(builder, ok),
            error => errorBuild?.Invoke(builder, error));
        return builder;
    }

    public static B AppendIfSome<B, T>(this B builder, Option<T> option)
        where B : FluentTextBuilder<B>
    {
        return option.HasSome(out var some) ? builder.Append<T>(some) : builder;
    }

    public static B AppendIf<B, T>(this B builder, Option<T> option, Action<B, T>? someBuild)
        where B : FluentTextBuilder<B>
    {
        if (option.HasSome(out var some))
            someBuild?.Invoke(builder, some);
        return builder;
    }


    public static B ExecuteIf<B>(this B builder, bool condition, Action<B>? trueBuild, Action<B>? falseBuild = null)
        where B : FluentTextBuilder<B>
    {
        return condition ? builder.Execute(trueBuild) : builder.Execute(falseBuild);
    }
}