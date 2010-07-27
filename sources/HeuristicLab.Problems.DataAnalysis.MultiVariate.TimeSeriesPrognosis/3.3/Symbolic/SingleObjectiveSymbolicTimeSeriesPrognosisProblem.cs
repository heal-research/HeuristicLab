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
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureManipulators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Manipulators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Crossovers;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Analyzers;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Evaluators;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Analyzers;
using HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Interfaces;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic {
  [Item("Symbolic Time Series Prognosis Problem", "Represents a symbolic time series prognosis problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public class SingleObjectiveSymbolicTimeSeriesPrognosisProblem : SymbolicTimeSeriesPrognosisProblem, ISingleObjectiveProblem {

    #region Parameter Properties
    public ValueParameter<ISingleObjectiveSymbolicTimeSeriesPrognosisEvaluator> EvaluatorParameter {
      get { return (ValueParameter<ISingleObjectiveSymbolicTimeSeriesPrognosisEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }

    #endregion

    #region Properties
    public ISingleObjectiveSymbolicTimeSeriesPrognosisEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return Evaluator; }
    }
    IEvaluator IProblem.Evaluator {
      get { return Evaluator; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
    }
    #endregion


    public SingleObjectiveSymbolicTimeSeriesPrognosisProblem()
      : base() {
      var evaluator = new SymbolicTimeSeriesPrognosisScaledNormalizedMseEvaluator();
      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as the error of the time series prognosis model should be minimized.", (BoolValue)new BoolValue(false).AsReadOnly()));
      Parameters.Add(new ValueParameter<ISingleObjectiveSymbolicTimeSeriesPrognosisEvaluator>("Evaluator", "The operator which should be used to evaluate symbolic time series prognosis solutions.", evaluator));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The minimal error value that reached by symbolic time series prognosis solutions for the problem."));
      evaluator.QualityParameter.ActualName = "TrainingMeanSquaredError";

      ParameterizeEvaluator();

      Initialize();
    }

    [StorableConstructor]
    protected SingleObjectiveSymbolicTimeSeriesPrognosisProblem(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SingleObjectiveSymbolicTimeSeriesPrognosisProblem clone = (SingleObjectiveSymbolicTimeSeriesPrognosisProblem)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    #region event handling
    protected override void OnMultiVariateDataAnalysisProblemChanged(EventArgs e) {
      base.OnMultiVariateDataAnalysisProblemChanged(e);
      BestKnownQualityParameter.Value = null;
      // paritions could be changed
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
    }

    protected override void OnSolutionParameterNameChanged(EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
    }

    protected virtual void OnEvaluatorChanged(EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      RaiseEvaluatorChanged(e);
    }
    #endregion

    #region Helpers
    private void Initialize() {
      InitializeOperators();
    }

    private void InitializeOperators() {
      AddOperator(new ValidationBestScaledSymbolicTimeSeriesPrognosisSolutionAnalyzer());
      ParameterizeAnalyzers();
    }

    private void ParameterizeEvaluator() {
      Evaluator.TimeSeriesPrognosisModelParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      Evaluator.TimeSeriesExpressionInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
      Evaluator.ProblemDataParameter.ActualName = MultiVariateDataAnalysisProblemDataParameter.Name;
      Evaluator.PredictionHorizonParameter.ActualName = PredictionHorizonParameter.Name;
      Evaluator.SamplesStartParameter.Value = TrainingSamplesStart;
      Evaluator.SamplesEndParameter.Value = TrainingSamplesEnd;
    }

    private void ParameterizeAnalyzers() {
      foreach (var analyzer in Analyzers) {
        var bestValidationSolutionAnalyzer = analyzer as ValidationBestScaledSymbolicTimeSeriesPrognosisSolutionAnalyzer;
        if (bestValidationSolutionAnalyzer != null) {
          bestValidationSolutionAnalyzer.ProblemDataParameter.ActualName = MultiVariateDataAnalysisProblemDataParameter.Name;
          bestValidationSolutionAnalyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
          bestValidationSolutionAnalyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
          bestValidationSolutionAnalyzer.ValidationSamplesStartParameter.Value = ValidationSamplesStart;
          bestValidationSolutionAnalyzer.ValidationSamplesEndParameter.Value = ValidationSamplesEnd;
          bestValidationSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
          bestValidationSolutionAnalyzer.PredictionHorizonParameter.ActualName = PredictionHorizonParameter.Name;
          bestValidationSolutionAnalyzer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
          bestValidationSolutionAnalyzer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
        }
      }
      foreach (ISymbolicExpressionTreeAnalyzer analyzer in Operators.OfType<ISymbolicExpressionTreeAnalyzer>()) {
        analyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
    }
    #endregion
  }
}
