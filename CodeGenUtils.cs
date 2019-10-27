namespace KayoCompiler
{
    internal static class CodeGenUtils
    {
        internal static int ErrorNum { get; set; } = 0;
        internal static int LabelNum { get; set; } = 1;
        internal static int StackDepth { get; set; } = 0;
        internal static bool HasWrite { get; set; } = false;
        internal static bool HasRead { get; set; } = false;

        internal static string CurrentStackTop64
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

        internal static string CurrentStackTop32
        {
            get
            {
                switch (StackDepth % 3)
                {
                    case 0:
                        return "r11d";
                    case 1:
                        return "eax";
                    case 2:
                        return "r10d";
                    default:
                        return string.Empty;
                }
            }
        }

        internal static string CurrentStackTop8
        {
            get
            {
                switch (StackDepth % 3)
                {
                    case 0:
                        return "r11b";
                    case 1:
                        return "al";
                    case 2:
                        return "r10b";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
