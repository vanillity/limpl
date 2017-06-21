using System;
using System.Collections.Generic;
using System.Text;
using Limpl.Syntax;

namespace Limpl
{
    public interface IToken : ISyntaxNode
    {
        ITokenKind Kind {get;}
        string Text {get;}
        SyntaxList<ISyntaxTrivia> LeadingTrivia  {get;}
        SyntaxList<ISyntaxTrivia> TrailingTrivia {get;}
    }
}
