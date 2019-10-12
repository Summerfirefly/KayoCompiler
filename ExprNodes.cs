namespace KayoCompiler.Ast
{
    abstract class ExprBaseNode : AstNode
    {
        public abstract VarType Type();
    }

    class ExprNode : ExprBaseNode
    {
        public LogicExprNode expr;

        public override string Gen()
        {
            return expr?.Gen() ?? string.Empty;
        }

        public override VarType Type()
        {
            return expr?.Type() ?? VarType.TYPE_ERROR;
        }
    }

    class LogicExprNode : ExprBaseNode
    {
        public LogicTermNode term;
        public LogicExprTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += term?.Gen() ?? string.Empty;
            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return term.Type();
            }

            if (tail.Type() != VarType.TYPE_BOOL || term.Type() != VarType.TYPE_BOOL)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_BOOL;
        }
    }

    class LogicExprTailNode : ExprBaseNode
    {
        public LogicTermNode term;
        public LogicExprTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += term?.Gen() ?? string.Empty;
            switch (CodeGenUtils.StackDepth)
            {
                case 2:
                    code += "or\trax, rdx\n";
                    break;
                case 3:
                    code += "or\trdx, rcx\n";
                    break;
                case 4:
                    code += "pop\trbx";
                    code += "or\trcx, rbx\n";
                    break;
                default:
                    code += "pop\trbx\n";
                    code += "or\tqword [rsp], rbx\n";
                    break;
            }
            CodeGenUtils.StackDepth--;
            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return term.Type();
            }

            if (tail.Type() != VarType.TYPE_BOOL || term.Type() != VarType.TYPE_BOOL)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_BOOL;
        }
    }

    class LogicTermNode : ExprBaseNode
    {
        public LogicFactorNode factor;
        public LogicTermTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += factor?.Gen() ?? string.Empty;
            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return factor.Type();
            }

            if (tail.Type() != VarType.TYPE_BOOL || factor.Type() != VarType.TYPE_BOOL)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_BOOL;
        }
    }

    class LogicTermTailNode : ExprBaseNode
    {
        public LogicFactorNode factor;
        public LogicTermTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += factor?.Gen() ?? string.Empty;
            switch (CodeGenUtils.StackDepth)
            {
                case 2:
                    code += "and\trax, rdx\n";
                    break;
                case 3:
                    code += "and\trdx, rcx\n";
                    break;
                case 4:
                    code += "pop\trbx";
                    code += "and\trcx, rbx\n";
                    break;
                default:
                    code += "pop\trbx\n";
                    code += "and\tqword [rsp], rbx\n";
                    break;
            }
            CodeGenUtils.StackDepth--;
            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return factor.Type();
            }

            if (tail.Type() != VarType.TYPE_BOOL || factor.Type() != VarType.TYPE_BOOL)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_BOOL;
        }
    }

    class LogicFactorNode : ExprBaseNode
    {
        public LogicRelNode rel;
        public LogicFactorTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += rel?.Gen() ?? string.Empty;
            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return rel.Type();
            }

            if (tail.Type() != rel.Type() ||
                tail.Type() == VarType.TYPE_ERROR ||
                rel.Type() == VarType.TYPE_ERROR)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_BOOL;
        }
    }

    class LogicFactorTailNode : ExprBaseNode
    {
        public Tag op;
        public LogicRelNode rel;
        public LogicFactorTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += rel?.Gen() ?? string.Empty;

            switch (CodeGenUtils.StackDepth)
            {
                case 2:
                    code += "cmp\trax, rdx\n";
                    break;
                case 3:
                    code += "cmp\trdx, rcx\n";
                    break;
                case 4:
                    code += "pop\trbx";
                    code += "cmp\trcx, rbx\n";
                    break;
                default:
                    code += "pop\trbx\n";
                    code += "cmp\tqword [rsp], rbx\n";
                    break;
            }

            switch (op)
            {
                case Tag.DL_EQ:
                    code += "sete\tbl\n";
                    break;
                case Tag.DL_NEQ:
                    code += "setne\tbl\n";
                    break;
            }

            code += "movzx\trbx, bl\n";

            switch (CodeGenUtils.StackDepth)
            {
                case 2:
                    code += "mov\trax, rbx\n";
                    break;
                case 3:
                    code += "mov\trdx, rbx\n";
                    break;
                case 4:
                    code += "mov\trcx, rbx\n";
                    break;
                default:
                    code += "push\trbx\n";
                    break;
            }

            CodeGenUtils.StackDepth--;

            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return rel.Type();
            }

            if (tail.Type() != rel.Type() ||
                tail.Type() == VarType.TYPE_ERROR ||
                rel.Type() == VarType.TYPE_ERROR)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_BOOL;
        }
    }

    class LogicRelNode : ExprBaseNode
    {
        public MathExprNode expr;
        public LogicRelTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += expr?.Gen() ?? string.Empty;
            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return expr.Type();
            }

            if (tail.Type() != expr.Type() ||
                tail.Type() == VarType.TYPE_ERROR ||
                expr.Type() == VarType.TYPE_ERROR)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_BOOL;
        }
    }

    class LogicRelTailNode : ExprBaseNode
    {
        public Tag op;
        public MathExprNode expr;
        public LogicRelTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += expr?.Gen() ?? string.Empty;

            switch (CodeGenUtils.StackDepth)
            {
                case 2:
                    code += "cmp\trax, rdx\n";
                    break;
                case 3:
                    code += "cmp\trdx, rcx\n";
                    break;
                case 4:
                    code += "pop\trbx";
                    code += "cmp\trcx, rbx\n";
                    break;
                default:
                    code += "pop\trbx\n";
                    code += "cmp\tqword [rsp], rbx\n";
                    break;
            }

            switch (op)
            {
                case Tag.DL_GT:
                    code += "setg\tbl\n";
                    break;
                case Tag.DL_NGT:
                    code += "setng\tbl\n";
                    break;
                case Tag.DL_LT:
                    code += "setl\tbl\n";
                    break;
                case Tag.DL_NLT:
                    code += "setnl\tbl\n";
                    break;
            }

            code += "movzx\trbx, bl\n";

            switch (CodeGenUtils.StackDepth)
            {
                case 2:
                    code += "mov\trax, rbx\n";
                    break;
                case 3:
                    code += "mov\trdx, rbx\n";
                    break;
                case 4:
                    code += "mov\trcx, rbx\n";
                    break;
                default:
                    code += "push\trbx\n";
                    break;
            }

            CodeGenUtils.StackDepth--;

            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return expr.Type();
            }

            if (tail.Type() != expr.Type() ||
                tail.Type() == VarType.TYPE_ERROR ||
                expr.Type() == VarType.TYPE_ERROR)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_BOOL;
        }
    }

    class MathExprNode : ExprBaseNode
    {
        public MathTermNode term;
        public MathExprTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += term?.Gen() ?? string.Empty;
            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return term.Type();
            }

            if (tail.Type() != VarType.TYPE_INT || term.Type() != VarType.TYPE_INT)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_INT;
        }
    }

    class MathExprTailNode : ExprBaseNode
    {
        public Tag op;
        public MathTermNode term;
        public MathExprTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += term?.Gen() ?? string.Empty;

            if (op == Tag.DL_PLUS)
            {
                switch (CodeGenUtils.StackDepth)
                {
                    case 2:
                        code += "add\trax, rdx\n";
                        break;
                    case 3:
                        code += "add\trdx, rcx\n";
                        break;
                    case 4:
                        code += "pop\trbx\n";
                        code += "add\trcx, rbx\n";
                        break;
                    default:
                        code += "pop\trbx\n";
                        code += "add\tqword [rsp], rbx\n";
                        break;
                }
            }
            else
            {
                switch (CodeGenUtils.StackDepth)
                {
                    case 2:
                        code += "sub\trax, rdx\n";
                        break;
                    case 3:
                        code += "sub\trdx, rcx\n";
                        break;
                    case 4:
                        code += "pop\trbx\n";
                        code += "sub\trcx, rbx\n";
                        break;
                    default:
                        code += "pop\trbx\n";
                        code += "sub\tqword [rsp], rbx\n";
                        break;
                }
            }

            CodeGenUtils.StackDepth--;

            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return term.Type();
            }

            if (tail.Type() != VarType.TYPE_INT || term.Type() != VarType.TYPE_INT)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_INT;
        }
    }

    class MathTermNode : ExprBaseNode
    {
        public MathFactorNode factor;
        public MathTermTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += factor?.Gen() ?? string.Empty;
            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return factor.Type();
            }

            if (tail.Type() != VarType.TYPE_INT || factor.Type() != VarType.TYPE_INT)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_INT;
        }
    }

    class MathTermTailNode : ExprBaseNode
    {
        public Tag op;
        public MathFactorNode factor;
        public MathTermTailNode tail;

        public override string Gen()
        {
            string code = string.Empty;
            code += factor?.Gen() ?? string.Empty;

            if (op == Tag.DL_MULTI)
            {
                switch (CodeGenUtils.StackDepth)
                {
                    case 2:
                        code += "imul\trax, rdx\n";
                        break;
                    case 3:
                        code += "imul\trdx, rcx\n";
                        break;
                    case 4:
                        code += "pop\trbx\n";
                        code += "imul\trcx, rbx\n";
                        break;
                    default:
                        code += "pop\trbx\n";
                        code += "imul\trbx, qword [rsp]\n";
                        code += "mov\trbx, qword [rsp]\n";
                        break;
                }
            }
            else
            {
                switch (CodeGenUtils.StackDepth)
                {
                    case 2:
                        code += "mov\trbx, rdx\n";
                        code += "cqo\n";
                        code += "idiv\trbx\n";
                        break;
                    case 3:
                        code += "push\trax\n";
                        code += "mov\trax, rdx\n";
                        code += "cqo\n";
                        code += "idiv\trcx\n";
                        code += "mov\trdx, rax\n";
                        code += "pop\trax\n";
                        break;
                    case 4:
                        code += "pop\trbx\n";
                        code += "push\trax\n";
                        code += "push\trdx\n";
                        code += "mov\trax, rcx\n";
                        code += "cqo\n";
                        code += "idiv\trbx";
                        code += "mov\trcx, rax\n";
                        code += "pop\trdx\n";
                        code += "pop\trax\n";
                        break;
                    default:
                        code += "pop\trbx\n";
                        code += "push\trax\n";
                        code += "push\trdx\n";
                        code += "mov\trax, qword 16[rsp]\n";
                        code += "cqo\n";
                        code += "idiv\trbx\n";
                        code += "mov\trbx, rax\n";
                        code += "pop\trdx\n";
                        code += "pop\trax\n";
                        code += "mov\tqword [rsp], rbx\n";
                        break;
                }
            }

            CodeGenUtils.StackDepth--;

            code += tail?.Gen() ?? string.Empty;

            return code;
        }

        public override VarType Type()
        {
            if (tail == null)
            {
                return factor.Type();
            }

            if (tail.Type() != VarType.TYPE_INT || factor.Type() != VarType.TYPE_INT)
            {
                return VarType.TYPE_ERROR;
            }

            return VarType.TYPE_INT;
        }
    }

    class MathFactorNode : ExprBaseNode
    {
        public TerminalNode value;
        public ExprNode expr;
        public MathFactorNode factor;

        public override string Gen()
        {
            string code = string.Empty;
            code += value?.Gen() ?? string.Empty;
            code += expr?.Gen() ?? string.Empty;
            code += factor?.Gen() ?? string.Empty;

            if (factor != null)
            {
                switch (CodeGenUtils.StackDepth)
                {
                    case 1:
                        code += "not\trax\n";
                        break;
                    case 2:
                        code += "not\trdx\n";
                        break;
                    case 3:
                        code += "not\trcx\n";
                        break;
                    default:
                        code += "not\tqword [rsp]\n";
                        break;
                }
            }

            return code;
        }

        public override VarType Type()
        {
            return value?.Type() ?? expr?.Type() ?? factor?.Type() ?? VarType.TYPE_ERROR;
        }
    }
}
