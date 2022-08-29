using System;
using PilotLang;

namespace PilotInterpreter.Visitors
{
    public class Interpreter : IExprVisitor<object>, ITopLevelVisitor<int>, IStatementVisitor<int>, ITypeVisitor<int>
    {
        public object VisitIdentifier(IdentifierAstExpr ident)
        {
            throw new System.NotImplementedException();
        }

        public object VisitInteger(IntegerAstExpr integer)
        {
            return integer.Token.Number;
        }

        public object VisitAssignment(AssignmentAstExpr assign)
        {
            throw new System.NotImplementedException();
        }

        
        public object VisitBinaryExpr(BinaryAstExpr astExpr)
        {
            switch (astExpr.Op)
            {
                case TwoUnitOp.Plus:
                    return (int)this.VisitExpr(astExpr.Left) + (int)this.VisitExpr(astExpr.Right);
                case TwoUnitOp.Divide:
                    return (int)this.VisitExpr(astExpr.Left) / (int)this.VisitExpr(astExpr.Right);
                default:
                    throw new NotImplementedException();
            }
        }

        public int VisitFunction(FunctionAstPart func)
        {
            this.VisitBlock(func.FuncBody);
            return 0;
        }

        public int VisitBlock(BlockAst block)
        {
            foreach (var statement in block.Statements)
            {
                this.VisitStatement(statement);
            }

            return 0;
        }

        public int VisitExprStatement(ExprStatement exprStmnt)
        {
            Console.WriteLine(this.VisitExpr(exprStmnt.Expr));
            return 0;
        }

        public int VisitWhileLoop(WhileLoopAstStatement loop)
        {
            throw new System.NotImplementedException();
        }

        public int VisitVariableDeclaration(VariableDeclarationAstStatement v)
        {
            throw new System.NotImplementedException();
        }

        public int VisitReturnStatement(ReturnAstStatement r)
        {
            throw new System.NotImplementedException();
        }

        public int VisitSimpleType(SimpleType st)
        {
            throw new System.NotImplementedException();
        }

        public int VisitArrayType(ArrayType at)
        {
            throw new System.NotImplementedException();
        }

        public int VisitGenericType(GenericType gt)
        {
            throw new System.NotImplementedException();
        }
    }
}