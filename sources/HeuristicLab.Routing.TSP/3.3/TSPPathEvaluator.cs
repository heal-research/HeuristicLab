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
using HeuristicLab.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Routing.TSP {
  /// <summary>
  /// A base class for operators which evaluate TSP solutions given in path representation.
  /// </summary>
  [Item("TSPPathEvaluator", "A base class for operators which evaluate TSP solutions given in path representation.")]
  [EmptyStorableClass]
  public abstract class TSPPathEvaluator : SingleSuccessorOperator {
    public LookupParameter<DoubleMatrixData> CoordinatesParameter {
      get { return (LookupParameter<DoubleMatrixData>)Parameters["Coordinates"]; }
    }
    public LookupParameter<Permutation.Permutation> PermutationParameter {
      get { return (LookupParameter<Permutation.Permutation>)Parameters["Permutation"]; }
    }
    public LookupParameter<DoubleData> QualityParameter {
      get { return (LookupParameter<DoubleData>)Parameters["Quality"]; }
    }

    protected TSPPathEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrixData>("Coordinates", "The x- and y-Coordinates of the cities."));
      Parameters.Add(new LookupParameter<Permutation.Permutation>("Permutation", "The TSP solution given in path representation which should be evaluated."));
      Parameters.Add(new LookupParameter<DoubleData>("Quality", "The evaluated quality of the given TSP solution."));
    }

    public override IExecutionSequence Apply() {
      DoubleMatrixData coordinates = CoordinatesParameter.ActualValue;
      Permutation.Permutation permutation = PermutationParameter.ActualValue;

      double length = 0;
      for (int i = 0; i < permutation.Length - 1; i++)
        length += CalculateDistance(coordinates[permutation[i], 0],
                                    coordinates[permutation[i], 1],
                                    coordinates[permutation[i + 1], 0],
                                    coordinates[permutation[i + 1], 1]);
      length += CalculateDistance(coordinates[permutation[permutation.Length - 1], 0],
                                  coordinates[permutation[permutation.Length - 1], 1],
                                  coordinates[permutation[0], 0],
                                  coordinates[permutation[0], 1]);
      QualityParameter.ActualValue = new DoubleData(length);

      return base.Apply();
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
