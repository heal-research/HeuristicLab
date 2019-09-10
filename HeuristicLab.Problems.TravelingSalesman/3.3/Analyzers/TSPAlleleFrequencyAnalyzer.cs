#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator for analyzing the frequency of alleles in solutions of Traveling Salesman Problems given in path representation.
  /// </summary>
  [Item("TSPAlleleFrequencyAnalyzer", "An operator for analyzing the frequency of alleles in solutions of Traveling Salesman Problems given in path representation.")]
  [StorableType("43a9ec34-c917-43f8-bd14-4d42e2d0a458")]
  public sealed class TSPAlleleFrequencyAnalyzer : AlleleFrequencyAnalyzer<Permutation> {
    [Storable] public ILookupParameter<ITSPData> TSPDataParameter { get; private set; }

    [StorableConstructor]
    private TSPAlleleFrequencyAnalyzer(StorableConstructorFlag _) : base(_) { }
    private TSPAlleleFrequencyAnalyzer(TSPAlleleFrequencyAnalyzer original, Cloner cloner)
      : base(original, cloner) {
      TSPDataParameter = cloner.Clone(original.TSPDataParameter);
    }
    public TSPAlleleFrequencyAnalyzer()
      : base() {
      Parameters.Add(TSPDataParameter = new LookupParameter<ITSPData>("TSPData", "The main parameters of the TSP."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPAlleleFrequencyAnalyzer(this, cloner);
    }

    protected override Allele[] CalculateAlleles(Permutation solution) {
      var tspData = TSPDataParameter.ActualValue;
      Allele[] alleles = new Allele[solution.Length];
      int source, target, h;
      double impact;

      for (int i = 0; i < solution.Length - 1; i++) {
        source = solution[i];
        target = solution[i + 1];
        if (source > target) { h = source; source = target; target = h; }
        impact = tspData.GetDistance(source, target);
        alleles[i] = new Allele(source.ToString() + "-" + target.ToString(), impact);
      }
      source = solution[solution.Length - 1];
      target = solution[0];
      if (source > target) { h = source; source = target; target = h; }
      impact = tspData.GetDistance(source, target);
      alleles[alleles.Length - 1] = new Allele(source.ToString() + "-" + target.ToString(), impact);

      return alleles;
    }
  }
}
