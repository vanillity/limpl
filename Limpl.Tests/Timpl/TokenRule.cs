using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Limpl;

namespace Timpl
{
public class DotTokenRule : Limpl.ITokenRule<Token>
{
    public static readonly DotTokenRule Instance = new DotTokenRule();
    
    private DotTokenRule() {}

    public bool IsAllowedInOtherToken => throw new NotImplementedException();

    public Token CreateToken(IEnumerable<char> chars)
    {
        return new Token(new string(chars.ToArray()), TokenKind.Dot);
    }

    public Token Lex(Scanner<char> chars)
    {
        var c = chars.Consume();
        Debug.Assert(c=='.');
        return CreateToken(".");
    }

    public bool MatchesUpTo(IScanner<char> chars,int k)
        => k==0 && chars.Current=='.';
}
}
