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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.ArchitectureManipulators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Manipulators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Crossovers;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [Item("Symbolic Regression Problem", "Represents a symbolic regression problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public class SymbolicRegressionProblem : DataAnalysisProblem, ISingleObjectiveProblem {

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
    public ValueParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["LowerEstimationLimit"]; }
    }
    public ValueParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["UpperEstimationLimit"]; }
    }
    public ValueParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (ValueParameter<ISymbolicExpressionTreeInterpreter>)Parameters["SymbolicExpressionTreeInterpreter"]; }
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
    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
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
    public IntValue MaxFunctionDefiningBranches {
      get { return MaxFunctionDefiningBranchesParameter.Value; }
      set { MaxFunctionDefiningBranchesParameter.Value = value; }
    }
    public IntValue MaxFunctionArguments {
      get { return MaxFunctionArgumentsParameter.Value; }
      set { MaxFunctionArgumentsParameter.Value = value; }
    }
    public SymbolicExpressionTreeCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.Value; }
      set { SymbolicExpressionTreeInterpreterParameter.Value = value; }
    }
    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.Value; }
      set { LowerEstimationLimitParameter.Value = value; }
    }
    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.Value; }
      set { UpperEstimationLimitParameter.Value = value; }
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
    public ISymbolicExpressionGrammar FunctionTreeGrammar {
      get { return (ISymbolicExpressionGrammar)FunctionTreeGrammarParameter.Value; }
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
    public DoubleValue PunishmentFactor {
      get { return new DoubleValue(10.0); }
    }
    public IntValue TrainingSamplesStart {
      get { return new IntValue(DataAnalysisProblemData.TrainingSamplesStart.Value); }
    }
    public IntValue TrainingSamplesEnd {
      get {
        return new IntValue((DataAnalysisProblemData.TrainingSamplesStart.Value +
          DataAnalysisProblemData.TrainingSamplesEnd.Value) / 2);
      }
    }
    public IntValue ValidationSamplesStart {
      get { return TrainingSamplesEnd; }
    }
    public IntValue ValidationSamplesEnd {
      get { return new IntValue(DataAnalysisProblemData.TrainingSamplesEnd.Value); }
    }
    #endregion

    public SymbolicRegressionProblem()
      : base() {
      SymbolicExpressionTreeCreator creator = new ProbabilisticTreeCreator();
      var evaluator = new SymbolicRegressionScaledMeanSquaredErrorEvaluator();
      var grammar = new ArithmeticExpressionGrammar();
      var globalGrammar = new GlobalSymbolicExpressionGrammar(grammar);
      var visualizer = new BestValidationSymbolicRegressionSolutionVisualizer();
      var interpreter = new SimpleArithmeticExpressionInterpreter();
      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as the error of the regression model should be minimized.", (BoolValue)new BoolValue(false).AsReadOnly()));
      Parameters.Add(new ValueParameter<SymbolicExpressionTreeCreator>("SolutionCreator", "The operator which should be used to create new symbolic regression solutions.", creator));
      Parameters.Add(new ValueParameter<ISymbolicExpressionTreeInterpreter>("SymbolicExpressionTreeInterpreter", "The interpreter that should be used to evaluate the symbolic expression tree.", interpreter));
      Parameters.Add(new ValueParameter<ISymbolicRegressionEvaluator>("Evaluator", "The operator which should be used to evaluate symbolic regression solutions.", evaluator));
      Parameters.Add(new ValueParameter<DoubleValue>("LowerEstimationLimit", "The lower limit for the estimated value that can be returned by the symbolic regression model.", new DoubleValue(double.NegativeInfinity)));
      Parameters.Add(new ValueParameter<DoubleValue>("UpperEstimationLimit", "The upper limit for the estimated value that can be returned by the symbolic regression model.", new DoubleValue(double.PositiveInfinity)));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The minimal error value that reached by symbolic regression solutions for the problem."));
      Parameters.Add(new ValueParameter<ISymbolicExpressionGrammar>("FunctionTreeGrammar", "The grammar that should be used for symbolic regression models.", globalGrammar));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionLength", "Maximal length of the symbolic expression.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionDepth", "Maximal depth of the symbolic expression.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("MaxFunctionDefiningBranches", "Maximal number of automatically defined functions.", (IntValue)new IntValue(0).AsReadOnly()));
      Parameters.Add(new ValueParameter<IntValue>("MaxFunctionArguments", "Maximal number of arguments of automatically defined functions.", (IntValue)new IntValue(0).AsReadOnly()));
      Parameters.Add(new ValueParameter<ISingleObjectiveSolutionsVisualizer>("Visualizer", "The operator which should be used to visualize symbolic regression solutions.", visualizer));

      creator.SymbolicExpressionTreeParameter.ActualName = "SymbolicRegressionModel";
      evaluator.QualityParameter.ActualName = "TrainingMeanSquaredError";

      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeVisualizer();

      UpdateGrammar();
      UpdateEstimationLimits();
      Initialize();
    }

    [StorableConstructor]
    private SymbolicRegressionProblem(bool deserializing) : base() { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SymbolicRegressionProblem clone = (SymbolicRegressionProblem)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    private void RegisterParameterValueEvents() {
      MaxFunctionArgumentsParameter.ValueChanged += new EventHandler(ArchitectureParameter_ValueChanged);
      MaxFunctionDefiningBranchesParameter.ValueChanged += new EventHandler(ArchitectureParameter_ValueChanged);
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      VisualizerParameter.ValueChanged += new EventHandler(VisualizerParameter_ValueChanged);
    }

    private void RegisterParameterEvents() {
      MaxFunctionArgumentsParameter.Value.ValueChanged += new EventHandler(ArchitectureParameter_ValueChanged);
      MaxFunctionDefiningBranchesParameter.Value.ValueChanged += new EventHandler(ArchitectureParameter_ValueChanged);
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
    }

    #region event handling
    protected override void OnDataAnalysisProblemChanged(EventArgs e) {
      base.OnDataAnalysisProblemChanged(e);
      // paritions could be changed
      ParameterizeEvaluator();
      ParameterizeVisualizer();
      // input variables could have been changed
      UpdateGrammar();
      // estimation limits have to be recalculated
      UpdateEstimationLimits();
    }
    protected virtual void OnArchitectureParameterChanged(EventArgs e) {
      var globalGrammar = FunctionTreeGrammar as GlobalSymbolicExpressionGrammar;
      if (globalGrammar != null) {
        globalGrammar.MaxFunctionArguments = MaxFunctionArguments.Value;
        globalGrammar.MaxFunctionDefinitions = MaxFunctionDefiningBranches.Value;
      }
    }
    protected virtual void OnGrammarChanged(EventArgs e) { }
    protected virtual void OnOperatorsChanged(EventArgs e) { RaiseOperatorsChanged(e); }
    protected virtual void OnSolutionCreatorChanged(EventArgs e) {
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      OnSolutionParameterNameChanged(e);
      RaiseSolutionCreatorChanged(e);
    }

    protected virtual void OnSolutionParameterNameChanged(EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeVisualizer();
      ParameterizeOperators();
    }

    protected virtual void OnEvaluatorChanged(EventArgs e) {
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      ParameterizeEvaluator();
      ParameterizeVisualizer();
      RaiseEvaluatorChanged(e);
    }
    protected virtual void OnQualityParameterNameChanged(EventArgs e) {
      ParameterizeVisualizer();
    }
    protected virtual void OnVisualizerChanged(EventArgs e) {
      ParameterizeVisualizer();
      RaiseVisualizerChanged(e);
    }
    #endregion

    #region event handlers
    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      OnSolutionCreatorChanged(e);
    }
    private void SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged(object sender, EventArgs e) {
      OnSolutionParameterNameChanged(e);
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      OnEvaluatorChanged(e);
    }
    private void VisualizerParameter_ValueChanged(object sender, EventArgs e) {
      OnVisualizerChanged(e);
    }
    private void ArchitectureParameter_ValueChanged(object sender, EventArgs e) {
      OnArchitectureParameterChanged(e);
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      OnQualityParameterNameChanged(e);
    }
    #endregion

    #region events
    public event EventHandler SolutionCreatorChanged;
    private void RaiseSolutionCreatorChanged(EventArgs e) {
      var changed = SolutionCreatorChanged;
      if (changed != null)
        changed(this, e);
    }
    public event EventHandler EvaluatorChanged;
    private void RaiseEvaluatorChanged(EventArgs e) {
      var changed = EvaluatorChanged;
      if (changed != null)
        changed(this, e);
    }
    public event EventHandler VisualizerChanged;
    private void RaiseVisualizerChanged(EventArgs e) {
      var changed = VisualizerChanged;
      if (changed != null)
        changed(this, e);
    }

    public event EventHandler OperatorsChanged;
    private void RaiseOperatorsChanged(EventArgs e) {
      var changed = OperatorsChanged;
      if (changed != null)
        changed(this, e);
    }
    #endregion

    #region Helpers
    private void Initialize() {
      InitializeOperators();
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    private void UpdateGrammar() {
      foreach (var varSymbol in FunctionTreeGrammar.Symbols.OfType<HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable>()) {
        varSymbol.VariableNames = DataAnalysisProblemData.InputVariables.Select(x => x.Value);
      }
    }

    private void UpdateEstimationLimits() {
      if (TrainingSamplesStart.Value < TrainingSamplesEnd.Value &&
        DataAnalysisProblemData.Dataset.VariableNames.Contains(DataAnalysisProblemData.TargetVariable.Value)) {
        var targetValues = DataAnalysisProblemData.Dataset.GetVariableValues(DataAnalysisProblemData.TargetVariable.Value, TrainingSamplesStart.Value, TrainingSamplesEnd.Value);
        var mean = targetValues.Average();
        var range = targetValues.Max() - targetValues.Min();
        UpperEstimationLimit = new DoubleValue(mean + PunishmentFactor.Value * range);
        LowerEstimationLimit = new DoubleValue(mean - PunishmentFactor.Value * range);
      }
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
      SolutionCreator.MaxFunctionArgumentsParameter.ActualName = MaxFunctionArgumentsParameter.Name;
      SolutionCreator.MaxFunctionDefinitionsParameter.ActualName = MaxFunctionDefiningBranchesParameter.Name;
    }

    private void ParameterizeEvaluator() {
      Evaluator.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      Evaluator.RegressionProblemDataParameter.ActualName = DataAnalysisProblemDataParameter.Name;
      Evaluator.SamplesStartParameter.Value = TrainingSamplesStart;
      Evaluator.SamplesEndParameter.Value = TrainingSamplesEnd;
    }

    private void ParameterizeVisualizer() {
      var solutionVisualizer = Visualizer as BestValidationSymbolicRegressionSolutionVisualizer;
      if (solutionVisualizer != null) {
        solutionVisualizer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        solutionVisualizer.DataAnalysisProblemDataParameter.ActualName = DataAnalysisProblemDataParameter.Name;
        solutionVisualizer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
        solutionVisualizer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
        solutionVisualizer.QualityParameter.ActualName = Evaluator.QualityParameter.Name;
        solutionVisualizer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
        solutionVisualizer.ValidationSamplesStartParameter.Value = ValidationSamplesStart;
        solutionVisualizer.ValidationSamplesEndParameter.Value = ValidationSamplesEnd;
      }
    }

    private void ParameterizeOperators() {
      foreach (ISymbolicExpressionTreeOperator op in Operators.OfType<ISymbolicExpressionTreeOperator>()) {
        op.MaxTreeHeightParameter.ActualName = MaxExpressionDepthParameter.Name;
        op.MaxTreeSizeParameter.ActualName = MaxExpressionLengthParameter.Name;
        op.SymbolicExpressionGrammarParameter.ActualName = FunctionTreeGrammarParameter.Name;
      }
      foreach (ISymbolicExpressionTreeCrossover op in Operators.OfType<ISymbolicExpressionTreeCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (ISymbolicExpressionTreeManipulator op in Operators.OfType<ISymbolicExpressionTreeManipulator>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (ISymbolicExpressionTreeArchitectureManipulator op in Operators.OfType<ISymbolicExpressionTreeArchitectureManipulator>()) {
        op.MaxFunctionArgumentsParameter.ActualName = MaxFunctionArgumentsParameter.Name;
        op.MaxFunctionDefinitionsParameter.ActualName = MaxFunctionDefiningBranchesParameter.Name;
      }
    }
    #endregion
  }
}
