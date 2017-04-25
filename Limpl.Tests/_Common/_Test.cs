﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Xunit;
using static System.Linq.Expressions.ExpressionType;

namespace Limpl.Tests
{
    //Test base class
    public partial class Test : IDisposable
{
    //ctor
    protected Test()
    {
        TestData.Init();
        TestData = new TestData(this);
        Initialize();
    }

    public TestData TestData {get;}

    //Stopwatch
    public Stopwatch Stopwatch {get {return stopwatch;}}
    Stopwatch stopwatch = new Stopwatch();
   
    //TestContextWriter
    public TestContextTextWriter TestContextWriter
    { get
        { if (_TestContextWriter == null) _TestContextWriter = new TestContextTextWriter(this);
            return _TestContextWriter;
        }
    } TestContextTextWriter _TestContextWriter;


    //Initialize()
    public void Initialize()
    { Setup();
    }
   
    //Cleanup()
    public void Dispose() 
    {
        if (_TestContextWriter != null && _TestContextWriter.Buffered) 
            _TestContextWriter.flush2();
        TearDown();
        TestData.Cleanup();
    }
  
    //TearDown()
    protected virtual void TearDown()
    { 
    }

    //assert_equals()
    public void assert_equals<T>(T expected, T actual, string f = null, params object[] a)
    {
        assert_equals(()=>expected,()=>actual,f,a);
    }
    public void assert_equals<T>(T expected, Expression<Func<T>> actual, string f = null, params object[] a)
    {
        assert_equals(()=>expected,actual,f,a);
    }
    public void assert_equals<T>( Expression<Func<T>> expected
                                 ,Expression<Func<T>> actual
                                 ,string              format = null
                                 ,params object[]     args) 
    { 
        var f = expected.Compile();
        var g = actual.Compile();
        var x = f();
        var y = g();

        var _x = expected.Body is ConstantExpression || expected.Body.NodeType == ExpressionType.MemberAccess ? exprStr(expected.Body) : $"{exprStr(expected.Body)} ⩵ «{x}»";
        var _y = actual.Body is ConstantExpression || actual.Body.NodeType == ExpressionType.MemberAccess ? exprStr(actual.Body) : $"{exprStr(actual.Body)} ⩵ «{y}»";
        Write($"assert EQUAL: ({_x}) == ({_y})");

        if (format == null && !x.Equals(y))
        {
            Write("FAIL!");
            Write(string.Format(format,args));
        }
        
        Assert.Equal(x,y);
    } 

    //assert_true()
    public void assert_true(bool b) => Assert.True(b);
    public void assert_true(Expression<Func<bool>> e, Expression<Func<object>> ifFail = null) 
    { 
        var b = e.Body;
        Write("assert TRUE: {0}",exprStr(e.Body));

        switch(b.NodeType)
        { 
            case ExpressionType.Equal:
            {
                var be = (BinaryExpression) b;
                var l1 = Expression.Lambda(be.Left);
                var l2 = Expression.Lambda(be.Right);
                var f1 = l1.Compile();
                var f2 = l2.Compile();
                var v1 = f1.DynamicInvoke();
                var v2 = f2.DynamicInvoke();

                if (ifFail!=null && !v1.Equals(v2))
                {
                    Write("FAIL!");
                    Write(exprStr(ifFail));
                    Write(ifFail.Compile()());
                }
            
                Assert.Equal(v2,v1);
                break;
            }

            default: 
                var f = e.Compile();
                var x = f();

                if (ifFail!=null && !x)
                {
                    Write("FAIL!");
                    Write(exprStr(ifFail));
                    Write(ifFail.Compile()());
                }

                Assert.True(x);
                break;
        }
    } 

    void onFail(string f, object[] a = null)
    {
        Write("FAIL!");
        if (a != null)
            Write(string.Format(f,a));
        else
            Write(f);
    }
   
    //assert_type()
    public void assert_type<T>(Expression<Func<object>> e, Expression<Func<object>> msgIfFail = null)
    {
        var t = typeof(T);
        Write("assert TYPE({0}): {1}",t,exprStr(e.Body));
        var f = e.Compile();
        var x = f();

        if (msgIfFail != null && !t.IsInstanceOfType(x))
        {
            var msg = objStr(msgIfFail.Compile().Invoke());
            onFail(msg);
            Assert.IsType(t,x);
        }
        else
        {
            Assert.IsType(t,x);
        }
    }

    //assert_not_type()
    public void assert_not_type<T>(Expression<Func<object>> e)
    {
        var t = typeof(T);
        Write("assert NOT OF TYPE({0}): {1}",t,exprStr(e.Body));
        var f = e.Compile();
        var x = f();
        Assert.IsNotType(t,x);
    }

    //assert_false()
    public void assert_false(Expression<Func<bool>> e, string msgIfTrue = null) 
    { 
        Write("assert FALSE: {0}",exprStr(e.Body));
        var f = e.Compile();
        var x = f();

        if (msgIfTrue != null && !x)
        {
            onFail(msgIfTrue);
            Assert.False(x);
        }
        else
            Assert.False(x);
    } 

   
    //ASSERT_NULL()
    public void assert_null<T>(Expression<Func<T>> e) where T : class
    { 
        Write("assert NULL: {0}",exprStr(e.Body));
        var f = e.Compile();
        var x = f();
        Assert.Null(x);
    }
   
    //AssertNotNull
    public void assert_not_null<T>(T o) 
        => Assert.NotNull(o);
    public void assert_not_null<T>(Expression<Func<T>> e, Expression<Func<object>> ifFail = null) where T : class
    { 
        var _str = exprStr(e.Body);
        Write("assert NOT NULL: {0}",_str);
        var f = e.Compile();
        var x = f();

        if (ifFail!=null && x==null)
        {
            Write("FAIL!");
            Write(exprStr(ifFail));
            Write(ifFail.Compile()());
        }

        Assert.NotNull(x);
    }

   
    //Setup()
    protected virtual void Setup()
    { 

    }

    //Wait()
    protected internal void Wait(float seconds)
    {
        Thread.Sleep((int)(seconds*1000f));
    }
   
    //Write()
    protected internal void Write(string msg)
    {
        Console.WriteLine("[{0:yyyy-MM-dd hh:mm:ss.fff}] {1}",DateTime.Now, msg);
    }
    protected internal void Write(string f, params object[] args)
    { 
        Console.WriteLine("[{0:yyyy-MM-dd hh:mm:ss.fff}] {1}",DateTime.Now, string.Format(f,args));
    }

    //Write()
    protected internal void Write<T>(Expression<Func<T>> exp)
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff}] {exprStr(exp.Body).Replace("{","{{").Replace("}","}}")} == {objStr(exp.Compile()()).Replace("{","{{").Replace("}","}}")}");
    }
   
    //Write()
    protected internal void Write(object o)
    { Console.WriteLine("[{0:yyyy-MM-dd hh:mm:ss.fff}] {1}",DateTime.Now,objStr(o));
    }


    //objStr()
    string objStr(object o)
    {
        if (o == null)
            return "<null>";

        var e = o as IEnumerable;
        if (e != null && !(o is string))
            return "["+string.Join(", ",e.Cast<object>().Select(objStr))+"]";

        var t = o.GetType();

        return    (t == typeof(string)) 
                    ? $"\"{o}\""

                : (t == typeof(int))
                    ? o.ToString()

                : (t.Name.Contains("DisplayClass")) 
                    ? ""

                : (t.Name.StartsWith("Func`"))
                    ? "<func>" 

                : o.ToString()+$" ({t})";
    }



    //exprString() 
    string exprStr(Expression e)
    {
        if (e==null) 
            return "<null expression>";

        switch(e.NodeType)
        {
            //Add (+)
            case Add: return binaryStr(e,"+");

            //AndAlso (&&)
            case AndAlso: return binaryStr(e,"&&");


            //Call
            case Call:
            {
                var mce = (MethodCallExpression) e;
                var m   = mce.Method;

                return    (m.Name=="get_Item") 
                            ? exprStr(mce.Object)+"."+"["+exprStr(mce.Arguments[0])+"]" 
                            
                        : (m.DeclaringType==typeof(Enumerable)) 
                            ? $"{exprStr(mce.Arguments[0])}.{mce.Method.Name}({string.Join(",",mce.Arguments.Skip(1).Select(exprStr))})" 

                        : (m.IsStatic) 
                            ? m.DeclaringType.Name 
                        
                        : exprStr(mce.Object)+"."+m.Name+"("+string.Join(",",mce.Arguments.Select(exprStr))+")";
        
            }

            //Constant
            case Constant: 
            {
                var ce = (ConstantExpression) e;
                return objStr(ce.Value);
            }

            //Convert
            case ExpressionType.Convert:
            {
                var ue = (UnaryExpression) e;
                return "(("+ue.Type.Name+") "+exprStr(ue.Operand)+")";
            }

            //Equal (==)
            case Equal: return binaryStr(e,"==");

            //Invoke()
            case Invoke: 
            {
                var ie = (InvocationExpression) e;
                return $"{exprStr(ie.Expression)}({string.Join(",",ie.Arguments.Select(exprStr))})";
            }
            

            //Lambda
            case Lambda:
            {
                var le = (LambdaExpression) e;
                var ps = le.Parameters.Count==1 ? 
                            le.Parameters[0].Name : 
                            $"({string.Join(", ",le.Parameters.Select(_=>_.Name))})";
                return $"{ps} => {exprStr(le.Body)}";
            }

            //Member
            case MemberAccess:
            {
                var mae = (MemberExpression) e;
                var m   = mae.Member;
                var o   = exprStr(mae.Expression);

                return (m.Name=="expected" || m.Name=="actual") 
                            ? objStr(getValue(mae)) 

                        : (   m.DeclaringType.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute))
                            || o.Length == 0) 
                            ? (mae.Expression.NodeType!= Parameter? $"{m.Name} ⩵ «{objStr(getValue(mae))}»" : m.Name) 

                        : (mae.Expression.NodeType!= Parameter ? $"{o}.{m.Name} ⩵ «{objStr(getValue(mae))}»" : $"{o}.{m.Name}");
            }

            //New
            case New:
            {
                var ne = (NewExpression) e;
                if (ne.Members.Count > 0)
                    return $"new {{{string.Join(", ",ne.Members.Select((m,i)=>$"{m.Name}={exprStr(ne.Arguments[i])}"))}}}";
                goto default;
            }

            //Parameter
            case Parameter: return ((ParameterExpression)e).Name;

            //DEFAULT
            default: 
                Write("exprString(): "+e.NodeType) ; 
                throw new NotSupportedException("exprString(): "+e.NodeType);
                //return e.ToString();
        }
    }

    object getValue(MemberExpression me)
    {
        var m = me.Member;
        var p = m as PropertyInfo;
        var o = Expression.Lambda(me.Expression).Compile().DynamicInvoke();

        return
            (p != null) ? 
                p.GetValue(o) :

            ((FieldInfo)m).GetValue(o);
    }

    string binaryStr(Expression e, string op)
    {
        var be = (BinaryExpression) e;
        return $"{exprStr(be.Left)} {op} {exprStr(be.Right)}";
    }

    //testEmailAddress()
    public string testEmailAddress 
    { get 
        { if (m_testEmailAddress == null)  m_testEmailAddress = "test@example.com";
            return m_testEmailAddress; 
        }
    } string m_testEmailAddress;

}

//TestContextTextWriter class
public class TestContextTextWriter : TextWriter
{
    readonly Test test;

    List<char> charBuffer   = new List<char>();
    List<string> lineBuffer = new List<string>(); //if Buffered = true

    //.ctor (TestContextTextWriter)
    internal TestContextTextWriter(Test test) 
    { this.test = test;
        Buffered = false;
        BufferedMaxOutput = 5;
    }

    //BufferedMaxOutput
    /// <summary>Maximum number of lines to write if Buffered = true</summary>
    public int BufferedMaxOutput {get; set;}

    //Buffered
    /// <summary>If true, output is buffered until end of test. Calling Flush()
    /// won't flush the buffer.</summary>
    public bool Buffered {get; set;}

    public override void Write(char value) 
    { if (Buffered) lineBuffer[lineBuffer.Count-1] += value;      
        else charBuffer.Add(value); 
    }

    public override void Flush() 
    { if (charBuffer.Count==0) return;
        WriteLine(new string(charBuffer.ToArray()));
        charBuffer.Clear();
    }

    public override void WriteLine()
    { Flush();
        if (Buffered) Write(new string(base.CoreNewLine));
        else test.Write("");       
    }

    public override void WriteLine(string value)
    { Flush();
        if (Buffered) lineBuffer.Add(value);
        else test.Write(value);
    }
   
    public override Encoding  Encoding {get {  return Encoding.Unicode; }}

    //called from test.Cleanup()
    internal void flush2()
    { foreach(var s in lineBuffer.Skip(lineBuffer.Count-BufferedMaxOutput)) test.Write(s);
    }
}
}