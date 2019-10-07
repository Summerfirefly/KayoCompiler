using System;
using System.Collections.Generic;
using System.Text;
using KayoCompiler.Ast;

namespace KayoCompiler
{
    class Parser
    {
        readonly Scanner scanner = null;
        readonly ProgramNode p = new ProgramNode();
        Token next = null;

        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
            Move();
        }

        public void StartParse()
        {
            if (next == null) return;

            Program(p);
            p.Gen();
        }

        private void Program(ProgramNode node)
        {
            if (next.Tag == Tag.DL_LBRACE)
            {
                BlockNode block = new BlockNode();
                node.AddChild(block);

                Block(block);
            }
            else
            {
                new Error(scanner.LineNum).PrintErrMsg();
            }
        }

        private void Block(BlockNode node)
        {
            if (next.Tag == Tag.DL_LBRACE)
            {
                Move();

                DeclsNode decls = new DeclsNode();
                node.AddChild(decls);
                Decls(decls);

                StmtsNode stmts = new StmtsNode();
                node.AddChild(stmts);
                Stmts(stmts);

                if (next.Tag == Tag.DL_RBRACE)
                {
                    Move();
                }
                else
                {
                    new Error(scanner.LineNum).PrintErrMsg();
                }
            }
            else
            {
                new Error(scanner.LineNum).PrintErrMsg();
            }
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
                    node.type = "int";
                    Move();
                    break;
                case Tag.KW_BOOL:
                    node.type = "bool";
                    Move();
                    break;
            }

            node.name = next.Value;
            Move();

            if (next.Tag == Tag.DL_SEM)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();
        }

        private void Stmts(StmtsNode node)
        {
            switch (next.Tag)
            {
                case Tag.ID:
                case Tag.KW_IF:
                case Tag.KW_WHILE:
                case Tag.KW_WRITE:
                case Tag.KW_READ:
                case Tag.DL_LBRACE:
                    StmtNode child = new StmtNode();
                    node.AddChild(child);

                    Stmt(child);
                    Stmts(node);
                    break;
                default:
                    break;
            }
        }

        private void Stmt(StmtNode node)
        {
            node.stmt = new EmptyNode();
            switch (next.Tag)
            {
                case Tag.ID:
                    SetStmtNode setStmt = new SetStmtNode();
                    node.stmt = setStmt;
                    SetStmt(setStmt);
                    break;
                case Tag.KW_WHILE:
                    WhileStmtNode whileStmt = new WhileStmtNode();
                    node.stmt = whileStmt;
                    WhileStmt(whileStmt);
                    break;
                case Tag.KW_WRITE:
                    WriteStmtNode stmt = new WriteStmtNode();
                    node.stmt = stmt;
                    WriteStmt(stmt);
                    break;
                case Tag.KW_READ:
                    ReadStmtNode readStmt = new ReadStmtNode();
                    node.stmt = readStmt;
                    ReadStmt(readStmt);
                    break;
                case Tag.DL_LBRACE:
                    BlockNode block = new BlockNode();
                    node.stmt = block;
                    Block(block);
                    break;
                case Tag.DL_SEM:
                    Move();
                    break;
                default:
                    new Error(scanner.LineNum).PrintErrMsg();
                    Move();
                    break;
            }
        }

        private void SetStmt(SetStmtNode node)
        {
            node.id = next.Value;
            node.expr = new ExprNode();
            Move();

            if (next.Tag == Tag.DL_SET)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();

            Expr(node.expr);

            if (next.Tag == Tag.DL_SEM)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();
        }

        private void WhileStmt(WhileStmtNode node)
        {
            node.condition = new ExprNode();
            node.body = new StmtNode();
            Move();

            if (next.Tag == Tag.DL_LPAR)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();

            Expr(node.condition);

            if (next.Tag == Tag.DL_RPAR)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();

            Stmt(node.body);
        }

        private void WriteStmt(WriteStmtNode node)
        {
            node.expr = new ExprNode();
            Move();
            Expr(node.expr);

            if (next.Tag == Tag.DL_SEM)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();
        }

        private void ReadStmt(ReadStmtNode node)
        {
            Move();

            if (next.Tag == Tag.ID)
            {
                node.id = next.Value;
                Move();
            }
            else
                new Error(scanner.LineNum).PrintErrMsg();

            if (next.Tag == Tag.DL_SEM)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();
        }

        private void Expr(ExprNode node)
        {
            if (next == null) return;
            node.term = new TermNode();
            node.expr = new ExprTail();

            Term(node.term);
            ExprTail(node.expr);
        }

        private void ExprTail(ExprTail node)
        {
            if (next == null) return;
            node.term = new TermNode();
            node.expr = new ExprTail();

            if (next.Tag == Tag.DL_PLUS)
            {
                Move();
                node.op = "+";
                Term(node.term);
                ExprTail(node.expr);
            }
            else if (next.Tag == Tag.DL_MINUS)
            {
                Move();
                node.op = "-";
                Term(node.term);
                ExprTail(node.expr);
            }
        }

        private void Term(TermNode node)
        {
            if (next == null) return;

            node.factor = new FactorNode();
            node.term = new TermTail();

            Factor(node.factor);
            TermTail(node.term);
        }

        private void TermTail(TermTail node)
        {
            if (next == null) return;

            node.factor = new FactorNode();
            node.term = new TermTail();

            if (next.Tag == Tag.DL_MULTI)
            {
                Move();
                node.op = "*";
                Factor(node.factor);
                TermTail(node.term);
            }
            else if (next.Tag == Tag.DL_OBELUS)
            {
                Move();
                node.op = "/";
                Factor(node.factor);
                TermTail(node.term);
            }
        }

        private void Factor(FactorNode node)
        {
            if (next == null) return;

            if (next.Tag == Tag.NUM)
            {
                node.value = new IntNode(int.Parse(next.Value));
                Move();
            }
            else if (next.Tag == Tag.ID)
            {
                node.value = new IdNode(next.Value);
                Move();
            }
            else if (next.Tag == Tag.DL_LPAR)
            {
                node.value = new ExprNode();
                Move();
                Expr(node.value as ExprNode);
                if (next.Tag == Tag.DL_RPAR)
                    Move();
                else
                    new Error(scanner.LineNum).PrintErrMsg();
            }
        }

        private void Move()
        {
            next = scanner.NextToken();
        }
    }
}
