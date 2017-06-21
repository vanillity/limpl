using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Limpl.Parsing;
using Limpl.Syntax;

namespace Limpl
{
public interface ILexer<TToken,TTrivia> where TToken : IToken where TTrivia : ISyntaxTrivia
{
    IEnumerable<TToken> Lex(IEnumerable<char> input);
}

public abstract class Lexer<TToken,TTrivia> : ILexer<TToken,TTrivia> where TToken : IToken where TTrivia : TToken, ISyntaxTrivia
{      
    readonly Scanner<char> chars;

    public Lexer(IEnumerable<ITokenRule<TToken>> tokenRules, IEnumerable<ITriviaRule<TTrivia>> triviaRules, Scanner<char> scanner = null)
    {
        TokenRules = new TokenRuleList<ITokenRule<TToken>,TToken>(tokenRules);
        TriviaRules = new TriviaRuleList<ITriviaRule<TTrivia>,TTrivia>(triviaRules);
        this.chars = scanner ?? new Scanner<char>();
    }
         
    public TokenRuleList<ITokenRule<TToken>,TToken>   TokenRules {get;}
    public TriviaRuleList<ITriviaRule<TTrivia>, TTrivia> TriviaRules {get;}

    //StartOfFile event
    public static event EventHandler<LexerEventsArgs< Lexer<TToken,TTrivia> ,TToken,TTrivia>> StartOfFile;
    public static event EventHandler<LexerEventsArgs< Lexer<TToken,TTrivia> ,TToken,TTrivia>> EndOfFile;
    
    public virtual IEnumerable<TToken> Lex(IEnumerable<char> input)
    {
        chars.Initialize(input.GetEnumerator());

        var leadingTrivia  = new List<TTrivia>();
        var trailingTrivia = new List<TTrivia>();

        //<StartOfFile>? 
        //ISyntaxTrivia sof = null;
        //if (TokenRules.Contains(Limpl.TokenRule.StartOfFile))
            //yield return (sof = new AtSyntaxTrivia(TokenKind.StartOfFile,0));
        OnStartOfFile(out TToken sof);
        if (!sof.Equals(default(TToken)))
            yield return sof;
            
        TToken _token = default(TToken);
        while (!chars.End || !_token.Equals(default(TToken)))
        {        
            var c = chars.Current;
            var trivia = (_token.Equals(default(TToken))) ? leadingTrivia : trailingTrivia;

            if (c == '\0') //NUL is before beginning and after end
            {
                if (chars.End)
                    goto end;
                else
                    chars.MoveNext();
            }

            //trivia (non-tokens)
            var triviaRule = getRule(TriviaRules,chars,TriviaRules.Matches);
            if (triviaRule!=null)
            {
                var p = chars.Position;
                var _triv = (TTrivia) triviaRule.Lex(chars);
                trivia.Add(_triv);
                if (p == chars.Position && _triv.Text?.Length > 0)
                    chars.MoveNext();
                continue;
            }
            
            //tokens
            if (_token.Equals(default(TToken)))
            {
                var tokenRule = getRule(TokenRules,chars,TokenRules.Matches);    

                if (tokenRule!=null)
                {
                    var p = chars.Position;
                    _token = tokenRule.Lex(chars);
                    if (p == chars.Position && _token.Text.Length > 0)
                        chars.MoveNext();
                    continue;
                }
            }

            if (_token.Equals(default(TToken)))
            {
                _token = LexFallbackToken(chars); //tokenCluster()
                continue;    
            }                            

            
            end:
            TToken eof = default(TToken);
            {
                if (chars.End)
                {
                    OnEndOfFile(out eof);
                    if (!eof.Equals(default(TToken)) && eof is TTrivia eofTrivia)
                       trailingTrivia.Add(eofTrivia);
                }
            
                if (leadingTrivia.Count > 0)
                    SetLeadingTrivia(_token,new SyntaxList<TTrivia>(_token,leadingTrivia,SetParent));
                    //_token.LeadingTrivia = new AtSyntaxList<AtSyntaxTrivia>(_token,leadingTrivia);
                
                if (trailingTrivia.Count > 0)
                    SetTrailingTrivia(_token,new SyntaxList<TTrivia>(_token,trailingTrivia,SetParent));
                    //_token.TrailingTrivia = new AtSyntaxList<AtSyntaxTrivia>(_token,trailingTrivia);
                
                if (!_token.Equals(default(TToken)))
                    yield return _token;
            
                _token = default(TToken);
                leadingTrivia.Clear();
                trailingTrivia.Clear();            
            }


            if ((object) eof == (object) default(TToken))
                OnEndOfFile(out eof);
         }
    }

    protected abstract TToken LexFallbackToken(Scanner<char> chars);
    protected abstract void SetParent(TTrivia trivia, ISyntaxNode parent);
    protected abstract void SetLeadingTrivia(TToken token, SyntaxList<TTrivia> leadingTrivia);
    protected abstract void SetTrailingTrivia(TToken token, SyntaxList<TTrivia> leadingTrivia);

    protected virtual void OnStartOfFile(out TToken sof)
    {
        StartOfFile?.Invoke(this,new LexerEventsArgs<Lexer<TToken, TTrivia>, TToken, TTrivia>(this));
        sof = default(TToken);
    }

    protected virtual void OnEndOfFile(out TToken sof)
    {
        EndOfFile?.Invoke(this,new LexerEventsArgs<Lexer<TToken, TTrivia>, TToken, TTrivia>(this));
        sof = default(TToken);
    }

    T getRule<T>(IReadOnlyList<T> rules, IScanner<char> chars, Func<IScanner<char>,int,IReadOnlyList<T>> getMatches)
    {
        int k = -1;
        var anyMatch = false;
        IReadOnlyList<T> lastMatches = rules, matches;

        if (rules.Count > 0)
        {
            k = -1;
            while((matches = getMatches(chars,++k)).Count > 0)  //lastMatches.Matches(chars,++k)).Count>0)
            {
                lastMatches = matches;
                anyMatch = true;

                if (chars.End)
                    break;
            }

            if (anyMatch && lastMatches?.Count > 0)
                return lastMatches[0];
        }    

        return default(T);
    }
}
}
