#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HEAL.Fossil;

namespace HeuristicLab.Problems.QuadraticAssignment {
  /// <summary>
  /// An operator for analyzing the frequency of alleles in solutions of Quadratic Assignment Problems.
  /// </summary>
  [Item("QAPAlleleFrequencyAnalyzer", "An operator for analyzing the frequency of alleles in solutions of Quadratic Assignment Problems.")]
  [StorableType("490E5FD6-A269-4F5F-AC07-12056CDFFC7F")]
  public sealed class QAPAlleleFrequencyAnalyzer : AlleleFrequencyAnalyzer<Permutation> {
    public LookupParameter<DoubleMatrix> WeightsParameter {
      get { return (LookupParameter<DoubleMatrix>)Parameters["Weights"]; }
    }
    public LookupParameter<DoubleMatrix> DistancesParameter {
      get { return (LookupParameter<DoubleMatrix>)Parameters["Distances"]; }
    }

    [StorableConstructor]
    private QAPAlleleFrequencyAnalyzer(StorableConstructorFlag _) : base(_) { }
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
      Allele[] alleles = new Allele[solution.Length * solution.Length];
      Dictionary<string, int> allelesDict = new Dictionary<string, int>();
      DoubleMatrix weights = WeightsParameter.ActualValue;
      DoubleMatrix distances = DistancesParameter.ActualValue;
      double impact;

      for (int x = 0; x < solution.Length; x++) {
        for (int y = 0; y < solution.Length; y++) {
          string allele = weights[x, y].ToString() + ">" + distances[solution[x], solution[y]].ToString();
          int repetition = 1;
          if (allelesDict.ContainsKey(allele)) repetition += allelesDict[allele];
          allelesDict[allele] = repetition;
          impact = weights[x, y] * distances[solution[x], solution[y]];
          alleles[x * solution.Length + y] = new Allele(allele + "/" + repetition, impact);
        }
      }

      return alleles;
    }
  }
}
