#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// A base class for operators which evaluate TSP solutions given in path representation using a distance matrix.
  /// </summary>
  [Item("TSPDistanceMatrixEvaluator", "Evaluate TSP solutions given in path representation using the distance matrix.")]
  [StorableClass]
  public sealed class TSPDistanceMatrixEvaluator : TSPEvaluator, ITSPDistanceMatrixEvaluator {

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }

    [StorableConstructor]
    private TSPDistanceMatrixEvaluator(bool deserializing) : base(deserializing) { }
    private TSPDistanceMatrixEvaluator(TSPDistanceMatrixEvaluator original, Cloner cloner) : base(original, cloner) { }
    public TSPDistanceMatrixEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The TSP solution given in path representation which should be evaluated."));
      Parameters.Add(new LookupParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPDistanceMatrixEvaluator(this, cloner);
    }

    public static double Apply(DistanceMatrix distances, Permutation tour) {
      if (distances == null || distances.Rows == 0 || distances.Columns == 0
        || distances.Rows != distances.Columns)
        throw new InvalidOperationException("TSPDistanceMatrixEvaluator: The distance matrix is empty or not square");
      if (tour == null) throw new ArgumentNullException("tour", "TSPDistanceMatrixEvaluator: No tour is given.");
      Permutation p = tour;
      double length = 0;
      for (int i = 0; i < p.Length - 1; i++)
        length += distances[p[i], p[i + 1]];
      length += distances[p[p.Length - 1], p[0]];
      return length;
    }

    public override IOperation InstrumentedApply() {
      Permutation p = PermutationParameter.ActualValue;
      DistanceMatrix dm = DistanceMatrixParameter.ActualValue;

      QualityParameter.ActualValue = new DoubleValue(Apply(dm, p));
      return base.InstrumentedApply();
    }
  }
}
