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
  /// Base class for TSP path evaluation, based on length calculation.
  /// </summary>
  public abstract class PathTSPEvaluatorBase : SingleObjectiveEvaluatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="PathTSPEvaluatorBase"/> with two variable infos
    /// (<c>Coordinates</c> and <c>Permutation</c>).
    /// </summary>
    public PathTSPEvaluatorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Coordinates", "City coordinates", typeof(DoubleMatrixData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Permutation", "Permutation representing a TSP solution in path encoding", typeof(Permutation.Permutation), VariableKind.In));
    }

    /// <summary>
    /// Calculates the length of a whole path of the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to evaluate the path.</param>
    /// <returns>The calculated length.</returns>
    protected sealed override double Evaluate(IScope scope) {
      double[,] coordinates = GetVariableValue<DoubleMatrixData>("Coordinates", scope, true).Data;
      int[] perm = GetVariableValue<Permutation.Permutation>("Permutation", scope, false).Data;
      double length = 0;

      for (int i = 0; i < perm.Length - 1; i++)
        length += CalculateDistance(coordinates[perm[i], 0],
                                    coordinates[perm[i], 1],
                                    coordinates[perm[i + 1], 0],
                                    coordinates[perm[i + 1], 1]);
      length += CalculateDistance(coordinates[perm[perm.Length - 1], 0],
                                  coordinates[perm[perm.Length - 1], 1],
                                  coordinates[perm[0], 0],
                                  coordinates[perm[0], 1]);
      return length;
    }

    /// <summary>
    /// Calculates the distance between two given points.
    /// </summary>
    /// <param name="x1">The x coordinate of point 1.</param>
    /// <param name="y1">The y coordinate of point 1.</param>
    /// <param name="x2">The x coordinate of point 2.</param>
    /// <param name="y2">The y coordinate of point 2.</param>
    /// <returns>The calculated distance.</returns>
    protected abstract double CalculateDistance(double x1, double y1, double x2, double y2);
  }
}
