using System.Collections.Generic;
using PilotLang.Tokens;

namespace PilotLang
{
    public interface IAstPart
    {

    }

    public class AstTopLevel : IAstPart
    {
        public IAstPart Child;

        public AstTopLevel(IAstPart child)
        {
            Child = child;
        }
    }
    
    public struct BlockAst : IAstExpr
    {
        public List<IAstStatement> Statements;

        public BlockAst(List<IAstStatement> statements)
        {
            Statements = statements;
        }
    }

    public struct ExprStatement : IAstStatement
    {
        public IAstExpr Expr;

        public ExprStatement(IAstExpr expr)
        {
            Expr = expr;
        }
    }


    public struct FunctionAstPart : IAstPart
    {
        public IAstType ReturnType;
        public IdentifierToken FunctionName;
        public List<(IdentifierToken, IAstType)> Arguments;
        public IAstExpr FuncBody;

        public FunctionAstPart(IAstType returnType, IdentifierToken functionName,
            List<(IdentifierToken, IAstType)> arguments, IAstExpr funcBody)
        {
            ReturnType = returnType;
            FunctionName = functionName;
            Arguments = arguments;
            FuncBody = funcBody;
        }
    }

    
    public struct ForLoopShorthand1AstStatement : IAstStatement
    {
        public IAstType IterType;
        public IdentifierToken IterVar;
        public IAstExpr UpperBound;
        public BlockAst Block;
        public bool IsLesserThan;

        public ForLoopShorthand1AstStatement(IAstType iterType, IdentifierToken iterVar, IAstExpr upperBound, BlockAst block, bool isLesserThan)
        {
            IterType = iterType;
            IterVar = iterVar;
            UpperBound = upperBound;
            Block = block;
            IsLesserThan = isLesserThan;
        }
    }

    public interface IAstType : IAstPart
    {

    }

    public struct SimpleType : IAstType
    {
        public IdentifierToken TypeName;

        public SimpleType(IdentifierToken typeName)
        {
            TypeName = typeName;
        }
    }

    public struct ArrayType : IAstType
    {
        public IAstType TypeName;

        public ArrayType(IAstType typeName)
        {
            TypeName = typeName;
        }
    }

    public struct GenericType : IAstType
    {
        public IAstType TypeName;
        public List<IAstType> TypeArguments;

        public GenericType(IAstType typeName, List<IAstType> typeArguments)
        {
            TypeName = typeName;
            TypeArguments = typeArguments;
        }
    }

    public interface IAstStatement : IAstPart
    {
    }

    public struct ReturnAstStatement : IAstStatement
    {
        public IAstExpr Expr;

        public ReturnAstStatement(IAstExpr expr)
        {
            Expr = expr;
        }
    }

    public interface IAstExpr : IAstPart
    {

    }

    public struct IdentifierAstExpr : IAstExpr
    {
        public IdentifierToken Token;

        public IdentifierAstExpr(IdentifierToken token)
        {
            Token = token;
        }
    }

    public struct IntegerAstExpr : IAstExpr
    {
        public IntegerToken Token;

        public IntegerAstExpr(IntegerToken token)
        {
            Token = token;
        }
    }

    public struct AssignmentAstExpr : IAstExpr
    {
        public IdentifierToken left;
        public IAstExpr right;

        public AssignmentAstExpr(IdentifierToken left, IAstExpr right)
        {
            this.left = left;
            this.right = right;
        }
    }
}