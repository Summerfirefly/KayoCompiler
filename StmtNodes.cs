using System.Collections.Generic;

namespace KayoCompiler.Ast
{
    class IfStmtNode : AstNode
    {
        public ExprNode condition;
        public StmtNode body;
        public ElseStmtNode elseStmt;

        public override string Gen()
        {
            int falseLabel = CodeGenUtils.LabelNum++;
            int endIf = CodeGenUtils.LabelNum++;
            string code = string.Empty;
            ScopeManager.ScopeEnter();

            code += condition?.Gen() ?? string.Empty;
            code += $"cmp\t{CodeGenUtils.CurrentStackTop}, 0\n";
            CodeGenUtils.StackDepth--;

            code += $"je _L{falseLabel}\n";
            code += body?.Gen() ?? string.Empty;
            code += $"jmp _L{endIf}\n";
            code += $"_L{falseLabel}:\n";
            
            ScopeManager.ScopeLeave();

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
            ScopeManager.ScopeEnter();
            string code = body?.Gen() ?? string.Empty;
            ScopeManager.ScopeLeave();

            return code;
        }
    }

    class SetStmtNode : AstNode
    {
        public IdNode id;
        public ExprNode expr;

        public override string Gen()
        {
            string code = string.Empty;
            int index = SymbolTable.GetVarIndex(id.name);
            int offset = -(index + 1) * 8;
            code += expr?.Gen() ?? string.Empty;

            switch (CodeGenUtils.StackDepth)
            {
                case 1:
                    code += $"mov\tqword [rbp{(offset>0?"+":"")}{offset}], rax\n";
                    break;
                case 2:
                    code += $"mov\tqword [rbp{(offset>0?"+":"")}{offset}], rdx\n";
                    break;
                case 3:
                    code += $"mov\tqword [rbp{(offset>0?"+":"")}{offset}], rcx\n";
                    break;
                default:
                    code += $"push\tqword [rbp{(offset>0?"+":"")}{offset}]\n";
                    break;
            }

            CodeGenUtils.StackDepth--;

            return code;
        }
    }

    class WhileStmtNode : AstNode
    {
        public ExprNode condition;
        public StmtNode body;

        public override string Gen()
        {
            int startLabel = CodeGenUtils.LabelNum++;
            int endLabel = CodeGenUtils.LabelNum++;
            string code = string.Empty;
            ScopeManager.ScopeEnter();

            code += $"_L{startLabel}:\n";
            code += condition?.Gen() ?? string.Empty;
            code += $"cmp\t{CodeGenUtils.CurrentStackTop}, 0\n";
            CodeGenUtils.StackDepth--;

            code += $"je _L{endLabel}\n";
            code += body?.Gen() ?? string.Empty;
            code += $"jmp _L{startLabel}\n";
            code += $"_L{endLabel}:\n";

            ScopeManager.ScopeLeave();
            return code;
        }
    }

    class ForStmtNode : AstNode
    {
        internal StmtNode preStmt;
        internal ExprNode condition;
        internal StmtNode loopStmt;
        internal StmtNode body;

        public override string Gen()
        {
            int startLabel = CodeGenUtils.LabelNum++;
            int endLabel = CodeGenUtils.LabelNum++;
            string code = string.Empty;
            ScopeManager.ScopeEnter();

            code += preStmt?.Gen() ?? string.Empty;
            code += $"_L{startLabel}:\n";
            code += condition?.Gen() ?? string.Empty;
            code += $"cmp\t{CodeGenUtils.CurrentStackTop}, 0\n";
            CodeGenUtils.StackDepth--;

            code += $"je _L{endLabel}\n";
            code += body?.Gen() ?? string.Empty;
            code += loopStmt?.Gen() ?? string.Empty;
            code += $"jmp _L{startLabel}\n";
            code += $"_L{endLabel}:\n";

            ScopeManager.ScopeLeave();
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

            switch (CodeGenUtils.StackDepth)
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

            CodeGenUtils.StackDepth--;
            
            if (expr?.Type() == VarType.TYPE_BOOL)
                code += "push\t0\n";
            else
                code += "push\t1\n";

            code += "call\twrite\n";

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

    class FuncCallStmtNode : AstNode
    {
        internal string name;
        internal List<ExprNode> args = new List<ExprNode>();

        public override string Gen()
        {
            string code = string.Empty;
            int argc = 0;
            args.Reverse();

            // Save value in register
            switch (CodeGenUtils.StackDepth)
            {
                case 1:
                    code += "push\trax\n";
                    break;
                case 2:
                    code += "push\trax\n";
                    code += "push\trdx\n";
                    break;
                case 3:
                    code += "push\trax\n";
                    code += "push\trdx\n";
                    code += "push\trcx\n";
                    break;
            }

            foreach (ExprNode arg in args)
            {
                code += arg?.Gen();
                switch (CodeGenUtils.StackDepth)
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
                CodeGenUtils.StackDepth--;
                argc++;
            }

            code += $"call\tfunc_{name}\n";
            if (argc > 0)
            {
                code += $"add\trsp, {argc * 8}\n";
            }

            CodeGenUtils.StackDepth++;
            switch (CodeGenUtils.StackDepth)
            {
                case 1:
                    code += "mov\trax, rbx\n";
                    break;
                case 2:
                    code += "pop\trax\n";
                    code += "mov\trdx, rbx\n";
                    break;
                case 3:
                    code += "pop\trax\n";
                    code += "pop\trdx\n";
                    code += "mov\trcx, rbx\n";
                    break;
                default:
                    code += "pop\trax\n";
                    code += "pop\trdx\n";
                    code += "pop\trcx\n";
                    code += "push\trbx\n";
                    break;
            }

            return code;
        }

        internal VarType Type()
        {
            return SymbolTable.FindFun(name).returnType;
        }
    }

    class ReturnStmtNode : AstNode
    {
        public ExprNode expr;

        public override string Gen()
        {
            string code = string.Empty;

            code += expr?.Gen();
			if (expr != null)
			{
				code += "mov\trbx, rax\n";
                CodeGenUtils.StackDepth--;
			}
            code += $"jmp\tfunc_{ScopeManager.CurrentFun}_return\n";

            return code;
        }
    }
}
