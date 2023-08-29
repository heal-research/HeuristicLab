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

using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.Dynamic;

namespace HeuristicLab.Problems.DataAnalysis.Dynamic; 

[Item("Test Partition Any Time Quality Tracker", "")]
[StorableType("D08F3BF6-182E-4588-BBFD-86E0F849CA48")]
public class AnyTimeTestPartitionQualityTracker : AnyTimeQualityTracker {
  [Storable] private DynamicSymbolicRegressionProblemState latestState;
  
  protected override string PlotResultName { get => "Test Partition Any Time Performance"; }
  
  public AnyTimeTestPartitionQualityTracker() {
  }
  [StorableConstructor] protected AnyTimeTestPartitionQualityTracker(StorableConstructorFlag _) : base(_) { }

  protected AnyTimeTestPartitionQualityTracker(AnyTimeTestPartitionQualityTracker original, Cloner cloner) : base(original, cloner) {
    latestState = cloner.Clone(original.latestState);
  }
  public override IDeepCloneable Clone(Cloner cloner) { return new AnyTimeTestPartitionQualityTracker(this, cloner); }

  public override void OnEvaluation(IItem solution, double quality, long version, long time) {
    if (latestState is null) return;
    
    var state = latestState;
    var tree = (ISymbolicExpressionTree)solution;
    
    double testQuality = state.Problem.Evaluator.Evaluate(
      tree,
      state.Problem.ProblemData, 
      state.GetTestIndices(), 
      state.Problem.SymbolicExpressionTreeInterpreter, 
      state.Problem.ApplyLinearScaling.Value, 
      state.Problem.EstimationLimits?.Lower ?? 0, state.Problem.EstimationLimits?.Upper ?? 1);
    
    base.OnEvaluation(solution, testQuality, version, time);
  }

  public override void OnEpochChange(object state, long version, long time) {
    latestState = (DynamicSymbolicRegressionProblemState)state;
    base.OnEpochChange(state, version, time);
  }
}
