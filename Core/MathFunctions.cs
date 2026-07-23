using System;

namespace CalculadoraCientifica.Core
{
    public static class MathFunctions
    {
        public static double Factorial(double n)
        {
            if (n < 0 || n % 1 != 0)
                throw new ArgumentException("Fatorial só é suportado para inteiros não negativos.");

            double res = 1;
            for (int i = 2; i <= (int)n; i++)
                res *= i;

            return res;
        }

        public static double Apply(string name, double arg, AngleMode angleMode)
        {
            switch (name.ToLower())
            {
                case "sin":
                    if (angleMode == AngleMode.Degrees)
                        arg = arg * Math.PI / 180.0;
                    return Math.Sin(arg);

                case "cos":
                    if (angleMode == AngleMode.Degrees)
                        arg = arg * Math.PI / 180.0;
                    return Math.Cos(arg);

                case "tan":
                    if (angleMode == AngleMode.Degrees)
                        arg = arg * Math.PI / 180.0;
                    return Math.Tan(arg);

                case "ln":
                    if (arg <= 0)
                        throw new ArgumentException("Logaritmo exige argumento > 0.");
                    return Math.Log(arg);

                default:
                    throw new FormatException($"Função desconhecida: {name}");
            }
        }
    }
}