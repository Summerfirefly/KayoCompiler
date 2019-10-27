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

            code += $"cmp\trax, 0\n";
            CodeGenUtils.StackDepth = 0;

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
            int offset = -SymbolTable.GetVarOffset(id.name);
            code += expr?.Gen() ?? string.Empty;

            switch (SymbolTable.SizeOf(SymbolTable.FindVar(id.name).type))
            {
                case 1:
                    code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], al\n";
                    break;
                case 4:
                    code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], eax\n";
                    break;
                case 8:
                    code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], rax\n";
                    break;
            }
            CodeGenUtils.StackDepth = 0;

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

            code += $"cmp\trax, 0\n";
            CodeGenUtils.StackDepth = 0;

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

            code += $"cmp\trax, 0\n";
            CodeGenUtils.StackDepth = 0;

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
            code += $"push\trax\n";
            CodeGenUtils.StackDepth = 0;
            
            if (expr?.Type() == VarType.TYPE_BOOL)
                code += "push\t0\n";
            else if (SymbolTable.IsNumType(expr.Type()))
                code += "push\t1\n";

            code += "call\twrite\n";
            code += "add\trsp, 16\n";

            return code;
        }
    }

    class ReadStmtNode : AstNode
    {
        public IdNode id;

        public override string Gen()
        {
            string code = string.Empty;

            if (id.Type() == VarType.TYPE_BOOL)
            {
                code += "push\t0\n";
            }
            else if (SymbolTable.IsNumType(id.Type()))
            {
                code += "push\t1\n";
            }

            code += "call\tread\n";
            code += "add\trsp, 8\n";

            int offset = -SymbolTable.GetVarOffset(id.name);

            switch (SymbolTable.SizeOf(SymbolTable.FindVar(id.name).type))
            {
                case 1:
                    code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], al\n";
                    break;
                case 4:
                    code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], eax\n";
                    break;
                case 8:
                    code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], rax\n";
                    break;
            }

            return code;
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
                case 0:
                    break;
                case 1:
                    code += "push\trax\n";
                    break;
                case 2:
                    code += "push\trax\n";
                    code += "push\tr10\n";
                    break;
                default:
                    code += "push\trax\n";
                    code += "push\tr10\n";
                    code += "push\tr11\n";
                    break;
            }

            foreach (ExprNode arg in args)
            {
                code += arg?.Gen();
                code += $"push\t{CodeGenUtils.CurrentStackTop64}\n";
                if (CodeGenUtils.StackDepth > 3)
                {
                    code += $"pop\t{CodeGenUtils.CurrentStackTop64}\n";
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
                    break;
                case 2:
                    code += "mov\tr10, rax\n";
                    code += "pop\trax\n";
                    break;
                case 3:
                    code += "mov\tr11, rax\n";
                    code += "pop\tr10\n";
                    code += "pop\trax\n";
                    break;
                default:
                    code += "mov\trbx, rax\n";
                    code += "pop\tr11\n";
                    code += "pop\tr10\n";
                    code += "pop\trax\n";
                    code += $"push\t{CodeGenUtils.CurrentStackTop64}\n";
                    code += $"mov\t{CodeGenUtils.CurrentStackTop64}, rbx\n";
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

            code += expr.Gen();
            CodeGenUtils.StackDepth = 0;
            code += $"jmp\tfunc_{ScopeManager.CurrentFun}_return\n";

            return code;
        }
    }
}
