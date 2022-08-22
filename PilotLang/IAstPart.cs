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

        public FunctionAstPart(IAstType returnType, IdentifierToken functionName, List<(IdentifierToken, IAstType)> arguments, BlockAstPart funcBody)
        {
            ReturnType = returnType;
            FunctionName = functionName;
            Arguments = arguments;
            FuncBody = funcBody;
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

    public struct ReturnAstExpr : IAstExpr
    {
        public IAstPart Statement;

        public ReturnAstExpr(IAstPart statement)
        {
            Statement = statement;
        }
    }

    public interface IAstExpr : IAstPart
    {
        
    }

    public interface IAstTerminal : IAstExpr
    {
    }
    
    public struct IdentifierAstPart : IAstTerminal
    {
        public IdentifierToken Token;

        public IdentifierAstPart(IdentifierToken token)
        {
            Token = token;
        }
    }
    
    public struct IntegerAstPart : IAstTerminal
    {
        public IntegerToken Token;

        public IntegerAstPart(IntegerToken token)
        {
            Token = token;
        }
    }
}