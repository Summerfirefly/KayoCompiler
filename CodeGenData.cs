namespace KayoCompiler
{
    internal static class CodeGenData
    {
        internal static int LabelNum { get; set; } = 1;
        internal static int CurrentField { get; set; } = 0;
        internal static int StackDepth { get; set; } = 0;
    }
}
