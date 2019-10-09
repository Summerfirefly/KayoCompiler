using KayoCompiler.Ast;

namespace KayoCompiler
{
    class Debug
    {
        internal static void ParserDebug(string testFilePath)
        {
            Parser parser = new Parser(new Scanner(testFilePath));
            ProgramNode program = parser.Parse();
            program.Gen();
        }
    }
}
