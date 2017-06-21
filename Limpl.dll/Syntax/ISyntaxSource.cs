using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limpl
{
public interface ISyntaxSource<TSyntax, TInput> where TSyntax : ISyntaxNode
{
    TSyntax CreateNode(TInput input);
}
}
