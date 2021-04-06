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

using System.Collections.Generic;
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Classification {
  /// <summary>
  /// An operator that analyzes the training best symbolic classification solution for multi objective symbolic classification problems.
  /// </summary>
  [Item("SymbolicClassificationMultiObjectiveTrainingBestSolutionAnalyzer", "An operator that analyzes the training best symbolic classification solution for multi objective symbolic classification problems.")]
  [StorableType("EC30DC99-A5A8-43B0-81C1-BA9016A0A74C")]
  public sealed class SymbolicClassificationMultiObjectiveTrainingBestSolutionAnalyzer : SymbolicDataAnalysisMultiObjectiveTrainingBestSolutionAnalyzer<ISymbolicClassificationSolution>,
    ISymbolicDataAnalysisInterpreterOperator, ISymbolicDataAnalysisBoundedOperator, ISymbolicClassificationModelCreatorOperator {
    private const string ProblemDataParameterName = "ProblemData";
    private const string ModelCreatorParameterName = "ModelCreator";
    private const string SymbolicDataAnalysisTreeInterpreterParameterName = "SymbolicDataAnalysisTreeInterpreter";
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string ValidationPartitionParameterName = "ValidationPartition";
    private const string AnalyzeTestErrorParameterName = "Analyze Test Error";

    #region parameter properties
    public ILookupParameter<IClassificationProblemData> ProblemDataParameter {
      get { return (ILookupParameter<IClassificationProblemData>)Parameters[ProblemDataParameterName]; }
    }
    public IValueLookupParameter<ISymbolicClassificationModelCreator> ModelCreatorParameter {
      get { return (IValueLookupParameter<ISymbolicClassificationModelCreator>)Parameters[ModelCreatorParameterName]; }
    }
    ILookupParameter<ISymbolicClassificationModelCreator> ISymbolicClassificationModelCreatorOperator.ModelCreatorParameter {
      get { return ModelCreatorParameter; }
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
    public bool AnalyzeTestError {
      get { return AnalyzeTestErrorParameter.Value.Value; }
      set { AnalyzeTestErrorParameter.Value.Value = value; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicClassificationMultiObjectiveTrainingBestSolutionAnalyzer(StorableConstructorFlag _) : base(_) { }
    private SymbolicClassificationMultiObjectiveTrainingBestSolutionAnalyzer(SymbolicClassificationMultiObjectiveTrainingBestSolutionAnalyzer original, Cloner cloner) : base(original, cloner) { }
    public SymbolicClassificationMultiObjectiveTrainingBestSolutionAnalyzer()
      : base() {
      Parameters.Add(new LookupParameter<IClassificationProblemData>(ProblemDataParameterName, "The problem data for the symbolic classification solution."));
      Parameters.Add(new ValueLookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName, ""));
      Parameters.Add(new LookupParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicDataAnalysisTreeInterpreterParameterName, "The symbolic data analysis tree interpreter for the symbolic expression tree."));
      Parameters.Add(new ValueLookupParameter<DoubleLimit>(EstimationLimitsParameterName, "The lower and upper limit for the estimated values produced by the symbolic classification model."));
      Parameters.Add(new LookupParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, "Maximal length of the symbolic expression.") { Hidden = true });
      Parameters.Add(new ValueLookupParameter<IntRange>(ValidationPartitionParameterName, "The validation partition."));
      Parameters.Add(new FixedValueParameter<BoolValue>(AnalyzeTestErrorParameterName, "Flag whether the test error should be displayed in the Pareto-Front", new BoolValue(false)));

    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationMultiObjectiveTrainingBestSolutionAnalyzer(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.4
      #region Backwards compatible code, remove with 3.5
      if (!Parameters.ContainsKey(ModelCreatorParameterName))
        Parameters.Add(new ValueLookupParameter<ISymbolicClassificationModelCreator>(ModelCreatorParameterName, ""));
      #endregion
    }

    protected override ISymbolicClassificationSolution CreateSolution(ISymbolicExpressionTree bestTree, double[] bestQuality) {
      var model = ModelCreatorParameter.ActualValue.CreateSymbolicClassificationModel(ProblemDataParameter.ActualValue.TargetVariable, (ISymbolicExpressionTree)bestTree.Clone(), SymbolicDataAnalysisTreeInterpreterParameter.ActualValue, EstimationLimitsParameter.ActualValue.Lower, EstimationLimitsParameter.ActualValue.Upper);
      if (ApplyLinearScalingParameter.ActualValue.Value) model.Scale(ProblemDataParameter.ActualValue);

      model.RecalculateModelParameters(ProblemDataParameter.ActualValue, ProblemDataParameter.ActualValue.TrainingIndices);
      return model.CreateClassificationSolution((IClassificationProblemData)ProblemDataParameter.ActualValue.Clone());
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
        qualityToTreeSize.VisualProperties.YAxisMaximumFixedValue = 1;
        ResultCollection.Add(new Result("Pareto Front Analysis", qualityToTreeSize));
      } else {
        qualityToTreeSize = (ScatterPlot)result.Value;
      }

      int previousTreeLength = -1;
      var sizeParetoFront = new LinkedList<ISymbolicClassificationSolution>();
      foreach (var solution in paretoFront.OrderBy(s => s.Model.SymbolicExpressionTree.Length)) {
        int treeLength = solution.Model.SymbolicExpressionTree.Length;
        if (!sizeParetoFront.Any()) sizeParetoFront.AddLast(solution);
        if (solution.TrainingAccuracy > sizeParetoFront.Last.Value.TrainingAccuracy) {
          if (treeLength == previousTreeLength)
            sizeParetoFront.RemoveLast();
          sizeParetoFront.AddLast(solution);
        }
        previousTreeLength = treeLength;
      }

      qualityToTreeSize.Rows.Clear();
      var trainingRow = new ScatterPlotDataRow("Training Accuracy", "", sizeParetoFront.Select(x => new Point2D<double>(x.Model.SymbolicExpressionTree.Length, x.TrainingAccuracy, x)));
      trainingRow.VisualProperties.PointSize = 8;
      qualityToTreeSize.Rows.Add(trainingRow);

      if (AnalyzeTestError) {
        var testRow = new ScatterPlotDataRow("Test Accuracy", "",
          sizeParetoFront.Select(x => new Point2D<double>(x.Model.SymbolicExpressionTree.Length, x.TestAccuracy, x)));
        testRow.VisualProperties.PointSize = 8;
        qualityToTreeSize.Rows.Add(testRow);
      }

      var validationPartition = ValidationPartitionParameter.ActualValue;
      if (validationPartition.Size != 0) {
        var problemData = ProblemDataParameter.ActualValue;
        var validationIndizes = Enumerable.Range(validationPartition.Start, validationPartition.Size).ToList();
        var targetValues = problemData.Dataset.GetDoubleValues(problemData.TargetVariable, validationIndizes).ToList();
        OnlineCalculatorError error;
        var validationRow = new ScatterPlotDataRow("Validation Accuracy", "",
          sizeParetoFront.Select(x => new Point2D<double>(x.Model.SymbolicExpressionTree.Length,
          OnlineAccuracyCalculator.Calculate(targetValues, x.GetEstimatedClassValues(validationIndizes), out error))));
        validationRow.VisualProperties.PointSize = 7;
        qualityToTreeSize.Rows.Add(validationRow);
      }

      return operation;
    }
  }
}
