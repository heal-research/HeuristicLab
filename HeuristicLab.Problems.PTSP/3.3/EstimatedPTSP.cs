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

using System;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Fossil;
using HeuristicLab.Problems.Instances;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.PTSP {
  [Item("Estimated Probabilistic Traveling Salesman Problem (PTSP)", "Represents a probabilistic traveling salesman problem where the expected tour length is estimated by averaging over the length of tours on a number of, so called, realizations.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems)]
  [StorableType("D1F1DE71-54E3-40B6-856F-685CD71D97F9")]
  public sealed class EstimatedProbabilisticTravelingSalesmanProblem : ProbabilisticTravelingSalesmanProblem {

    #region Parameter Properties
    public IValueParameter<ItemList<BoolArray>> RealizationsParameter {
      get { return (IValueParameter<ItemList<BoolArray>>)Parameters["Realizations"]; }
    }
    public IFixedValueParameter<IntValue> RealizationsSizeParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters["RealizationsSize"]; }
    }
    #endregion

    #region Properties
    public ItemList<BoolArray> Realizations {
      get { return RealizationsParameter.Value; }
      set { RealizationsParameter.Value = value; }
    }

    public int RealizationsSize {
      get { return RealizationsSizeParameter.Value.Value; }
      set { RealizationsSizeParameter.Value.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private EstimatedProbabilisticTravelingSalesmanProblem(StorableConstructorFlag _) : base(_) { }
    private EstimatedProbabilisticTravelingSalesmanProblem(EstimatedProbabilisticTravelingSalesmanProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public EstimatedProbabilisticTravelingSalesmanProblem() {
      Parameters.Add(new FixedValueParameter<IntValue>("RealizationsSize", "Size of the sample for the estimation-based evaluation", new IntValue(100)));
      Parameters.Add(new ValueParameter<ItemList<BoolArray>>("Realizations", "The list of samples drawn from all possible stochastic instances.", new ItemList<BoolArray>()));

      Operators.Add(new BestPTSPSolutionAnalyzer());

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

      Operators.RemoveAll(x => x is SingleObjectiveMoveGenerator);
      Operators.RemoveAll(x => x is SingleObjectiveMoveMaker);
      Operators.RemoveAll(x => x is SingleObjectiveMoveEvaluator);

      Encoding.ConfigureOperators(Operators.OfType<IOperator>());
      foreach (var twopointfiveMoveOperator in Operators.OfType<ITwoPointFiveMoveOperator>()) {
        twopointfiveMoveOperator.TwoPointFiveMoveParameter.ActualName = "Permutation.TwoPointFiveMove";
      }

      UpdateRealizations();
      RegisterEventHandlers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new EstimatedProbabilisticTravelingSalesmanProblem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      RealizationsSizeParameter.Value.ValueChanged += RealizationsSizeParameter_ValueChanged;
    }

    private void RealizationsSizeParameter_ValueChanged(object sender, EventArgs e) {
      UpdateRealizations();
    }

    public override double Evaluate(Permutation tour, IRandom random) {
      // abeham: Cache parameters in local variables for performance reasons
      var realizations = Realizations;
      var realizationsSize = RealizationsSize;
      var useDistanceMatrix = UseDistanceMatrix;
      var distanceMatrix = DistanceMatrix;
      var distanceCalculator = DistanceCalculator;
      var coordinates = Coordinates;

      // Estimation-based evaluation, here without calculating variance for faster evaluation
      var estimatedSum = 0.0;
      for (var i = 0; i < realizations.Count; i++) {
        int singleRealization = -1, firstNode = -1;
        for (var j = 0; j < realizations[i].Length; j++) {
          if (realizations[i][tour[j]]) {
            if (singleRealization != -1) {
              estimatedSum += useDistanceMatrix ? distanceMatrix[singleRealization, tour[j]] : distanceCalculator.Calculate(singleRealization, tour[j], coordinates);
            } else {
              firstNode = tour[j];
            }
            singleRealization = tour[j];
          }
        }
        if (singleRealization != -1) {
          estimatedSum += useDistanceMatrix ? distanceMatrix[singleRealization, firstNode] : distanceCalculator.Calculate(singleRealization, firstNode, coordinates);
        }
      }
      return estimatedSum / realizationsSize;
    }

    /// <summary>
    /// An evaluate method that can be used if mean as well as variance should be calculated
    /// </summary>
    /// <param name="tour">The tour between all cities.</param>
    /// <param name="distanceMatrix">The distances between the cities.</param>
    /// <param name="realizations">A sample of realizations of the stochastic instance</param>
    /// <param name="variance">The estimated variance will be returned in addition to the mean.</param>
    /// <returns>A vector with length two containing mean and variance.</returns>
    public static double Evaluate(Permutation tour, DistanceMatrix distanceMatrix, ItemList<BoolArray> realizations, out double variance) {
      return Evaluate(tour, (a, b) => distanceMatrix[a, b], realizations, out variance);
    }

    /// <summary>
    /// An evaluate method that can be used if mean as well as variance should be calculated
    /// </summary>
    /// <param name="tour">The tour between all cities.</param>
    /// <param name="distance">A func that accepts the index of two cities and returns the distance as a double.</param>
    /// <param name="realizations">A sample of realizations of the stochastic instance</param>
    /// <param name="variance">The estimated variance will be returned in addition to the mean.</param>
    /// <returns>A vector with length two containing mean and variance.</returns>
    public static double Evaluate(Permutation tour, Func<int, int, double> distance, ItemList<BoolArray> realizations, out double variance) {
      // Estimation-based evaluation
      var estimatedSum = 0.0;
      var partialSums = new double[realizations.Count];
      for (var i = 0; i < realizations.Count; i++) {
        partialSums[i] = 0;
        int singleRealization = -1, firstNode = -1;
        for (var j = 0; j < realizations[i].Length; j++) {
          if (realizations[i][tour[j]]) {
            if (singleRealization != -1) {
              partialSums[i] += distance(singleRealization, tour[j]);
            } else {
              firstNode = tour[j];
            }
            singleRealization = tour[j];
          }
        }
        if (singleRealization != -1) {
          partialSums[i] += distance(singleRealization, firstNode);
        }
        estimatedSum += partialSums[i];
      }
      var mean = estimatedSum / realizations.Count;
      variance = 0.0;
      for (var i = 0; i < realizations.Count; i++) {
        variance += Math.Pow((partialSums[i] - mean), 2);
      }
      variance = variance / realizations.Count;
      return mean;
    }

    public override void Load(PTSPData data) {
      base.Load(data);
      UpdateRealizations();

      foreach (var op in Operators.OfType<IEstimatedPTSPOperator>()) {
        op.RealizationsParameter.ActualName = RealizationsParameter.Name;
      }
    }

    private void UpdateRealizations() {
      var realizations = new ItemList<BoolArray>(RealizationsSize);
      var rand = new MersenneTwister();
      for (var i = 0; i < RealizationsSize; i++) {
        var newRealization = new BoolArray(Probabilities.Length);
        var countOnes = 0;
        do {
          countOnes = 0;
          for (var j = 0; j < Probabilities.Length; j++) {
            newRealization[j] = Probabilities[j] < rand.NextDouble();
            if (newRealization[j]) countOnes++;
          }
          // only generate realizations with at least 4 cities visited
        } while (countOnes < 4 && Probabilities.Length > 3);
        realizations.Add(newRealization);
      }
      Realizations = realizations;
    }
  }
}