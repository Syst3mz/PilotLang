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

    public class BlockAstPart : IAstPart
    {
        public List<IAstPart> children;

        public BlockAstPart(List<IAstPart> children)
        {
            this.children = children;
        }
    }


    public struct FunctionAstPart : IAstPart
    {
        public IAstType ReturnType;
        public IdentifierToken FunctionName;
        public List<(IdentifierToken, IAstType)> Arguments;
        public BlockAstPart FuncBody;

        public FunctionAstPart(IAstType returnType, IdentifierToken functionName,
            List<(IdentifierToken, IAstType)> arguments, BlockAstPart funcBody)
        {
            ReturnType = returnType;
            FunctionName = functionName;
            Arguments = arguments;
            FuncBody = funcBody;
        }
    }

    public struct EndOfPhraseAstExpr : IAstExpr
    {

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
}