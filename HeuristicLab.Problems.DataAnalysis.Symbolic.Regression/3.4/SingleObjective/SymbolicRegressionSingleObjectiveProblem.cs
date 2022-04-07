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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;
using HeuristicLab.JsonInterface;
using System.Collections.Generic;
using System;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Symbolic Regression Problem (single-objective)", "Represents a single objective symbolic regression problem.")]
  [StorableType("7DDCF683-96FC-4F70-BF4F-FE3A0B0DE6E0")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 100)]
  public class SymbolicRegressionSingleObjectiveProblem : SymbolicDataAnalysisSingleObjectiveProblem<IRegressionProblemData, ISymbolicRegressionSingleObjectiveEvaluator, ISymbolicDataAnalysisSolutionCreator>, IRegressionProblem {
    private const double PunishmentFactor = 10;
    private const int InitialMaximumTreeDepth = 8;
    private const int InitialMaximumTreeLength = 25;
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string EstimationLimitsParameterDescription = "The limits for the estimated value that can be returned by the symbolic regression model.";

    #region parameter properties
    public IFixedValueParameter<DoubleLimit> EstimationLimitsParameter {
      get { return (IFixedValueParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName]; }
    }
    #endregion
    #region properties
    public DoubleLimit EstimationLimits {
      get { return EstimationLimitsParameter.Value; }
    }
    #endregion
    [StorableConstructor]
    protected SymbolicRegressionSingleObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    protected SymbolicRegressionSingleObjectiveProblem(SymbolicRegressionSingleObjectiveProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new SymbolicRegressionSingleObjectiveProblem(this, cloner); }

    public SymbolicRegressionSingleObjectiveProblem() : this(new RegressionProblemData(), new SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator(), new SymbolicDataAnalysisExpressionTreeCreator()) {
    }
    public SymbolicRegressionSingleObjectiveProblem(IRegressionProblemData problemData, ISymbolicRegressionSingleObjectiveEvaluator evaluator, ISymbolicDataAnalysisSolutionCreator solutionCreator) :
      base(problemData, evaluator, solutionCreator) {

      Parameters.Add(new FixedValueParameter<DoubleLimit>(EstimationLimitsParameterName, EstimationLimitsParameterDescription));

      EstimationLimitsParameter.Hidden = true;


      ApplyLinearScalingParameter.Value.Value = true;
      Maximization.Value = true;
      MaximumSymbolicExpressionTreeDepth.Value = InitialMaximumTreeDepth;
      MaximumSymbolicExpressionTreeLength.Value = InitialMaximumTreeLength;

      RegisterEventHandlers();
      ConfigureGrammarSymbols();
      InitializeOperators();
      UpdateEstimationLimits();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
      // compatibility
      bool changed = false;
      if (!Operators.OfType<SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer>().Any()) {
        Operators.Add(new SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer());
        changed = true;
      }
      if (!Operators.OfType<SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer>().Any()) {
        Operators.Add(new SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer());
        changed = true;
      }
      if (!Operators.OfType<SolutionQualityAnalyzer>().Any()) {
        Operators.Add(new SolutionQualityAnalyzer());
        changed = true;
      }

      if (!Operators.OfType<ShapeConstraintsAnalyzer>().Any()) {
        Operators.Add(new ShapeConstraintsAnalyzer());
        changed = true;
      }
      if (changed) {
        ParameterizeOperators();
      }
    }

    private void RegisterEventHandlers() {
      SymbolicExpressionTreeGrammarParameter.ValueChanged += (o, e) => ConfigureGrammarSymbols();
    }

    private void ConfigureGrammarSymbols() {
      var grammar = SymbolicExpressionTreeGrammar as TypeCoherentExpressionGrammar;
      if (grammar != null) grammar.ConfigureAsDefaultRegressionGrammar();
    }

    private void InitializeOperators() {
      Operators.Add(new SymbolicRegressionSingleObjectiveTrainingBestSolutionAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveValidationBestSolutionAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveOverfittingAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveTrainingParetoBestSolutionAnalyzer());
      Operators.Add(new SymbolicRegressionSingleObjectiveValidationParetoBestSolutionAnalyzer());
      Operators.Add(new SolutionQualityAnalyzer());
      Operators.Add(new SymbolicExpressionTreePhenotypicSimilarityCalculator());
      Operators.Add(new ShapeConstraintsAnalyzer());
      Operators.Add(new SymbolicRegressionPhenotypicDiversityAnalyzer(Operators.OfType<SymbolicExpressionTreePhenotypicSimilarityCalculator>()) { DiversityResultName = "Phenotypic Diversity" });
      ParameterizeOperators();
    }

    private void UpdateEstimationLimits() {
      if (ProblemData.TrainingIndices.Any()) {
        var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices).ToList();
        var mean = targetValues.Average();
        var range = targetValues.Max() - targetValues.Min();
        EstimationLimits.Upper = mean + PunishmentFactor * range;
        EstimationLimits.Lower = mean - PunishmentFactor * range;
      } else {
        EstimationLimits.Upper = double.MaxValue;
        EstimationLimits.Lower = double.MinValue;
      }
    }

    protected override void OnProblemDataChanged() {
      base.OnProblemDataChanged();
      UpdateEstimationLimits();
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
      if (Parameters.ContainsKey(EstimationLimitsParameterName)) {
        var operators = Parameters.OfType<IValueParameter>().Select(p => p.Value).OfType<IOperator>().Union(Operators);
        foreach (var op in operators.OfType<ISymbolicDataAnalysisBoundedOperator>()) {
          op.EstimationLimitsParameter.ActualName = EstimationLimitsParameter.Name;
        }
      }

      foreach (var op in Operators.OfType<ISolutionSimilarityCalculator>()) {
        op.SolutionVariableName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.QualityVariableName = Evaluator.QualityParameter.ActualName;

        if (op is SymbolicExpressionTreePhenotypicSimilarityCalculator) {
          var phenotypicSimilarityCalculator = (SymbolicExpressionTreePhenotypicSimilarityCalculator)op;
          phenotypicSimilarityCalculator.ProblemData = ProblemData;
          phenotypicSimilarityCalculator.Interpreter = SymbolicExpressionTreeInterpreter;
        }
      }
    }

    #region IJsonConvertable Members
    private const string TrainingPartitionStartPropertyName = "TrainingPartition.Start";
    private const string TrainingPartitionEndPropertyName = "TrainingPartition.End";
    private const string TestPartitionStartPropertyName = "TestPartition.Start";
    private const string TestPartitionEndPropertyName = "TestPartition.End";

    public override void Inject(JsonItem item, JsonItemConverter converter) {
      var dataset = Dataset.Instantiate(item);
      var allowedInputVariables = item.GetProperty<IEnumerable<string>>(nameof(ProblemData.AllowedInputVariables));
      var targetVariable = item.GetProperty<string>(nameof(ProblemData.TargetVariable));

      var problemData = new RegressionProblemData(dataset, allowedInputVariables, targetVariable);
      problemData.TrainingPartition.Start = item.GetProperty<int>(TrainingPartitionStartPropertyName);
      problemData.TrainingPartition.End = item.GetProperty<int>(TrainingPartitionEndPropertyName);
      problemData.TestPartition.Start = item.GetProperty<int>(TestPartitionStartPropertyName);
      problemData.TestPartition.End = item.GetProperty<int>(TestPartitionEndPropertyName);
      ProblemData = problemData;

      ApplyLinearScaling.Value = item.GetProperty<bool>(nameof(ApplyLinearScaling));
      MaximumSymbolicExpressionTreeDepth.Value = item.GetProperty<int>(nameof(MaximumSymbolicExpressionTreeDepth));
      MaximumSymbolicExpressionTreeLength.Value = item.GetProperty<int>(nameof(MaximumSymbolicExpressionTreeLength));
    }

    public override JsonItem Extract(JsonItemConverter converter) {
      var item = new JsonItem(ItemName, this, converter) {
        Name = Name,
        Description = Description
      };
      if (ProblemData.Dataset is Dataset dataset)
        item.MergeProperties(converter.ConvertToJson(dataset));
      item.AddProperty(nameof(ProblemData.AllowedInputVariables), ProblemData.AllowedInputVariables.ToArray());
      item.AddProperty(nameof(ProblemData.TargetVariable), ProblemData.TargetVariable);
      item.AddProperty(TrainingPartitionStartPropertyName, ProblemData.TrainingPartition.Start);
      item.AddProperty(TrainingPartitionEndPropertyName, ProblemData.TrainingPartition.End);
      item.AddProperty(TestPartitionStartPropertyName, ProblemData.TestPartition.Start);
      item.AddProperty(TestPartitionEndPropertyName, ProblemData.TestPartition.End);
      item.AddProperty(nameof(ApplyLinearScaling), ApplyLinearScaling.Value);
      item.AddProperty(nameof(MaximumSymbolicExpressionTreeDepth), MaximumSymbolicExpressionTreeDepth.Value);
      item.AddProperty(nameof(MaximumSymbolicExpressionTreeLength), MaximumSymbolicExpressionTreeLength.Value);
      return item;
    }
    #endregion
  }
}
