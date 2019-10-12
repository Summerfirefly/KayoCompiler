using System.Collections.Generic;

namespace KayoCompiler.Ast
{
    abstract class AstNode
    {
        public abstract string Gen();
    }

    class ProgramNode : AstNode
    {
        internal List<BlockNode> children;

        public override string Gen()
        {
            if (children == null) return string.Empty;

            string code = string.Empty;

            code += "GLOBAL _start\n";
            code += "EXTERN puts\n";
            code += "SECTION .text\n";
            code += "_start:\n";
            code += "call\t_entry\n";
            code += "mov\trax, 60\n";
            code += "mov\trdi, 0\n";
            code += "syscall\n";

            code += "_entry:\n";
            code += "push\trbp\n";
            code += "mov\trbp, rsp\n";
            code += $"sub\trsp, {SymbolTable.VarCount * 8}\n";

            foreach (var child in children)
            {
                code += child?.Gen() ?? string.Empty;
            }

            code += "mov\trsp, rbp\n";
            code += "pop\trbp\n";
            code += "ret\n";

            return code;
        }

        public void AddChild(BlockNode child)
        {
            if (children == null)
                children = new List<BlockNode>();

            children.Add(child);
        }
    }

    class BlockNode : AstNode
    {
        internal List<AstNode> children;

        public override string Gen()
        {
            if (children == null) return string.Empty;

            string code = string.Empty;
            CodeGenData.CurrentField++;

            foreach (var child in children)
            {
                code += child?.Gen() ?? string.Empty;
            }

            CodeGenData.CurrentField--;
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
