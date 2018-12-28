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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator which evaluates TSP solutions given in path representation using the Euclidean distance metric.
  /// </summary>
  [Item("TSPEuclideanPathEvaluator", "An operator which evaluates TSP solutions given in path representation using the Euclidean distance metric.")]
  [StorableType("583A53B5-2ABD-44C4-A530-3B5395FB5855")]
  public sealed class TSPEuclideanPathEvaluator : TSPCoordinatesPathEvaluator {
    [StorableConstructor]
    private TSPEuclideanPathEvaluator(StorableConstructorFlag _) : base(_) { }
    private TSPEuclideanPathEvaluator(TSPEuclideanPathEvaluator original, Cloner cloner) : base(original, cloner) { }
    public TSPEuclideanPathEvaluator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPEuclideanPathEvaluator(this, cloner);
    }
    
    /// <summary>
    /// Calculates the distance between two points using the Euclidean distance metric.
    /// </summary>
    /// <param name="x1">The x-coordinate of point 1.</param>
    /// <param name="y1">The y-coordinate of point 1.</param>
    /// <param name="x2">The x-coordinate of point 2.</param>
    /// <param name="y2">The y-coordinate of point 2.</param>
    /// <returns>The calculated distance.</returns>
    protected override double CalculateDistance(double x1, double y1, double x2, double y2) {
      return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
  }
}
