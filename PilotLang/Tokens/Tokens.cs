using System;

namespace PilotLang.Tokens
{

   public class StringRepresentationAttribute : Attribute
   {
      public string Name;

      public StringRepresentationAttribute(string name)
      {
         Name = name;
      }
   }

   public enum TokenType
   {
      [StringRepresentation("fn")]
      Function,
      [StringRepresentation("<")]
      LesserThan,
      [StringRepresentation("=")]
      Assign,
      [StringRepresentation(">")]
      GreaterThan,
      LesserThanOrEqualTo,
      GreaterThanOrEqualTo,
      [StringRepresentation("[")]
      LeftBracket,
      [StringRepresentation("]")]
      RightBracket,
      [StringRepresentation("{")]
      LeftBrace,
      [StringRepresentation("}")]
      RightBrace,
      [StringRepresentation("(")]
      LeftParentheses,
      [StringRepresentation(")")]
      RightParentheses,
      [StringRepresentation(";")]
      Semicolon,
      [StringRepresentation("return")]
      Return,
      [StringRepresentation(".")]
      Dot,
      [StringRepresentation(",")]
      Comma,
      [StringRepresentation("+")]
      Plus,
      [StringRepresentation("-")]
      Minus,
      [StringRepresentation("*")]
      Multiply,
      [StringRepresentation("/")]
      Divide,
      [StringRepresentation("&&")]
      And,
      [StringRepresentation("||")]
      Or,
      [StringRepresentation("for")]
      For,
      [StringRepresentation("while")]
      While,
      [StringRepresentation("!")]
      ExclamationMark,
      [StringRepresentation("in")]
      In,
      [StringRepresentation(":")]
      Colon,
      [StringRepresentation("++")]
      Increment,
      [StringRepresentation("--")]
      Decrement,
      [StringRepresentation("var")]
      VariableKeyword,
      [StringRepresentation("?")]
      QuestionMark,
      Identifier,
      Integer
   }

   public interface IToken
   {
      public TokenType Type { get; }
      public int LinePos { get; }
      public int charPos { get; }
   }

   public struct StaticToken : IToken
   {
      public TokenType Type { get; }
      public int LinePos { get; }
      public int charPos { get; }

      public StaticToken(TokenType type, int linePos, int charPos)
      {
         Type = type;
         LinePos = linePos;
         this.charPos = charPos;
      }

      public override string ToString()
      {
         return Type.ToString();
      }
   }

   public struct IdentifierToken : IToken
   {
      public TokenType Type { get; }
      public int LinePos { get; }
      public int charPos { get; }
      public string Text;

      public IdentifierToken(TokenType type,string text, int linePos, int charPos)
      {
         Text = text;
         Type = type;
         LinePos = linePos;
         this.charPos = charPos;
      }

      public override string ToString()
      {
         return $"{Type}: {Text}";
      }
   }

   public struct IntegerToken : IToken
   {
      public TokenType Type { get; }
      public int LinePos { get; }
      public int charPos { get; }
      public int Number;

      public IntegerToken(TokenType type, int number, int linePos, int charPos)
      {
         Number = number;
         Type = type;
         LinePos = linePos;
         this.charPos = charPos;
      }


      public override string ToString()
      {
         return $"{Type}: {Number}";
      }
   }
}