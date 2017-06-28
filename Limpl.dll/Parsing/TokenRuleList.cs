using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limpl
{
public class TokenRuleList<TRule, TToken>  : TokenSourceList<TRule, TToken> where TRule : Limpl.ITokenRule<TToken> where TToken : IToken
{

    public TokenRuleList() : base(new TRule[0]) { }

    internal TokenRuleList(IEnumerable<TRule> matches) : base(matches)
    {
        if (matches==null)
            return;

        foreach(var m in matches)
            InnerList.Add(m);
    }

    public TokenRuleList<TRule, TToken> Matches(IScanner<char> chars, int k)
    {
        return new TokenRuleList<TRule, TToken>(InnerList.Where(_=>_.MatchesUpTo(chars,k)));
    }

    public TokenRuleList<TRule, TToken> Add(params TRule[] rules)
    {
        return new TokenRuleList<TRule, TToken>(InnerList.Concat(rules));
    }

    public TokenRuleList<TRule, TToken> Remove(params TRule[] rules)
    {
        return new TokenRuleList<TRule, TToken>(InnerList.Except(rules));
    }
}

public abstract class TokenSourceList<TSource, TToken> : IReadOnlyList<TSource> where TSource : Limpl.ITokenSource<TToken> where TToken : IToken
{
    protected ImmutableList<TSource> InnerList {get;} = ImmutableList<TSource>.Empty;
    public TokenSourceList(IEnumerable<TSource> src) 
    {
        if (src != null)
            InnerList = InnerList.AddRange(src);    
    }

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
