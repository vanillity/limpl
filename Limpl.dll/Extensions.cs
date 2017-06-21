using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Limpl
{
public static class Extensions
{
    public static void SetInput(this IScanner<char> scanner, IEnumerable<char> input)
      => scanner.SetInput(input);
}
}
