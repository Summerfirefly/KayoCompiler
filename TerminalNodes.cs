namespace KayoCompiler.Ast
{
    abstract class TerminalNode : AstNode
    {
        public abstract VarType Type();
    }

    class IntNode : TerminalNode
    {
        public long value;

        public IntNode(long num)
        {
            value = num;
        }

        public override string Gen()
        {
            string code = string.Empty;

            CodeGenUtils.StackDepth++;
            if (CodeGenUtils.StackDepth > 3)
            {
                code += $"push\t{CodeGenUtils.CurrentStackTop}\n";
            }

            code += $"mov\t{CodeGenUtils.CurrentStackTop}, {value}\n";

            return code;
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
            string code = string.Empty;

            CodeGenUtils.StackDepth++;
            if (CodeGenUtils.StackDepth > 3)
            {
                code += $"push\t{CodeGenUtils.CurrentStackTop}\n";
            }

            code += $"mov\t{CodeGenUtils.CurrentStackTop}, {value}\n";

            return code;
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
            string code = string.Empty;
            int index = SymbolTable.GetVarIndex(name);
            int offset = -(index + 1) * 8;

            CodeGenUtils.StackDepth++;
            if (CodeGenUtils.StackDepth > 3)
            {
                code += $"push\t{CodeGenUtils.CurrentStackTop}\n";
            }

            code += $"mov\t{CodeGenUtils.CurrentStackTop}, [rbp{(offset>0?"+":"")}{offset}]\n";

            return code;
        }

        public override VarType Type()
        {
            VarSymbol item = SymbolTable.FindVar(name);
            return item?.type ?? VarType.TYPE_ERROR;
        }
    }
}
