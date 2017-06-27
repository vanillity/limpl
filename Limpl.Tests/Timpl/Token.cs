using System;
using System.Collections.Generic;
using System.Text;
using Limpl;
using Limpl.Syntax;

namespace Timpl
{
public struct Token : Limpl.IToken, Limpl.ISyntaxTrivia
{
    public Token(string text, TokenKind kind, ISyntaxNode parent = null, bool isTrivia = false)
    {
        Text = text;
        Kind = kind;
        LeadingTrivia = null;
        TrailingTrivia = null;
        IsTrivia = isTrivia;
        Parent = parent;
    }

    public TokenKind Kind {get;}

    public string Text {get;}

    public SyntaxList<Token> LeadingTrivia {get; internal set;}
    public SyntaxList<Token> TrailingTrivia {get; internal set;}

    public bool IsToken => true;

    //TO-DO: verify
    public bool IsTrivia {get;}

    public ISyntaxNode Parent {get; internal set;}

    ITokenKind Limpl.IToken.Kind => Kind;

    IReadOnlyList<ISyntaxTrivia> IToken.LeadingTrivia => (IReadOnlyList<ISyntaxTrivia>) LeadingTrivia;
    IReadOnlyList<ISyntaxTrivia> IToken.TrailingTrivia => (IReadOnlyList<ISyntaxTrivia>) TrailingTrivia;

    public ISyntaxNode Clone()
    {
        throw new NotImplementedException();
    }

    public static Token Fallback(string text) //token cluster
    {
        return new Token(text,TokenKind.Misc);
    }

    public override string ToString()
    {
        return $"{{{Kind} : {Text}}}";
    }

        ISyntaxNode ISyntaxNode.Clone()
        {
            throw new NotImplementedException();
        }
    }
}
