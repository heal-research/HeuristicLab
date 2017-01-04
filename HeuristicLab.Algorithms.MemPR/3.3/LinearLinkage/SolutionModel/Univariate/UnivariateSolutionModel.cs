#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.LinearLinkageEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Algorithms.MemPR.Grouping.SolutionModel.Univariate {
  [Item("Univariate solution model (linear linkage)", "")]
  [StorableClass]
  public sealed class UnivariateModel : Item, ISolutionModel<LinearLinkage> {
    [Storable]
    public IntMatrix Frequencies { get; set; }
    [Storable]
    public IRandom Random { get; set; }
    [Storable]
    public IntValue Maximum { get; set; }

    [StorableConstructor]
    private UnivariateModel(bool deserializing) : base(deserializing) { }
    private UnivariateModel(UnivariateModel original, Cloner cloner)
      : base(original, cloner) {
      Frequencies = cloner.Clone(original.Frequencies);
      Random = cloner.Clone(original.Random);
    }
    public UnivariateModel(IRandom random, int[,] frequencies, int max) {
      Frequencies = new IntMatrix(frequencies);
      Random = random;
      Maximum = new IntValue(max);
    }
    public UnivariateModel(IRandom random, IntMatrix frequencies, int max) {
      Frequencies = frequencies;
      Random = random;
      Maximum = new IntValue(max);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UnivariateModel(this, cloner);
    }

    public LinearLinkage Sample() {
      var N = Frequencies.Rows;
      var centroid = LinearLinkage.SingleElementGroups(N);
      var dict = new Dictionary<int, int>();
      for (var i = N - 1; i >= 0; i--) {
        centroid[i] = i; // default be a cluster of your own
        for (var j = i + 1; j < N; j++) {
          // try to find a suitable link
          if (Maximum.Value * Random.NextDouble() < Frequencies[i, j]) {
            int pred;
            if (dict.TryGetValue(j, out pred)) {
              int tmp, k = pred;
              while (dict.TryGetValue(k, out tmp)) {
                if (k == tmp) break;
                k = tmp;
              }
              centroid[i] = k;
            } else centroid[i] = j;
            dict[centroid[i]] = i;
            break;
          }
        }
      }
      return centroid;
    }

    public static ISolutionModel<LinearLinkage> Create(IRandom random, IEnumerable<LinearLinkage> population) {
      var iter = population.GetEnumerator();
      if (!iter.MoveNext()) throw new ArgumentException("Cannot create solution model from empty population.");
      var popSize = 1;
      var N = iter.Current.Length;
      var freq = new int[N, N];
      do {
        var current = iter.Current;
        popSize++;
        foreach (var g in current.GetGroups()) {
          for (var i = 0; i < g.Count - 1; i++)
            for (var j = i + 1; j < g.Count; j++) {
              freq[g[i], g[j]]++;
              freq[g[j], g[i]]++;
            }
        }
      } while (iter.MoveNext());
      return new UnivariateModel(random, freq, popSize);
    }
  }
}
