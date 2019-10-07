using System;

namespace KayoCompiler.Ast
{
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
}
