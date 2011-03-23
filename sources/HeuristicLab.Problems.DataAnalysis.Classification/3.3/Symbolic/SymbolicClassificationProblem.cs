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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Symbols;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis.Classification.Symbolic.Analyzers;
using HeuristicLab.Problems.DataAnalysis.Regression.Symbolic.Analyzers;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols;

namespace HeuristicLab.Problems.DataAnalysis.Classification {
  [Item("Classification Problem", "Represents a classfication problem.")]
  [StorableClass]
  [NonDiscoverableType]
  public sealed class SymbolicClassificationProblem : SingleObjectiveClassificationProblem<ISymbolicClassificationEvaluator, ISymbolicExpressionTreeCreator>, IStorableContent {
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string FunctionTreeGrammarParameterName = "FunctionTreeGrammar";
    private const string MaxExpressionLengthParameterName = "MaxExpressionLength";
    private const string MaxExpressionDepthParameterName = "MaxExpressionDepth";
    private const string UpperEstimationLimitParameterName = "UpperEstimationLimit";
    private const string LowerEstimationLimitParameterName = "LowerEstimationLimit";
    private const string MaxFunctionDefiningBranchensParameterName = "MaxFunctionDefiningBranches";
    private const string MaxFunctionArgumentsParameterName = "MaxFunctionArguments";

    #region properties
    public string Filename { get; set; }

    public ISymbolicExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.Value; }
      private set { SymbolicExpressionTreeInterpreterParameter.Value = value; }
    }
    public IValueParameter<ISymbolicExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueParameter<ISymbolicExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }

    public ISymbolicExpressionGrammar FunctionTreeGrammar {
      get { return (ISymbolicExpressionGrammar)FunctionTreeGrammarParameter.Value; }
      private set { FunctionTreeGrammarParameter.Value = value; }
    }
    public IValueParameter<ISymbolicExpressionGrammar> FunctionTreeGrammarParameter {
      get { return (IValueParameter<ISymbolicExpressionGrammar>)Parameters[FunctionTreeGrammarParameterName]; }
    }

    public IntValue MaxExpressionLength {
      get { return MaxExpressionLengthParameter.Value; }
      private set { MaxExpressionLengthParameter.Value = value; }
    }
    public IValueParameter<IntValue> MaxExpressionLengthParameter {
      get { return (IValueParameter<IntValue>)Parameters[MaxExpressionLengthParameterName]; }
    }

    public IntValue MaxExpressionDepth {
      get { return MaxExpressionDepthParameter.Value; }
      private set { MaxExpressionDepthParameter.Value = value; }
    }
    public ValueParameter<IntValue> MaxExpressionDepthParameter {
      get { return (ValueParameter<IntValue>)Parameters[MaxExpressionDepthParameterName]; }
    }

    public DoubleValue UpperEstimationLimit {
      get { return UpperEstimationLimitParameter.Value; }
      private set { UpperEstimationLimitParameter.Value = value; }
    }
    public IValueParameter<DoubleValue> UpperEstimationLimitParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[UpperEstimationLimitParameterName]; }
    }

    public DoubleValue LowerEstimationLimit {
      get { return LowerEstimationLimitParameter.Value; }
      private set { LowerEstimationLimitParameter.Value = value; }
    }
    public IValueParameter<DoubleValue> LowerEstimationLimitParameter {
      get { return (IValueParameter<DoubleValue>)Parameters[LowerEstimationLimitParameterName]; }
    }

    public IntValue MaxFunctionDefiningBranches {
      get { return MaxFunctionDefiningBranchesParameter.Value; }
      private set { MaxFunctionDefiningBranchesParameter.Value = value; }
    }
    public IValueParameter<IntValue> MaxFunctionDefiningBranchesParameter {
      get { return (IValueParameter<IntValue>)Parameters[MaxFunctionDefiningBranchensParameterName]; }
    }

    public IntValue MaxFunctionArguments {
      get { return MaxFunctionArgumentsParameter.Value; }
      private set { MaxFunctionArgumentsParameter.Value = value; }
    }
    public IValueParameter<IntValue> MaxFunctionArgumentsParameter {
      get { return (IValueParameter<IntValue>)Parameters[MaxFunctionArgumentsParameterName]; }
    }

    public DoubleValue PunishmentFactor {
      get { return new DoubleValue(10.0); }
    }
    public IntValue TrainingSamplesStart { get { return new IntValue(ClassificationProblemData.TrainingIndizes.First()); } }
    public IntValue TrainingSamplesEnd {
      get {
        int endIndex = (int)(ClassificationProblemData.TrainingIndizes.Count() * (1.0 - ClassificationProblemData.ValidationPercentage.Value) - 1);
        if (endIndex < 0) endIndex = 0;
        return new IntValue(ClassificationProblemData.TrainingIndizes.ElementAt(endIndex));
      }
    }
    public IntValue ValidationSamplesStart { get { return TrainingSamplesEnd; } }
    public IntValue ValidationSamplesEnd { get { return new IntValue(ClassificationProblemData.TrainingIndizes.Last() + 1); } }
    public IntValue TestSamplesStart { get { return ClassificationProblemData.TestSamplesStart; } }
    public IntValue TestSamplesEnd { get { return ClassificationProblemData.TestSamplesEnd; } }
    #endregion

    [StorableConstructor]
    private SymbolicClassificationProblem(bool deserializing) : base(deserializing) { }
    private SymbolicClassificationProblem(SymbolicClassificationProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterParameterEvents();
    }

    public SymbolicClassificationProblem()
      : base() {
      Parameters.Add(new ValueParameter<ISymbolicExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, "The interpreter that should be used to evaluate the symbolic expression tree."));
      Parameters.Add(new ValueParameter<ISymbolicExpressionGrammar>(FunctionTreeGrammarParameterName, "The grammar that should be used for symbolic regression models."));
      Parameters.Add(new ValueParameter<IntValue>(MaxExpressionLengthParameterName, "Maximal length of the symbolic expression."));
      Parameters.Add(new ValueParameter<IntValue>(MaxExpressionDepthParameterName, "Maximal depth of the symbolic expression."));
      Parameters.Add(new ValueParameter<DoubleValue>(UpperEstimationLimitParameterName, "The upper limit for the estimated value that can be returned by the symbolic regression model."));
      Parameters.Add(new ValueParameter<DoubleValue>(LowerEstimationLimitParameterName, "The lower limit for the estimated value that can be returned by the symbolic regression model."));
      Parameters.Add(new ValueParameter<IntValue>(MaxFunctionDefiningBranchensParameterName, "Maximal number of automatically defined functions."));
      Parameters.Add(new ValueParameter<IntValue>(MaxFunctionArgumentsParameterName, "Maximal number of arguments of automatically defined functions."));

      SolutionCreator = new ProbabilisticTreeCreator();
      Evaluator = new SymbolicClassifacitionMeanSquaredErrorEvaluator();
      ParameterizeSolutionCreator();
      Maximization = new BoolValue(false);
      FunctionTreeGrammar = new GlobalSymbolicExpressionGrammar(new FullFunctionalExpressionGrammar());
      SymbolicExpressionTreeInterpreter = new SimpleArithmeticExpressionInterpreter();
      MaxExpressionLength = new IntValue(100);
      MaxExpressionDepth = new IntValue(10);
      MaxFunctionDefiningBranches = new IntValue(0);
      MaxFunctionArguments = new IntValue(0);

      InitializeOperators();
      RegisterParameterEvents();

      UpdateEstimationLimits();
      ParameterizeEvaluator();
      ParameterizeSolutionCreator();
      ParameterizeGrammar();
      ParameterizeOperators();
      ParameterizeAnalyzers();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SymbolicClassificationProblem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterParameterEvents();
    }

    private void RegisterParameterEvents() {
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      FunctionTreeGrammarParameter.ValueChanged += new EventHandler(FunctionTreeGrammarParameter_ValueChanged);

      MaxFunctionArgumentsParameter.ValueChanged += new EventHandler(ArchitectureParameter_ValueChanged);
      MaxFunctionDefiningBranchesParameter.ValueChanged += new EventHandler(ArchitectureParameter_ValueChanged);
      MaxFunctionArgumentsParameter.Value.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
      MaxFunctionDefiningBranchesParameter.Value.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
    }

    protected override void OnEvaluatorChanged() {
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      ParameterizeProblem();
      base.OnEvaluatorChanged();
    }

    protected override void OnSolutionCreatorChanged() {
      ParameterizeSolutionCreator();
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      base.OnSolutionCreatorChanged();
    }
    private void SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged(object sender, System.EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeOperators();
      ParameterizeAnalyzers();
    }

    protected override void OnClassificationProblemDataChanged() {
      ParameterizeAnalyzers();
      ParameterizeGrammar();
      ParameterizeEvaluator();
      UpdateEstimationLimits();
      base.OnClassificationProblemDataChanged();
    }

    private void FunctionTreeGrammarParameter_ValueChanged(object sender, System.EventArgs e) {
      if (!(FunctionTreeGrammar is GlobalSymbolicExpressionGrammar)) {
        FunctionTreeGrammar = new GlobalSymbolicExpressionGrammar(FunctionTreeGrammar);
      }
      OnGrammarChanged();
    }
    private void OnGrammarChanged() {
      ParameterizeGrammar();
    }

    private void ArchitectureParameter_ValueChanged(object sender, EventArgs e) {
      MaxFunctionArgumentsParameter.Value.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
      MaxFunctionDefiningBranchesParameter.Value.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
      OnArchitectureParameterChanged();
    }
    private void ArchitectureParameterValue_ValueChanged(object sender, EventArgs e) {
      OnArchitectureParameterChanged();
    }
    private void OnArchitectureParameterChanged() {
      ParameterizeGrammar();
    }

    private void InitializeOperators() {
      Operators.AddRange(ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeOperator>().OfType<IOperator>());
      Operators.Add(new MinAverageMaxSymbolicExpressionTreeSizeAnalyzer());
      Operators.Add(new SymbolicRegressionVariableFrequencyAnalyzer());
      Operators.Add(new SymbolicExpressionSymbolFrequencyAnalyzer());
      Operators.Add(new ValidationBestSymbolicClassificationSolutionAnalyzer());
      Operators.Add(new TrainingBestSymbolicClassificationSolutionAnalyzer());
    }

    #region operator parameterization
    private void UpdateEstimationLimits() {
      if (TrainingSamplesStart.Value < TrainingSamplesEnd.Value &&
        ClassificationProblemData.Dataset.VariableNames.Contains(ClassificationProblemData.TargetVariable.Value)) {
        var targetValues = ClassificationProblemData.Dataset.GetVariableValues(ClassificationProblemData.TargetVariable.Value, TrainingSamplesStart.Value, TrainingSamplesEnd.Value);
        var mean = targetValues.Average();
        var range = targetValues.Max() - targetValues.Min();
        UpperEstimationLimit = new DoubleValue(mean + PunishmentFactor.Value * range);
        LowerEstimationLimit = new DoubleValue(mean - PunishmentFactor.Value * range);
      }
    }

    private void ParameterizeEvaluator() {
      if (Evaluator != null) {
        Evaluator.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        Evaluator.RegressionProblemDataParameter.ActualName = ClassificationProblemDataParameter.Name;
        Evaluator.SamplesStartParameter.Value = TrainingSamplesStart;
        Evaluator.SamplesEndParameter.Value = TrainingSamplesEnd;
      }
    }

    private void ParameterizeGrammar() {
      List<LaggedVariable> laggedSymbols = FunctionTreeGrammar.Symbols.OfType<LaggedVariable>().ToList();
      foreach (Symbol symbol in laggedSymbols)
        FunctionTreeGrammar.RemoveSymbol(symbol);
      foreach (var varSymbol in FunctionTreeGrammar.Symbols.OfType<HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.Variable>()) {
        varSymbol.VariableNames = ClassificationProblemData.InputVariables.CheckedItems.Select(x => x.Value.Value);
      }
      foreach (var varSymbol in FunctionTreeGrammar.Symbols.OfType<HeuristicLab.Problems.DataAnalysis.Symbolic.Symbols.VariableCondition>()) {
        varSymbol.VariableNames = ClassificationProblemData.InputVariables.CheckedItems.Select(x => x.Value.Value);
      }
      var globalGrammar = FunctionTreeGrammar as GlobalSymbolicExpressionGrammar;
      if (globalGrammar != null) {
        globalGrammar.MaxFunctionArguments = MaxFunctionArguments.Value;
        globalGrammar.MaxFunctionDefinitions = MaxFunctionDefiningBranches.Value;
      }
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

    private void ParameterizeAnalyzers() {
      foreach (ISymbolicRegressionAnalyzer analyzer in Operators.OfType<ISymbolicRegressionAnalyzer>()) {
        analyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        var bestValidationSolutionAnalyzer = analyzer as ValidationBestSymbolicClassificationSolutionAnalyzer;
        if (bestValidationSolutionAnalyzer != null) {
          bestValidationSolutionAnalyzer.ClassificationProblemDataParameter.ActualName = ClassificationProblemDataParameter.Name;
          bestValidationSolutionAnalyzer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
          bestValidationSolutionAnalyzer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
          bestValidationSolutionAnalyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
          bestValidationSolutionAnalyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
          bestValidationSolutionAnalyzer.ValidationSamplesStartParameter.Value = ValidationSamplesStart;
          bestValidationSolutionAnalyzer.ValidationSamplesEndParameter.Value = ValidationSamplesEnd;
        }
        var bestTrainingSolutionAnalyzer = analyzer as TrainingBestSymbolicClassificationSolutionAnalyzer;
        if (bestTrainingSolutionAnalyzer != null) {
          bestTrainingSolutionAnalyzer.ProblemDataParameter.ActualName = ClassificationProblemDataParameter.Name;
          bestTrainingSolutionAnalyzer.UpperEstimationLimitParameter.ActualName = UpperEstimationLimitParameter.Name;
          bestTrainingSolutionAnalyzer.LowerEstimationLimitParameter.ActualName = LowerEstimationLimitParameter.Name;
          bestTrainingSolutionAnalyzer.SymbolicExpressionTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
          bestTrainingSolutionAnalyzer.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        }
        var varFreqAnalyzer = analyzer as SymbolicRegressionVariableFrequencyAnalyzer;
        if (varFreqAnalyzer != null) {
          varFreqAnalyzer.ProblemDataParameter.ActualName = ClassificationProblemDataParameter.Name;
        }
      }
    }

    private void ParameterizeProblem() {
      if (Maximization != null) {
        Maximization.Value = Evaluator.Maximization;
      } else {
        Maximization = new BoolValue(Evaluator.Maximization);
      }
    }
    #endregion
  }
}
