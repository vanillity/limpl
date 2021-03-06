﻿namespace Timpl
{
public struct TokenKind : Limpl.ITokenKind
{
    static TokenKind()
    {
        Limpl.TokenKind.InitTokenKinds<TokenKind>();
    }

    public TokenKind(int value) => Value = value;

    public int Value {get; private set;}

    public readonly static TokenKind Misc  = new TokenKind(Limpl.Common.TokenKinds.Unspecified);
    public readonly static TokenKind Dot   = new TokenKind(Limpl.Common.TokenKinds.Dot);
    public readonly static TokenKind SOF   = new TokenKind(Limpl.Common.TokenKinds.StartOfFile);
    public readonly static TokenKind EOF   = new TokenKind(Limpl.Common.TokenKinds.EndOfFile);
    public readonly static TokenKind Space = new TokenKind(Limpl.Common.TokenKinds.Space);

    public override string ToString()
    {
        return Limpl.TokenKind.GetName(this);
    }
}
}
