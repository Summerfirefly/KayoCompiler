using System.IO;

namespace KayoCompiler
{
    class Debug
    {
        internal static void ScannerDebug(string testFilePath, string outFilePath)
        {
            Scanner scanner = new Scanner(testFilePath);
            var tokens = scanner.Scan();

            StreamWriter sw = new StreamWriter(new FileStream(outFilePath, FileMode.OpenOrCreate));

            foreach (var item in tokens)
            {
                sw.Write(item.ToString());
            }

            sw.Flush();
            sw.Close();
        }
    }
}
