#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Problems.DataAnalysis.Symbolic;
using HeuristicLab.Problems.DataAnalysis.Symbolic.Regression;
using HeuristicLab.Problems.Dynamic;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.DataAnalysis.Dynamic; 

[Item("Dynamic Symbolic Regression Problem", "Applies different dataset partitions as a regression problem.")]
[Creatable(CreatableAttribute.Categories.Problems, Priority = 210)]
[StorableType("47E9498A-545C-47FE-AD2E-9B10DB683320")]
public class DynamicSymbolicRegressionProblem
  : SingleObjectiveStatefulDynamicProblem<SymbolicExpressionTreeEncoding, SymbolicExpressionTree, DynamicSymbolicRegressionProblemState>, IProblemInstanceConsumer<DynamicRegressionProblemData> {
  #region Propeties
  public override bool Maximization => Parameters.ContainsKey(StateParameterName) ? State.Maximization : true;
  
  private const string ProblemDataParameterName = "ProblemData";
  private const string ModelEvaluatorParameterName = "Model Evaluator";
  private const string GrammarParameterName = "Grammar";
  private const string InterpreterParameterName = "SymbolicExpressionTreeInterpreter";
  private const string ApplyLinearScalingParameterName = "Apply Linear Scaling";
  private const string OptimizeParametersParameterName = "Optimize Parameters";
  private const string MaximumTreeDepthParameterName = "MaximumTreeDepth";
  private const string MaximumTreeLengthParameterName = "MaximumTreeSize";
  private const string EstimationLimitsParameterName = "EstimationLimits";

  
  
  public IValueParameter<DynamicRegressionProblemData> ProblemDataParameter => (IValueParameter<DynamicRegressionProblemData>)Parameters[ProblemDataParameterName];
  public IValueParameter<ISymbolicRegressionSingleObjectiveEvaluator> ModelEvaluatorParameter => (IValueParameter<ISymbolicRegressionSingleObjectiveEvaluator>)Parameters[ModelEvaluatorParameterName];
  public IValueParameter<ISymbolicDataAnalysisGrammar> GrammarParameter => (IValueParameter<ISymbolicDataAnalysisGrammar>)Parameters[GrammarParameterName];
  public IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter> InterpreterParameter => (IValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>)Parameters[InterpreterParameterName];
  public IFixedValueParameter<BoolValue> ApplyLinearScalingParameter => (IFixedValueParameter<BoolValue>)Parameters[ApplyLinearScalingParameterName];
  public IFixedValueParameter<BoolValue> OptimizeParametersParameter => (IFixedValueParameter<BoolValue>)Parameters[OptimizeParametersParameterName];
  public IFixedValueParameter<IntValue> MaximumTreeDepthParameter => (IFixedValueParameter<IntValue>)Parameters[MaximumTreeDepthParameterName];
  public IFixedValueParameter<IntValue> MaximumTreeLengthParameter => (IFixedValueParameter<IntValue>)Parameters[MaximumTreeLengthParameterName];
  public IFixedValueParameter<DoubleLimit> EstimationLimitsParameter => (IFixedValueParameter<DoubleLimit>)Parameters[EstimationLimitsParameterName];
  
  public DynamicRegressionProblemData ProblemData { get { return ProblemDataParameter.Value; } set { ProblemDataParameter.Value = value; } }
  public ISymbolicRegressionSingleObjectiveEvaluator ModelEvaluator { get { return ModelEvaluatorParameter.Value; } set { ModelEvaluatorParameter.Value = value; } }
  public ISymbolicDataAnalysisGrammar Grammar { get { return GrammarParameter.Value; } set { GrammarParameter.Value = value; } }
  public ISymbolicDataAnalysisExpressionTreeInterpreter Interpreter { get { return InterpreterParameter.Value; } set { InterpreterParameter.Value = value; } }
  public bool ApplyLinearScaling { get { return ApplyLinearScalingParameter.Value.Value; } set { ApplyLinearScalingParameter.Value.Value = value; } }
  public bool OptimizeParameters { get { return OptimizeParametersParameter.Value.Value; } set { OptimizeParametersParameter.Value.Value = value; } }
  public int MaximumTreeDepth { get { return MaximumTreeDepthParameter.Value.Value; } set { MaximumTreeDepthParameter.Value.Value = value; } }
  public int MaximumTreeLength { get { return MaximumTreeLengthParameter.Value.Value; } set { MaximumTreeLengthParameter.Value.Value = value; } }
  public DoubleLimit EstimationLimits { get { return EstimationLimitsParameter.Value; } }
  #endregion
  
  
  #region Constructors and Cloning
  public DynamicSymbolicRegressionProblem() {
    Parameters.Add(new ValueParameter<DynamicRegressionProblemData>(ProblemDataParameterName));
    Parameters.Add(new ValueParameter<ISymbolicRegressionSingleObjectiveEvaluator>(ModelEvaluatorParameterName, new SymbolicRegressionSingleObjectivePearsonRSquaredEvaluator()));
    Parameters.Add(new ValueParameter<ISymbolicDataAnalysisGrammar>(GrammarParameterName, new TypeCoherentExpressionGrammar()));
    Parameters.Add(new ValueParameter<ISymbolicDataAnalysisExpressionTreeInterpreter>(InterpreterParameterName, new SymbolicDataAnalysisExpressionTreeLinearInterpreter()) { Hidden = true });
    Parameters.Add(new FixedValueParameter<BoolValue>(ApplyLinearScalingParameterName, new BoolValue(true)));
    Parameters.Add(new FixedValueParameter<BoolValue>(OptimizeParametersParameterName, new BoolValue(false)));
    Parameters.Add(new FixedValueParameter<IntValue>(MaximumTreeDepthParameterName, new IntValue(8)));
    Parameters.Add(new FixedValueParameter<IntValue>(MaximumTreeLengthParameterName, new IntValue(25)));
    Parameters.Add(new FixedValueParameter<DoubleLimit>(EstimationLimitsParameterName, new DoubleLimit(double.MinValue, double.MaxValue)));
    
    RegisterProblemEventHandlers();
    
    Load(new DynamicRegressionProblemData());
  }

  private void RegisterProblemEventHandlers() {
    ProblemDataParameter.ValueChanged += (a, b) => UpdateInitialState();
    ModelEvaluatorParameter.ValueChanged += (a, b) => UpdateInitialState();
    GrammarParameter.ValueChanged += (a, b) => UpdateInitialState();
    InterpreterParameter.ValueChanged += (a, b) => UpdateInitialState();
    ApplyLinearScalingParameter.Value.ValueChanged += (a, b) => UpdateInitialState();
    SolutionCreatorParameter.ValueChanged += (a, b) => UpdateInitialState();
    OptimizeParametersParameter.Value.ValueChanged += (a, b) => UpdateInitialState();
    MaximumTreeDepthParameter.Value.ValueChanged += (a, b) => UpdateInitialState();
    MaximumTreeLengthParameter.Value.ValueChanged += (a, b) => UpdateInitialState();
  }

  [StorableConstructor]
  protected DynamicSymbolicRegressionProblem(StorableConstructorFlag _) : base(_) { }

    
  [StorableHook(HookType.AfterDeserialization)]
  private void AfterDeserialization() {
    RegisterProblemEventHandlers();
  }

  protected DynamicSymbolicRegressionProblem(DynamicSymbolicRegressionProblem original, Cloner cloner) : base(original, cloner) {
    RegisterEventHandlers();
  }

  public override IDeepCloneable Clone(Cloner cloner) {
    return new DynamicSymbolicRegressionProblem(this, cloner);
  }
  #endregion

  #region ProblemMethods
  protected override double Evaluate(Individual individual, IRandom random, bool dummy) {
    return State.Evaluate(individual, random);
  }

  protected override void Analyze(Individual[] individuals, double[] qualities, ResultCollection results, IRandom random, bool dummy) {
    base.Analyze(individuals, qualities, results, random, dummy);
    State.Analyze(individuals, qualities, results, random);
  }

  #endregion

  public void Load(DynamicRegressionProblemData data) {
    ProblemData = data;
  }

  private void UpdateInitialState() {
    Grammar.ConfigureVariableSymbols(ProblemData);
    UpdateEstimationLimits();
    
    EventHandler updateCreatorHandler = (_, _) => SolutionCreator = InitialState.SolutionCreator;
    if (InitialState is not null) InitialState.SolutionCreatorChanged -= updateCreatorHandler;
    InitialState = new DynamicSymbolicRegressionProblemState(this);
    InitialState.SolutionCreatorChanged += updateCreatorHandler;
    Encoding = InitialState.Encoding;
    SolutionCreator = InitialState.SolutionCreator;
  }
  
  private void UpdateEstimationLimits() {
    const double PunishmentFactor = 10;
    if (ProblemData.TrainingIndices.Any()) {
      var targetValues = ProblemData.Dataset.GetDoubleValues(ProblemData.TargetVariable, ProblemData.TrainingIndices).ToList();
      var mean = targetValues.Average();
      var range = targetValues.Max() - targetValues.Min();
      EstimationLimits.Upper = mean + PunishmentFactor * range;
      EstimationLimits.Lower = mean - PunishmentFactor * range;
    } else {
      EstimationLimits.Upper = double.MaxValue;
      EstimationLimits.Lower = double.MinValue;
    }
  }
}
