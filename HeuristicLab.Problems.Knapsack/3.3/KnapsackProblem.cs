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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Problems.Knapsack {
  [Item("Knapsack Problem", "Represents a Knapsack problem.")]
  [Creatable("Problems")]
  [StorableClass]
  public sealed class KnapsackProblem : SingleObjectiveHeuristicOptimizationProblem<IKnapsackEvaluator, IBinaryVectorCreator>, IStorableContent {
    public string Filename { get; set; }

    #region Parameter Properties
    public ValueParameter<IntValue> KnapsackCapacityParameter {
      get { return (ValueParameter<IntValue>)Parameters["KnapsackCapacity"]; }
    }
    public ValueParameter<IntArray> WeightsParameter {
      get { return (ValueParameter<IntArray>)Parameters["Weights"]; }
    }
    public ValueParameter<IntArray> ValuesParameter {
      get { return (ValueParameter<IntArray>)Parameters["Values"]; }
    }
    public ValueParameter<DoubleValue> PenaltyParameter {
      get { return (ValueParameter<DoubleValue>)Parameters["Penalty"]; }
    }
    public OptionalValueParameter<BinaryVector> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<BinaryVector>)Parameters["BestKnownSolution"]; }
    }
    #endregion

    #region Properties
    public IntValue KnapsackCapacity {
      get { return KnapsackCapacityParameter.Value; }
      set { KnapsackCapacityParameter.Value = value; }
    }
    public IntArray Weights {
      get { return WeightsParameter.Value; }
      set { WeightsParameter.Value = value; }
    }
    public IntArray Values {
      get { return ValuesParameter.Value; }
      set { ValuesParameter.Value = value; }
    }
    public DoubleValue Penalty {
      get { return PenaltyParameter.Value; }
      set { PenaltyParameter.Value = value; }
    }
    public BinaryVector BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    private BestKnapsackSolutionAnalyzer BestKnapsackSolutionAnalyzer {
      get { return Operators.OfType<BestKnapsackSolutionAnalyzer>().FirstOrDefault(); }
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
    private KnapsackProblem(bool deserializing) : base(deserializing) { }
    private KnapsackProblem(KnapsackProblem original, Cloner cloner)
      : base(original, cloner) {
      AttachEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new KnapsackProblem(this, cloner);
    }
    public KnapsackProblem()
      : base(new KnapsackEvaluator(), new RandomBinaryVectorCreator()) {
      Parameters.Add(new ValueParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack.", new IntValue(0)));
      Parameters.Add(new ValueParameter<IntArray>("Weights", "The weights of the items.", new IntArray(5)));
      Parameters.Add(new ValueParameter<IntArray>("Values", "The values of the items.", new IntArray(5)));
      Parameters.Add(new ValueParameter<DoubleValue>("Penalty", "The penalty value for each unit of overweight.", new DoubleValue(1)));
      Parameters.Add(new OptionalValueParameter<BinaryVector>("BestKnownSolution", "The best known solution of this Knapsack instance."));

      Maximization.Value = true;
      MaximizationParameter.Hidden = true;

      SolutionCreator.BinaryVectorParameter.ActualName = "KnapsackSolution";

      InitializeRandomKnapsackInstance();

      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      InitializeOperators();
      AttachEventHandlers();
    }

    #region Events
    protected override void OnSolutionCreatorChanged() {
      base.OnSolutionCreatorChanged();
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_BinaryVectorParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
    }
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
    }
    private void SolutionCreator_BinaryVectorParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
    }
    private void KnapsackCapacityParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
    }
    private void WeightsParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeSolutionCreator();

      WeightsParameter.Value.Reset += new EventHandler(WeightsValue_Reset);
    }
    private void WeightsValue_Reset(object sender, EventArgs e) {
      ParameterizeSolutionCreator();

      if (WeightsParameter.Value != null && ValuesParameter.Value != null)
        ((IStringConvertibleArray)ValuesParameter.Value).Length = WeightsParameter.Value.Length;
    }
    private void ValuesParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeSolutionCreator();

      ValuesParameter.Value.Reset += new EventHandler(ValuesValue_Reset);
    }
    private void ValuesValue_Reset(object sender, EventArgs e) {
      ParameterizeSolutionCreator();

      if (WeightsParameter.Value != null && ValuesParameter.Value != null)
        ((IStringConvertibleArray)WeightsParameter.Value).Length = ValuesParameter.Value.Length;
    }
    private void PenaltyParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
    }
    private void OneBitflipMoveParameter_ActualNameChanged(object sender, EventArgs e) {
      string name = ((ILookupParameter<OneBitflipMove>)sender).ActualName;
      foreach (IOneBitflipMoveOperator op in Operators.OfType<IOneBitflipMoveOperator>()) {
        op.OneBitflipMoveParameter.ActualName = name;
      }
    }
    #endregion

    #region Helpers
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code (remove with 3.4)
      if (Operators.Count == 0) InitializeOperators();
      #endregion
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_BinaryVectorParameter_ActualNameChanged);
      KnapsackCapacityParameter.ValueChanged += new EventHandler(KnapsackCapacityParameter_ValueChanged);
      WeightsParameter.ValueChanged += new EventHandler(WeightsParameter_ValueChanged);
      WeightsParameter.Value.Reset += new EventHandler(WeightsValue_Reset);
      ValuesParameter.ValueChanged += new EventHandler(ValuesParameter_ValueChanged);
      ValuesParameter.Value.Reset += new EventHandler(ValuesValue_Reset);
      PenaltyParameter.ValueChanged += new EventHandler(PenaltyParameter_ValueChanged);
    }
    private void ParameterizeSolutionCreator() {
      if (SolutionCreator.LengthParameter.Value == null ||
        SolutionCreator.LengthParameter.Value.Value != WeightsParameter.Value.Length)
        SolutionCreator.LengthParameter.Value = new IntValue(WeightsParameter.Value.Length);
    }
    private void ParameterizeEvaluator() {
      if (Evaluator is KnapsackEvaluator) {
        KnapsackEvaluator knapsackEvaluator =
          (KnapsackEvaluator)Evaluator;
        knapsackEvaluator.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
        knapsackEvaluator.BinaryVectorParameter.Hidden = true;
        knapsackEvaluator.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
        knapsackEvaluator.KnapsackCapacityParameter.Hidden = true;
        knapsackEvaluator.WeightsParameter.ActualName = WeightsParameter.Name;
        knapsackEvaluator.WeightsParameter.Hidden = true;
        knapsackEvaluator.ValuesParameter.ActualName = ValuesParameter.Name;
        knapsackEvaluator.ValuesParameter.Hidden = true;
        knapsackEvaluator.PenaltyParameter.ActualName = PenaltyParameter.Name;
        knapsackEvaluator.PenaltyParameter.Hidden = true;
      }
    }
    private void ParameterizeAnalyzer() {
      BestKnapsackSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
      BestKnapsackSolutionAnalyzer.MaximizationParameter.Hidden = true;
      BestKnapsackSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
      BestKnapsackSolutionAnalyzer.BestKnownQualityParameter.Hidden = true;
      BestKnapsackSolutionAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
      BestKnapsackSolutionAnalyzer.BestKnownSolutionParameter.Hidden = true;
      BestKnapsackSolutionAnalyzer.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      BestKnapsackSolutionAnalyzer.BinaryVectorParameter.Hidden = true;
      BestKnapsackSolutionAnalyzer.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
      BestKnapsackSolutionAnalyzer.KnapsackCapacityParameter.Hidden = true;
      BestKnapsackSolutionAnalyzer.WeightsParameter.ActualName = WeightsParameter.Name;
      BestKnapsackSolutionAnalyzer.WeightsParameter.Hidden = true;
      BestKnapsackSolutionAnalyzer.ValuesParameter.ActualName = ValuesParameter.Name;
      BestKnapsackSolutionAnalyzer.ValuesParameter.Hidden = true;
    }
    private void InitializeOperators() {
      Operators.Add(new BestKnapsackSolutionAnalyzer());
      ParameterizeAnalyzer();
      foreach (IBinaryVectorOperator op in ApplicationManager.Manager.GetInstances<IBinaryVectorOperator>()) {
        if (!(op is ISingleObjectiveMoveEvaluator) || (op is IKnapsackMoveEvaluator)) {
          Operators.Add(op);
        }
      }
      ParameterizeOperators();
      InitializeMoveGenerators();
    }
    private void InitializeMoveGenerators() {
      foreach (IOneBitflipMoveOperator op in Operators.OfType<IOneBitflipMoveOperator>()) {
        if (op is IMoveGenerator) {
          op.OneBitflipMoveParameter.ActualNameChanged += new EventHandler(OneBitflipMoveParameter_ActualNameChanged);
        }
      }
    }
    private void ParameterizeOperators() {
      foreach (IBinaryVectorCrossover op in Operators.OfType<IBinaryVectorCrossover>()) {
        op.ParentsParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
        op.ParentsParameter.Hidden = true;
        op.ChildParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
        op.ChildParameter.Hidden = true;
      }
      foreach (IBinaryVectorManipulator op in Operators.OfType<IBinaryVectorManipulator>()) {
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
        op.BinaryVectorParameter.Hidden = true;
      }
      foreach (IBinaryVectorMoveOperator op in Operators.OfType<IBinaryVectorMoveOperator>()) {
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
        op.BinaryVectorParameter.Hidden = true;
      }
      foreach (IKnapsackMoveEvaluator op in Operators.OfType<IKnapsackMoveEvaluator>()) {
        op.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
        op.KnapsackCapacityParameter.Hidden = true;
        op.PenaltyParameter.ActualName = PenaltyParameter.Name;
        op.PenaltyParameter.Hidden = true;
        op.WeightsParameter.ActualName = WeightsParameter.Name;
        op.WeightsParameter.Hidden = true;
        op.ValuesParameter.ActualName = ValuesParameter.Name;
        op.ValuesParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IBinaryVectorMultiNeighborhoodShakingOperator>()) {
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
        op.BinaryVectorParameter.Hidden = true;
      }
    }
    #endregion

    private void InitializeRandomKnapsackInstance() {
      System.Random rand = new System.Random();

      int itemCount = rand.Next(10, 100);
      Weights = new IntArray(itemCount);
      Values = new IntArray(itemCount);

      double totalWeight = 0;

      for (int i = 0; i < itemCount; i++) {
        int value = rand.Next(1, 10);
        int weight = rand.Next(1, 10);

        Values[i] = value;
        Weights[i] = weight;
        totalWeight += weight;
      }

      int capacity = (int)Math.Round(0.7 * totalWeight);
      KnapsackCapacity = new IntValue(capacity);
    }
  }
}
