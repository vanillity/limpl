using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Limpl
{
public abstract class StringLiteralRule<TToken> : ITokenRule<TToken> where TToken : IToken
{
    readonly char[] delimiters;    

    char delimiter = (char) 0;
    bool escaping = false;
    bool closed = false;
    bool matchesSoFar = false;

    public StringLiteralRule(params char[] delimiters)
    {
       this.delimiters = delimiters; 
    }

    public bool IsAllowedInOtherToken => false;

    public abstract TToken CreateToken(IEnumerable<char> chars);

    public TToken Lex(Scanner<char> chars)
    {
        Debug.Assert(chars.Current==delimiter);
        chars.Consume();

        var p  = chars.Position+1;
        var sb = new StringBuilder();

        while (!chars.End && chars.Current != delimiter)
        {
            if (chars.Current=='\\' && (chars.LookAhead(1)==delimiter||chars.LookAhead(1)=='\\'))
                sb.Append(chars.Consume()).Append(chars.Consume()); // \" or \\
            else
                sb.Append(chars.Consume());
        }

        Debug.Assert(chars.Current==delimiter);
        chars.Consume();

        var text = sb.ToString();
        var t = CreateToken(text);
        return t;
    }

    public bool MatchesUpTo(IScanner<char> chars,int k)
    {
        if (k==0)
        {
            closed   = false;
            escaping = false;
                
            if (delimiters.Contains(chars.Current))
            {
                delimiter = chars.Current;
                matchesSoFar = true;
            }
            else
            {
                delimiter = (char) 0;
                matchesSoFar = false;
            }

            return matchesSoFar;                 
        }

        var c = chars.LookAhead(k);

        if (matchesSoFar && !closed && c!='\0')
        {
            if (c==delimiter && !escaping)
                closed = true;

            escaping = (!escaping && c=='\\');
  
            return true;
        }

        return false;    
    }
}
}