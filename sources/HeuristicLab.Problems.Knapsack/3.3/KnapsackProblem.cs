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
using System.Drawing;
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
  public sealed class KnapsackProblem : ParameterizedNamedItem, ISingleObjectiveHeuristicOptimizationProblem, IStorableContent {
    public string Filename { get; set; }

    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Type; }
    }

    #region Parameter Properties
    public ValueParameter<BoolValue> MaximizationParameter {
      get { return (ValueParameter<BoolValue>)Parameters["Maximization"]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.MaximizationParameter {
      get { return MaximizationParameter; }
    }
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
    public ValueParameter<IBinaryVectorCreator> SolutionCreatorParameter {
      get { return (ValueParameter<IBinaryVectorCreator>)Parameters["SolutionCreator"]; }
    }
    IParameter IHeuristicOptimizationProblem.SolutionCreatorParameter {
      get { return SolutionCreatorParameter; }
    }
    public ValueParameter<IKnapsackEvaluator> EvaluatorParameter {
      get { return (ValueParameter<IKnapsackEvaluator>)Parameters["Evaluator"]; }
    }
    IParameter IHeuristicOptimizationProblem.EvaluatorParameter {
      get { return EvaluatorParameter; }
    }
    public OptionalValueParameter<DoubleValue> BestKnownQualityParameter {
      get { return (OptionalValueParameter<DoubleValue>)Parameters["BestKnownQuality"]; }
    }
    IParameter ISingleObjectiveHeuristicOptimizationProblem.BestKnownQualityParameter {
      get { return BestKnownQualityParameter; }
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
    public IBinaryVectorCreator SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
      set { SolutionCreatorParameter.Value = value; }
    }
    ISolutionCreator IHeuristicOptimizationProblem.SolutionCreator {
      get { return SolutionCreatorParameter.Value; }
    }
    public IKnapsackEvaluator Evaluator {
      get { return EvaluatorParameter.Value; }
      set { EvaluatorParameter.Value = value; }
    }
    ISingleObjectiveEvaluator ISingleObjectiveHeuristicOptimizationProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    IEvaluator IHeuristicOptimizationProblem.Evaluator {
      get { return EvaluatorParameter.Value; }
    }
    public DoubleValue BestKnownQuality {
      get { return BestKnownQualityParameter.Value; }
      set { BestKnownQualityParameter.Value = value; }
    }
    public BinaryVector BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    public IEnumerable<IOperator> Operators {
      get { return operators.Cast<IOperator>(); }
    }
    private BestKnapsackSolutionAnalyzer BestKnapsackSolutionAnalyzer {
      get { return operators.OfType<BestKnapsackSolutionAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    [Storable]
    private List<IOperator> operators;

    [StorableConstructor]
    private KnapsackProblem(bool deserializing) : base(deserializing) { }
    private KnapsackProblem(KnapsackProblem original, Cloner cloner)
      : base(original, cloner) {
      this.operators = original.operators.Select(x => (IOperator)cloner.Clone(x)).ToList();
      AttachEventHandlers();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new KnapsackProblem(this, cloner);
    }
    public KnapsackProblem()
      : base() {
      RandomBinaryVectorCreator creator = new RandomBinaryVectorCreator();
      KnapsackEvaluator evaluator = new KnapsackEvaluator();

      Parameters.Add(new ValueParameter<BoolValue>("Maximization", "Set to true as the Knapsack Problem is a maximization problem.", new BoolValue(true)));
      Parameters.Add(new ValueParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack.", new IntValue(0)));
      Parameters.Add(new ValueParameter<IntArray>("Weights", "The weights of the items.", new IntArray(5)));
      Parameters.Add(new ValueParameter<IntArray>("Values", "The values of the items.", new IntArray(5)));
      Parameters.Add(new ValueParameter<DoubleValue>("Penalty", "The penalty value for each unit of overweight.", new DoubleValue(1)));
      Parameters.Add(new ValueParameter<IBinaryVectorCreator>("SolutionCreator", "The operator which should be used to create new Knapsack solutions.", creator));
      Parameters.Add(new ValueParameter<IKnapsackEvaluator>("Evaluator", "The operator which should be used to evaluate Knapsack solutions.", evaluator));
      Parameters.Add(new OptionalValueParameter<DoubleValue>("BestKnownQuality", "The quality of the best known solution of this Knapsack instance."));
      Parameters.Add(new OptionalValueParameter<BinaryVector>("BestKnownSolution", "The best known solution of this Knapsack instance."));

      creator.BinaryVectorParameter.ActualName = "KnapsackSolution";

      InitializeRandomKnapsackInstance();

      ParameterizeSolutionCreator();
      ParameterizeEvaluator();

      InitializeOperators();
      AttachEventHandlers();
    }

    #region Events
    public event EventHandler SolutionCreatorChanged;
    private void OnSolutionCreatorChanged() {
      EventHandler handler = SolutionCreatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler EvaluatorChanged;
    private void OnEvaluatorChanged() {
      EventHandler handler = EvaluatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler OperatorsChanged;
    private void OnOperatorsChanged() {
      EventHandler handler = OperatorsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Reset;
    private void OnReset() {
      EventHandler handler = Reset;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    private void SolutionCreatorParameter_ValueChanged(object sender, EventArgs e) {
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_BinaryVectorParameter_ActualNameChanged);
      ParameterizeSolutionCreator();
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
      OnSolutionCreatorChanged();
    }
    private void SolutionCreator_BinaryVectorParameter_ActualNameChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeOperators();
    }
    private void EvaluatorParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      OnEvaluatorChanged();
    }
    void KnapsackCapacityParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
    }
    void WeightsParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeSolutionCreator();

      WeightsParameter.Value.Reset += new EventHandler(WeightsValue_Reset);
    }
    void WeightsValue_Reset(object sender, EventArgs e) {
      ParameterizeSolutionCreator();

      if (WeightsParameter.Value != null && ValuesParameter.Value != null)
        ((IStringConvertibleArray)ValuesParameter.Value).Length = WeightsParameter.Value.Length;
    }
    void ValuesParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
      ParameterizeAnalyzer();
      ParameterizeSolutionCreator();

      ValuesParameter.Value.Reset += new EventHandler(ValuesValue_Reset);
    }
    void ValuesValue_Reset(object sender, EventArgs e) {
      ParameterizeSolutionCreator();

      if (WeightsParameter.Value != null && ValuesParameter.Value != null)
        ((IStringConvertibleArray)WeightsParameter.Value).Length = ValuesParameter.Value.Length;
    }
    void PenaltyParameter_ValueChanged(object sender, EventArgs e) {
      ParameterizeEvaluator();
    }
    void OneBitflipMoveParameter_ActualNameChanged(object sender, EventArgs e) {
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
      if (operators == null) InitializeOperators();
      #endregion
      AttachEventHandlers();
    }

    private void AttachEventHandlers() {
      SolutionCreatorParameter.ValueChanged += new EventHandler(SolutionCreatorParameter_ValueChanged);
      SolutionCreator.BinaryVectorParameter.ActualNameChanged += new EventHandler(SolutionCreator_BinaryVectorParameter_ActualNameChanged);
      EvaluatorParameter.ValueChanged += new EventHandler(EvaluatorParameter_ValueChanged);
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
        knapsackEvaluator.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
        knapsackEvaluator.WeightsParameter.ActualName = WeightsParameter.Name;
        knapsackEvaluator.ValuesParameter.ActualName = ValuesParameter.Name;
        knapsackEvaluator.PenaltyParameter.ActualName = PenaltyParameter.Name;
      }
    }
    private void ParameterizeAnalyzer() {
      BestKnapsackSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
      BestKnapsackSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
      BestKnapsackSolutionAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
      BestKnapsackSolutionAnalyzer.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      BestKnapsackSolutionAnalyzer.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
      BestKnapsackSolutionAnalyzer.WeightsParameter.ActualName = WeightsParameter.Name;
      BestKnapsackSolutionAnalyzer.ValuesParameter.ActualName = ValuesParameter.Name;
      BestKnapsackSolutionAnalyzer.ResultsParameter.ActualName = "Results";
    }
    private void InitializeOperators() {
      operators = new List<IOperator>();
      operators.Add(new BestKnapsackSolutionAnalyzer());
      ParameterizeAnalyzer();
      foreach (IBinaryVectorOperator op in ApplicationManager.Manager.GetInstances<IBinaryVectorOperator>()) {
        if (!(op is ISingleObjectiveMoveEvaluator) || (op is IKnapsackMoveEvaluator)) {
          operators.Add(op);
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
        op.ChildParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      }
      foreach (IBinaryVectorManipulator op in Operators.OfType<IBinaryVectorManipulator>()) {
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      }
      foreach (IBinaryVectorMoveOperator op in Operators.OfType<IBinaryVectorMoveOperator>()) {
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
      }
      foreach (IKnapsackMoveEvaluator op in Operators.OfType<IKnapsackMoveEvaluator>()) {
        op.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
        op.PenaltyParameter.ActualName = PenaltyParameter.Name;
        op.WeightsParameter.ActualName = WeightsParameter.Name;
        op.ValuesParameter.ActualName = ValuesParameter.Name;
      }
      foreach (var op in Operators.OfType<IBinaryVectorMultiNeighborhoodShakingOperator>())
        op.BinaryVectorParameter.ActualName = SolutionCreator.BinaryVectorParameter.ActualName;
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
