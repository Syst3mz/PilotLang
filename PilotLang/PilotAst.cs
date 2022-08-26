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
        private static int _positionInTokenArray = 0;

        // Actually parse
        public static List<AstTopLevel> BuildAbstractSyntaxTree(IEnumerable<IToken> tokens)
        {
            _tokens = tokens.ToList();
            List<AstTopLevel> ret = new List<AstTopLevel>();
            /*foreach (AstTopLevel level in ParseFile())
            {
                ret.Add(level);
            }*/
            ret.Add(new AstTopLevel(ParseExpr()));

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
                if (Match(TokenType.Function))
                {
                    ret.Add(new AstTopLevel(ParseFunction()));
                }

                Advance();
            }

            return ret;
        }

        private static FunctionAstPart ParseFunction()
        {
            if (!Expect(TokenType.Identifier))
            {
                //error case
                throw new Exception("fn should be followed by an identifier");
            }

            var retType = ParseType();
            
            
            if (!Expect(TokenType.Identifier))
            {
                throw new Exception("Function must have a name");
            }

            var funcName = (IdentifierToken)_current;
            Advance();
            
            if (!Match(TokenType.LeftParentheses))
            {
                throw new Exception("A function name must be followed by its arguments");
            }
            
            List<(IdentifierToken, IAstType)> argList = ParseArgList();

            if (!Match(TokenType.LeftBrace))
            {
                throw new Exception("A function must have a body");
            }

            List<IAstPart> funcBlock = new List<IAstPart>();
            while (!Match(TokenType.RightBrace))
            {
                funcBlock.Add(ParseStatement());
            }
            
            return new FunctionAstPart(retType, funcName, argList, new BlockAstPart(funcBlock));
        }

        private static IAstStatement ParseStatement()
        {
            if (Match(TokenType.Return))
            {
                return ParseReturnStatement();
            }
            else
            {
                throw new Exception($"Unexpected {_current} token");    
            }
        }

        private static IAstStatement ParseReturnStatement()
        {
            return new ReturnAstStatement(ParseExpr());
        }

        private static IAstExpr ParseExpr()
        {
            return ParseExprPrecedence1();
            
        }

        private static IAstExpr ParseExprPrecedence1()
        {
            IAstExpr left = ParseAstTerminal();
            if (Match(TokenType.Plus))
            {
                return new BinaryAstExpr(left, ParseAstTerminal(), TwoUnitOperatorType.Plus);
            }

            throw new Exception($"Unexpected Token {_current}");
        }
        
        private static IAstExpr ParseExprPrecedence2()
        {
            IAstExpr left = ParseAstTerminal();
            
            if (Match(TokenType.Plus))
            {
                IAstExpr right = ParseAstTerminal();
                Advance();
                return new BinaryAstExpr(left, right, TwoUnitOperatorType.Plus);
            }

            throw new Exception($"Error parsing at {_current}");
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
                return ParseExpr();
            }

            throw new Exception($"Unexpected {_current} token!");
        }


        private static List<(IdentifierToken, IAstType)> ParseArgList()
        {
            List<(IdentifierToken, IAstType)> argList = new List<(IdentifierToken, IAstType)>();
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
                    if (!Match(TokenType.Coma))
                    {
                        if (Match(TokenType.RightParentheses))
                        {
                            return argList;
                        }
                    }
                }
            }

            throw new Exception("No end to argument list");
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
                        if (Match(TokenType.Coma))
                        {
                        }
                        else
                        {
                            if (Match(TokenType.GreaterThan))
                            {
                                break;
                            }
                            
                            throw new Exception("A generic type argument list must be coma delimited");
                        }
                    }

                    cur = new GenericType(cur, args);
                }
                else if (Match(TokenType.LeftBracket))
                {
                    if (!Match(TokenType.RightBracket))
                    {
                        throw new Exception("A opening [ must have a closing ] as part of an array type definition");
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
                throw new Exception("Type Expected but not found");
            }
            
            var ret = new SimpleType((IdentifierToken)_current);
            Advance();
            return ret;
        }
    }
}