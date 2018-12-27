#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
 * 
 * Author: Sabine Winkler
 */

#endregion

using System;
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.IntegerVectorEncoding;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.DataAnalysis;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.GrammaticalEvolution.Mappers;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.GrammaticalEvolution {
  [StorableClass]
  public abstract class GESymbolicDataAnalysisProblem<T, U, V> : HeuristicOptimizationProblem<U, V>, IDataAnalysisProblem<T>,
                                                                 IGESymbolicDataAnalysisProblem, IStorableContent,
                                                                 IProblemInstanceConsumer<T>, IProblemInstanceExporter<T>
    where T : class, IDataAnalysisProblemData
    where U : class, IGESymbolicDataAnalysisEvaluator<T>
    where V : class, IIntegerVectorCreator {

    #region parameter names & descriptions
    private const string ProblemDataParameterName = "ProblemData";
    private const string SymbolicExpressionTreeGrammarParameterName = "SymbolicExpressionTreeGrammar";
    private const string SymbolicExpressionTreeInterpreterParameterName = "SymbolicExpressionTreeInterpreter";
    private const string MaximumSymbolicExpressionTreeLengthParameterName = "MaximumSymbolicExpressionTreeLength";
    private const string RelativeNumberOfEvaluatedSamplesParameterName = "RelativeNumberOfEvaluatedSamples";
    private const string FitnessCalculationPartitionParameterName = "FitnessCalculationPartition";
    private const string ValidationPartitionParameterName = "ValidationPartition";
    private const string ApplyLinearScalingParameterName = "ApplyLinearScaling";
    private const string BoundsParameterName = "Bounds";
    private const string GenotypeToPhenotypeMapperParameterName = "GenotypeToPhenotypeMapper";
    private const string ProblemDataParameterDescription = "";
    private const string SymbolicExpressionTreeGrammarParameterDescription = "The grammar that should be used for symbolic expression tree.";
    private const string SymbolicExpressionTreeInterpreterParameterDescription = "The interpreter that should be used to evaluate the symbolic expression tree.";
    private const string MaximumSymbolicExpressionTreeLengthParameterDescription = "Maximal length of the symbolic expression.";
    private const string RelativeNumberOfEvaluatedSamplesParameterDescription = "The relative number of samples of the dataset partition, which should be randomly chosen for evaluation.";
    private const string FitnessCalculationPartitionParameterDescription = "The partition of the problem data training partition, that should be used to calculate the fitness of an individual.";
    private const string ValidationPartitionParameterDescription = "The partition of the problem data training partition, that should be used to select the best model from (optional).";
    private const string ApplyLinearScalingParameterDescription = "Flag that indicates if the individual should be linearly scaled before evaluating.";
    private const string BoundsParameterDescription = "The integer number range in which the single genomes of a genotype are created.";
    private const string GenotypeToPhenotypeMapperParameterDescription = "Maps the genotype (an integer vector) to the phenotype (a symbolic expression tree).";
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
    public IFixedValueParameter<IntValue> MaximumSymbolicExpressionTreeLengthParameter {
      get { return (IFixedValueParameter<IntValue>)Parameters[MaximumSymbolicExpressionTreeLengthParameterName]; }
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
    public IValueParameter<IntMatrix> BoundsParameter {
      get { return (IValueParameter<IntMatrix>)Parameters[BoundsParameterName]; }
    }
    public IValueParameter<IGenotypeToPhenotypeMapper> GenotypeToPhenotypeMapperParameter {
      get { return (IValueParameter<IGenotypeToPhenotypeMapper>)Parameters[GenotypeToPhenotypeMapperParameterName]; }
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

    public IntValue MaximumSymbolicExpressionTreeLength {
      get { return MaximumSymbolicExpressionTreeLengthParameter.Value; }
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
    protected GESymbolicDataAnalysisProblem(bool deserializing) : base(deserializing) { }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }
    protected GESymbolicDataAnalysisProblem(GESymbolicDataAnalysisProblem<T, U, V> original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }

    protected GESymbolicDataAnalysisProblem(T problemData, U evaluator, V solutionCreator)
      : base(evaluator, solutionCreator) {
      Parameters.Add(new ValueParameter<T>(ProblemDataParameterName, ProblemDataParameterDescription, problemData));
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisGrammar>(SymbolicExpressionTreeGrammarParameterName, SymbolicExpressionTreeGrammarParameterDescription));
      Parameters.Add(new ValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(SymbolicExpressionTreeInterpreterParameterName, SymbolicExpressionTreeInterpreterParameterDescription));
      Parameters.Add(new FixedValueParameter<IntValue>(MaximumSymbolicExpressionTreeLengthParameterName, MaximumSymbolicExpressionTreeLengthParameterDescription));
      Parameters.Add(new FixedValueParameter<IntRange>(FitnessCalculationPartitionParameterName, FitnessCalculationPartitionParameterDescription));
      Parameters.Add(new FixedValueParameter<IntRange>(ValidationPartitionParameterName, ValidationPartitionParameterDescription));
      Parameters.Add(new FixedValueParameter<PercentValue>(RelativeNumberOfEvaluatedSamplesParameterName, RelativeNumberOfEvaluatedSamplesParameterDescription, new PercentValue(1)));
      Parameters.Add(new FixedValueParameter<BoolValue>(ApplyLinearScalingParameterName, ApplyLinearScalingParameterDescription, new BoolValue(false)));
      IntMatrix m = new IntMatrix(new int[,] { { 0, 100 } });
      Parameters.Add(new ValueParameter<IntMatrix>(BoundsParameterName, BoundsParameterDescription, m));
      Parameters.Add(new ValueParameter<IGenotypeToPhenotypeMapper>(GenotypeToPhenotypeMapperParameterName, GenotypeToPhenotypeMapperParameterDescription, new DepthFirstMapper()));

      SymbolicExpressionTreeInterpreterParameter.Hidden = true;
      ApplyLinearScalingParameter.Hidden = true;

      if (problemData.AllowedInputVariables.Any(name => !problemData.Dataset.VariableHasType<double>(name))) throw new NotSupportedException("Categorical variables are not supported");
      SymbolicExpressionTreeGrammar = new GESymbolicExpressionGrammar(problemData.AllowedInputVariables, problemData.AllowedInputVariables.Count() * 3);
      SymbolicExpressionTreeInterpreter = new SymbolicDataAnalysisExpressionTreeLinearInterpreter();

      FitnessCalculationPartition.Start = ProblemData.TrainingPartition.Start;
      FitnessCalculationPartition.End = ProblemData.TrainingPartition.End;

      InitializeOperators();

      UpdateGrammar();
      RegisterEventHandlers();
    }

    private void DeregisterGrammarHandler() {
      SymbolicExpressionTreeGrammarParameter.ValueChanged -= SymbolicExpressionTreeGrammarParameter_ValueChanged;
    }
    private void RegisterGrammarHandler() {
      SymbolicExpressionTreeGrammarParameter.ValueChanged += SymbolicExpressionTreeGrammarParameter_ValueChanged;
    }

    private void UpdateGrammar() {
      DeregisterGrammarHandler();
      // create a new grammar instance with the correct allowed input variables
      SymbolicExpressionTreeGrammarParameter.Value =
        new GESymbolicExpressionGrammar(ProblemData.AllowedInputVariables, ProblemData.AllowedInputVariables.Count() * 3);
      RegisterGrammarHandler();
    }

    private void InitializeOperators() {
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IIntegerVectorOperator>());
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

      RegisterGrammarHandler();
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

    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      Evaluator.SymbolicExpressionTreeParameter.ActualNameChanged += new EventHandler(Evaluator_SymbolicExpressionTreeParameter_ActualNameChanged);
      ParameterizeOperators();
    }

    private void Evaluator_SymbolicExpressionTreeParameter_ActualNameChanged(object sender, EventArgs e) {
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
      foreach (var op in operators.OfType<IGESymbolicDataAnalysisEvaluator<T>>()) {
        op.ProblemDataParameter.ActualName = ProblemDataParameterName;
        op.SymbolicExpressionTreeParameter.ActualName = Evaluator.SymbolicExpressionTreeParameter.ActualName;
        op.EvaluationPartitionParameter.ActualName = FitnessCalculationPartitionParameter.Name;
        op.RelativeNumberOfEvaluatedSamplesParameter.ActualName = RelativeNumberOfEvaluatedSamplesParameter.Name;
        op.ApplyLinearScalingParameter.ActualName = ApplyLinearScalingParameter.Name;
        op.IntegerVectorParameter.ActualName = SolutionCreator.IntegerVectorParameter.Name;
        op.GenotypeToPhenotypeMapperParameter.ActualName = GenotypeToPhenotypeMapperParameter.Name;
        op.SymbolicExpressionTreeGrammarParameter.ActualName = SymbolicExpressionTreeGrammarParameter.Name;
      }
      foreach (var op in operators.OfType<IIntegerVectorCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.IntegerVectorParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.IntegerVectorParameter.ActualName;
      }
      foreach (var op in operators.OfType<IIntegerVectorManipulator>()) {
        op.IntegerVectorParameter.ActualName = SolutionCreator.IntegerVectorParameter.ActualName;
      }
      foreach (var op in operators.OfType<IIntegerVectorCreator>()) {
        op.BoundsParameter.ActualName = BoundsParameter.Name;
        op.LengthParameter.ActualName = MaximumSymbolicExpressionTreeLengthParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicExpressionTreeAnalyzer>()) {
        op.SymbolicExpressionTreeParameter.ActualName = Evaluator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (var op in operators.OfType<ISymbolicDataAnalysisSingleObjectiveAnalyzer>()) {
        op.ApplyLinearScalingParameter.ActualName = ApplyLinearScalingParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicDataAnalysisMultiObjectiveAnalyzer>()) {
        op.ApplyLinearScalingParameter.ActualName = ApplyLinearScalingParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicDataAnalysisAnalyzer>()) {
        op.SymbolicExpressionTreeParameter.ActualName = Evaluator.SymbolicExpressionTreeParameter.ActualName;
      }
      foreach (var op in operators.OfType<IGESymbolicDataAnalysisValidationAnalyzer<U, T>>()) {
        op.RelativeNumberOfEvaluatedSamplesParameter.ActualName = RelativeNumberOfEvaluatedSamplesParameter.Name;
        op.ValidationPartitionParameter.ActualName = ValidationPartitionParameter.Name;
      }
      foreach (var op in operators.OfType<ISymbolicDataAnalysisInterpreterOperator>()) {
        op.SymbolicDataAnalysisTreeInterpreterParameter.ActualName = SymbolicExpressionTreeInterpreterParameter.Name;
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
