using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculadoraCientifica.Core
{
    public static class StatisticsEngine
    {
        public static double Mean(IEnumerable<double> values)
        {
            var list = values.ToList();
            if (list.Count == 0)
                throw new ArgumentException("A lista de valores não pode estar vazia.");

            return list.Average();
        }

        public static double Median(IEnumerable<double> values)
        {
            var sorted = values.OrderBy(n => n).ToList();
            if (sorted.Count == 0)
                throw new ArgumentException("A lista de valores não pode estar vazia.");

            int mid = sorted.Count / 2;
            return sorted.Count % 2 != 0
                ? sorted[mid]
                : (sorted[mid - 1] + sorted[mid]) / 2.0;
        }
    }
}