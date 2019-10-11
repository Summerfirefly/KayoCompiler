namespace KayoCompiler.Ast
{
    abstract class TerminalNode : AstNode
    {
        public abstract VarType Type();
    }

    class IntNode : TerminalNode
    {
        public int value;

        public IntNode(int num)
        {
            value = num;
        }

        public override string Gen()
        {
            CodeGenData.stackDepth++;
            switch (CodeGenData.stackDepth)
            {
                case 1:
                    return $"movq\t${value}, %rax\n";
                case 2:
                    return $"movq\t${value}, %rdx\n";
                case 3:
                    return $"movq\t${value}, %rcx\n";
                default:
                    return $"pushq\t${value}\n";
            }
        }

        public override VarType Type()
        {
            return VarType.TYPE_INT;
        }
    }

    class BoolNode : TerminalNode
    {
        public int value;

        public BoolNode(bool value)
        {
            this.value = value ? 1 : 0;
        }

        public override string Gen()
        {
            CodeGenData.stackDepth++;
            switch (CodeGenData.stackDepth)
            {
                case 1:
                    return $"movq\t${value}, %rax\n";
                case 2:
                    return $"movq\t${value}, %rdx\n";
                case 3:
                    return $"movq\t${value}, %rcx\n";
                default:
                    return $"pushq\t${value}\n";
            }
        }

        public override VarType Type()
        {
            return VarType.TYPE_BOOL;
        }
    }

    class IdNode : TerminalNode
    {
        public string name;

        public IdNode(string name)
        {
            this.name = name;
        }

        public override string Gen()
        {
            int index = SymbolTable.GetVarIndex(name, CodeGenData.currentField);
            int offset = (index + 1) * 8;
            CodeGenData.stackDepth++;
            switch (CodeGenData.stackDepth)
            {
                case 1:
                    return $"movq\t-{offset}(%rbp), %rax\n";
                case 2:
                    return $"movq\t-{offset}(%rbp), %rdx\n";
                case 3:
                    return $"movq\t-{offset}(%rbp), %rcx\n";
                default:
                    return $"pushq\t-{offset}(%rbp)\n";
            }
        }

        public override VarType Type()
        {
            TableVarItem? item = SymbolTable.FindVar(name, Parser.CurrentField);
            return item?.type ?? VarType.TYPE_ERROR;
        }
    }
}
