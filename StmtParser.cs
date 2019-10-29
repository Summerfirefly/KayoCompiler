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
			Move();

			RequiredToken(Tag.DL_LPAR);

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

			RequiredToken(Tag.DL_RPAR);

			Stmt(node.body);
			ScopeManager.ScopeLeave();

			if (TagIs(Tag.KW_ELSE))
			{
				node.elseStmt = new ElseStmtNode();
				ElseStmt(node.elseStmt);
			}
		}

		private void ElseStmt(ElseStmtNode node)
		{
			ScopeManager.ScopeEnter();
			node.body = new StmtNode();
			Move();

			Stmt(node.body);
			ScopeManager.ScopeLeave();
		}

		private void SetStmt(SetStmtNode node)
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

			RequiredToken(Tag.DL_SEM);
		}

		private void WhileStmt(WhileStmtNode node)
		{
			ScopeManager.ScopeEnter();
			node.body = new StmtNode();
			Move();

			RequiredToken(Tag.DL_LPAR);

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

			RequiredToken(Tag.DL_RPAR);

			Stmt(node.body);
			ScopeManager.ScopeLeave();
		}

		private void ForStmt(ForStmtNode node)
		{
			ScopeManager.ScopeEnter();
			Move();

			RequiredToken(Tag.DL_LPAR);

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

			RequiredToken(Tag.DL_SEM);

			node.loopStmt = new StmtNode();
			Stmt(node.loopStmt);

			RequiredToken(Tag.DL_RPAR);

			node.body = new StmtNode();
			Stmt(node.body);
			ScopeManager.ScopeLeave();
		}

		private void WriteStmt(WriteStmtNode node)
		{
			CodeGenUtils.HasWrite = true;
			Move();
            node.expr = new ExprNode();
			Expr(node.expr);

			RequiredToken(Tag.DL_SEM);
		}

		private void ReadStmt(ReadStmtNode node)
		{
			CodeGenUtils.HasRead = true;
			Move();

			if (TagIs(Tag.ID))
			{
				node.id = new IdNode(next.Value);
				Move();

				if (node.id.Type() == VarType.TYPE_ERROR)
				{
					new Error().PrintErrMsg();
				}
			}
			else
			{
				new TokenMissingError(Tag.ID).PrintErrMsg();
			}

			RequiredToken(Tag.DL_SEM);
		}

		private void FuncCallStmt(FuncCallStmtNode node)
		{
			node.name = RequiredToken(Tag.ID);
			RequiredToken(Tag.DL_LPAR);

			if (!TagIs(Tag.DL_RPAR))
			{
				do
				{
                    if (next.Tag == Tag.DL_COM)
                        Move();
					var child = new ExprNode();
                    node.args.Add(child);
					Expr(child);
				} while (next.Tag == Tag.DL_COM);
			}

			RequiredToken(Tag.DL_RPAR);

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
                        if (!Utils.IsNumType(argList[i].Type()) || !Utils.IsNumType(paraList[i]))
						{
							new TypeMismatchError(argList[i].Type(), paraList[i]).PrintErrMsg();
                        	break;
						}
                    }
                }
            }
		}

		private void ReturnStmt(ReturnStmtNode node)
		{
			Move();

			if (!TagIs(Tag.DL_SEM))
			{
                node.expr = new ExprNode();
				Expr(node.expr);

				if (node.expr.Type() != SymbolTable.FindFun(ScopeManager.CurrentFun).returnType)
				{
					if (!Utils.IsNumType(node.expr.Type()) ||
						!Utils.IsNumType(SymbolTable.FindFun(ScopeManager.CurrentFun).returnType))
					{
						new Error().PrintErrMsg();
					}
				}
			}

			RequiredToken(Tag.DL_SEM);
		}
	}
}
