#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("Symbolic Regression Problem (single objective)", "Represents a single objective symbolic regression problem.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class SymbolicRegressionProblem : SymbolicRegressionProblemBase, ISingleObjectiveDataAnalysisProblem {

    #region Parameter Properties
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public new ValueParameter<ISymbolicRegressionEvaluator> EvaluatorParameter {
      get { return (ValueParameter<ISymbolicRegressionEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IHeuristicOptimizationProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    #endregion

    #region Properties
    public new ISymbolicRegressionEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IHeuristicOptimizationProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    private SymbolicRegressionProblem(bool deserializing) : base(deserializing) { }
    private SymbolicRegressionProblem(SymbolicRegressionProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    public SymbolicRegressionProblem()
      : base() {
      var evaluator = new SymbolicRegressionPearsonsRSquaredEvaluator();
      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as the error of the regression model should be minimized.", (BoolValue)new BoolValue(true)));
      Parameters.Add(new ValueParameter<ISymbolicRegressionEvaluator>("Evaluator", "The operator which should be used to evaluate symbolic regression solutions.", evaluator));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The minimal error value that reached by symbolic regression solutions for the problem."));

      InitializeOperators();
      ParameterizeEvaluator();

      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicRegressionProblem(this, cloner);
    }

    private void RegisterParameterValueEvents() {
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
    }

    private void RegisterParameterEvents() { }

    #region event handling
    protected override void OnDataAnalysisProblemChanged(EventArgs e) {
      base.OnDataAnalysisProblemChanged(e);
      BestKnownQualityParameter.Value = null;
      // paritions could be changed
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
    }

    protected override void OnSolutionParameterNameChanged(EventArgs e) {
      base.OnSolutionParameterNameChanged(e);
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
    }

    protected override void OnEvaluatorChanged(EventArgs e) {
      base.OnEvaluatorChanged(e);
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      ParameterizeProblem();
      RaiseEvaluatorChanged(e);
    }
    #endregion

    #region event handlers
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      OnEvaluatorChanged(e);
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (Operators == null || Operators.Count() == 0) InitializeOperators();
      #endregion
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    private void InitializeOperators() {
      AddOperator(new FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer());
      AddOperator(new SymbolicRegressionOverfittingAnalyzer());
      AddOperator(new TrainingBestScaledSymbolicRegressionSolutionAnalyzer());
      ParameterizeAnalyzers();
    }

    private void ParameterizeEvaluator() {
      Evaluator.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      Evaluator.RegressionProblemDataParameter.ActualName = DataAnalysisProblemDataParameter.Name;
      Evaluator.SamplesStartParameter.Value = TrainingSamplesStart;
      Evaluator.SamplesEndParameter.Value = TrainingSamplesEnd;
    }

    private void ParameterizeAnalyzers() {
      foreach (var analyzer in Analyzers) {
        analyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        var validationSolutionAnalyzer = analyzer as SymbolicRegressionValidationAnalyzer;
        if (validationSolutionAnalyzer != null) {
          validationSolutionAnalyzer.ProblemDataParameter.ActualName = DataAnalysisProblemDataParameter.Name;
          validationSolutionAnalyzer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
          validationSolutionAnalyzer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
          validationSolutionAnalyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
          validationSolutionAnalyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
          validationSolutionAnalyzer.ValidationSamplesStartParameter.Value = ValidationSamplesStart;
          validationSolutionAnalyzer.ValidationSamplesEndParameter.Value = ValidationSamplesEnd;
        }

        var fixedBestValidationSolutionAnalyzer = analyzer as FixedValidationBestScaledSymbolicRegressionSolutionAnalyzer;
        if (fixedBestValidationSolutionAnalyzer != null) {
          fixedBestValidationSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
        }

        var bestValidationSolutionAnalyzer = analyzer as ValidationBestScaledSymbolicRegressionSolutionAnalyzer;
        if (bestValidationSolutionAnalyzer != null) {
          bestValidationSolutionAnalyzer.ProblemDataParameter.ActualName = DataAnalysisProblemDataParameter.Name;
          bestValidationSolutionAnalyzer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
          bestValidationSolutionAnalyzer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
          bestValidationSolutionAnalyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
          bestValidationSolutionAnalyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
          bestValidationSolutionAnalyzer.ValidationSamplesStartParameter.Value = ValidationSamplesStart;
          bestValidationSolutionAnalyzer.ValidationSamplesEndParameter.Value = ValidationSamplesEnd;
          bestValidationSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
          bestValidationSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        }
      }
    }

    private void ParameterizeProblem() {
      if (MaximizationParameter.Value != null) {
        MaximizationParameter.Value.Value = Evaluator.Maximization;
      } else {
        MaximizationParameter.Value = new BoolValue(Evaluator.Maximization);
      }
    }
    #endregion
  }
}
