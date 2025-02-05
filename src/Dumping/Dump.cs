using System.Reflection;
using ScrubJay.Utilities;

namespace ScrubJay.Text.Dumping;

public static class Dump
{
    private static B DumpMember<B>(this B builder, MemberInfo? member, bool includeOwner = false)
        where B : FluentIndentTextBuilder<B>
    {
        return member switch
        {
            null => builder,
            EventInfo eventInfo => builder
                .AppendType(eventInfo.EventHandlerType)
                .Append(' ')
                .AppendIf(includeOwner, b => b.AppendType(eventInfo.DeclaringType).Append('.'))
                .Append(eventInfo.Name),
            ConstructorInfo constructorInfo => builder
                .AppendType(constructorInfo.DeclaringType)
                .Append('(').Delimit(", ", constructorInfo.GetParameters(), (b, p) => b.AppendType(p.ParameterType)).Append(')'),
            FieldInfo fieldInfo => builder
                .AppendType(fieldInfo.FieldType)
                .Append(' ')
                .AppendIf(includeOwner, b => b.AppendType(fieldInfo.DeclaringType).Append('.'))
                .Append(fieldInfo.Name),
            MethodInfo methodInfo => builder
                .AppendType(methodInfo.ReturnType)
                .Append(' ')
                .AppendIf(includeOwner, b => b.AppendType(methodInfo.DeclaringType).Append('.'))
                .Append(methodInfo.Name)
                .Append('(').Delimit(", ", methodInfo.GetParameters(), (b, p) => b.AppendType(p.ParameterType)).Append(')'),
            PropertyInfo propertyInfo => builder
                .AppendType(propertyInfo.PropertyType)
                .Append(' ')
                .AppendIf(includeOwner, b => b.AppendType(propertyInfo.DeclaringType).Append('.'))
                .Append(propertyInfo.Name),
            Type type => builder.AppendType(type),
            _ => throw new ArgumentOutOfRangeException(nameof(member)),
        };
    }

    private static B DumpComplexValue<B>(this B builder, Type valueType, object value)
        where B : FluentIndentTextBuilder<B>
    {
        if (!valueType.IsClass)
            Debugger.Break();

        return DumpProperties(builder, valueType, value);
    }

    private static B DumpProperties<B>(this B builder, Type valueType, object value)
        where B : FluentIndentTextBuilder<B>
    {
        // public instance members
        var members = valueType
            .GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .OfType<PropertyInfo>()
            .ToList();

        var typeName = valueType.NameOf();

        return builder
            .Append(typeName)
            .Append(": ")
            .Append(value)
            .InvokeIf(
                members.Count > 0, mb => mb.Block(
                    pb => pb.Delimit(b => b.NewLine(),
                        members, (b, prop) =>
                        {
                            var pValue = Result.TryFunc(() => prop.GetValue(value)).Match<object?>(ok => ok, ex => ex);
                            b.Append(prop.PropertyType.NameOf())
                                .Append(' ')
                                .Append(prop.Name)
                                .Append(": ")
                                .DumpValue(pValue);
                        })));
    }

    private static B DumpValue<B>(this B builder, object? value, bool allowComplex = true)
        where B : FluentIndentTextBuilder<B>
    {
        if (value is null)
            return builder.Append("null");
        Type valueType = value.GetType();

        // Have to check for Enum here, as its TypeCode is its underlying Type
        if (valueType.IsEnum)
        {
            var enumName = Enum.GetName(valueType, value);
            return builder.Append(valueType.NameOf()).Append('.').Append(enumName);
        }

        var typeCode = Type.GetTypeCode(valueType);
        switch (typeCode)
        {
            case TypeCode.Empty:
                return builder.Append("null");
            case TypeCode.DBNull:
                return builder.Append("DBNull");
            case TypeCode.Boolean:
                bool boolean = Notsafe.Unbox<bool>(value!);
                return builder.Append(boolean ? "true" : "false");
            case TypeCode.Char:
                char ch = Notsafe.Unbox<char>(value!);
                return builder.Append('\'').Append(ch).Append('\'');
            case TypeCode.SByte:
                sbyte sb = Notsafe.Unbox<sbyte>(value!);
                return builder
                    .Append('(')
                    .Append(valueType!.NameOf())
                    .Append(')')
                    .Append(sb);
            case TypeCode.Byte:
                byte b = Notsafe.Unbox<byte>(value!);
                return builder
                    .Append('(')
                    .Append(valueType!.NameOf())
                    .Append(')')
                    .Append(b);
            case TypeCode.Int16:
                short s = Notsafe.Unbox<short>(value!);
                return builder
                    .Append('(')
                    .Append(valueType!.NameOf())
                    .Append(')')
                    .Append(s);
            case TypeCode.UInt16:
                ushort us = Notsafe.Unbox<ushort>(value!);
                return builder
                    .Append('(')
                    .Append(valueType!.NameOf())
                    .Append(')')
                    .Append(us);
            case TypeCode.Int32:
                int i = Notsafe.Unbox<int>(value!);
                return builder.Append<int>(i);
            case TypeCode.UInt32:
                uint ui = Notsafe.Unbox<uint>(value!);
                return builder.Append(ui).Append('U');
            case TypeCode.Int64:
                long l = Notsafe.Unbox<long>(value!);
                return builder.Append(l).Append('L');
            case TypeCode.UInt64:
                ulong ul = Notsafe.Unbox<ulong>(value!);
                return builder.Append(ul).Append("UL");
            case TypeCode.Single:
                float f = Notsafe.Unbox<float>(value!);
                return builder.Format(f, "N").Append('f');
            case TypeCode.Double:
                double d = Notsafe.Unbox<double>(value!);
                return builder.Format(d, "N").Append('m');
            case TypeCode.Decimal:
                decimal m = Notsafe.Unbox<decimal>(value!);
                return builder.Format(m, "N").Append('m');
            case TypeCode.DateTime:
                DateTime dt = Notsafe.Unbox<DateTime>(value!);
                return builder.Format(dt, "yyyy-MM-dd HH:mm:ss");
            case TypeCode.String:
                string str = Notsafe.CastClass<string>(value!);
                return builder.Append('"').Append(str).Append('"');
            case TypeCode.Object:
            default:
            {
                break;
            }
        }

        if (value is TimeSpan timeSpan)
        {
            return builder.Format(timeSpan, "c");
        }
        else if (value is Guid guid)
        {
            return builder.Format(guid, "D");
        }
        else if (value is Type type)
        {
            return builder.Append(type.NameOf());
        }
        else if (value is MemberInfo member)
        {
            return builder.DumpMember(member, true);
        }
        else if (value is Exception ex)
        {
            return builder.DumpProperties(valueType, ex);
        }
        else if (value is IList list)
        {
            return builder.Append('[').Delimit(", ", list.OfType<object?>(), static (b, i) => DumpValue(b, i)).Append(']');
        }
#if !NETSTANDARD2_0
        else if (value is ITuple tuple)
        {
            return builder.Append('(')
                .Delimit(", ", Enumerable.Range(0, tuple.Length), (b, i) => b.DumpValue(tuple[i]))
                .Append(')');
        }
#endif

        if (allowComplex)
            return DumpComplexValue(builder, valueType, value);
        return builder.Append(value);
    }

    public static string Value<T>(T? value)
    {
        using var code = new FluentIndentTextBuilder();
        code.DumpValue(value);
        return code.ToString();
    }
}