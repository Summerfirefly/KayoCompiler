using System;

namespace KayoCompiler.Ast
{
    class IfStmtNode : AstNode
    {
        public ExprNode condition;
        public StmtNode body;
        public ElseStmtNode elseStmt;

        public override string Gen()
        {
            int label = CodeGenData.labelNum++;
            int endIf = CodeGenData.labelNum++;
            string code = string.Empty;

            code += condition?.Gen() ?? string.Empty;
            code += $"jz {label}f\n";
            code += body?.Gen() ?? string.Empty;
            code += $"jmp {endIf}f\n";
            code += $"{label}:\n";

            code += elseStmt?.Gen() ?? string.Empty;

            code += $"{endIf}:\n";

            return code;
        }
    }

    class ElseStmtNode : AstNode
    {
        public StmtNode body;

        public override string Gen()
        {
            return body?.Gen() ?? string.Empty;
        }
    }

    class SetStmtNode : AstNode
    {
        public IdNode id;
        public ExprNode expr;

        public override string Gen()
        {
            string code = string.Empty;
            code += expr?.Gen() ?? string.Empty;
            code += $"pop [{id.name}]\n";

            return code;
        }
    }

    class WhileStmtNode : AstNode
    {
        public ExprNode condition;
        public StmtNode body;

        public override string Gen()
        {
            int label = CodeGenData.labelNum++;
            string code = string.Empty;

            code += $"{label}:\n";
            code += condition?.Gen() ?? string.Empty;
            code += $"jz {label}f\n";
            code += body?.Gen() ?? string.Empty;
            code += $"jmp {label}b\n";
            code += $"{label}:\n";

            return code;
        }
    }

    class WriteStmtNode : AstNode
    {
        public ExprNode expr;

        public override string Gen()
        {
            string code = string.Empty;
            code += expr?.Gen() ?? string.Empty;
            code += "put\n";

            return code;
        }
    }

    class ReadStmtNode : AstNode
    {
        public IdNode id;

        public override string Gen()
        {
            return $"get [{id.name}]\n";
        }
    }
}
