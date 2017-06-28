using System;
using System.Collections.Generic;
using System.Text;

namespace Limpl
{
    public interface IToken : ISyntaxNode
    {
        ITokenKind Kind {get;}
        string Text {get;}
        IReadOnlyList<ISyntaxTrivia> LeadingTrivia  {get;}
        IReadOnlyList<ISyntaxTrivia> TrailingTrivia {get;}
    }
}
