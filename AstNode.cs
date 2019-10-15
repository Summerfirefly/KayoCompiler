using System.Collections.Generic;

namespace KayoCompiler.Ast
{
    abstract class AstNode
    {
        public abstract string Gen();
    }

    class ProgramNode : AstNode
    {
        internal List<FunctionNode> children;

        public override string Gen()
        {
            if (children == null) return string.Empty;

            string code = string.Empty;

            foreach (var child in children)
            {
                code += child?.Gen() ?? string.Empty;
            }

            return code;
        }

        public void AddChild(FunctionNode child)
        {
            if (children == null)
                children = new List<FunctionNode>();

            children.Add(child);
        }
    }

    class FunctionNode : AstNode
    {
        internal string name;
        internal BlockNode body;

        public override string Gen()
        {
            string code = string.Empty;
            ScopeManager.FunctionEnter(name);
            ScopeManager.ScopeEnter();

            code += $"{name}:\n";
            code += "push\trbp\n";
            code += "mov\trbp, rsp\n";
            code += $"sub\trsp, {SymbolTable.CurFunVarCount * 8}\n";

            code += body?.Gen();

            code += "mov\trsp, rbp\n";
            code += "pop\trbp\n";
            code += "ret\n";

            ScopeManager.ScopeLeave();
            ScopeManager.FunctionLeave();

            return code;
        }
    }

    class BlockNode : AstNode
    {
        internal List<AstNode> children;

        public override string Gen()
        {
            if (children == null) return string.Empty;

            string code = string.Empty;

            foreach (var child in children)
            {
                code += child?.Gen() ?? string.Empty;
            }

            return code;
        }

        public void AddChild(AstNode child)
        {
            if (children == null)
                children = new List<AstNode>();

            children.Add(child);
        }
    }

    class DeclsNode : AstNode
    {
        internal List<DeclNode> decls;

        public override string Gen()
        {
            if (decls == null) return string.Empty;

            string code = string.Empty;

            foreach (var child in decls)
            {
                code += child?.Gen() ?? string.Empty;
            }

            return code;
        }

        public void AddChild(DeclNode decl)
        {
            if (decls == null)
                decls = new List<DeclNode>();

            decls.Add(decl);
        }
    }

    class DeclNode : AstNode
    {
        public Tag type;
        public string name;

        public override string Gen()
        {
            return string.Empty;
        }
    }

    class StmtsNode : AstNode
    {
        internal List<AstNode> children;

        public override string Gen()
        {
            if (children == null) return string.Empty;

            string code = string.Empty;

            foreach (var child in children)
            {
                code += child?.Gen() ?? string.Empty;
            }

            return code;
        }

        public void AddChild(AstNode child)
        {
            if (children == null)
                children = new List<AstNode>();

            children.Add(child);
        }
    }

    class StmtNode : AstNode
    {
        public AstNode stmt;

        public override string Gen()
        {
            return stmt?.Gen() ?? string.Empty;
        }
    }
}
