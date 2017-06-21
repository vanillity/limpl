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
    bool MatchesUpTo(IScanner<char> chars, int k);
    T Lex(Scanner<char> chars);
    bool IsAllowedInOtherToken {get;} //isAllowedInTokenCluster
}

/*
public class TokenRule<T> : ITokenRule<T> where T : IToken
{
  
    public virtual bool IsAllowedInOtherToken => false;

    public virtual T CreateToken(IEnumerable<char> chars)
    {
        throw new NotImplementedException();
    }

    public virtual T Lex(Scanner<char> chars)
    {
        throw new NotImplementedException();
    }

    public bool MatchesUpTo(IScanner<char> chars,int k)
    {
        throw new NotImplementedException();
    }
}*/

}