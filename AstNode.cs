using System;
using System.Collections.Generic;

namespace KayoCompiler.Ast
{
    abstract class AstNode
    {
        public abstract void Gen();
    }

    class ProgramNode : AstNode
    {
        List<AstNode> children;

        public override void Gen()
        {
            foreach (var child in children)
            {
                child.Gen();
            }
        }

        public void AddChild(AstNode child)
        {
            if (children == null)
                children = new List<AstNode>();

            children.Add(child);
        }
    }

    class ExprNode : AstNode
    {
        public TermNode term;
        public ExprTail expr;

        public override void Gen()
        {
            term?.Gen();
            if (expr?.op != null)
            {
                expr?.term?.Gen();

                if (expr.op == "+")
                    Console.WriteLine("add");
                else if (expr.op == "-")
                    Console.WriteLine("sub");

                expr?.Gen();
            }
        }
    }
    
    class ExprTail : AstNode
    {
        public string op;
        public TermNode term;
        public ExprTail expr;

        public override void Gen()
        {
            if (expr?.op != null)
            {
                expr?.term?.Gen();

                if (expr.op == "+")
                    Console.WriteLine("add");
                else if (expr.op == "-")
                    Console.WriteLine("sub");

                expr?.Gen();
            }
        }
    }

    class TermNode : AstNode
    {
        public FactorNode factor;
        public TermTail term;

        public override void Gen()
        {
            factor?.Gen();
            if (term?.op != null)
            {
                term?.factor?.Gen();

                if (term.op == "*")
                    Console.WriteLine("mul");
                else if (term.op == "/")
                    Console.WriteLine("div");

                term?.Gen();
            }
        }
    }

    class TermTail : AstNode
    {
        public string op;
        public FactorNode factor;
        public TermTail term;

        public override void Gen()
        {
            if (term?.op != null)
            {
                term?.factor?.Gen();

                if (term.op == "*")
                    Console.WriteLine("mul");
                else if (term.op == "/")
                    Console.WriteLine("div");

                term?.Gen();
            }
        }
    }

    class FactorNode : AstNode
    {
        public string value;

        public override void Gen()
        {
            if (value != null)
                Console.WriteLine($"push {value}");
        }
    }
}
