#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Permutation;

namespace HeuristicLab.Routing.TSP {
  /// <summary>
  /// Evaluates the TSP path by using the euclidean distance between two points.
  /// </summary>
  public class RoundedEuclideanPathTSPEvaluator : PathTSPEvaluatorBase {
    /// <inheritdoc/>
    public override string Description {
      get { return @"TODO\r\nOperator description still missing ..."; }
    }

    /// <summary>
    /// Calculates the distance between two points by using the euclidean distance.
    /// </summary>
    /// <param name="x1">The x coordinate of point 1.</param>
    /// <param name="y1">The y coordinate of point 1.</param>
    /// <param name="x2">The x coordinate of point 2.</param>
    /// <param name="y2">The y coordinate of point 2.</param>
    /// <returns>The euclidean distance between the two points.</returns>
    protected override double CalculateDistance(double x1, double y1, double x2, double y2) {
      return Math.Round(Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
    }
  }
}
