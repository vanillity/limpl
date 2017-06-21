using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limpl.Syntax
{
/// <summary>A list of syntax nodes.</summary>
/// <typeparam name="TNode"></typeparam>
public class SyntaxList<TNode> :  IReadOnlyList<TNode> where TNode : ISyntaxNode
{
    readonly ISyntaxNode owner;
    ImmutableList<TNode> nodes = ImmutableList<TNode>.Empty;

    public SyntaxList(ISyntaxNode owner, IEnumerable<TNode> nodes, Action<TNode,ISyntaxNode> setParent)
    {
        this.owner = owner;

        if (nodes == null)
            return;
        
        var nodeList = new List<TNode>();
        foreach(var node in nodes)
        {
            if (node == null)
                continue;

            // - this might give a false negative where the parent wasn't changed
            //   but the child nodes have changed (e.g., in another thread)... maybe?
            if (node.Parent != null)
            {                
               var _node = (TNode) node.Clone();
               setParent(_node,owner);
               nodeList.Add(_node);
            }
            else
            {
               setParent(node,owner);
               nodeList.Add(node);
            }
        }

        Debug.Assert(nodeList.TrueForAll(_=>_.Parent==(object)owner));
        this.nodes = this.nodes.AddRange(nodeList);
    }

    public TNode this[int index]
    {
        get
        {
            return nodes[index];
        }
    }

    public int Count
    {
        get
        {
            return nodes.Count;
        }
    }


    public IEnumerator<TNode> GetEnumerator()
    {
        return nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return nodes.GetEnumerator();
    }

    internal void append(IToken endToken)
    {
        nodes = nodes.Add((TNode)(object)endToken);
    }
}
}

