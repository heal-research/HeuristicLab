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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Operators;
using System;

namespace HeuristicLab.Problems.Knapsack {
  /// <summary>
  /// A base class for operators which evaluates Knapsack solutions given in BinaryVector encoding.
  /// </summary>
  [Item("KnapsackEvaluator", "Evaluates solutions for the Knapsack problem.")]
  [StorableClass]
  public class KnapsackEvaluator : SingleSuccessorOperator, IKnapsackEvaluator {
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    
    public ILookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }

    public ILookupParameter<IntValue> KnapsackCapacityParameter {
      get { return (ILookupParameter<IntValue>)Parameters["KnapsackCapacity"]; }
    }
    public ILookupParameter<DoubleValue> PenaltyParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Penalty"]; }
    }
    public ILookupParameter<IntArray> WeightsParameter {
      get { return (ILookupParameter<IntArray>)Parameters["Weights"]; }
    }
    public ILookupParameter<IntArray> ValuesParameter {
      get { return (ILookupParameter<IntArray>)Parameters["Values"]; }
    }

    public KnapsackEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The evaluated quality of the OneMax solution."));
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The OneMax solution given in path representation which should be evaluated."));
      Parameters.Add(new LookupParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack."));
      Parameters.Add(new LookupParameter<IntArray>("Weights", "The weights of the items."));
      Parameters.Add(new LookupParameter<IntArray>("Values", "The values of the items."));
      Parameters.Add(new LookupParameter<DoubleValue>("Penalty", "The penalty value for each unit of overweight."));
    }

    public static DoubleValue Apply(BinaryVector v, IntValue capacity, DoubleValue penalty, IntArray weights, IntArray values) {
      if (weights.Length != values.Length)
        throw new InvalidOperationException("The weights and values parameters of the Knapsack problem have different sizes"); 
      
      double quality = 0;

      int weight = 0;
      int value = 0;

      for (int i = 0; i < v.Length; i++) {
        if (v[i]) {
          weight += weights[i];
          value += values[i];
        }
      }

      if (weight > capacity.Value) {
        quality =
          value - penalty.Value * (weight - capacity.Value);
      } else {
        quality = value;
      }

      return new DoubleValue(quality);
    }

    public sealed override IOperation Apply() {
      BinaryVector v = BinaryVectorParameter.ActualValue;

      DoubleValue quality = Apply(BinaryVectorParameter.ActualValue,
        KnapsackCapacityParameter.ActualValue, 
        PenaltyParameter.ActualValue, 
        WeightsParameter.ActualValue, 
        ValuesParameter.ActualValue);

      QualityParameter.ActualValue = quality;

      return base.Apply();
    }
  }
}
