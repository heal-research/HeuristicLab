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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Analyzers;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Creators;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Interfaces;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers;
using HeuristicLab.Problems.DataAnalysis.Symbolic;

namespace HeuristicLab.Problems.DataAnalysis.Regression.Symbolic {
  [StorableClass]
  public abstract class SymbolicRegressionProblemBase : DataAnalysisProblem, IHeuristicOptimizationProblem {

    #region Parameter Properties
    public new ValueParameter<SymbolicExpressionTreeCreator> SolutionCreatorParameter {
      get { return (ValueParameter<SymbolicExpressionTreeCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IHeuristicOptimizationProblem.SolutionCreatorParameter {
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
    public new SymbolicExpressionTreeCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IHeuristicOptimizationProblem.SolutionCreator {
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

    public ISymbolicExpressionGrammar FunctionTreeGrammar {
      get { return (ISymbolicExpressionGrammar)FunctionTreeGrammarParameter.Value; }
      private set { FunctionTreeGrammarParameter.Value = value; }
    }
    public override IEnumerable<IOperator> Operators {
      get { return operators; }
    }
    public IEnumerable<ISymbolicRegressionAnalyzer> Analyzers {
      get { return operators.OfType<ISymbolicRegressionAnalyzer>(); }
    }
    public DoubleValue PunishmentFactor {
      get { return new DoubleValue(10.0); }
    }
    public IntValue TrainingSamplesStart {
      get { return new IntValue(DataAnalysisProblemData.TrainingIndizes.First()); }
    }
    public IntValue TrainingSamplesEnd {
      get {
        int endIndex = (int)(DataAnalysisProblemData.TrainingIndizes.Count() * (1.0 - DataAnalysisProblemData.ValidationPercentage.Value) - 1);
        if (endIndex < 0) endIndex = 0;
        return new IntValue(DataAnalysisProblemData.TrainingIndizes.ElementAt(endIndex));
      }
    }
    public IntValue ValidationSamplesStart {
      get { return TrainingSamplesEnd; }
    }
    public IntValue ValidationSamplesEnd {
      get { return new IntValue(DataAnalysisProblemData.TrainingIndizes.Last() + 1); }
    }
    public IntValue TestSamplesStart {
      get { return DataAnalysisProblemData.TestSamplesStart; }
    }
    public IntValue TestSamplesEnd {
      get { return DataAnalysisProblemData.TestSamplesEnd; }
    }
    #endregion

    [Storable]
    private List<IOperator> operators;

    [StorableConstructor]
    protected SymbolicRegressionProblemBase(bool deserializing) : base(deserializing) { }
    protected SymbolicRegressionProblemBase(SymbolicRegressionProblemBase original, Cloner cloner)
      : base(original, cloner) {
      operators = original.operators.Select(x => (IOperator)cloner.Clone(x)).ToList();
      RegisterParameterValueEvents();
      RegisterParameterEvents();
    }
    public SymbolicRegressionProblemBase()
      : base() {
      SymbolicExpressionTreeCreator creator = new ProbabilisticTreeCreator();
      var grammar = new FullFunctionalExpressionGrammar();
      var globalGrammar = new GlobalSymbolicExpressionGrammar(grammar);
      var interpreter = new SimpleArithmeticExpressionInterpreter();
      Parameters.Add(new ValueParameter<SymbolicExpressionTreeCreator>("SolutionCreator", "The operator which should be used to create new symbolic regression solutions.", creator));
      Parameters.Add(new ValueParameter<ISymbolicExpressionTreeInterpreter>("SymbolicExpressionTreeInterpreter", "The interpreter that should be used to evaluate the symbolic expression tree.", interpreter));
      Parameters.Add(new ValueParameter<DoubleValue>("LowerEstimationLimit", "The lower limit for the estimated value that can be returned by the symbolic regression model.", new DoubleValue(double.NegativeInfinity)));
      Parameters.Add(new ValueParameter<DoubleValue>("UpperEstimationLimit", "The upper limit for the estimated value that can be returned by the symbolic regression model.", new DoubleValue(double.PositiveInfinity)));
      Parameters.Add(new ValueParameter<ISymbolicExpressionGrammar>("FunctionTreeGrammar", "The grammar that should be used for symbolic regression models.", globalGrammar));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionLength", "Maximal length of the symbolic expression.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionDepth", "Maximal depth of the symbolic expression. The minimum depth needed for the algorithm is 3 because two levels are reserved for the ProgramRoot and the Start symbol.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("MaxFunctionDefiningBranches", "Maximal number of automatically defined functions.", new IntValue(0)));
      Parameters.Add(new ValueParameter<IntValue>("MaxFunctionArguments", "Maximal number of arguments of automatically defined functions.", new IntValue(0)));

      ParameterizeSolutionCreator();

      UpdateGrammar();
      UpdateEstimationLimits();
      InitializeOperators();
      RegisterParameterValueEvents();
      RegisterParameterEvents();
    }

    private void RegisterParameterEvents() {
      MaxFunctionArgumentsParameter.ValueChanged += new EventHandler(ArchitectureParameter_ValueChanged);
      MaxFunctionDefiningBranchesParameter.ValueChanged += new EventHandler(ArchitectureParameter_ValueChanged);
      MaxExpressionDepthParameter.ValueChanged += new EventHandler(MaxExpressionDepthParameter_ValueChanged);
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      FunctionTreeGrammarParameter.ValueChanged += new EventHandler(FunctionTreeGrammarParameter_ValueChanged);
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
    }

    private void RegisterParameterValueEvents() {
      MaxFunctionArgumentsParameter.Value.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
      MaxFunctionDefiningBranchesParameter.Value.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
      MaxExpressionDepthParameter.Value.ValueChanged += new EventHandler(MaxExpressionDepthParameterValue_ValueChanged);
    }

    #region event handling
    protected override void OnDataAnalysisProblemChanged(EventArgs e) {
      base.OnDataAnalysisProblemChanged(e);
      // paritions could be changed
      ParameterizeAnalyzers();
      // input variables could have been changed
      UpdateGrammar();
      // estimation limits have to be recalculated
      UpdateEstimationLimits();
    }
    protected virtual void OnArchitectureParameterChanged(EventArgs e) {
      UpdateGrammar();
    }
    protected virtual void OnGrammarChanged() { UpdateGrammar(); }
    protected virtual void OnOperatorsChanged(EventArgs e) { RaiseOperatorsChanged(e); }
    protected virtual void OnSolutionCreatorChanged(EventArgs e) {
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      OnSolutionParameterNameChanged(e);
      RaiseSolutionCreatorChanged(e);
    }

    protected virtual void OnSolutionParameterNameChanged(EventArgs e) {
      ParameterizeAnalyzers();
      ParameterizeOperators();
    }

    protected virtual void OnEvaluatorChanged(EventArgs e) {
      RaiseEvaluatorChanged(e);
    }
    #endregion

    #region event handlers
    private void FunctionTreeGrammarParameter_ValueChanged(object sender, EventArgs e) {
      if (!(FunctionTreeGrammar is GlobalSymbolicExpressionGrammar))
        FunctionTreeGrammar = new GlobalSymbolicExpressionGrammar(FunctionTreeGrammar);
      OnGrammarChanged();
    }

    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      OnSolutionCreatorChanged(e);
    }
    private void SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged(object sender, EventArgs e) {
      OnSolutionParameterNameChanged(e);
    }
    private void ArchitectureParameter_ValueChanged(object sender, EventArgs e) {
      MaxFunctionArgumentsParameter.Value.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
      MaxFunctionDefiningBranchesParameter.Value.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
      OnArchitectureParameterChanged(e);
    }
    private void ArchitectureParameterValue_ValueChanged(object sender, EventArgs e) {
      OnArchitectureParameterChanged(e);
    }

    private void MaxExpressionDepthParameter_ValueChanged(object sender, EventArgs e) {
      MaxExpressionDepthParameterValue_ValueChanged(sender, e);
      MaxExpressionDepthParameter.Value.ValueChanged += MaxExpressionDepthParameterValue_ValueChanged;
    }
    private void MaxExpressionDepthParameterValue_ValueChanged(object sender, EventArgs e) {
      if (MaxExpressionDepth != null && MaxExpressionDepth.Value < 3)
        MaxExpressionDepth.Value = 3;
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (operators == null) InitializeOperators();
      #endregion
      RegisterParameterValueEvents();
      RegisterParameterEvents();
    }

    protected void AddOperator(IOperator op) {
      operators.Add(op);
    }

    private void UpdateGrammar() {
      foreach (var varSymbol in FunctionTreeGrammar.Symbols.OfType<HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable>()) {
        varSymbol.VariableNames = DataAnalysisProblemData.InputVariables.CheckedItems.Select(x => x.Value.Value);
      }
      foreach (var varSymbol in FunctionTreeGrammar.Symbols.OfType<HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.VariableCondition>()) {
        varSymbol.VariableNames = DataAnalysisProblemData.InputVariables.CheckedItems.Select(x => x.Value.Value);
      }
      var globalGrammar = FunctionTreeGrammar as GlobalSymbolicExpressionGrammar;
      if (globalGrammar != null) {
        globalGrammar.MaxFunctionArguments = MaxFunctionArguments.Value;
        globalGrammar.MaxFunctionDefinitions = MaxFunctionDefiningBranches.Value;
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
      operators = new List<IOperator>();
      operators.AddRange(ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeOperator>().OfType<IOperator>());
      operators.Add(new SymbolicRegressionVariableFrequencyAnalyzer());
      operators.Add(new SymbolicExpressionSymbolFrequencyAnalyzer());
      operators.Add(new MinAverageMaxSymbolicExpressionTreeSizeAnalyzer());
      operators.Add(new SymbolicRegressionModelQualityAnalyzer());
      ParameterizeOperators();
      ParameterizeAnalyzers();
    }

    private void ParameterizeSolutionCreator() {
      SolutionCreator.SymbolicExpressionGrammarParameter.ActualName = FunctionTreeGrammarParameter.Name;
      SolutionCreator.MaxTreeHeightParameter.ActualName = MaxExpressionDepthParameter.Name;
      SolutionCreator.MaxTreeSizeParameter.ActualName = MaxExpressionLengthParameter.Name;
      SolutionCreator.MaxFunctionArgumentsParameter.ActualName = MaxFunctionArgumentsParameter.Name;
      SolutionCreator.MaxFunctionDefinitionsParameter.ActualName = MaxFunctionDefiningBranchesParameter.Name;
    }

    private void ParameterizeAnalyzers() {
      foreach (var analyzer in Analyzers) {
        analyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        var symbolicRegressionModelQualityAnalyzer = analyzer as SymbolicRegressionModelQualityAnalyzer;
        if (symbolicRegressionModelQualityAnalyzer != null) {
          symbolicRegressionModelQualityAnalyzer.ProblemDataParameter.ActualName = DataAnalysisProblemDataParameter.Name;
          symbolicRegressionModelQualityAnalyzer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
          symbolicRegressionModelQualityAnalyzer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
          symbolicRegressionModelQualityAnalyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
          symbolicRegressionModelQualityAnalyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        }
        var varFreqAnalyzer = analyzer as SymbolicRegressionVariableFrequencyAnalyzer;
        if (varFreqAnalyzer != null) {
          varFreqAnalyzer.ProblemDataParameter.ActualName = DataAnalysisProblemDataParameter.Name;
        }
      }
      foreach (ISymbolicExpressionTreeAnalyzer analyzer in Operators.OfType<ISymbolicExpressionTreeAnalyzer>()) {
        analyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
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
