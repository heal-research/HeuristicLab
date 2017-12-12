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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.PTSP {
  [Item("Analytical Probabilistic Traveling Salesman Problem (PTSP)", "Represents a probabilistic traveling salesman problem where the expected tour length is calculated exactly.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems)]
  [StorableClass]
  public sealed class AnalyticalProbabilisticTravelingSalesmanProblem : ProbabilisticTravelingSalesmanProblem {

    [StorableConstructor]
    private AnalyticalProbabilisticTravelingSalesmanProblem(bool deserializing) : base(deserializing) { }
    private AnalyticalProbabilisticTravelingSalesmanProblem(AnalyticalProbabilisticTravelingSalesmanProblem original, Cloner cloner) : base(original, cloner) { }
    public AnalyticalProbabilisticTravelingSalesmanProblem() {
      Operators.Add(new BestPTSPSolutionAnalyzer());

      Operators.Add(new PTSPAnalyticalInversionMoveEvaluator());
      Operators.Add(new PTSPAnalyticalInsertionMoveEvaluator());
      Operators.Add(new PTSPAnalyticalInversionLocalImprovement());
      Operators.Add(new PTSPAnalyticalInsertionLocalImprovement());
      Operators.Add(new PTSPAnalyticalTwoPointFiveLocalImprovement());

      Operators.Add(new ExhaustiveTwoPointFiveMoveGenerator());
      Operators.Add(new StochasticTwoPointFiveMultiMoveGenerator());
      Operators.Add(new StochasticTwoPointFiveSingleMoveGenerator());
      Operators.Add(new TwoPointFiveMoveMaker());
      Operators.Add(new PTSPAnalyticalTwoPointFiveMoveEvaluator());

      Operators.RemoveAll(x => x is SingleObjectiveMoveGenerator);
      Operators.RemoveAll(x => x is SingleObjectiveMoveMaker);
      Operators.RemoveAll(x => x is SingleObjectiveMoveEvaluator);

      Encoding.ConfigureOperators(Operators.OfType<IOperator>());
      foreach (var twopointfiveMoveOperator in Operators.OfType<ITwoPointFiveMoveOperator>()) {
        twopointfiveMoveOperator.TwoPointFiveMoveParameter.ActualName = "Permutation.TwoPointFiveMove";
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AnalyticalProbabilisticTravelingSalesmanProblem(this, cloner);
    }

    public override double Evaluate(Permutation tour, IRandom random) {
      // abeham: Cache in local variable for performance reasons
      var distanceMatrix = DistanceMatrix;
      return Evaluate(tour, (a, b) => distanceMatrix[a, b], Probabilities);
    }

    public static double Evaluate(Permutation tour, Func<int, int, double> distance, DoubleArray probabilities) {
      // Analytical evaluation
      var firstSum = 0.0;
      for (var i = 0; i < tour.Length - 1; i++) {
        for (var j = i + 1; j < tour.Length; j++) {
          var prod1 = distance(tour[i], tour[j]) * probabilities[tour[i]] * probabilities[tour[j]];
          for (var k = i + 1; k < j; k++) {
            prod1 = prod1 * (1 - probabilities[tour[k]]);
          }
          firstSum += prod1;
        }
      }
      var secondSum = 0.0;
      for (var j = 0; j < tour.Length; j++) {
        for (var i = 0; i < j; i++) {
          var prod2 = distance(tour[j], tour[i]) * probabilities[tour[i]] * probabilities[tour[j]];
          for (var k = j + 1; k < tour.Length; k++) {
            prod2 = prod2 * (1 - probabilities[tour[k]]);
          }
          for (var k = 0; k < i; k++) {
            prod2 = prod2 * (1 - probabilities[tour[k]]);
          }
          secondSum += prod2;
        }
      }
      return firstSum + secondSum;
    }

    public static double Evaluate(Permutation tour, DistanceMatrix distanceMatrix, DoubleArray probabilities) {
      return Evaluate(tour, (a, b) => distanceMatrix[a, b], probabilities);
    }
  }
}
