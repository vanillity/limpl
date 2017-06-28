using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Limpl
{
public abstract class NumericLiteralRule<TToken> : ITokenRule<TToken> where TToken : IToken
{
    bool alreadyHasDecimalPoint = false;

    public virtual bool IsAllowedInOtherToken => true;

    public abstract TToken CreateToken(string chars, double value, int position);

    public TToken Lex(IScanner<char> chars)
    {
        Debug.Assert(char.IsDigit(chars.Current));
        alreadyHasDecimalPoint = false;
        var position = chars.Position+1;
        var sb = new StringBuilder();
                
        while (!chars.End && (char.IsDigit(chars.Current) || !alreadyHasDecimalPoint && chars.Current=='.' && char.IsDigit(chars.Next)))
        {            
            if (chars.Current=='.')
                alreadyHasDecimalPoint = true;         

            sb.Append(chars.Consume());
        }

        var text  = sb.ToString();
        var value = alreadyHasDecimalPoint ? double.Parse(text) : int.Parse(text);

        var t = CreateToken(sb.ToString(), value, position);
        return t;
    }

    public bool MatchesUpTo(IReadOnlyScanner<char> chars,int k)
    {
        if (k==0)
            alreadyHasDecimalPoint = false; //reset
            
        var c = chars.LookAhead(k);
        var isDigit = char.IsDigit(c);

        if (isDigit)
            return true;

        var isDecimalPoint = (c=='.');

        if (k==0 || !isDecimalPoint || alreadyHasDecimalPoint)
            return false;

        Debug.Assert(isDecimalPoint);
        alreadyHasDecimalPoint = true;
        return char.IsDigit(chars.LookAhead(k+1));    
    }

    TToken ITokenSource<TToken>.CreateToken(IEnumerable<char> chars) =>  Lex(new Scanner<char>(chars));
}
}
