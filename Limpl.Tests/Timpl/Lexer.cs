using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Limpl;
using Limpl.Syntax;

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

    protected override Token LexFallbackToken(Scanner<char> chars)
    {
        var sb = new StringBuilder();
        while (!chars.End)
            sb.Append(chars.Consume());
        return Token.Fallback(sb.ToString());
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
}
}
