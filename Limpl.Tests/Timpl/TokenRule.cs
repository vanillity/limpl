using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Limpl;

namespace Timpl
{
    public class TokenRule : Limpl.TokenRule<Token>, Limpl.ITriviaRule<Token>
{
    public readonly static TokenRule Dot = SimpleTokenRule(kind: TokenKind.Dot,text: ".");
    public readonly static TokenRule SOF = new TokenRule(TokenKind.SOF,(s,i)=>s.Position<0&&i<1,(td,s)=>new Token(null,TokenKind.SOF,isTrivia:true));
    public readonly static TokenRule EOF = new TokenRule(TokenKind.EOF,(s,i)=>s.End,(td,s)=>new Token(null,TokenKind.EOF,isTrivia:true));
    public readonly static StringLiteralRule StringLiteral = new StringLiteralRule();

    public static readonly TokenRule Space = new TokenRule
    (
        TokenKind.Space,
        (s,i)=>((s.LookAhead(i)==' ' || s.LookAhead(i)=='\t')),
        (td,s)=>Lexer.token(TokenKind.Space,s,td,_=>_==' ' || _=='\t')
    );

    public TokenRule(TokenKind kind, Func<Limpl.IReadOnlyScanner<char>,int,bool> matchesUpTo, Func<TokenRule,Limpl.IScanner<char>,Token> lex, bool allowedInOtherToken = false) 
                : base(
                        kind,
                        matchesUpTo,
                        lex: (rule,scanner)=>lex((TokenRule)rule,scanner),
                        createToken: chars=>new Token(new string(chars.ToArray()),kind),
                        allowedInOtherToken:allowedInOtherToken)
    {      
    }

    public class StringLiteralRule : Limpl.StringLiteralRule<Token>
    {
        public StringLiteralRule() : base('\"','\'') {}

        public override Token CreateToken(string text,int position)
        {
            
            return new Token(text,TokenKind.Misc,position);
        }
    }

    public class NumericLiteralRule:Limpl.NumericLiteralRule<Token>
    {
        public override Token CreateToken(string chars,double value,int position)
        {
            return new Token(chars,TokenKind.Misc,position,value:value);
        }
    }

    public class DotsDefinition : Limpl.ITokenRule<Token>
    {
        public bool IsAllowedInOtherToken => true;

        public Token Lex(Limpl.IScanner<char> chars)
        {
            Debug.Assert(chars.Current=='.');
            chars.MoveNext();
       
            if (chars.Current=='.')
            {
                chars.MoveNext();
                if (chars.Current=='.') 
                {
                    chars.MoveNext();
                    return new Token("...",TokenKind.Misc);
                }

                return new Token("..",TokenKind.Misc);
            }
            else
            {
                return new Token(".",TokenKind.Dot);
            }       
        }

        public bool MatchesUpTo(Limpl.IReadOnlyScanner<char> chars,int k) => (k>=0 && k<=2 && chars.LookAhead(k)=='.');
        Token Limpl.ITokenSource<Token>.CreateToken(IEnumerable<char> chars) => Lex(new Limpl.Scanner<char>(chars));           
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
