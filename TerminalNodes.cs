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
            CodeGenUtils.StackDepth++;
            switch (CodeGenUtils.StackDepth)
            {
                case 1:
                    return $"mov\trax, {value}\n";
                case 2:
                    return $"mov\trdx, {value}\n";
                case 3:
                    return $"mov\trcx, {value}\n";
                default:
                    return $"push\tqword {value}\n";
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
            CodeGenUtils.StackDepth++;
            switch (CodeGenUtils.StackDepth)
            {
                case 1:
                    return $"mov\trax, {value}\n";
                case 2:
                    return $"mov\trdx, {value}\n";
                case 3:
                    return $"mov\trcx, {value}\n";
                default:
                    return $"push\tqword {value}\n";
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
            int index = SymbolTable.GetVarIndex(name);
            int offset = -(index + 1) * 8;
            CodeGenUtils.StackDepth++;
            switch (CodeGenUtils.StackDepth)
            {
                case 1:
                    return $"mov\trax, [rbp{(offset>0?"+":"")}{offset}]\n";
                case 2:
                    return $"mov\trdx, [rbp{(offset>0?"+":"")}{offset}]\n";
                case 3:
                    return $"mov\trcx, [rbp{(offset>0?"+":"")}{offset}]\n";
                default:
                    return $"push\tqword [rbp{(offset>0?"+":"")}{offset}]\n";
            }
        }

        public override VarType Type()
        {
            VarSymbol item = SymbolTable.FindVar(name);
            return item?.type ?? VarType.TYPE_ERROR;
        }
    }
}
