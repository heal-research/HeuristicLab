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

using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HEAL.Attic;
using HeuristicLab.Problems.DataAnalysis;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// 0R classification algorithm.
  /// </summary>
  [Item("ZeroR Classification", "The simplest possible classifier, ZeroR always predicts the majority class.")]
  [StorableType("A2C4BA0A-008B-44EB-B93A-3B53B867F0EA")]
  public sealed class ZeroR : FixedDataAnalysisAlgorithm<IClassificationProblem> {

    [StorableConstructor]
    private ZeroR(StorableConstructorFlag _) : base(_) { }
    private ZeroR(ZeroR original, Cloner cloner)
      : base(original, cloner) {
    }
    public ZeroR()
      : base() {
      Problem = new ClassificationProblem();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ZeroR(this, cloner);
    }

    protected override void Run(CancellationToken cancellationToken) {
      var solution = CreateZeroRSolution(Problem.ProblemData);
      Results.Add(new Result("ZeroR solution", "The simplest possible classifier, ZeroR always predicts the majority class.", solution));
    }

    public static IClassificationSolution CreateZeroRSolution(IClassificationProblemData problemData) {
      var dataset = problemData.Dataset;
      string target = problemData.TargetVariable;
      var targetValues = dataset.GetDoubleValues(target, problemData.TrainingIndices);


      // if multiple classes have the same number of observations then simply take the first one
      var dominantClass = targetValues.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count())
        .MaxItems(kvp => kvp.Value).Select(x => x.Key).First();

      var model = new ConstantModel(dominantClass, target);
      var solution = model.CreateClassificationSolution(problemData);
      return solution;
    }
  }
}
