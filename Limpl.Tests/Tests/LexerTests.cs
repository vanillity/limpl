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

    [Fact] public void TokenRuleTest1()
    {
        var s = new Limpl.Scanner<char>();
        s.Initialize(".".GetEnumerator());
        s.MoveNext();
        assert_equals('.',()=>s.Current);
        var b = TokenRule.Dot.MatchesUpTo(s,0);
        assert_true(b);
    }


    [Fact] public void LexerTest1()
    {
   
       assert_equals(Timpl.TokenKind.Dot,()=>lexerTest(".",1,null).Single().Kind);
       assert_equals(TokenKind.Misc,()=>lexerTest("<>",1,null).Single().Kind);

       lexer = new Lexer(lexer.TokenRules.Concat(new[]{TokenRule.SOF}));
       lexerTest("x",expectedTokenCount: 2); // <StartOfFile> & 'x'


    }

    [Fact] public void TriviaTest1()
    {
        lexer = new Lexer(lexer.TokenRules,new[]{TokenRule.SOF});
        lexerTest("x", 1,tokens =>
        {
            Write(()=>tokens[0].LeadingTrivia);
            var sof = tokens[0].LeadingTrivia.Single();
            assert_equals(TokenKind.SOF,sof.Kind);
        }); 
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
