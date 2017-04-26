using System;
using System.Collections.Generic;
using System.Text;
using Limpl;

namespace Timpl
{
    struct Token : Limpl.IToken
    {
        public TokenKind Kind {get;}
        ITokenKind Limpl.IToken.Kind => Kind;
    }
}
