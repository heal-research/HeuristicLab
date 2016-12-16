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
  [Item("Univariate solution model (Permutation.Relative)", "")]
  [StorableClass]
  public sealed class UnivariateRelativeModel : Item, ISolutionModel<Encodings.PermutationEncoding.Permutation> {
    [Storable]
    public DoubleMatrix Probabilities { get; set; }

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
    public UnivariateRelativeModel(IRandom random, double[,] probabilities, PermutationTypes permutationType) {
      Probabilities = new DoubleMatrix(probabilities);
      Random = random;
      PermutationType = permutationType;
    }
    public UnivariateRelativeModel(IRandom random, DoubleMatrix probabilties, PermutationTypes permutationType) {
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
      var model = new double[N, N];
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

    public static UnivariateRelativeModel CreateDirectedWithRankBias(IRandom random, bool maximization, IList<Encodings.PermutationEncoding.Permutation> population, IEnumerable<double> qualities, int N) {
      var popSize = 0;
      var model = new double[N, N];

      var pop = population.Zip(qualities, (b, q) => new { Solution = b, Fitness = q });
      foreach (var ind in maximization ? pop.OrderBy(x => x.Fitness) : pop.OrderByDescending(x => x.Fitness)) {
        // from worst to best, worst solution has 1 vote, best solution N votes
        popSize++;
        for (var j = 0; j < N - 1; j++) {
          for (var k = j + 1; k < N; k++) {
            model[ind.Solution[j], ind.Solution[k]] += popSize;
          }
          model[ind.Solution[N - 1], ind.Solution[0]] += popSize;
        }
      }
      if (popSize == 0) throw new ArgumentException("Cannot train model from empty population.");
      return new UnivariateRelativeModel(random, model, PermutationTypes.RelativeDirected);
    }

    public static UnivariateRelativeModel CreateDirectedWithFitnessBias(IRandom random, bool maximization, IList<Encodings.PermutationEncoding.Permutation> population, IEnumerable<double> qualities, int N) {
      var proportions = RandomEnumerable.PrepareProportional(qualities, true, !maximization);
      var factor = 1.0 / proportions.Sum();
      var model = new double[N, N];

      foreach (var ind in population.Zip(proportions, (p, q) => new { Solution = p, Proportion = q })) {
        for (var x = 0; x < model.Length; x++) {
          for (var j = 0; j < N - 1; j++) {
            for (var k = j + 1; k < N; k++) {
              model[ind.Solution[j], ind.Solution[k]] += ind.Proportion * factor;
            }
            model[ind.Solution[N - 1], ind.Solution[0]] += ind.Proportion * factor;
          }
        }
      }
      return new UnivariateRelativeModel(random, model, PermutationTypes.RelativeDirected);
    }

    public static UnivariateRelativeModel CreateUndirected(IRandom random, IList<Encodings.PermutationEncoding.Permutation> pop, int N) {
      var model = new double[N, N];
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

    public static UnivariateRelativeModel CreateUndirectedWithRankBias(IRandom random, bool maximization, IList<Encodings.PermutationEncoding.Permutation> population, IEnumerable<double> qualities, int N) {
      var popSize = 0;
      var model = new double[N, N];

      var pop = population.Zip(qualities, (b, q) => new { Solution = b, Fitness = q });
      foreach (var ind in maximization ? pop.OrderBy(x => x.Fitness) : pop.OrderByDescending(x => x.Fitness)) {
        // from worst to best, worst solution has 1 vote, best solution N votes
        popSize++;
        for (var j = 0; j < N - 1; j++) {
          for (var k = j + 1; k < N; k++) {
            model[ind.Solution[j], ind.Solution[k]] += popSize;
            model[ind.Solution[k], ind.Solution[j]] += popSize;
          }
          model[ind.Solution[0], ind.Solution[N - 1]] += popSize;
          model[ind.Solution[N - 1], ind.Solution[0]] += popSize;
        }
      }
      if (popSize == 0) throw new ArgumentException("Cannot train model from empty population.");
      return new UnivariateRelativeModel(random, model, PermutationTypes.RelativeUndirected);
    }

    public static UnivariateRelativeModel CreateUndirectedWithFitnessBias(IRandom random, bool maximization, IList<Encodings.PermutationEncoding.Permutation> population, IEnumerable<double> qualities, int N) {
      var proportions = RandomEnumerable.PrepareProportional(qualities, true, !maximization);
      var factor = 1.0 / proportions.Sum();
      var model = new double[N, N];

      foreach (var ind in population.Zip(proportions, (p, q) => new { Solution = p, Proportion = q })) {
        for (var x = 0; x < model.Length; x++) {
          for (var j = 0; j < N - 1; j++) {
            for (var k = j + 1; k < N; k++) {
              model[ind.Solution[j], ind.Solution[k]] += ind.Proportion * factor;
              model[ind.Solution[k], ind.Solution[j]] += ind.Proportion * factor;
            }
            model[ind.Solution[0], ind.Solution[N - 1]] += ind.Proportion * factor;
            model[ind.Solution[N - 1], ind.Solution[0]] += ind.Proportion * factor;
          }
        }
      }
      return new UnivariateRelativeModel(random, model, PermutationTypes.RelativeUndirected);
    }
  }
}
