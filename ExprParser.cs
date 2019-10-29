using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
    internal partial class Parser
    {
        private void Expr(ExprNode node)
        {
            if (TagIs(Tag.ID) && Lookahead(2).Tag == Tag.DL_SET)
            {
                node.assignment = new AssignmentExprNode();
                AssignmentExpr(node.assignment);
            }
            else
            {
                var child = node.AddChild(new AndExprNode());
                AndExpr(child);
                while (TagIs(Tag.DL_OR))
                {
                    child = node.AddChild(new AndExprNode());
                    child.Op = Tag.DL_OR;
                    Move();
                    AndExpr(child);
                }
            }
        }
        
		private void AssignmentExpr(AssignmentExprNode node)
		{
			node.id = new IdNode(next.Value);
			Move();

			RequiredToken(Tag.DL_SET);

            node.expr = new ExprNode();
			Expr(node.expr);

			if (node.expr == null)
			{
				new Error().PrintErrMsg();
			}
			else if (node.id.Type() == VarType.TYPE_ERROR)
			{
				new Error().PrintErrMsg();
			}
			else if (node.id.Type() != node.expr.Type())
			{
				if (!Utils.IsNumType(node.id.Type()) || !Utils.IsNumType(node.expr.Type()))
					new TypeMismatchError(node.id.Type(), node.expr.Type()).PrintErrMsg();
			}
		}

        private void AndExpr(AndExprNode node)
        {
            var child = node.AddChild(new EqualExprNode());
            EqualExpr(child);
            while (TagIs(Tag.DL_AND))
            {
                child = node.AddChild(new EqualExprNode());
                child.Op = Tag.DL_AND;
                Move();
                EqualExpr(child);
            }
        }

        private void EqualExpr(EqualExprNode node)
        {
            var child = node.AddChild(new CmpExprNode());
            CmpExpr(child);
            while (TagIs(Tag.DL_EQ) || TagIs(Tag.DL_NEQ))
            {
                child = node.AddChild(new CmpExprNode());
                child.Op = next.Tag;
                Move();
                CmpExpr(child);
            }
        }

        private void CmpExpr(CmpExprNode node)
        {
            var child = node.AddChild(new AddExprNode());
            AddExpr(child);
            while (
                TagIs(Tag.DL_LT) ||
                TagIs(Tag.DL_NLT) ||
                TagIs(Tag.DL_GT) ||
                TagIs(Tag.DL_NGT))
            {
                child = node.AddChild(new AddExprNode());
                child.Op = next.Tag;
                Move();
                AddExpr(child);
            }
        }

        private void AddExpr(AddExprNode node)
        {
            var child = node.AddChild(new MulExprNode());
            MulExpr(child);
            while (TagIs(Tag.DL_PLUS) || TagIs(Tag.DL_MINUS))
            {
                child = node.AddChild(new MulExprNode());
                child.Op = next.Tag;
                Move();
                MulExpr(child);
            }
        }

        private void MulExpr(MulExprNode node)
        {
            var child = node.AddChild(new FactorNode());
            Factor(child);
            while (TagIs(Tag.DL_MULTI) || TagIs(Tag.DL_OBELUS) || TagIs(Tag.DL_MOD))
            {
                child = node.AddChild(new FactorNode());
                child.Op = next.Tag;
                Move();
                Factor(child);
            }
        }

        private void Factor(FactorNode node)
        {
            switch (next.Tag)
            {
                case Tag.ID:
                    if (Lookahead(2).Tag == Tag.DL_LPAR)
                    {
                        node.func = new FuncCallStmtNode();
                        FuncCallStmt(node.func);
                    }
                    else
                    {
                        node.value = new IdNode(next.Value);
                        Move();
                    }
                    break;
                case Tag.NUM:
                    node.value = new IntNode(IntParse(next.Value));
                    Move();
                    break;
                case Tag.DL_PLUS:
                case Tag.DL_MINUS:
                case Tag.DL_NOT:
                    node.factorOp = next.Tag;
                    Move();
                    node.factor = new FactorNode();
                    Factor(node.factor);
                    break;
                case Tag.DL_LPAR:
                    Move();
                    node.expr = new ExprNode();
                    Expr(node.expr);
                    RequiredToken(Tag.DL_RPAR);
                    break;
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                    node.value = new BoolNode(bool.Parse(next.Value));
                    Move();
                    break;
                default:
                    new Error().PrintErrMsg();
                    break;
            }
        }

        private long IntParse(string str)
        {
            long result = 0;

            foreach (byte ch in str)
            {
                result = result * 10 + ch - '0';
            }

            return result;
        }
    }
}
