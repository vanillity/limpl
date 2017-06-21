using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Limpl
{
public interface IReadOnlyScanner<T>
{
    
    T LookAhead(int k);
    T Current  {get;}
    T Next {get;}
    bool End {get;}
    int  Position {get;}
}


public interface IScanner<T> : IReadOnlyScanner<T>
{
    void Initialize(IEnumerator<T> input);
    bool MoveNext();
    T Consume();
}


public class Scanner<T> : IScanner<T>, IEnumerator<T>
{
    readonly List<T> buffer = new List<T>();
    IEnumerator<T> enumerator;

    T current;
    int position = -2;
    
    public T    Current  {get {return current;}}
    public T    Next     {get {return LookAhead(1);}}
    public bool End      {get; private set; }   
    public int  Position {get {return position;}}

    public virtual void Initialize(IEnumerator<T> input)
    {
        enumerator = input ?? throw new ArgumentNullException(nameof(input));
        position = -1;
        End = false;
    }

    public virtual T Consume()
    {
        var c = current;
        MoveNext();
        return c;
    }

    public virtual bool MoveNext()
    {
       if (enumerator == null)
           throw new InvalidOperationException("Call Initialize() first");

       if (buffer.Count>0) 
       {
          current = buffer[0];
          buffer.RemoveAt(0);
          position++;
          return true;
       }       

       if (enumerator.MoveNext()) 
       {
          current = enumerator.Current;
          position++;
          return true;
       }

       current  = default(T);
       this.End = true;
       return false;
    }


    public virtual T LookAhead(int k)
    {
       if (k==0) return current;

       while (k > buffer.Count) 
       {
          if (!enumerator.MoveNext()) 
            return default(T);

          buffer.Add(enumerator.Current);
       }

       return buffer[k-1];
    }

    void IDisposable.Dispose(){}
    object System.Collections.IEnumerator.Current{ get { return this.Current; }}
    void System.Collections.IEnumerator.Reset(){throw new NotSupportedException();}
}
}
