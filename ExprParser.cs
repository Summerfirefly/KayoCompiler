using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
    internal partial class Parser
    {
        private void Expr(ExprNode node)
        {
            switch (next.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    var child = node.AddChild(new LogicTermNode());
                    LogicTerm(ref child);
                    while (next.Tag == Tag.DL_OR)
                    {
                        child = node.AddChild(new LogicTermNode());
                        LogicTerm(ref child);
                    }
                    break;
            }
        }

        private void LogicTerm(ref LogicTermNode node)
        {
            if (next.Tag == Tag.DL_OR)
            {
                node.Op = next.Tag;
                DiscardToken();
            }

            switch (next.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    var child = node.AddChild(new LogicFactorNode());
                    LogicFactor(ref child);
                    while (next.Tag == Tag.DL_AND)
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
            if (next.Tag == Tag.DL_AND)
            {
                node.Op = next.Tag;
                DiscardToken();
            }

            switch (next.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    var child = node.AddChild(new LogicRelNode());
                    LogicRel(ref child);
                    while (next.Tag == Tag.DL_EQ || next.Tag == Tag.DL_NEQ)
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
            if (next.Tag == Tag.DL_EQ || next.Tag == Tag.DL_NEQ)
            {
                node.Op = next.Tag;
                DiscardToken();
            }

            switch (next.Tag)
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
                        next.Tag == Tag.DL_LT ||
                        next.Tag == Tag.DL_NLT ||
                        next.Tag == Tag.DL_GT ||
                        next.Tag == Tag.DL_NGT)
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
            if (next.Tag == Tag.DL_LT ||
                next.Tag == Tag.DL_NLT ||
                next.Tag == Tag.DL_GT ||
                next.Tag == Tag.DL_NGT)
            {
                node.Op = next.Tag;
                DiscardToken();
            }

            switch (next.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    var child = node.AddChild(new MathTermNode());
                    MathTerm(ref child);
                    while (next.Tag == Tag.DL_PLUS || next.Tag == Tag.DL_MINUS)
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
            if (next.Tag == Tag.DL_PLUS || next.Tag == Tag.DL_MINUS)
            {
                node.Op = next.Tag;
                DiscardToken();
            }

            switch (next.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    var child = node.AddChild(new MathFactorNode());
                    MathFactor(ref child);
                    while (next.Tag == Tag.DL_MULTI || next.Tag == Tag.DL_OBELUS || next.Tag == Tag.DL_MOD)
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
            if (next.Tag == Tag.DL_MULTI || next.Tag == Tag.DL_OBELUS || next.Tag == Tag.DL_MOD)
            {
                node.Op = next.Tag;
                DiscardToken();
            }

            switch (next.Tag)
            {
                case Tag.ID:
                    var moreToken = MoreToken();
                    if (moreToken.Tag == Tag.DL_LPAR)
                    {
                        node.func = new FuncCallStmtNode();
                        FuncCallStmt(node.func);
                    }
                    else
                    {
                        node.value = new IdNode(next.Value);
                        DiscardToken();
                    }
                    break;
                case Tag.NUM:
                    node.value = new IntNode(int.Parse(next.Value));
                    DiscardToken();
                    break;
                case Tag.DL_LPAR:
                    DiscardToken();
                    node.expr = new ExprNode();
                    Expr(node.expr);
                    TryMatch(Tag.DL_RPAR);
                    break;
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                    node.value = new BoolNode(bool.Parse(next.Value));
                    DiscardToken();
                    break;
                case Tag.DL_NOT:
                    DiscardToken();
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
