using System;
using System.Collections.Generic;
using PilotLang;

namespace PilotInterpreter.Visitors
{
    public class Interpreter : IExprVisitor<object>, ITopLevelVisitor<int>, IStatementVisitor<int>, ITypeVisitor<int>
    {
        private Stack<Dictionary<string, IValue>> ScopedVariablesStack = new Stack<Dictionary<string, IValue>>();

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
            if (!ScopedVariablesStack.Peek().ContainsKey(assign.VarName.Text))
            {
                throw new RuntimeError($"{assign.VarName.Text} does not exist in current scope");
            }
            
            switch (assign.Op)
            {
                case AssignmentAstExpr.OpCode.NoOp:
                    ScopedVariablesStack.Peek()[assign.VarName.Text] = this.VisitExpr(assign.VarValue);
            }
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
            ScopedVariablesStack.Push(new Dictionary<string, IValue>());
            VisitBlock(func.FuncBody);
            ScopedVariablesStack.Pop();
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

    public interface IInterpreterValue
    {
        IInterpreterValue From(object o);
    }

    public class InterpreterInt : IInterpreterValue
    {
        public int Inside;

        public InterpreterInt(int inside)
        {
            Inside = inside;
        }

        public IInterpreterValue From(object o)
        {
            if (o is )
            {
                throw new NotImplementedException();
            }
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