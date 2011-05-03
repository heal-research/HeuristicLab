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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Evaluators;
using HeuristicLab.Problems.DataAnalysis.Regression.LinearRegression;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Algorithms.DataAnalysis {
  /// <summary>
  /// Linear regression data analysis algorithm.
  /// </summary>
  [NonDiscoverableType]
  [Item("Linear Regression", "Linear regression data analysis algorithm.")]
  [StorableClass]
  public sealed class LinearRegression : EngineAlgorithm, IStorableContent {
    private const string TrainingSamplesStartParameterName = "Training start";
    private const string TrainingSamplesEndParameterName = "Training end";
    private const string LinearRegressionModelParameterName = "LinearRegressionModel";
    private const string ModelInterpreterParameterName = "Model interpreter";

    public string Filename { get; set; }

    #region Problem Properties
    public override Type ProblemType {
      get { return typeof(DataAnalysisProblem); }
    }
    public new DataAnalysisProblem Problem {
      get { return (DataAnalysisProblem)base.Problem; }
      set { base.Problem = value; }
    }
    #endregion

    #region parameter properties
    public IValueParameter<IntValue> TrainingSamplesStartParameter {
      get { return (IValueParameter<IntValue>)Parameters[TrainingSamplesStartParameterName]; }
    }
    public IValueParameter<IntValue> TrainingSamplesEndParameter {
      get { return (IValueParameter<IntValue>)Parameters[TrainingSamplesEndParameterName]; }
    }
    public IValueParameter<ISymbolicExpressionTreeInterpreter> ModelInterpreterParameter {
      get { return (IValueParameter<ISymbolicExpressionTreeInterpreter>)Parameters[ModelInterpreterParameterName]; }
    }
    #endregion

    [Storable]
    private LinearRegressionSolutionCreator solutionCreator;
    [Storable]
    private SimpleSymbolicRegressionEvaluator evaluator;
    [Storable]
    private SimpleMSEEvaluator mseEvaluator;
    [Storable]
    private BestSymbolicRegressionSolutionAnalyzer analyzer;
    public LinearRegression()
      : base() {
      Parameters.Add(new ValueParameter<IntValue>(TrainingSamplesStartParameterName, "The first index of the data set partition to use for training."));
      Parameters.Add(new ValueParameter<IntValue>(TrainingSamplesEndParameterName, "The last index of the data set partition to use for training."));
      Parameters.Add(new ValueParameter<ISymbolicExpressionTreeInterpreter>(ModelInterpreterParameterName, "The interpreter to use for evaluation of the model.", new SimpleArithmeticExpressionInterpreter()));

      solutionCreator = new LinearRegressionSolutionCreator();
      evaluator = new SimpleSymbolicRegressionEvaluator();
      mseEvaluator = new SimpleMSEEvaluator();
      analyzer = new BestSymbolicRegressionSolutionAnalyzer();

      OperatorGraph.InitialOperator = solutionCreator;
      solutionCreator.Successor = evaluator;
      evaluator.Successor = mseEvaluator;
      mseEvaluator.Successor = analyzer;

      Initialize();
    }
    [StorableConstructor]
    private LinearRegression(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private LinearRegression(LinearRegression original, Cloner cloner)
      : base(original, cloner) {
      solutionCreator = cloner.Clone(original.solutionCreator);
      evaluator = cloner.Clone(original.evaluator);
      mseEvaluator = cloner.Clone(original.mseEvaluator);
      analyzer = cloner.Clone(original.analyzer);
      Initialize();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new LinearRegression(this, cloner);
    }

    public override void Prepare() {
      if (Problem != null) base.Prepare();
    }

    protected override void Problem_Reset(object sender, EventArgs e) {
      UpdateAlgorithmParameterValues();
      base.Problem_Reset(sender, e);
    }

    #region Events
    protected override void OnProblemChanged() {
      solutionCreator.DataAnalysisProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
      evaluator.RegressionProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
      analyzer.ProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
      UpdateAlgorithmParameterValues();
      Problem.Reset += new EventHandler(Problem_Reset);
      base.OnProblemChanged();
    }


    #endregion

    #region Helpers
    private void Initialize() {
      solutionCreator.SamplesStartParameter.ActualName = TrainingSamplesStartParameter.Name;
      solutionCreator.SamplesEndParameter.ActualName = TrainingSamplesEndParameter.Name;
      solutionCreator.SymbolicExpressionTreeParameter.ActualName = LinearRegressionModelParameterName;

      evaluator.SymbolicExpressionTreeParameter.ActualName = solutionCreator.SymbolicExpressionTreeParameter.ActualName;
      evaluator.SymbolicExpressionTreeInterpreterParameter.ActualName = ModelInterpreterParameter.Name;
      evaluator.ValuesParameter.ActualName = "Training values";
      evaluator.SamplesStartParameter.ActualName = TrainingSamplesStartParameterName;
      evaluator.SamplesEndParameter.ActualName = TrainingSamplesEndParameterName;

      mseEvaluator.ValuesParameter.ActualName = "Training values";
      mseEvaluator.MeanSquaredErrorParameter.ActualName = "Training MSE";

      analyzer.SymbolicExpressionTreeParameter.ActualName = solutionCreator.SymbolicExpressionTreeParameter.ActualName;
      analyzer.SymbolicExpressionTreeParameter.Depth = 0;
      analyzer.QualityParameter.ActualName = mseEvaluator.MeanSquaredErrorParameter.ActualName;
      analyzer.QualityParameter.Depth = 0;
      analyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = ModelInterpreterParameter.Name;

      if (Problem != null) {
        solutionCreator.DataAnalysisProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
        evaluator.RegressionProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
        analyzer.ProblemDataParameter.ActualName = Problem.DataAnalysisProblemDataParameter.Name;
        Problem.Reset += new EventHandler(Problem_Reset);
      }
    }

    private void UpdateAlgorithmParameterValues() {
      TrainingSamplesStartParameter.ActualValue = Problem.DataAnalysisProblemData.TrainingSamplesStart;
      TrainingSamplesEndParameter.ActualValue = Problem.DataAnalysisProblemData.TrainingSamplesEnd;
      //var targetValues =
      //  Problem.DataAnalysisProblemData.Dataset.GetVariableValues(Problem.DataAnalysisProblemData.TargetVariable.Value,
      //  TrainingSamplesStartParameter.Value.Value, TrainingSamplesEndParameter.Value.Value);
      //double range = targetValues.Max() - targetValues.Min();
      //double lowerEstimationLimit = targetValues.Average() - 10.0 * range;
      //double upperEstimationLimit = targetValues.Average() + 10.0 * range;
      //evaluator.LowerEstimationLimitParameter.Value = new DoubleValue(lowerEstimationLimit);
      //evaluator.UpperEstimationLimitParameter.Value = new DoubleValue(upperEstimationLimit);
      //analyzer.LowerEstimationLimitParameter.Value = new DoubleValue(lowerEstimationLimit);
      //analyzer.UpperEstimationLimitParameter.Value = new DoubleValue(upperEstimationLimit);
    }
    #endregion
  }
}
