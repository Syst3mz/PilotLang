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
      EndOfPhrase,
      [StringRepresentation("return")]
      Return,
      [StringRepresentation(".")]
      Dot,
      [StringRepresentation(",")]
      Coma,
      Identifier,
      Integer
   }

   public interface IToken
   {
      public TokenType Type { get; }
   }

   public struct StaticToken : IToken
   {
      public TokenType Type { get; }

      public StaticToken(TokenType type)
      {
         Type = type;
      }

      public override string ToString()
      {
         return Type.ToString();
      }
   }

   public struct TextToken : IToken
   {
      public TokenType Type { get; }
      public string Text;

      public TextToken(TokenType type, string text)
      {
         Type = type;
         Text = text;
      }

      public override string ToString()
      {
         return $"{Type}: {Text}";
      }
   }

   public struct IntegerToken : IToken
   {
      public TokenType Type { get; }
      public int Number;

      public IntegerToken(TokenType type, int number)
      {
         Number = number;
         Type = type;
         
      }
      
      public override string ToString()
      {
         return $"{Type}: {Number}";
      }
   }
}