using System;
using System.Collections.Generic;
using System.Text;

namespace Limpl
{
    public interface ISyntaxNode
    {
        bool IsToken {get;}
        bool IsTrivia {get;}
        ISyntaxNodeSource Source {get;}    
    }

    public interface ISyntaxNodeSource
    {
    
    }
}
