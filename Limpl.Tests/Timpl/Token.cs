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

    public SyntaxList<ISyntaxTrivia> LeadingTrivia {get;}

    public SyntaxList<ISyntaxTrivia> TrailingTrivia {get;}

    public bool IsToken => true;

    //TO-DO: verify
    public bool IsTrivia {get;}

    public ISyntaxNode Parent {get;}

    ITokenKind Limpl.IToken.Kind => Kind;

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
}
}
