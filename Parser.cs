using System.Collections.Generic;
using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
	internal partial class Parser
	{
		private readonly Scanner scanner = null;
		private readonly Queue<Token> buffer;
		private Token next
		{
			get
			{
				if (buffer.Count == 0)
					MoreToken();
				
				return buffer.Peek();
			}
		}

		internal Parser(Scanner scanner)
		{
			this.scanner = scanner;
			buffer = new Queue<Token>();
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
			ScopeManager.ScopeEnter();
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

			RequiredToken(Tag.DL_LPAR);
			Paras(fun);
			RequiredToken(Tag.DL_RPAR);

			SymbolTable.AddFun(fun);

			node.body = new BlockNode();
			Block(node.body);

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

				string id = RequiredToken(Tag.ID);
				if (id != null)
				{
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
				case Tag.ID:
					var moreToken = MoreToken();
					if (moreToken.Tag == Tag.DL_SET)
					{
						node.stmt = new SetStmtNode();
						SetStmt(node.stmt as SetStmtNode);
					}
					else if (moreToken.Tag == Tag.DL_LPAR)
					{
						node.stmt = new FuncCallStmtNode();
						FuncCallStmt(node.stmt as FuncCallStmtNode);
					}
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
			if (buffer.Count > 0)
				buffer.Dequeue();
		}

		private Token MoreToken()
		{
			Token token = scanner.NextToken();

			while (token.Tag == Tag.COMMENT)
			{
				token = scanner.NextToken();
			}

			buffer.Enqueue(token);
			return token;
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
