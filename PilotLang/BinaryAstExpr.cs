namespace PilotLang
{
    public enum TwoUnitOp
    {
        LesserThan,
        Leq,
        GreaterThan,
        Geq,
        Equals,
        NotEquals,
        Plus,
        Minus,
        Multiply,
        Divide,
        Or,
        And
    }

    public struct BinaryAstExpr : IAstExpr
    {
        public IAstExpr Left, Right;
        public TwoUnitOp Op;

        public BinaryAstExpr(IAstExpr left, IAstExpr right, TwoUnitOp op)
        {
            Left = left;
            Right = right;
            Op = op;
        }
    }
}