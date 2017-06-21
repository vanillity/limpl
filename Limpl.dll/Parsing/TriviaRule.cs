using System.Collections.Generic;

namespace Limpl
{
    public interface ITriviaSource<T>  where T : ISyntaxTrivia
    {
        T CreateTrivia(IEnumerable<char> chars);
    }

    public interface ITriviaRule<TTrivia> : ITriviaSource<TTrivia>  where TTrivia : ISyntaxTrivia
    {
        bool MatchesUpTo(IScanner<char> chars, int k);
        TTrivia Lex(Scanner<char> chars);
    }
}