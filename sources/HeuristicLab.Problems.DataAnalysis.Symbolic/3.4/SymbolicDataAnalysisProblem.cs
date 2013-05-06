#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic {
  [StorableClass]
  public abstract class SymbolicDataAnalysisProblem<T, U, V> : HeuristicOptimizationProblem<U, V>, IDataAnalysisProblem<T>, ISymbolicDataAnalysisProblem, IStorableContent,
    IProblemInstanceConsumer<T>, IProblemInstanceExporter<T>
    where T : class, IDataAnalysisProblemData
    where U : class, ISymbolicDataAnalysisEvaluator<T>
    where V : class, ISymbolicDataAnalysisSolutionCreator {

    #region parameter names & descriptions
    private const string ProblemDataParameterName = "ProblemData";
    private const string SymbolicExpressionTreeGrammarParameterName = "SymbolicExpressionTreeGrammar";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string MaximumSymbolicExpressionTreeDepthParameterName = "MaximumSymbolicExpressionTreeDepth";
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string MaximumFunctionDefinitionsParameterName = "MaximumFunctionDefinitions";
    private const string MaximumFunctionArgumentsParameterName = "MaximumFunctionArguments";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";
    private const string FitnessCalculationPartitionParameterName = "FitnessCalculationPartition";
    private const string ValidationPartitionParameterName = "ValidationPartition";
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";

    private const string ProblemDataParameterDescription = "";
    private const string SymbolicExpressionTreeGrammarParameterDescription = "The grammar that should be used for symbolic expression tree.";
    private const string SymoblicExpressionTreeInterpreterParameterDescription = "The interpreter that should be used to evaluate the symbolic expression tree.";
    private const string MaximumSymbolicExpressionTreeDepthParameterDescription = "Maximal depth of the symbolic expression. The minimum depth needed for the algorithm is 3 because two levels are reserved for the ProgramRoot and the Start symbol.";
    private const string MaximumSymbolicExpressionTreeLengthParameterDescription = "Maximal length of the symbolic expression.";
    private const string MaximumFunctionDefinitionsParameterDescription = "Maximal number of automatically defined functions";
    private const string MaximumFunctionArgumentsParameterDescription = "Maximal number of arguments of automatically defined functions.";
    private const string RelativeNumberOfEvaluatedSamplesParameterDescription = "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation.";
    private const string FitnessCalculationPartitionParameterDescription = "The partition of the problem data training partition, that should be used to calculate the fitness of an individual.";
    private const string ValidationPartitionParameterDescription = "The partition of the problem data training partition, that should be used to select the best model from (optional).";
    private const string ApplyLinearScalingParameterDescription = "Flag that indicates if the individual should be linearly scaled before evaluating.";
    #endregion

    #region parameter properties
    IParameter IDataAnalysisProblem.ProblemDataParameter {
      get { return ProblemDataParameter; }
    }
    public IValueParameter<T> ProblemDataParameter {
      get { return (IValueParameter<T>)Parameters[ProblemDataParameterName]; }
    }
    public IValueParameter<ISymbolicDataAnalysisGrammar> SymbolicExpressionTreeGrammarParameter {
      get { return (IValueParameter<ISymbolicDataAnalysisGrammar>)Parameters[SymbolicExpressionTreeGrammarParameterName]; }
    }
    public IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> SymbolicExpressionTreeInterpreterParameter {
      get { return (IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[SymbolicExpressionTreeInterpreterParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaximumSymbolicExpressionTreeDepthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeDepthParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaximumFunctionDefinitionsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumFunctionDefinitionsParameterName]; }
    }
    public IFixedValueParameter<IntValue> MaximumFunctionArgumentsParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumFunctionArgumentsParameterName]; }
    }
    public IFixedValueParameter<PercentValue> RelativeNumberOfEvaluatedSamplesParameter {
      get { return (IFixedValueParameter<PercentValue>)Parameters[RelativeNumberOfEvaluatedSamplesParameterName]; }
    }
    public IFixedValueParameter<IntRange> FitnessCalculationPartitionParameter {
      get { return (IFixedValueParameter<IntRange>)Parameters[FitnessCalculationPartitionParameterName]; }
    }
    public IFixedValueParameter<IntRange> ValidationPartitionParameter {
      get { return (IFixedValueParameter<IntRange>)Parameters[ValidationPartitionParameterName]; }
    }
    public IFixedValueParameter<BoolValue> ApplyLinearScalingParameter {
      get { return (IFixedValueParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName]; }
    }
    #endregion

    #region properties
    public string Filename { get; set; }
    public static new Image StaticItemImage { get { return VSImageLibrary.Type; } }

    IDataAnalysisProblemData IDataAnalysisProblem.ProblemData {
      get { return ProblemData; }
    }
    public T ProblemData {
      get { return ProblemDataParameter.Value; }
      set { ProblemDataParameter.Value = value; }
    }

    public ISymbolicDataAnalysisGrammar SymbolicExpressionTreeGrammar {
      get { return SymbolicExpressionTreeGrammarParameter.Value; }
      set { SymbolicExpressionTreeGrammarParameter.Value = value; }
    }
    public ISymbolicDataAnalysisExpressionTreeInterpreter SymbolicExpressionTreeInterpreter {
      get { return SymbolicExpressionTreeInterpreterParameter.Value; }
      set { SymbolicExpressionTreeInterpreterParameter.Value = value; }
    }

    public IntValue MaximumSymbolicExpressionTreeDepth {
      get { return MaximumSymbolicExpressionTreeDepthParameter.Value; }
    }
    public IntValue MaximumSymbolicExpressionTreeLength {
      get { return MaximumSymbolicExpressionTreeLengthParameter.Value; }
    }
    public IntValue MaximumFunctionDefinitions {
      get { return MaximumFunctionDefinitionsParameter.Value; }
    }
    public IntValue MaximumFunctionArguments {
      get { return MaximumFunctionArgumentsParameter.Value; }
    }
    public PercentValue RelativeNumberOfEvaluatedSamples {
      get { return RelativeNumberOfEvaluatedSamplesParameter.Value; }
    }

    public IntRange FitnessCalculationPartition {
      get { return FitnessCalculationPartitionParameter.Value; }
    }
    public IntRange ValidationPartition {
      get { return ValidationPartitionParameter.Value; }
    }
    public BoolValue ApplyLinearScaling {
      get { return ApplyLinearScalingParameter.Value; }
    }
    #endregion

    [StorableConstructor]
    protected SymbolicDataAnalysisProblem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (!Parameters.ContainsKey(ApplyLinearScalingParameterName)) {
        Parameters.Add(new FixedValueParameter<BoolValue>(ApplyLinearScalingParameterName, ApplyLinearScalingParameterDescription, new BoolValue(false)));
        ApplyLinearScalingParameter.Hidden = true;

        //it is assumed that for all symbolic regression algorithms linear scaling was set to true
        //there is no possibility to determine the previous value of the parameter as it was stored in the evaluator
        if (GetType().Name.Contains("SymbolicRegression"))
          ApplyLinearScaling.Value = true;
      }

      RegisterEventHandlers();
    }
    protected SymbolicDataAnalysisProblem(SymbolicDataAnalysisProblem<T, U, V> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    protected SymbolicDataAnalysisProblem(T problemData, U evaluator, V solutionCreator)
      : base(evaluator, solutionCreator) {
      Parameters.Add(new ValueParameter<T>(ProblemDataParameterName, ProblemDataParameterDescription, problemData));
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisGrammar>(SymbolicExpressionTreeGrammarParameterName, SymbolicExpressionTreeGrammarParameterDescription));
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, SymoblicExpressionTreeInterpreterParameterDescription));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumSymbolicExpressionTreeDepthParameterName, MaximumSymbolicExpressionTreeDepthParameterDescription));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, MaximumSymbolicExpressionTreeLengthParameterDescription));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumFunctionDefinitionsParameterName, MaximumFunctionDefinitionsParameterDescription));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumFunctionArgumentsParameterName, MaximumFunctionArgumentsParameterDescription));
      Parameters.Add(new FixedValueParameter<IntRange>(FitnessCalculationPartitionParameterName, FitnessCalculationPartitionParameterDescription));
      Parameters.Add(new FixedValueParameter<IntRange>(ValidationPartitionParameterName, ValidationPartitionParameterDescription));
      Parameters.Add(new FixedValueParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, RelativeNumberOfEvaluatedSamplesParameterDescription, new PercentValue(1)));
      Parameters.Add(new FixedValueParameter<BoolValue>(ApplyLinearScalingParameterName, ApplyLinearScalingParameterDescription, new BoolValue(false)));

      SymbolicExpressionTreeInterpreterParameter.Hidden = true;
      MaximumFunctionArgumentsParameter.Hidden = true;
      MaximumFunctionDefinitionsParameter.Hidden = true;
      ApplyLinearScalingParameter.Hidden = true;

      SymbolicExpressionTreeGrammar = new TypeCoherentExpressionGrammar();
      SymbolicExpressionTreeInterpreter = new SymbolicDataAnalysisExpressionTreeInterpreter();

      FitnessCalculationPartition.Start = ProblemData.TrainingPartition.Start;
      FitnessCalculationPartition.End = ProblemData.TrainingPartition.End;

      InitializeOperators();

      UpdateGrammar();
      RegisterEventHandlers();
    }

    protected virtual void UpdateGrammar() {
      SymbolicExpressionTreeGrammar.MaximumFunctionArguments = MaximumFunctionArguments.Value;
      SymbolicExpressionTreeGrammar.MaximumFunctionDefinitions = MaximumFunctionDefinitions.Value;
      foreach (var varSymbol in SymbolicExpressionTreeGrammar.Symbols.OfType<HeuristicLab.Problems.DataAnalysis.Symbolic.Variable>()) {
        if (!varSymbol.Fixed) {
          varSymbol.AllVariableNames = ProblemData.InputVariables.Select(x => x.Value);
          varSymbol.VariableNames = ProblemData.AllowedInputVariables;
        }
      }
      foreach (var varSymbol in SymbolicExpressionTreeGrammar.Symbols.OfType<HeuristicLab.Problems.DataAnalysis.Symbolic.VariableCondition>()) {
        if (!varSymbol.Fixed) {
          varSymbol.AllVariableNames = ProblemData.InputVariables.Select(x => x.Value);
          varSymbol.VariableNames = ProblemData.AllowedInputVariables;
        }
      }
    }

    private void InitializeOperators() {
      Operators.AddRange(ApplicationManager.Manager.GetInstances<ISymbolicExpressionTreeOperator>());
      Operators.AddRange(ApplicationManager.Manager.GetInstances<ISymbolicDataAnalysisExpressionCrossover<T>>());
      Operators.Add(new SymbolicExpressionSymbolFrequencyAnalyzer());
      Operators.Add(new SymbolicDataAnalysisVariableFrequencyAnalyzer());
      Operators.Add(new MinAverageMaxSymbolicExpressionTreeLengthAnalyzer());
      Operators.Add(new SymbolicExpressionTreeLengthAnalyzer());
      ParameterizeOperators();
    }

    #region events
    private void RegisterEventHandlers() {
      ProblemDataParameter.ValueChanged += new EventHandler(ProblemDataParameter_ValueChanged);
      ProblemDataParameter.Value.Changed += (object sender, EventArgs e) => OnProblemDataChanged();

      SymbolicExpressionTreeGrammarParameter.ValueChanged += new EventHandler(SymbolicExpressionTreeGrammarParameter_ValueChanged);

      MaximumFunctionArguments.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
      MaximumFunctionDefinitions.ValueChanged += new EventHandler(ArchitectureParameterValue_ValueChanged);
      MaximumSymbolicExpressionTreeDepth.ValueChanged += new EventHandler(MaximumSymbolicExpressionTreeDepth_ValueChanged);
    }

    private void ProblemDataParameter_ValueChanged(object sender, EventArgs e) {
      ValidationPartition.Start = 0;
      ValidationPartition.End = 0;
      ProblemDataParameter.Value.Changed += (object s, EventArgs args) => OnProblemDataChanged();
      OnProblemDataChanged();
    }

    private void SymbolicExpressionTreeGrammarParameter_ValueChanged(object sender, EventArgs e) {
      UpdateGrammar();
    }

    private void ArchitectureParameterValue_ValueChanged(object sender, EventArgs e) {
      UpdateGrammar();
    }

    private void MaximumSymbolicExpressionTreeDepth_ValueChanged(object sender, EventArgs e) {
      if (MaximumSymbolicExpressionTreeDepth != null && MaximumSymbolicExpressionTreeDepth.Value < 3)
        MaximumSymbolicExpressionTreeDepth.Value = 3;
    }

    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      SolutionCreator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged);
      ParameterizeOperators();
    }

    private void SolutionCreator_SymbolicExpressionTreeParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeOperators();
    }

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeOperators();
    }

    public event EventHandler ProblemDataChanged;
    protected virtual void OnProblemDataChanged() {
      FitnessCalculationPartition.Start = ProblemData.TrainingPartition.Start;
      FitnessCalculationPartition.End = ProblemData.TrainingPartition.End;

      UpdateGrammar();
      ParameterizeOperators();

      var handler = ProblemDataChanged;
      if (handler != null) handler(this, EventArgs.Empty);

      OnReset();
    }
    #endregion

    protected virtual void ParameterizeOperators() {
      var operators = Parameters.OfType<IValueParameter>().Select(p => p.Value).OfType<IOperator>().Union(Operators).ToList();

      foreach (var op in operators.OfType<ISymbolicExpressionTreeGrammarBasedOperator>()) {
        op.SymbolicExpressionTreeGrammarParameter.ActualName = SymbolicExpressionTreeGrammarParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicExpressionTreeSizeConstraintOperator>()) {
        op.MaximumSymbolicExpressionTreeDepthParameter.ActualName = MaximumSymbolicExpressionTreeDepthParameter.Name;
        op.MaximumSymbolicExpressionTreeLengthParameter.ActualName = MaximumSymbolicExpressionTreeLengthParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicExpressionTreeArchitectureAlteringOperator>()) {
        op.MaximumFunctionArgumentsParameter.ActualName = MaximumFunctionArgumentsParameter.Name;
        op.MaximumFunctionDefinitionsParameter.ActualName = MaximumFunctionDefinitionsParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicDataAnalysisEvaluator<T>>()) {
        op.ProblemDataParameter.ActualName = ProblemDataParameterName;
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.EvaluationPartitionParameter.ActualName = FitnessCalculationPartitionParameter.Name;
        op.RelativeNumberOfEvaluatedSamplesParameter.ActualName = RelativeNumberOfEvaluatedSamplesParameter.Name;
        op.ApplyLinearScalingParameter.ActualName = ApplyLinearScalingParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicExpressionTreeCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (var op in operators.OfType<ISymbolicExpressionTreeManipulator>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (var op in operators.OfType<ISymbolicExpressionTreeAnalyzer>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (var op in operators.OfType<ISymbolicDataAnalysisSingleObjectiveAnalyzer>()) {
        op.ApplyLinearScalingParameter.ActualName = ApplyLinearScalingParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicDataAnalysisMultiObjectiveAnalyzer>()) {
        op.ApplyLinearScalingParameter.ActualName = ApplyLinearScalingParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicDataAnalysisAnalyzer>()) {
        op.SymbolicExpressionTreeParameter.ActualName = SolutionCreator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (var op in operators.OfType<ISymbolicDataAnalysisValidationAnalyzer<U, T>>()) {
        op.RelativeNumberOfEvaluatedSamplesParameter.ActualName = RelativeNumberOfEvaluatedSamplesParameter.Name;
        op.ValidationPartitionParameter.ActualName = ValidationPartitionParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicDataAnalysisInterpreterOperator>()) {
        op.SymbolicDataAnalysisTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicDataAnalysisExpressionCrossover<T>>()) {
        op.EvaluationPartitionParameter.ActualName = FitnessCalculationPartitionParameter.Name;
        op.ProblemDataParameter.ActualName = ProblemDataParameter.Name;
        op.EvaluationPartitionParameter.ActualName = FitnessCalculationPartitionParameter.Name;
        op.RelativeNumberOfEvaluatedSamplesParameter.ActualName = RelativeNumberOfEvaluatedSamplesParameter.Name;
        op.EvaluatorParameter.ActualName = EvaluatorParameter.Name;
      }
    }

    #region Import & Export
    public virtual void Load(T data) {
      Name = data.Name;
      Description = data.Description;
      ProblemData = data;
    }

    public virtual T Export() {
      return ProblemData;
    }
    #endregion
  }
}
