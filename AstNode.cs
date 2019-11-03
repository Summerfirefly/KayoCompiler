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

            if (body != null)
            {
                code += $"func_{name}:\n";
                code += "push\trbp\n";
                code += "mov\trbp, rsp\n";
                code += $"sub\trsp, {SymbolTable.CurFunVarSize}\n";

                code += body.Gen();

                code += $"func_{name}_return:\n";
                code += "mov\trsp, rbp\n";
                code += "pop\trbp\n";
                code += "ret\n";
            }

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

    class DeclNode : AstNode
    {
        public VarType type;
        public string name;
        public ExprNode init;
        public ExprNode size;

        public override string Gen()
        {
            string code = string.Empty;
            int offset = -SymbolTable.GetVarOffset(name);

            CodeGenUtils.StackDepth = 0;
            if (init != null)
            {
                code += init.Gen();
                switch (Utils.SizeOf(type))
                {
                    case 1:
                        code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], al\n";
                        break;
                    case 4:
                        code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], eax\n";
                        break;
                    case 8:
                        code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], rax\n";
                        break;
                }
            }
            else if (size != null)
            {
                int ptrSize = Utils.SizeOf(VarType.TYPE_PTR);
                code += $"lea\trbx, [rbp{(offset+ptrSize>0?"+":"")}{offset+ptrSize}]\n";
                code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], rbx\n";
            }

            return code;
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
