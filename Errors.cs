﻿using System;
using KayoCompiler.Ast;

namespace KayoCompiler.Errors
{
    class Error
    {
        protected readonly int lineNum = 0;

        public Error()
        {
            lineNum = Scanner.LineNum;
            CodeGenUtils.ErrorNum++;
        }

        public virtual void PrintErrMsg()
        {
            PrintErrMsg($"error: An error occurred at line {lineNum}");
        }

        protected void PrintErrMsg(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }

    class ConflictingDeclarationError : Error
    {
        public override void PrintErrMsg()
        {
            PrintErrMsg($"error: conflicting declaration at line {lineNum}");
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

            PrintErrMsg($"error: missing {missingToken} at line {lineNum}");
        }
    }

    class TypeMismatchError : Error
    {
        readonly VarType typeA;
        readonly VarType typeB;

        public TypeMismatchError(VarType typeA, VarType typeB)
        {
            this.typeA = typeA;
            this.typeB = typeB;
        }

        public override void PrintErrMsg()
        {
            PrintErrMsg($"error: type mismatch between {GetTypeStr(typeA)} and {GetTypeStr(typeB)} at line {lineNum}");
        }

        private string GetTypeStr(VarType type)
        {
            if (type == VarType.TYPE_INT)
                return "int";

            if (type == VarType.TYPE_BOOL)
                return "bool";

            return "unknown";
        }
    }
}
