using System;
using System.Collections.Generic;
using PilotLang.Tokens;

namespace PilotLang
{
    public static class PilotAst
    {
        // Make my own token stream sensibly
        private static bool Done;
        private static IToken Current
        {
            get => _tokens[_positionInArray];
        }
        private static int _positionInArray = 0;
        private static List<IToken> _tokens;


        private static bool CTokMatches(Type t)
        {
            if (Current.GetType() == t)
            {
                GetNextToken();
                return true;
            }

            return false;
        }

        private static bool CTokIs(Type t)
        {
            if (Current.GetType() == t)
            {
                return true;
            }

            return false;
        }

        private static IToken GetNextToken()
        {
            Advance();
            return Current;
        }
        
        private static void Advance()
        {
            if (_positionInArray + 1 >= _tokens.Count)
            {
                Done = true;
            }
            else
            {
                _positionInArray++;
            }
        }

        private static void Reset()
        {
            _positionInArray = 0;
        }

        // Actually parse
        public static List<AstTopLevel> BuildAbstractSyntaxTree(List<IToken> tokens)
        {
            // Make a root node
            _tokens = tokens;
            
            var roots = ParseFile();
            return roots;
        }

        private static List<AstTopLevel> ParseFile()
        {
            List<AstTopLevel> roots = new List<AstTopLevel>();
            while (!Done)
            {
                if (CTokMatches(typeof(Function)))
                {
                    roots.Add(new AstTopLevel(ParseFunction()));
                }
                else
                {
                    Advance();
                }
                
            }

            return roots;
        }

        private static FunctionAstPart ParseFunction()
        {
            if (!CTokIs(typeof(Identifier)))
            {
                //error case
                throw new Exception("fn should be followed by an identifier");
            }

            var retType = ParseType();
            
            
            if (!CTokIs(typeof(Identifier)))
            {
                throw new Exception("Function must have a name");
            }

            var funcName = (Identifier)Current;
            Advance();
            
            if (!CTokMatches(typeof(LeftParen)))
            {
                throw new Exception("A function name must be followed by its arguments");
            }
            
            List<(Identifier, IAstType)> argList = ParseArgList();

            if (!CTokMatches(typeof(LeftBrace)))
            {
                throw new Exception("A function must have a body");
            }

            List<IAstPart> funcBlock = new List<IAstPart>();
            while (!CTokMatches(typeof(RightBrace)))
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
                if (CTokMatches(typeof(Return)))
                {
                    return ParseReturnStatement();    
                }

                if (CTokMatches(typeof(Identifier)) || CTokMatches(typeof(Integer)))
                {
                    return ParseAstTerminal();
                }
            }
        }

        private static IAstExpr ParseAstTerminal()
        {
            if (CTokIs(typeof(Integer)))
            {
                var ret = (Integer)Current;
                Advance();
                return new IntegerAstPart(ret);
            }
            
            if (CTokIs(typeof(Identifier)))
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


        private static List<(Identifier, IAstType)> ParseArgList()
        {
            List<(Identifier, IAstType)> argList = new List<(Identifier, IAstType)>();
            if (CTokIs(typeof(RightParen)))
            {
                return argList;
            }
            while (CTokIs(typeof(Identifier)))
            {
                var argName = (Identifier) Current;
                Advance();
                if (CTokIs(typeof(Identifier)))
                {
                    argList.Add((argName, ParseType()));
                    if (!CTokMatches(typeof(Coma)))
                    {
                        if (CTokMatches(typeof(RightParen)))
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