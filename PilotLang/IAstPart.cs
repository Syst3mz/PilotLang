using System.Collections.Generic;
using PilotLang.Tokens;

namespace PilotLang
{
    public interface IAstPart
    {
        
    }

    public struct BlockAst : IAstStatement
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
        public BlockAst FuncBody;

        public FunctionAstPart(IAstType returnType, IdentifierToken functionName,
            List<(IdentifierToken, IAstType)> arguments, BlockAst funcBody)
        {
            ReturnType = returnType;
            FunctionName = functionName;
            Arguments = arguments;
            FuncBody = funcBody;
        }
    }

    public struct WhileLoopAstStatement : IAstStatement
    {
        public IAstExpr Check;
        public BlockAst Block;

        public WhileLoopAstStatement(IAstExpr check, BlockAst block)
        {
            Check = check;
            Block = block;
        }
    }

    public interface IForLoopAstStatement : IAstStatement
    {
        public IdentifierToken IterVar { get; }
        public BlockAst Block { get; }
    }

    public struct ShorthandForLoopAstStatement : IForLoopAstStatement
    {
        public IdentifierToken IterVar { get; }
        public IAstExpr UpperBound;
        public BlockAst Block { get; }
        public bool IsLesserThan;

        public ShorthandForLoopAstStatement(IAstExpr upperBound, bool isLesserThan, IdentifierToken iterVar, BlockAst block)
        {
            UpperBound = upperBound;
            IsLesserThan = isLesserThan;
            IterVar = iterVar;
            Block = block;
        }
    }
    
    public struct ExplicitForLoopAstStatement : IForLoopAstStatement
    {
        public IdentifierToken IterVar { get; }
        public BlockAst Block { get; }
        public IAstExpr UpperBound;
        public IAstExpr InitialValue;
        public IAstExpr Incrementer;

        public ExplicitForLoopAstStatement(IAstExpr upperBound, IAstExpr initialValue, IAstExpr incrementer, IdentifierToken iterVar, BlockAst block)
        {
            UpperBound = upperBound;
            InitialValue = initialValue;
            Incrementer = incrementer;
            IterVar = iterVar;
            Block = block;
        }
    }


    public interface IForEachAstStatement : IForLoopAstStatement
    {
        public IdentifierToken IterThough { get; }
    }

    public struct ExplicitForEachStatement : IForEachAstStatement
    {
        public IAstType Type;
        public IdentifierToken IterVar { get; }
        public BlockAst Block { get; }
        public IdentifierToken IterThough { get; }

        public ExplicitForEachStatement(IAstType type, IdentifierToken iterVar, BlockAst block, IdentifierToken iterThough)
        {
            Type = type;
            IterVar = iterVar;
            Block = block;
            IterThough = iterThough;
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
        public IAstType InternalType;

        public ArrayType(IAstType internalType)
        {
            InternalType = internalType;
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

    public struct OptionType : IAstType
    {
        public IAstType InternalType;

        public OptionType(IAstType internalType)
        {
            InternalType = internalType;
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

    public struct VariableDeclarationAstStatement : IAstStatement
    {
        
        public IdentifierToken VarName;
        public IAstType Type;
        public IAstExpr VarValue;

        public VariableDeclarationAstStatement(IdentifierToken varName, IAstType type, IAstExpr varValue)
        {
            VarName = varName;
            VarValue = varValue;
            Type = type;
        }
    }

    public struct AssignmentAstExpr : IAstExpr
    {
        public enum OpCode
        {
            NoOp,
            Add,
            Subtract,
            Multiply,
            Divide
        }
        
        public IdentifierToken VarName;
        public IAstExpr VarValue;
        public OpCode Op;

        public AssignmentAstExpr(IdentifierToken varName, IAstExpr varValue, OpCode op=OpCode.NoOp)
        {
            VarName = varName;
            VarValue = varValue;
            Op = op;
        }
    }

    public struct TraitDeclaration : IAstPart
    {
        public IAstType Trait;
        public List<FunctionAstPart> Funcs;

        public TraitDeclaration(IAstType trait, List<FunctionAstPart> funcs)
        {
            Trait = trait;
            Funcs = funcs;
        }
    }

    public interface IEnum : IAstPart
    {
        public IdentifierToken Name { get; }
    }

    public struct GenericEnum : IEnum
    {
        public IdentifierToken Name { get; }
        public List<IdentifierToken> TypeArgs;
        public List<(IdentifierToken, List<IdentifierToken>)> Variants;

        public GenericEnum(List<IdentifierToken> typeArgs, List<(IdentifierToken, List<IdentifierToken>)> variants, IdentifierToken name)
        {
            TypeArgs = typeArgs;
            Variants = variants;
            Name = name;
        }
    }
    
    public struct Enum : IEnum
    {
        public IdentifierToken Name { get; }
        public List<(IdentifierToken, List<IdentifierToken>)> Variants;

        public Enum(List<(IdentifierToken, List<IdentifierToken>)> variants, IdentifierToken name)
        {
            Variants = variants;
            Name = name;
        }
    }
}