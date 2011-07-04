#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.QuadraticAssignment {
  /// <summary>
  /// An operator for analyzing the frequency of alleles in solutions of Quadratic Assignment Problems.
  /// </summary>
  [Item("QAPAlleleFrequencyAnalyzer", "An operator for analyzing the frequency of alleles in solutions of Quadratic Assignment Problems.")]
  [StorableClass]
  public sealed class QAPAlleleFrequencyAnalyzer : AlleleFrequencyAnalyzer<Permutation> {
    public LookupParameter<DoubleMatrix> WeightsParameter {
      get { return (LookupParameter<DoubleMatrix>)Parameters["Weights"]; }
    }
    public LookupParameter<DoubleMatrix> DistancesParameter {
      get { return (LookupParameter<DoubleMatrix>)Parameters["Distances"]; }
    }

    [StorableConstructor]
    private QAPAlleleFrequencyAnalyzer(bool deserializing) : base(deserializing) { }
    private QAPAlleleFrequencyAnalyzer(QAPAlleleFrequencyAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public QAPAlleleFrequencyAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Weights", "The matrix contains the weights between the facilities."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Distances", "The matrix which contains the distances between the locations."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPAlleleFrequencyAnalyzer(this, cloner);
    }

    protected override Allele[] CalculateAlleles(Permutation solution) {
      Allele[] alleles = new Allele[solution.Length];
      DoubleMatrix weights = WeightsParameter.ActualValue;
      DoubleMatrix distances = DistancesParameter.ActualValue;
      int source, target;
      double impact;

      for (int i = 0; i < solution.Length; i++) {
        source = i;
        target = solution[i];
        impact = 0;
        for (int j = 0; j < solution.Length; j++)
          impact += weights[source, j] * distances[target, solution[j]];
        alleles[i] = new Allele(source.ToString() + "->" + target.ToString(), impact);
      }

      return alleles;
    }
  }
}
