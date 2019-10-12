using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
    internal partial class Parser
    {
        private void IfStmt(IfStmtNode node)
        {
            node.body = new StmtNode();
            Move();

            if (next?.Tag == Tag.DL_LPAR)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_LPAR).PrintErrMsg();
            }

            Expr(ref node.condition);

            if (node.condition == null)
            {
                new Error().PrintErrMsg();
            }
            else if (node.condition.Type() != VarType.TYPE_BOOL)
            {
                new Error().PrintErrMsg();
            }

            if (next?.Tag == Tag.DL_RPAR)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_RPAR).PrintErrMsg();
            }

            Stmt(node.body);

            if (next?.Tag == Tag.KW_ELSE)
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
            node.id = new IdNode(next.Value);
            Move();

            if (next?.Tag == Tag.DL_SET)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_SET).PrintErrMsg();
            }
            
            Expr(ref node.expr);

            if (node.expr == null)
            {
                new Error().PrintErrMsg();
            }
            else if (node.id.Type() == VarType.TYPE_ERROR)
            {
                new Error().PrintErrMsg();
            }
            else if (node.id.Type() != node.expr.Type())
            {
                new TypeMismatchError(node.id.Type(), node.expr.Type()).PrintErrMsg();
            }

            if (next?.Tag == Tag.DL_SEM)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_SEM).PrintErrMsg();
            }
        }

        private void WhileStmt(WhileStmtNode node)
        {
            node.body = new StmtNode();
            Move();

            if (next?.Tag == Tag.DL_LPAR)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_LPAR).PrintErrMsg();
            }

            Expr(ref node.condition);

            if (node.condition == null)
            {
                new Error().PrintErrMsg();
            }
            else if (node.condition.Type() != VarType.TYPE_BOOL)
            {
                new Error().PrintErrMsg();
            }

            if (next?.Tag == Tag.DL_RPAR)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_RPAR).PrintErrMsg();
            }

            Stmt(node.body);
        }

        private void ForStmt(ForStmtNode node)
        {
            Move();

            if (next?.Tag == Tag.DL_LPAR)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_LPAR).PrintErrMsg();
            }

            node.preStmt = new StmtNode();
            Stmt(node.preStmt);

            Expr(ref node.condition);

            if (node.condition == null)
            {
                new Error().PrintErrMsg();
            }
            else if (node.condition.Type() != VarType.TYPE_BOOL)
            {
                new Error().PrintErrMsg();
            }

            if (next?.Tag == Tag.DL_SEM)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_SEM).PrintErrMsg();
            }

            node.loopStmt = new StmtNode();
            Stmt(node.loopStmt);

            if (next?.Tag == Tag.DL_RPAR)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_RPAR).PrintErrMsg();
            }

            node.body = new StmtNode();
            Stmt(node.body);
        }

        private void WriteStmt(WriteStmtNode node)
        {
            CodeGenUtils.HasWrite = true;
            Move();
            Expr(ref node.expr);

            if (next?.Tag == Tag.DL_SEM)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_SEM).PrintErrMsg();
            }
        }

        private void ReadStmt(ReadStmtNode node)
        {
            Move();

            if (next?.Tag == Tag.ID)
            {
                node.id = new IdNode(next.Value);
                Move();

                if (node.id.Type() == VarType.TYPE_ERROR)
                {
                    new Error().PrintErrMsg();
                }
            }
            else
            {
                new TokenMissingError(Tag.ID).PrintErrMsg();
            }

            if (next.Tag == Tag.DL_SEM)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_SEM).PrintErrMsg();
            }
        }
    }
}
