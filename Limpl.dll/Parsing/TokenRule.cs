using System;
using System.Collections.Generic;

namespace Limpl
{
public interface ITokenSource<T>  where T : IToken
{
    T CreateToken(IEnumerable<char> chars);
}

public interface ITokenRule<T> : ITokenSource<T> where T : IToken
{
    /// <summary>Returns true if the token definition matches the input up to 
    /// the given k-lookahead.</summary>
    bool MatchesUpTo(IReadOnlyScanner<char> chars, int k);
    T Lex(IScanner<char> chars);
    bool IsAllowedInOtherToken {get;} //isAllowedInTokenCluster
}


public class TokenRule<T> : ITokenRule<T> where T : IToken
{
    private readonly Func<IReadOnlyScanner<char>,int,bool> matchesUpTo;
    private readonly Func<TokenRule<T>,IScanner<char>,T> lex;
    private readonly Func<IEnumerable<char>,T> createToken;

    protected TokenRule(ITokenKind kind, Func<IReadOnlyScanner<char>,int,bool> matchesUpTo, Func<TokenRule<T>,IScanner<char>,T> lex, Func<IEnumerable<char>,T> createToken ,bool allowedInOtherToken = false) 
    {
        this.matchesUpTo = matchesUpTo;
        this.lex = lex;
        this.createToken = createToken;
        IsAllowedInOtherToken = allowedInOtherToken;    
    }
  
    public virtual bool IsAllowedInOtherToken {get;}

    public virtual T CreateToken(IEnumerable<char> chars)
    {
        return createToken(chars);
    }

    public virtual T Lex(IScanner<char> chars)
    {
        return lex(this,chars);
    }

    public bool MatchesUpTo(IReadOnlyScanner<char> chars,int k)
    {
        return matchesUpTo(chars,k);
    }
}

}