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
				switch (next.Tag)
				{
					case Tag.KW_VOID:
					case Tag.KW_INT:
					case Tag.KW_BOOL:
						FunctionNode function = new FunctionNode();
						node.AddChild(function);

						Function(function);
						break;
					default:
						new UnknownTokenError().PrintErrMsg();
						DiscardToken();
						break;
				}
			}

			if (next.Tag != Tag.NULL)
			{
				new UnknownTokenError().PrintErrMsg();
				DiscardToken();
			}
		}

		private void Function(FunctionNode node)
		{
			ScopeManager.ScopeEnter();
			FunSymbol fun = new FunSymbol();

			switch (next.Tag)
			{
				case Tag.KW_VOID:
					fun.returnType = VarType.TYPE_VOID;
					DiscardToken();
					break;
				case Tag.KW_BOOL:
					fun.returnType = VarType.TYPE_BOOL;
					DiscardToken();
					break;
				case Tag.KW_INT:
					fun.returnType = VarType.TYPE_INT;
					DiscardToken();
					break;
				default:
					new Error().PrintErrMsg();
					break;
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

			fun.localVarCount = ScopeManager.LocalVarCount;

			ScopeManager.ScopeLeave();
			ScopeManager.FunctionLeave();
		}

		private void Paras(FunSymbol fun)
		{
			switch (next.Tag)
			{
				case Tag.KW_VOID:
				case Tag.KW_INT:
				case Tag.KW_BOOL:
					Para(fun);
					if (next.Tag == Tag.DL_COM)
					{
						DiscardToken();
						Paras(fun);
					}
					break;
			}
		}

		private void Para(FunSymbol fun)
		{
			switch (next.Tag)
			{
				case Tag.KW_VOID:
					DiscardToken();
					break;
				case Tag.KW_INT:
				case Tag.KW_BOOL:
					VarType paraType = VarType.TYPE_ERROR;
					switch (next.Tag)
					{
						case Tag.KW_INT:
							paraType = VarType.TYPE_INT;
							break;
						case Tag.KW_BOOL:
							paraType = VarType.TYPE_BOOL;
							break;
					}

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
							indexInFun = -2 - fun.parasType.Count
						});

						if (status == TableAddStatus.SYMBOL_EXIST)
						{
							new ConflictingDeclarationError().PrintErrMsg();
						}
					}
					break;
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
			switch (next.Tag)
			{
				case Tag.KW_INT:
				case Tag.KW_BOOL:
					DeclNode decl = new DeclNode();
					node.AddChild(decl);
					Decl(decl);
					Decls(node);
					break;
			}
		}

		private void Decl(DeclNode node)
		{
			switch (next.Tag)
			{
				case Tag.KW_INT:
					node.type = VarType.TYPE_INT;
					DiscardToken();
					break;
				case Tag.KW_BOOL:
					node.type = VarType.TYPE_BOOL;
					DiscardToken();
					break;
			}

			node.name = TryMatch(Tag.ID);

			if (node.name != null)
			{
				VarSymbol variable = new VarSymbol
				{
					name = node.name,
					type = node.type,
					scopeId = ScopeManager.CurrentScope,
					indexInFun = ScopeManager.LocalVarCount++
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
