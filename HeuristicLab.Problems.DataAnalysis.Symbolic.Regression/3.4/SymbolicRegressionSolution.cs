#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  /// <summary>
  /// Represents a symbolic regression solution (model + data) and attributes of the solution like accuracy and complexity
  /// </summary>
  [StorableClass]
  [Item(Name = "SymbolicRegressionSolution", Description = "Represents a symbolic regression solution (model + data) and attributes of the solution like accuracy and complexity.")]
  public sealed class SymbolicRegressionSolution : RegressionSolution, ISymbolicRegressionSolution {
    private const string ModelLengthResultName = "Model Length";
    private const string ModelDepthResultName = "Model Depth";

    private const string EstimationLimitsResultsResultName = "Estimation Limits Results";
    private const string EstimationLimitsResultName = "Estimation Limits";
    private const string TrainingUpperEstimationLimitHitsResultName = "Training Upper Estimation Limit Hits";
    private const string TestLowerEstimationLimitHitsResultName = "Test Lower Estimation Limit Hits";
    private const string TrainingLowerEstimationLimitHitsResultName = "Training Lower Estimation Limit Hits";
    private const string TestUpperEstimationLimitHitsResultName = "Test Upper Estimation Limit Hits";
    private const string TrainingNaNEvaluationsResultName = "Training NaN Evaluations";
    private const string TestNaNEvaluationsResultName = "Test NaN Evaluations";

    public new ISymbolicRegressionModel Model {
      get { return (ISymbolicRegressionModel)base.Model; }
      set { base.Model = value; }
    }
    ISymbolicDataAnalysisModel ISymbolicDataAnalysisSolution.Model {
      get { return (ISymbolicDataAnalysisModel)base.Model; }
    }
    public int ModelLength {
      get { return ((IntValue)this[ModelLengthResultName].Value).Value; }
      private set { ((IntValue)this[ModelLengthResultName].Value).Value = value; }
    }

    public int ModelDepth {
      get { return ((IntValue)this[ModelDepthResultName].Value).Value; }
      private set { ((IntValue)this[ModelDepthResultName].Value).Value = value; }
    }

    private ResultCollection EstimationLimitsResultCollection {
      get { return (ResultCollection)this[EstimationLimitsResultsResultName].Value; }
    }
    public DoubleLimit EstimationLimits {
      get { return (DoubleLimit)EstimationLimitsResultCollection[EstimationLimitsResultName].Value; }
    }

    public int TrainingUpperEstimationLimitHits {
      get { return ((IntValue)EstimationLimitsResultCollection[TrainingUpperEstimationLimitHitsResultName].Value).Value; }
      private set { ((IntValue)EstimationLimitsResultCollection[TrainingUpperEstimationLimitHitsResultName].Value).Value = value; }
    }
    public int TestUpperEstimationLimitHits {
      get { return ((IntValue)EstimationLimitsResultCollection[TestUpperEstimationLimitHitsResultName].Value).Value; }
      private set { ((IntValue)EstimationLimitsResultCollection[TestUpperEstimationLimitHitsResultName].Value).Value = value; }
    }
    public int TrainingLowerEstimationLimitHits {
      get { return ((IntValue)EstimationLimitsResultCollection[TrainingLowerEstimationLimitHitsResultName].Value).Value; }
      private set { ((IntValue)EstimationLimitsResultCollection[TrainingLowerEstimationLimitHitsResultName].Value).Value = value; }
    }
    public int TestLowerEstimationLimitHits {
      get { return ((IntValue)EstimationLimitsResultCollection[TestLowerEstimationLimitHitsResultName].Value).Value; }
      private set { ((IntValue)EstimationLimitsResultCollection[TestLowerEstimationLimitHitsResultName].Value).Value = value; }
    }
    public int TrainingNaNEvaluations {
      get { return ((IntValue)EstimationLimitsResultCollection[TrainingNaNEvaluationsResultName].Value).Value; }
      private set { ((IntValue)EstimationLimitsResultCollection[TrainingNaNEvaluationsResultName].Value).Value = value; }
    }
    public int TestNaNEvaluations {
      get { return ((IntValue)EstimationLimitsResultCollection[TestNaNEvaluationsResultName].Value).Value; }
      private set { ((IntValue)EstimationLimitsResultCollection[TestNaNEvaluationsResultName].Value).Value = value; }
    }

    [StorableConstructor]
    private SymbolicRegressionSolution(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionSolution(SymbolicRegressionSolution original, Cloner cloner)
      : base(original, cloner) {
    }
    public SymbolicRegressionSolution(ISymbolicRegressionModel model, IRegressionProblemData problemData)
      : base(model, problemData) {
      Add(new Result(ModelLengthResultName, "Length of the symbolic regression model.", new IntValue()));
      Add(new Result(ModelDepthResultName, "Depth of the symbolic regression model.", new IntValue()));

      ResultCollection estimationLimitResults = new ResultCollection();
      estimationLimitResults.Add(new Result(EstimationLimitsResultName, "", new DoubleLimit()));
      estimationLimitResults.Add(new Result(TrainingUpperEstimationLimitHitsResultName, "", new IntValue()));
      estimationLimitResults.Add(new Result(TestUpperEstimationLimitHitsResultName, "", new IntValue()));
      estimationLimitResults.Add(new Result(TrainingLowerEstimationLimitHitsResultName, "", new IntValue()));
      estimationLimitResults.Add(new Result(TestLowerEstimationLimitHitsResultName, "", new IntValue()));
      estimationLimitResults.Add(new Result(TrainingNaNEvaluationsResultName, "", new IntValue()));
      estimationLimitResults.Add(new Result(TestNaNEvaluationsResultName, "", new IntValue()));
      Add(new Result(EstimationLimitsResultsResultName, "Results concerning the estimation limits of symbolic regression solution", estimationLimitResults));

      RecalculateResults();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionSolution(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!ContainsKey(EstimationLimitsResultsResultName)) {
        ResultCollection estimationLimitResults = new ResultCollection();
        estimationLimitResults.Add(new Result(EstimationLimitsResultName, "", new DoubleLimit()));
        estimationLimitResults.Add(new Result(TrainingUpperEstimationLimitHitsResultName, "", new IntValue()));
        estimationLimitResults.Add(new Result(TestUpperEstimationLimitHitsResultName, "", new IntValue()));
        estimationLimitResults.Add(new Result(TrainingLowerEstimationLimitHitsResultName, "", new IntValue()));
        estimationLimitResults.Add(new Result(TestLowerEstimationLimitHitsResultName, "", new IntValue()));
        estimationLimitResults.Add(new Result(TrainingNaNEvaluationsResultName, "", new IntValue()));
        estimationLimitResults.Add(new Result(TestNaNEvaluationsResultName, "", new IntValue()));
        Add(new Result(EstimationLimitsResultsResultName, "Results concerning the estimation limits of symbolic regression solution", estimationLimitResults));
        CalculateResults();
      }
    }

    protected override void RecalculateResults() {
      base.RecalculateResults();
      CalculateResults();
    }

    private void CalculateResults() {
      ModelLength = Model.SymbolicExpressionTree.Length;
      ModelDepth = Model.SymbolicExpressionTree.Depth;

      EstimationLimits.Lower = Model.LowerEstimationLimit;
      EstimationLimits.Upper = Model.UpperEstimationLimit;

      TrainingUpperEstimationLimitHits = EstimatedTrainingValues.Count(x => x.IsAlmost(Model.UpperEstimationLimit));
      TestUpperEstimationLimitHits = EstimatedTestValues.Count(x => x.IsAlmost(Model.UpperEstimationLimit));
      TrainingLowerEstimationLimitHits = EstimatedTrainingValues.Count(x => x.IsAlmost(Model.LowerEstimationLimit));
      TestLowerEstimationLimitHits = EstimatedTestValues.Count(x => x.IsAlmost(Model.LowerEstimationLimit));
      TrainingNaNEvaluations = Model.Interpreter.GetSymbolicExpressionTreeValues(Model.SymbolicExpressionTree, ProblemData.Dataset, ProblemData.TrainingIndices).Count(double.IsNaN);
      TestNaNEvaluations = Model.Interpreter.GetSymbolicExpressionTreeValues(Model.SymbolicExpressionTree, ProblemData.Dataset, ProblemData.TestIndices).Count(double.IsNaN);
    }
  }
}
