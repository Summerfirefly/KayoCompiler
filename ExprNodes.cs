using System;

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
            code += "or\n";
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
            code += "and\n";
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
            code += $"{op}\n";
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
            code += $"{op}\n";
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
            code += $"{op}\n";
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
            code += $"{op}\n";
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
                code += "not\n";

            return code;
        }

        public override VarType Type()
        {
            return value?.Type() ?? expr?.Type() ?? factor?.Type() ?? VarType.TYPE_ERROR;
        }
    }
}
