#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  internal static class ValueGenerator {
    private static FastRandom rand = new FastRandom();

    /// <summary>
    /// Generates uniformly distributed values between start and end (inclusive!) 
    /// </summary>
    /// <param name="n">Number of values to generate.</param>
    /// <param name="start">The lower value (inclusive)</param>
    /// <param name="end">The upper value (inclusive)</param>
    /// <returns>An enumerable including n values in [start, end]</returns>
    public static IEnumerable<double> GenerateUniformDistributedValues(int n, double start, double end) {
      for (int i = 0; i < n; i++) {
        // we need to return a random value including end.
        // so we cannot use rand.NextDouble() as it returns a value strictly smaller than 1.
        double r = rand.NextUInt() / (double)uint.MaxValue;    // r \in [0,1]
        yield return r * (end - start) + start;
      }
    }

    /// <summary>
    /// Generates normally distributed values sampling from N(mu, sigma) 
    /// </summary>
    /// <param name="n">Number of values to generate.</param>
    /// <param name="mu">The mu parameter of the normal distribution</param>
    /// <param name="sigma">The sigma parameter of the normal distribution</param>
    /// <returns>An enumerable including n values ~ N(mu, sigma)</returns>
    public static IEnumerable<double> GenerateNormalDistributedValues(int n, double mu, double sigma) {
      for (int i = 0; i < n; i++)
        yield return NormalDistributedRandom.NextDouble(rand, mu, sigma);
    }

    // Generate the cartesian product.
    // The result is transposed, therefore the inner lists represent a column of values instead of a combination-pair.
    public static IEnumerable<IEnumerable<double>> GenerateAllCombinationsOfValuesInLists(List<List<double>> lists) {
      List<List<double>> allCombinations = new List<List<double>>();
      if (lists.Count < 1) {
        return allCombinations;
      }

      List<IEnumerator<double>> enumerators = new List<IEnumerator<double>>();
      foreach (var list in lists) {
        allCombinations.Add(new List<double>());
        enumerators.Add(list.GetEnumerator());
      }

      bool finished = !enumerators.All(x => x.MoveNext());

      while (!finished) {
        GetCurrentCombination(enumerators, allCombinations);
        finished = MoveNext(enumerators, lists);
      }
      return allCombinations;
    }

    private static bool MoveNext(List<IEnumerator<double>> enumerators, List<List<double>> lists) {
      int cur = enumerators.Count - 1;
      while (cur >= 0 && !enumerators[cur].MoveNext()) {
        enumerators[cur] = lists[cur].GetEnumerator();
        enumerators[cur].MoveNext();
        cur--;
      }
      return cur < 0;
    }

    private static void GetCurrentCombination(List<IEnumerator<double>> enumerators, List<List<double>> allCombinations) {
      for (int i = 0; i < enumerators.Count(); i++) {
        allCombinations[i].Add(enumerators[i].Current);
      }
    }
  }
}
