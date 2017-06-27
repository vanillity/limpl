using Limpl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timpl
{
public class TokenRule : Limpl.TokenRule<Token>, Limpl.ITriviaRule<Token>
{
    public static readonly TokenRule Dot = SimpleTokenRule(kind: TokenKind.Dot,text: ".");
    public static readonly TokenRule SOF = new TokenRule(TokenKind.SOF,(s,i)=>s.Position<0&&i<1,(td,s)=>new Token(null,TokenKind.SOF,null,true));


    private TokenRule(TokenKind kind, Func<Limpl.IScanner<char>,int,bool> matchesUpTo, Func<TokenRule,Limpl.Scanner<char>,Token> lex, bool allowedInOtherToken = false) 
                    : base(
                        kind,
                        matchesUpTo,
                        lex: (rule,scanner)=>lex((TokenRule)rule,scanner),
                        createToken: chars=>new Token(new string(chars.ToArray()),kind),
                        allowedInOtherToken:allowedInOtherToken)
    {      
    }

    public bool IsAllowedInTokenCluster
    {
        get;
    }

    public TokenKind TokenKind
    {
        get;
    }

  
    public Token CreateTrivia(IEnumerable<char> chars)
    {
        throw new NotImplementedException();
    }

    private static TokenRule SimpleTokenRule(TokenKind kind,string text)
     => new TokenRule(
                    kind,
                    matchesUpTo: (scnr, k) => k > -1 && k < text.Length && text.Substring(0,k+1)==scnr.Scan(k+1),
                    lex: (rule,scanner)=>new Token(text, kind));
    
}
}
