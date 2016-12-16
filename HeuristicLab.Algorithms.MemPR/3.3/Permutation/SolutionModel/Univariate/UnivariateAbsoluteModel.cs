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
    public DoubleMatrix Probabilities { get; set; }
    [Storable]
    public IRandom Random { get; set; }

    [StorableConstructor]
    private UnivariateAbsoluteModel(bool deserializing) : base(deserializing) { }
    private UnivariateAbsoluteModel(UnivariateAbsoluteModel original, Cloner cloner)
      : base(original, cloner) {
      Probabilities = cloner.Clone(original.Probabilities);
      Random = cloner.Clone(original.Random);
    }
    public UnivariateAbsoluteModel(IRandom random, double[,] probabilities) {
      Probabilities = new DoubleMatrix(probabilities);
      Random = random;
    }
    public UnivariateAbsoluteModel(IRandom random, DoubleMatrix probabilties) {
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
        var ball = Random.NextDouble();
        for (var v = 0; v < values.Count; v++) {
          ball -= Probabilities[nextIndex, values[v]] + 1.0 / N;
          if (ball > 0.0) continue;
          child[nextIndex] = values[v];
          values.RemoveAt(v);
          indices.RemoveAt(i);
          break;
        }
        if (ball > 0) {
          var v = values.Count - 1;
          child[nextIndex] = values[v];
          values.RemoveAt(v);
          indices.RemoveAt(i);
        }
      }
      child[indices[0]] = values[0];
      return child;
    }

    public static UnivariateAbsoluteModel CreateUnbiased(IRandom random, IList<Encodings.PermutationEncoding.Permutation> pop, int N) {
      var model = new double[N, N];
      var factor = 1.0 / pop.Count;
      for (var i = 0; i < pop.Count; i++) {
        for (var j = 0; j < N; j++) {
          model[j, pop[i][j]] += factor;
        }
      }
      return new UnivariateAbsoluteModel(random, model);
    }

    public static UnivariateAbsoluteModel CreateWithRankBias(IRandom random, bool maximization, IList<Encodings.PermutationEncoding.Permutation> population, IEnumerable<double> qualities, int N) {
      var popSize = 0;
      var model = new double[N, N];

      var pop = population.Zip(qualities, (b, q) => new { Solution = b, Fitness = q });
      foreach (var ind in maximization ? pop.OrderBy(x => x.Fitness) : pop.OrderByDescending(x => x.Fitness)) {
        // from worst to best, worst solution has 1 vote, best solution N votes
        popSize++;
        for (var j = 0; j < N; j++) {
          model[j, ind.Solution[j]] += popSize;
        }
      }
      // normalize to [0;1]
      var factor = 2.0 / (popSize + 1);
      for (var i = 0; i < N; i++) {
        for (var j = 0; j < N; j++)
          model[i, j] *= factor / popSize;
      }
      if (popSize == 0) throw new ArgumentException("Cannot train model from empty population.");
      return new UnivariateAbsoluteModel(random, model);
    }

    public static UnivariateAbsoluteModel CreateWithFitnessBias(IRandom random, bool maximization, IList<Encodings.PermutationEncoding.Permutation> population, IEnumerable<double> qualities, int N) {
      var proportions = RandomEnumerable.PrepareProportional(qualities, true, !maximization);
      var factor = 1.0 / proportions.Sum();
      var model = new double[N, N];

      foreach (var ind in population.Zip(proportions, (p, q) => new { Solution = p, Proportion = q })) {
        for (var x = 0; x < model.Length; x++) {
          for (var j = 0; j < N; j++) {
            model[j, ind.Solution[j]] += ind.Proportion * factor;
          }
        }
      }
      return new UnivariateAbsoluteModel(random, model);
    }
  }
}
