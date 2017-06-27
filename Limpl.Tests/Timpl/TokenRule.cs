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
    public static readonly TokenRule EOF = new TokenRule(TokenKind.EOF,(s,i)=>s.End,(td,s)=>new Token(null,TokenKind.EOF,null,true));
    public readonly static StringLiteralRule StringLiteral = new StringLiteralRule();

    public static readonly TokenRule Space = new TokenRule
    (
        TokenKind.Space,
        (s,i)=>((s.LookAhead(i)==' ' || s.LookAhead(i)=='\t')),
        (td,s)=>Lexer.token(TokenKind.Space,s,td,_=>_==' ' || _=='\t')
    );

    public TokenRule(TokenKind kind, Func<Limpl.IScanner<char>,int,bool> matchesUpTo, Func<TokenRule,Limpl.Scanner<char>,Token> lex, bool allowedInOtherToken = false) 
                : base(
                        kind,
                        matchesUpTo,
                        lex: (rule,scanner)=>lex((TokenRule)rule,scanner),
                        createToken: chars=>new Token(new string(chars.ToArray()),kind),
                        allowedInOtherToken:allowedInOtherToken)
    {      
    }

    public class StringLiteralRule : StringLiteralRule<Token>
    {
        public StringLiteralRule() : base('\"','\'') {}
    
        public override Token CreateToken(IEnumerable<char> chars)
        {
            return new Token(new string(chars.ToArray()),TokenKind.Misc);
        }
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

    public static TokenRule SimpleTokenRule(TokenKind kind,string text)
     => new TokenRule(
                    kind,
                    matchesUpTo: (scnr, k) => k > -1 && k < text.Length && text.Substring(0,k+1)==scnr.Scan(k+1),
                    lex: (rule,scanner)=>new Token(text, kind));
    
}
}
