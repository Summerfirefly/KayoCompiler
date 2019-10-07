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
            if (next == null) return;

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

        private void IfStmt(IfStmtNode node)
        {
            if (next == null) return;
            node.body = new StmtNode();
            Move();

            if (next.Tag == Tag.DL_LPAR)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();

            Expr(ref node.condition);

            if (next.Tag == Tag.DL_RPAR)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();

            Stmt(node.body);

            if (next.Tag == Tag.KW_ELSE)
            {
                node.elseStmt = new ElseStmtNode();
                ElseStmt(node.elseStmt);
            }
        }

        private void ElseStmt(ElseStmtNode node)
        {
            node.body = new StmtNode();
            Move();

            Stmt(node.body);
        }

        private void SetStmt(SetStmtNode node)
        {
            if (next == null) return;
            node.id = next.Value;
            Move();

            if (next.Tag == Tag.DL_SET)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();

            Expr(ref node.expr);

            if (next.Tag == Tag.DL_SEM)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();
        }

        private void WhileStmt(WhileStmtNode node)
        {
            if (next == null) return;
            node.body = new StmtNode();
            Move();

            if (next.Tag == Tag.DL_LPAR)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();

            Expr(ref node.condition);

            if (next.Tag == Tag.DL_RPAR)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();

            Stmt(node.body);
        }

        private void WriteStmt(WriteStmtNode node)
        {
            if (next == null) return;
            Move();
            Expr(ref node.expr);

            if (next.Tag == Tag.DL_SEM)
                Move();
            else
                new Error(scanner.LineNum).PrintErrMsg();
        }

        private void ReadStmt(ReadStmtNode node)
        {
            if (next == null) return;
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

        private void Expr(ref ExprNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new ExprNode();
                    LogicExpr(ref node.expr);
                    break;
            }
        }

        private void LogicExpr(ref LogicExprNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new LogicExprNode();
                    LogicTerm(ref node.term);
                    LogicExprTail(ref node.tail);
                    break;
            }
        }

        private void LogicExprTail(ref LogicExprTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_OR:
                    node = new LogicExprTailNode();
                    Move();
                    LogicTerm(ref node.term);
                    LogicExprTail(ref node.tail);
                    break;
            }
        }

        private void LogicTerm(ref LogicTermNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new LogicTermNode();
                    LogicFactor(ref node.factor);
                    LogicTermTail(ref node.tail);
                    break;
            }
        }

        private void LogicTermTail(ref LogicTermTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_AND:
                    node = new LogicTermTailNode();
                    Move();
                    LogicFactor(ref node.factor);
                    LogicTermTail(ref node.tail);
                    break;
            }
        }

        private void LogicFactor(ref LogicFactorNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new LogicFactorNode();
                    LogicRel(ref node.rel);
                    LogicFactorTail(ref node.tail);
                    break;
            }
        }

        private void LogicFactorTail(ref LogicFactorTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_EQ:
                case Tag.DL_NEQ:
                    node = new LogicFactorTailNode
                    {
                        op = next.Tag
                    };
                    Move();
                    LogicRel(ref node.rel);
                    LogicFactorTail(ref node.tail);
                    break;
            }
        }

        private void LogicRel(ref LogicRelNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new LogicRelNode();
                    MathExpr(ref node.expr);
                    LogicRelTail(ref node.tail);
                    break;
            }
        }

        private void LogicRelTail(ref LogicRelTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_LT:
                case Tag.DL_NLT:
                case Tag.DL_GT:
                case Tag.DL_NGT:
                    node = new LogicRelTailNode
                    {
                        op = next.Tag
                    };
                    Move();
                    MathExpr(ref node.expr);
                    LogicRelTail(ref node.tail);
                    break;
            }
        }

        private void MathExpr(ref MathExprNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new MathExprNode();
                    MathTerm(ref node.term);
                    MathExprTail(ref node.tail);
                    break;
            }
        }

        private void MathExprTail(ref MathExprTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_PLUS:
                case Tag.DL_MINUS:
                    node = new MathExprTailNode
                    {
                        op = next.Tag
                    };
                    Move();
                    MathTerm(ref node.term);
                    MathExprTail(ref node.tail);
                    break;
            }
        }

        private void MathTerm(ref MathTermNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new MathTermNode();
                    MathFactor(ref node.factor);
                    MathTermTail(ref node.tail);
                    break;
            }
        }

        private void MathTermTail(ref MathTermTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_MULTI:
                case Tag.DL_OBELUS:
                    node = new MathTermTailNode
                    {
                        op = next.Tag
                    };
                    Move();
                    MathFactor(ref node.factor);
                    MathTermTail(ref node.tail);
                    break;
            }
        }

        private void MathFactor(ref MathFactorNode node)
        {
            node = new MathFactorNode();

            switch (next?.Tag)
            {
                case Tag.ID:
                    node.value = new IdNode(next.Value);
                    Move();
                    break;
                case Tag.NUM:
                    node.value = new IntNode(int.Parse(next.Value));
                    Move();
                    break;
                case Tag.DL_LPAR:
                    Move();
                    Expr(ref node.expr);
                    if (next.Tag == Tag.DL_RPAR)
                        Move();
                    else
                        new Error(scanner.LineNum).PrintErrMsg();
                    break;
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                    node.value = new BoolNode(bool.Parse(next.Value));
                    Move();
                    break;
                case Tag.DL_NOT:
                    Move();
                    MathFactor(ref node.factor);
                    break;
                default:
                    node = null;
                    break;
            }
        }

        private void Move()
        {
            next = scanner.NextToken();
        }
    }
}
