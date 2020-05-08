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

using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;

namespace HeuristicLab.Problems.PTSP {
  [Item("Analytical Probabilistic TSP (pTSP)", "Represents a probabilistic traveling salesman problem where the expected tour length is calculated exactly.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems)]
  [StorableType("509B6AB5-F4DE-4144-A031-43EEBAD02CA6")]
  public sealed class AnalyticalPTSP : ProbabilisticTSP {

    [StorableConstructor]
    private AnalyticalPTSP(StorableConstructorFlag _) : base(_) { }
    private AnalyticalPTSP(AnalyticalPTSP original, Cloner cloner) : base(original, cloner) { }
    public AnalyticalPTSP() {
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

      Encoding.ConfigureOperators(Operators);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AnalyticalPTSP(this, cloner);
    }

    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Encoding.ConfigureOperators(Operators);
    }

    public override ISingleObjectiveEvaluationResult Evaluate(Permutation tour, IRandom random, CancellationToken cancellationToken) {
      var quality = Evaluate(tour, ProbabilisticTSPData, cancellationToken);
      return new SingleObjectiveEvaluationResult(quality);
    }

    public static double Evaluate(Permutation tour, IProbabilisticTSPData data, CancellationToken cancellationToken) {
      // Analytical evaluation
      var firstSum = 0.0;
      for (var i = 0; i < tour.Length - 1; i++) {
        for (var j = i + 1; j < tour.Length; j++) {
          var prod1 = data.GetDistance(tour[i], tour[j]) * data.GetProbability(tour[i]) * data.GetProbability(tour[j]);
          for (var k = i + 1; k < j; k++) {
            prod1 *= (1 - data.GetProbability(tour[k]));
          }
          firstSum += prod1;
        }
      }
      var secondSum = 0.0;
      for (var j = 0; j < tour.Length; j++) {
        for (var i = 0; i < j; i++) {
          var prod2 = data.GetDistance(tour[j], tour[i]) * data.GetProbability(tour[i]) * data.GetProbability(tour[j]);
          for (var k = j + 1; k < tour.Length; k++) {
            prod2 *= (1 - data.GetProbability(tour[k]));
          }
          for (var k = 0; k < i; k++) {
            prod2 *= (1 - data.GetProbability(tour[k]));
          }
          secondSum += prod2;
        }
      }
      return firstSum + secondSum;
    }
  }
}
