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

namespace HeuristicLab.Routing.TSP {
  /// <summary>
  /// Base class to inject the calculated distance matrix of the TSP path into the current scope.
  /// </summary>
  public abstract class TSPDistanceMatrixInjectorBase : OperatorBase {
    /// <summary>
    /// Initializes a new instance of <see cref="TSPDistanceMatrixInjectorBase"/> with three variable infos
    /// (<c>Cities</c>, <c>Coordinates</c> and <c>DistanceMatrix</c>).
    /// </summary>
    public TSPDistanceMatrixInjectorBase()
      : base() {
      AddVariableInfo(new VariableInfo("Cities", "Number of cities", typeof(IntData), VariableKind.In));
      AddVariableInfo(new VariableInfo("Coordinates", "City coordinates", typeof(DoubleMatrixData), VariableKind.In));
      AddVariableInfo(new VariableInfo("DistanceMatrix", "Distance matrix containing all distances between cities", typeof(DoubleMatrixData), VariableKind.New));
    }

    /// <summary>
    /// Generates a distance matrix with the distances between all cities in the current path.
    /// </summary>
    /// <param name="scope">The current scope with the path.</param>
    /// <returns><c>null</c>.</returns>
    public override IOperation Apply(IScope scope) {
      int cities = GetVariableValue<IntData>("Cities", scope, true).Data;
      double[,] coordinates = GetVariableValue<DoubleMatrixData>("Coordinates", scope, true).Data;
      double[,] distanceMatrix = new double[cities, cities];

      for (int i = 0; i < cities; i++) {
        for (int j = 0; j < cities; j++) {
          distanceMatrix[i, j] = CalculateDistance(coordinates[i, 0],
                                                   coordinates[i, 1],
                                                   coordinates[j, 0],
                                                   coordinates[j, 1]);
        }
      }
      scope.AddVariable(new Variable(scope.TranslateName("DistanceMatrix"), new DoubleMatrixData(distanceMatrix)));

      return null;
    }

    /// <summary>
    /// Calculates the distance between two points in the path.
    /// </summary>
    /// <param name="x1">The x coordinate of point 1.</param>
    /// <param name="y1">The y coordinate of point 1.</param>
    /// <param name="x2">The x coordinate of point 2.</param>
    /// <param name="y2">The y coordinate of point 2.</param>
    /// <returns>The calculated distance.</returns>
    protected abstract double CalculateDistance(double x1, double y1, double x2, double y2);
  }
}
