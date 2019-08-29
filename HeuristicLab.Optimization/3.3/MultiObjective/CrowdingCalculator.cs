#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization {
  /// <summary>
  /// CrowdingCalculator distance d(x,A) is usually defined between a point x and a set of points A
  /// d(x,A) is the sum over all dimensions where for each dimension the next larger and the next smaller Point to x are subtracted
  /// see in more detail: "A fast elitist non-dominated sorting genetic algorithm for multi-objective optimization: NSGA-II" by K Deb, S Agrawal, A Pratap, T Meyarivan
  /// CrowdingCalculator as a quality of the complete qualities is defined here as the mean of the crowding distances of every point x in A 
  /// C(A) = mean(d(x,A)) where x in A  and d(x,A) is not infinite
  /// Beware that CrowdingCalculator is not normalized for the number of dimensions. A higher number of dimensions normally causes higher CrowdingCalculator values
  /// </summary>
  public static class CrowdingCalculator {
    public static double CalculateCrowding<TP>(IEnumerable<TP> qualities) where TP : IReadOnlyList<double> {
      return CalculateCrowdingDistances(qualities.ToArray()).Where(d => !double.IsPositiveInfinity(d)).DefaultIfEmpty(double.PositiveInfinity).Average();
    }

    public static IList<double> CalculateCrowdingDistances<TP>(TP[] qualities) where TP : IReadOnlyList<double> {
      if (qualities == null) throw new ArgumentException("qualities must not be null");
      if (!qualities.Any()) throw new ArgumentException("qualities must not be empty");

      var lastIndex = qualities.Length - 1;
      int objectiveCount = qualities[0].Count;

      var pointsums = qualities.ToDictionary(x => x, x => 0.0);
      for (var dim = 0; dim < objectiveCount; dim++) {
        var arr = qualities.OrderBy(x => x[dim]).ToArray();

        pointsums[arr[0]] = double.PositiveInfinity;
        pointsums[arr[lastIndex]] = double.PositiveInfinity;

        var d = arr[lastIndex][dim] - arr[0][dim];
        if (d.IsAlmost(0.0)) d = 1.0;
        for (var i = 1; i < lastIndex; i++)
          pointsums[arr[i]] += (arr[i + 1][dim] - arr[i - 1][dim]) / d;
      }
      return qualities.Select(x => pointsums[x]).ToList();
    }
  }
}