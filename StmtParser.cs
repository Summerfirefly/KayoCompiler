using KayoCompiler.Ast;

namespace KayoCompiler
{
    partial class Parser
    {
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
    }
}
