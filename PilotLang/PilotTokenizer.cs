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
        public static List<IToken> Tokenize(FileStream fs)
        {
            // Set up return var
            List<IToken> ret = new List<IToken>();
            
            // Use reflection to go grab the the simple tokens
            var simpleTokenTypes = GetSimpleTokenTypes();
            
            // Load them into a dict for easy access
            Dictionary<string, Type> simpleLookup = new Dictionary<string, Type>();
            string eofStr = ";";
            foreach (Type simpleTokenType in simpleTokenTypes)
            {
                string id = simpleTokenType.GetCustomAttribute<StringTokenIdentifier>()!.Id;
                simpleLookup.Add(id, simpleTokenType);
            }


            // Get ready to read from file, and set up input buffer
            StreamReader sr = new StreamReader(fs);
            string inputBuffer = "";
            
            // While there are characters left to read
            while (!sr.EndOfStream)
            {
                // Get the next character, and if it is whitespace it is the end of a block like thing,
                // if it isn't white space check if its a token char, and if it is check if inputBuf is a token,
                // otherwise, add it to inputBuf
                char nextChar = (char)sr.Read();
                if (!char.IsWhiteSpace(nextChar))
                {
                    if (simpleLookup.ContainsKey(nextChar + ""))
                    {
                        if (inputBuffer.Trim().Length > 0)
                        {
                            ret.Add(TokenizeString(inputBuffer, simpleLookup));
                            inputBuffer = "";
                        }
                        
                        ret.Add((IToken) Activator.CreateInstance(simpleLookup[nextChar + ""]));
                    }
                    else
                    {
                        inputBuffer += nextChar;
                    }
                }
                else
                {
                    if (inputBuffer.Trim().Length > 0)
                    {
                        ret.Add(TokenizeString(inputBuffer, simpleLookup));
                        inputBuffer = "";
                    }
                }
            }
            
            
            return ret;
        }

        /// <summary>
        ///  Take a multi character string and turn it into a token.
        /// </summary>
        /// <param name="inputBuffer">The string to look at</param>
        /// <param name="simpleLookup">A dictionary of strings to static tokens (;,. and the like)</param>
        /// <returns>The generated token</returns>
        /// <exception cref="ArgumentException">You fucked up big time</exception>
        private static IToken TokenizeString(string inputBuffer, Dictionary<string, Type> simpleLookup)
        {
            string lbuf = inputBuffer.Trim().ToLower();

            if (lbuf.Length > 0)
            {
                //todo make everything below me less retarded
                IToken output;
                if (simpleLookup.ContainsKey(lbuf))
                {
                    output = (IToken)Activator.CreateInstance(simpleLookup[lbuf]);
                } else if (char.IsNumber(lbuf[0]))
                {
                    output = new Integer(int.Parse(lbuf));
                }
                else
                {
                    output = new Identifier(inputBuffer.Trim());
                }

                return output;
            }
            // If we got here, we fucked up
            throw new ArgumentException($"\"{inputBuffer}\" Cannot be parsed or understood");
        }
    
        
        // Look at all of the classes in the PilotLang Namespace and return all which have a StringTokenIdentifier.
        // Edit: I hate linq
        private static List<Type> GetSimpleTokenTypes()
        {
            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                where t.Namespace == "PilotLang.Tokens" && t.GetCustomAttribute<StringTokenIdentifier>() != null
                select t;
            return q.ToList();
        } 
    }
}