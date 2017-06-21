using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limpl
{
public class LexerEventsArgs<TLexer,TToken,TTrivia> : EventArgs         
    where TToken : IToken
    where TTrivia: ISyntaxTrivia
    where TLexer : ILexer<TToken,TTrivia>
{
    public LexerEventsArgs(TLexer lexer)
    {
        Lexer = lexer;
        //Character = c;
    }

    public TLexer Lexer {get;}
    //public char   Character {get;}
}
}
