using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PilotLang.Tokens;

namespace PilotLang
{
    public static class PilotTokenizer
    {
        private static Dictionary<string, TokenType> _forward;
        private static Dictionary<TokenType, string> _backward;
        private static StreamReader _rawStream;
        private static int _linePos = 1, _charPos = 1;
        private static void GetSimpleLookupTable()
        {
            var enumType = typeof(TokenType);
            var enumValues = enumType.GetEnumValues();
            _forward = new Dictionary<string, TokenType>();
            _backward = new Dictionary<TokenType, string>();
            foreach (var value in enumValues)    
            {    
                // with our Type object we can get the information about    
                // the members of it    
                MemberInfo memberInfo =    
                    enumType.GetMember(value.ToString()).First();    
     
                // we can then attempt to retrieve the    
                // description attribute from the member info    
                var stringRepresentationAttribute =    
                    memberInfo.GetCustomAttribute<StringRepresentationAttribute>();
     
                // if we find the attribute we can access its values    
                if (stringRepresentationAttribute != null)    
                {    
                    _forward.Add(stringRepresentationAttribute.Name, (TokenType) value);
                    _backward.Add((TokenType) value, stringRepresentationAttribute.Name);
                }
            }
        }
        
        private static string _inputBuffer = "";
        
        public static IEnumerable<IToken> Tokenize(FileStream fs)
        {
            // Use reflection to go grab the the simple tokens
            GetSimpleLookupTable();

            _rawStream = new StreamReader(fs);
            while (!_rawStream.EndOfStream)
            {
                var nextChar = GetNextChar();
                _charPos++;
                if (nextChar == '\n')
                {
                    _linePos++;
                    _charPos = 1;
                }
                if (!char.IsWhiteSpace(nextChar))
                {
                    if (Compare(TokenType.LesserThan, nextChar))
                    {
                        if (_inputBuffer.Length > 0)
                        {
                            yield return TokenizeInputBuffer();
                        }
                        if (MatchNextChar(TokenType.SingleEquals))
                        {
                            yield return new StaticToken(TokenType.LesserThanOrEqualTo, _linePos, _charPos);
                        }
                        else
                        {
                            yield return new StaticToken(TokenType.LesserThan, _linePos, _charPos);
                        }
                    }
                    else if (Compare(TokenType.GreaterThan, nextChar))
                    {
                        if (_inputBuffer.Length > 0)
                        {
                            yield return TokenizeInputBuffer();
                        }
                        if (MatchNextChar(TokenType.SingleEquals))
                        {
                            yield return new StaticToken(TokenType.GreaterThanOrEqualTo, _linePos, _charPos);
                        }
                        else
                        {
                            yield return new StaticToken(TokenType.GreaterThan, _linePos, _charPos);
                        }
                    }
                    else if (Compare(TokenType.Plus, nextChar))
                    {
                        if (_inputBuffer.Length > 0)
                        {
                            yield return TokenizeInputBuffer();
                        }
                        if (MatchNextChar(TokenType.Plus))
                        {
                            yield return new StaticToken(TokenType.Increment, _linePos, _charPos);
                        }
                        else
                        {
                            yield return new StaticToken(TokenType.Plus, _linePos, _charPos);
                        }
                    }
                    else if (Compare(TokenType.Minus, nextChar))
                    {
                        if (_inputBuffer.Length > 0)
                        {
                            yield return TokenizeInputBuffer();
                        }
                        if (MatchNextChar(TokenType.Minus))
                        {
                            yield return new StaticToken(TokenType.Decrement, _linePos, _charPos);
                        }
                        else
                        {
                            yield return new StaticToken(TokenType.Minus, _linePos, _charPos);
                        }
                    }
                    else if (Compare(TokenType.SingleEquals, nextChar))
                    {
                        if (_inputBuffer.Length > 0)
                        {
                            yield return TokenizeInputBuffer();
                        }
                        if (MatchNextChar(TokenType.SingleEquals))
                        {
                            yield return new StaticToken(TokenType.EqualsEquals, _linePos, _charPos);
                        }
                        else
                        {
                            yield return new StaticToken(TokenType.SingleEquals, _linePos, _charPos);
                        }
                    }
                    else if (Compare(TokenType.ExclamationMark, nextChar))
                    {
                        if (_inputBuffer.Length > 0)
                        {
                            yield return TokenizeInputBuffer();
                        }
                        if (MatchNextChar(TokenType.SingleEquals))
                        {
                            yield return new StaticToken(TokenType.ExclamationEquals, _linePos, _charPos);
                        }
                        else
                        {
                            yield return new StaticToken(TokenType.ExclamationMark, _linePos, _charPos);
                        }
                    }
                    else if (_forward.ContainsKey(nextChar + ""))
                    {
                        if (_inputBuffer.Length > 0)
                        {
                            yield return TokenizeInputBuffer();
                        }
                        yield return new StaticToken(_forward[nextChar+""], _linePos, _charPos);
                    }
                    else
                    {
                        _inputBuffer += nextChar;
                    }
                }
                else
                {
                    if (_inputBuffer.Length > 0)
                    {
                        yield return TokenizeInputBuffer();
                    }
                }
            }

            if (_inputBuffer.Length > 0)
            {
                yield return TokenizeInputBuffer();
            }
        }

        private static IToken TokenizeInputBuffer()
        {
            if (_inputBuffer.Length > 0)
            {
                var ret = TokenizeString(_inputBuffer);
                _inputBuffer = "";
                return ret;
            }

            return null;
        }

        private static IToken TokenizeString(string input)
        {
            if (input.Length < 1)
            {
                throw new ArgumentException($"Input to {nameof(TokenizeString)} must contain some characters");
            }
            IToken ret;
            if (_forward.ContainsKey(input))
            {
                ret = new StaticToken(_forward[input], _linePos, _charPos);
            }else if (char.IsDigit(input[0]))
            {
                ret = new IntegerToken(TokenType.Integer, int.Parse(input), _linePos, _charPos);
            }
            else
            {
                ret = new IdentifierToken(TokenType.Identifier, input, _linePos, _charPos);
            }
            
            return ret;
        }

        private static char GetNextChar()
        {
            return (char)_rawStream.Read();
        }
        
        private static char ViewNextChar()
        {
            return (char)_rawStream.Peek();
        }

        private static bool MatchNextChar(char c)
        {
            if (ViewNextChar() == c)
            {
                GetNextChar();
                return true;
            }

            return false;
        }
        private static bool MatchNextChar(TokenType t)
        {
            if (ViewNextChar() == _backward[t][0])
            {
                GetNextChar();
                return true;
            }

            return false;
        }

        private static bool Compare(TokenType t, char c)
        {
            return _backward[t] == "" + c;
        }
    }
}