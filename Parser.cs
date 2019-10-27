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
			MoreToken();
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
			while (next.Tag != Tag.NULL)
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
					DiscardToken();
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
				DiscardToken();
			}
			else
			{
				new Error().PrintErrMsg();
			}

			string id = TryMatch(Tag.ID);
			if (id != null)
			{
				fun.name = id;
				node.name = id;
				ScopeManager.FunctionEnter(id);
			}

			TryMatch(Tag.DL_LPAR);
			Paras(fun);
			TryMatch(Tag.DL_RPAR);

			SymbolTable.AddFun(fun);

			node.body = new BlockNode();
			Block(node.body);

			fun.localVarSize = ScopeManager.LocalVarSize;

			ScopeManager.ScopeLeave();
			ScopeManager.FunctionLeave();
		}

		private void Paras(FunSymbol fun)
		{
			if (Utils.IsTypeTag(next.Tag, true))
			{
				Para(fun);
				if (next.Tag == Tag.DL_COM)
				{
					DiscardToken();
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
				DiscardToken();

				string id = TryMatch(Tag.ID);
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
			else if (next.Tag == Tag.KW_VOID)
			{
				if (fun.parasType.Count > 0)
				{
					new Error().PrintErrMsg();
				}

				DiscardToken();

				if (next.Tag == Tag.ID)
				{
					DiscardToken();
				}
			}
		}

		private void Block(BlockNode node)
		{
			TryMatch(Tag.DL_LBRACE);

			DeclsNode decls = new DeclsNode();
			node.AddChild(decls);
			Decls(decls);

			StmtsNode stmts = new StmtsNode();
			node.AddChild(stmts);
			Stmts(stmts);

			TryMatch(Tag.DL_RBRACE);
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
				DiscardToken();
			}

			node.name = TryMatch(Tag.ID);

			if (node.name != null)
			{
				ScopeManager.LocalVarSize += Utils.SizeOf(node.type);
				VarSymbol variable = new VarSymbol
				{
					name = node.name,
					type = node.type,
					scopeId = ScopeManager.CurrentScope,
					offsetInFun = ScopeManager.LocalVarSize
				};

				if (SymbolTable.AddVar(variable) == TableAddStatus.SYMBOL_EXIST)
				{
					new ConflictingDeclarationError().PrintErrMsg();
				}
			}

			if (next.Tag == Tag.DL_SET)
			{
				TryMatch(Tag.DL_SET);
				node.init = new ExprNode();
				Expr(node.init);

				if (node.init.Type() != node.type)
					if (!Utils.IsNumType(node.init.Type()) || !Utils.IsNumType(node.type))
						new TypeMismatchError(node.type, node.init.Type()).PrintErrMsg();
			}

			TryMatch(Tag.DL_SEM);
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
					DiscardToken();
					break;
				default:
					new UnknownTokenError().PrintErrMsg();
					DiscardToken();
					break;
			}
		}

		private void DiscardToken()
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

		private string TryMatch(Tag tokenTag)
		{
			string value = null;

			if (next.Tag == tokenTag)
			{
				value = next.Value;
				buffer.Dequeue();
			}
			else
			{
				new TokenMissingError(tokenTag).PrintErrMsg();
			}

			return value;
		}
	}
}
