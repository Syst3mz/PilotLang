using PilotLang;

namespace PilotInterpreter.Visitors
{
    public class ShittyPrintVisitor : IExprVisitor<string>, IStatementVisitor<string>, ITopLevelVisitor<string>, ITypeVisitor<string>
    {
        public string VisitIdentifier(IdentifierAstExpr ident)
        {
            return ident.Token.Text;
        }

        public string VisitInteger(IntegerAstExpr integer)
        {
            return integer.Token.Number.ToString();
        }

        public string VisitAssignment(AssignmentAstExpr assign)
        {
            return $"{assign.VarName.Text} {assign.Op} {this.VisitExpr(assign.VarValue)}";
        }

        public string VisitBinaryExpr(BinaryAstExpr astExpr)
        {
            return $"{this.VisitExpr(astExpr.Left)} {astExpr.Op} {this.VisitExpr(astExpr.Right)}";
        }

        public string VisitBlock(BlockAst block)
        {
            string ret = "{\n";
            foreach (var statement in block.Statements)
            {
                ret += this.VisitStatement(statement) + "\n";
            }

            return ret + "}";
        }

        public string VisitExprStatement(ExprStatement exprStmnt)
        {
            return this.VisitExpr(exprStmnt.Expr) + ";";
        }

        public string VisitWhileLoop(WhileLoopAstStatement loop)
        {
            return $"while {this.VisitExpr(loop.Check)} {VisitBlock(loop.Block)}";
        }

        public string VisitVariableDeclaration(VariableDeclarationAstStatement v)
        {
            if (v.VarValue != null)
            {
                return $"var {this.VisitType(v.Type)} {v.VarName.Text} = {this.VisitExpr(v.VarValue)};";
            }
            return $"var {this.VisitType(v.Type)} {v.VarName.Text};";
        }

        public string VisitReturnStatement(ReturnAstStatement r)
        {
            return $"return {this.VisitExpr(r.Expr)};";
        }

        public string VisitFunction(FunctionAstPart func)
        {
            string funcArgs = "";
            if (func.Arguments.Count > 0)
            {
                foreach (var (name, astType) in func.Arguments)
                {
                    funcArgs += this.VisitType(astType);
                }

                funcArgs = funcArgs.Substring(0, funcArgs.Length - 2);
            }
            return $"fn {this.VisitType(func.ReturnType)} {func.FunctionName.Text}({funcArgs}) {VisitBlock(func.FuncBody)}";
        }

        public string VisitSimpleType(SimpleType st)
        {
            return st.TypeName.Text;
        }

        public string VisitArrayType(ArrayType at)
        {
            return this.VisitType(at.InternalType) + "[]";
        }

        public string VisitGenericType(GenericType gt)
        {
            string typeArgs = "";
            foreach (var t in gt.TypeArguments)
            {
                typeArgs += this.VisitType(t) +", ";
            }

            typeArgs = typeArgs.Substring(0, typeArgs.Length - 2);
            return this.VisitType(gt.TypeName) + "<" + typeArgs + ">";
        }
    }
}