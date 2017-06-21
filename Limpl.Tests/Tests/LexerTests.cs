using System;
using System.Collections.Generic;
using System.Linq;
using Timpl;
using Xunit;
using Xunit.Abstractions;

namespace LimplTests
{
public class LexerTests : LimplTest
{
    Timpl.Lexer lexer = new Timpl.Lexer();

    public LexerTests(ITestOutputHelper o) : base(o) {} 

    [Fact] public void LexerTest1()
    {
    
        assert_equals(Timpl.TokenKind.Dot,()=>lexerTest(".",1,null).Single().Kind);
    }

    IList<Token> lexerTest(string input, int? expectedTokenCount = null, Action<IList<Token>> a = null)
    {
        var tokens = lexer.Lex(input);
        var tokenList = tokens.ToList();

        Write(()=>input);
        Write(()=>tokenList);

        if (expectedTokenCount != null)
            assert_equals(expectedTokenCount,()=>tokenList.Count);        

        a?.Invoke(tokenList);

        return tokenList;
    }

}
}
