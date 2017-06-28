using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limpl
{
public class TriviaRuleList<TRule, TTrivia>  : TriviaSourceList<TRule, TTrivia> 
                                                        where TRule : Limpl.ITriviaSource<TTrivia>, Limpl.ITriviaRule<TTrivia>
                                                        where TTrivia : ISyntaxTrivia                                                        
{

    public TriviaRuleList() : base(new TRule[0]) { }

    internal TriviaRuleList(IEnumerable<TRule> matches) : base(matches)
    {
        if (matches != null)    
            foreach(var m in matches)
                InnerList.Add(m);
    }

    public TriviaRuleList<TRule, TTrivia> Matches(IScanner<char> chars, int k)
    {
        return new TriviaRuleList<TRule, TTrivia>(InnerList.Where(_=>_.MatchesUpTo(chars,k)));
    }

    public TriviaRuleList<TRule, TTrivia> Add(params TRule[] rules)
    {
        return new TriviaRuleList<TRule, TTrivia>(InnerList.Concat(rules));
    }

}

public abstract class TriviaSourceList<TSource, TTrivia> : IReadOnlyList<TSource> where TSource : Limpl.ITriviaSource<TTrivia> where TTrivia : ISyntaxTrivia
{
    protected ImmutableList<TSource> InnerList {get;} = ImmutableList<TSource>.Empty;
    public TriviaSourceList(IEnumerable<TSource> src)
    {
        if (src !=null) 
            InnerList = InnerList.AddRange(src);
    }

    public TSource this[int index]
    {
        get
        {
            return InnerList[index];
        }

    }

    public int  Count => InnerList.Count;
    public bool IsReadOnly => false;

   
    IEnumerator IEnumerable.GetEnumerator()
    {
        return InnerList.GetEnumerator();
    }

    public IEnumerator<TSource> GetEnumerator()
    {
        return InnerList.GetEnumerator();
    }
}
}
