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

        public virtual bool IsConstant()
        {
            throw new NotImplementedException();
        }

        public virtual TerminalNode Val()
        {
            throw new NotImplementedException();
        }
    }

    class ExprNode : ExprBaseNode<AndExprNode>
    {
        public AssignmentExprNode assignment;

        public override string Gen()
        {
            if (this.IsConstant() && assignment == null)
                return this.Val().Gen();
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
                            code += $"pop\t{CodeGenUtils.CurrentStackTop64}\n";
                        }
                        CodeGenUtils.StackDepth--;
                    }
                }
            }
            else if (assignment != null)
            {
                code += assignment.Gen();
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_NULL;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;
                    type = Utils.TypeCalc(type, node.Type(), true);
                }
            }
            else if (assignment != null)
            {
                type = assignment.Type();
            }

            return type == VarType.TYPE_NULL ? VarType.TYPE_ERROR : type;
        }

        public override bool IsConstant()
        {
            if (children == null)
                return false;
            if (assignment != null)
                return assignment.IsConstant();
            foreach (var child in children)
            {
                if (!child.IsConstant())
                    return false;
            }
            return true;
        }

        public override TerminalNode Val()
        {
            if (!this.IsConstant()) return null;
            if (assignment != null)
                return assignment.Val();
            TerminalNode result = new BoolNode(false);
            foreach (var node in children)
            {
                if (node.Op != Tag.NULL)
                {
                    result = new BoolNode(((result as BoolNode).value | (node.Val() as BoolNode).value) == 1);
                }
                else
                {
                    result = node.Val();
                }
            }
            return result;
        }
    }

    class AssignmentExprNode : ExprBaseNode<AstNode>
    {
        public UnaryNode leftV;
        public ExprNode expr;

        public override string Gen()
        {
            string code = string.Empty;
            int offset = -SymbolTable.GetVarOffset((leftV.factor.value as IdNode).name);
            if (this.IsConstant())
                code += this.Val().Gen();
            else
                code += expr?.Gen() ?? string.Empty;

            if (leftV.factor.indexer == null)
            {
                switch (Utils.SizeOf(leftV.Type()))
                {
                    case 1:
                        code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], {CodeGenUtils.CurrentStackTop8}\n";
                        break;
                    case 4:
                        code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], {CodeGenUtils.CurrentStackTop32}\n";
                        break;
                    case 8:
                        code += $"mov\t[rbp{(offset>0?"+":"")}{offset}], {CodeGenUtils.CurrentStackTop64}\n";
                        break;
                }
            }
            else
            {
                int eleSize = SymbolTable.FindVar((leftV.factor.value as IdNode).name).eleSize;
                code += leftV.factor.indexer.Gen();
                code += $"mov\trbx, [rbp{(offset>0?"+":"")}{offset}]\n";
                code += $"lea\trbx, [rbx+{CodeGenUtils.CurrentStackTop64}*{eleSize}]\n";
                if (CodeGenUtils.StackDepth > 3)
                {
                    code += $"pop\t{CodeGenUtils.CurrentStackTop64}\n";
                }
                CodeGenUtils.StackDepth--;
                switch (eleSize)
                {
                    case 1:
                        code += $"mov\t[rbx], {CodeGenUtils.CurrentStackTop8}\n";
                        break;
                    case 4:
                        code += $"mov\t[rbx], {CodeGenUtils.CurrentStackTop32}\n";
                        break;
                    case 8:
                        code += $"mov\t[rbx], {CodeGenUtils.CurrentStackTop64}\n";
                        break;
                }
            }

            return code;
        }

        public override VarType Type()
        {
            return expr.Type();
        }

        public override bool IsConstant()
        {
            return expr.IsConstant();
        }

        public override TerminalNode Val()
        {
            return expr.Val();
        }
    }

    class AndExprNode : ExprBaseNode<EqualExprNode>
    {
        public override string Gen()
        {
            if (this.IsConstant())
                return this.Val().Gen();
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
                            code += $"pop\t{CodeGenUtils.CurrentStackTop64}\n";
                        }
                        CodeGenUtils.StackDepth--;
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_NULL;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;
                    type = Utils.TypeCalc(type, node.Type(), true);
                }
            }

            return type == VarType.TYPE_NULL ? VarType.TYPE_ERROR : type;
        }

        public override bool IsConstant()
        {
            if (children == null)
                return false;
            foreach (var child in children)
            {
                if (!child.IsConstant())
                    return false;
            }
            return true;
        }

        public override TerminalNode Val()
        {
            if (!this.IsConstant()) return null;
            TerminalNode result = new BoolNode(false);
            foreach (var node in children)
            {
                if (node.Op != Tag.NULL)
                {
                    result = new BoolNode(((result as BoolNode).value & (node.Val() as BoolNode).value) == 1);
                }
                else
                {
                    result = node.Val();
                }
            }
            return result;
        }
    }

    class EqualExprNode : ExprBaseNode<CmpExprNode>
    {
        public override string Gen()
        {
            if (this.IsConstant())
                return this.Val().Gen();
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
                            code += $"pop\t{CodeGenUtils.CurrentStackTop64}\n";
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

                        code += $"movzx\t{CodeGenUtils.CurrentStackTop64}, bl\n";
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_NULL;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;
                    type = Utils.TypeCmp(type, node.Type(), true);
                }
            }

            return type == VarType.TYPE_NULL ? VarType.TYPE_ERROR : type;
        }

        public override bool IsConstant()
        {
            if (children == null)
                return false;
            foreach (var child in children)
            {
                if (!child.IsConstant())
                    return false;
            }
            return true;
        }

        public override TerminalNode Val()
        {
            if (!this.IsConstant()) return null;
            TerminalNode result = new BoolNode(false);
            foreach (var node in children)
            {
                if (node.Op != Tag.NULL)
                {
                    if (node.Op == Tag.DL_EQ)
                    {
                        if (node.Type() == VarType.TYPE_BOOL)
                            result = new BoolNode(((result as BoolNode).value == (node.Val() as BoolNode).value));
                        else
                            result = new BoolNode(((result as IntNode).value == (node.Val() as IntNode).value));
                    }
                    else if (node.Op == Tag.DL_NEQ)
                    {
                        if (node.Type() == VarType.TYPE_BOOL)
                            result = new BoolNode(((result as BoolNode).value != (node.Val() as BoolNode).value));
                        else
                            result = new BoolNode(((result as IntNode).value != (node.Val() as IntNode).value));
                    }
                }
                else
                {
                    result = node.Val();
                }
            }
            return result;
        }
    }

    class CmpExprNode : ExprBaseNode<AddExprNode>
    {
        public override string Gen()
        {
            if (this.IsConstant())
                return this.Val().Gen();
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
                            code += $"pop\t{CodeGenUtils.CurrentStackTop64}\n";
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

                        code += $"movzx\t{CodeGenUtils.CurrentStackTop64}, bl\n";
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_NULL;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;
                    type = Utils.TypeCmp(type, node.Type(), false);
                }
            }

            return type == VarType.TYPE_NULL ? VarType.TYPE_ERROR : type;
        }

        public override bool IsConstant()
        {
            if (children == null)
                return false;
            foreach (var child in children)
            {
                if (!child.IsConstant())
                    return false;
            }
            return true;
        }

        public override TerminalNode Val()
        {
            if (!this.IsConstant()) return null;
            TerminalNode result = new BoolNode(false);
            foreach (var node in children)
            {
                if (node.Op != Tag.NULL)
                {
                    if (node.Op == Tag.DL_LT)
                    {
                        if (Utils.IsNumType(node.Type()))
                            result = new BoolNode(((result as IntNode).value < (node.Val() as IntNode).value));
                    }
                    else if (node.Op == Tag.DL_NLT)
                    {
                        if (Utils.IsNumType(node.Type()))
                            result = new BoolNode(((result as IntNode).value >= (node.Val() as IntNode).value));
                    }
                    else if (node.Op == Tag.DL_GT)
                    {
                        if (Utils.IsNumType(node.Type()))
                            result = new BoolNode(((result as IntNode).value > (node.Val() as IntNode).value));
                    }
                    else if (node.Op == Tag.DL_NGT)
                    {
                        if (Utils.IsNumType(node.Type()))
                            result = new BoolNode(((result as IntNode).value <= (node.Val() as IntNode).value));
                    }
                }
                else
                {
                    result = node.Val();
                }
            }
            return result;
        }
    }

    class AddExprNode : ExprBaseNode<MulExprNode>
    {
        public override string Gen()
        {
            if (this.IsConstant())
                return this.Val().Gen();
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
                            code += $"pop\t{CodeGenUtils.CurrentStackTop64}\n";
                        }
                        CodeGenUtils.StackDepth--;
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_NULL;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;
                    type = Utils.TypeCalc(type, node.Type(), false);
                }
            }

            return type == VarType.TYPE_NULL ? VarType.TYPE_ERROR : type;
        }

        public override bool IsConstant()
        {
            if (children == null)
                return false;
            foreach (var child in children)
            {
                if (!child.IsConstant())
                    return false;
            }
            return true;
        }

        public override TerminalNode Val()
        {
            if (!this.IsConstant()) return null;
            TerminalNode result = new BoolNode(false);
            foreach (var node in children)
            {
                if (node.Op != Tag.NULL)
                {
                    if (node.Op == Tag.DL_PLUS)
                    {
                        if (Utils.IsNumType(node.Type()))
                            result = new IntNode(((result as IntNode).value + (node.Val() as IntNode).value));
                    }
                    else if (node.Op == Tag.DL_MINUS)
                    {
                        if (Utils.IsNumType(node.Type()))
                            result = new IntNode(((result as IntNode).value - (node.Val() as IntNode).value));
                    }
                }
                else
                {
                    result = node.Val();
                }
            }
            return result;
        }
    }

    class MulExprNode : ExprBaseNode<UnaryNode>
    {
        public override string Gen()
        {
            if (this.IsConstant())
                return this.Val().Gen();
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
                            code += $"pop\t{CodeGenUtils.CurrentStackTop64}\n";
                        }
                        CodeGenUtils.StackDepth--;
                    }
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = VarType.TYPE_NULL;

            if (children != null)
            {
                foreach (var node in children)
                {
                    if (node == null) continue;
                    type = Utils.TypeCalc(type, node.Type(), false);
                }
            }

            return type == VarType.TYPE_NULL ? VarType.TYPE_ERROR : type;
        }

        public override bool IsConstant()
        {
            if (children == null)
                return false;
            foreach (var child in children)
            {
                if (!child.IsConstant())
                    return false;
            }
            return true;
        }

        public override TerminalNode Val()
        {
            if (!this.IsConstant()) return null;
            TerminalNode result = new BoolNode(false);
            foreach (var node in children)
            {
                if (node.Op != Tag.NULL)
                {
                    if (node.Op == Tag.DL_MULTI)
                    {
                        if (Utils.IsNumType(node.Type()))
                            result = new IntNode(((result as IntNode).value * (node.Val() as IntNode).value));
                    }
                    else if (node.Op == Tag.DL_OBELUS)
                    {
                        if (Utils.IsNumType(node.Type()))
                            result = new IntNode(((result as IntNode).value / (node.Val() as IntNode).value));
                    }
                }
                else
                {
                    result = node.Val();
                }
            }
            return result;
        }
    }

    class UnaryNode : ExprBaseNode<FactorNode>
    {
        public List<Tag> unaryOp = new List<Tag>();
        public FactorNode factor;

        public override string Gen()
        {
            if (this.IsConstant())
                return this.Val().Gen();
            string code = string.Empty;
            Tag op = unaryOp.Count > 0 ? unaryOp[0] : Tag.NULL;

            code += factor.Gen();
            if (op == Tag.DL_NOT)
            {
                if (unaryOp.Count % 2 != 0)
                {
                    code += $"xor\t{CodeGenUtils.CurrentStackTop64}, 1\n";
                }
            }
            else if (op == Tag.DL_MINUS || op == Tag.DL_PLUS)
            {
                op = Tag.DL_PLUS;
                foreach (var item in unaryOp)
                {
                    if (item == Tag.DL_MINUS)
                    {
                        op = op == Tag.DL_MINUS ? Tag.DL_PLUS : Tag.DL_MINUS;
                    }
                }

                if (op == Tag.DL_MINUS)
                {
                    code += $"neg\t{CodeGenUtils.CurrentStackTop64}\n";
                }
            }

            return code;
        }

        public override VarType Type()
        {
            return factor.Type();
        }

        public override bool IsConstant()
        {
            return factor.IsConstant();
        }

        public override TerminalNode Val()
        {
            if (!this.IsConstant()) return null;
            TerminalNode result = factor.Val();
            Tag op = unaryOp.Count > 0 ? unaryOp[0] : Tag.NULL;
            if (op == Tag.DL_NOT)
            {
                if (unaryOp.Count % 2 != 0)
                {
                    result = new BoolNode(!((result as BoolNode).value == 1));
                }
            }
            else if (op == Tag.DL_MINUS || op == Tag.DL_PLUS)
            {
                op = Tag.DL_PLUS;
                foreach (var item in unaryOp)
                {
                    if (item == Tag.DL_MINUS)
                    {
                        op = op == Tag.DL_MINUS ? Tag.DL_PLUS : Tag.DL_MINUS;
                    }
                }

                if (op == Tag.DL_MINUS)
                {
                    result = new IntNode(-(result as IntNode).value);
                }
            }

            return result;
        }
    }

    class FactorNode : ExprBaseNode<AstNode>
    {
        public TerminalNode value;
        public ExprNode expr;
        public ExprNode indexer;
        public FuncCallStmtNode func;

        public override string Gen()
        {
            if (this.IsConstant())
                return this.Val().Gen();
            string code = string.Empty;
            if (indexer == null)
            {
                code += value?.Gen() ?? string.Empty;
                code += expr?.Gen() ?? string.Empty;
                code += func?.Gen() ?? string.Empty;
            }
            else
            {
                VarSymbol arr = SymbolTable.FindVar((value as IdNode).name);
                code += indexer.Gen();
                code += $"mov\trbx, [rbp{(-arr.offsetInFun>0?"+":"")}{-arr.offsetInFun}]\n";
                code += $"lea\t{CodeGenUtils.CurrentStackTop64}, [rbx+{CodeGenUtils.CurrentStackTop64}*{arr.eleSize}]\n";
                switch (arr.eleSize)
                {
                    case 1:
                        code += $"movsx\t{CodeGenUtils.CurrentStackTop64}, byte [{CodeGenUtils.CurrentStackTop64}]\n";
                        break;
                    case 4:
                        code += $"mov\t{CodeGenUtils.CurrentStackTop32}, [{CodeGenUtils.CurrentStackTop64}]\n";
                        if (CodeGenUtils.CurrentStackTop32 == "eax")
                        {
                            code += "cdqe\n";
                        }
                        else
                        {
                            code += $"xchg\trax, {CodeGenUtils.CurrentStackTop64}\n";
                            code += "cdqe\n";
                            code += $"xchg\trax, {CodeGenUtils.CurrentStackTop64}\n";
                        }
                        break;
                    case 8:
                        code += $"mov\t{CodeGenUtils.CurrentStackTop64}, [{CodeGenUtils.CurrentStackTop64}]\n";
                        break;
                }
            }

            return code;
        }

        public override VarType Type()
        {
            VarType type = value?.Type() ?? expr?.Type() ?? func?.Type() ?? VarType.TYPE_ERROR;
            if (indexer != null)
                type = SymbolTable.FindVar((value as IdNode).name).eleType;
            return type;
        }

        public override bool IsConstant()
        {
            return value?.IsConstant() ?? expr?.IsConstant() ?? false;
        }

        public override TerminalNode Val()
        {
            if (!this.IsConstant()) return null;
            return value == null ? expr?.Val() ?? null : value;
        }
    }
}
