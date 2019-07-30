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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator which evaluates TSP solutions given in path representation using the rounded Euclidean distance metric.
  /// </summary>
  [Item("TSPRoundedEuclideanPathEvaluator", "An operator which evaluates TSP solutions given in path representation using the rounded Euclidean distance metric.")]
  [StorableType("AD108E29-244A-42E1-AA46-23A09C9894B9")]
  public sealed class TSPRoundedEuclideanPathEvaluator : TSPCoordinatesPathEvaluator {
    [StorableConstructor]
    private TSPRoundedEuclideanPathEvaluator(StorableConstructorFlag _) : base(_) { }
    private TSPRoundedEuclideanPathEvaluator(TSPRoundedEuclideanPathEvaluator original, Cloner cloner) : base(original, cloner) { }
    public TSPRoundedEuclideanPathEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPRoundedEuclideanPathEvaluator(this, cloner);
    }
    
    /// <summary>
    /// Calculates the distance between two points using the rounded Euclidean distance metric.
    /// </summary>
    /// <param name="x1">The x-coordinate of point 1.</param>
    /// <param name="y1">The y-coordinate of point 1.</param>
    /// <param name="x2">The x-coordinate of point 2.</param>
    /// <param name="y2">The y-coordinate of point 2.</param>
    /// <returns>The calculated distance.</returns>
    protected override double CalculateDistance(double x1, double y1, double x2, double y2) {
      return Math.Round(Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }
  }
}
