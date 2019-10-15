﻿using System;
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
                string code = string.Empty;

                code += "GLOBAL _start\n";
                if (CodeGenUtils.HasWrite)
                    code += "EXTERN write\n";
                code += "SECTION .text\n";
                code += "_start:\n";
                code += "call\t_entry\n";
                code += "mov\trax, 60\n";
                code += "mov\trdi, 0\n";
                code += "syscall\n";

                code += "_entry:\n";
                code += "push\trbp\n";
                code += "mov\trbp, rsp\n";
                code += $"sub\trsp, {SymbolTable.CurFunVarCount * 8}\n";

                code += program.Gen();

                code += "mov\trsp, rbp\n";
                code += "pop\trbp\n";
                code += "ret\n";

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
