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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TSP {
  /// <summary>
  /// An operator which evaluates TSP solutions given in path representation using a distance matrix.
  /// </summary>
  [Item("TSPDistanceMatrixPathEvaluator", "An operator which evaluates TSP solutions given in path representation using a distance matrix.")]
  [Creatable("Test")]
  [StorableClass]
  public sealed class TSPDistanceMatrixPathEvaluator : TSPEvaluator, ITSPDistanceMatrixPathEvaluator {
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<DoubleMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["DistanceMatrix"]; }
    }

    public TSPDistanceMatrixPathEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The TSP solution given in path representation which should be evaluated."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("DistanceMatrix", "The distance matrix of the cities."));
    }

    public sealed override IOperation Apply() {
      Permutation p = PermutationParameter.ActualValue;
      DoubleMatrix d = DistanceMatrixParameter.ActualValue;

      double length = 0;
      for (int i = 0; i < p.Length - 1; i++)
        length += d[p[i], p[i + 1]];
      length += d[p[p.Length - 1], p[0]];
      QualityParameter.ActualValue = new DoubleValue(length);

      return base.Apply();
    }
  }
}
