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
using System.Drawing;
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

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("SymbolicRegressionProblem", "Represents a symbolic regression problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class SymbolicRegressionProblem : DataAnalysisProblem, ISingleObjectiveProblem {

    #region Parameter Properties
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public ValueParameter<SymbolicExpressionTreeCreator> SolutionCreatorParameter {
      get { return (ValueParameter<SymbolicExpressionTreeCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public ValueParameter<ISymbolicRegressionEvaluator> EvaluatorParameter {
      get { return (ValueParameter<ISymbolicRegressionEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public ValueParameter<ISymbolicExpressionGrammar> FunctionTreeGrammarParameter {
      get { return (ValueParameter<ISymbolicExpressionGrammar>)Parameters["FunctionTreeGrammar"]; }
    }
    public ValueParameter<IntValue> MaxExpressionLengthParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxExpressionLength"]; }
    }
    public ValueParameter<IntValue> MaxExpressionDepthParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxExpressionDepth"]; }
    }
    public ValueParameter<DoubleValue> NumberOfEvaluatedNodesParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["NumberOfEvaluatedNodes"]; }
    }
    public ValueParameter<IntValue> MaxFunctionDefiningBranchesParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxFunctionDefiningBranches"]; }
    }
    public ValueParameter<IntValue> MaxFunctionArgumentsParameter {
      get { return (ValueParameter<IntValue>)Parameters["MaxFunctionArguments"]; }
    }
    public OptionalValueParameter<ISingleObjectiveSolutionsVisualizer> VisualizerParameter {
      get { return (OptionalValueParameter<ISingleObjectiveSolutionsVisualizer>)Parameters["Visualizer"]; }
    }
    IParameter IProblem.VisualizerParameter {
      get { return VisualizerParameter; }
    }
    public ValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    #endregion

    #region Properties
    public IntValue MaxExpressionLength {
      get { return MaxExpressionLengthParameter.Value; }
      set { MaxExpressionLengthParameter.Value = value; }
    }
    public IntValue MaxExpressionDepth {
      get { return MaxExpressionDepthParameter.Value; }
      set { MaxExpressionDepthParameter.Value = value; }
    }
    public SymbolicExpressionTreeCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public ISymbolicRegressionEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public ArithmeticExpressionGrammar FunctionTreeGrammar {
      get { return (ArithmeticExpressionGrammar)FunctionTreeGrammarParameter.Value; }
    }
    public ISingleObjectiveSolutionsVisualizer Visualizer {
      get { return VisualizerParameter.Value; }
      set { VisualizerParameter.Value = value; }
    }
    ISolutionsVisualizer IProblem.Visualizer {
      get { return VisualizerParameter.Value; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
    }
    private List<ISymbolicExpressionTreeOperator> operators;
    public IEnumerable<IOperator> Operators {
      get { return operators.Cast<IOperator>(); }
    }
    #endregion

    public SymbolicRegressionProblem()
      : base() {
      SymbolicExpressionTreeCreator creator = new ProbabilisticTreeCreator();
      var evaluator = new SymbolicRegressionMeanSquaredErrorEvaluator();
      var grammar = new ArithmeticExpressionGrammar();
      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as the error of the regression model should be minimized.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<SymbolicExpressionTreeCreator>("SolutionCreator", "The operator which should be used to create new symbolic regression solutions.", creator));
      Parameters.Add(new ValueParameter<ISymbolicRegressionEvaluator>("Evaluator", "The operator which should be used to evaluate symbolic regression solutions.", evaluator));
      Parameters.Add(new ValueParameter<DoubleValue>("BestKnownQuality", "The minimal error value that can be reached by symbolic regression models.", new DoubleValue(0)));
      Parameters.Add(new ValueParameter<ISymbolicExpressionGrammar>("FunctionTreeGrammar", "The grammar that should be used for symbolic regression models.", grammar));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionLength", "Maximal length of the symbolic expression.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionDepth", "Maximal depth of the symbolic expression.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("MaxFunctionDefiningBranches", "Maximal number of automatically defined functions.", new IntValue(3)));
      Parameters.Add(new ValueParameter<IntValue>("MaxFunctionArguments", "Maximal number of arguments of automatically defined functions.", new IntValue(3)));
      Parameters.Add(new ValueParameter<DoubleValue>("NumberOfEvaluatedNodes", "The total number of evaluated function tree nodes (for performance measurements.)", new DoubleValue()));
      Parameters.Add(new ValueParameter<ISingleObjectiveSolutionsVisualizer>("Visualizer", "The operator which should be used to visualize artificial ant solutions.", null));

      creator.SymbolicExpressionTreeParameter.ActualName = "SymbolicRegressionModel";
      evaluator.QualityParameter.ActualName = "TrainingMeanSquaredError";
      DataAnalysisProblemDataParameter.ValueChanged += new EventHandler(DataAnalysisProblemDataParameter_ValueChanged);
      DataAnalysisProblemData.InputVariablesChanged += new EventHandler(DataAnalysisProblemData_InputVariablesChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeVisualizer();

      Initialize();
    }

    void DataAnalysisProblemDataParameter_ValueChanged(object sender, EventArgs e) {
      DataAnalysisProblemData.InputVariablesChanged += new EventHandler(DataAnalysisProblemData_InputVariablesChanged);
    }

    void DataAnalysisProblemData_InputVariablesChanged(object sender, EventArgs e) {
      FunctionTreeGrammar.VariableNames = DataAnalysisProblemData.InputVariables.Select(x => x.Value);
    }

    [StorableConstructor]
    private SymbolicRegressionProblem(bool deserializing) : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      SymbolicRegressionProblem clone = (SymbolicRegressionProblem)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    #region Events
    public event EventHandler SolutionCreatorChanged;
    private void OnSolutionCreatorChanged() {
      var changed = SolutionCreatorChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler EvaluatorChanged;
    private void OnEvaluatorChanged() {
      var changed = EvaluatorChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }
    public event EventHandler VisualizerChanged;
    private void OnVisualizerChanged() {
      var changed = VisualizerChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    public event EventHandler OperatorsChanged;
    private void OnOperatorsChanged() {
      var changed = OperatorsChanged;
      if (changed != null)
        changed(this, EventArgs.Empty);
    }

    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeVisualizer();
      ParameterizeOperators();
      OnSolutionCreatorChanged();
    }
    private void SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeVisualizer();
      ParameterizeOperators();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      ParameterizeEvaluator();
      ParameterizeVisualizer();
      OnEvaluatorChanged();
    }

    private void VisualizerParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeVisualizer();
      OnVisualizerChanged();
    }

    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeVisualizer();
    }

    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      InitializeOperators();
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      VisualizerParameter.ValueChanged += new EventHandler(VisualizerParameter_ValueChanged);
    }

    private void InitializeOperators() {
      operators = new List<ISymbolicExpressionTreeOperator>();
      operators.AddRange(ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeOperator>());
      ParameterizeOperators();
    }

    private void ParameterizeSolutionCreator() {
      SolutionCreator.SymbolicExpressionGrammarParameter.ActualName = FunctionTreeGrammarParameter.Name;
      SolutionCreator.MaxTreeHeightParameter.ActualName = MaxExpressionDepthParameter.Name;
      SolutionCreator.MaxTreeSizeParameter.ActualName = MaxExpressionLengthParameter.Name;
    }
    private void ParameterizeEvaluator() {
      Evaluator.FunctionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      Evaluator.RegressionProblemDataParameter.ActualName = DataAnalysisProblemDataParameter.Name;
    }
    private void ParameterizeVisualizer() {
      if (Visualizer != null) {
        //Visualizer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        //var antTrailVisualizer = Visualizer as IAntTrailVisualizer;
        //if (antTrailVisualizer != null) {
        //  antTrailVisualizer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        //  antTrailVisualizer.WorldParameter.ActualName = WorldParameter.Name;
        //  antTrailVisualizer.MaxTimeStepsParameter.ActualName = MaxTimeStepsParameter.Name;
        //}
        //var bestSymExpressionVisualizer = Visualizer as BestSymbolicExpressionTreeVisualizer;
        //if (bestSymExpressionVisualizer != null) {
        //  bestSymExpressionVisualizer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        //}
      }
    }

    private void ParameterizeOperators() {
      foreach (ISymbolicExpressionTreeOperator op in Operators.OfType<ISymbolicExpressionTreeOperator>()) {
        op.MaxTreeHeightParameter.ActualName = MaxExpressionDepthParameter.Name;
        op.MaxTreeSizeParameter.ActualName = MaxExpressionLengthParameter.Name;
        op.SymbolicExpressionGrammarParameter.ActualName = FunctionTreeGrammarParameter.Name;
      }
      foreach (ISymbolicRegressionEvaluator op in Operators.OfType<ISymbolicRegressionEvaluator>()) {
        op.FunctionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.RegressionProblemDataParameter.ActualName = DataAnalysisProblemDataParameter.Name;
        op.NumberOfEvaluatedNodesParameter.ActualName = NumberOfEvaluatedNodesParameter.Name;
      }
      foreach (SymbolicExpressionTreeCrossover op in Operators.OfType<SymbolicExpressionTreeCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (SymbolicExpressionTreeManipulator op in Operators.OfType<SymbolicExpressionTreeManipulator>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (SymbolicExpressionTreeArchitectureAlteringOperator op in Operators.OfType<SymbolicExpressionTreeArchitectureAlteringOperator>()) {
      }
    }
    #endregion
  }
}
