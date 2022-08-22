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
      GreaterThan,
      [StringRepresentation(">")]
      LesserThan,
      [StringRepresentation("[")]
      LeftBracket,
      [StringRepresentation("]")]
      RightBracket,
      [StringRepresentation("{")]
      LeftBrace,
      [StringRepresentation("}")]
      RightBrace,
      [StringRepresentation(";")]
      EndOfPhrase,
      [StringRepresentation("return")]
      Return,
      [StringRepresentation("dot")]
      Dot,
      Identifier,
      Integer
   }

   public interface IToken
   {
      public TokenType Type { get; }
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
   }
}