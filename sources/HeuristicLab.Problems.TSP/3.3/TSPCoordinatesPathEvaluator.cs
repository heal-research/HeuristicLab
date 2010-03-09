#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TSP {
  /// <summary>
  /// A base class for operators which evaluate TSP solutions given in path representation using city coordinates.
  /// </summary>
  [Item("TSPCoordinatesPathEvaluator", "A base class for operators which evaluate TSP solutions given in path representation using city coordinates.")]
  [EmptyStorableClass]
  public abstract class TSPCoordinatesPathEvaluator : TSPPathEvaluator, ITSPCoordinatesPathEvaluator {
    public ILookupParameter<DoubleMatrixData> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrixData>)Parameters["Coordinates"]; }
    }

    protected TSPCoordinatesPathEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrixData>("Coordinates", "The x- and y-Coordinates of the cities."));
    }

    protected sealed override double CalculateDistance(int a, int b) {
      DoubleMatrixData coordinates = CoordinatesParameter.ActualValue;
      return CalculateDistance(coordinates[a, 0], coordinates[a, 1],
                               coordinates[b, 0], coordinates[b, 1]);
    }

    /// <summary>
    /// Calculates the distance between two points.
    /// </summary>
    /// <param name="x1">The x-coordinate of point 1.</param>
    /// <param name="y1">The y-coordinate of point 1.</param>
    /// <param name="x2">The x-coordinate of point 2.</param>
    /// <param name="y2">The y-coordinate of point 2.</param>
    /// <returns>The calculated distance.</returns>
    protected abstract double CalculateDistance(double x1, double y1, double x2, double y2);
  }
}
