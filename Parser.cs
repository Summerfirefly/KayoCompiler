using KayoCompiler.Ast;
using KayoCompiler.Errors;

namespace KayoCompiler
{
    partial class Parser
    {
        readonly Scanner scanner = null;
        Token next = null;

        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
            Move();
        }

        public ProgramNode StartParse()
        {
            ProgramNode p = new ProgramNode();
            Program(p);
            return p;
        }

        private void Program(ProgramNode node)
        {
            BlockNode block = new BlockNode();
            node.AddChild(block);

            Block(block);

            if (next != null)
            {
                new Error().PrintErrMsg();
            }
        }

        private void Block(BlockNode node)
        {
            if (next == null) return;

            if (next.Tag == Tag.DL_LBRACE)
            {
                Move();
            }
            else
            {
                new TokenMissingError(Tag.DL_LBRACE).PrintErrMsg();
            }

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
                new TokenMissingError(Tag.DL_RBRACE).PrintErrMsg();
            }
        }

        private void Decls(DeclsNode node)
        {
            switch (next?.Tag)
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
            switch (next?.Tag)
            {
                case Tag.KW_INT:
                case Tag.KW_BOOL:
                    node.type = next.Tag;
                    Move();

                    if (next?.Tag == Tag.ID)
                    {
                        node.name = next.Value;
                        Move();
                    }
                    else
                    {
                        new TokenMissingError(Tag.ID).PrintErrMsg();
                        break;
                    }

                    if (next.Tag == Tag.DL_SEM)
                    {
                        Move();
                    }
                    else
                    {
                        new TokenMissingError(Tag.DL_SEM).PrintErrMsg();
                    }

                    break;
            }
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
                    new Error().PrintErrMsg();
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
