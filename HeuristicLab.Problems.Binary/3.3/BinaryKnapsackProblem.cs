#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Binary {
  [Item("Binary Knapsack Problem (BKSP)", "Represents a problem whose objective is to maximize the number of true values.")]
  [Creatable(CreatableAttribute.Categories.CombinatorialProblems, Priority = 999)]
  [StorableClass]
  public class BinaryKnapsackProblem : BinaryProblem {
    public override bool Maximization {
      get { return true; }
    }

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
    #endregion

    [StorableConstructor]
    protected BinaryKnapsackProblem(bool deserializing) : base(deserializing) { }
    protected BinaryKnapsackProblem(BinaryKnapsackProblem original, Cloner cloner) : base(original, cloner) { }
    public BinaryKnapsackProblem()
      : base() {
      Encoding.Length = 5;
      Parameters.Add(new ValueParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack.", new IntValue(0)));
      Parameters.Add(new ValueParameter<IntArray>("Weights", "The weights of the items.", new IntArray(5)));
      Parameters.Add(new ValueParameter<IntArray>("Values", "The values of the items.", new IntArray(5)));

      InitializeRandomKnapsackInstance();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new BinaryKnapsackProblem(this, cloner);
    }

    public override double Evaluate(BinaryVector vector, IRandom random) {
      var itemWeights = Weights;
      var itemValues = Values;
      var weight = 0;
      var value = 0;
      for (var i = 0; i < vector.Length; i++) {
        if (!vector[i]) continue;
        weight += itemWeights[i];
        value += itemValues[i];
      }
      var cap = KnapsackCapacity.Value;
      // assuming only positive values in the knapsack, maximizing in negative
      // range makes solutions feasible, in positive range increases their value
      return weight <= cap ? value : cap - weight;
    }

    private void InitializeRandomKnapsackInstance() {
      var rand = new System.Random();

      var itemCount = (int)Math.Pow(2, rand.Next(5, 10));
      Weights = new IntArray(itemCount);
      Values = new IntArray(itemCount);

      double totalWeight = 0;

      for (var i = 0; i < itemCount; i++) {
        var value = rand.Next(1, 50);
        var weight = rand.Next(1, 50);

        Values[i] = value;
        Weights[i] = weight;
        totalWeight += weight;
      }
      
      KnapsackCapacity = new IntValue((int)Math.Round(0.6 * totalWeight));
      Encoding.Length = itemCount;
    }
  }
}
