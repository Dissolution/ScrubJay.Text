namespace ScrubJay.Text.Utilities;

using Comparison;

/// <summary>
/// Provides a way to declare a method with a <see cref="FormattableString"/> argument alongside a
/// similar method with a <see cref="string"/> argument and not have method resolution always pick
/// the <see cref="string"/> method.
/// </summary>
/// <remarks>
/// With code such as:<br/>
/// <c>void DoThing(string str)</c><br/>
/// <c>void DoThing(FormattableString formattedString)</c><br/>
/// The <c>DoThing(string)</c> will be chosen when calling <c>DoThing($"...")</c>, <b>NOT</b> <c>DoThing(FormattableString)</c><br/>
/// due to the way overload resolution works.<br/>
/// Instead, use:<br/>
/// <c>void DoThing(NonFormattableString str)</c><br/>
/// <c>void DoThing(FormattableString formattedString)</c><br/>
/// Then calls to <c>DoThing($"...")</c> will convert to <see cref="FormattableString"/><br/>
/// and calls to <c>DoThing("...")</c> will capture the <see cref="string"/> in a <see cref="NonFormattableString"/><br/>
/// <br/>
/// This is primarily used for backwards compatability, as InterpolatedStringHandlers have largely replaced its necessity<br/>
/// as they can easily coexist:<br/>
/// <c>void DoThing(string str)</c><br/>
/// <c>void DoThing(ref DefaultInterpolatedStringHandler interpolatedString)</c><br/>
/// </remarks>
public readonly struct NonFormattableString
{
    public static implicit operator NonFormattableString(string? str) => new NonFormattableString(str);
    public static implicit operator NonFormattableString(FormattableString _) => throw new InvalidOperationException();
    public static implicit operator NonFormattableString(text _) => throw new InvalidOperationException();

    private readonly string? _str;

    /// <summary>
    /// Gets the captured <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see>
    /// </summary>
    public text Text => _str.AsSpan();
    
    /// <summary>
    /// Gets the captured <see cref="string">string?</see>
    /// </summary>
    public string? String => _str;

    private NonFormattableString(string? str)
    {
        _str = str;
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            NonFormattableString nfs => Relate.Equal.Text(_str, nfs._str),
            string str => Relate.Equal.Text(_str, str),
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return _str?.GetHashCode() ?? 0;
    }

    public override string ToString()
    {
        return _str ?? string.Empty;
    }
}