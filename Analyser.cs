using System;
using System.Collections.Generic;
using System.Text;
using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
    internal class Analyser
    {
        private readonly ProgramNode program;

        internal Analyser(Parser parser)
        {
            program = parser.Parse();
        }

        internal void Analysis()
        {

        }
    }
}
