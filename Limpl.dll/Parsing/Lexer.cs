using System;
using System.Collections.Generic;
using System.Text;

namespace Limpl
{
    public interface ILexer<T> where T : IToken
    {
        IEnumerable<T> Lex(IEnumerable<char> input);
    }

    public static class Lexer
    {
        
    }
}
