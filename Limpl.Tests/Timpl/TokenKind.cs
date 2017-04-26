using System;
using System.Collections.Generic;
using System.Text;


namespace Timpl
{
struct TokenKind :Limpl.ITokenKind
{
    static TokenKind()
    {
        Limpl.TokenKind.InitTokenKinds<TokenKind>();
    }

    public int Value {get; private set;}

    public const int Dot = Limpl.Common.TokenKinds.Dot;
}
}
