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

using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  /// <summary>
  /// An operator that analyzes the training best symbolic regression solution for multi objective symbolic regression problems.
  /// </summary>
  [Item("SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer", "An operator that analyzes the training best symbolic regression solution for multi objective symbolic regression problems.")]
  [StorableClass]
  public sealed class SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer : SymbolicDataAnalysisMultiObjectiveTrainingBestSolutionAnalyzer<ISymbolicRegressionSolution>,
    ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator {
    private const string ProblemDataParameterName = "ProblemData";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string ValidationPartitionParameterName = "ValidationPartition";
    private const string AnalyzeTestErrorParameterName = "Analyze Test Error";

    #region parameter properties
    public ILookupParameter<IRegressionProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IRegressionProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicDataAnalysisTreeInterpreterParameter {
      get { return (ILookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicDataAnalysisTreeInterpreterParameterName]; }
    }
    public IValueLookupParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IValueLookupParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    public ILookupParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (ILookupParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }

    public IValueLookupParameter<IntRange> ValidationPartitionParameter {
      get { return (IValueLookupParameter<IntRange>)Parameters[ValidationPartitionParameterName]; }
    }

    public IFixedValueParameter<BoolValue> AnalyzeTestErrorParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[AnalyzeTestErrorParameterName]; }
    }
    #endregion

    public bool AnalyzeTestError {
      get { return AnalyzeTestErrorParameter.Value.Value; }
      set { AnalyzeTestErrorParameter.Value.Value = value; }
    }

    [StorableConstructor]
    private SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer(SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IRegressionProblemData>(ProblemDataParameterName, "The problem data for the symbolic regression solution.") { Hidden = true });
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The symbolic data analysis tree interpreter for the symbolic expression tree.") { Hidden = true });
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic regression model.") { Hidden = true });
      Parameters.Add(new LookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "Maximal length of the symbolic expression.") { Hidden = true });
      Parameters.Add(new ValueLookupParameter<IntRange>(ValidationPartitionParameterName, "The validation partition."));
      Parameters.Add(new FixedValueParameter<BoolValue>(AnalyzeTestErrorParameterName, "Flag whether the test error should be displayed in the Pareto-Front", new BoolValue(false)));
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(MaximumSymbolicExpressionTreeLengthParameterName))
        Parameters.Add(new LookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "Maximal length of the symbolic expression.") { Hidden = true });
      if (!Parameters.ContainsKey(ValidationPartitionParameterName))
        Parameters.Add(new ValueLookupParameter<IntRange>(ValidationPartitionParameterName, "The validation partition."));
      if (!Parameters.ContainsKey(AnalyzeTestErrorParameterName))
        Parameters.Add(new FixedValueParameter<BoolValue>(AnalyzeTestErrorParameterName, "Flag whether the test error should be displayed in the Pareto-Front", new BoolValue(false)));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer(this, cloner);
    }

    protected override ISymbolicRegressionSolution CreateSolution(ISymbolicExpressionTree bestTree, double[] bestQuality) {
      var model = new SymbolicRegressionModel(ProblemDataParameter.ActualValue.TargetVariable, (ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScalingParameter.ActualValue.Value) model.Scale(ProblemDataParameter.ActualValue);
      return new SymbolicRegressionSolution(model, (IRegressionProblemData)ProblemDataParameter.ActualValue.Clone());
    }

    public override IOperation Apply() {
      var operation = base.Apply();
      var paretoFront = TrainingBestSolutionsParameter.ActualValue;

      IResult result;
      ScatterPlot qualityToTreeSize;
      if (!ResultCollection.TryGetValue("Pareto Front Analysis", out result)) {
        qualityToTreeSize = new ScatterPlot("Quality vs Tree Size", "");
        qualityToTreeSize.VisualProperties.XAxisMinimumAuto = false;
        qualityToTreeSize.VisualProperties.XAxisMaximumAuto = false;
        qualityToTreeSize.VisualProperties.YAxisMinimumAuto = false;
        qualityToTreeSize.VisualProperties.YAxisMaximumAuto = false;

        qualityToTreeSize.VisualProperties.XAxisMinimumFixedValue = 0;
        qualityToTreeSize.VisualProperties.XAxisMaximumFixedValue = MaximumSymbolicExpressionTreeLengthParameter.ActualValue.Value;
        qualityToTreeSize.VisualProperties.YAxisMinimumFixedValue = 0;
        qualityToTreeSize.VisualProperties.YAxisMaximumFixedValue = 2;
        ResultCollection.Add(new Result("Pareto Front Analysis", qualityToTreeSize));
      } else {
        qualityToTreeSize = (ScatterPlot)result.Value;
      }


      int previousTreeLength = -1;
      var sizeParetoFront = new LinkedList<ISymbolicRegressionSolution>();
      foreach (var solution in paretoFront.OrderBy(s => s.Model.SymbolicExpressionTree.Length)) {
        int treeLength = solution.Model.SymbolicExpressionTree.Length;
        if (!sizeParetoFront.Any()) sizeParetoFront.AddLast(solution);
        if (solution.TrainingNormalizedMeanSquaredError < sizeParetoFront.Last.Value.TrainingNormalizedMeanSquaredError) {
          if (treeLength == previousTreeLength)
            sizeParetoFront.RemoveLast();
          sizeParetoFront.AddLast(solution);
        }
        previousTreeLength = treeLength;
      }

      qualityToTreeSize.Rows.Clear();
      var trainingRow = new ScatterPlotDataRow("Training NMSE", "", sizeParetoFront.Select(x => new Point2D<double>(x.Model.SymbolicExpressionTree.Length, x.TrainingNormalizedMeanSquaredError, x)));
      trainingRow.VisualProperties.PointSize = 8;
      qualityToTreeSize.Rows.Add(trainingRow);

      if (AnalyzeTestError) {
        var testRow = new ScatterPlotDataRow("Test NMSE", "",
          sizeParetoFront.Select(x => new Point2D<double>(x.Model.SymbolicExpressionTree.Length, x.TestNormalizedMeanSquaredError, x)));
        testRow.VisualProperties.PointSize = 8;
        qualityToTreeSize.Rows.Add(testRow);
      }

      var validationPartition = ValidationPartitionParameter.ActualValue;
      if (validationPartition.Size != 0) {
        var problemData = ProblemDataParameter.ActualValue;
        var validationIndizes = Enumerable.Range(validationPartition.Start, validationPartition.Size).ToList();
        var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, validationIndizes).ToList();
        OnlineCalculatorError error;
        var validationRow = new ScatterPlotDataRow("Validation NMSE", "",
          sizeParetoFront.Select(x => new Point2D<double>(x.Model.SymbolicExpressionTree.Length,
          OnlineNormalizedMeanSquaredErrorCalculator.Calculate(targetValues, x.GetEstimatedValues(validationIndizes), out error))));
        validationRow.VisualProperties.PointSize = 7;
        qualityToTreeSize.Rows.Add(validationRow);
      }

      return operation;
    }

  }
}
