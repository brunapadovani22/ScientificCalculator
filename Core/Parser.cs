using System;
using System.Collections.Generic;

namespace CalculadoraCientifica.Core
{
    public enum AngleMode
    {
        Degrees,
        Radians
    }

    public sealed class Parser
    {
        private readonly List<Token> tokens;
        private readonly AngleMode angleMode;
        private int position;

        private Parser(List<Token> tokens, AngleMode angleMode)
        {
            this.tokens = tokens;
            this.angleMode = angleMode;
        }

        public static double Evaluate(string expression, AngleMode angleMode)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new FormatException("Expressão vazia.");

            Parser parser = new(Tokenizer.Tokenize(expression), angleMode);
            double result = parser.ParseExpression();

            if (!parser.IsAtEnd)
                throw new FormatException($"Símbolo inesperado: '{parser.Current.Text}'.");

            if (double.IsNaN(result) || double.IsInfinity(result))
                throw new ArithmeticException("O resultado não é um número válido.");

            return result;
        }

        private Token Current => tokens[position];
        private bool IsAtEnd => position >= tokens.Count;

        private bool Check(TokenType type)
        {
            return !IsAtEnd && Current.Type == type;
        }

        private bool CheckOperator(string symbol)
        {
            return !IsAtEnd &&
                   Current.Type == TokenType.Operator &&
                   Current.Text == symbol;
        }

        private Token Advance()
        {
            return tokens[position++];
        }

        private double ParseExpression()
        {
            double value = ParseTerm();

            while (CheckOperator("+") || CheckOperator("-"))
            {
                string operation = Advance().Text;
                double rightValue = ParseTerm();

                value = operation == "+"
                    ? value + rightValue
                    : value - rightValue;
            }

            return value;
        }

        private double ParseTerm()
        {
            double value = ParseUnary();

            while (CheckOperator("*") || CheckOperator("/"))
            {
                string operation = Advance().Text;
                double rightValue = ParseUnary();

                if (operation == "/")
                {
                    if (rightValue == 0)
                        throw new DivideByZeroException("Divisão por zero.");

                    value /= rightValue;
                }
                else
                {
                    value *= rightValue;
                }
            }

            return value;
        }

        private double ParseUnary()
        {
            if (CheckOperator("-"))
            {
                Advance();
                return -ParseUnary();
            }

            return ParsePower();
        }

        private double ParsePower()
        {
            double baseValue = ParsePostfix();

            if (CheckOperator("^"))
            {
                Advance();
                double exponent = ParseUnary();

                return Math.Pow(baseValue, exponent);
            }

            return baseValue;
        }

        private double ParsePostfix()
        {
            double value = ParsePrimary();

            while (Check(TokenType.Factorial))
            {
                Advance();
                value = MathFunctions.Factorial(value);
            }

            return value;
        }

        private double ParsePrimary()
        {
            if (Check(TokenType.Number))
                return Advance().Value;

            if (Check(TokenType.Function))
            {
                string functionName = Advance().Text;

                Expect(TokenType.LeftParen, "'('");
                double argument = ParseExpression();
                Expect(TokenType.RightParen, "')'");

                return MathFunctions.Apply(functionName, argument, angleMode);
            }

            if (Check(TokenType.LeftParen))
            {
                Advance();

                double value = ParseExpression();

                Expect(TokenType.RightParen, "')'");
                return value;
            }

            throw new FormatException(
                IsAtEnd
                    ? "Expressão incompleta."
                    : $"Símbolo inesperado: '{Current.Text}'.");
        }

        private void Expect(TokenType type, string description)
        {
            if (!Check(type))
                throw new FormatException($"Era esperado {description}.");

            Advance();
        }
    }
}