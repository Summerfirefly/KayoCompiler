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
            return $"push {value}\n";
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
            return $"push {value}\n";
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
            return $"push [{name}]\n";
        }

        public override VarType Type()
        {
            TableVarItem? item = SymbolTable.FindVar(name, Parser.CurrentField);
            return item?.type ?? VarType.TYPE_ERROR;
        }
    }
}
