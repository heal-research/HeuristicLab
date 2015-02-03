#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public static class ValueGenerator {
    private static FastRandom rand = new FastRandom();

    /// <summary>
    /// Generates a sequence of evenly spaced points by returning the start value and adding the stepwidth until the end is reached or surpassed.
    /// 
    /// </summary>
    /// <param name="start">The smallest and first value of the sequence.</param>
    /// <param name="end">The largest and last value of the sequence.</param>
    /// <param name="stepWidth">The step size between subsequent values.</param>
    /// <param name="includeEnd">Determines if the end should be included in the sequence regardless if the end is divisible by the stepwidth.</param>
    /// <returns>A sequence of values from start to end (inclusive)</returns>
    [Obsolete("It is recommended to use the decimal overload to achieve a higher numerical accuracy.")] 
    public static IEnumerable<double> GenerateSteps(double start, double end, double stepWidth, bool includeEnd = false) {
      //mkommend: IEnumerable.Cast fails due to boxing and unboxing of the involved types
      // http://referencesource.microsoft.com/#System.Core/System/Linq/Enumerable.cs#27bb217a6d5457ec
      // http://blogs.msdn.com/b/ericlippert/archive/2009/03/19/representation-and-identity.aspx     

      return GenerateSteps((decimal)start, (decimal)end, (decimal)stepWidth, includeEnd).Select(x => (double)x);
    }

    /// <summary>
    /// Generates a sequence of evenly spaced points by returning the start value and adding the stepwidth until the end is reached or surpassed.
    /// </summary>
    /// <param name="start">The smallest and first value of the sequence.</param>
    /// <param name="end">The largest and last value of the sequence.</param>
    /// <param name="stepWidth">The step size between subsequent values.</param>
    /// /// <param name="includeEnd">Determines if the end should be included in the sequence regardless if the end is divisible by the stepwidth.</param>
    /// <returns>A sequence of values from start to end</returns>
    public static IEnumerable<decimal> GenerateSteps(decimal start, decimal end, decimal stepWidth, bool includeEnd = false) {
      if (stepWidth == 0)
        throw new ArgumentException("The step width cannot be zero.");
      if (start < end && stepWidth < 0)
        throw new ArgumentException("The step width must be larger than zero for increasing sequences (start < end).");
      if (start > end && stepWidth > 0)
        throw new ArgumentException("The step width must be smaller than zero for decreasing sequences (start > end).");

      decimal x = start;
      while (x <= end) {
        yield return x;
        x += stepWidth;
      }
      if (x - stepWidth < end && includeEnd) yield return end;
    }

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

    // iterative approach
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
