using System;

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

            new Generator(new Parser(new Scanner(args[0]))).Generate();
        }
    }
}
