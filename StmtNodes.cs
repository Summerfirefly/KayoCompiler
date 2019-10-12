namespace KayoCompiler.Ast
{
    class IfStmtNode : AstNode
    {
        public ExprNode condition;
        public StmtNode body;
        public ElseStmtNode elseStmt;

        public override string Gen()
        {
            int label = CodeGenData.LabelNum++;
            int endIf = CodeGenData.LabelNum++;
            string code = string.Empty;

            code += condition?.Gen() ?? string.Empty;
            code += $"jz _L{label}\n";

            CodeGenData.StackDepth--;

            code += body?.Gen() ?? string.Empty;
            code += $"jmp _L{endIf}\n";
            code += $"_L{label}:\n";

            code += elseStmt?.Gen() ?? string.Empty;

            code += $"_L{endIf}:\n";

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
            int index = SymbolTable.GetVarIndex(id.name, CodeGenData.CurrentField);
            int offset = (index + 1) * 8;
            code += expr?.Gen() ?? string.Empty;

            switch (CodeGenData.StackDepth)
            {
                case 1:
                    code += $"mov\tqword [rbp-{offset}], rax\n";
                    break;
                case 2:
                    code += $"mov\tqword [rbp-{offset}], rdx\n";
                    break;
                case 3:
                    code += $"mov\tqword [rbp-{offset}], rcx\n";
                    break;
                default:
                    code += $"push\tqword [rbp-{offset}]\n";
                    break;
            }

            CodeGenData.StackDepth--;

            return code;
        }
    }

    class WhileStmtNode : AstNode
    {
        public ExprNode condition;
        public StmtNode body;

        public override string Gen()
        {
            int startLabel = CodeGenData.LabelNum++;
            int endLabel = CodeGenData.LabelNum++;
            string code = string.Empty;

            code += $"_L{startLabel}:\n";
            code += condition?.Gen() ?? string.Empty;

            CodeGenData.StackDepth--;

            code += $"jz _L{endLabel}\n";
            code += body?.Gen() ?? string.Empty;
            code += $"jmp _L{startLabel}\n";
            code += $"_L{endLabel}:\n";

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

            switch (CodeGenData.StackDepth)
            {
                case 1:
                    code += "push\trax\n";
                    break;
                case 2:
                    code += "push\trdx\n";
                    break;
                case 3:
                    code += "push\trcx\n";
                    break;
            }

            CodeGenData.StackDepth--;
            code += $";{expr?.Type()}\n";
            if (expr?.Type() == VarType.TYPE_BOOL)
                code += "push\t0\n";
            else
                code += "push\t1\n";

            code += "call\tputs\n";

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
