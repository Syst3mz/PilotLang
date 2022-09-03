using System;
using System.Collections.Generic;
using PilotLang;

namespace PilotInterpreter.Visitors
{
    public class Interpreter : IExprVisitor<IInterpreterValue>, ITopLevelVisitor<int>, IStatementVisitor<int>, ITypeVisitor<int>
    {
        private List<Dictionary<string, IInterpreterValue>> _scopedVariablesStack = new List<Dictionary<string, IInterpreterValue>>();

        public IInterpreterValue VisitIdentifier(IdentifierAstExpr ident)
        {
            foreach (var scope in _scopedVariablesStack)
            {
                if (scope.ContainsKey(ident.Token.Text))
                {
                    return scope[ident.Token.Text];
                }
            }

            throw new RuntimeError($"{ident.Token.Text} not found in scope.");
        }

        public IInterpreterValue VisitInteger(IntegerAstExpr integer)
        {
            return new InterpreterInt(integer.Token.Number);
        }

        public IInterpreterValue VisitAssignment(AssignmentAstExpr assign)
        {
            foreach (var scope in _scopedVariablesStack)
            {
                if (scope.ContainsKey(assign.VarName.Text))
                {
                    switch (assign.Op)
                    {
                        case AssignmentAstExpr.OpCode.NoOp:
                            scope[assign.VarName.Text] = this.VisitExpr(assign.VarValue);
                            break;
                    }
                }
            }

            throw new RuntimeError($"{assign.VarName} not found in scope.");
        }

        
        public IInterpreterValue VisitBinaryExpr(BinaryAstExpr astExpr)
        {
            switch (astExpr.Op)
            {
                case TwoUnitOp.Plus:
                    throw new NotImplementedException();
                case TwoUnitOp.Divide:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        public int VisitFunction(FunctionAstPart func)
        {
            VisitBlock(func.FuncBody);
            return 0;
        }

        public int VisitBlock(BlockAst block)
        {
            _scopedVariablesStack.Insert(0, new Dictionary<string, IInterpreterValue>());
            foreach (var statement in block.Statements)
            {
                this.VisitStatement(statement);
            }
            _scopedVariablesStack.RemoveAt(0);

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

    public interface IInterpreterValue
    {
    }

    public interface INumericValue : IInterpreterValue
    {
        public INumericValue Add(INumericValue n);
        public INumericValue Subtract(INumericValue n);
        public INumericValue Multiply(INumericValue n);
        public INumericValue Divide(INumericValue n);
    }

    public class InterpreterInt : INumericValue
    {
        public int Inside;

        public InterpreterInt(int inside)
        {
            Inside = inside;
        }

        public INumericValue Add(INumericValue n)
        {
            throw new NotImplementedException();
        }

        public INumericValue Subtract(INumericValue n)
        {
            throw new NotImplementedException();
        }

        public INumericValue Multiply(INumericValue n)
        {
            throw new NotImplementedException();
        }

        public INumericValue Divide(INumericValue n)
        {
            throw new NotImplementedException();
        }
    }

    public class RuntimeError : Exception
    {
        public string ErrorText;

        public RuntimeError(string errorText)
        {
            ErrorText = errorText;
        }
    }
}