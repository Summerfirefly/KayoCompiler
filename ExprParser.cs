using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
    internal partial class Parser
    {
        private void Expr(ref ExprNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new ExprNode();
                    var child = node.AddChild(new LogicTermNode());
                    LogicTerm(ref child);
                    while (next?.Tag == Tag.DL_OR)
                    {
                        child = node.AddChild(new LogicTermNode());
                        LogicTerm(ref child);
                    }
                    break;
            }
        }

        private void LogicTerm(ref LogicTermNode node)
        {
            if (next?.Tag == Tag.DL_OR)
            {
                node.Op = next.Tag;
                Move();
            }

            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    var child = node.AddChild(new LogicFactorNode());
                    LogicFactor(ref child);
                    while (next?.Tag == Tag.DL_AND)
                    {
                        child = node.AddChild(new LogicFactorNode());
                        LogicFactor(ref child);
                    }
                    break;
                default:
                    node = null;
                    break;
            }
        }

        private void LogicFactor(ref LogicFactorNode node)
        {
            if (next?.Tag == Tag.DL_AND)
            {
                node.Op = next.Tag;
                Move();
            }

            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    var child = node.AddChild(new LogicRelNode());
                    LogicRel(ref child);
                    while (next?.Tag == Tag.DL_EQ || next?.Tag == Tag.DL_NEQ)
                    {
                        child = node.AddChild(new LogicRelNode());
                        LogicRel(ref child);
                    }
                    break;
                default:
                    node = null;
                    break;
            }
        }

        private void LogicRel(ref LogicRelNode node)
        {
            if (next?.Tag == Tag.DL_EQ || next?.Tag == Tag.DL_NEQ)
            {
                node.Op = next.Tag;
                Move();
            }

            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    var child = node.AddChild(new MathExprNode());
                    MathExpr(ref child);
                    while (
                        next?.Tag == Tag.DL_LT ||
                        next?.Tag == Tag.DL_NLT ||
                        next?.Tag == Tag.DL_GT ||
                        next?.Tag == Tag.DL_NGT)
                    {
                        child = node.AddChild(new MathExprNode());
                        MathExpr(ref child);
                    }
                    break;
                default:
                    node = null;
                    break;
            }
        }

        private void MathExpr(ref MathExprNode node)
        {
            if (next?.Tag == Tag.DL_LT ||
                next?.Tag == Tag.DL_NLT ||
                next?.Tag == Tag.DL_GT ||
                next?.Tag == Tag.DL_NGT)
            {
                node.Op = next.Tag;
                Move();
            }

            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    var child = node.AddChild(new MathTermNode());
                    MathTerm(ref child);
                    while (next?.Tag == Tag.DL_PLUS || next?.Tag == Tag.DL_MINUS)
                    {
                        child = node.AddChild(new MathTermNode());
                        MathTerm(ref child);
                    }
                    break;
                default:
                    node = null;
                    break;
            }
        }

        private void MathTerm(ref MathTermNode node)
        {
            if (next?.Tag == Tag.DL_PLUS || next?.Tag == Tag.DL_MINUS)
            {
                node.Op = next.Tag;
                Move();
            }

            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    var child = node.AddChild(new MathFactorNode());
                    MathFactor(ref child);
                    while (next?.Tag == Tag.DL_MULTI || next?.Tag == Tag.DL_OBELUS)
                    {
                        child = node.AddChild(new MathFactorNode());
                        MathFactor(ref child);
                    }
                    break;
                default:
                    node = null;
                    break;
            }
        }

        private void MathFactor(ref MathFactorNode node)
        {
            while (next?.Tag == Tag.DL_MULTI || next?.Tag == Tag.DL_OBELUS)
            {
                node.Op = next.Tag;
                Move();
            }

            switch (next?.Tag)
            {
                case Tag.ID:
                    node.value = new IdNode(next.Value);
                    Move();
                    break;
                case Tag.NUM:
                    node.value = new IntNode(int.Parse(next.Value));
                    Move();
                    break;
                case Tag.DL_LPAR:
                    Move();
                    Expr(ref node.expr);
                    if (next.Tag == Tag.DL_RPAR)
                        Move();
                    else
                        new TokenMissingError(Tag.DL_RPAR).PrintErrMsg();
                    break;
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                    node.value = new BoolNode(bool.Parse(next.Value));
                    Move();
                    break;
                case Tag.DL_NOT:
                    Move();
                    node.factor = new MathFactorNode();
                    MathFactor(ref node.factor);
                    break;
                default:
                    node = null;
                    break;
            }
        }
    }
}
