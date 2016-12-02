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
  [Item("Univariate solution model (Permutation.Relative)", "")]
  [StorableClass]
  public sealed class UnivariateRelativeModel : Item, ISolutionModel<Encodings.PermutationEncoding.Permutation> {
    [Storable]
    public IntMatrix Probabilities { get; set; }

    [Storable]
    public IRandom Random { get; set; }

    [Storable]
    public PermutationTypes PermutationType { get; set; }

    [StorableConstructor]
    private UnivariateRelativeModel(bool deserializing) : base(deserializing) { }
    private UnivariateRelativeModel(UnivariateRelativeModel original, Cloner cloner)
      : base(original, cloner) {
      Probabilities = cloner.Clone(original.Probabilities);
      Random = cloner.Clone(original.Random);
      PermutationType = original.PermutationType;
    }
    public UnivariateRelativeModel(IRandom random, int[,] probabilities, PermutationTypes permutationType) {
      Probabilities = new IntMatrix(probabilities);
      Random = random;
      PermutationType = permutationType;
    }
    public UnivariateRelativeModel(IRandom random, IntMatrix probabilties, PermutationTypes permutationType) {
      Probabilities = probabilties;
      Random = random;
      PermutationType = permutationType;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UnivariateRelativeModel(this, cloner);
    }

    public Encodings.PermutationEncoding.Permutation Sample() {
      var N = Probabilities.Rows;
      var next = Random.Next(N);
      var child = new Encodings.PermutationEncoding.Permutation(PermutationType, N);
      child[0] = next;
      var open = Enumerable.Range(0, N).Where(x => x != next).Shuffle(Random).ToList();
      for (var i = 1; i < N - 1; i++) {
        var total = 0.0;
        for (var j = 0; j < open.Count; j++) {
          total += Probabilities[next, open[j]] + 1.0 / N;
        }
        var ball = Random.NextDouble() * total;
        for (var j = 0; j < open.Count; j++) {
          ball -= Probabilities[next, open[j]] + 1.0 / N;
          if (ball <= 0.0) {
            child[i] = open[j];
            next = open[j];
            open.RemoveAt(j);
            break;
          }
        }
      }
      child[N - 1] = open[0];
      return child;
    }

    public static UnivariateRelativeModel CreateDirected(IRandom random, IList<Encodings.PermutationEncoding.Permutation> pop, int N) {
      var model = new int[N, N];
      for (var i = 0; i < pop.Count; i++) {
        for (var j = 0; j < N - 1; j++) {
          for (var k = j + 1; k < N; k++) {
            model[pop[i][j], pop[i][k]]++;
          }
          model[pop[i][N - 1], pop[i][0]]++;
        }
      }
      return new UnivariateRelativeModel(random, model, PermutationTypes.RelativeDirected);
    }

    public static UnivariateRelativeModel CreateUndirected(IRandom random, IList<Encodings.PermutationEncoding.Permutation> pop, int N) {
      var model = new int[N, N];
      for (var i = 0; i < pop.Count; i++) {
        for (var j = 0; j < N - 1; j++) {
          for (var k = j + 1; k < N; k++) {
            model[pop[i][j], pop[i][k]]++;
            model[pop[i][k], pop[i][j]]++;
          }
          model[pop[i][0], pop[i][N - 1]]++;
          model[pop[i][N - 1], pop[i][0]]++;
        }
      }
      return new UnivariateRelativeModel(random, model, PermutationTypes.RelativeUndirected);
    }
  }
}
