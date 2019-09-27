using System.IO;

namespace KayoCompiler
{
    class Debug
    {
        internal static void ScannerDebug(string testFilePath, string outFilePath)
        {
            Scanner scanner = new Scanner(testFilePath);
            StreamWriter sw = new StreamWriter(new FileStream(outFilePath, FileMode.OpenOrCreate));

            for (Token token = scanner.NextToken(); token != null; token = scanner.NextToken())
            {
                sw.Write(token.ToString());
            }

            sw.Flush();
            sw.Close();
        }
    }
}
