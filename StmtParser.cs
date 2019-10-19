using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
    internal partial class Parser
    {
        private void IfStmt(IfStmtNode node)
        {
            ScopeManager.ScopeEnter();
            node.body = new StmtNode();
            Move();

            RequireTag(Tag.DL_LPAR);

            Expr(ref node.condition);

            if (node.condition == null)
            {
                new Error().PrintErrMsg();
            }
            else if (node.condition.Type() != VarType.TYPE_BOOL)
            {
                new TypeMismatchError(VarType.TYPE_BOOL, node.condition.Type()).PrintErrMsg();
            }

            RequireTag(Tag.DL_RPAR);

            Stmt(node.body);
            ScopeManager.ScopeLeave();

            if (next.Tag == Tag.KW_ELSE)
            {
                node.elseStmt = new ElseStmtNode();
                ElseStmt(node.elseStmt);
            }
        }

        private void ElseStmt(ElseStmtNode node)
        {
            ScopeManager.ScopeEnter();
            node.body = new StmtNode();
            Move();

            Stmt(node.body);
            ScopeManager.ScopeLeave();
        }

        private void SetStmt(SetStmtNode node)
        {
            node.id = new IdNode(next.Value);
            Move();

            RequireTag(Tag.DL_SET);
            
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

            RequireTag(Tag.DL_SEM);
        }

        private void WhileStmt(WhileStmtNode node)
        {
            ScopeManager.ScopeEnter();
            node.body = new StmtNode();
            Move();

            RequireTag(Tag.DL_LPAR);

            Expr(ref node.condition);

            if (node.condition == null)
            {
                new Error().PrintErrMsg();
            }
            else if (node.condition.Type() != VarType.TYPE_BOOL)
            {
                new TypeMismatchError(VarType.TYPE_BOOL, node.condition.Type()).PrintErrMsg();
            }

            RequireTag(Tag.DL_RPAR);

            Stmt(node.body);
            ScopeManager.ScopeLeave();
        }

        private void ForStmt(ForStmtNode node)
        {
            ScopeManager.ScopeEnter();
            Move();

            RequireTag(Tag.DL_LPAR);

            node.preStmt = new StmtNode();
            Stmt(node.preStmt);

            Expr(ref node.condition);

            if (node.condition == null)
            {
                new Error().PrintErrMsg();
            }
            else if (node.condition.Type() != VarType.TYPE_BOOL)
            {
                new TypeMismatchError(VarType.TYPE_BOOL, node.condition.Type()).PrintErrMsg();
            }

            RequireTag(Tag.DL_SEM);

            node.loopStmt = new StmtNode();
            Stmt(node.loopStmt);

            RequireTag(Tag.DL_RPAR);

            node.body = new StmtNode();
            Stmt(node.body);
            ScopeManager.ScopeLeave();
        }

        private void WriteStmt(WriteStmtNode node)
        {
            CodeGenUtils.HasWrite = true;
            Move();
            Expr(ref node.expr);

            RequireTag(Tag.DL_SEM);
        }

        private void ReadStmt(ReadStmtNode node)
        {
            Move();

            if (next.Tag == Tag.ID)
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

            RequireTag(Tag.DL_SEM);
        }

        private void ReturnStmt(ReturnStmtNode node)
        {
            Move();

            if (next.Tag != Tag.DL_SEM)
            {
                Expr(ref node.expr);

                if (node.expr?.Type() != SymbolTable.FindFun(ScopeManager.CurrentFun).returnType)
                {
                    new Error().PrintErrMsg();
                }
            }

            RequireTag(Tag.DL_SEM);
        }
    }
}
