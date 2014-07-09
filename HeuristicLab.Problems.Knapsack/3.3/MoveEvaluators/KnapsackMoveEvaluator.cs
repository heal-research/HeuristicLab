#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Knapsack {
  /// <summary>
  /// A base class for operators which evaluate Knapsack moves.
  /// </summary>
  [Item("KnapsackMoveEvaluator", "A base class for operators which evaluate Knapsack moves.")]
  [StorableClass]
  public abstract class KnapsackMoveEvaluator : SingleSuccessorOperator, IKnapsackMoveEvaluator, IMoveOperator, IBinaryVectorMoveOperator {
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
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

    [StorableConstructor]
    protected KnapsackMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected KnapsackMoveEvaluator(KnapsackMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    protected KnapsackMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of a Knapsack solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The evaluated quality of a move on a Knapsack solution."));
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The solution as BinaryVector."));
      Parameters.Add(new LookupParameter<IntValue>("KnapsackCapacity", "Capacity of the Knapsack."));
      Parameters.Add(new LookupParameter<IntArray>("Weights", "The weights of the items."));
      Parameters.Add(new LookupParameter<IntArray>("Values", "The values of the items."));
      Parameters.Add(new LookupParameter<DoubleValue>("Penalty", "The penalty value for each unit of overweight."));
    }
  }
}
