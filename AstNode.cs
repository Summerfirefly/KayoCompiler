using System;
using System.Collections.Generic;

namespace KayoCompiler.Ast
{
    abstract class AstNode
    {
        public abstract void Gen();
    }

    internal static class CodeGenData
    {
        internal static int labelNum = 1;
    }

    class EmptyNode : AstNode
    {
        public override void Gen()
        {
        }
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
            Console.WriteLine($".{type} {name}");
        }
    }

    class StmtsNode : AstNode
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

    class StmtNode : AstNode
    {
        public AstNode stmt;
        public override void Gen()
        {
            stmt.Gen();
        }
    }

    class IfStmtNode : AstNode
    {
        public ExprNode condition;
        public StmtNode body;
        public ElseStmtNode elseStmt;

        public override void Gen()
        {
            int label = CodeGenData.labelNum++;
            int endIf = CodeGenData.labelNum++;

            condition.Gen();
            Console.WriteLine($"jz {label}f");
            body.Gen();
            Console.WriteLine($"jmp {endIf}f");
            Console.WriteLine($"{label}:");

            if (elseStmt != null)
                elseStmt.Gen();

            Console.WriteLine($"{endIf}:");
        }
    }

    class ElseStmtNode : AstNode
    {
        public StmtNode body;

        public override void Gen()
        {
            body.Gen();
        }
    }

    class SetStmtNode : AstNode
    {
        public string id;
        public ExprNode expr;

        public override void Gen()
        {
            expr.Gen();
            Console.WriteLine($"pop [{id}]");
        }
    }

    class WhileStmtNode : AstNode
    {
        public ExprNode condition;
        public StmtNode body;

        public override void Gen()
        {
            int label = CodeGenData.labelNum++;

            Console.WriteLine($"{label}:");
            condition.Gen();
            Console.WriteLine($"jz {label}f");
            body.Gen();
            Console.WriteLine($"jmp {label}b");
            Console.WriteLine($"{label}:");
        }
    }

    class WriteStmtNode : AstNode
    {
        public ExprNode expr;

        public override void Gen()
        {
            expr?.Gen();
            Console.WriteLine("put");
        }
    }

    class ReadStmtNode : AstNode
    {
        public string id;

        public override void Gen()
        {
            Console.WriteLine($"get [{id}]");
        }
    }

    class ExprNode : AstNode
    {
        public TermNode term;
        public ExprTail expr;

        public override void Gen()
        {
            term?.Gen();
            expr?.Gen();
        }
    }
    
    class ExprTail : AstNode
    {
        public string op;
        public TermNode term;
        public ExprTail expr;

        public override void Gen()
        {
            if (op != null)
            {
                term?.Gen();

                if (op == "+")
                    Console.WriteLine("add");
                else if (op == "-")
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
            term?.Gen();
        }
    }

    class TermTail : AstNode
    {
        public string op;
        public FactorNode factor;
        public TermTail term;

        public override void Gen()
        {
            if (op != null)
            {
                factor?.Gen();

                if (op == "*")
                    Console.WriteLine("mul");
                else if (op == "/")
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
            value?.Gen();
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
