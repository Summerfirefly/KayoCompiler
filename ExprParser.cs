using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
    internal partial class Parser
    {
        private void Expr(ExprNode node)
        {
            var child = node.AddChild(new AndExprNode());
            AndExpr(child);
            while (next.Tag == Tag.DL_OR)
            {
                child = node.AddChild(new AndExprNode());
                child.Op = Tag.DL_OR;
                DiscardToken();
                AndExpr(child);
            }
        }

        private void AndExpr(AndExprNode node)
        {
            var child = node.AddChild(new EqualExprNode());
            EqualExpr(child);
            while (next.Tag == Tag.DL_AND)
            {
                child = node.AddChild(new EqualExprNode());
                child.Op = Tag.DL_AND;
                DiscardToken();
                EqualExpr(child);
            }
        }

        private void EqualExpr(EqualExprNode node)
        {
            var child = node.AddChild(new CmpExprNode());
            CmpExpr(child);
            while (next.Tag == Tag.DL_EQ || next.Tag == Tag.DL_NEQ)
            {
                child = node.AddChild(new CmpExprNode());
                child.Op = next.Tag;
                DiscardToken();
                CmpExpr(child);
            }
        }

        private void CmpExpr(CmpExprNode node)
        {
            var child = node.AddChild(new AddExprNode());
            AddExpr(child);
            while (
                next.Tag == Tag.DL_LT ||
                next.Tag == Tag.DL_NLT ||
                next.Tag == Tag.DL_GT ||
                next.Tag == Tag.DL_NGT)
            {
                child = node.AddChild(new AddExprNode());
                child.Op = next.Tag;
                DiscardToken();
                AddExpr(child);
            }
        }

        private void AddExpr(AddExprNode node)
        {
            var child = node.AddChild(new MulExprNode());
            MulExpr(child);
            while (next.Tag == Tag.DL_PLUS || next.Tag == Tag.DL_MINUS)
            {
                child = node.AddChild(new MulExprNode());
                child.Op = next.Tag;
                DiscardToken();
                MulExpr(child);
            }
        }

        private void MulExpr(MulExprNode node)
        {
            var child = node.AddChild(new FactorNode());
            Factor(child);
            while (next.Tag == Tag.DL_MULTI || next.Tag == Tag.DL_OBELUS || next.Tag == Tag.DL_MOD)
            {
                child = node.AddChild(new FactorNode());
                child.Op = next.Tag;
                DiscardToken();
                Factor(child);
            }
        }

        private void Factor(FactorNode node)
        {
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
                case Tag.DL_PLUS:
                case Tag.DL_MINUS:
                case Tag.DL_NOT:
                    node.factorOp = next.Tag;
                    DiscardToken();
                    node.factor = new FactorNode();
                    Factor(node.factor);
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
                default:
                    new Error().PrintErrMsg();
                    break;
            }
        }
    }
}
