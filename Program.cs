﻿using System;
using System.Diagnostics;

namespace KayoCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No file input");
                return;
            }

            if (new Generator(new Parser(new Scanner(args[0]))).Generate())
            {
                Process nasm = new Process();
                nasm.StartInfo.FileName = "nasm";
                nasm.StartInfo.Arguments = "-felf64 tmp.asm";
                nasm.Start();
                nasm.WaitForExit();
                nasm.Close();

                Process ld = new Process();
                ld.StartInfo.FileName = "ld";
                ld.StartInfo.Arguments = "-o test tmp.o write.o read.o";
                ld.Start();
                ld.WaitForExit();
                ld.Close();
            }
        }
    }
}
