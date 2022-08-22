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
        
        public static IEnumerable<IToken> Tokenize(FileStream fs)
        {
            // Use reflection to go grab the the simple tokens
            GetSimpleLookupTable();
            
            StreamReader sr = new StreamReader(fs);
            string inputBuffer = "";
            while (!sr.EndOfStream)
            {
                char nextChar = (char)sr.Read();
                if (!char.IsWhiteSpace(nextChar))
                {
                    if (Compare(TokenType.EndOfPhrase, nextChar))
                    {
                        
                    }
                }
            }
        }

        private static bool Compare(TokenType t, char c)
        {
            return _backward[TokenType.EndOfPhrase] == "" + c;
        }
    }
}