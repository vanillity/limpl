using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limpl.Parsing
{
public class TokenRuleList<TRule, TToken>  : TokenSourceList<TRule, TToken> where TRule : Limpl.ITokenRule<TToken> where TToken : IToken
{

    public TokenRuleList() : base(new TRule[0]) { }

    internal TokenRuleList(IEnumerable<TRule> matches) : base(matches)
    {
        foreach(var m in matches)
            InnerList.Add(m);
    }

    public TokenRuleList<TRule, TToken> Matches(IScanner<char> chars, int k)
    {
        return new TokenRuleList<TRule, TToken>(InnerList.Where(_=>_.MatchesUpTo(chars,k)));
    }

    protected TokenRuleList<TRule, TToken> Add(string tokenText, ITokenKind kind = null)
    { 
       throw new NotImplementedException();
    }

    public TRule AddPattern(string pattern, ITokenKind kind = null)
    { 
       throw new NotImplementedException();
    }

    //                        (iscanner, positionFromStart) => maxPositionChecked
    public TRule Add(Func<IScanner<char>,int,int> f, Func<Scanner<char>,TToken> lex) 
    { 
       throw new NotImplementedException();
    }

}

public abstract class TokenSourceList<TSource, TToken> : IReadOnlyList<TSource> where TSource : Limpl.ITokenSource<TToken> where TToken : IToken
{
    protected ImmutableList<TSource> InnerList {get;} = ImmutableList<TSource>.Empty;
    public TokenSourceList(IEnumerable<TSource> src) => InnerList = InnerList.AddRange(src);

    public TSource this[int index]
    {
        get
        {
            return InnerList[index];
        }

    }

    public int  Count => InnerList.Count;
    public bool IsReadOnly => false;

   
    IEnumerator IEnumerable.GetEnumerator()
    {
        return InnerList.GetEnumerator();
    }

    public IEnumerator<TSource> GetEnumerator()
    {
        return InnerList.GetEnumerator();
    }
}
}
