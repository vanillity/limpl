using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

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
    protected IScanner<char> Scanner => chars;

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
        if (sof != null && !sof.Equals(default(TToken)))
            yield return sof;
            
        TToken _token = default(TToken);
        while (!chars.End || _token != null && !_token.Equals(default(TToken)))
        {        
            var c = chars.Current;
            var trivia = (_token == null || _token.Equals(default(TToken))) ? leadingTrivia : trailingTrivia;

            if (c == '\0') //NUL is before beginning and after end
            {
               if (chars.Position < 0)
                {
                    var sofRule = GetTriviaRule(chars);
                    if (sofRule != null)
                    {
                        var _triv = (TTrivia) sofRule.Lex(chars);
                        leadingTrivia.Add(_triv);
                    }
                }

            
                if (chars.End)
                    goto end;
                else
                    chars.MoveNext();
            }

            //trivia (non-tokens)
            var triviaRule = GetTriviaRule(chars);
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
            if (_token == null || _token.Equals(default(TToken)))
            {
                var tokenRule = GetTokenRule(chars);    
                if (tokenRule!=null)
                {
                    var p = chars.Position;
                    _token = tokenRule.Lex(chars);
                    if (p == chars.Position && _token.Text?.Length > 0)
                        chars.MoveNext();
                    continue;
                }
            }

            //fall-back token (cluster)
            if (_token == null || _token.Equals(default(TToken)))
            {
                _token = LexFallbackToken(chars); //tokenCluster()
                continue;    
            }                            

            
            end:
            var eof_trivia = default(TTrivia);
            {
                if (chars.End)
                {
                    OnEndOfFileTrivia(out eof_trivia);
                    if (eof_trivia != null && !eof_trivia.Equals(default(TTrivia)))
                       trailingTrivia.Add(eof_trivia);
                }
            
                if (leadingTrivia.Count > 0)
                    SetLeadingTrivia(ref  _token,new SyntaxList<TTrivia>(_token,leadingTrivia,SetParent));
                    //_token.LeadingTrivia = new AtSyntaxList<AtSyntaxTrivia>(_token,leadingTrivia);
                
                if (trailingTrivia.Count > 0)
                    SetTrailingTrivia(ref _token,new SyntaxList<TTrivia>(_token,trailingTrivia,SetParent));
                    //_token.TrailingTrivia = new AtSyntaxList<AtSyntaxTrivia>(_token,trailingTrivia);
                
                if (!_token.Equals(default(TToken)))
                    yield return _token;
            
                _token = default(TToken);
                leadingTrivia.Clear();
                trailingTrivia.Clear();            
            }


            OnEndOfFileToken(out TToken eof);
            if (eof != null && !eof.Equals(default(TToken)))
                yield return eof;
         }
    }

    protected abstract TToken LexFallbackToken(IScanner<char> chars);
    protected abstract void SetParent(ref TTrivia trivia, ISyntaxNode parent);
    protected abstract void SetLeadingTrivia(ref  TToken token, SyntaxList<TTrivia> leadingTrivia);
    protected abstract void SetTrailingTrivia(ref  TToken token, SyntaxList<TTrivia> leadingTrivia);

    protected virtual void OnStartOfFile(out TToken sof)
    {
        StartOfFile?.Invoke(this,new LexerEventsArgs<Lexer<TToken, TTrivia>, TToken, TTrivia>(this));
        sof = default(TToken);
    }

    protected virtual void OnEndOfFileTrivia(out TTrivia eof)
    {
        EndOfFile?.Invoke(this,new LexerEventsArgs<Lexer<TToken, TTrivia>, TToken, TTrivia>(this));
        eof = default(TTrivia);
    }

    protected virtual void OnEndOfFileToken(out TToken eof)
    {
        EndOfFile?.Invoke(this,new LexerEventsArgs<Lexer<TToken, TTrivia>, TToken, TTrivia>(this));
        eof = default(TToken);
    }

    protected ITokenRule<TToken> GetTokenRule(IScanner<char> chars)
    {
        int k = -1;
        var anyMatch = false;
        TokenRuleList<ITokenRule<TToken>,TToken> lastMatches = TokenRules, matches;

        if (TokenRules.Count > 0)
        {
            k = -1;
            while((matches = lastMatches.Matches(chars,++k)).Count > 0)  
            {
                lastMatches = matches;
                anyMatch = true;

                if (chars.End)
                    break;
            }

            if (anyMatch && lastMatches?.Count > 0)
                return lastMatches[0];
        }    

        return null;
    }

    protected ITriviaRule<TTrivia> GetTriviaRule(IScanner<char> chars)
    {
        int k = -1;
        var anyMatch = false;
        TriviaRuleList<ITriviaRule<TTrivia>,TTrivia> lastMatches = TriviaRules, matches;

        if (TriviaRules.Count > 0)
        {
            k = -1;
            while((matches = lastMatches.Matches(chars,++k)).Count > 0)  
            {
                lastMatches = matches;
                anyMatch = true;

                if (chars.End)
                    break;
            }

            if (anyMatch && lastMatches?.Count > 0)
                return lastMatches[0];
        }    

        return null;
    }
}
}
