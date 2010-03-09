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
  /// An operator which evaluates TSP solutions given in path representation using a distance matrix.
  /// </summary>
  [Item("TSPDistanceMatrixPathEvaluator", "An operator which evaluates TSP solutions given in path representation using a distance matrix.")]
  [Creatable("Test")]
  [EmptyStorableClass]
  public sealed class TSPDistanceMatrixPathEvaluator : TSPPathEvaluator, ITSPDistanceMatrixPathEvaluator {
    public ILookupParameter<DoubleMatrixData> DistanceMatrixParameter {
      get { return (ILookupParameter<DoubleMatrixData>)Parameters["DistanceMatrix"]; }
    }

    public TSPDistanceMatrixPathEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrixData>("DistanceMatrix", "The distance matrix of the cities."));
    }

    protected override double CalculateDistance(int a, int b) {
      DoubleMatrixData distanceMatrix = DistanceMatrixParameter.ActualValue;
      return distanceMatrix[a, b];
    }
  }
}
