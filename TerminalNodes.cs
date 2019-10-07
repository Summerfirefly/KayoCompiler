using System;

namespace KayoCompiler.Ast
{
    abstract class TerminalNode : AstNode
    {
        public abstract override void Gen();
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
    }
}
