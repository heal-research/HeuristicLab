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
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.DataAnalysis.SupportVectorMachine.ParameterAdjustmentProblem {
  [Item("Support Vector Machine Parameter Adjustment Problem", "Represents the problem of finding good parameter settings for support vector machines.")]
  [StorableClass]
  [Creatable("Problems")]
  public sealed class SupportVectorMachineParameterAdjustmentProblem : DataAnalysisProblem, ISingleObjectiveProblem {

    #region Parameter Properties
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
    public ValueParameter<DoubleMatrix> BoundsParameter {
      get { return (ValueParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }
    public ValueParameter<IntValue> ProblemSizeParameter {
      get { return (ValueParameter<IntValue>)Parameters["ProblemSize"]; }
    }
    public new ValueParameter<IRealVectorCreator> SolutionCreatorParameter {
      get { return (ValueParameter<IRealVectorCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public new ValueParameter<SupportVectorMachineParameterAdjustmentEvaluator> EvaluatorParameter {
      get { return (ValueParameter<SupportVectorMachineParameterAdjustmentEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
    }
    public OptionalValueParameter<RealVector> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<RealVector>)Parameters["BestKnownSolution"]; }
    }
    #endregion

    #region Properties
    public BoolValue Maximization {
      get { return MaximizationParameter.Value; }
      set { MaximizationParameter.Value = value; }
    }
    public DoubleMatrix Bounds {
      get { return BoundsParameter.Value; }
    }
    public IntValue ProblemSize {
      get { return ProblemSizeParameter.Value; }
      set { ProblemSizeParameter.Value = value; }
    }
    public new IRealVectorCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public new SupportVectorMachineParameterAdjustmentEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
      set { BestKnownQualityParameter.Value = value; }
    }
    public override IEnumerable<IOperator> Operators {
      get { return operators; }
    }
    #endregion

    public IntValue TrainingSamplesStart {
      get { return new IntValue(DataAnalysisProblemData.TrainingSamplesStart.Value); }
    }
    public IntValue TrainingSamplesEnd {
      get { return new IntValue(DataAnalysisProblemData.TrainingSamplesEnd.Value); }
    }

    [Storable]
    private List<IOperator> operators;
    [Storable]
    private StdDevStrategyVectorCreator strategyVectorCreator;
    [Storable]
    private StdDevStrategyVectorCrossover strategyVectorCrossover;
    [Storable]
    private StdDevStrategyVectorManipulator strategyVectorManipulator;

    [StorableConstructor]
    private SupportVectorMachineParameterAdjustmentProblem(bool deserializing) : base(deserializing) { }
    public SupportVectorMachineParameterAdjustmentProblem()
      : base() {
      UniformRandomRealVectorCreator creator = new UniformRandomRealVectorCreator();
      SupportVectorMachineParameterAdjustmentEvaluator evaluator = new SupportVectorMachineParameterAdjustmentEvaluator();

      var bounds = new DoubleMatrix(new double[,] { 
        { 0.01, 1.0 },
        { -7, 9},
        { -7, 9}
      });

      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as we want to minimize the error.", new BoolValue(false)));
      Parameters.Add(new ValueParameter<DoubleMatrix>("Bounds", "The lower and upper bounds in each dimension.", bounds));
      Parameters.Add(new ValueParameter<IntValue>("ProblemSize", "The dimension of the problem.", new IntValue(3)));
      Parameters.Add(new ValueParameter<IRealVectorCreator>("SolutionCreator", "The operator which should be used to create new test function solutions.", creator));
      Parameters.Add(new ValueParameter<SupportVectorMachineParameterAdjustmentEvaluator>("Evaluator", "The operator which should be used to evaluate test function solutions.", evaluator));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this test function.", new DoubleValue(0)));
      Parameters.Add(new OptionalValueParameter<RealVector>("BestKnownSolution", "The best known solution for this test function instance."));
      Parameters.Add(new OptionalValueParameter<PercentValue>("ActualSamples", "The percentage of samples to use for cross validation."));

      strategyVectorCreator = new StdDevStrategyVectorCreator();
      strategyVectorCreator.LengthParameter.Value = ProblemSize;
      strategyVectorCrossover = new StdDevStrategyVectorCrossover();
      strategyVectorManipulator = new StdDevStrategyVectorManipulator();
      strategyVectorManipulator.LearningRateParameter.Value = new DoubleValue(0.5);
      strategyVectorManipulator.GeneralLearningRateParameter.Value = new DoubleValue(0.5);

      creator.RealVectorParameter.ActualName = "ParameterVector";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      InitializeOperators();
      AttachEventHandlers();
      UpdateStrategyVectorBounds();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SupportVectorMachineParameterAdjustmentProblem clone = (SupportVectorMachineParameterAdjustmentProblem)base.Clone(cloner);
      clone.operators = operators.Where(x => IsNotFieldReferenced(x)).Select(x => (IOperator)cloner.Clone(x)).ToList();
      clone.strategyVectorCreator = (StdDevStrategyVectorCreator)cloner.Clone(strategyVectorCreator);
      clone.operators.Add(clone.strategyVectorCreator);
      clone.strategyVectorCrossover = (StdDevStrategyVectorCrossover)cloner.Clone(strategyVectorCrossover);
      clone.operators.Add(strategyVectorCrossover);
      clone.strategyVectorManipulator = (StdDevStrategyVectorManipulator)cloner.Clone(strategyVectorManipulator);
      clone.operators.Add(strategyVectorManipulator);
      clone.AttachEventHandlers();
      return clone;
    }

    private bool IsNotFieldReferenced(IOperator x) {
      return !(x == strategyVectorCreator
        || x == strategyVectorCrossover
        || x == strategyVectorManipulator);
    }

    protected override void OnDataAnalysisProblemChanged(EventArgs e) {
      ParameterizeEvaluator();
      base.OnDataAnalysisProblemChanged(e);
    }

    #region Events
    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
      ParameterizeAnalyzers();
      SolutionCreator_RealVectorParameter_ActualNameChanged(null, EventArgs.Empty);
    }
    private void SolutionCreator_RealVectorParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeOperators();
      ParameterizeAnalyzers();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzers();
      RaiseReset(EventArgs.Empty);
    }
    private void strategyVectorCreator_StrategyParameterParameter_ActualNameChanged(object sender, EventArgs e) {
      string name = strategyVectorCreator.StrategyParameterParameter.ActualName;
      strategyVectorCrossover.ParentsParameter.ActualName = name;
      strategyVectorCrossover.StrategyParameterParameter.ActualName = name;
      strategyVectorManipulator.StrategyParameterParameter.ActualName = name;
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (operators == null) InitializeOperators();
      #endregion
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      SolutionCreator.RealVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_RealVectorParameter_ActualNameChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      strategyVectorCreator.StrategyParameterParameter.ActualNameChanged += new EventHandler(strategyVectorCreator_StrategyParameterParameter_ActualNameChanged);
    }
    private void InitializeOperators() {
      operators = new List<IOperator>();
      operators.AddRange(ApplicationManager.Manager.GetInstances<IRealVectorOperator>().Cast<IOperator>());
      operators.RemoveAll(x => x is IMoveOperator);
      operators.Add(new SupportVectorMachineParameterAdjustmentBestSolutionAnalyzer());
      operators.Add(strategyVectorCreator);
      operators.Add(strategyVectorCrossover);
      operators.Add(strategyVectorManipulator);
      ParameterizeOperators();
    }
    private void ParameterizeSolutionCreator() {
      SolutionCreator.LengthParameter.Value = new IntValue(ProblemSize.Value);
    }
    private void ParameterizeEvaluator() {
      Evaluator.ParameterVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
      Evaluator.SamplesStartParameter.Value = TrainingSamplesStart;
      Evaluator.SamplesEndParameter.Value = TrainingSamplesEnd;
      Evaluator.NumberOfFoldsParameter.Value = new IntValue(5);
    }
    private void ParameterizeOperators() {
      foreach (IRealVectorCrossover op in Operators.OfType<IRealVectorCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.ChildParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.BoundsParameter.ActualName = BoundsParameter.Name;
      }
      foreach (IRealVectorManipulator op in Operators.OfType<IRealVectorManipulator>()) {
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.BoundsParameter.ActualName = BoundsParameter.Name;
      }
      foreach (IRealVectorMoveOperator op in Operators.OfType<IRealVectorMoveOperator>()) {
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
      }
      foreach (IRealVectorMoveGenerator op in Operators.OfType<IRealVectorMoveGenerator>()) {
        op.BoundsParameter.ActualName = BoundsParameter.Name;
      }
      foreach (SupportVectorMachineParameterAdjustmentEvaluator op in Operators.OfType<SupportVectorMachineParameterAdjustmentEvaluator>()) {
        op.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        op.ParameterVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
      }
    }
    private void ParameterizeAnalyzers() {
      foreach (SupportVectorMachineParameterAdjustmentBestSolutionAnalyzer analyzer in Operators.OfType<SupportVectorMachineParameterAdjustmentBestSolutionAnalyzer>()) {
        analyzer.DataAnalysisProblemDataParameter.ActualName = DataAnalysisProblemDataParameter.Name;
        analyzer.ParameterVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        analyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
      }
    }
    private void UpdateStrategyVectorBounds() {
      DoubleMatrix strategyBounds = (DoubleMatrix)Bounds.Clone();
      for (int i = 0; i < strategyBounds.Rows; i++)
        if (strategyBounds[i, 0] < 0) strategyBounds[i, 0] = 0;
      strategyVectorCreator.BoundsParameter.Value = strategyBounds;
    }
    #endregion
  }
}
