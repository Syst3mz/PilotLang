namespace PilotLang
{
    public class ParseError
    {
        public int LinePos, CharPos;
        public string Message;

        public ParseError(int linePos, int charPos, string message)
        {
            LinePos = linePos;
            CharPos = charPos;
            Message = message;
        }

        public override string ToString()
        {
            return $"Error at {LinePos}:{CharPos}: {Message}";
        }
    }
}