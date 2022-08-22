using System;

namespace PilotLang.Tokens
{
    public class StringTokenIdentifier : Attribute
    {
        public string Id;

        public StringTokenIdentifier(string id)
        {
            Id = id;
        }
    }
    
    public interface IToken
    {
        
    }

    // Simple Tokens
    [StringTokenIdentifier("{")]
    public struct LeftBrace : IToken
    {
            
    }
    [StringTokenIdentifier("}")]
    public struct RightBrace : IToken
    {
            
    }
    [StringTokenIdentifier(";")]
    public struct EndOfPhrase : IToken
    {
            
    }
    [StringTokenIdentifier(",")]
    public struct Coma : IToken
    {
            
    }
    [StringTokenIdentifier(".")]
    public struct Dot : IToken
    {
            
    }
    [StringTokenIdentifier("(")]
    public struct LeftParen : IToken
    {
            
    }
    [StringTokenIdentifier(")")]
    public struct RightParen : IToken
    {
            
    }
    [StringTokenIdentifier("[")]
    public struct LeftBracket : IToken
    {
            
    }
    [StringTokenIdentifier("]")]
    public struct RightBracket : IToken
    {
            
    }
    [StringTokenIdentifier("+")]
    public struct Plus : IToken
    {
            
    }
    [StringTokenIdentifier("-")]
    public struct Minus : IToken
    {
            
    }
    [StringTokenIdentifier("/")]
    public struct Devide : IToken
    {
            
    }
    [StringTokenIdentifier("*")]
    public struct Multiply : IToken
    {
            
    }
        
    [StringTokenIdentifier("=")]
    public struct Equals : IToken
    {
            
    }
        
    [StringTokenIdentifier(">")]
    public struct GreaterThan : IToken
    {
            
    }
        
    [StringTokenIdentifier("<")]
    public struct LesserThan : IToken
    {
            
    }
        
    // Keyword Tokens
    [StringTokenIdentifier("fn")]
    public struct Function : IToken
    {
            
    }
    [StringTokenIdentifier("if")]
    public struct If : IToken
    {
            
    }
    [StringTokenIdentifier("else")]
    public struct Else : IToken
    {
            
    }
    [StringTokenIdentifier("return")]
    public struct Return : IToken
    {
            
    }
    [StringTokenIdentifier("for")]
    public struct For : IToken
    {
            
    }
    [StringTokenIdentifier("while")]
    public struct While : IToken
    {
            
    }
    [StringTokenIdentifier("classdef")]
    public struct ClassDefinition : IToken
    {
            
    }

        

    // Dynamic Tokens
    public struct Identifier : IToken
    {
        public string Text;

        public Identifier(string text)
        {
            Text = text;
        }
    }
    public struct Integer : IToken
    {
        public int Value;

        public Integer(int value)
        {
            Value = value;
        }
    }
}