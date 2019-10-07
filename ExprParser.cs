using KayoCompiler.Ast;

namespace KayoCompiler
{
    partial class Parser
    {
        private void Expr(ref ExprNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new ExprNode();
                    LogicExpr(ref node.expr);
                    break;
            }
        }

        private void LogicExpr(ref LogicExprNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new LogicExprNode();
                    LogicTerm(ref node.term);
                    LogicExprTail(ref node.tail);
                    break;
            }
        }

        private void LogicExprTail(ref LogicExprTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_OR:
                    node = new LogicExprTailNode();
                    Move();
                    LogicTerm(ref node.term);
                    LogicExprTail(ref node.tail);
                    break;
            }
        }

        private void LogicTerm(ref LogicTermNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new LogicTermNode();
                    LogicFactor(ref node.factor);
                    LogicTermTail(ref node.tail);
                    break;
            }
        }

        private void LogicTermTail(ref LogicTermTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_AND:
                    node = new LogicTermTailNode();
                    Move();
                    LogicFactor(ref node.factor);
                    LogicTermTail(ref node.tail);
                    break;
            }
        }

        private void LogicFactor(ref LogicFactorNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new LogicFactorNode();
                    LogicRel(ref node.rel);
                    LogicFactorTail(ref node.tail);
                    break;
            }
        }

        private void LogicFactorTail(ref LogicFactorTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_EQ:
                case Tag.DL_NEQ:
                    node = new LogicFactorTailNode
                    {
                        op = next.Tag
                    };
                    Move();
                    LogicRel(ref node.rel);
                    LogicFactorTail(ref node.tail);
                    break;
            }
        }

        private void LogicRel(ref LogicRelNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new LogicRelNode();
                    MathExpr(ref node.expr);
                    LogicRelTail(ref node.tail);
                    break;
            }
        }

        private void LogicRelTail(ref LogicRelTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_LT:
                case Tag.DL_NLT:
                case Tag.DL_GT:
                case Tag.DL_NGT:
                    node = new LogicRelTailNode
                    {
                        op = next.Tag
                    };
                    Move();
                    MathExpr(ref node.expr);
                    LogicRelTail(ref node.tail);
                    break;
            }
        }

        private void MathExpr(ref MathExprNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new MathExprNode();
                    MathTerm(ref node.term);
                    MathExprTail(ref node.tail);
                    break;
            }
        }

        private void MathExprTail(ref MathExprTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_PLUS:
                case Tag.DL_MINUS:
                    node = new MathExprTailNode
                    {
                        op = next.Tag
                    };
                    Move();
                    MathTerm(ref node.term);
                    MathExprTail(ref node.tail);
                    break;
            }
        }

        private void MathTerm(ref MathTermNode node)
        {
            switch (next?.Tag)
            {
                case Tag.ID:
                case Tag.NUM:
                case Tag.DL_LPAR:
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                case Tag.DL_NOT:
                    node = new MathTermNode();
                    MathFactor(ref node.factor);
                    MathTermTail(ref node.tail);
                    break;
            }
        }

        private void MathTermTail(ref MathTermTailNode node)
        {
            switch (next?.Tag)
            {
                case Tag.DL_MULTI:
                case Tag.DL_OBELUS:
                    node = new MathTermTailNode
                    {
                        op = next.Tag
                    };
                    Move();
                    MathFactor(ref node.factor);
                    MathTermTail(ref node.tail);
                    break;
            }
        }

        private void MathFactor(ref MathFactorNode node)
        {
            node = new MathFactorNode();

            switch (next?.Tag)
            {
                case Tag.ID:
                    node.value = new IdNode(next.Value);
                    Move();
                    break;
                case Tag.NUM:
                    node.value = new IntNode(int.Parse(next.Value));
                    Move();
                    break;
                case Tag.DL_LPAR:
                    Move();
                    Expr(ref node.expr);
                    if (next.Tag == Tag.DL_RPAR)
                        Move();
                    else
                        new Error(scanner.LineNum).PrintErrMsg();
                    break;
                case Tag.KW_TRUE:
                case Tag.KW_FALSE:
                    node.value = new BoolNode(bool.Parse(next.Value));
                    Move();
                    break;
                case Tag.DL_NOT:
                    Move();
                    MathFactor(ref node.factor);
                    break;
                default:
                    node = null;
                    break;
            }
        }
    }
}
