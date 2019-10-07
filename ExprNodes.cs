using System;

namespace KayoCompiler.Ast
{
    class ExprNode : AstNode
    {
        public LogicExprNode expr;

        public override void Gen()
        {
            expr?.Gen();
        }
    }

    class LogicExprNode : AstNode
    {
        public LogicTermNode term;
        public LogicExprTailNode tail;

        public override void Gen()
        {
            term?.Gen();
            tail?.Gen();
        }
    }

    class LogicExprTailNode : AstNode
    {
        public LogicTermNode term;
        public LogicExprTailNode tail;

        public override void Gen()
        {
            term?.Gen();
            Console.WriteLine("or");
            tail?.Gen();
        }
    }

    class LogicTermNode : AstNode
    {
        public LogicFactorNode factor;
        public LogicTermTailNode tail;

        public override void Gen()
        {
            factor?.Gen();
            tail?.Gen();
        }
    }

    class LogicTermTailNode : AstNode
    {
        public LogicFactorNode factor;
        public LogicTermTailNode tail;

        public override void Gen()
        {
            factor?.Gen();
            Console.WriteLine("and");
            tail?.Gen();
        }
    }

    class LogicFactorNode : AstNode
    {
        public LogicRelNode rel;
        public LogicFactorTailNode tail;

        public override void Gen()
        {
            rel?.Gen();
            tail?.Gen();
        }
    }

    class LogicFactorTailNode : AstNode
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
    }

    class LogicRelNode : AstNode
    {
        public MathExprNode expr;
        public LogicRelTailNode tail;

        public override void Gen()
        {
            expr?.Gen();
            tail?.Gen();
        }
    }

    class LogicRelTailNode : AstNode
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
    }

    class MathExprNode : AstNode
    {
        public MathTermNode term;
        public MathExprTailNode tail;

        public override void Gen()
        {
            term?.Gen();
            tail?.Gen();
        }
    }

    class MathExprTailNode : AstNode
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
    }

    class MathTermNode : AstNode
    {
        public MathFactorNode factor;
        public MathTermTailNode tail;

        public override void Gen()
        {
            factor?.Gen();
            tail?.Gen();
        }
    }

    class MathTermTailNode : AstNode
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
    }

    class MathFactorNode : AstNode
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
    }
}
