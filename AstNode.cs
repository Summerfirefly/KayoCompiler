using System;
using System.Collections.Generic;

namespace KayoCompiler.Ast
{
    abstract class AstNode
    {
        public abstract void Gen();
    }

    internal static class CodeGenData
    {
        internal static int labelNum = 1;
    }

    class ProgramNode : AstNode
    {
        List<BlockNode> children;

        public override void Gen()
        {
            if (children == null) return;

            foreach (var child in children)
            {
                child?.Gen();
            }
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
        List<AstNode> children;

        public override void Gen()
        {
            if (children == null) return;

            foreach (var child in children)
            {
                child?.Gen();
            }
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
        List<DeclNode> decls;

        public override void Gen()
        {
            if (decls == null) return;

            foreach (var decl in decls)
            {
                decl?.Gen();
            }
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

        public override void Gen()
        {
            Console.WriteLine($".{type} {name}");
        }
    }

    class StmtsNode : AstNode
    {
        List<AstNode> children;

        public override void Gen()
        {
            if (children == null) return;

            foreach (var child in children)
            {
                child?.Gen();
            }
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

        public override void Gen()
        {
            stmt?.Gen();
        }
    }
}
