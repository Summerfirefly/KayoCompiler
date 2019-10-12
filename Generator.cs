using System;
using System.IO;

namespace KayoCompiler
{
    class Generator
    {
        private readonly Parser parser;
        private readonly string tmpFilePath;

        internal Generator(Parser parser, string tmpFilePath = "tmp.asm")
        {
            this.parser = parser;
            this.tmpFilePath = tmpFilePath;
        }

        internal void Generate()
        {
            var program = parser.Parse();

            if (CodeGenUtils.ErrorNum == 0)
            {
                StreamWriter sw = new StreamWriter(new FileStream(tmpFilePath, FileMode.Create));
                string code = program.Gen();
                sw.Write(code);
                sw.Flush();
                sw.Close();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{CodeGenUtils.ErrorNum} errors, stop");
                Console.ResetColor();
            }
        }
    }
}
