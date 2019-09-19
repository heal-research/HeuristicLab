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

using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.PTSP {
  [Item("Estimated Probabilistic TSP (pTSP)", "Represents a probabilistic traveling salesman problem where the expected tour length is estimated by averaging over the length of tours on a number of, so called, realizations.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems)]
  [StorableType("d1b4149b-8ab9-4314-8d96-9ea04a4d5b8b")]
  public sealed class EstimatedPTSP : ProbabilisticTSP {

    #region Parameter Properties
    [Storable] public IFixedValueParameter<IntValue> RealizationsSeedParameter { get; private set; }
    [Storable] public IFixedValueParameter<IntValue> RealizationsParameter { get; private set; }
    [Storable] private IValueParameter<ReadOnlyItemList<BoolArray>> RealizationDataParameter { get; set; }
    #endregion

    #region Properties

    public int RealizationsSeed {
      get { return RealizationsSeedParameter.Value.Value; }
      set { RealizationsSeedParameter.Value.Value = value; }
    }

    public int Realizations {
      get { return RealizationsParameter.Value.Value; }
      set { RealizationsParameter.Value.Value = value; }
    }
    
    private ReadOnlyItemList<BoolArray> RealizationData {
      get { return RealizationDataParameter.Value; }
      set { RealizationDataParameter.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private EstimatedPTSP(StorableConstructorFlag _) : base(_) { }
    private EstimatedPTSP(EstimatedPTSP original, Cloner cloner)
      : base(original, cloner) {
      RealizationsSeedParameter = cloner.Clone(original.RealizationsSeedParameter);
      RealizationsParameter = cloner.Clone(original.RealizationsParameter);
      RealizationDataParameter = cloner.Clone(original.RealizationDataParameter);
      RegisterEventHandlers();
    }
    public EstimatedPTSP() {
      Parameters.Add(RealizationsSeedParameter = new FixedValueParameter<IntValue>("RealizationsSeed", "The starting seed of the RNG from which realizations should be drawn.", new IntValue(1)));
      Parameters.Add(RealizationsParameter = new FixedValueParameter<IntValue>("Realizations", "The number of realizations that should be made.", new IntValue(100)));
      Parameters.Add(RealizationDataParameter = new ValueParameter<ReadOnlyItemList<BoolArray>>("RealizationData", "The actual realizations.") { Hidden = true, GetsCollected = false });

      Operators.Add(new PTSPEstimatedInversionMoveEvaluator());
      Operators.Add(new PTSPEstimatedInsertionMoveEvaluator());
      Operators.Add(new PTSPEstimatedInversionLocalImprovement());
      Operators.Add(new PTSPEstimatedInsertionLocalImprovement());
      Operators.Add(new PTSPEstimatedTwoPointFiveLocalImprovement());

      Operators.Add(new ExhaustiveTwoPointFiveMoveGenerator());
      Operators.Add(new StochasticTwoPointFiveMultiMoveGenerator());
      Operators.Add(new StochasticTwoPointFiveSingleMoveGenerator());
      Operators.Add(new TwoPointFiveMoveMaker());
      Operators.Add(new PTSPEstimatedTwoPointFiveMoveEvaluator());

      Encoding.ConfigureOperators(Operators.OfType<IOperator>());
      foreach (var twopointfiveMoveOperator in Operators.OfType<ITwoPointFiveMoveOperator>()) {
        twopointfiveMoveOperator.TwoPointFiveMoveParameter.ActualName = "Permutation.TwoPointFiveMove";
      }

      UpdateRealizations();
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EstimatedPTSP(this, cloner);
    }

    public override double Evaluate(Permutation tour, IRandom random) {
      return Evaluate(tour, ProbabilisticTSPData, RealizationData);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    public static double Evaluate(Permutation tour, IProbabilisticTSPData data, IEnumerable<BoolArray> realizations) {
      // Estimation-based evaluation, here without calculating variance for faster evaluation
      var estimatedSum = 0.0;
      var count = 0;
      foreach (var r in realizations) {
        int singleRealization = -1, firstNode = -1;
        for (var j = 0; j < data.Cities; j++) {
          if (r[tour[j]]) {
            if (singleRealization != -1) {
              estimatedSum += data.GetDistance(singleRealization, tour[j]);
            } else {
              firstNode = tour[j];
            }
            singleRealization = tour[j];
          }
        }
        if (singleRealization != -1)
          estimatedSum += data.GetDistance(singleRealization, firstNode);
        count++;
      }
      return estimatedSum / count;
    }

    /// <summary>
    /// An evaluate method that can be used if mean as well as variance should be calculated
    /// </summary>
    /// <param name="tour">The tour between all cities.</param>
    /// <param name="data">The main parameters of the pTSP.</param>
    /// <param name="realizations">How many realizations to achieve.</param>
    /// <param name="seed">The starting seed of generating the realizations.</param>
    /// <param name="variance">The estimated variance will be returned in addition to the mean.</param>
    /// <returns>A vector with length two containing mean and variance.</returns>
    public static double Evaluate(Permutation tour, IProbabilisticTSPData data, IEnumerable<BoolArray> realizations, out double variance) {
      // Estimation-based evaluation
      var estimatedSum = 0.0;
      var partialSums = new List<double>();
      var count = 0;
      foreach (var r in realizations) {
        var pSum = 0.0;
        int singleRealization = -1, firstNode = -1;
        for (var j = 0; j < data.Cities; j++) {
          if (r[tour[j]]) {
            if (singleRealization != -1) {
              pSum += data.GetDistance(singleRealization, tour[j]);
            } else {
              firstNode = tour[j];
            }
            singleRealization = tour[j];
          }
        }
        if (singleRealization != -1) {
          pSum += data.GetDistance(singleRealization, firstNode);
        }
        estimatedSum += pSum;
        partialSums.Add(pSum);
        count++;
      }
      var mean = estimatedSum / count;
      variance = 0.0;
      for (var i = 0; i < count; i++) {
        variance += Math.Pow((partialSums[i] - mean), 2);
      }
      variance = variance / count;
      return mean;
    }

    private void RegisterEventHandlers() {
      RealizationsParameter.Value.ValueChanged += RealizationsOnChanged;
      RealizationsSeedParameter.Value.ValueChanged += RealizationsSeedOnChanged;
    }

    private void RealizationsSeedOnChanged(object sender, EventArgs e) {
      UpdateRealizations();
    }

    private void RealizationsOnChanged(object sender, EventArgs e) {
      if (Realizations <= 0) Realizations = 1;
      else UpdateRealizations();
    }

    public override void Load(PTSPData data) {
      base.Load(data);
      UpdateRealizations();
    }

    private void UpdateRealizations() {
      var data = new List<BoolArray>(Realizations);
      var rng = new MersenneTwister((uint)RealizationsSeed);
      if (Enumerable.Range(0, ProbabilisticTSPData.Cities).All(c => ProbabilisticTSPData.GetProbability(c) <= 0))
        throw new InvalidOperationException("All probabilities are zero.");
      while (data.Count < Realizations) {
        var cities = 0;
        var r = new bool[ProbabilisticTSPData.Cities];
        for (var j = 0; j < ProbabilisticTSPData.Cities; j++) {
          if (rng.NextDouble() < ProbabilisticTSPData.GetProbability(j)) {
            r[j] = true;
            cities++;
          }
        }
        if (cities > 0) {
          data.Add(new BoolArray(r, @readonly: true));
        }
      }
      RealizationData = (new ItemList<BoolArray>(data)).AsReadOnly();
    }
  }
}