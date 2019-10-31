using System;
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

            string tmpFilename = args[0].Substring(0, args[0].LastIndexOf('.')) + ".asm";
            if (new Generator(new Parser(new Scanner(args[0])), tmpFilename).Generate())
            {
                Process nasm = new Process();
                nasm.StartInfo.FileName = "nasm";
                nasm.StartInfo.Arguments = $"-felf64 {tmpFilename}";
                nasm.Start();
                nasm.WaitForExit();
                nasm.Close();
            }
        }
    }
}
