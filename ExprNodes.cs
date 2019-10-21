using System;
using System.Collections.Generic;
using KayoCompiler.Errors;

namespace KayoCompiler.Ast
{
    class ExprBaseNode<T> : AstNode where T : AstNode
    {
        public Tag Op { get; set; }
        protected List<T> children;

        public virtual T AddChild(T child)
        {
            if (children == null)
                children = new List<T>();

            children.Add(child);
            return child;
        }

        public override string Gen()
        {
            throw new NotImplementedException();
        }

        public virtual VarType Type()
        {
            throw new NotImplementedException();
        }
    }

    class ExprNode : ExprBaseNode<AndExprNode>
    {
        public override string Gen()
        {
            string code = string.Empty;

            if (children != null)
            {
                foreach (var node in children)
                {
                    code += node.Gen();
                    if (node.Op != Tag.NULL)
                    {
                        switch (CodeGenUtils.StackDepth)
                        {
                            case 2:
                                code += "or\trax, rdx\n";
                                break;
                            case 3:
                                code += "or\trdx, rcx\n";
                                break;
                            case 4:
                                code += "pop\trbx";
                                code += "or\trcx, rbx\n";
                                break;
                            default:
                                code += "pop\trbx\n";
                                code += "or\tqword [rsp], rbx\n";
                                break;
                        }
                        CodeGenUtils.StackDepth--;
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_ERROR;
            int index = 0;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;

                    if (index == 0)
                    {
                        type = node.Type();
                    }
                    else
                    {
                        if (type != VarType.TYPE_BOOL)
                        {
                            type = VarType.TYPE_ERROR;
                            break;
                        }
                        else
                            type = node.Type() == VarType.TYPE_BOOL ? VarType.TYPE_BOOL : VarType.TYPE_ERROR;
                    }

                    index++;
                }
            }

            return type;
        }
    }

    class AndExprNode : ExprBaseNode<EqualExprNode>
    {
        public override string Gen()
        {
            string code = string.Empty;

            if (children != null)
            {
                foreach (var node in children)
                {
                    code += node.Gen();
                    if (node.Op != Tag.NULL)
                    {
                        switch (CodeGenUtils.StackDepth)
                        {
                            case 2:
                                code += "and\trax, rdx\n";
                                break;
                            case 3:
                                code += "and\trdx, rcx\n";
                                break;
                            case 4:
                                code += "pop\trbx";
                                code += "and\trcx, rbx\n";
                                break;
                            default:
                                code += "pop\trbx\n";
                                code += "and\tqword [rsp], rbx\n";
                                break;
                        }
                        CodeGenUtils.StackDepth--;
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_ERROR;
            int index = 0;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;

                    if (index == 0)
                    {
                        type = node.Type();
                    }
                    else
                    {
                        if (type != VarType.TYPE_BOOL)
                        {
                            type = VarType.TYPE_ERROR;
                            break;
                        }
                        else
                            type = node.Type() == VarType.TYPE_BOOL ? VarType.TYPE_BOOL : VarType.TYPE_ERROR;
                    }

                    index++;
                }
            }

            return type;
        }
    }

    class EqualExprNode : ExprBaseNode<CmpExprNode>
    {
        public override string Gen()
        {
            string code = string.Empty;

            if (children != null)
            {
                foreach (var node in children)
                {
                    code += node.Gen();
                    if (node.Op != Tag.NULL)
                    {
                        switch (CodeGenUtils.StackDepth)
                        {
                            case 2:
                                code += "cmp\trax, rdx\n";
                                break;
                            case 3:
                                code += "cmp\trdx, rcx\n";
                                break;
                            case 4:
                                code += "pop\trbx";
                                code += "cmp\trcx, rbx\n";
                                break;
                            default:
                                code += "pop\trbx\n";
                                code += "cmp\tqword [rsp], rbx\n";
                                break;
                        }

                        switch (node.Op)
                        {
                            case Tag.DL_EQ:
                                code += "sete\tbl\n";
                                break;
                            case Tag.DL_NEQ:
                                code += "setne\tbl\n";
                                break;
                        }

                        code += "movzx\trbx, bl\n";

                        switch (CodeGenUtils.StackDepth)
                        {
                            case 2:
                                code += "mov\trax, rbx\n";
                                break;
                            case 3:
                                code += "mov\trdx, rbx\n";
                                break;
                            case 4:
                                code += "mov\trcx, rbx\n";
                                break;
                            default:
                                code += "push\trbx\n";
                                break;
                        }

                        CodeGenUtils.StackDepth--;
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_ERROR;
            int index = 0;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;

                    if (index == 0)
                    {
                        type = node.Type();
                    }
                    else
                    {
                        type = node.Type() == type ? VarType.TYPE_BOOL : VarType.TYPE_ERROR;
                    }

                    index++;
                }
            }

            return type;
        }
    }

    class CmpExprNode : ExprBaseNode<AddExprNode>
    {
        public override string Gen()
        {
            string code = string.Empty;

            if (children != null)
            {
                foreach (var node in children)
                {
                    code += node.Gen();
                    if (node.Op != Tag.NULL)
                    {
                        switch (CodeGenUtils.StackDepth)
                        {
                            case 2:
                                code += "cmp\trax, rdx\n";
                                break;
                            case 3:
                                code += "cmp\trdx, rcx\n";
                                break;
                            case 4:
                                code += "pop\trbx";
                                code += "cmp\trcx, rbx\n";
                                break;
                            default:
                                code += "pop\trbx\n";
                                code += "cmp\tqword [rsp], rbx\n";
                                break;
                        }

                        switch (node.Op)
                        {
                            case Tag.DL_GT:
                                code += "setg\tbl\n";
                                break;
                            case Tag.DL_NGT:
                                code += "setng\tbl\n";
                                break;
                            case Tag.DL_LT:
                                code += "setl\tbl\n";
                                break;
                            case Tag.DL_NLT:
                                code += "setnl\tbl\n";
                                break;
                        }

                        code += "movzx\trbx, bl\n";

                        switch (CodeGenUtils.StackDepth)
                        {
                            case 2:
                                code += "mov\trax, rbx\n";
                                break;
                            case 3:
                                code += "mov\trdx, rbx\n";
                                break;
                            case 4:
                                code += "mov\trcx, rbx\n";
                                break;
                            default:
                                code += "push\trbx\n";
                                break;
                        }

                        CodeGenUtils.StackDepth--;
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_ERROR;
            int index = 0;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;

                    if (index == 0)
                    {
                        type = node.Type();
                    }
                    else
                    {
                        type = node.Type() == type ? VarType.TYPE_BOOL : VarType.TYPE_ERROR;
                    }

                    index++;
                }
            }

            return type;
        }
    }

    class AddExprNode : ExprBaseNode<MulExprNode>
    {
        public override string Gen()
        {
            string code = string.Empty;

            if (children != null)
            {
                foreach (var node in children)
                {
                    code += node.Gen();

                    if (node.Op != Tag.NULL)
                    {
                        string opStr = node.Op == Tag.DL_PLUS ? "add" : "sub";
                        switch (CodeGenUtils.StackDepth)
                        {
                            case 2:
                                code += $"{opStr}\trax, rdx\n";
                                break;
                            case 3:
                                code += $"{opStr}\trdx, rcx\n";
                                break;
                            case 4:
                                code += "pop\trbx\n";
                                code += $"{opStr}\trcx, rbx\n";
                                break;
                            default:
                                code += "pop\trbx\n";
                                code += $"{opStr}\tqword [rsp], rbx\n";
                                break;
                        }

                        CodeGenUtils.StackDepth--;
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_ERROR;
            int index = 0;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;

                    if (index == 0)
                    {
                        type = node.Type();
                    }
                    else
                    {
                        if (type != VarType.TYPE_INT)
                        {
                            type = VarType.TYPE_ERROR;
                            break;
                        }
                        else
                            type = node.Type() == VarType.TYPE_INT ? VarType.TYPE_INT : VarType.TYPE_ERROR;
                    }

                    index++;
                }
            }

            return type;
        }
    }

    class MulExprNode : ExprBaseNode<FactorNode>
    {
        public override string Gen()
        {
            string code = string.Empty;

            if (children != null)
            {
                foreach (var node in children)
                {
                    code += node.Gen();
                    if (node.Op != Tag.NULL)
                    {
                        if (node.Op == Tag.DL_MULTI)
                        {
                            switch (CodeGenUtils.StackDepth)
                            {
                                case 2:
                                    code += "imul\trax, rdx\n";
                                    break;
                                case 3:
                                    code += "imul\trdx, rcx\n";
                                    break;
                                case 4:
                                    code += "pop\trbx\n";
                                    code += "imul\trcx, rbx\n";
                                    break;
                                default:
                                    code += "pop\trbx\n";
                                    code += "imul\trbx, qword [rsp]\n";
                                    code += "mov\trbx, qword [rsp]\n";
                                    break;
                            }
                        }
                        else if (node.Op == Tag.DL_OBELUS)
                        {
                            switch (CodeGenUtils.StackDepth)
                            {
                                case 2:
                                    code += "mov\trbx, rdx\n";
                                    code += "cqo\n";
                                    code += "idiv\trbx\n";
                                    break;
                                case 3:
                                    code += "push\trax\n";
                                    code += "mov\trax, rdx\n";
                                    code += "cqo\n";
                                    code += "idiv\trcx\n";
                                    code += "mov\trdx, rax\n";
                                    code += "pop\trax\n";
                                    break;
                                case 4:
                                    code += "pop\trbx\n";
                                    code += "push\trax\n";
                                    code += "push\trdx\n";
                                    code += "mov\trax, rcx\n";
                                    code += "cqo\n";
                                    code += "idiv\trbx";
                                    code += "mov\trcx, rax\n";
                                    code += "pop\trdx\n";
                                    code += "pop\trax\n";
                                    break;
                                default:
                                    code += "pop\trbx\n";
                                    code += "push\trax\n";
                                    code += "push\trdx\n";
                                    code += "mov\trax, qword [rsp+16]\n";
                                    code += "cqo\n";
                                    code += "idiv\trbx\n";
                                    code += "mov\trbx, rax\n";
                                    code += "pop\trdx\n";
                                    code += "pop\trax\n";
                                    code += "mov\tqword [rsp], rbx\n";
                                    break;
                            }
                        }
                        else if (node.Op == Tag.DL_MOD)
                        {
                            switch (CodeGenUtils.StackDepth)
                            {
                                case 2:
                                    code += "mov\trbx, rdx\n";
                                    code += "cqo\n";
                                    code += "idiv\trbx\n";
                                    code += "mov\trax, rdx\n";
                                    break;
                                case 3:
                                    code += "push\trax\n";
                                    code += "mov\trax, rdx\n";
                                    code += "cqo\n";
                                    code += "idiv\trcx\n";
                                    code += "pop\trax\n";
                                    break;
                                case 4:
                                    code += "pop\trbx\n";
                                    code += "push\trax\n";
                                    code += "push\trdx\n";
                                    code += "mov\trax, rcx\n";
                                    code += "cqo\n";
                                    code += "idiv\trbx";
                                    code += "mov\trcx, rdx\n";
                                    code += "pop\trdx\n";
                                    code += "pop\trax\n";
                                    break;
                                default:
                                    code += "pop\trbx\n";
                                    code += "push\trax\n";
                                    code += "push\trdx\n";
                                    code += "mov\trax, qword [rsp+16]\n";
                                    code += "cqo\n";
                                    code += "idiv\trbx\n";
                                    code += "mov\trbx, rdx\n";
                                    code += "pop\trdx\n";
                                    code += "pop\trax\n";
                                    code += "mov\tqword [rsp], rbx\n";
                                    break;
                            }
                        }

                        CodeGenUtils.StackDepth--;
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_ERROR;
            int index = 0;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;

                    if (index == 0)
                    {
                        type = node.Type();
                    }
                    else
                    {
                        if (type != VarType.TYPE_INT)
                        {
                            type = VarType.TYPE_ERROR;
                            break;
                        }
                        else
                            type = node.Type() == VarType.TYPE_INT ? VarType.TYPE_INT : VarType.TYPE_ERROR;
                    }

                    index++;
                }
            }

            return type;
        }
    }

    class FactorNode : ExprBaseNode<AstNode>
    {
        public Tag factorOp;
        public TerminalNode value;
        public ExprNode expr;
        public FactorNode factor;
        public FuncCallStmtNode func;

        public override string Gen()
        {
            string code = string.Empty;
            code += value?.Gen() ?? string.Empty;
            code += expr?.Gen() ?? string.Empty;
            code += factor?.Gen() ?? string.Empty;
            code += func?.Gen() ?? string.Empty;

            if (factor != null)
            {
                if (factorOp == Tag.DL_NOT)
                {
                    switch (CodeGenUtils.StackDepth)
                    {
                        case 1:
                            code += "xor\trax, 1\n";
                            break;
                        case 2:
                            code += "xor\trdx, 1\n";
                            break;
                        case 3:
                            code += "xor\trcx, 1\n";
                            break;
                        default:
                            code += "xor\tqword [rsp], 1\n";
                            break;
                    }
                }
                else if (factorOp == Tag.DL_MINUS)
                {
                    switch (CodeGenUtils.StackDepth)
                    {
                        case 1:
                            code += "not\trax\n";
                            code += "add\trax, 1\n";
                            break;
                        case 2:
                            code += "not\trdx\n";
                            code += "add\trdx, 1\n";
                            break;
                        case 3:
                            code += "not\trcx\n";
                            code += "add\trcx, 1\n";
                            break;
                        default:
                            code += "not\tqword [rsp]\n";
                            code += "add\tqword [rsp], 1\n";
                            break;
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = value?.Type() ?? expr?.Type() ?? factor?.Type() ?? func?.Type() ?? VarType.TYPE_ERROR;
            if (factor != null)
            {
                if (factorOp == Tag.DL_NOT && type != VarType.TYPE_BOOL)
                    new TypeMismatchError(type, VarType.TYPE_BOOL).PrintErrMsg();
                else if (type != VarType.TYPE_INT)
                    new TypeMismatchError(type, VarType.TYPE_INT).PrintErrMsg();
            }

            return type;
        }
    }
}
