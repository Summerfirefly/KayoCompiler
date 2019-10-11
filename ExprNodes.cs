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
            switch (CodeGenData.stackDepth)
            {
                case 2:
                    code += "orq\t%rax, %rdx\n";
                    break;
                case 3:
                    code += "orq\t%rdx, %rcx\n";
                    break;
                case 4:
                    code += "popq\t%rbx";
                    code += "orq\t%rcx, %rbx\n";
                    break;
                default:
                    code += "popq\t%rbx\n";
                    code += "orq\t(%rsp), %rbx\n";
                    break;
            }
            CodeGenData.stackDepth--;
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
            switch (CodeGenData.stackDepth)
            {
                case 2:
                    code += "andq\t%rax, %rdx\n";
                    break;
                case 3:
                    code += "andq\t%rdx, %rcx\n";
                    break;
                case 4:
                    code += "popq\t%rbx";
                    code += "andq\t%rcx,%rbx\n";
                    break;
                default:
                    code += "popq\t%rbx\n";
                    code += "andq\t(%rsp), %rbx\n";
                    break;
            }
            CodeGenData.stackDepth--;
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

            switch (CodeGenData.stackDepth)
            {
                case 2:
                    code += "cmpq\t%rax, %rdx\n";
                    break;
                case 3:
                    code += "cmpq\t%rdx, %rcx\n";
                    break;
                case 4:
                    code += "popq\t%rbx";
                    code += "cmpq\t%rcx, %rbx\n";
                    break;
                default:
                    code += "popq\t%rbx\n";
                    code += "cmpq\t(%rsp), %rbx\n";
                    break;
            }
            CodeGenData.stackDepth -= 2;

            int trueLabel = CodeGenData.labelNum++;
            int endLabel = CodeGenData.labelNum++;
            switch (op)
            {
                case Tag.DL_EQ:
                    code += $"je\t{trueLabel}\n";
                    break;
                case Tag.DL_NEQ:
                    code += $"jne\t{trueLabel}\n";
                    break;
            }

            switch (CodeGenData.stackDepth)
            {
                case 0:
                    code += "movq\t$0, %rax\n";
                    break;
                case 1:
                    code += "movq\t$0, %rdx\n";
                    break;
                case 2:
                    code += "movq\t$0, %rcx\n";
                    break;
                default:
                    code += "pushq\t$0\n";
                    break;
            }
            code += $"jmp\t{endLabel}\n";
            code += $"{trueLabel}:\n";
            switch (CodeGenData.stackDepth)
            {
                case 0:
                    code += "movq\t$1, %rax\n";
                    break;
                case 1:
                    code += "movq\t$1, %rdx\n";
                    break;
                case 2:
                    code += "movq\t$1, %rcx\n";
                    break;
                default:
                    code += "pushq\t$1\n";
                    break;
            }
            code += $"{endLabel}:\n";
            CodeGenData.stackDepth++;

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

            switch (CodeGenData.stackDepth)
            {
                case 2:
                    code += "cmpq\t%rax, %rdx\n";
                    break;
                case 3:
                    code += "cmpq\t%rdx, %rcx\n";
                    break;
                case 4:
                    code += "popq\t%rbx";
                    code += "cmpq\t%rcx, %rbx\n";
                    break;
                default:
                    code += "popq\t%rbx\n";
                    code += "cmpq\t(%rsp), %rbx\n";
                    break;
            }
            CodeGenData.stackDepth -= 2;

            int trueLabel = CodeGenData.labelNum++;
            int endLabel = CodeGenData.labelNum++;
            switch (op)
            {
                case Tag.DL_GT:
                    code += $"jg\t{trueLabel}\n";
                    break;
                case Tag.DL_NGT:
                    code += $"jle\t{trueLabel}\n";
                    break;
                case Tag.DL_LT:
                    code += $"jl\t{trueLabel}\n";
                    break;
                case Tag.DL_NLT:
                    code += $"jge\t{trueLabel}\n";
                    break;
            }

            switch (CodeGenData.stackDepth)
            {
                case 0:
                    code += "movq\t$0, %rax\n";
                    break;
                case 1:
                    code += "movq\t$0, %rdx\n";
                    break;
                case 2:
                    code += "movq\t$0, %rcx\n";
                    break;
                default:
                    code += "pushq\t$0\n";
                    break;
            }
            code += $"jmp\t{endLabel}\n";
            code += $"{trueLabel}:\n";
            switch (CodeGenData.stackDepth)
            {
                case 0:
                    code += "movq\t$1, %rax\n";
                    break;
                case 1:
                    code += "movq\t$1, %rdx\n";
                    break;
                case 2:
                    code += "movq\t$1, %rcx\n";
                    break;
                default:
                    code += "pushq\t$1\n";
                    break;
            }
            code += $"{endLabel}:\n";
            CodeGenData.stackDepth++;

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
                switch (CodeGenData.stackDepth)
                {
                    case 2:
                        code += "addq\t%rax, %rdx\n";
                        break;
                    case 3:
                        code += "addq\t%rdx, %rcx\n";
                        break;
                    case 4:
                        code += "popq\t%rbx\n";
                        code += "addq\t%rcx, %rbx\n";
                        break;
                    default:
                        code += "popq\t%rbx\n";
                        code += "addq\t(%rsp), %rbx\n";
                        break;
                }
            }
            else
            {
                switch (CodeGenData.stackDepth)
                {
                    case 2:
                        code += "subq\t%rax, %rdx\n";
                        break;
                    case 3:
                        code += "subq\t%rdx, %rcx\n";
                        break;
                    case 4:
                        code += "popq\t%rbx\n";
                        code += "subq\t%rcx, %rbx\n";
                        break;
                    default:
                        code += "popq\t%rbx\n";
                        code += "subq\t(%rsp), %rbx\n";
                        break;
                }
            }

            CodeGenData.stackDepth--;

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
                switch (CodeGenData.stackDepth)
                {
                    case 2:
                        code += "imulq\t%rax, %rdx\n";
                        break;
                    case 3:
                        code += "imulq\t%rdx, %rcx\n";
                        break;
                    case 4:
                        code += "popq\t%rbx\n";
                        code += "imulq\t%rcx, %rbx\n";
                        break;
                    default:
                        code += "popq\t%rbx\n";
                        code += "imulq\t%rbx, (%rsp)\n";
                        code += "movq\t%rbx, (%rsp)\n";
                        break;
                }
            }
            else
            {
                switch (CodeGenData.stackDepth)
                {
                    case 2:
                        code += "movq\t%rdx, %rbx\n";
                        code += "cqto\n";
                        code += "idivq\t%rbx\n";
                        break;
                    case 3:
                        code += "pushq\t%rax\n";
                        code += "movq\t%rdx, %rax\n";
                        code += "cqto\n";
                        code += "idivq\t%rcx\n";
                        code += "movq\t%rax, %rdx\n";
                        code += "popq\t%rax\n";
                        break;
                    case 4:
                        code += "popq\t%rbx\n";
                        code += "pushq\t%rax\n";
                        code += "pushq\t%rdx\n";
                        code += "movq\t%rcx, %rax\n";
                        code += "cqto\n";
                        code += "idivq\t%rbx";
                        code += "movq\t%rax, %rcx\n";
                        code += "popq\t%rdx\n";
                        code += "popq\t%rax\n";
                        break;
                    default:
                        code += "popq\t%rbx\n";
                        code += "pushq\t%rax\n";
                        code += "pushq\t%rdx\n";
                        code += "movq\t16(%rsp), %rax\n";
                        code += "cqto\n";
                        code += "idivq\t%rbx\n";
                        code += "movq\t%rax, %rbx\n";
                        code += "popq\t%rdx\n";
                        code += "popq\t%rax\n";
                        code += "movq\t%rbx, (%rsp)\n";
                        break;
                }
            }

            CodeGenData.stackDepth--;

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
                switch (CodeGenData.stackDepth)
                {
                    case 1:
                        code += "not\t%rax\n";
                        break;
                    case 2:
                        code += "not\t%rdx\n";
                        break;
                    case 3:
                        code += "not\t%rcx\n";
                        break;
                    default:
                        code += "not\t(%rsp)\n";
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
