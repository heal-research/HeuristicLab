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

namespace HeuristicLab.Problems.TestFunctions.SingleObjective {
  [Item("SingleObjective TestFunction", "Test function with real valued inputs and a single objective.")]
  [StorableClass]
  [Creatable("Problems")]
  public sealed class SingleObjectiveTestFunction : ParameterizedNamedItem, ISingleObjectiveProblem {
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
    public ValueParameter<ISingleObjectiveTestFunctionEvaluator> EvaluatorParameter {
      get { return (ValueParameter<ISingleObjectiveTestFunctionEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public OptionalValueParameter<ISingleObjectiveTestFunctionSolutionsVisualizer> VisualizerParameter {
      get { return (OptionalValueParameter<ISingleObjectiveTestFunctionSolutionsVisualizer>)Parameters["Visualizer"]; }
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
    public ISingleObjectiveTestFunctionEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public ISingleObjectiveTestFunctionSolutionsVisualizer Visualizer {
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
    private List<IRealVectorOperator> operators;
    public IEnumerable<IOperator> Operators {
      get { return operators.Cast<IOperator>(); }
    }
    #endregion

    [StorableConstructor]
    private SingleObjectiveTestFunction(bool deserializing) : base() { }
    public SingleObjectiveTestFunction()
      : base() {
      UniformRandomRealVectorCreator creator = new UniformRandomRealVectorCreator();
      AckleyEvaluator evaluator = new AckleyEvaluator();

      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to false as most test functions are minimization problems.", new BoolValue(evaluator.Maximization)));
      Parameters.Add(new ValueParameter<DoubleMatrix>("Bounds", "The lower and upper bounds in each dimension.", evaluator.Bounds));
      Parameters.Add(new ValueParameter<IntValue>("ProblemSize", "The dimension of the problem.", new IntValue(2)));
      Parameters.Add(new ValueParameter<IRealVectorCreator>("SolutionCreator", "The operator which should be used to create new TSP solutions.", creator));
      Parameters.Add(new ValueParameter<ISingleObjectiveTestFunctionEvaluator>("Evaluator", "The operator which should be used to evaluate TSP solutions.", evaluator));
      Parameters.Add(new OptionalValueParameter<ISingleObjectiveTestFunctionSolutionsVisualizer>("Visualizer", "The operator which should be used to visualize TSP solutions."));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this TSP instance.", new DoubleValue(evaluator.BestKnownQuality)));

      creator.RealVectorParameter.ActualName = "Point";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeVisualizer();

      Initialize();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      SingleObjectiveTestFunction clone = (SingleObjectiveTestFunction)base.Clone(cloner);
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
      ParameterizeSolutionCreator();
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
    }

    private void InitializeOperators() {
      operators = new List<IRealVectorOperator>();
      if (ApplicationManager.Manager != null) {
        operators.AddRange(ApplicationManager.Manager.GetInstances<IRealVectorOperator>());
        ParameterizeOperators();
      }
      //InitializeMoveGenerators();
    }
    /*private void InitializeMoveGenerators() {
      foreach (ITwoOptPermutationMoveOperator op in Operators.OfType<ITwoOptPermutationMoveOperator>()) {
        if (op is IMoveGenerator) {
          op.TwoOptMoveParameter.ActualNameChanged += new EventHandler(MoveGenerator_TwoOptMoveParameter_ActualNameChanged);
        }
      }
      foreach (IThreeOptPermutationMoveOperator op in Operators.OfType<IThreeOptPermutationMoveOperator>()) {
        if (op is IMoveGenerator) {
          op.ThreeOptMoveParameter.ActualNameChanged += new EventHandler(MoveGenerator_ThreeOptMoveParameter_ActualNameChanged);
        }
      }
    }*/
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
      }
      foreach (IRealVectorManipulator op in Operators.OfType<IRealVectorManipulator>()) {
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
      }
      /*foreach (IPermutationMoveOperator op in Operators.OfType<IPermutationMoveOperator>()) {
        op.PermutationParameter.ActualName = SolutionCreator.PermutationParameter.ActualName;
      }
      foreach (ITSPPathMoveEvaluator op in Operators.OfType<ITSPPathMoveEvaluator>()) {
        op.CoordinatesParameter.ActualName = CoordinatesParameter.Name;
        op.DistanceMatrixParameter.ActualName = DistanceMatrixParameter.Name;
        op.UseDistanceMatrixParameter.ActualName = UseDistanceMatrixParameter.Name;
      }*/
    }
    #endregion
  }
}
