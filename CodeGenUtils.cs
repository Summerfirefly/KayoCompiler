namespace KayoCompiler
{
    internal static class CodeGenUtils
    {
        internal static int ErrorNum { get; set; } = 0;
        internal static int LabelNum { get; set; } = 1;
        internal static int StackDepth { get; set; } = 0;
        internal static bool HasWrite { get; set; } = false;
        internal static bool HasRead { get; set; } = false;

        internal static string CurrentStackTop
        {
            get
            {
                switch (StackDepth % 3)
                {
                    case 0:
                        return "r11";
                    case 1:
                        return "rax";
                    case 2:
                        return "r10";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
