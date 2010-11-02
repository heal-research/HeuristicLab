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

using System;
using HeuristicLab.Analysis;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator for analyzing the frequency of alleles in solutions of Traveling Salesman Problems given in path representation.
  /// </summary>
  [Item("TSPAlleleFrequencyAnalyzer", "An operator for analyzing the frequency of alleles in solutions of Traveling Salesman Problems given in path representation.")]
  [StorableClass]
  public sealed class TSPAlleleFrequencyAnalyzer : AlleleFrequencyAnalyzer<Permutation> {
    public LookupParameter<DoubleMatrix> CoordinatesParameter {
      get { return (LookupParameter<DoubleMatrix>)Parameters["Coordinates"]; }
    }

    [StorableConstructor]
    private TSPAlleleFrequencyAnalyzer(bool deserializing) : base(deserializing) { }
    public TSPAlleleFrequencyAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<DoubleMatrix>("Coordinates", "The x- and y-Coordinates of the cities."));
    }

    protected override Allele[] CalculateAlleles(Permutation solution) {
      Allele[] alleles = new Allele[solution.Length];
      DoubleMatrix coords = CoordinatesParameter.ActualValue;

      for (int i = 0; i < solution.Length - 1; i++)
        alleles[i] = CreateAllele(solution[i], solution[i + 1], coords);
      alleles[alleles.Length - 1] = CreateAllele(solution[solution.Length - 1], solution[0], coords);

      return alleles;
    }

    private Allele CreateAllele(int source, int target, DoubleMatrix coords) {
      if (source > target) {
        int h = source;
        source = target;
        target = h;
      }

      return new Allele(source.ToString() + "-" + target.ToString(),
                        CalculateLength(coords[source, 0], coords[source, 1], coords[target, 0], coords[target, 1]));
    }

    private double CalculateLength(double x1, double y1, double x2, double y2) {
      return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
    }
  }
}
