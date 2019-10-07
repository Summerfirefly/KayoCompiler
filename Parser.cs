using System;
using System.Collections.Generic;
using System.Text;
using KayoCompiler.Ast;

namespace KayoCompiler
{
    partial class Parser
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
            if (next == null) return;

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
            if (next == null) return;

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
            if (next == null) return;

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
            if (next == null) return;

            switch (next.Tag)
            {
                case Tag.KW_INT:
                case Tag.KW_BOOL:
                    node.type = next.Tag;
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
            switch (next?.Tag)
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
            switch (next?.Tag)
            {
                case Tag.ID:
                    node.stmt = new SetStmtNode();
                    SetStmt(node.stmt as SetStmtNode);
                    break;
                case Tag.KW_IF:
                    node.stmt = new IfStmtNode();
                    IfStmt(node.stmt as IfStmtNode);
                    break;
                case Tag.KW_WHILE:
                    node.stmt = new WhileStmtNode();
                    WhileStmt(node.stmt as WhileStmtNode);
                    break;
                case Tag.KW_WRITE:
                    node.stmt = new WriteStmtNode();
                    WriteStmt(node.stmt as WriteStmtNode);
                    break;
                case Tag.KW_READ:
                    node.stmt = new ReadStmtNode();
                    ReadStmt(node.stmt as ReadStmtNode);
                    break;
                case Tag.DL_LBRACE:
                    node.stmt = new BlockNode();
                    Block(node.stmt as BlockNode);
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

        private void Move()
        {
            next = scanner.NextToken();

            while (next?.Tag == Tag.COMMENT)
            {
                next = scanner.NextToken();
            }
        }
    }
}
