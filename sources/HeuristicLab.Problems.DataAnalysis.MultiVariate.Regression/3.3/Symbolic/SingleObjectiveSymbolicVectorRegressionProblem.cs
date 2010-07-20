#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Problems.DataAnalysis.Regression;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureManipulators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Manipulators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Crossovers;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Analyzers;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Interfaces;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Evaluators;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic.Analyzers;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.Regression.Symbolic {
  [Item("Symbolic Vector Regression Problem", "Represents a symbolic vector regression problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public class SingleObjectiveSymbolicVectorRegressionProblem : SymbolicVectorRegressionProblem, ISingleObjectiveProblem {

    #region Parameter Properties
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public new ValueParameter<ISingleObjectiveSymbolicVectorRegressionEvaluator> EvaluatorParameter {
      get { return (ValueParameter<ISingleObjectiveSymbolicVectorRegressionEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }

    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    #endregion

    #region Properties
    public new ISingleObjectiveSymbolicVectorRegressionEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
    }
    #endregion

    public SingleObjectiveSymbolicVectorRegressionProblem()
      : base() {
      var evaluator = new SymbolicVectorRegressionScaledNormalizedMseEvaluator();
      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as the error of the regression model should be minimized.", (BoolValue)new BoolValue(false).AsReadOnly()));
      Parameters.Add(new ValueParameter<ISingleObjectiveSymbolicVectorRegressionEvaluator>("Evaluator", "The operator which should be used to evaluate symbolic regression solutions.", evaluator));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The minimal error value that reached by symbolic regression solutions for the problem."));

      ParameterizeEvaluator();

      Initialize();
    }

    [StorableConstructor]
    private SingleObjectiveSymbolicVectorRegressionProblem(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SingleObjectiveSymbolicVectorRegressionProblem clone = (SingleObjectiveSymbolicVectorRegressionProblem)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    private void RegisterParameterValueEvents() {
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
    }

    #region event handling
    protected override void OnMultiVariateDataAnalysisProblemChanged(EventArgs e) {
      base.OnMultiVariateDataAnalysisProblemChanged(e);
      BestKnownQualityParameter.Value = null;
      // paritions could be changed
      ParameterizeEvaluator();
    }

    protected override void OnSolutionParameterNameChanged(EventArgs e) {
      ParameterizeEvaluator();
    }

    protected virtual void OnEvaluatorChanged(EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      RaiseEvaluatorChanged(e);
    }
    #endregion

    #region event handlers
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      OnEvaluatorChanged(e);
    }
    #endregion

    #region Helpers
    private void Initialize() {
      InitializeOperators();
      RegisterParameterValueEvents();
    }

    private void InitializeOperators() {
      AddOperator(new ValidationBestScaledSymbolicVectorRegressionSolutionAnalyzer());
      ParameterizeAnalyzers();
    }

    private void ParameterizeEvaluator() {
      Evaluator.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      Evaluator.MultiVariateDataAnalysisProblemDataParameter.ActualName = MultiVariateDataAnalysisProblemDataParameter.Name;
      Evaluator.SamplesStartParameter.Value = TrainingSamplesStart;
      Evaluator.SamplesEndParameter.Value = TrainingSamplesEnd;
    }

    private void ParameterizeAnalyzers() {
      foreach (var analyzer in Analyzers) {
        var bestValidationSolutionAnalyzer = analyzer as ValidationBestScaledSymbolicVectorRegressionSolutionAnalyzer;
        if (bestValidationSolutionAnalyzer != null) {
          bestValidationSolutionAnalyzer.ProblemDataParameter.ActualName = MultiVariateDataAnalysisProblemDataParameter.Name;
          bestValidationSolutionAnalyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
          bestValidationSolutionAnalyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
          bestValidationSolutionAnalyzer.ValidationSamplesStartParameter.Value = ValidationSamplesStart;
          bestValidationSolutionAnalyzer.ValidationSamplesEndParameter.Value = ValidationSamplesEnd;
          bestValidationSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
          bestValidationSolutionAnalyzer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
          bestValidationSolutionAnalyzer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
        }
      }
    }
    #endregion
  }
}
