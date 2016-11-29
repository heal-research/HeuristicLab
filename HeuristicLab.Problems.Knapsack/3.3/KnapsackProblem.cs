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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Knapsack {
  [Item("Knapsack Problem (KSP)", "Represents a Knapsack problem.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 200)]
  [StorableClass]
  public sealed class KnapsackProblem : SingleObjectiveProblem<BinaryVectorEncoding, BinaryVector> {
    public override bool Maximization { get { return true; } }

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
    public OptionalValueParameter<BinaryVector> BestKnownSolutionParameter {
      get { return (OptionalValueParameter<BinaryVector>)Parameters["BestKnownSolution"]; }
    }
    #endregion

    #region Properties
    public int KnapsackCapacity {
      get { return KnapsackCapacityParameter.Value.Value; }
      set { KnapsackCapacityParameter.Value.Value = value; }
    }
    public IntArray Weights {
      get { return WeightsParameter.Value; }
      set { WeightsParameter.Value = value; }
    }
    public IntArray Values {
      get { return ValuesParameter.Value; }
      set { ValuesParameter.Value = value; }
    }
    public BinaryVector BestKnownSolution {
      get { return BestKnownSolutionParameter.Value; }
      set { BestKnownSolutionParameter.Value = value; }
    }
    private BestKnapsackSolutionAnalyzer BestKnapsackSolutionAnalyzer {
      get { return Operators.OfType<BestKnapsackSolutionAnalyzer>().FirstOrDefault(); }
    }
    #endregion

    [StorableConstructor]
    private KnapsackProblem(bool deserializing) : base(deserializing) { }
    private KnapsackProblem(KnapsackProblem original, Cloner cloner)
      : base(original, cloner) {
      RegisterEventHandlers();
    }
    public KnapsackProblem()
      : base(new BinaryVectorEncoding("Selection")) {
      Parameters.Add(new ValueParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack.", new IntValue(1)));
      Parameters.Add(new ValueParameter<IntArray>("Weights", "The weights of the items.", new IntArray(5)));
      Parameters.Add(new ValueParameter<IntArray>("Values", "The values of the items.", new IntArray(5)));
      Parameters.Add(new OptionalValueParameter<BinaryVector>("BestKnownSolution", "The best known solution of this Knapsack instance."));

      InitializeRandomKnapsackInstance();
      Encoding.Length = Weights.Length;

      InitializeOperators();
      RegisterEventHandlers();
    }

    public override double Evaluate(BinaryVector solution, IRandom random) {
      var weights = Weights;
      var values = Values;
      var totalWeight = 0.0;
      var totalValue = 0.0;
      for (var i = 0; i < solution.Length; i++) {
        if (!solution[i]) continue;
        totalWeight += weights[i];
        totalValue += values[i];
      }
      return totalWeight > KnapsackCapacity ? KnapsackCapacity - totalWeight : totalValue;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KnapsackProblem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
      KnapsackCapacityParameter.ValueChanged += KnapsackCapacityParameter_ValueChanged;
      WeightsParameter.ValueChanged += WeightsParameter_ValueChanged;
      WeightsParameter.Value.Reset += WeightsValue_Reset;
      ValuesParameter.ValueChanged += ValuesParameter_ValueChanged;
      ValuesParameter.Value.Reset += ValuesValue_Reset;
      // TODO: There is no even to detect if the parameter itself was changed
      Encoding.LengthParameter.ValueChanged += Encoding_LengthParameter_ValueChanged;
    }

    #region Events
    protected override void OnEncodingChanged() {
      base.OnEncodingChanged();
      Parameterize();
    }
    //TODO check with abeham if this is really necessary
    //protected override void OnSolutionCreatorChanged() {
    //  base.OnSolutionCreatorChanged();
    //  Parameterize();
    //}
    protected override void OnEvaluatorChanged() {
      base.OnEvaluatorChanged();
      Evaluator.QualityParameter.ActualNameChanged += Evaluator_QualityParameter_ActualNameChanged;
      Parameterize();
    }
    private void Evaluator_QualityParameter_ActualNameChanged(object sender, EventArgs e) {
      Parameterize();
    }
    private void KnapsackCapacityParameter_ValueChanged(object sender, EventArgs e) {
      Parameterize();
    }
    private void WeightsParameter_ValueChanged(object sender, EventArgs e) {
      Parameterize();
      WeightsParameter.Value.Reset += WeightsValue_Reset;
    }
    private void WeightsValue_Reset(object sender, EventArgs e) {
      if (WeightsParameter.Value != null && ValuesParameter.Value != null) {
        ((IStringConvertibleArray)ValuesParameter.Value).Length = Weights.Length;
        Encoding.Length = Weights.Length;
      }
      Parameterize();
    }
    private void ValuesParameter_ValueChanged(object sender, EventArgs e) {
      Parameterize();
      ValuesParameter.Value.Reset += ValuesValue_Reset;
    }
    private void ValuesValue_Reset(object sender, EventArgs e) {
      if (WeightsParameter.Value != null && ValuesParameter.Value != null) {
        ((IStringConvertibleArray)WeightsParameter.Value).Length = Values.Length;
        Encoding.Length = Values.Length;
      }
      Parameterize();
    }
    private void Encoding_LengthParameter_ValueChanged(object sender, EventArgs e) {
      if (Weights.Length != Encoding.Length) {
        ((IStringConvertibleArray)WeightsParameter.Value).Length = Encoding.Length;
      }
      if (Values.Length != Encoding.Length) {
        ((IStringConvertibleArray)ValuesParameter.Value).Length = Encoding.Length;
      }
      Parameterize();
    }
    #endregion

    #region Helpers
    private void InitializeOperators() {
      Operators.Add(new KnapsackImprovementOperator());
      Operators.Add(new KnapsackPathRelinker());
      Operators.Add(new KnapsackSimultaneousPathRelinker());
      Operators.Add(new KnapsackSimilarityCalculator());
      Operators.Add(new QualitySimilarityCalculator());
      Operators.Add(new NoSimilarityCalculator());

      Operators.Add(new BestKnapsackSolutionAnalyzer());
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      Parameterize();
    }
    private void Parameterize() {
      var operators = new List<IItem>();

      if (BestKnapsackSolutionAnalyzer != null) {
        operators.Add(BestKnapsackSolutionAnalyzer);
        BestKnapsackSolutionAnalyzer.MaximizationParameter.ActualName = MaximizationParameter.Name;
        BestKnapsackSolutionAnalyzer.MaximizationParameter.Hidden = true;
        BestKnapsackSolutionAnalyzer.BestKnownQualityParameter.ActualName = BestKnownQualityParameter.Name;
        BestKnapsackSolutionAnalyzer.BestKnownQualityParameter.Hidden = true;
        BestKnapsackSolutionAnalyzer.BestKnownSolutionParameter.ActualName = BestKnownSolutionParameter.Name;
        BestKnapsackSolutionAnalyzer.BestKnownSolutionParameter.Hidden = true;
        BestKnapsackSolutionAnalyzer.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
        BestKnapsackSolutionAnalyzer.KnapsackCapacityParameter.Hidden = true;
        BestKnapsackSolutionAnalyzer.WeightsParameter.ActualName = WeightsParameter.Name;
        BestKnapsackSolutionAnalyzer.WeightsParameter.Hidden = true;
        BestKnapsackSolutionAnalyzer.ValuesParameter.ActualName = ValuesParameter.Name;
        BestKnapsackSolutionAnalyzer.ValuesParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<IKnapsackMoveEvaluator>()) {
        operators.Add(op);
        op.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
        op.KnapsackCapacityParameter.Hidden = true;
        op.WeightsParameter.ActualName = WeightsParameter.Name;
        op.WeightsParameter.Hidden = true;
        op.ValuesParameter.ActualName = ValuesParameter.Name;
        op.ValuesParameter.Hidden = true;

        var bitflipMoveEval = op as IKnapsackOneBitflipMoveEvaluator;
        if (bitflipMoveEval != null) {
          foreach (var moveOp in Encoding.Operators.OfType<IOneBitflipMoveQualityOperator>()) {
            moveOp.MoveQualityParameter.ActualName = bitflipMoveEval.MoveQualityParameter.ActualName;
            moveOp.MoveQualityParameter.Hidden = true;
          }
        }
      }
      foreach (var op in Operators.OfType<ISingleObjectiveImprovementOperator>()) {
        operators.Add(op);
        op.SolutionParameter.ActualName = Encoding.Name;
        op.SolutionParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<ISingleObjectivePathRelinker>()) {
        operators.Add(op);
        op.ParentsParameter.ActualName = Encoding.Name;
        op.ParentsParameter.Hidden = true;
      }
      foreach (var op in Operators.OfType<ISolutionSimilarityCalculator>()) {
        operators.Add(op);
        op.SolutionVariableName = Encoding.Name;
        op.QualityVariableName = Evaluator.QualityParameter.ActualName;
      }

      if (operators.Count > 0) Encoding.ConfigureOperators(Operators);
    }
    #endregion

    private void InitializeRandomKnapsackInstance() {
      var sysrand = new System.Random();

      var power = sysrand.Next(5, 11);
      var itemCount = (int)Math.Pow(2, power);
      Weights = new IntArray(itemCount);
      Values = new IntArray(itemCount);

      double totalWeight = 0;

      for (int i = 0; i < itemCount; i++) {
        var value = sysrand.Next(1, 30);
        var weight = sysrand.Next(1, 30);

        Values[i] = value;
        Weights[i] = weight;
        totalWeight += weight;
      }

      KnapsackCapacity = (int)Math.Round(0.5 * totalWeight);
    }
  }
}
