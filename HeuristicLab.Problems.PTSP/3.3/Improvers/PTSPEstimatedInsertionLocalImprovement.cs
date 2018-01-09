#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.PTSP {
  /// <summary>
  /// An operator that improves probabilistic traveling salesman solutions.
  /// </summary>
  /// <remarks>
  /// The operator tries to improve the probabilistic traveling salesman solution by inserting a city in the tour between two other cities for a certain number of times.
  /// </remarks>
  [Item("PTSP Estimated Insertion Local Improvement", "An operator that improves probabilistic traveling salesman solutions. The operator tries to improve the probabilistic traveling salesman solution by swapping two randomly chosen edges for a certain number of times.")]
  [StorableClass]
  public sealed class PTSPEstimatedInsertionLocalImprovement : SingleSuccessorOperator, IEstimatedPTSPOperator, ILocalImprovementOperator {

    public ILookupParameter<IntValue> LocalIterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["LocalIterations"]; }
    }

    public IValueLookupParameter<IntValue> MaximumIterationsParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["MaximumIterations"]; }
    }

    public ILookupParameter<IntValue> EvaluatedSolutionsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["EvaluatedSolutions"]; }
    }

    public ILookupParameter<ResultCollection> ResultsParameter {
      get { return (ILookupParameter<ResultCollection>)Parameters["Results"]; }
    }

    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public ILookupParameter<DistanceMatrix> DistanceMatrixParameter {
      get { return (ILookupParameter<DistanceMatrix>)Parameters["DistanceMatrix"]; }
    }

    public ILookupParameter<ItemList<BoolArray>> RealizationsParameter {
      get { return (ILookupParameter<ItemList<BoolArray>>)Parameters["Realizations"]; }
    }

    [StorableConstructor]
    private PTSPEstimatedInsertionLocalImprovement(bool deserializing) : base(deserializing) { }
    private PTSPEstimatedInsertionLocalImprovement(PTSPEstimatedInsertionLocalImprovement original, Cloner cloner) : base(original, cloner) { }
    public PTSPEstimatedInsertionLocalImprovement()
      : base() {
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
      Parameters.Add(new LookupParameter<IntValue>("LocalIterations", "The number of iterations that have already been performed."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum amount of iterations that should be performed (note that this operator will abort earlier when a local optimum is reached).", new IntValue(10000)));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The amount of evaluated solutions (here a move is counted only as 4/n evaluated solutions with n being the length of the permutation)."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The collection where to store results."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value of the assignment."));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem should be maximized or minimized."));
      Parameters.Add(new LookupParameter<DistanceMatrix>("DistanceMatrix", "The matrix which contains the distances between the cities."));
      Parameters.Add(new LookupParameter<ItemList<BoolArray>>("Realizations", "The list of samples drawn from all possible stochastic instances."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new PTSPEstimatedInsertionLocalImprovement(this, cloner);
    }

    public static void Improve(Permutation assignment, DoubleMatrix distances, DoubleValue quality, IntValue localIterations, IntValue evaluatedSolutions, bool maximization, int maxIterations, ItemList<BoolArray> realizations, CancellationToken cancellation) {
      var distanceM = (DistanceMatrix)distances;
      Func<int, int, double> distance = (a, b) => distanceM[a, b];
      for (var i = localIterations.Value; i < maxIterations; i++) {
        TranslocationMove bestMove = null;
        double bestQuality = 0; // we have to make an improvement, so 0 is the baseline
        double evaluations = 0.0;
        foreach (var move in ExhaustiveInsertionMoveGenerator.Generate(assignment)) {
          double moveQuality = PTSPEstimatedInsertionMoveEvaluator.EvaluateMove(assignment, move, distance, realizations);
          evaluations += realizations.Count * 6.0 / (assignment.Length * assignment.Length);
          if (maximization && moveQuality > bestQuality
            || !maximization && moveQuality < bestQuality) {
            bestQuality = moveQuality;
            bestMove = move;
          }
        }
        evaluatedSolutions.Value += (int)Math.Ceiling(evaluations);
        if (bestMove == null) break;
        TranslocationManipulator.Apply(assignment, bestMove.Index1, bestMove.Index2, bestMove.Index3);
        quality.Value += bestQuality;
        localIterations.Value++;
        cancellation.ThrowIfCancellationRequested();
      }
    }

    public override IOperation Apply() {
      var maxIterations = MaximumIterationsParameter.ActualValue.Value;
      var assignment = PermutationParameter.ActualValue;
      var maximization = MaximizationParameter.ActualValue.Value;
      var distances = DistanceMatrixParameter.ActualValue;
      var quality = QualityParameter.ActualValue;
      var localIterations = LocalIterationsParameter.ActualValue;
      var evaluations = EvaluatedSolutionsParameter.ActualValue;
      var realizations = RealizationsParameter.ActualValue;
      if (localIterations == null) {
        localIterations = new IntValue(0);
        LocalIterationsParameter.ActualValue = localIterations;
      }

      Improve(assignment, distances, quality, localIterations, evaluations, maximization, maxIterations, realizations, CancellationToken);

      localIterations.Value = 0;
      return base.Apply();
    }
  }
}
