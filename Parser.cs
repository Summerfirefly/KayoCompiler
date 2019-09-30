using System;
using System.Collections.Generic;
using System.Text;
using KayoCompiler.Ast;

namespace KayoCompiler
{
    class Parser
    {
        readonly Scanner scanner = null;
        readonly ExprNode p = new ExprNode();
        Token next = null;

        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
            Move();
        }

        public void StartParse()
        {
            if (next == null) return;

            Expr(p);
            p.Gen();
        }

        private void Program()
        {
            // TODO
        }

        private void Expr(ExprNode node)
        {
            if (next == null) return;
            node.term = new TermNode();
            node.expr = new ExprTail();

            Term(node.term);
            Expr2(node.expr);
        }

        private void Expr2(ExprTail node)
        {
            if (next == null) return;
            node.term = new TermNode();
            node.expr = new ExprTail();

            if (next.Tag == Tag.DL_PLUS)
            {
                Move();
                node.op = "+";
                Term(node.term);
                Expr2(node.expr);
            }
            else if (next.Tag == Tag.DL_MINUS)
            {
                Move();
                node.op = "-";
                Term(node.term);
                Expr2(node.expr);
            }
        }

        private void Term(TermNode node)
        {
            if (next == null) return;

            node.factor = new FactorNode();
            node.term = new TermTail();

            Factor(node.factor);
            Term2(node.term);
        }

        private void Term2(TermTail node)
        {
            if (next == null) return;

            node.factor = new FactorNode();
            node.term = new TermTail();

            if (next.Tag == Tag.DL_MULTI)
            {
                Move();
                node.op = "*";
                Factor(node.factor);
                Term2(node.term);
            }
            else if (next.Tag == Tag.DL_OBELUS)
            {
                Move();
                node.op = "/";
                Factor(node.factor);
                Term2(node.term);
            }
        }

        private void Factor(FactorNode node)
        {
            if (next == null) return;

            if (next.Tag == Tag.NUM)
            {
                node.value = next.Value;
                Move();
            }
        }

        private void Move()
        {
            next = scanner.NextToken();
        }
    }
}
