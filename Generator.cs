using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace KayoCompiler
{
    class Generator
    {
        private readonly Parser parser;
        private readonly string tmpFilePath;

        internal Generator(Parser parser, string tmpFilePath = "tmp.code")
        {
            this.parser = parser;
            this.tmpFilePath = tmpFilePath;
        }

        internal void Generate()
        {
            StreamWriter sw = new StreamWriter(new FileStream(tmpFilePath, FileMode.Create));
            string code = parser.Parse().Gen();
            sw.Write(code);

            sw.Flush();
            sw.Close();
        }
    }
}
