using System.Collections.Generic;
using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
	internal partial class Parser
	{
		public static int LineNum
		{
			get
			{
				return next.LineNum;
			}
		}

		private static readonly List<Token> buffer = new List<Token>();
		private static int index = 0;
		private static Token next
		{
			get
			{
				return Lookahead(1);
			}
		}

		internal Parser(Scanner scanner)
		{
			var token = scanner.NextToken();
			do
			{
				if (token.Tag != Tag.COMMENT)
					buffer.Add(token);
				token = scanner.NextToken();
			} while (token.Tag != Tag.NULL);
			buffer.Add(scanner.NextToken());
		}

		internal ProgramNode Parse()
		{
			ProgramNode p = new ProgramNode();
			Program(p);
			ScopeManager.ScopeIdReset();

			return p;
		}

		private void Program(ProgramNode node)
		{
			while (!TagIs(Tag.NULL))
			{
				if (Utils.IsTypeTag(next.Tag, true))
				{
					FunctionNode function = new FunctionNode();
					node.AddChild(function);

					Function(function);
				}
				else
				{
					new UnknownTokenError().PrintErrMsg();
					Move();
				}
			}
		}

		private void Function(FunctionNode node)
		{
			FunSymbol fun = new FunSymbol();

			if (Utils.IsTypeTag(next.Tag, true))
			{
				fun.returnType = Utils.TagToType(next.Tag);
				Move();
			}
			else
			{
				new Error().PrintErrMsg();
			}

			string id = RequiredToken(Tag.ID);
			if (id != null)
			{
				fun.name = id;
				node.name = id;
				ScopeManager.FunctionEnter(id);
			}

			ScopeManager.ScopeEnter();

			RequiredToken(Tag.DL_LPAR);
			Paras(fun);
			RequiredToken(Tag.DL_RPAR);

			// 查看符号表中是否有同名函数
			FunSymbol tmp = SymbolTable.FindFun(fun.name);
			bool sameFunExist = false;
			if (tmp != null)
			{
				// 若存在同名函数则检查函数签名
				if (tmp.parasType.Count == fun.parasType.Count)
				{
					int i = 0;
					for (i = 0; i < tmp.parasType.Count; i++)
					{
						if (tmp.parasType[i] != fun.parasType[i])
							break;
					}

					sameFunExist = i == tmp.parasType.Count && tmp.returnType == fun.returnType;
				}
			}

			if (TagIs(Tag.DL_LBRACE))
			{
				// 函数定义
				fun.isExtern = false;
				if (sameFunExist)
				{
					if (tmp.isExtern)
						tmp.isExtern = false; // 存在函数定义，符号表中已有的函数不再是extern
					else
						new Error().PrintErrMsg(); // 存在重复定义
				}
			}
			else if (TagIs(Tag.DL_SEM))
			{
				// 函数声明
				fun.isExtern = true;
			}

			if (!sameFunExist)
			{
				if (SymbolTable.AddFun(fun) == TableAddStatus.SYMBOL_EXIST)
				{
					// 不同签名的函数名冲突
					new Error().PrintErrMsg();
				}
			}

			if (TagIs(Tag.DL_SEM))
			{
				Move();
			}
			else
			{
				node.body = new BlockNode();
				Block(node.body);
			}

			ScopeManager.ScopeLeave();
			ScopeManager.FunctionLeave();
		}

		private void Paras(FunSymbol fun)
		{
			if (Utils.IsTypeTag(next.Tag, true))
			{
				Para(fun);
				if (TagIs(Tag.DL_COM))
				{
					Move();
					Paras(fun);
				}
			}
		}

		private void Para(FunSymbol fun)
		{
			if (Utils.IsTypeTag(next.Tag, false))
			{
				VarType paraType = Utils.TagToType(next.Tag);
				fun.parasType.Add(paraType);
				Move();

				string id = next.Tag == Tag.ID ? next.Value : null;
				if (id != null)
				{
					Move();
					var status = SymbolTable.AddVar(new VarSymbol
					{
						type = paraType,
						name = id,
						scopeId = ScopeManager.CurrentScope,
						offsetInFun = -(1 + fun.parasType.Count) * 8
					});

					if (status == TableAddStatus.SYMBOL_EXIST)
					{
						new ConflictingDeclarationError().PrintErrMsg();
					}
				}
			}
			else if (TagIs(Tag.KW_VOID))
			{
				if (fun.parasType.Count > 0)
				{
					new Error().PrintErrMsg();
				}

				Move();

				if (TagIs(Tag.ID))
				{
					Move();
				}
			}
		}

		private void Block(BlockNode node)
		{
			RequiredToken(Tag.DL_LBRACE);

			DeclsNode decls = new DeclsNode();
			node.AddChild(decls);
			Decls(decls);

			StmtsNode stmts = new StmtsNode();
			node.AddChild(stmts);
			Stmts(stmts);

			RequiredToken(Tag.DL_RBRACE);
		}

		private void Decls(DeclsNode node)
		{
			if (Utils.IsTypeTag(next.Tag, false))
			{
				DeclNode decl = new DeclNode();
				node.AddChild(decl);
				Decl(decl);
				Decls(node);
			}
		}

		private void Decl(DeclNode node)
		{
			if (Utils.IsTypeTag(next.Tag, false))
			{
				node.type = Utils.TagToType(next.Tag);
				Move();
			}

			node.name = RequiredToken(Tag.ID);

			if (node.name != null)
			{
				FunSymbol function = SymbolTable.FindFun(ScopeManager.CurrentFun);
				function.localVarSize += Utils.SizeOf(node.type);

				VarSymbol variable = new VarSymbol
				{
					name = node.name,
					type = node.type,
					scopeId = ScopeManager.CurrentScope,
					offsetInFun = function.localVarSize
				};

				if (SymbolTable.AddVar(variable) == TableAddStatus.SYMBOL_EXIST)
				{
					new ConflictingDeclarationError().PrintErrMsg();
				}
			}

			if (TagIs(Tag.DL_SET))
			{
				Move();
				node.init = new ExprNode();
				Expr(node.init);

				if (node.init.Type() != node.type)
					if (!Utils.IsNumType(node.init.Type()) || !Utils.IsNumType(node.type))
						new TypeMismatchError(node.type, node.init.Type()).PrintErrMsg();
			}

			RequiredToken(Tag.DL_SEM);
		}

		private void Stmts(StmtsNode node)
		{
			switch (next.Tag)
			{
				case Tag.ID:
				case Tag.KW_TRUE:
				case Tag.KW_FALSE:
				case Tag.NUM:
				case Tag.DL_LPAR:
				case Tag.DL_PLUS:
				case Tag.DL_MINUS:
				case Tag.DL_NOT:
				case Tag.KW_IF:
				case Tag.KW_WHILE:
				case Tag.KW_FOR:
				case Tag.KW_WRITE:
				case Tag.KW_READ:
				case Tag.KW_RETURN:
				case Tag.DL_LBRACE:
				case Tag.DL_SEM:
					StmtNode child = new StmtNode();
					node.AddChild(child);
					Stmt(child);
					Stmts(node);
					break;
			}
		}

		private void Stmt(StmtNode node)
		{
			switch (next.Tag)
			{
				case Tag.KW_TRUE:
				case Tag.KW_FALSE:
				case Tag.DL_LPAR:
				case Tag.DL_PLUS:
				case Tag.DL_MINUS:
				case Tag.DL_NOT:
				case Tag.ID:
				case Tag.NUM:
					node.stmt = new ExprNode();
					Expr(node.stmt as ExprNode);
            		if ((node.stmt as ExprNode).Type() == VarType.TYPE_ERROR)
            		{
                		new Error().PrintErrMsg();
            		}
					RequiredToken(Tag.DL_SEM);
					break;
				case Tag.KW_IF:
					node.stmt = new IfStmtNode();
					IfStmt(node.stmt as IfStmtNode);
					break;
				case Tag.KW_WHILE:
					node.stmt = new WhileStmtNode();
					WhileStmt(node.stmt as WhileStmtNode);
					break;
				case Tag.KW_FOR:
					node.stmt = new ForStmtNode();
					ForStmt(node.stmt as ForStmtNode);
					break;
				case Tag.KW_WRITE:
					node.stmt = new WriteStmtNode();
					WriteStmt(node.stmt as WriteStmtNode);
					break;
				case Tag.KW_READ:
					node.stmt = new ReadStmtNode();
					ReadStmt(node.stmt as ReadStmtNode);
					break;
				case Tag.KW_RETURN:
					node.stmt = new ReturnStmtNode();
					ReturnStmt(node.stmt as ReturnStmtNode);
					break;
				case Tag.DL_LBRACE:
					node.stmt = new BlockNode();
					Block(node.stmt as BlockNode);
					break;
				case Tag.DL_SEM:
					Move();
					break;
				default:
					new UnknownTokenError().PrintErrMsg();
					Move();
					break;
			}
		}

		private void Move()
		{
			if (buffer.Count > index)
				index++;
		}

		private static Token Lookahead(int k)
		{
			if (buffer.Count <= index + k)
			{
				return buffer[buffer.Count - 1];
			}

			return buffer[index + k - 1];
		}

		private string RequiredToken(Tag tokenTag)
		{
			string value = null;

			if (next.Tag == tokenTag)
			{
				value = next.Value;
				Move();
			}
			else
			{
				new TokenMissingError(tokenTag).PrintErrMsg();
			}

			return value;
		}

		private bool TagIs(Tag expectTag)
		{
			return next.Tag == expectTag;
		}
	}
}
