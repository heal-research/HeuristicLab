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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.TestFunctions {
  [Item("Single Objective Test Function", "Test function with real valued inputs and a single objective.")]
  [StorableClass]
  [Creatable("Problems")]
  public sealed class SingleObjectiveTestFunctionProblem : ParameterizedNamedItem, ISingleObjectiveProblem {
    [Storable]
    private StrategyVectorCreator strategyVectorCreator;
    [Storable]
    private StrategyVectorCrossover strategyVectorCrossover;
    [Storable]
    private StrategyVectorManipulator strategyVectorManipulator;

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Type; }
    }

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
    public ValueParameter<IRealVectorCreator> SolutionCreatorParameter {
      get { return (ValueParameter<IRealVectorCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public ValueParameter<ISingleObjectiveTestFunctionProblemEvaluator> EvaluatorParameter {
      get { return (ValueParameter<ISingleObjectiveTestFunctionProblemEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public OptionalValueParameter<ISingleObjectiveTestFunctionProblemSolutionsVisualizer> VisualizerParameter {
      get { return (OptionalValueParameter<ISingleObjectiveTestFunctionProblemSolutionsVisualizer>)Parameters["Visualizer"]; }
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
    public BoolValue Maximization {
      get { return MaximizationParameter.Value; }
      set { MaximizationParameter.Value = value; }
    }
    public DoubleMatrix Bounds {
      get { return BoundsParameter.Value; }
      set { BoundsParameter.Value = value; }
    }
    public IntValue ProblemSize {
      get { return ProblemSizeParameter.Value; }
      set { ProblemSizeParameter.Value = value; }
    }
    public IRealVectorCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public ISingleObjectiveTestFunctionProblemEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public ISingleObjectiveTestFunctionProblemSolutionsVisualizer Visualizer {
      get { return VisualizerParameter.Value; }
      set { VisualizerParameter.Value = value; }
    }
    ISolutionsVisualizer IProblem.Visualizer {
      get { return VisualizerParameter.Value; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
      set { BestKnownQualityParameter.Value = value; }
    }
    private List<IOperator> operators;
    public IEnumerable<IOperator> Operators {
      get { return operators; }
    }
    #endregion

    [StorableConstructor]
    private SingleObjectiveTestFunctionProblem(bool deserializing) : base() { }
    public SingleObjectiveTestFunctionProblem()
      : base() {
      UniformRandomRealVectorCreator creator = new UniformRandomRealVectorCreator();
      AckleyEvaluator evaluator = new AckleyEvaluator();

      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as most test functions are minimization problems.", new BoolValue(evaluator.Maximization)));
      Parameters.Add(new ValueParameter<DoubleMatrix>("Bounds", "The lower and upper bounds in each dimension.", evaluator.Bounds));
      Parameters.Add(new ValueParameter<IntValue>("ProblemSize", "The dimension of the problem.", new IntValue(2)));
      Parameters.Add(new ValueParameter<IRealVectorCreator>("SolutionCreator", "The operator which should be used to create new TSP solutions.", creator));
      Parameters.Add(new ValueParameter<ISingleObjectiveTestFunctionProblemEvaluator>("Evaluator", "The operator which should be used to evaluate TSP solutions.", evaluator));
      Parameters.Add(new OptionalValueParameter<ISingleObjectiveTestFunctionProblemSolutionsVisualizer>("Visualizer", "The operator which should be used to visualize TSP solutions."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this TSP instance.", new DoubleValue(evaluator.BestKnownQuality)));

      strategyVectorCreator = new StrategyVectorCreator();
      strategyVectorCreator.LengthParameter.ActualName = ProblemSizeParameter.Name;
      strategyVectorCrossover = new StrategyVectorCrossover();
      strategyVectorManipulator = new StrategyVectorManipulator();

      creator.RealVectorParameter.ActualName = "Point";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeVisualizer();

      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SingleObjectiveTestFunctionProblem clone = (SingleObjectiveTestFunctionProblem)base.Clone(cloner);
      clone.Initialize();
      return clone;
    }

    #region Events
    public event EventHandler SolutionCreatorChanged;
    private void OnSolutionCreatorChanged() {
      if (SolutionCreatorChanged != null)
        SolutionCreatorChanged(this, EventArgs.Empty);
    }
    public event EventHandler EvaluatorChanged;
    private void OnEvaluatorChanged() {
      if (EvaluatorChanged != null)
        EvaluatorChanged(this, EventArgs.Empty);
    }
    public event EventHandler VisualizerChanged;
    private void OnVisualizerChanged() {
      if (VisualizerChanged != null)
        VisualizerChanged(this, EventArgs.Empty);
    }
    public event EventHandler OperatorsChanged;
    private void OnOperatorsChanged() {
      if (OperatorsChanged != null)
        OperatorsChanged(this, EventArgs.Empty);
    }
    private void ProblemSizeParameter_ValueChanged(object sender, EventArgs e) {
      ProblemSize.ValueChanged += new EventHandler(ProblemSize_ValueChanged);
      ProblemSize_ValueChanged(null, EventArgs.Empty);
    }
    private void ProblemSize_ValueChanged(object sender, EventArgs e) {
      if (ProblemSize.Value < 1) ProblemSize.Value = 1;
      ParameterizeSolutionCreator();
      strategyVectorManipulator.GeneralLearningRateParameter.Value = new DoubleValue(1.0 / Math.Sqrt(2 * ProblemSize.Value));
      strategyVectorManipulator.LearningRateParameter.Value = new DoubleValue(1.0 / Math.Sqrt(2 * Math.Sqrt(ProblemSize.Value)));
    }
    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeSolutionCreator();
      SolutionCreator_RealVectorParameter_ActualNameChanged(null, EventArgs.Empty);
    }
    private void SolutionCreator_RealVectorParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeOperators();
      ParameterizeVisualizer();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      UpdateMoveEvaluators();
      Maximization.Value = Evaluator.Maximization;
      BoundsParameter.Value = Evaluator.Bounds;
      if (ProblemSize.Value < Evaluator.MinimumProblemSize)
        ProblemSize.Value = Evaluator.MinimumProblemSize;
      else if (ProblemSize.Value > Evaluator.MaximumProblemSize)
        ProblemSize.Value = Evaluator.MaximumProblemSize;
      BestKnownQuality = new DoubleValue(Evaluator.BestKnownQuality);
      Evaluator_QualityParameter_ActualNameChanged(null, EventArgs.Empty);
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeOperators();
    }
    private void VisualizerParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeVisualizer();
    }
    private void BoundsParameter_ValueChanged(object sender, EventArgs e) {
      Bounds.ToStringChanged += new EventHandler(Bounds_ToStringChanged);
      Bounds_ToStringChanged(null, EventArgs.Empty);
    }
    private void Bounds_ToStringChanged(object sender, EventArgs e) {
      if (Bounds.Columns != 2 || Bounds.Rows < 1)
        Bounds = new DoubleMatrix(1, 2);
    }
    private void Bounds_ItemChanged(object sender, EventArgs<int, int> e) {
      if (e.Value2 == 0 && Bounds[e.Value, 1] <= Bounds[e.Value, 0])
        Bounds[e.Value, 1] = Bounds[e.Value, 0] + 0.1;
      if (e.Value2 == 1 && Bounds[e.Value, 0] >= Bounds[e.Value, 1])
        Bounds[e.Value, 0] = Bounds[e.Value, 1] - 0.1;
    }
    private void MoveGenerator_AdditiveMoveParameter_ActualNameChanged(object sender, EventArgs e) {
      string name = ((ILookupParameter<AdditiveMove>)sender).ActualName;
      foreach (IAdditiveRealVectorMoveOperator op in Operators.OfType<IAdditiveRealVectorMoveOperator>()) {
        op.AdditiveMoveParameter.ActualName = name;
      }
    }
    private void SphereEvaluator_Parameter_ValueChanged(object sender, EventArgs e) {
      SphereEvaluator eval = (Evaluator as SphereEvaluator);
      if (eval != null) {
        foreach (ISphereMoveEvaluator op in Operators.OfType<ISphereMoveEvaluator>()) {
          op.C = eval.C;
          op.Alpha = eval.Alpha;
        }
      }
    }
    private void RastriginEvaluator_Parameter_ValueChanged(object sender, EventArgs e) {
      RastriginEvaluator eval = (Evaluator as RastriginEvaluator);
      if (eval != null) {
        foreach (IRastriginMoveEvaluator op in Operators.OfType<IRastriginMoveEvaluator>()) {
          op.A = eval.A;
        }
      }
    }
    private void strategyVectorCreator_BoundsParameter_ValueChanged(object sender, EventArgs e) {
      strategyVectorManipulator.BoundsParameter.Value = strategyVectorCreator.BoundsParameter.Value;
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
    private void Initialize() {
      InitializeOperators();
      ProblemSizeParameter.ValueChanged += new EventHandler(ProblemSizeParameter_ValueChanged);
      ProblemSize.ValueChanged += new EventHandler(ProblemSize_ValueChanged);
      BoundsParameter.ValueChanged += new EventHandler(BoundsParameter_ValueChanged);
      Bounds.ToStringChanged += new EventHandler(Bounds_ToStringChanged);
      Bounds.ItemChanged += new EventHandler<EventArgs<int, int>>(Bounds_ItemChanged);
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      SolutionCreator.RealVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_RealVectorParameter_ActualNameChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      VisualizerParameter.ValueChanged += new EventHandler(VisualizerParameter_ValueChanged);
      strategyVectorCreator.BoundsParameter.ValueChanged += new EventHandler(strategyVectorCreator_BoundsParameter_ValueChanged);
      strategyVectorCreator.StrategyParameterParameter.ActualNameChanged += new EventHandler(strategyVectorCreator_StrategyParameterParameter_ActualNameChanged);
    }
    private void InitializeOperators() {
      operators = new List<IOperator>();
      operators.AddRange(ApplicationManager.Manager.GetInstances<IRealVectorOperator>().Cast<IOperator>());
      operators.Add(strategyVectorCreator);
      operators.Add(strategyVectorCrossover);
      operators.Add(strategyVectorManipulator);
      UpdateMoveEvaluators();
      ParameterizeOperators();
      InitializeMoveGenerators();
    }
    private void InitializeMoveGenerators() {
      foreach (IAdditiveRealVectorMoveOperator op in Operators.OfType<IAdditiveRealVectorMoveOperator>()) {
        if (op is IMoveGenerator) {
          op.AdditiveMoveParameter.ActualNameChanged += new EventHandler(MoveGenerator_AdditiveMoveParameter_ActualNameChanged);
        }
      }
    }
    private void UpdateMoveEvaluators() {
      foreach (ISingleObjectiveTestFunctionMoveEvaluator op in Operators.OfType<ISingleObjectiveTestFunctionMoveEvaluator>().ToList())
        operators.Remove(op);
      foreach (ISingleObjectiveTestFunctionMoveEvaluator op in ApplicationManager.Manager.GetInstances<ISingleObjectiveTestFunctionMoveEvaluator>())
        if (op.EvaluatorType == Evaluator.GetType()) {
          operators.Add(op);
          #region Synchronize evaluator specific parameters with the parameters of the corresponding move evaluators
          if (op is ISphereMoveEvaluator) {
            SphereEvaluator e = (Evaluator as SphereEvaluator);
            e.AlphaParameter.ValueChanged += new EventHandler(SphereEvaluator_Parameter_ValueChanged);
            e.CParameter.ValueChanged += new EventHandler(SphereEvaluator_Parameter_ValueChanged);
            ISphereMoveEvaluator em = (op as ISphereMoveEvaluator);
            em.C = e.C;
            em.Alpha = e.Alpha;
          } else if (op is IRastriginMoveEvaluator) {
            RastriginEvaluator e = (Evaluator as RastriginEvaluator);
            e.AParameter.ValueChanged += new EventHandler(RastriginEvaluator_Parameter_ValueChanged);
            IRastriginMoveEvaluator em = (op as IRastriginMoveEvaluator);
            em.A = e.A;
          }
          #endregion
        }
      ParameterizeOperators();
      OnOperatorsChanged();
    }
    private void ParameterizeSolutionCreator() {
      SolutionCreator.LengthParameter.Value = new IntValue(ProblemSize.Value);
    }
    private void ParameterizeEvaluator() {
      Evaluator.PointParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
    }
    private void ParameterizeVisualizer() {
      if (Visualizer != null) {
        Visualizer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        Visualizer.PointParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
      }
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
      foreach (ISingleObjectiveTestFunctionAdditiveMoveEvaluator op in Operators.OfType<ISingleObjectiveTestFunctionAdditiveMoveEvaluator>()) {
        op.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
      }
    }
    #endregion
  }
}
