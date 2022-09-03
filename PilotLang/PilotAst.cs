using System;
using System.Collections.Generic;
using System.Linq;
using PilotLang.Tokens;

namespace PilotLang
{
    public static class PilotAst
    {
        // Make my own token stream sensibly
        private static List<IToken> _tokens;
        private static bool _done = false;
        private static IToken _current { get => _tokens[_positionInTokenArray]; }
        private static IToken _lastToken
        {
            get {
                if (_positionInTokenArray > 0)
                {
                    return _tokens[_positionInTokenArray];
                }
                return _tokens[0];
            }
        }

        private static int _positionInTokenArray = 0;

        // Actually parse
        public static List<IAstPart> BuildAbstractSyntaxTree(IEnumerable<IToken> tokens)
        {
            _tokens = tokens.ToList();
            List<IAstPart> ret = new List<IAstPart>();

            return ParseFile();
        }

        private static bool Match(TokenType t)
        {
            if (_current.Type == t)
            {
                Advance();
                return true;
            }

            return false;
        }
        
        private static bool Expect(TokenType t)
        {
            if (_current.Type == t)
            {
                return true;
            }

            return false;
        }

        private static void Advance()
        {
            if (_positionInTokenArray + 1 >= _tokens.Count)
            {
                _done = true;
            }
            else
            {
                _positionInTokenArray++;
            }
        }
        
        private static void Retreat()
        {
            if (_positionInTokenArray - 1 >= 0)
            {
                _positionInTokenArray--;
            }
        }
        
        private static List<IAstPart> ParseFile()
        {
            var ret = new List<IAstPart>();
            while (!_done)
            {
                if (Expect(TokenType.Function))
                {
                    ret.Add(ParseFunction());
                }
                else if (Expect(TokenType.Struct))
                {
                    ret.Add(ParseStruct());
                }
                else if (Expect(TokenType.Trait))
                {
                    ret.Add(ParseTrait());
                }
                else
                {
                    ret.Add(ParseEnum());
                }

                Advance();
            }

            return ret;
        }

        private static IAstPart ParseEnum()
        {
            if (!Expect(TokenType.Identifier))
            {
                
            }
        }

        private static IAstPart ParseTrait()
        {
            throw new NotImplementedException();
        }

        private static IAstPart ParseStruct()
        {
            throw new NotImplementedException();
        }

        private static FunctionAstPart ParseFunction()
        {
            var fnTok = _current;
            Advance();
            var retType = ParseType();
            
            
            if (!Expect(TokenType.Identifier))
            {
                throw new ParseError(fnTok.LinePos, fnTok.charPos - 1,"Function must have a name.");
            } 

            var funcName = (IdentifierToken)_current;
            Advance();

            List<(IdentifierToken, IAstType)> argList = ParseArgList();

            return new FunctionAstPart(retType, funcName, argList, ParseBlock());
        }

        private static BlockAst ParseBlock()
        {
            if (!Match(TokenType.LeftBrace))
            {
                throw new ParseError(_lastToken, "A block must start with {");
            }

            List<IAstStatement> statements = new List<IAstStatement>();
            while (!Match(TokenType.RightBrace))
            {
                statements.Add(ParseStatement());
            }

            return new BlockAst(statements);
        }

        private static IAstStatement ParseStatement()
        {
            if (Match(TokenType.Return))
            {

                ReturnAstStatement tmp = ParseReturnStatement();
                if (!Match(TokenType.Semicolon))
                {
                    throw new ParseError(_lastToken,"Statement must end with a ;");
                }

                return tmp;
            }

            if (Match(TokenType.VariableKeyword))
            {
                return ParseDeclaration();
            }
            if (Match(TokenType.While))
            {
                IAstExpr check = ParseExpr();
                BlockAst block = ParseBlock();

                return new WhileLoopAstStatement(check, block);
            }
            if (Match(TokenType.For))
            {
                if (Expect(TokenType.Identifier))
                {
                    var id = (IdentifierToken)_current;
                    Advance();
                    if (Expect(TokenType.LesserThan) || Expect(TokenType.LesserThanOrEqualTo))
                    {
                        bool lessThan = Expect(TokenType.LesserThan);
                        Advance();
                        IAstExpr upperBound = ParseExpr();
                        return new ShorthandForLoopAstStatement(upperBound, lessThan, id, ParseBlock());
                    }
                    else if (Match(TokenType.SingleEquals))
                    {
                        IAstExpr initializer = ParseExpr();
                        if (!Match(TokenType.Semicolon))
                        {
                            throw new ParseError(_current, "Initializer must be followed by a semicolon");
                        }

                        IAstExpr upperBound = ParseExpr();
                        if (!Match(TokenType.Semicolon))
                        {
                            throw new ParseError(_current, "Upper bound must be followed by a semicolon");
                        }

                        IAstExpr incrementer = ParseExpr();
                        Match(TokenType.Semicolon);

                        return new ExplicitForLoopAstStatement(upperBound, initializer, incrementer, id, ParseBlock());
                    }
                    else
                    {
                        throw new ParseError(_current, "Expected one of \"=\", \"<\", \"<=\"");
                    }
                }
                else if (Match(TokenType.VariableKeyword))
                {
                    if (!Expect(TokenType.Identifier))
                    {
                        throw new ParseError(_lastToken, "Var must be followed by a identifier");
                    }

                    IdentifierToken id = (IdentifierToken)_current;
                    IAstType varType = ParseType();
                    Advance();
                    if (!Match(TokenType.In))
                    {
                        throw new ParseError(_current, $"Expected in after variable declaration, found {_current}");
                    }

                    if (!Expect(TokenType.Identifier))
                    {
                        throw new ParseError(_current, "In must be followed by an identifier");
                    }

                    IdentifierToken iterable = (IdentifierToken)_current;
                    Advance();

                    return new ExplicitForEachStatement(varType, id, ParseBlock(), iterable);
                }

                throw new ParseError(_current, $"Unexpected {_current} token found after for");
            }
            else
            {
                return ParseExprStatement();
            }
        }
        
        private static (bool, IAstExpr) GetForLoopUpperBound()
        {
            bool lessThan = Expect(TokenType.LesserThan);
            Advance();
            
            IAstExpr upperBound = ParseExpr();
            return (lessThan, upperBound);
        }

        private static IAstStatement ParseExprStatement()
        {
            ExprStatement exprStatement = new ExprStatement(ParseExpr());
            if (!Match(TokenType.Semicolon))
            {
                throw new ParseError(_lastToken,"Statement must end with a ;");
            }

            return exprStatement;
        }

        private static ReturnAstStatement ParseReturnStatement()
        {
            return new ReturnAstStatement(ParseExpr());
        }

        private static IAstExpr ParseExpr()
        {
            return ParseExprOr();
        }

        private static VariableDeclarationAstStatement ParseDeclaration()
        {
            if (!Expect(TokenType.Identifier))
            {
                throw new ParseError(_current, "Var must be followed by a name");
            }

            IdentifierToken varName = (IdentifierToken)_current;
            Advance();

            IAstType varType = ParseType();
            if (Match(TokenType.Semicolon))
            {
                return new VariableDeclarationAstStatement(varName, varType, null);
            }

            if (Match(TokenType.SingleEquals))
            {
                var expr = ParseExpr();
                if (!Match(TokenType.Semicolon))
                {
                    throw new ParseError(_current, "Semicolon expected after assignment expression");
                }

                return new VariableDeclarationAstStatement(varName, varType, expr);
            }

            throw new ParseError(_current, "Variable must be followed by a \";\" or an \"=\" expression");
        }

        private static IAstExpr ParseExprOr()
        {
            IAstExpr left = ParseExprAnd();
            if (Match(TokenType.Or))
            {
                left = new BinaryAstExpr(left, ParseExprAnd(), TwoUnitOp.Or);
            }

            return left;
        }

        private static IAstExpr ParseExprAnd()
        {
            IAstExpr left = ParseComparisons();
            if (Match(TokenType.And))
            {
                left = new BinaryAstExpr(left, ParseComparisons(), TwoUnitOp.And);
            }

            return left;
        }

        private static IAstExpr ParseComparisons()
        {
            IAstExpr left = ParseExprPrecedence1();
            while (true)
            {
                if (Match(TokenType.LesserThan))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence1(), TwoUnitOp.LesserThan);
                }
                else if (Match(TokenType.LesserThanOrEqualTo))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence1(), TwoUnitOp.Leq);
                }
                else if (Match(TokenType.GreaterThan))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence1(), TwoUnitOp.GreaterThan);
                }
                else if (Match(TokenType.GreaterThanOrEqualTo))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence1(), TwoUnitOp.Geq);
                }
                else if (Match(TokenType.EqualsEquals))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence1(), TwoUnitOp.Equals);
                }
                else if (Match(TokenType.ExclamationEquals))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence1(), TwoUnitOp.NotEquals);
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private static IAstExpr ParseExprPrecedence1()
        {
            IAstExpr left = ParseExprPrecedence2();
            while (true)
            {
                if (Match(TokenType.Plus))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence2(), TwoUnitOp.Plus);
                }
                else if (Match(TokenType.Minus))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence2(), TwoUnitOp.Minus);
                }
                else if (Match(TokenType.Increment))
                {
                    if (left is IdentifierAstExpr id)
                    {
                        left = new AssignmentAstExpr(id.Token,
                            new IntegerAstExpr(new IntegerToken(TokenType.Integer, 1, -1, -1)) , AssignmentAstExpr.OpCode.Add);    
                    }
                    else
                    {
                        throw new ParseError(_lastToken, "Increment only only valid on variables");
                    }
                }
                else if (Match(TokenType.Decrement))
                {
                    if (left is IdentifierAstExpr id)
                    {
                        left = new AssignmentAstExpr(id.Token,
                            new IntegerAstExpr(new IntegerToken(TokenType.Integer, 1, -1, -1)) , AssignmentAstExpr.OpCode.Subtract);      
                    }
                    else
                    {
                        throw new ParseError(_lastToken, "Decrement only only valid on variables");
                    }
                }
                else
                {
                    break;
                }
            }

            return left;
        }
        
        private static IAstExpr ParseExprPrecedence2()
        {
            IAstExpr left = ParseAstTerminal();

            while (true)
            {
                if (Match(TokenType.Dot))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence3(), TwoUnitOp.Divide);
                }
                else if (Match(TokenType.Multiply))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence3(), TwoUnitOp.Multiply);
                }
                else if (Match(TokenType.Divide))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence3(), TwoUnitOp.Divide);
                }
                else
                {
                    break;
                }
            }

            return left;
        }
        
        private static IAstExpr ParseExprPrecedence3()
        {
            IAstExpr left = ParseAstTerminal();

            while (true)
            {
                if (Match(TokenType.Divide))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence2(), TwoUnitOp.Divide);
                }
                else if (Match(TokenType.Multiply))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence2(), TwoUnitOp.Multiply);
                }
                else
                {
                    break;
                }
            }

            return left;
        }
        
        private static IAstExpr ParseAstTerminal()
        {
            if (Expect(TokenType.Integer))
            {
                var ret = (IntegerToken)_current;
                Advance();
                return new IntegerAstExpr(ret);
            }
            
            if (Expect(TokenType.Identifier))
            {
                var ret = (IdentifierToken) _current;
                Advance();
                return new IdentifierAstExpr(ret);
            }
            
            if (Match(TokenType.LeftParentheses))
            {
                var xpr = ParseExpr();
                if (Match(TokenType.RightParentheses))
                {
                    return xpr;
                }
                else
                {
                    throw new ParseError(_lastToken,"Opening parenthesis need a closer");
                }
            }

            throw new ParseError(_current,$"Unexpected {_current} token!");
        }


        private static List<(IdentifierToken, IAstType)> ParseArgList()
        {
            List<(IdentifierToken, IAstType)> argList = new List<(IdentifierToken, IAstType)>();
            if (!Match(TokenType.LeftParentheses))
            {
                throw new ParseError(_lastToken, "An arg list's opening paren must be followed by its arguments");
            }
            if (Match(TokenType.RightParentheses))
            {
                return argList;
            }
            while (Expect(TokenType.Identifier))
            {
                var argName = (IdentifierToken) _current;
                Advance();
                if (Expect(TokenType.Identifier))
                {
                    argList.Add((argName, ParseType()));
                    if (!Match(TokenType.Comma))
                    {
                        if (Match(TokenType.RightParentheses))
                        {
                            return argList;
                        }
                    }
                }
            }

            throw new ParseError(_lastToken,"No end to argument list");
        }

        private static IAstType ParseType()
        {
            IAstType cur = ParseSimpleType();
            while (true)
            {
                if (Match(TokenType.LesserThan))
                {
                    var args = new List<IAstType>();
                    while (!Match(TokenType.GreaterThan))
                    {
                        args.Add(ParseType());
                        if (Match(TokenType.Comma))
                        {
                        }
                        else
                        {
                            if (Match(TokenType.GreaterThan))
                            {
                                break;
                            }
                            
                            throw new ParseError(_lastToken,"A generic type argument list must be coma delimited");
                        }
                    }

                    cur = new GenericType(cur, args);
                }
                else if (Match(TokenType.LeftBracket))
                {
                    if (!Match(TokenType.RightBracket))
                    {
                        throw new ParseError(_lastToken,"A opening [ must have a closing ] as part of an array type definition");
                    }

                    cur = new ArrayType(cur);
                }
                else
                {
                    break;
                }
            }

            if (Match(TokenType.QuestionMark))
            {
                return new OptionType(cur);
            }
            
            return cur;
        }

        private static SimpleType ParseSimpleType()
        {
            // Have a name for the type
            if (!Expect(TokenType.Identifier))
            {
                throw new ParseError(_lastToken,"Identifier Expected but not found");
            }
            
            var ret = new SimpleType((IdentifierToken)_current);
            Advance();
            return ret;
        }
    }
}