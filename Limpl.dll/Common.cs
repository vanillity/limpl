using System;
using System.Collections.Generic;
using System.Text;

namespace Limpl
{
public static class Common
{
    public static class TokenKinds
    {
        //see http://source.roslyn.io/#Microsoft.CodeAnalysis.CSharp/Syntax/SyntaxKind.cs
        public const int Other = 100;
        public const int Unspecified = 0;
        public const int StartOfFile = 1;
        public const int EndOfFile = 8496;
        public const int EndOfLine = 4;
                
        public const int Space =8540;
        public const int AtSymbol= 5;
                
        public const int SemiColon= 8212; 
        public const int LessThan=8215;
        public const int GreaterThan=8217;
        public const int NumericLiteral= 8509; 
        public const int StringLiteral= 8511;
        public const int Colon=8211;
        public const int OpenBrace= 8205;
        public const int OpenParenthesis= 8200;
        public const int CloseBrace= 8206;
        public const int CloseParenthesis= 8201;
        public const int OpenBracket = 8207;
        public const int CloseBracket = 8208;
        public const int Dot= 8218;
        public const int DotDot= 6;
        public const int Ellipsis= 7;
        public const int Comma= 8216;
        public const int Plus = 8203;
        public const int Asterisk = 8199;
    }

    public static class TokenRules
    {
        public static bool MatchesStartOfFile(Scanner<char> scanner, int position) => scanner.Position<0 && position<1;
        public static bool MatchesEndOfFile(Scanner<char> scanner, int position) => scanner.End;
    }
}
}
