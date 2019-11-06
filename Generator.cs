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

        internal bool Generate()
        {
            var program = parser.Parse();

            if (CodeGenUtils.ErrorNum == 0)
            {
                StreamWriter sw = new StreamWriter(new FileStream(tmpFilePath, FileMode.Create));
                string code = string.Empty;
                bool hasMain = false;

                foreach (string name in SymbolTable.GetGlobalFun())
                {
                    code += $"GLOBAL {name}\n";
                    if (name == "main")
                    {
                        hasMain = true;
                        code += "GLOBAL _start\n";
                    }
                }
                if (CodeGenUtils.HasWrite)
                {
                    code += "EXTERN write_num\n";
                    code += "EXTERN write_bool\n";
                }
                if (CodeGenUtils.HasRead)
                {
                    code += "EXTERN read_num\n";
                    code += "EXTERN read_bool\n";
                }
                foreach (string name in SymbolTable.GetExternFun())
                {
                    code += $"EXTERN {name}\n";
                }

                code += "SECTION .text\n";
                if (hasMain)
                {
                    code += "_start:\n";
                    code += "call\t$main\n";
                    code += "mov\trax, 60\n";
                    code += "mov\trdi, 0\n";
                    code += "syscall\n";
                }

                code += program.Gen();

                sw.Write(code);
                sw.Flush();
                sw.Close();
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{CodeGenUtils.ErrorNum} errors, stop");
                Console.ResetColor();
                return false;
            }
        }
    }
}
