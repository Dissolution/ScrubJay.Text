public interface IParse
{
    IParse Match(char ch);
    IParse Match(scoped text text);
    IParse Match(string text);

    IParse MatchWhile(Fun<char, bool> charPredicate);
    IParse MatchWhile(RSFun<char, bool> textPredicate);

    IParse MatchAny(params ReadOnlySpan<char> chars);
    IParse MatchAny(ICollection<char> chars);

    IParse Skip(int count);

    IParse Skip(Fun<char, bool> charPredicate);
}

public static class Parsable2
{

}