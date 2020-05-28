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
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;

namespace HeuristicLab.Problems.Knapsack {
  [Item("Knapsack Problem (KSP)", "Represents a Knapsack problem.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 200)]
  [StorableType("8CEDAFA2-6E0A-4D4B-B6C6-F85CC58B824E")]
  public sealed class KnapsackProblem : BinaryVectorProblem {

    #region Parameter Properties
    [Storable] public ValueParameter<IntValue> KnapsackCapacityParameter { get; private set; }
    [Storable] public ValueParameter<IntArray> WeightsParameter { get; private set; }
    [Storable] public ValueParameter<IntArray> ValuesParameter { get; private set; }
    [Storable] public OptionalValueParameter<BinaryVector> BestKnownSolutionParameter { get; private set; }
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
    #endregion

    [StorableConstructor]
    private KnapsackProblem(StorableConstructorFlag _) : base(_) { }
    private KnapsackProblem(KnapsackProblem original, Cloner cloner)
      : base(original, cloner) {
      KnapsackCapacityParameter = cloner.Clone(original.KnapsackCapacityParameter);
      WeightsParameter = cloner.Clone(original.WeightsParameter);
      ValuesParameter = cloner.Clone(original.ValuesParameter);
      BestKnownSolutionParameter = cloner.Clone(original.BestKnownSolutionParameter);
      RegisterEventHandlers();
    }
    public KnapsackProblem()
      : base(new BinaryVectorEncoding("Selection")) {
      DimensionRefParameter.ReadOnly = true;
      Maximization = true;
      Parameters.Add(KnapsackCapacityParameter = new ValueParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack.", new IntValue(1)));
      Parameters.Add(WeightsParameter = new ValueParameter<IntArray>("Weights", "The weights of the items.", new IntArray(5)));
      Parameters.Add(ValuesParameter = new ValueParameter<IntArray>("Values", "The values of the items.", new IntArray(5)));
      Parameters.Add(BestKnownSolutionParameter = new OptionalValueParameter<BinaryVector>("BestKnownSolution", "The best known solution of this Knapsack instance."));
      Dimension = Weights.Length;

      InitializeRandomKnapsackInstance();

      InitializeOperators();
      RegisterEventHandlers();
    }

    public override ISingleObjectiveEvaluationResult Evaluate(BinaryVector solution, IRandom random, CancellationToken cancellationToken) {
      var totalWeight = 0.0;
      var totalValue = 0.0;
      for (var i = 0; i < solution.Length; i++) {
        if (!solution[i]) continue;
        totalWeight += Weights[i];
        totalValue += Values[i];
      }
      var quality = totalWeight > KnapsackCapacity ? KnapsackCapacity - totalWeight : totalValue;
      return new SingleObjectiveEvaluationResult(quality);
    }

    public override void Analyze(BinaryVector[] solutions, double[] qualities, ResultCollection results, IRandom random) {
      base.Analyze(solutions, qualities, results, random);

      var best = GetBestSolution(solutions, qualities);

      if (double.IsNaN(BestKnownQuality) || IsBetter(best.Item2, BestKnownQuality)) {
        BestKnownQuality = best.Item2;
        BestKnownSolution = (BinaryVector)best.Item1.Clone();
      }

      IResult result;
      if (!results.TryGetValue("Best Knapsack Solution", out result)) {
        results.Add(result = new Result("Best Knapsack Solution", typeof(KnapsackSolution)));
      }
      var solution = (KnapsackSolution)result.Value;
      if (solution == null) {
        solution = new KnapsackSolution((BinaryVector)best.Item1.Clone(), new DoubleValue(best.Item2),
          KnapsackCapacityParameter.Value, WeightsParameter.Value, ValuesParameter.Value);
        result.Value = solution;
      } else {
        if (IsBetter(best.Item2, solution.Quality.Value)) {
          solution.BinaryVector = (BinaryVector)best.Item1.Clone();
          solution.Quality = new DoubleValue(best.Item2);
          solution.Capacity = KnapsackCapacityParameter.Value;
          solution.Weights = WeightsParameter.Value;
          solution.Values = ValuesParameter.Value;
        }
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KnapsackProblem(this, cloner);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers() {
      WeightsParameter.ValueChanged += WeightsParameter_ValueChanged;
      WeightsParameter.Value.Reset += (_, __) => SyncValuesToWeights();
      ValuesParameter.ValueChanged += ValuesParameter_ValueChanged;
      ValuesParameter.Value.Reset += (_, __) => SyncWeightsToValues();
    }

    #region Events
    protected override void DimensionOnChanged() {
      base.DimensionOnChanged();
      if (Weights.Length != Dimension) {
        ((IStringConvertibleArray)WeightsParameter.Value).Length = Dimension;
      }
      if (Values.Length != Dimension) {
        ((IStringConvertibleArray)ValuesParameter.Value).Length = Dimension;
      }
    }
    private void WeightsParameter_ValueChanged(object sender, EventArgs e) {
      WeightsParameter.Value.Reset += (_, __) => SyncValuesToWeights();
      SyncValuesToWeights();
    }
    private void ValuesParameter_ValueChanged(object sender, EventArgs e) {
      ValuesParameter.Value.Reset += (_, __) => SyncWeightsToValues();
      SyncWeightsToValues();
    }
    private void SyncWeightsToValues() {
      if (WeightsParameter.Value != null && ValuesParameter.Value != null) {
        ((IStringConvertibleArray)WeightsParameter.Value).Length = Values.Length;
        Dimension = Values.Length;
      }
    }
    private void SyncValuesToWeights() {
      if (WeightsParameter.Value != null && ValuesParameter.Value != null) {
        ((IStringConvertibleArray)ValuesParameter.Value).Length = Weights.Length;
        Dimension = Weights.Length;
      }
    }
    #endregion

    #region Helpers
    private void InitializeOperators() {
      Operators.AddRange(new IItem[] { new KnapsackImprovementOperator(),
      new KnapsackPathRelinker(), new KnapsackSimultaneousPathRelinker(),
      new QualitySimilarityCalculator(), new NoSimilarityCalculator(),
      new KnapsackOneBitflipMoveEvaluator()});
      Operators.Add(new PopulationSimilarityAnalyzer(Operators.OfType<ISolutionSimilarityCalculator>()));

      ParameterizeOperators();
    }

    protected override void ParameterizeOperators() {
      base.ParameterizeOperators();
      Parameterize();
    }

    private void Parameterize() {
      foreach (var op in Operators.OfType<IKnapsackMoveEvaluator>()) {
        op.KnapsackCapacityParameter.ActualName = KnapsackCapacityParameter.Name;
        op.KnapsackCapacityParameter.Hidden = true;
        op.WeightsParameter.ActualName = WeightsParameter.Name;
        op.WeightsParameter.Hidden = true;
        op.ValuesParameter.ActualName = ValuesParameter.Name;
        op.ValuesParameter.Hidden = true;
      }
    }
    #endregion

    private void InitializeRandomKnapsackInstance() {
      var sysrand = new System.Random();

      var itemCount = sysrand.Next(10, 100);
      Weights = new IntArray(itemCount);
      Values = new IntArray(itemCount);

      double totalWeight = 0;

      for (int i = 0; i < itemCount; i++) {
        var value = sysrand.Next(1, 10);
        var weight = sysrand.Next(1, 10);

        Values[i] = value;
        Weights[i] = weight;
        totalWeight += weight;
      }

      KnapsackCapacity = (int)Math.Round(0.7 * totalWeight);
      Dimension = Weights.Length;
    }
  }
}
