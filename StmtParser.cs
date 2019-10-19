using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
	internal partial class Parser
	{
		private void IfStmt(IfStmtNode node)
		{
			ScopeManager.ScopeEnter();
			node.body = new StmtNode();
			DiscardToken();

			TryMatch(Tag.DL_LPAR);

            node.condition = new ExprNode();
			Expr(node.condition);

			if (node.condition == null)
			{
				new Error().PrintErrMsg();
			}
			else if (node.condition.Type() != VarType.TYPE_BOOL)
			{
				new TypeMismatchError(VarType.TYPE_BOOL, node.condition.Type()).PrintErrMsg();
			}

			TryMatch(Tag.DL_RPAR);

			Stmt(node.body);
			ScopeManager.ScopeLeave();

			if (next.Tag == Tag.KW_ELSE)
			{
				node.elseStmt = new ElseStmtNode();
				ElseStmt(node.elseStmt);
			}
		}

		private void ElseStmt(ElseStmtNode node)
		{
			ScopeManager.ScopeEnter();
			node.body = new StmtNode();
			DiscardToken();

			Stmt(node.body);
			ScopeManager.ScopeLeave();
		}

		private void SetStmt(SetStmtNode node)
		{
			node.id = new IdNode(next.Value);
			DiscardToken();

			TryMatch(Tag.DL_SET);

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
				new TypeMismatchError(node.id.Type(), node.expr.Type()).PrintErrMsg();
			}

			TryMatch(Tag.DL_SEM);
		}

		private void WhileStmt(WhileStmtNode node)
		{
			ScopeManager.ScopeEnter();
			node.body = new StmtNode();
			DiscardToken();

			TryMatch(Tag.DL_LPAR);

            node.condition = new ExprNode();
			Expr(node.condition);

			if (node.condition == null)
			{
				new Error().PrintErrMsg();
			}
			else if (node.condition.Type() != VarType.TYPE_BOOL)
			{
				new TypeMismatchError(VarType.TYPE_BOOL, node.condition.Type()).PrintErrMsg();
			}

			TryMatch(Tag.DL_RPAR);

			Stmt(node.body);
			ScopeManager.ScopeLeave();
		}

		private void ForStmt(ForStmtNode node)
		{
			ScopeManager.ScopeEnter();
			DiscardToken();

			TryMatch(Tag.DL_LPAR);

			node.preStmt = new StmtNode();
			Stmt(node.preStmt);

            node.condition = new ExprNode();
			Expr(node.condition);

			if (node.condition == null)
			{
				new Error().PrintErrMsg();
			}
			else if (node.condition.Type() != VarType.TYPE_BOOL)
			{
				new TypeMismatchError(VarType.TYPE_BOOL, node.condition.Type()).PrintErrMsg();
			}

			TryMatch(Tag.DL_SEM);

			node.loopStmt = new StmtNode();
			Stmt(node.loopStmt);

			TryMatch(Tag.DL_RPAR);

			node.body = new StmtNode();
			Stmt(node.body);
			ScopeManager.ScopeLeave();
		}

		private void WriteStmt(WriteStmtNode node)
		{
			CodeGenUtils.HasWrite = true;
			DiscardToken();
            node.expr = new ExprNode();
			Expr(node.expr);

			TryMatch(Tag.DL_SEM);
		}

		private void ReadStmt(ReadStmtNode node)
		{
			DiscardToken();

			if (next.Tag == Tag.ID)
			{
				node.id = new IdNode(next.Value);
				DiscardToken();

				if (node.id.Type() == VarType.TYPE_ERROR)
				{
					new Error().PrintErrMsg();
				}
			}
			else
			{
				new TokenMissingError(Tag.ID).PrintErrMsg();
			}

			TryMatch(Tag.DL_SEM);
		}

		private void FuncCallStmt(FuncCallStmtNode node)
		{
			node.name = TryMatch(Tag.ID);
			TryMatch(Tag.DL_LPAR);

			switch (next.Tag)
			{
				case Tag.ID:
				case Tag.NUM:
				case Tag.DL_LPAR:
				case Tag.KW_TRUE:
				case Tag.KW_FALSE:
				case Tag.DL_NOT:
					do
					{
                        if (next.Tag == Tag.DL_COM)
                            DiscardToken();
						var child = new ExprNode();
                        node.args.Add(child);
						Expr(child);
					} while (next.Tag == Tag.DL_COM);

					break;
			}

			TryMatch(Tag.DL_RPAR);

            if (node.args.Count != SymbolTable.FindFun(node.name).parasType.Count)
            {
                new Error().PrintErrMsg();
            }
            else
            {
                var argList = node.args;
                var paraList = SymbolTable.FindFun(node.name).parasType;
                for (int i = 0; i < argList.Count; i++)
                {
                    if (argList[i].Type() != paraList[i])
                    {
                        new TypeMismatchError(argList[i].Type(), paraList[i]).PrintErrMsg();
                        break;
                    }
                }
            }
		}

		private void ReturnStmt(ReturnStmtNode node)
		{
			DiscardToken();

			if (next.Tag != Tag.DL_SEM)
			{
                node.expr = new ExprNode();
				Expr(node.expr);

				if (node.expr?.Type() != SymbolTable.FindFun(ScopeManager.CurrentFun).returnType)
				{
					new Error().PrintErrMsg();
				}
			}

			TryMatch(Tag.DL_SEM);
		}
	}
}
