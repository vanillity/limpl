using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Limpl
{
    public interface ITokenKind
{
    int Value {get;}
}

public static class TokenKind
{
    static HashSet<ITokenKind>      builtin = new HashSet<ITokenKind>();
    static HashSet<int>             values  = new HashSet<int>();
    static Dictionary<int,string>   names   = new Dictionary<int, string>();
    
    /// <summary>Given a ITokenKind type, will consider all 
    /// public static fields on that type, and optionally of type Int32, as built-in, 
    /// named token kinds.</summary>
    /// <typeparam name="T">A type that implements ITokenKind.</typeparam>
    public static void InitTokenKinds<T>(Func<int,T> makeTokenKind = null) where T : ITokenKind
    {
        var t = typeof(T);
        var fields = t.GetTypeInfo().GetFields().Where(_=>_.IsStatic && _.IsPublic).ToList();            

        foreach(var f in fields.Where(_=>_.FieldType==t))
        {
            builtin.Add((T) f.GetValue(null));
            names.Add(((T)f.GetValue(null)).Value,f.Name);
        }

        if (makeTokenKind != null)
        {
            foreach(var intf in fields.Where(_=>_.FieldType==typeof(int)))
            {
                var tk = makeTokenKind((int) intf.GetValue(null));

                names.Add(tk.Value, intf.Name);

                if (intf.IsStatic && intf.IsPublic)
                builtin.Add(tk);
            }
        }
        
    }

    public static bool IsBuiltIn(ITokenKind tokenKind) => builtin.Contains(tokenKind); 

    public static string GetName(ITokenKind tk) 
        => (names.ContainsKey(tk.Value))
                ? names[tk.Value]
                : $"#{tk.Value}";

    public static bool Equals(ITokenKind a, ITokenKind b) => (a.Value == b.Value);
    public static bool Equals(ITokenKind a, object b) => b is ITokenKind tk && Equals(a, tk);
    public static int  GetHashCode(ITokenKind tk) => tk.Value.GetHashCode();
    
    public static string ToString(ITokenKind tk) => $"{{{tk.GetType().Name}: {GetName(tk)}}}";

}
}
