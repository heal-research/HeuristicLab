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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Random;

namespace HeuristicLab.Algorithms.MemPR.Binary.SolutionModel.Univariate {
  [Item("Univariate solution model (binary)", "")]
  [StorableClass]
  public sealed class UnivariateModel : Item, ISolutionModel<BinaryVector> {
    [Storable]
    public DoubleArray Probabilities { get; set; }
    [Storable]
    public IRandom Random { get; set; }

    [StorableConstructor]
    private UnivariateModel(bool deserializing) : base(deserializing) { }
    private UnivariateModel(UnivariateModel original, Cloner cloner)
      : base(original, cloner) {
      Probabilities = cloner.Clone(original.Probabilities);
      Random = cloner.Clone(original.Random);
    }
    public UnivariateModel(IRandom random, int N) : this(random, Enumerable.Range(0, N).Select(x => 0.5).ToArray()) { }
    public UnivariateModel(IRandom random, double[] probabilities) {
      Probabilities = new DoubleArray(probabilities);
      Random = random;
    }
    public UnivariateModel(IRandom random, DoubleArray probabilties) {
      Probabilities = probabilties;
      Random = random;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UnivariateModel(this, cloner);
    }

    public BinaryVector Sample() {
      var vec = new BinaryVector(Probabilities.Length);
      for (var i = 0; i < Probabilities.Length; i++)
        vec[i] = Random.NextDouble() < Probabilities[i];
      return vec;
    }

    public static ISolutionModel<BinaryVector> CreateWithoutBias(IRandom random, IEnumerable<BinaryVector> population) {
      double[] model = null;
      var popSize = 0;
      foreach (var p in population) {
        popSize++;
        if (model == null) model = new double[p.Length];
        for (var x = 0; x < model.Length; x++) {
          if (p[x]) model[x]++;
        }
      }
      if (model == null) throw new ArgumentException("Cannot train model from empty population.");
      // normalize to [0;1]
      var factor = 1.0 / popSize;
      for (var x = 0; x < model.Length; x++) {
        model[x] *= factor;
      }
      return new UnivariateModel(random, model);
    }

    public static ISolutionModel<BinaryVector> CreateWithRankBias(IRandom random, bool maximization, IEnumerable<BinaryVector> population, IEnumerable<double> qualities) {
      var popSize = 0;

      double[] model = null;
      var pop = population.Zip(qualities, (b, q) => new { Solution = b, Fitness = q });
      foreach (var ind in maximization ? pop.OrderBy(x => x.Fitness) : pop.OrderByDescending(x => x.Fitness)) {
        // from worst to best, worst solution has 1 vote, best solution N votes
        popSize++;
        if (model == null) model = new double[ind.Solution.Length];
        for (var x = 0; x < model.Length; x++) {
          if (ind.Solution[x]) model[x] += popSize;
        }
      }
      if (model == null) throw new ArgumentException("Cannot train model from empty population.");
      // normalize to [0;1]
      var factor = 2.0 / (popSize + 1);
      for (var i = 0; i < model.Length; i++) {
        model[i] *= factor / popSize;
      }
      return new UnivariateModel(random, model);
    }

    public static ISolutionModel<BinaryVector> CreateWithFitnessBias(IRandom random, bool maximization, IEnumerable<BinaryVector> population, IEnumerable<double> qualities) {
      var proportions = RandomEnumerable.PrepareProportional(qualities, true, !maximization);
      var factor = 1.0 / proportions.Sum();
      double[] model = null;
      foreach (var ind in population.Zip(proportions, (p, q) => new { Solution = p, Proportion = q })) {
        if (model == null) model = new double[ind.Solution.Length];
        for (var x = 0; x < model.Length; x++) {
          if (ind.Solution[x]) model[x] += ind.Proportion * factor;
        }
      }
      if (model == null) throw new ArgumentException("Cannot train model from empty population.");
      return new UnivariateModel(random, model);
    }
  }
}
