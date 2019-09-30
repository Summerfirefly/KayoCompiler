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

        private void Stmts(StmtsNode node)
        {
            if (next.Tag == Tag.KW_WRITE)
            {
                WriteStmtNode stmt = new WriteStmtNode();
                node.AddChild(stmt);
                WriteStmt(stmt);
                Stmts(node);
            }
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
        }

        private void Move()
        {
            next = scanner.NextToken();
        }
    }
}
