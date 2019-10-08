using System;
using System.Collections.Generic;
using System.Text;

namespace KayoCompiler.Errors
{
    class Error
    {
        protected readonly int lineNum = 0;

        public Error()
        {
            lineNum = Scanner.LineNum;
        }

        public virtual void PrintErrMsg()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"error: An error occurred at line {lineNum}");
            Console.ResetColor();
        }
    }

    class TokenMissingError : Error
    {
        readonly Tag missingTokenTag;

        public TokenMissingError(Tag missingTokenTag = Tag.NULL)
        {
            this.missingTokenTag = missingTokenTag;
        }

        public override void PrintErrMsg()
        {
            string missingToken = "token";

            switch (missingTokenTag)
            {
                case Tag.DL_SET:
                    missingToken = "=";
                    break;
                case Tag.DL_SEM:
                    missingToken = ";";
                    break;
                case Tag.DL_RPAR:
                    missingToken = ")";
                    break;
                case Tag.DL_RBRACE:
                    missingToken = "}";
                    break;
                case Tag.DL_LPAR:
                    missingToken = "(";
                    break;
                case Tag.DL_LBRACE:
                    missingToken = "{";
                    break;
                case Tag.ID:
                    missingToken = "identifier";
                    break;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"error: missing {missingToken} at line {lineNum}");
            Console.ResetColor();
        }
    }
}
