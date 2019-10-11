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
            code += $"jz {label}\n";

            CodeGenData.stackDepth--;

            code += body?.Gen() ?? string.Empty;
            code += $"jmp {endIf}\n";
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
            int index = SymbolTable.GetVarIndex(id.name, CodeGenData.currentField);
            int offset = (index + 1) * 8;
            code += expr?.Gen() ?? string.Empty;

            switch (CodeGenData.stackDepth)
            {
                case 1:
                    code += $"movq\t%rax, -{offset}(%rbp)\n";
                    break;
                case 2:
                    code += $"movq\t%rdx, -{offset}(%rbp)\n";
                    break;
                case 3:
                    code += $"movq\t%rcx, -{offset}(%rbp)\n";
                    break;
                default:
                    code += $"popq\t-{offset}(%rbp)\n";
                    break;
            }

            CodeGenData.stackDepth--;

            return code;
        }
    }

    class WhileStmtNode : AstNode
    {
        public ExprNode condition;
        public StmtNode body;

        public override string Gen()
        {
            int startLabel = CodeGenData.labelNum++;
            int endLabel = CodeGenData.labelNum++;
            string code = string.Empty;

            code += $"{startLabel}:\n";
            code += condition?.Gen() ?? string.Empty;

            CodeGenData.stackDepth--;

            code += $"jz {endLabel}\n";
            code += body?.Gen() ?? string.Empty;
            code += $"jmp {startLabel}\n";
            code += $"{endLabel}:\n";

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
