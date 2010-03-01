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
  /// An operator which evaluates TSP solutions given in path representation using a distance matrix.
  /// </summary>
  [Item("TSPDistanceMatrixPathEvaluator", "An operator which evaluates TSP solutions given in path representation using a distance matrix.")]
  [Creatable("Test")]
  [EmptyStorableClass]
  public sealed class TSPDistanceMatrixPathEvaluator : TSPEvaluator {
    public LookupParameter<DoubleMatrixData> DistanceMatrixParameter {
      get { return (LookupParameter<DoubleMatrixData>)Parameters["DistanceMatrix"]; }
    }
    public LookupParameter<Permutation> PermutationParameter {
      get { return (LookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    public TSPDistanceMatrixPathEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrixData>("DistanceMatrix", "The distance matrix of the cities."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The TSP solution given in path representation which should be evaluated."));
    }

    public override IOperation Apply() {
      DoubleMatrixData distanceMatrix = DistanceMatrixParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;

      double length = 0;
      for (int i = 0; i < permutation.Length - 1; i++)
        length += distanceMatrix[permutation[i], permutation[i + 1]];
      length += distanceMatrix[permutation[permutation.Length - 1], permutation[0]];
      QualityParameter.ActualValue = new DoubleData(length);

      return base.Apply();
    }
  }
}
