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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Algorithms.MemPR.Interfaces;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.MemPR.Permutation.SolutionModel.Univariate {
  [Item("Univariate solution model (Permutation.Absolute)", "")]
  [StorableClass]
  public sealed class UnivariateAbsoluteModel : Item, ISolutionModel<Encodings.PermutationEncoding.Permutation> {
    [Storable]
    public IntMatrix Probabilities { get; set; }
    [Storable]
    public IRandom Random { get; set; }

    [StorableConstructor]
    private UnivariateAbsoluteModel(bool deserializing) : base(deserializing) { }
    private UnivariateAbsoluteModel(UnivariateAbsoluteModel original, Cloner cloner)
      : base(original, cloner) {
      Probabilities = cloner.Clone(original.Probabilities);
      Random = cloner.Clone(original.Random);
    }
    public UnivariateAbsoluteModel(IRandom random, int[,] probabilities) {
      Probabilities = new IntMatrix(probabilities);
      Random = random;
    }
    public UnivariateAbsoluteModel(IRandom random, IntMatrix probabilties) {
      Probabilities = probabilties;
      Random = random;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UnivariateAbsoluteModel(this, cloner);
    }

    public Encodings.PermutationEncoding.Permutation Sample() {
      var N = Probabilities.Rows;
      var child = new Encodings.PermutationEncoding.Permutation(PermutationTypes.Absolute, N);
      var indices = Enumerable.Range(0, N).Shuffle(Random).ToList();
      var values = Enumerable.Range(0, N).Shuffle(Random).ToList();
      for (var i = N - 1; i > 0; i--) {
        var nextIndex = indices[i];
        var total = 0.0;
        for (var v = 0; v < values.Count; v++) {
          total += Probabilities[nextIndex, values[v]] + 1.0 / N;
        }
        var ball = Random.NextDouble() * total;
        for (var v = 0; v < values.Count; v++) {
          ball -= Probabilities[nextIndex, values[v]] + 1.0 / N;
          if (ball <= 0.0) {
            child[nextIndex] = values[v];
            values.RemoveAt(v);
            indices.RemoveAt(i);
            break;
          }
        }
      }
      child[indices[0]] = values[0];
      return child;
    }

    public static UnivariateAbsoluteModel Create(IRandom random, IList<Encodings.PermutationEncoding.Permutation> pop, int N) {
      var model = new int[N, N];
      for (var i = 0; i < pop.Count; i++) {
        for (var j = 0; j < N; j++) {
          model[j, pop[i][j]]++;
        }
      }
      return new UnivariateAbsoluteModel(random, model);
    }
  }
}
