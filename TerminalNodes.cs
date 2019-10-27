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
            return VarType.TYPE_LONG;
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
            int offset = -SymbolTable.GetVarOffset(name);

            CodeGenUtils.StackDepth++;
            if (CodeGenUtils.StackDepth > 3)
            {
                code += $"push\t{CodeGenUtils.CurrentStackTop}\n";
            }

            switch (SymbolTable.FindVar(name).type)
            {
                case VarType.TYPE_BOOL:
                    code += $"xor\t{CodeGenUtils.CurrentStackTop}, {CodeGenUtils.CurrentStackTop}\n";
                    code += $"mov\t{CodeGenUtils.CurrentStackTop8}, [rbp{(offset>0?"+":"")}{offset}]\n";
                    break;
                case VarType.TYPE_INT:
                    code += $"mov\t{CodeGenUtils.CurrentStackTop32}, [rbp{(offset>0?"+":"")}{offset}]\n";
                    if (CodeGenUtils.CurrentStackTop32 == "eax")
                    {
                        code += "cdqe\n";
                    }
                    else
                    {
                        code += $"xchg\trax, {CodeGenUtils.CurrentStackTop}\n";
                        code += "cdqe\n";
                        code += $"xchg\trax, {CodeGenUtils.CurrentStackTop}\n";
                    }
                    break;
                case VarType.TYPE_LONG:
                    code += $"mov\t{CodeGenUtils.CurrentStackTop}, [rbp{(offset>0?"+":"")}{offset}]\n";
                    break;
            }

            return code;
        }

        public override VarType Type()
        {
            VarSymbol item = SymbolTable.FindVar(name);
            return item?.type ?? VarType.TYPE_ERROR;
        }
    }
}
