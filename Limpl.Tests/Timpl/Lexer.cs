using System;
using System.Collections.Generic;
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

    protected override void SetLeadingTrivia(Token token,SyntaxList<Token> leadingTrivia)
    {
        throw new NotImplementedException();
    }

    protected override void SetParent(Token trivia,ISyntaxNode parent)
    {
        throw new NotImplementedException();
    }

    protected override void SetTrailingTrivia(Token token,SyntaxList<Token> leadingTrivia)
    {
        throw new NotImplementedException();
    }
}
}
