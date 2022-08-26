namespace PilotLang
{
    public enum TwoUnitOperatorType
    {
        LesserThan,
        Leq,
        GreaterThan,
        Geq,
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
        public TwoUnitOperatorType Type;

        public BinaryAstExpr(IAstExpr left, IAstExpr right, TwoUnitOperatorType type)
        {
            Left = left;
            Right = right;
            Type = type;
        }
    }
}