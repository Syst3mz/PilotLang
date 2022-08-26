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
                else
                {
                    return _tokens[0];
                }
            }
        }

        private static int _positionInTokenArray = 0;

        // Actually parse
        public static List<AstTopLevel> BuildAbstractSyntaxTree(IEnumerable<IToken> tokens)
        {
            _tokens = tokens.ToList();
            List<AstTopLevel> ret = new List<AstTopLevel>();
            foreach (AstTopLevel level in ParseFile())
            {
                ret.Add(level);
            }

            return ret;
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
        
        private static List<AstTopLevel> ParseFile()
        {
            var ret = new List<AstTopLevel>();
            while (!_done)
            {
                if (Expect(TokenType.Function))
                {
                    ret.Add(new AstTopLevel(ParseFunction()));
                }

                Advance();
            }

            return ret;
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

            if (Match(TokenType.For))
            {
                var forTok = _lastToken;
                if (!Expect(TokenType.Identifier))
                {
                    throw new ParseError(forTok, "\for\" must be followed by a identifier");
                }

                var iterName = (IdentifierToken)_current;
                if (!Match(TokenType.In))
                {
                    if (!Expect(TokenType.Identifier))
                    {
                        throw new ParseError(forTok, "\"in\" must be followed by an identifier");
                    }
                    // for each
                }
                else if (!Expect(TokenType.LesserThan) || !Expect(TokenType.LesserThanOrEqualTo))
                {
                    throw new ParseError(forTok, "For shorthand must have either \"<\" or \"<=\"");
                }

                bool lessThan = Match(TokenType.LesserThan);

                IAstExpr upperBound = ParseExpr();

                return new ForLoopShorthand1AstStatement(
                    new SimpleType(
                        new IdentifierToken(TokenType.Identifier, "int", forTok.LinePos, forTok.charPos)),
                        iterName,
                        upperBound,
                        ParseBlock(),
                        lessThan
                    );

            }
            else
            {
                return ParseExprStatement();
            }
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
            return ParseAssignment();
        }

        private static IAstExpr ParseAssignment()
        {
            IAstExpr left = ParseExprOr();
            if (Match(TokenType.Assign))
            {
                if (left is IdentifierAstExpr id)
                {
                    left = new AssignmentAstExpr(id.Token, ParseExprOr());
                }
                else
                {
                    throw new ParseError(_lastToken,"Assignment needs to be to an identifier");
                }
            }

            return left;
        }

        private static IAstExpr ParseExprOr()
        {
            IAstExpr left = ParseExprAnd();
            if (Match(TokenType.Or))
            {
                left = new BinaryAstExpr(left, ParseExprAnd(), TwoUnitOperatorType.Or);
            }

            return left;
        }

        private static IAstExpr ParseExprAnd()
        {
            IAstExpr left = ParseExprPrecedence1();
            if (Match(TokenType.And))
            {
                left = new BinaryAstExpr(left, ParseExprPrecedence1(), TwoUnitOperatorType.And);
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
                    left = new BinaryAstExpr(left, ParseExprPrecedence2(), TwoUnitOperatorType.Plus);
                }
                else if (Match(TokenType.Minus))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence2(), TwoUnitOperatorType.Minus);
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
                    left = new BinaryAstExpr(left, ParseExprPrecedence3(), TwoUnitOperatorType.Divide);
                }
                else if (Match(TokenType.Multiply))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence3(), TwoUnitOperatorType.Multiply);
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
                    left = new BinaryAstExpr(left, ParseExprPrecedence2(), TwoUnitOperatorType.Divide);
                }
                else if (Match(TokenType.Multiply))
                {
                    left = new BinaryAstExpr(left, ParseExprPrecedence2(), TwoUnitOperatorType.Multiply);
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
                throw new ParseError(_lastToken, "A function name must be followed by its arguments");
            }
            
            if (Expect(TokenType.RightParentheses))
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