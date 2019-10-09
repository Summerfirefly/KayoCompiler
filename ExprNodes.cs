using System;

namespace KayoCompiler.Ast
{
    abstract class ExprBaseNode : AstNode
    {
        public abstract Type Type();
    }

    class ExprNode : ExprBaseNode
    {
        public LogicExprNode expr;

        public override void Gen()
        {
            expr?.Gen();
        }

        public override Type Type()
        {
            return expr?.Type() ?? Ast.Type.TYPE_ERROR;
        }
    }

    class LogicExprNode : ExprBaseNode
    {
        public LogicTermNode term;
        public LogicExprTailNode tail;

        public override void Gen()
        {
            term?.Gen();
            tail?.Gen();
        }

        public override Type Type()
        {
            if (tail == null)
            {
                return term.Type();
            }

            if (tail.Type() != Ast.Type.TYPE_BOOL || term.Type() != Ast.Type.TYPE_BOOL)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_BOOL;
        }
    }

    class LogicExprTailNode : ExprBaseNode
    {
        public LogicTermNode term;
        public LogicExprTailNode tail;

        public override void Gen()
        {
            term?.Gen();
            Console.WriteLine("or");
            tail?.Gen();
        }

        public override Type Type()
        {
            if (tail == null)
            {
                return term.Type();
            }

            if (tail.Type() != Ast.Type.TYPE_BOOL || term.Type() != Ast.Type.TYPE_BOOL)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_BOOL;
        }
    }

    class LogicTermNode : ExprBaseNode
    {
        public LogicFactorNode factor;
        public LogicTermTailNode tail;

        public override void Gen()
        {
            factor?.Gen();
            tail?.Gen();
        }

        public override Type Type()
        {
            if (tail == null)
            {
                return factor.Type();
            }

            if (tail.Type() != Ast.Type.TYPE_BOOL || factor.Type() != Ast.Type.TYPE_BOOL)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_BOOL;
        }
    }

    class LogicTermTailNode : ExprBaseNode
    {
        public LogicFactorNode factor;
        public LogicTermTailNode tail;

        public override void Gen()
        {
            factor?.Gen();
            Console.WriteLine("and");
            tail?.Gen();
        }

        public override Type Type()
        {
            if (tail == null)
            {
                return factor.Type();
            }

            if (tail.Type() != Ast.Type.TYPE_BOOL || factor.Type() != Ast.Type.TYPE_BOOL)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_BOOL;
        }
    }

    class LogicFactorNode : ExprBaseNode
    {
        public LogicRelNode rel;
        public LogicFactorTailNode tail;

        public override void Gen()
        {
            rel?.Gen();
            tail?.Gen();
        }

        public override Type Type()
        {
            if (tail == null)
            {
                return rel.Type();
            }

            if (tail.Type() != rel.Type() ||
                tail.Type() == Ast.Type.TYPE_ERROR ||
                rel.Type() == Ast.Type.TYPE_ERROR)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_BOOL;
        }
    }

    class LogicFactorTailNode : ExprBaseNode
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

        public override Type Type()
        {
            if (tail == null)
            {
                return rel.Type();
            }

            if (tail.Type() != rel.Type() ||
                tail.Type() == Ast.Type.TYPE_ERROR ||
                rel.Type() == Ast.Type.TYPE_ERROR)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_BOOL;
        }
    }

    class LogicRelNode : ExprBaseNode
    {
        public MathExprNode expr;
        public LogicRelTailNode tail;

        public override void Gen()
        {
            expr?.Gen();
            tail?.Gen();
        }

        public override Type Type()
        {
            if (tail == null)
            {
                return expr.Type();
            }

            if (tail.Type() != expr.Type() ||
                tail.Type() == Ast.Type.TYPE_ERROR ||
                expr.Type() == Ast.Type.TYPE_ERROR)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_BOOL;
        }
    }

    class LogicRelTailNode : ExprBaseNode
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

        public override Type Type()
        {
            if (tail == null)
            {
                return expr.Type();
            }

            if (tail.Type() != expr.Type() ||
                tail.Type() == Ast.Type.TYPE_ERROR ||
                expr.Type() == Ast.Type.TYPE_ERROR)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_BOOL;
        }
    }

    class MathExprNode : ExprBaseNode
    {
        public MathTermNode term;
        public MathExprTailNode tail;

        public override void Gen()
        {
            term?.Gen();
            tail?.Gen();
        }

        public override Type Type()
        {
            if (tail == null)
            {
                return term.Type();
            }

            if (tail.Type() != Ast.Type.TYPE_INT || term.Type() != Ast.Type.TYPE_INT)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_INT;
        }
    }

    class MathExprTailNode : ExprBaseNode
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

        public override Type Type()
        {
            if (tail == null)
            {
                return term.Type();
            }

            if (tail.Type() != Ast.Type.TYPE_INT || term.Type() != Ast.Type.TYPE_INT)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_INT;
        }
    }

    class MathTermNode : ExprBaseNode
    {
        public MathFactorNode factor;
        public MathTermTailNode tail;

        public override void Gen()
        {
            factor?.Gen();
            tail?.Gen();
        }

        public override Type Type()
        {
            if (tail == null)
            {
                return factor.Type();
            }

            if (tail.Type() != Ast.Type.TYPE_INT || factor.Type() != Ast.Type.TYPE_INT)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_INT;
        }
    }

    class MathTermTailNode : ExprBaseNode
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

        public override Type Type()
        {
            if (tail == null)
            {
                return factor.Type();
            }

            if (tail.Type() != Ast.Type.TYPE_INT || factor.Type() != Ast.Type.TYPE_INT)
            {
                return Ast.Type.TYPE_ERROR;
            }

            return Ast.Type.TYPE_INT;
        }
    }

    class MathFactorNode : ExprBaseNode
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

        public override Type Type()
        {
            return value?.Type() ?? expr?.Type() ?? factor?.Type() ?? Ast.Type.TYPE_ERROR;
        }
    }
}
