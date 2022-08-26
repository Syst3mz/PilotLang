using System;
using PilotLang.Tokens;

namespace PilotLang
{
    public class ParseError : Exception
    {
        private int _linePos, _charPos;
        private string _message;

        public ParseError(IToken token, string message)
        {
            _linePos = token.LinePos;
            _charPos = token.charPos;
            _message = message;
        }

        public ParseError(int linePos, int charPos, string message)
        {
            _linePos = linePos;
            _charPos = charPos;
            _message = message;
        }

        public override string ToString()
        {
            return $"Error parsing at [{_linePos}:{_charPos}]: {_message}";
        }
    }
}