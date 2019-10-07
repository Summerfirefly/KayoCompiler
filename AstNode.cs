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

    class ProgramNode : AstNode
    {
        List<BlockNode> children;

        public override void Gen()
        {
            if (children == null) return;

            foreach (var child in children)
            {
                child?.Gen();
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
                child?.Gen();
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
                decl?.Gen();
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
        public Tag type;
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
                child?.Gen();
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
            stmt?.Gen();
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

            condition?.Gen();
            Console.WriteLine($"jz {label}f");
            body?.Gen();
            Console.WriteLine($"jmp {endIf}f");
            Console.WriteLine($"{label}:");

            elseStmt?.Gen();

            Console.WriteLine($"{endIf}:");
        }
    }

    class ElseStmtNode : AstNode
    {
        public StmtNode body;

        public override void Gen()
        {
            body?.Gen();
        }
    }

    class SetStmtNode : AstNode
    {
        public string id;
        public ExprNode expr;

        public override void Gen()
        {
            expr?.Gen();
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
            condition?.Gen();
            Console.WriteLine($"jz {label}f");
            body?.Gen();
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
        public LogicExprNode expr;

        public override void Gen()
        {
            expr?.Gen();
        }
    }

    class LogicExprNode : AstNode
    {
        public LogicTermNode term;
        public LogicExprTailNode tail;

        public override void Gen()
        {
            term?.Gen();
            tail?.Gen();
        }
    }

    class LogicExprTailNode : AstNode
    {
        public LogicTermNode term;
        public LogicExprTailNode tail;

        public override void Gen()
        {
            term?.Gen();
            Console.WriteLine("or");
            tail?.Gen();
        }
    }

    class LogicTermNode : AstNode
    {
        public LogicFactorNode factor;
        public LogicTermTailNode tail;

        public override void Gen()
        {
            factor?.Gen();
            tail?.Gen();
        }
    }

    class LogicTermTailNode : AstNode
    {
        public LogicFactorNode factor;
        public LogicTermTailNode tail;

        public override void Gen()
        {
            factor?.Gen();
            Console.WriteLine("and");
            tail?.Gen();
        }
    }

    class LogicFactorNode : AstNode
    {
        public LogicRelNode rel;
        public LogicFactorTailNode tail;

        public override void Gen()
        {
            rel?.Gen();
            tail?.Gen();
        }
    }

    class LogicFactorTailNode : AstNode
    {
        public Tag op;
        public LogicRelNode rel;
        public LogicFactorTailNode tail;

        public override void Gen()
        {
            rel?.Gen();
            Console.WriteLine(op);
            tail?.Gen();
        }
    }

    class LogicRelNode : AstNode
    {
        public MathExprNode expr;
        public LogicRelTailNode tail;

        public override void Gen()
        {
            expr?.Gen();
            tail?.Gen();
        }
    }

    class LogicRelTailNode : AstNode
    {
        public Tag op;
        public MathExprNode expr;
        public LogicRelTailNode tail;

        public override void Gen()
        {
            expr?.Gen();
            Console.WriteLine(op);
            tail?.Gen();
        }
    }

    class MathExprNode : AstNode
    {
        public MathTermNode term;
        public MathExprTailNode tail;

        public override void Gen()
        {
            term?.Gen();
            tail?.Gen();
        }
    }

    class MathExprTailNode : AstNode
    {
        public Tag op;
        public MathTermNode term;
        public MathExprTailNode tail;

        public override void Gen()
        {
            term?.Gen();
            Console.WriteLine(op);
            tail?.Gen();
        }
    }

    class MathTermNode : AstNode
    {
        public MathFactorNode factor;
        public MathTermTailNode tail;

        public override void Gen()
        {
            factor?.Gen();
            tail?.Gen();
        }
    }

    class MathTermTailNode : AstNode
    {
        public Tag op;
        public MathFactorNode factor;
        public MathTermTailNode tail;

        public override void Gen()
        {
            factor?.Gen();
            Console.WriteLine(op);
            tail?.Gen();
        }
    }

    class MathFactorNode : AstNode
    {
        public TerminalNode value;
        public ExprNode expr;
        public MathFactorNode factor;

        public override void Gen()
        {
            value?.Gen();
            expr?.Gen();
            factor?.Gen();

            if (factor != null)
                Console.WriteLine("not");
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
