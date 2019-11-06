using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
    internal partial class Parser
    {
        private void Expr(ExprNode node)
        {
            int tmp = index;
            Unary(new UnaryNode());
            if (TagIs(Tag.DL_SET))
            {
                index = tmp;
                node.assignment = new AssignmentExprNode();
                AssignmentExpr(node.assignment);
            }
            else
            {
                index = tmp;
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
			node.leftV = new UnaryNode();
			Unary(node.leftV);

            if (!Utils.IsValidLeftValue(node.leftV))
            {
                new InvalidLeftValueError().PrintErrMsg();
            }

			RequiredToken(Tag.DL_SET);

            node.expr = new ExprNode();
			Expr(node.expr);

			if (node.leftV.Type() == VarType.TYPE_ERROR)
			{
				new Error().PrintErrMsg();
			}
			else if (node.leftV.Type() != node.expr.Type())
			{
				if (!Utils.IsNumType(node.leftV.Type()) || !Utils.IsNumType(node.expr.Type()))
					new TypeMismatchError(node.leftV.Type(), node.expr.Type()).PrintErrMsg();
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
            var child = node.AddChild(new UnaryNode());
            Unary(child);
            while (TagIs(Tag.DL_MULTI) || TagIs(Tag.DL_OBELUS) || TagIs(Tag.DL_MOD))
            {
                child = node.AddChild(new UnaryNode());
                child.Op = next.Tag;
                Move();
                Unary(child);
            }
        }

        private void Unary(UnaryNode node)
        {
            while (TagIs(Tag.DL_PLUS) || TagIs(Tag.DL_MINUS) || TagIs(Tag.DL_NOT))
            {
                node.unaryOp.Add(next.Tag);
                Move();
            }

            node.factor = new FactorNode();
            Factor(node.factor);

            if (node.factor.Type() == VarType.TYPE_BOOL)
            {
                foreach (var op in node.unaryOp)
                {
                    if (op != Tag.DL_NOT)
                    {
                        new Error().PrintErrMsg();
                        break;
                    }
                }
            }
            else if (Utils.IsNumType(node.factor.Type()))
            {
                foreach (var op in node.unaryOp)
                {
                    if (op != Tag.DL_PLUS && op != Tag.DL_MINUS)
                    {
                        new Error().PrintErrMsg();
                        break;
                    }
                }
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
                    else if (Lookahead(2).Tag == Tag.DL_LSQU)
                    {
                        node.value = new IdNode(next.Value);
                        Move();
                        Move();
                        node.indexer = new ExprNode();
                        Expr(node.indexer);
                        RequiredToken(Tag.DL_RSQU);
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
                    new UnknownTokenError().PrintErrMsg();
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
