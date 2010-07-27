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
using HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic.Interfaces;

namespace HeuristicLab.Problems.DataAnalysis.MultiVariate.TimeSeriesPrognosis.Symbolic {
  [Item("Symbolic Time Series Prognosis Problem", "Represents a symbolic time series prognosis problem.")]
  [StorableClass]
  public class SymbolicTimeSeriesPrognosisProblem : MultiVariateDataAnalysisProblem, IProblem {

    #region Parameter Properties
    public ValueParameter<SymbolicExpressionTreeCreator> SolutionCreatorParameter {
      get { return (ValueParameter<SymbolicExpressionTreeCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public ValueParameter<ISymbolicTimeSeriesExpressionInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (ValueParameter<ISymbolicTimeSeriesExpressionInterpreter>)Parameters["SymbolicExpressionTreeInterpreter"]; }
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
    public ValueParameter<IntValue> PredictionHorizonParameter {
      get { return (ValueParameter<IntValue>)Parameters["PredictionHorizon"]; }
    }
    public ValueParameter<DoubleArray> UpperEstimationLimitParameter {
      get { return (ValueParameter<DoubleArray>)Parameters["UpperEstimationLimit"]; }
    }
    public ValueParameter<DoubleArray> LowerEstimationLimitParameter {
      get { return (ValueParameter<DoubleArray>)Parameters["LowerEstimationLimit"]; }
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
    public DoubleArray UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.Value; }
      set { UpperEstimationLimitParameter.Value = value; }
    }
    public DoubleArray LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.Value; }
      set { LowerEstimationLimitParameter.Value = value; }
    }
    public new SymbolicExpressionTreeCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public ISymbolicTimeSeriesExpressionInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.Value; }
      set { SymbolicExpressionTreeInterpreterParameter.Value = value; }
    }

    public ISymbolicExpressionGrammar FunctionTreeGrammar {
      get { return (ISymbolicExpressionGrammar)FunctionTreeGrammarParameter.Value; }
      set { FunctionTreeGrammarParameter.Value = value; }
    }
    public override IEnumerable<IOperator> Operators {
      get { return operators; }
    }
    public IEnumerable<IAnalyzer> Analyzers {
      get { return operators.OfType<IAnalyzer>(); }
    }
    public IntValue TrainingSamplesStart {
      get { return new IntValue(MultiVariateDataAnalysisProblemData.TrainingSamplesStart.Value); }
    }
    public IntValue TrainingSamplesEnd {
      get {
        return new IntValue((MultiVariateDataAnalysisProblemData.TrainingSamplesStart.Value +
          MultiVariateDataAnalysisProblemData.TrainingSamplesEnd.Value) / 2);
      }
    }
    public IntValue ValidationSamplesStart {
      get { return TrainingSamplesEnd; }
    }
    public IntValue ValidationSamplesEnd {
      get { return new IntValue(MultiVariateDataAnalysisProblemData.TrainingSamplesEnd.Value); }
    }
    public IntValue TestSamplesStart {
      get { return MultiVariateDataAnalysisProblemData.TestSamplesStart; }
    }
    public IntValue TestSamplesEnd {
      get { return MultiVariateDataAnalysisProblemData.TestSamplesEnd; }
    }
    public DoubleValue PunishmentFactor {
      get { return new DoubleValue(10.0); }
    }
    #endregion

    [Storable]
    private SymbolicTimeSeriesPrognosisGrammar grammar;
    [Storable]
    private List<IOperator> operators;

    [StorableConstructor]
    protected SymbolicTimeSeriesPrognosisProblem(bool deserializing) : base(deserializing) { }
    public SymbolicTimeSeriesPrognosisProblem()
      : base() {
      SymbolicExpressionTreeCreator creator = new ProbabilisticTreeCreator();
      grammar = new SymbolicTimeSeriesPrognosisGrammar(1);
      var globalGrammar = new GlobalSymbolicExpressionGrammar(grammar);
      var interpreter = new SymbolicTimeSeriesExpressionInterpreter();
      Parameters.Add(new ValueParameter<SymbolicExpressionTreeCreator>("SolutionCreator", "The operator which should be used to create new symbolic time series prognosis solutions.", creator));
      Parameters.Add(new ValueParameter<ISymbolicTimeSeriesExpressionInterpreter>("SymbolicExpressionTreeInterpreter", "The interpreter that should be used to evaluate the symbolic expression tree.", interpreter));
      Parameters.Add(new ValueParameter<ISymbolicExpressionGrammar>("FunctionTreeGrammar", "The grammar that should be used for symbolic time series prognosis models.", globalGrammar));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionLength", "Maximal length of the symbolic expression.", new IntValue(100)));
      Parameters.Add(new ValueParameter<IntValue>("MaxExpressionDepth", "Maximal depth of the symbolic expression.", new IntValue(10)));
      Parameters.Add(new ValueParameter<IntValue>("MaxFunctionDefiningBranches", "Maximal number of automatically defined functions.", (IntValue)new IntValue(0).AsReadOnly()));
      Parameters.Add(new ValueParameter<IntValue>("MaxFunctionArguments", "Maximal number of arguments of automatically defined functions.", (IntValue)new IntValue(0).AsReadOnly()));
      Parameters.Add(new ValueParameter<IntValue>("PredictionHorizon", "The number of time steps for which to create a forecast.", new IntValue(1)));
      Parameters.Add(new ValueParameter<DoubleArray>("UpperEstimationLimit", "The upper limit for the estimated values for each component."));
      Parameters.Add(new ValueParameter<DoubleArray>("LowerEstimationLimit", "The lower limit for the estimated values for each component."));
      creator.SymbolicExpressionTreeParameter.ActualName = "SymbolicTimeSeriesPrognosisModel";

      ParameterizeSolutionCreator();

      UpdateGrammar();
      InitializeOperators();
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (operators == null) InitializeOperators();
      #endregion
      RegisterParameterEvents();
      RegisterParameterValueEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SymbolicTimeSeriesPrognosisProblem clone = (SymbolicTimeSeriesPrognosisProblem)base.Clone(cloner);
      clone.operators = operators.Select(x => (IOperator)cloner.Clone(x)).ToList();
      clone.grammar = (SymbolicTimeSeriesPrognosisGrammar)cloner.Clone(grammar);
      clone.RegisterParameterEvents();
      clone.RegisterParameterValueEvents();
      return clone;
    }

    private void RegisterParameterValueEvents() {
      MaxFunctionArgumentsParameter.ValueChanged += new EventHandler(ArchitectureParameter_ValueChanged);
      MaxFunctionDefiningBranchesParameter.ValueChanged += new EventHandler(ArchitectureParameter_ValueChanged);
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
    }

    private void RegisterParameterEvents() {
      MaxFunctionArgumentsParameter.Value.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
      MaxFunctionDefiningBranchesParameter.Value.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
    }

    #region event handling
    protected override void OnMultiVariateDataAnalysisProblemChanged(EventArgs e) {
      base.OnMultiVariateDataAnalysisProblemChanged(e);
      // input variables could have been changed
      UpdateGrammar();
      UpdateEstimationLimits();
    }
    protected virtual void OnArchitectureParameterChanged(EventArgs e) {
      UpdateGrammar();
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
      ParameterizeOperators();
    }
    #endregion

    #region event handlers
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
    #endregion

    #region Helpers
    protected void AddOperator(IOperator op) {
      operators.Add(op);
    }

    private void UpdateGrammar() {
      var selectedTargetVariables = MultiVariateDataAnalysisProblemData.TargetVariables.CheckedItems;
      grammar.SetResultProducingBranches(selectedTargetVariables.Count());

      foreach (var varSymbol in grammar.Symbols.OfType<HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable>()) {
        varSymbol.VariableNames = MultiVariateDataAnalysisProblemData.InputVariables.CheckedItems.Select(x => x.Value.Value);
      }

      var globalGrammar = new GlobalSymbolicExpressionGrammar(grammar);
      globalGrammar.MaxFunctionArguments = MaxFunctionArguments.Value;
      globalGrammar.MaxFunctionDefinitions = MaxFunctionDefiningBranches.Value;
      FunctionTreeGrammar = globalGrammar;
    }
    private void UpdateEstimationLimits() {
      IEnumerable<string> selectedTargetVariables = MultiVariateDataAnalysisProblemData.TargetVariables.CheckedItems.Select(x => x.Value.Value);
      UpperEstimationLimit = new DoubleArray(selectedTargetVariables.Count());
      LowerEstimationLimit = new DoubleArray(selectedTargetVariables.Count());
      int i = 0;
      foreach (string targetVariable in selectedTargetVariables) {
        if (TrainingSamplesStart.Value < TrainingSamplesEnd.Value) {
          var targetValues = MultiVariateDataAnalysisProblemData.Dataset.GetVariableValues(targetVariable, TrainingSamplesStart.Value, TrainingSamplesEnd.Value);
          var mean = targetValues.Average();
          var range = targetValues.Max() - targetValues.Min();
          UpperEstimationLimit[i] = mean + PunishmentFactor.Value * range;
          LowerEstimationLimit[i] = mean - PunishmentFactor.Value * range;
        } else {
          UpperEstimationLimit[i] = 0;
          LowerEstimationLimit[i] = 0;
        }
        i++;
      }
    }

    private void InitializeOperators() {
      operators = new List<IOperator>();
      operators.AddRange(ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeOperator>().OfType<IOperator>());
      operators.Add(new MinAverageMaxSymbolicExpressionTreeSizeAnalyzer());
      ParameterizeOperators();
    }

    private void ParameterizeSolutionCreator() {
      SolutionCreator.SymbolicExpressionGrammarParameter.ActualName = FunctionTreeGrammarParameter.Name;
      SolutionCreator.MaxTreeHeightParameter.ActualName = MaxExpressionDepthParameter.Name;
      SolutionCreator.MaxTreeSizeParameter.ActualName = MaxExpressionLengthParameter.Name;
      SolutionCreator.MaxFunctionArgumentsParameter.ActualName = MaxFunctionArgumentsParameter.Name;
      SolutionCreator.MaxFunctionDefinitionsParameter.ActualName = MaxFunctionDefiningBranchesParameter.Name;
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
