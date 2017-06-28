using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Limpl;

namespace Timpl
{
class Lexer : Limpl.Lexer<Token,Token>
{
    public Lexer(IEnumerable<Limpl.ITokenRule<Token>> tokenRules = null,IEnumerable<Limpl.ITriviaRule<Token>> triviaRules=null) : base(tokenRules??DefaultTokenRules(),triviaRules??DefaultTriviaRules())
    {
    }

    private static IEnumerable<ITriviaRule<Token>> DefaultTriviaRules()
    {
        return new ITriviaRule<Token>[0];
    }

    private static IEnumerable<ITokenRule<Token>> DefaultTokenRules()
    {
        yield return TokenRule.Dot;
    }

    protected override Token LexFallbackToken(IScanner<char> chars)
    {
        return token(TokenKind.Misc,chars,null,c=>GetTriviaRule(chars)==null && (GetTokenRule(chars)?.IsAllowedInOtherToken ?? true));
    }

    protected override void OnStartOfFile(out Token sof)
    {
        base.OnStartOfFile(out sof);
         
        if (TokenRules.Contains(TokenRule.SOF))
            sof = new Token(null,TokenKind.SOF);
    }

    protected override void OnEndOfFileTrivia(out Token eof)
    {
        base.OnEndOfFileToken(out eof);

        if (TriviaRules.Contains(TokenRule.EOF))
            eof = new Token(null,TokenKind.EOF);
    }

    protected override void OnEndOfFileToken(out Token eof)
    {
        base.OnEndOfFileToken(out eof);

        if (TokenRules.Contains(TokenRule.EOF))
            eof = new Token(null,TokenKind.EOF);
    }

    protected override void SetLeadingTrivia(ref Token token,SyntaxList<Token> leadingTrivia)
    {
        token.LeadingTrivia = leadingTrivia;
    }

    protected override void SetParent(ref Token token,ISyntaxNode parent)
    {
        token.Parent = parent;
    }

    protected override void SetTrailingTrivia(ref  Token token,SyntaxList<Token> trailingTrivia)
    {
        token.TrailingTrivia = trailingTrivia;
    }

    internal static  Token token(
                            TokenKind kind, 
                            IScanner<char> buffer, 
                            ITokenRule<Token> tokendef,
                            Func<char,bool> predicate=null) 
    {
        //TODO: change to false only after called once before? 
        if (predicate==null) 
            predicate = c => false;     
       
        var sb  = new StringBuilder().Append(buffer.Consume());
        var pos = buffer.Position;
        
        while (!buffer.End && predicate(buffer.Current)) 
            sb.Append(buffer.Consume());  

        var text = sb.ToString();

        return new Token(text,kind);
    }

}
}
