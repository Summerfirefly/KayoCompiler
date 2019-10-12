namespace KayoCompiler
{
    internal static class CodeGenUtils
    {
        internal static int ErrorNum { get; set; } = 0;
        internal static int LabelNum { get; set; } = 1;
        internal static int CurrentField { get; set; } = 0;
        internal static int StackDepth { get; set; } = 0;
        internal static bool HasWrite { get; set; } = false;

        internal static string CurrentStackTop
        {
            get
            {
                switch (StackDepth)
                {
                    case 0:
                        return string.Empty;
                    case 1:
                        return "rax";
                    case 2:
                        return "rdx";
                    case 3:
                        return "rcx";
                    default:
                        return "[rsp]";
                }
            }
        }
    }
}
