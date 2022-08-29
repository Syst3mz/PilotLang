using System;
using PilotLang;
using PilotLang.Tokens;

namespace PilotInterpreter
{
    public interface ITopLevelVisitor<T>
    {
        public T VisitFunction(FunctionAstPart func);
    }

    public static class VisitTopLevelExtender
    {
        public static T VisitTopLevel<T>(this ITopLevelVisitor<T> top, IAstPart part)
        {
            switch (part)
            {
                case FunctionAstPart f:
                    return top.VisitFunction(f);
                default:
                    throw new ArgumentException($"{part} is an unsupported top level error.");
            }
        }
    }

    public interface IStatementVisitor<T>
    {
        public T VisitBlock(BlockAst block);
        public T VisitExprStatement(ExprStatement exprStmnt);
        public T VisitWhileLoop(WhileLoopAstStatement loop);
        public T VisitVariableDeclaration(VariableDeclarationAstStatement v);
        public T VisitReturnStatement(ReturnAstStatement r);
    }
    
    public static class StatementVisitorExtender
    {
        public static T VisitStatement<T>(this IStatementVisitor<T> v, IAstStatement statement)
        {
            switch (statement)
            {
                case BlockAst b:
                    return v.VisitBlock(b);
                case ExprStatement e:
                    return v.VisitExprStatement(e);
                case WhileLoopAstStatement w:
                    return v.VisitWhileLoop(w);
                case VariableDeclarationAstStatement va:
                    return v.VisitVariableDeclaration(va);
                case ReturnAstStatement r:
                    return v.VisitReturnStatement(r);
                default:
                    throw new ArgumentException($"{statement} is an unsupported statement.");
            }
        }
    }
    public interface IExprVisitor<T>
    {
        public T VisitIdentifier(IdentifierAstExpr ident);
        public T VisitInteger(IntegerAstExpr integer);
        public T VisitAssignment(AssignmentAstExpr assign);
        public T VisitBinaryExpr(BinaryAstExpr astExpr);
    }
    
    public static class ExprVisitorExtender
    {
        public static T VisitExpr<T>(this IExprVisitor<T> v, IAstExpr expr)
        {
            switch (expr)
            {
                case IdentifierAstExpr b:
                    return v.VisitIdentifier(b);
                case IntegerAstExpr e:
                    return v.VisitInteger(e);
                case AssignmentAstExpr w:
                    return v.VisitAssignment(w);
                case BinaryAstExpr bi:
                    return v.VisitBinaryExpr(bi);
                default:
                    throw new ArgumentException($"{expr} is an unsupported expr.");
            }
        }
    }

    public interface ITypeVisitor<T>
    {
        public T VisitSimpleType(SimpleType st);
        public T VisitArrayType(ArrayType at);
        public T VisitGenericType(GenericType gt);

    }

    public static class TypeVisitorExtender
    {
        public static T VisitType<T>(this ITypeVisitor<T> v, IAstType t)
        {
            switch (t)
            {
                case SimpleType s:
                    return v.VisitSimpleType(s);
                case ArrayType a:
                    return v.VisitArrayType(a);
                case GenericType g:
                    return v.VisitGenericType(g);
                default:
                    throw new ArgumentException($"{t} is an unsupported type.");
            }
        }
    }
}