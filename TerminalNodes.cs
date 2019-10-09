using System;

namespace KayoCompiler.Ast
{
    abstract class TerminalNode : AstNode
    {
        public abstract Type Type();
    }

    class IntNode : TerminalNode
    {
        public int value;

        public IntNode(int num)
        {
            value = num;
        }

        public override void Gen()
        {
            Console.WriteLine($"push {value}");
        }

        public override Type Type()
        {
            return Ast.Type.TYPE_INT;
        }
    }

    class BoolNode : TerminalNode
    {
        public int value;

        public BoolNode(bool value)
        {
            this.value = value ? 1 : 0;
        }

        public override void Gen()
        {
            Console.WriteLine($"push {value}");
        }

        public override Type Type()
        {
            return Ast.Type.TYPE_BOOL;
        }
    }

    class IdNode : TerminalNode
    {
        public string name;

        public IdNode(string name)
        {
            this.name = name;
        }

        public override void Gen()
        {
            Console.WriteLine($"push [{name}]");
        }

        public override Type Type()
        {
            throw new NotImplementedException();
        }
    }
}
