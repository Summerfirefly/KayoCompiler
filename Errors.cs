using System;
using System.Collections.Generic;
using System.Text;

namespace KayoCompiler
{
    class Error
    {
        private readonly int lineNum = 0;

        public Error(int lineNum)
        {
            this.lineNum = lineNum;
        }

        public void PrintErrMsg()
        {
            Console.WriteLine(this);
        }

        public override string ToString()
        {
            return $"[ERROR] An error occurred at line {lineNum}";
        }
    }
}
