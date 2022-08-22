
using System;
using System.Collections.Generic;
using PilotLang.Tokens;

namespace PilotLang
{
    public class TokenTreeNode
    {
        public char Character { get; }
        public TokenType Token;
        private List<TokenTreeNode> _connections;
        
        public TokenTreeNode(char character)
        {
            Character = character;
            _connections = new List<TokenTreeNode>();
        }

        public bool IsTerminal {get => _connections.Count == 0;}

        public void BuildTree(string input, TokenType tok)
        {
            if (input.Length == 0)
            {
                Token = tok;
                return;
            }
            
            foreach (var node in _connections)
            {
                if (node.Character == input[0])
                {
                    node.BuildTree(input.Substring(1, input.Length - 1), tok);
                    return;
                }
            }
            
            var newNode = new TokenTreeNode(input[0]);
            newNode.BuildTree(input.Substring(1, input.Length - 1), tok);
            _connections.Add(newNode);
        }

        public TokenTreeNode Follow(char c)
        {
            string errMsg = "Unexpected character found. Expected one of ";
            foreach (var node in _connections)
            {
                if (node.Character == c)
                {
                    return node;
                }

                errMsg += $"\"{node.Character}\", ";
            }

            errMsg += "]";


            throw new Exception(errMsg);
        }
    }
}