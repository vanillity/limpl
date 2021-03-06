﻿using System;
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

       //SOF as token
       lexer = new Lexer(lexer.TokenRules.Concat(new[]{TokenRule.SOF}));
       lexerTest("x",expectedTokenCount: 2); // <StartOfFile> & 'x'

       //EOF (trivia)
       lexer = new Lexer(lexer.TokenRules,lexer.TriviaRules.Concat(new[]{TokenRule.EOF}));
       lexerTest("x",2, a:tokens=>
        {            
            Write(()=>tokens[1].TrailingTrivia);
            var eof = tokens[1].TrailingTrivia.Single();
            assert_equals(TokenKind.EOF,eof.Kind);  
        });

        //EOF as token
        lexer = new Lexer(lexer.TokenRules.Add(TokenRule.EOF),lexer.TriviaRules);  
        lexerTest("x",3); // <StartOfFile> & 'x' & <EOF>

        lexer = new Lexer(triviaRules: lexer.TriviaRules.Add(TokenRule.Space));
        lexerTest("a b",2,tokens=>Write(()=>tokens.Select(_=>new {_, _.LeadingTrivia, _.TrailingTrivia})));

        //EndOfLine
        lexer = new Lexer(lexer.TokenRules,lexer.TriviaRules.Add(new TokenRule(TokenKind.Misc,(s,i)=>(i>=0 && s.LookAhead(i)=='\r' || i>=0 && i<=1 && s.LookAhead(i)=='\n'),
        (td,s)=>
        {
            var c = s.Consume();
 
            if (c == '\r' && s.Current == '\n')
            {
                s.MoveNext();
                return new Token("\r\n",TokenKind.Misc);
            }
            else
            {
                return new Token(c.ToString(),TokenKind.Misc);
            }
            
        })));
        lexerTest("a\r\n b",2);

        //...
        lexer = new Lexer(lexer.TokenRules.Add
        (
           TokenRule.SimpleTokenRule(TokenKind.Misc, "@"),
           TokenRule.SimpleTokenRule(TokenKind.Misc, "<"),
           TokenRule.SimpleTokenRule(TokenKind.Misc, ">"),
           TokenRule.SimpleTokenRule(TokenKind.Misc, ";"),
           TokenRule.SimpleTokenRule(TokenKind.Misc, ":"),
           TokenRule.SimpleTokenRule(TokenKind.Misc, "("),
           TokenRule.SimpleTokenRule(TokenKind.Misc, ")"),
           TokenRule.SimpleTokenRule(TokenKind.Misc, "{"),
           TokenRule.SimpleTokenRule(TokenKind.Misc, "}"),
           TokenRule.SimpleTokenRule(TokenKind.Misc, ",")
        ), lexer.TriviaRules);
        lexerTest("a;<>,\r\n@b:{}(c)",13);

        //string literal
        lexer = new Lexer(lexer.TokenRules.Add(TokenRule.StringLiteral),lexer.TriviaRules);
        lexerTest(@"a""b{};\""\ \\""c",3); // a, "b{};\"\ \", c

        //dots
        lexer = new Lexer(lexer.TokenRules.Add(new TokenRule.DotsDefinition()),lexer.TriviaRules);
        lexerTest(".a ..b ...c",6); //., a, .., b, ..., c

        //number literal 
        lexer = new Lexer(lexer.TokenRules.Add(new TokenRule.NumericLiteralRule()),lexer.TriviaRules);
        lexerTest("a 9.0.b c",5); // a, 9.0, ., b, c
    }


    //# Default Lexer Test
    [Fact] public void LexerTest2()
    {
        lexer = new Timpl.Lexer(
                        tokenRules: new Limpl.ITokenRule<Token>[]
                        {
                            TokenRule.StringLiteral,
                            TokenRule.SimpleTokenRule(TokenKind.Misc, "("),
                            TokenRule.SimpleTokenRule(TokenKind.Misc, ")"),
                            TokenRule.SimpleTokenRule(TokenKind.Misc, "@"),
                            TokenRule.SimpleTokenRule(TokenKind.Misc, "<"),
                            TokenRule.SimpleTokenRule(TokenKind.Misc, ">"),
                        },
                        triviaRules: new Limpl.ITriviaRule<Token>[]
                        {
                            
                        });

        lexerTest("(5)",3);
        lexerTest("@X<>",4);
        lexerTest("'Hello world!'",1);        
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
