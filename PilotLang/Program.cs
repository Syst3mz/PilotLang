using System;
using System.IO;
using PilotLang.Tokens;

namespace PilotLang
{
    class Program
    {
        static void Main(string[] args)
        {

            var toks = PilotTokenizer.Tokenize(File.OpenRead("test.pil"));
            //var ast = PilotAst.BuildAbstractSyntaxTree(toks);
            return;
        }
    }
}
