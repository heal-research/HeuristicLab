#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Attic;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {

  public static class NonDominatedSelect {
    [StorableType("5EDA6CC0-30C6-4B71-8713-F84BB61B1180")]
    public enum DominationResult { Dominates, IsDominated, IsNonDominated };

    public static IEnumerable<double[]> SelectNonDominatedVectors(IEnumerable<double[]> qualities, bool[] maximization, bool dominateOnEqualQualities) {

      List<double[]> front = new List<double[]>();
      foreach (double[] row in qualities) {
        bool insert = true;
        for (int i = 0; i < front.Count; i++) {
          DominationResult res = Dominates(front[i], row, maximization, dominateOnEqualQualities);
          if (res == DominationResult.Dominates) { insert = false; break; }           //Vector domiates Row
          else if (res == DominationResult.IsDominated) {   //Row dominates Vector
            front.RemoveAt(i);
          }
        }
        if (insert) {
          front.Add(row);
        }
      }

      return front;
    }

    public static IEnumerable<double[]> GetDominatingVectors(IEnumerable<double[]> qualities, double[] reference, bool[] maximization, bool dominateOnEqualQualities) {
      List<double[]> front = new List<double[]>();
      foreach (double[] vec in qualities) {
        if (Dominates(vec, reference, maximization, dominateOnEqualQualities) == DominationResult.Dominates) {
          front.Add(vec);
        }
      }
      return front;
    }

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

    private static bool IsDominated(double left, double right, bool maximization) {
      return maximization && left < right
        || !maximization && left > right;
    }

  }
}
