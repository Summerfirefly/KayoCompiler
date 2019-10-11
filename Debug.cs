using System;
using KayoCompiler.Ast;

namespace KayoCompiler
{
    class Debug
    {
        internal static void Run(string testFilePath)
        {
            Parser parser = new Parser(new Scanner(testFilePath));
            Generator generator = new Generator(parser);
            generator.Generate();

            for (int i = 0; i < SymbolTable.VarCount; i++)
            {
                var item = SymbolTable.GetVar(i);
                Console.WriteLine($"{item.name} {item.type} {item.field}");
            }
        }
    }
}
