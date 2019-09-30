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
        List<BlockNode> children;

        public override void Gen()
        {
            if (children == null) return;
            foreach (var child in children)
            {
                child.Gen();
            }
        }

        public void AddChild(BlockNode child)
        {
            if (children == null)
                children = new List<BlockNode>();

            children.Add(child);
        }
    }

    class BlockNode : AstNode
    {
        List<AstNode> children;

        public override void Gen()
        {
            if (children == null) return;
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

    class DeclsNode : AstNode
    {
        List<DeclNode> decls;

        public override void Gen()
        {
            if (decls == null) return;
            foreach (var decl in decls)
            {
                decl.Gen();
            }
        }

        public void AddChild(DeclNode decl)
        {
            if (decls == null)
                decls = new List<DeclNode>();

            decls.Add(decl);
        }
    }

    class DeclNode : AstNode
    {
        public string type;
        public string name;

        public override void Gen()
        {
        }
    }

    class StmtsNode : AstNode
    {
        List<StmtNode> children;

        public override void Gen()
        {
            if (children == null) return;
            foreach (var child in children)
            {
                child.Gen();
            }
        }

        public void AddChild(StmtNode child)
        {
            if (children == null)
                children = new List<StmtNode>();

            children.Add(child);
        }
    }

    class StmtNode : AstNode
    {
        public AstNode stmt;

        public override void Gen()
        {
            stmt?.Gen();
        }
    }

    class WriteStmtNode : StmtNode
    {
        public ExprNode expr;

        public override void Gen()
        {
            expr?.Gen();
            Console.WriteLine($"put");
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
        public AstNode value;

        public override void Gen()
        {
            value.Gen();
            if (value is FactorNode)
            {
                Console.WriteLine("not");
            }
        }
    }

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
