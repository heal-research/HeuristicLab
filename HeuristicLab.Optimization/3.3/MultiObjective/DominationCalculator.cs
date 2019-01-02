#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Optimization {
  [StorableType("d76eb753-5088-4490-ad18-e78d3629c60b")]
  public enum DominationResult { Dominates, IsDominated, IsNonDominated };

  public static class DominationCalculator<T> {
    /// <summary>
    /// Calculates the best pareto front only. The fast non-dominated sorting algorithm is used
    /// as described in Deb, K., Pratap, A., Agarwal, S., and Meyarivan, T. (2002).
    /// A Fast and Elitist Multiobjective Genetic Algorithm: NSGA-II.
    /// IEEE Transactions on Evolutionary Computation, 6(2), 182-197.
    /// </summary>
    /// <remarks>
    /// When there are plateaus in the fitness landscape several solutions might have exactly
    /// the same fitness vector. In this case parameter <paramref name="dominateOnEqualQualities"/>
    /// can be set to true to avoid plateaus becoming too attractive for the search process.
    /// </remarks>
    /// <param name="solutions">The solutions of the population.</param>
    /// <param name="qualities">The qualities resp. fitness for each solution.</param>
    /// <param name="maximization">The objective in each dimension.</param>
    /// <param name="dominateOnEqualQualities">Whether solutions of exactly equal quality should dominate one another.</param>
    /// <returns>The pareto front containing the best solutions and their associated quality resp. fitness.</returns>
    public static List<Tuple<T, double[]>> CalculateBestParetoFront(T[] solutions, double[][] qualities, bool[] maximization, bool dominateOnEqualQualities = true) {
      int populationSize = solutions.Length;

      Dictionary<T, List<int>> dominatedIndividuals;
      int[] dominationCounter, rank;
      return CalculateBestFront(solutions, qualities, maximization, dominateOnEqualQualities, populationSize, out dominatedIndividuals, out dominationCounter, out rank);
    }

    /// <summary>
    /// Calculates all pareto fronts. The first in the list is the best front.
    /// The fast non-dominated sorting algorithm is used as described in
    /// Deb, K., Pratap, A., Agarwal, S., and Meyarivan, T. (2002).
    /// A Fast and Elitist Multiobjective Genetic Algorithm: NSGA-II.
    /// IEEE Transactions on Evolutionary Computation, 6(2), 182-197.
    /// </summary>
    /// <remarks>
    /// When there are plateaus in the fitness landscape several solutions might have exactly
    /// the same fitness vector. In this case parameter <paramref name="dominateOnEqualQualities"/>
    /// can be set to true to avoid plateaus becoming too attractive for the search process.
    /// </remarks>
    /// <param name="solutions">The solutions of the population.</param>
    /// <param name="qualities">The qualities resp. fitness for each solution.</param>
    /// <param name="maximization">The objective in each dimension.</param>
    /// <param name="rank">The rank of each of the solutions, corresponds to the front it is put in.</param>
    /// <param name="dominateOnEqualQualities">Whether solutions of exactly equal quality should dominate one another.</param>
    /// <returns>A sorted list of the pareto fronts from best to worst.</returns>
    public static List<List<Tuple<T, double[]>>> CalculateAllParetoFronts(T[] solutions, double[][] qualities, bool[] maximization, out int[] rank, bool dominateOnEqualQualities = true) {
      int populationSize = solutions.Length;

      Dictionary<T, List<int>> dominatedIndividuals;
      int[] dominationCounter;
      var fronts = new List<List<Tuple<T, double[]>>>();
      fronts.Add(CalculateBestFront(solutions, qualities, maximization, dominateOnEqualQualities, populationSize, out dominatedIndividuals, out dominationCounter, out rank));
      int i = 0;
      while (i < fronts.Count && fronts[i].Count > 0) {
        var nextFront = new List<Tuple<T, double[]>>();
        foreach (var p in fronts[i]) {
          List<int> dominatedIndividualsByp;
          if (dominatedIndividuals.TryGetValue(p.Item1, out dominatedIndividualsByp)) {
            for (int k = 0; k < dominatedIndividualsByp.Count; k++) {
              int dominatedIndividual = dominatedIndividualsByp[k];
              dominationCounter[dominatedIndividual] -= 1;
              if (dominationCounter[dominatedIndividual] == 0) {
                rank[dominatedIndividual] = i + 1;
                nextFront.Add(Tuple.Create(solutions[dominatedIndividual], qualities[dominatedIndividual]));
              }
            }
          }
        }
        i += 1;
        fronts.Add(nextFront);
      }
      return fronts;
    }

    private static List<Tuple<T, double[]>> CalculateBestFront(T[] solutions, double[][] qualities, bool[] maximization, bool dominateOnEqualQualities, int populationSize, out Dictionary<T, List<int>> dominatedIndividuals, out int[] dominationCounter, out int[] rank) {
      var front = new List<Tuple<T, double[]>>();
      dominatedIndividuals = new Dictionary<T, List<int>>();
      dominationCounter = new int[populationSize];
      rank = new int[populationSize];
      for (int pI = 0; pI < populationSize - 1; pI++) {
        var p = solutions[pI];
        List<int> dominatedIndividualsByp;
        if (!dominatedIndividuals.TryGetValue(p, out dominatedIndividualsByp))
          dominatedIndividuals[p] = dominatedIndividualsByp = new List<int>();
        for (int qI = pI + 1; qI < populationSize; qI++) {
          var test = Dominates(qualities[pI], qualities[qI], maximization, dominateOnEqualQualities);
          if (test == DominationResult.Dominates) {
            dominatedIndividualsByp.Add(qI);
            dominationCounter[qI] += 1;
          } else if (test == DominationResult.IsDominated) {
            dominationCounter[pI] += 1;
            if (!dominatedIndividuals.ContainsKey(solutions[qI]))
              dominatedIndividuals.Add(solutions[qI], new List<int>());
            dominatedIndividuals[solutions[qI]].Add(pI);
          }
          if (pI == populationSize - 2
            && qI == populationSize - 1
            && dominationCounter[qI] == 0) {
            rank[qI] = 0;
            front.Add(Tuple.Create(solutions[qI], qualities[qI]));
          }
        }
        if (dominationCounter[pI] == 0) {
          rank[pI] = 0;
          front.Add(Tuple.Create(p, qualities[pI]));
        }
      }
      return front;
    }

    /// <summary>
    /// Calculates the domination result of two solutions which are given in form
    /// of their quality resp. fitness vector.
    /// </summary>
    /// <param name="left">The fitness of the solution that is to be compared.</param>
    /// <param name="right">The fitness of the solution which is compared against.</param>
    /// <param name="maximizations">The objective in each dimension.</param>
    /// <param name="dominateOnEqualQualities">Whether the result should be Dominates in case both fitness vectors are exactly equal</param>
    /// <returns>Dominates if left dominates right, IsDominated if right dominates left and IsNonDominated otherwise.</returns>
    public static DominationResult Dominates(double[] left, double[] right, bool[] maximizations, bool dominateOnEqualQualities) {
      //mkommend Caution: do not use LINQ.SequenceEqual for comparing the two quality arrays (left and right) due to performance reasons
      if (dominateOnEqualQualities) {
        var equal = true;
        for (int i = 0; i < left.Length; i++) {
          if (left[i] != right[i]) {
            equal = false;
            break;
          }
        }
        if (equal) return DominationResult.Dominates;
      }

      bool leftIsBetter = false, rightIsBetter = false;
      for (int i = 0; i < left.Length; i++) {
        if (IsDominated(left[i], right[i], maximizations[i])) rightIsBetter = true;
        else if (IsDominated(right[i], left[i], maximizations[i])) leftIsBetter = true;
        if (leftIsBetter && rightIsBetter) break;
      }

      if (leftIsBetter && !rightIsBetter) return DominationResult.Dominates;
      if (!leftIsBetter && rightIsBetter) return DominationResult.IsDominated;
      return DominationResult.IsNonDominated;
    }

    /// <summary>
    /// A simple check if the quality resp. fitness in <paramref name="left"/> is better than
    /// that given in <paramref name="right"/>.
    /// </summary>
    /// <param name="left">The first fitness value</param>
    /// <param name="right">The second fitness value</param>
    /// <param name="maximization">The objective direction</param>
    /// <returns>True if left is better than right, false if it is not.</returns>
    public static bool IsDominated(double left, double right, bool maximization) {
      return maximization && left < right
        || !maximization && left > right;
    }
  }
}
