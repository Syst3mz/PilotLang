using System;
using System.IO;
using PilotInterpreter;
using PilotInterpreter.Visitors;
using PilotLang.Tokens;

namespace PilotLang
{
    class Program
    {
        static void Main(string[] args)
        {
            var toks = PilotTokenizer.Tokenize(File.OpenRead("test.pil"));
            var ast = PilotAst.BuildAbstractSyntaxTree(toks);
            ShittyPrintVisitor pv = new ShittyPrintVisitor();
            foreach (var topLevel in ast)
            {
                Console.WriteLine(pv.VisitTopLevel(topLevel));
            }

            Interpreter i = new Interpreter();
            foreach (IAstPart part in ast)
            {
                i.VisitTopLevel(part);
            }
            return;
        }
    }
}
