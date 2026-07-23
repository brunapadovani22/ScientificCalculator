using System;
using System.Collections.Generic;

namespace CalculadoraCientifica.Core
{
    public enum TokenType
    {
        Number,
        Operator,
        Function,
        LeftParen,
        RightParen,
        Factorial
    }

    public class Token(TokenType type, string text, double value = 0)
    {
        public TokenType Type { get; } = type;
        public string Text { get; } = text;
        public double Value { get; } = value;
    }

    public static class Tokenizer
    {
        public static List<Token> Tokenize(string expression)
        {
            var tokens = new List<Token>();
            int i = 0;

            while (i < expression.Length)
            {
                char c = expression[i];

                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                if (char.IsDigit(c) || c == ',')
                {
                    string numStr = "";
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == ','))
                    {
                        numStr += expression[i];
                        i++;
                    }
                    double val = double.Parse(numStr.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                    tokens.Add(new Token(TokenType.Number, numStr, val));
                    continue;
                }

                if (char.IsLetter(c))
                {
                    string funcStr = "";
                    while (i < expression.Length && char.IsLetter(expression[i]))
                    {
                        funcStr += expression[i];
                        i++;
                    }
                    tokens.Add(new Token(TokenType.Function, funcStr));
                    continue;
                }

                if (c == '+' || c == '-' || c == '*' || c == '/' || c == '^')
                {
                    tokens.Add(new Token(TokenType.Operator, c.ToString()));
                    i++;
                    continue;
                }

                if (c == '(')
                {
                    tokens.Add(new Token(TokenType.LeftParen, "("));
                    i++;
                    continue;
                }

                if (c == ')')
                {
                    tokens.Add(new Token(TokenType.RightParen, ")"));
                    i++;
                    continue;
                }

                if (c == '!')
                {
                    tokens.Add(new Token(TokenType.Factorial, "!"));
                    i++;
                    continue;
                }

                throw new FormatException($"Caractere inválido na expressão: '{c}'");
            }

            return tokens;
        }
    }
}