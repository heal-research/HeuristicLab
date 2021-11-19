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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression {
  [Item("Shape-constrained symbolic regression problem (multi-objective)", "Represents a multi-objective shape-constrained regression problem.")]
  [StorableType("2956C66F-4B71-4A62-998F-B52C5E8C02CD")]
  [Creatable(CreatableAttribute.Categories.GeneticProgrammingProblems, Priority = 150)]
  public class ShapeConstrainedRegressionMultiObjectiveProblem : SymbolicDataAnalysisMultiObjectiveProblem<IRegressionProblemData, IMultiObjectiveConstraintsEvaluator>, IRegressionProblem {
    private const double PunishmentFactor = 10;
    private const int InitialMaximumTreeDepth = 8;
    private const int InitialMaximumTreeLength = 25;
    private const string EstimationLimitsParameterName = "EstimationLimits";
    private const string EstimationLimitsParameterDescription = "The lower and upper limit for the estimated value that can be returned by the symbolic regression model.";

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
    protected ShapeConstrainedRegressionMultiObjectiveProblem(StorableConstructorFlag _) : base(_) { }
    protected ShapeConstrainedRegressionMultiObjectiveProblem(ShapeConstrainedRegressionMultiObjectiveProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) { return new ShapeConstrainedRegressionMultiObjectiveProblem(this, cloner); }

    public ShapeConstrainedRegressionMultiObjectiveProblem()
      : base(new ShapeConstrainedRegressionProblemData(), new NMSEMultiObjectiveConstraintsEvaluator()) {

      Parameters.Add(new FixedValueParameter<DoubleLimit>(EstimationLimitsParameterName, EstimationLimitsParameterDescription));
      EstimationLimitsParameter.Hidden = true;

      ApplyLinearScalingParameter.Value.Value = true;
      SymbolicExpressionTreeGrammarParameter.Value = new LinearScalingGrammar();

      MaximumSymbolicExpressionTreeDepth.Value = InitialMaximumTreeDepth;
      MaximumSymbolicExpressionTreeLength.Value = InitialMaximumTreeLength;

      InitializeOperators();
      UpdateEstimationLimits();
      UpdateMaximization();
      RegisterEventHandlers();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      Evaluator.NumConstraintsParameter.Value.ValueChanged += NumConstraintsParameter_ValueChanged;
    }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      UpdateEvaluatorObjectives(); // update objectives in evaluator based ProblemData
      Evaluator.NumConstraintsParameter.Value.ValueChanged += NumConstraintsParameter_ValueChanged;
    }
    protected override void OnProblemDataChanged() {
      base.OnProblemDataChanged();

      UpdateEstimationLimits();
      UpdateMaximization();
      UpdateEvaluatorObjectives();
    }

    private void NumConstraintsParameter_ValueChanged(object sender, EventArgs e) {
      UpdateMaximization();
    }

    private void UpdateMaximization() {
      Maximization = new BoolArray(Evaluator.Maximization.ToArray());
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
    private void UpdateEvaluatorObjectives() {
      if (ProblemData is ShapeConstrainedRegressionProblemData scProblemData) {
        Evaluator.NumConstraintsParameter.Value.Value = scProblemData.ShapeConstraints.EnabledConstraints.Count();
      } else {
        Evaluator.NumConstraintsParameter.Value.Value = 0;
      }
    }

    private void InitializeOperators() {
      Operators.Add(new SymbolicRegressionMultiObjectiveTrainingBestSolutionAnalyzer());
      Operators.Add(new SymbolicRegressionMultiObjectiveValidationBestSolutionAnalyzer());
      Operators.Add(new SymbolicExpressionTreePhenotypicSimilarityCalculator());
      Operators.Add(new SymbolicRegressionPhenotypicDiversityAnalyzer(Operators.OfType<SymbolicExpressionTreePhenotypicSimilarityCalculator>()));
      ParameterizeOperators();
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
        //ToDo Change to encoding.name
        //op.SolutionVariableName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.QualityVariableName = Evaluator.QualitiesParameter.ActualName;

        if (op is SymbolicExpressionTreePhenotypicSimilarityCalculator) {
          var phenotypicSimilarityCalculator = (SymbolicExpressionTreePhenotypicSimilarityCalculator)op;
          phenotypicSimilarityCalculator.ProblemData = ProblemData;
          phenotypicSimilarityCalculator.Interpreter = SymbolicExpressionTreeInterpreter;
        }
      }
    }


    public override void Load(IRegressionProblemData data) {
      var scProblemData = new ShapeConstrainedRegressionProblemData(data.Dataset, data.AllowedInputVariables, data.TargetVariable,
                                                                    data.TrainingPartition, data.TestPartition) {
        Name = data.Name,
        Description = data.Description
      };

      base.Load(scProblemData);
    }
  }
}
