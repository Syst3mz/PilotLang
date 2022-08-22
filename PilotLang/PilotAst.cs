using System;
using System.Collections.Generic;
using System.Linq;
using PilotLang.Tokens;

namespace PilotLang
{
    public static class PilotAst
    {
        // Make my own token stream sensibly
        private static IEnumerator<IToken> _tokens;
        private static bool _done = false;
        private static IToken _current { get => _tokens.Current; }

        // Actually parse
        public static List<AstTopLevel> BuildAbstractSyntaxTree(IEnumerable<IToken> tokens)
        {
            _tokens = tokens.GetEnumerator();
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
            _tokens.MoveNext();
            if (_current == null)
            {
                _done = false;
            }
        }
        
        private static IEnumerable<AstTopLevel> ParseFile()
        {
            while (_done)
            {
                if (Match(TokenType.Function))
                {
                    yield return new AstTopLevel(ParseFunction());
                }
                
                Advance();
            }
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
                funcBlock.Add(ParseExpr());
            }
            
            return new FunctionAstPart(retType, funcName, argList, new BlockAstPart(funcBlock));
        }

        private static IAstExpr ParseExpr()
        {
            // check if there *is* a return statement
            while (true)
            {
                if (Match(TokenType.Return))
                {
                    return ParseReturnStatement();    
                }

                if (Match(TokenType.Identifier) || Match(TokenType.Integer))
                {
                    return ParseAstTerminal();
                }
            }
        }

        private static IAstExpr ParseAstTerminal()
        {
            if (Expect(TokenType.Integer))
            {
                var ret = (IntegerToken)_current;
                Advance();
                return new IntegerAstPart(ret);
            }
            
            if (Expect(typeof(Identifier)))
            {
                var ret = (Identifier)Current;
                Advance();
                return new IdentifierAstPart(ret);
            }

            throw new Exception($"Unexpected {Current} token!");
        }

        private static IAstExpr ParseReturnStatement()
        {
            return ParseExpr();
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
                if (CTokMatches(typeof(LesserThan)))
                {
                    var args = new List<IAstType>();
                    while (!CTokMatches(typeof(GreaterThan)))
                    {
                        args.Add(ParseType());
                        if (CTokMatches(typeof(Coma)))
                        {
                        }
                        else
                        {
                            if (CTokMatches(typeof(GreaterThan)))
                            {
                                break;
                            }
                            
                            throw new Exception("A generic type argument list must be coma delimited");
                        }
                    }

                    cur = new GenericType(cur, args);
                }
                else if (CTokMatches(typeof(LeftBracket)))
                {
                    if (!CTokMatches(typeof(RightBracket)))
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
            if (!CTokIs(typeof(Identifier)))
            {
                throw new Exception("Type Expected but not found");
            }
            
            var ret = new SimpleType((Identifier)Current);
            Advance();
            return ret;
        }
    }
}