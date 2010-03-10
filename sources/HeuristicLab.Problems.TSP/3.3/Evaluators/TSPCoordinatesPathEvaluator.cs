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
using HeuristicLab.Encodings.Permutation;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TSP {
  /// <summary>
  /// A base class for operators which evaluate TSP solutions given in path representation using city coordinates.
  /// </summary>
  [Item("TSPCoordinatesPathEvaluator", "A base class for operators which evaluate TSP solutions given in path representation using city coordinates.")]
  [StorableClass(StorableClassType.Empty)]
  public abstract class TSPCoordinatesPathEvaluator : TSPEvaluator, ITSPCoordinatesPathEvaluator {
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<DoubleMatrixData> CoordinatesParameter {
      get { return (ILookupParameter<DoubleMatrixData>)Parameters["Coordinates"]; }
    }

    protected TSPCoordinatesPathEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The TSP solution given in path representation which should be evaluated."));
      Parameters.Add(new LookupParameter<DoubleMatrixData>("Coordinates", "The x- and y-Coordinates of the cities."));
    }

    public sealed override IOperation Apply() {
      Permutation p = PermutationParameter.ActualValue;
      DoubleMatrixData c = CoordinatesParameter.ActualValue;

      double length = 0;
      for (int i = 0; i < p.Length - 1; i++)
        length += CalculateDistance(c[p[i], 0], c[p[i], 1], c[p[i + 1], 0], c[p[i + 1], 1]);
      length += CalculateDistance(c[p[p.Length - 1], 0], c[p[p.Length - 1], 1], c[p[0], 0], c[p[0], 1]);
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
