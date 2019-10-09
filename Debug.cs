using System;
using KayoCompiler.Ast;

namespace KayoCompiler
{
    class Debug
    {
        internal static void ParserDebug(string testFilePath)
        {
            Parser parser = new Parser(new Scanner(testFilePath));
            ProgramNode program = parser.Parse();
            //Console.WriteLine(program.Gen());
            for (int i = 0; i < SymbolTable.VarCount; i++)
            {
                var item = SymbolTable.GetVar(i);
                Console.WriteLine($"{item.name} {item.type} {item.field}");
            }
        }
    }
}
