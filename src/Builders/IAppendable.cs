namespace ScrubJay.Text.Builders;

[PublicAPI]
public interface IAppendable
{
    TBuilder AppendTo<TBuilder>(TBuilder textBuilder)
        where TBuilder : FluentTextBuilder<TBuilder>;
}