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
                        switch (CodeGenUtils.StackDepth % 3)
                        {
                            case 0:
                                code += "or\tr10, r11\n";
                                break;
                            case 1:
                                code += "or\tr11, rax\n";
                                break;
                            case 2:
                                code += "or\trax, r10\n";
                                break;
                            default:
                                break;
                        }
                        if (CodeGenUtils.StackDepth > 3)
                        {
                            code += $"pop\t{CodeGenUtils.CurrentStackTop}\n";
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
                        switch (CodeGenUtils.StackDepth % 3)
                        {
                            case 0:
                                code += "and\tr10, r11\n";
                                break;
                            case 1:
                                code += "and\tr11, rax\n";
                                break;
                            case 2:
                                code += "and\trax, r10\n";
                                break;
                        }
                        if (CodeGenUtils.StackDepth > 3)
                        {
                            code += $"pop\t{CodeGenUtils.CurrentStackTop}\n";
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
                        switch (CodeGenUtils.StackDepth % 3)
                        {
                            case 0:
                                code += "cmp\tr10, r11\n";
                                break;
                            case 1:
                                code += "cmp\tr11, rax\n";
                                break;
                            case 2:
                                code += "cmp\trax, r10\n";
                                break;
                        }

                        if (CodeGenUtils.StackDepth > 3)
                        {
                            code += $"pop\t{CodeGenUtils.CurrentStackTop}\n";
                        }
                        CodeGenUtils.StackDepth--;

                        switch (node.Op)
                        {
                            case Tag.DL_EQ:
                                code += "sete\tbl\n";
                                break;
                            case Tag.DL_NEQ:
                                code += "setne\tbl\n";
                                break;
                        }

                        code += $"movzx\t{CodeGenUtils.CurrentStackTop}, bl\n";
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
                        if (SymbolTable.IsNumType(node.Type()) && SymbolTable.IsNumType(type))
                            type = VarType.TYPE_BOOL;
                        else
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
                        switch (CodeGenUtils.StackDepth % 3)
                        {
                            case 0:
                                code += "cmp\tr10, r11\n";
                                break;
                            case 1:
                                code += "cmp\tr11, rax\n";
                                break;
                            case 2:
                                code += "cmp\trax, r10\n";
                                break;
                        }

                        if (CodeGenUtils.StackDepth > 3)
                        {
                            code += $"pop\t{CodeGenUtils.CurrentStackTop}\n";
                        }
                        CodeGenUtils.StackDepth--;

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

                        code += $"movzx\t{CodeGenUtils.CurrentStackTop}, bl\n";
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
                        if (SymbolTable.IsNumType(node.Type()) && SymbolTable.IsNumType(type))
                            type = VarType.TYPE_BOOL;
                        else
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
                        switch (CodeGenUtils.StackDepth % 3)
                        {
                            case 0:
                                code += $"{opStr}\tr10, r11\n";
                                break;
                            case 1:
                                code += $"{opStr}\tr11, rax\n";
                                break;
                            case 2:
                                code += $"{opStr}\trax, r10\n";
                                break;
                        }

                        if (CodeGenUtils.StackDepth > 3)
                        {
                            code += $"pop\t{CodeGenUtils.CurrentStackTop}\n";
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
                        if (!SymbolTable.IsNumType(type))
                        {
                            type = VarType.TYPE_ERROR;
                            break;
                        }
                        else
                            type = SymbolTable.IsNumType(node.Type()) ? VarType.TYPE_LONG : VarType.TYPE_ERROR;
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
                            switch (CodeGenUtils.StackDepth % 3)
                            {
                                case 0:
                                    code += "imul\tr10, r11\n";
                                    break;
                                case 1:
                                    code += "imul\tr11, rax\n";
                                    break;
                                case 2:
                                    code += "imul\trax, r10\n";
                                    break;
                            }
                        }
                        else if (node.Op == Tag.DL_OBELUS)
                        {
                            switch (CodeGenUtils.StackDepth % 3)
                            {
                                case 0:
                                    code += "mov\trbx, rax\n";
                                    code += "mov\trax, r10\n";
                                    code += "cqo\n";
                                    code += "idiv\tr11\n";
                                    code += "mov\tr10, rax\n";
                                    code += "mov\trax, rbx\n";
                                    break;
                                case 1:
                                    code += "mov\trbx, rax\n";
                                    code += "mov\trax, r11\n";
                                    code += "cqo\n";
                                    code += "idiv\trbx\n";
                                    code += "mov\tr11, rax\n";
                                    break;
                                case 2:
                                    code += "cqo\n";
                                    code += "idiv\tr10\n";
                                    break;
                            }
                        }
                        else if (node.Op == Tag.DL_MOD)
                        {
                            switch (CodeGenUtils.StackDepth % 3)
                            {
                                case 0:
                                    code += "mov\trbx, rax\n";
                                    code += "mov\trax, r10\n";
                                    code += "cqo\n";
                                    code += "idiv\tr11\n";
                                    code += "mov\tr10, rdx\n";
                                    code += "mov\trax, rbx\n";
                                    break;
                                case 1:
                                    code += "mov\trbx, rax\n";
                                    code += "mov\trax, r11\n";
                                    code += "cqo\n";
                                    code += "idiv\trbx\n";
                                    code += "mov\tr11, rdx\n";
                                    break;
                                case 2:
                                    code += "cqo\n";
                                    code += "idiv\tr10\n";
                                    code += "mov\trax, rdx\n";
                                    break;
                            }
                        }

                        if (CodeGenUtils.StackDepth > 3)
                        {
                            code += $"pop\t{CodeGenUtils.CurrentStackTop}\n";
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
                        if (!SymbolTable.IsNumType(type))
                        {
                            type = VarType.TYPE_ERROR;
                            break;
                        }
                        else
                            type = SymbolTable.IsNumType(node.Type()) ? VarType.TYPE_LONG : VarType.TYPE_ERROR;
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
                    code += $"xor\t{CodeGenUtils.CurrentStackTop}, 1\n";
                }
                else if (factorOp == Tag.DL_MINUS)
                {
                    code += $"not\t{CodeGenUtils.CurrentStackTop}\n";
                    code += $"add\t{CodeGenUtils.CurrentStackTop}, 1\n";
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
                else if ((factorOp == Tag.DL_PLUS || factorOp == Tag.DL_MINUS) && !SymbolTable.IsNumType(type))
                    new TypeMismatchError(type, VarType.TYPE_INT).PrintErrMsg();
            }

            return type;
        }
    }
}
