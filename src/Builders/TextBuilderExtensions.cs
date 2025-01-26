﻿using ScrubJay.Utilities;

#pragma warning disable CA1715

namespace ScrubJay.Text.Builders;

[PublicAPI]
public static class TextBuilderExtensions
{
    public static B AppendName<B>(this B builder, Type? type)
        where B : FluentTextBuilder<B>
        => builder.Append(type.NameOf());

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

    public static B AppendIf<B>(this B builder, bool condition, Action<B>? trueBuild, Action<B>? falseBuild = null)
        where B : FluentTextBuilder<B>
    {
        if (condition)
            return builder.Invoke(trueBuild);
        else
            return builder.Invoke(falseBuild);
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
        return condition ? builder.Invoke(trueBuild) : builder.Invoke(falseBuild);
    }

    public delegate void BuildEnumeratedSplitValue<B, T>(B builder, ReadOnlySpan<T> segment)
        where B : FluentTextBuilder<B>
        where T : IEquatable<T>;
    public delegate void BuildEnumeratedSplitValueIndex<B, T>(B builder, ReadOnlySpan<T> segment, int index)
        where B : FluentTextBuilder<B>
        where T : IEquatable<T>;


    public static B Enumerate<B, T>(this B builder, SpanSplitter<T> splitSpan, BuildEnumeratedSplitValue<B, T> buildValue)
        where B : FluentTextBuilder<B>
        where T : IEquatable<T>
    {
        while (splitSpan.MoveNext())
        {
            buildValue(builder, splitSpan.Current);
        }
        return builder;
    }

    public static B Iterate<B, T>(this B builder, SpanSplitter<T> splitSpan, BuildEnumeratedSplitValueIndex<B, T> buildValueIndex)
        where B : FluentTextBuilder<B>
        where T : IEquatable<T>
    {
        int i = 0;
        while (splitSpan.MoveNext())
        {
            buildValueIndex(builder, splitSpan.Current, i);
            i++;
        }
        return builder;
    }

    public static B Delimit<B, T>(this B builder, char delimiter, SpanSplitter<T> splitSpan, BuildEnumeratedSplitValue<B, T> buildValue)
        where B : FluentTextBuilder<B>
        where T : IEquatable<T>
    {
        if (!splitSpan.MoveNext())
            return builder;
        buildValue(builder, splitSpan.Current);
        while (splitSpan.MoveNext())
        {
            builder.Append(delimiter);
            buildValue(builder, splitSpan.Current);
        }

        return builder;
    }

    public static B Delimit<B, T>(this B builder, scoped text delimiter, SpanSplitter<T> splitSpan, BuildEnumeratedSplitValue<B, T> buildValue)
        where B : FluentTextBuilder<B>
        where T : IEquatable<T>
    {
        if (!splitSpan.MoveNext())
            return builder;
        buildValue(builder, splitSpan.Current);
        while (splitSpan.MoveNext())
        {
            builder.Append(delimiter);
            buildValue(builder, splitSpan.Current);
        }

        return builder;
    }

    public static B Delimit<B, T>(this B builder, Action<B> delimit, SpanSplitter<T> splitSpan, BuildEnumeratedSplitValue<B, T> buildValue)
        where B : FluentTextBuilder<B>
        where T : IEquatable<T>
    {
        if (!splitSpan.MoveNext())
            return builder;
        buildValue(builder, splitSpan.Current);
        while (splitSpan.MoveNext())
        {
            delimit(builder);
            buildValue(builder, splitSpan.Current);
        }

        return builder;
    }
}