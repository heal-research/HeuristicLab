#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.RealVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Problems.Instances;

namespace HeuristicLab.Problems.TestFunctions {
  [Item("Single Objective Test Function", "Test function with real valued inputs and a single objective.")]
  [StorableClass]
  [Creatable("Problems")]
  public sealed class SingleObjectiveTestFunctionProblem : SingleObjectiveHeuristicOptimizationProblem<ISingleObjectiveTestFunctionProblemEvaluator, IRealVectorCreator>, IStorableContent, IProblemInstanceConsumer<SOTFData> {
    public string Filename { get; set; }

    [Storable]
    private StdDevStrategyVectorCreator strategyVectorCreator;
    [Storable]
    private StdDevStrategyVectorCrossover strategyVectorCrossover;
    [Storable]
    private StdDevStrategyVectorManipulator strategyVectorManipulator;

    #region Parameter Properties
    public ValueParameter<DoubleMatrix> BoundsParameter {
      get { return (ValueParameter<DoubleMatrix>)Parameters["Bounds"]; }
    }
    public ValueParameter<IntValue> ProblemSizeParameter {
      get { return (ValueParameter<IntValue>)Parameters["ProblemSize"]; }
    }
    public OptionalValueParameter<RealVector> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<RealVector>)Parameters["BestKnownSolution"]; }
    }
    #endregion

    #region Properties
    public DoubleMatrix Bounds {
      get { return BoundsParameter.Value; }
      set { BoundsParameter.Value = value; }
    }
    public IntValue ProblemSize {
      get { return ProblemSizeParameter.Value; }
      set { ProblemSizeParameter.Value = value; }
    }
    private BestSingleObjectiveTestFunctionSolutionAnalyzer BestSingleObjectiveTestFunctionSolutionAnalyzer {
      get { return Operators.OfType<BestSingleObjectiveTestFunctionSolutionAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    [Obsolete]
    [Storable(Name = "operators")]
    private IEnumerable<IOperator> oldOperators {
      get { return null; }
      set {
        if (value != null && value.Any())
          Operators.AddRange(value);
      }
    }
    #endregion

    [StorableConstructor]
    private SingleObjectiveTestFunctionProblem(bool deserializing) : base(deserializing) { }
    private SingleObjectiveTestFunctionProblem(SingleObjectiveTestFunctionProblem original, Cloner cloner)
      : base(original, cloner) {
      strategyVectorCreator = cloner.Clone(original.strategyVectorCreator);
      strategyVectorCrossover = cloner.Clone(original.strategyVectorCrossover);
      strategyVectorManipulator = cloner.Clone(original.strategyVectorManipulator);
      RegisterEventHandlers();
    }
    public SingleObjectiveTestFunctionProblem()
      : base(new AckleyEvaluator(), new UniformRandomRealVectorCreator()) {
      Parameters.Add(new ValueParameter<DoubleMatrix>("Bounds", "The lower and upper bounds in each dimension.", Evaluator.Bounds));
      Parameters.Add(new ValueParameter<IntValue>("ProblemSize", "The dimension of the problem.", new IntValue(2)));
      Parameters.Add(new OptionalValueParameter<RealVector>("BestKnownSolution", "The best known solution for this test function instance."));

      Maximization.Value = Evaluator.Maximization;
      BestKnownQuality = new DoubleValue(Evaluator.BestKnownQuality);

      strategyVectorCreator = new StdDevStrategyVectorCreator();
      strategyVectorCreator.LengthParameter.ActualName = ProblemSizeParameter.Name;
      strategyVectorCrossover = new StdDevStrategyVectorCrossover();
      strategyVectorManipulator = new StdDevStrategyVectorManipulator();
      strategyVectorManipulator.LearningRateParameter.Value = new DoubleValue(0.5);
      strategyVectorManipulator.GeneralLearningRateParameter.Value = new DoubleValue(0.5);

      SolutionCreator.RealVectorParameter.ActualName = "Point";
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      InitializeOperators();
      RegisterEventHandlers();
      UpdateStrategyVectorBounds();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SingleObjectiveTestFunctionProblem(this, cloner);
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      ParameterizeSolutionCreator();
      ParameterizeAnalyzers();
      SolutionCreator.RealVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_RealVectorParameter_ActualNameChanged);
      SolutionCreator_RealVectorParameter_ActualNameChanged(null, EventArgs.Empty);
    }
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      bool problemSizeChange = ProblemSize.Value < Evaluator.MinimumProblemSize
        || ProblemSize.Value > Evaluator.MaximumProblemSize;
      if (problemSizeChange) {
        ProblemSize.Value = Math.Max(Evaluator.MinimumProblemSize, Math.Min(ProblemSize.Value, Evaluator.MaximumProblemSize));
      } else {
        ParameterizeEvaluator();
      }
      UpdateMoveEvaluators();
      ParameterizeAnalyzers();
      Maximization.Value = Evaluator.Maximization;
      BoundsParameter.Value = Evaluator.Bounds;
      BestKnownQuality = new DoubleValue(Evaluator.BestKnownQuality);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      Evaluator_QualityParameter_ActualNameChanged(null, EventArgs.Empty);
      OnReset();
    }
    private void ProblemSizeParameter_ValueChanged(object sender, EventArgs e) {
      ProblemSize.ValueChanged += new EventHandler(ProblemSize_ValueChanged);
      ProblemSize_ValueChanged(null, EventArgs.Empty);
    }
    private void ProblemSize_ValueChanged(object sender, EventArgs e) {
      if (ProblemSize.Value < 1) ProblemSize.Value = 1;
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      strategyVectorManipulator.GeneralLearningRateParameter.Value = new DoubleValue(1.0 / Math.Sqrt(2 * ProblemSize.Value));
      strategyVectorManipulator.LearningRateParameter.Value = new DoubleValue(1.0 / Math.Sqrt(2 * Math.Sqrt(ProblemSize.Value)));
      OnReset();
    }
    private void SolutionCreator_RealVectorParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeOperators();
      ParameterizeAnalyzers();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeOperators();
    }
    private void BoundsParameter_ValueChanged(object sender, EventArgs e) {
      Bounds.ToStringChanged += new EventHandler(Bounds_ToStringChanged);
      Bounds_ToStringChanged(null, EventArgs.Empty);
    }
    private void Bounds_ToStringChanged(object sender, EventArgs e) {
      if (Bounds.Columns != 2 || Bounds.Rows < 1)
        Bounds = new DoubleMatrix(1, 2);
      ParameterizeOperators();
      UpdateStrategyVectorBounds();
    }
    private void Bounds_ItemChanged(object sender, EventArgs<int, int> e) {
      if (e.Value2 == 0 && Bounds[e.Value, 1] <= Bounds[e.Value, 0])
        Bounds[e.Value, 1] = Bounds[e.Value, 0] + 0.1;
      if (e.Value2 == 1 && Bounds[e.Value, 0] >= Bounds[e.Value, 1])
        Bounds[e.Value, 0] = Bounds[e.Value, 1] - 0.1;
      ParameterizeOperators();
      UpdateStrategyVectorBounds();
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
      strategyVectorManipulator.BoundsParameter.Value = (DoubleMatrix)strategyVectorCreator.BoundsParameter.Value.Clone();
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
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (Operators.Count == 0) InitializeOperators();
      #endregion
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      ProblemSizeParameter.ValueChanged += new EventHandler(ProblemSizeParameter_ValueChanged);
      ProblemSize.ValueChanged += new EventHandler(ProblemSize_ValueChanged);
      BoundsParameter.ValueChanged += new EventHandler(BoundsParameter_ValueChanged);
      Bounds.ToStringChanged += new EventHandler(Bounds_ToStringChanged);
      Bounds.ItemChanged += new EventHandler<EventArgs<int, int>>(Bounds_ItemChanged);
      SolutionCreator.RealVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_RealVectorParameter_ActualNameChanged);
      Evaluator.QualityParameter.ActualNameChanged += new EventHandler(Evaluator_QualityParameter_ActualNameChanged);
      strategyVectorCreator.BoundsParameter.ValueChanged += new EventHandler(strategyVectorCreator_BoundsParameter_ValueChanged);
      strategyVectorCreator.StrategyParameterParameter.ActualNameChanged += new EventHandler(strategyVectorCreator_StrategyParameterParameter_ActualNameChanged);
    }
    private void ParameterizeAnalyzers() {
      if (BestSingleObjectiveTestFunctionSolutionAnalyzer != null) {
        BestSingleObjectiveTestFunctionSolutionAnalyzer.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        BestSingleObjectiveTestFunctionSolutionAnalyzer.ResultsParameter.ActualName = "Results";
        BestSingleObjectiveTestFunctionSolutionAnalyzer.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        BestSingleObjectiveTestFunctionSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
        BestSingleObjectiveTestFunctionSolutionAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
        BestSingleObjectiveTestFunctionSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
        BestSingleObjectiveTestFunctionSolutionAnalyzer.EvaluatorParameter.ActualName = EvaluatorParameter.Name;
        BestSingleObjectiveTestFunctionSolutionAnalyzer.BoundsParameter.ActualName = BoundsParameter.Name;
      }
    }
    private void InitializeOperators() {
      Operators.Add(new SingleObjectiveTestFunctionImprovementOperator());
      Operators.Add(new SingleObjectiveTestFunctionPathRelinker());
      Operators.Add(new SingleObjectiveTestFunctionSimilarityCalculator());
      Operators.Add(new QualitySimilarityCalculator { QualityVariableName = SolutionCreator.RealVectorParameter.ActualName });
      Operators.Add(new NoSimilarityCalculator());

      Operators.Add(new BestSingleObjectiveTestFunctionSolutionAnalyzer());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));
      ParameterizeAnalyzers();
      Operators.AddRange(ApplicationManager.Manager.GetInstances<IRealVectorOperator>().Cast<IOperator>());
      Operators.Add(strategyVectorCreator);
      Operators.Add(strategyVectorCrossover);
      Operators.Add(strategyVectorManipulator);
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
        Operators.Remove(op);
      foreach (ISingleObjectiveTestFunctionMoveEvaluator op in ApplicationManager.Manager.GetInstances<ISingleObjectiveTestFunctionMoveEvaluator>())
        if (op.EvaluatorType == Evaluator.GetType()) {
          Operators.Add(op);
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
      SolutionCreator.LengthParameter.Hidden = true;
      SolutionCreator.BoundsParameter.ActualName = BoundsParameter.Name;
      SolutionCreator.BoundsParameter.Hidden = true;
    }
    private void ParameterizeEvaluator() {
      Evaluator.PointParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
      Evaluator.PointParameter.Hidden = true;
      try {
        BestKnownSolutionParameter.Value = Evaluator.GetBestKnownSolution(ProblemSize.Value);
      }
      catch (ArgumentException e) {
        ErrorHandling.ShowErrorDialog(e);
        ProblemSize.Value = Evaluator.MinimumProblemSize;
      }
    }
    private void ParameterizeOperators() {
      foreach (var op in Operators.OfType<IRealVectorCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.ParentsParameter.Hidden = true;
        op.ChildParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.ChildParameter.Hidden = true;
        op.BoundsParameter.ActualName = BoundsParameter.Name;
        op.BoundsParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IRealVectorManipulator>()) {
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.RealVectorParameter.Hidden = true;
        op.BoundsParameter.ActualName = BoundsParameter.Name;
        op.BoundsParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IRealVectorMoveOperator>()) {
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.RealVectorParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IRealVectorMoveGenerator>()) {
        op.BoundsParameter.ActualName = BoundsParameter.Name;
        op.BoundsParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<ISingleObjectiveTestFunctionAdditiveMoveEvaluator>()) {
        op.QualityParameter.ActualName = Evaluator.QualityParameter.ActualName;
        op.QualityParameter.Hidden = true;
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.RealVectorParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IRealVectorParticleCreator>()) {
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.RealVectorParameter.Hidden = true;
        op.BoundsParameter.ActualName = BoundsParameter.Name;
        op.BoundsParameter.Hidden = true;
        op.ProblemSizeParameter.ActualName = ProblemSizeParameter.Name;
        op.ProblemSizeParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IRealVectorParticleUpdater>()) {
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.RealVectorParameter.Hidden = true;
        op.BoundsParameter.ActualName = BoundsParameter.Name;
        op.BoundsParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IRealVectorSwarmUpdater>()) {
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.RealVectorParameter.Hidden = true;
        op.MaximizationParameter.ActualName = MaximizationParameter.Name;
        op.MaximizationParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IRealVectorMultiNeighborhoodShakingOperator>()) {
        op.RealVectorParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.RealVectorParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<ISingleObjectiveImprovementOperator>()) {
        op.SolutionParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.SolutionParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<ISingleObjectivePathRelinker>()) {
        op.ParentsParameter.ActualName = SolutionCreator.RealVectorParameter.ActualName;
        op.ParentsParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<SingleObjectiveTestFunctionSimilarityCalculator>()) {
        op.SolutionVariableName = SolutionCreator.RealVectorParameter.ActualName;
        op.QualityVariableName = Evaluator.QualityParameter.ActualName;
        op.Bounds = Bounds;
      }
    }
    private void UpdateStrategyVectorBounds() {
      var strategyBounds = (DoubleMatrix)Bounds.Clone();
      for (int i = 0; i < strategyBounds.Rows; i++) {
        if (strategyBounds[i, 0] < 0) strategyBounds[i, 0] = 0;
        strategyBounds[i, 1] = 0.1 * (Bounds[i, 1] - Bounds[i, 0]);
      }
      strategyVectorCreator.BoundsParameter.Value = strategyBounds;
    }
    #endregion

    public void Load(SOTFData data) {
      Name = data.Name;
      Description = data.Description;
      Evaluator = data.Evaluator;
    }
  }
}
