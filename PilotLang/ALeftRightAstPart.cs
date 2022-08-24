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
        Devide
    }

    public struct TwoUnitOperator
    {
        public IAstExpr Left, Right;
        public TwoUnitOperatorType Type;

        public TwoUnitOperator(IAstExpr left, IAstExpr right, TwoUnitOperatorType type)
        {
            Left = left;
            Right = right;
            Type = type;
        }
    }
}