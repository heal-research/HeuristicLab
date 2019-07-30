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
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Problems.QuadraticAssignment {
  [Item("QAPStochasticScrambleLocalImprovement", "Takes a solution and finds the local optimum with respect to the scramble neighborhood by decending along the steepest gradient.")]
  [StorableType("045B5151-E5DC-4AF3-8CAD-E160E0EE17FF")]
  public class QAPStochasticScrambleLocalImprovement : SingleSuccessorOperator, ILocalImprovementOperator, IStochasticOperator, ISingleObjectiveOperator {

    public ILookupParameter<IntValue> LocalIterationsParameter {
      get { return (ILookupParameter<IntValue>)Parameters["LocalIterations"]; }
    }

    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
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

    public ILookupParameter<Permutation> AssignmentParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Assignment"]; }
    }

    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }

    public ILookupParameter<BoolValue> MaximizationParameter {
      get { return (ILookupParameter<BoolValue>)Parameters["Maximization"]; }
    }

    public ILookupParameter<DoubleMatrix> WeightsParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Weights"]; }
    }

    public ILookupParameter<DoubleMatrix> DistancesParameter {
      get { return (ILookupParameter<DoubleMatrix>)Parameters["Distances"]; }
    }

    public IValueLookupParameter<IntValue> NeighborhoodSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["NeighborhoodSize"]; }
    }

    [StorableConstructor]
    protected QAPStochasticScrambleLocalImprovement(StorableConstructorFlag _) : base(_) { }
    protected QAPStochasticScrambleLocalImprovement(QAPStochasticScrambleLocalImprovement original, Cloner cloner)
      : base(original, cloner) {
    }
    public QAPStochasticScrambleLocalImprovement()
      : base() {
      Parameters.Add(new LookupParameter<IntValue>("LocalIterations", "The number of iterations that have already been performed."));
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator to use."));
      Parameters.Add(new ValueLookupParameter<IntValue>("MaximumIterations", "The maximum amount of iterations that should be performed (note that this operator will abort earlier when a local optimum is reached).", new IntValue(10000)));
      Parameters.Add(new LookupParameter<IntValue>("EvaluatedSolutions", "The amount of evaluated solutions (here a move is counted only as 4/n evaluated solutions with n being the length of the permutation)."));
      Parameters.Add(new LookupParameter<ResultCollection>("Results", "The collection where to store results."));
      Parameters.Add(new LookupParameter<Permutation>("Assignment", "The permutation that is to be locally optimized."));
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality value of the assignment."));
      Parameters.Add(new LookupParameter<BoolValue>("Maximization", "True if the problem should be maximized or minimized."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Weights", "The weights matrix."));
      Parameters.Add(new LookupParameter<DoubleMatrix>("Distances", "The distances matrix."));
      Parameters.Add(new ValueLookupParameter<IntValue>("NeighborhoodSize", "The number of moves to sample from the neighborhood.", new IntValue(100)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new QAPStochasticScrambleLocalImprovement(this, cloner);
    }

    public static void Improve(IRandom random, Permutation assignment, DoubleMatrix weights, DoubleMatrix distances, DoubleValue quality, IntValue localIterations, IntValue evaluatedSolutions, bool maximization, int maxIterations, int neighborhoodSize, CancellationToken cancellation) {
      for (int i = localIterations.Value; i < maxIterations; i++) {
        ScrambleMove bestMove = null;
        double bestQuality = 0; // we have to make an improvement, so 0 is the baseline
        double evaluations = 0.0;
        for (int j = 0; j < neighborhoodSize; j++) {
          var move = StochasticScrambleMultiMoveGenerator.GenerateRandomMove(assignment, random);
          double moveQuality = QAPScrambleMoveEvaluator.Apply(assignment, move, weights, distances);
          evaluations += 2.0 * move.ScrambledIndices.Length / assignment.Length;
          if (maximization && moveQuality > bestQuality
            || !maximization && moveQuality < bestQuality) {
            bestQuality = moveQuality;
            bestMove = move;
          }
        }
        evaluatedSolutions.Value += (int)Math.Ceiling(evaluations);
        if (bestMove == null) break;
        ScrambleManipulator.Apply(assignment, bestMove.StartIndex, bestMove.ScrambledIndices);
        quality.Value += bestQuality;
        localIterations.Value++;
        cancellation.ThrowIfCancellationRequested();
      }
    }

    public override IOperation Apply() {
      var random = RandomParameter.ActualValue;
      var maxIterations = MaximumIterationsParameter.ActualValue.Value;
      var neighborhoodSize = NeighborhoodSizeParameter.ActualValue.Value;
      var assignment = AssignmentParameter.ActualValue;
      var maximization = MaximizationParameter.ActualValue.Value;
      var weights = WeightsParameter.ActualValue;
      var distances = DistancesParameter.ActualValue;
      var quality = QualityParameter.ActualValue;
      var localIterations = LocalIterationsParameter.ActualValue;
      var evaluations = EvaluatedSolutionsParameter.ActualValue;
      if (localIterations == null) {
        localIterations = new IntValue(0);
        LocalIterationsParameter.ActualValue = localIterations;
      }

      Improve(random, assignment, weights, distances, quality, localIterations, evaluations, maximization, maxIterations, neighborhoodSize, CancellationToken);

      localIterations.Value = 0;
      return base.Apply();
    }
  }
}
