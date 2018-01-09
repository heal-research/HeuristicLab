#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Knapsack {
  /// <summary>
  /// An operator to evaluate one bitflip moves.
  /// </summary>
  [Item("KnapsackOneBitflipMoveEvaluator", "Base class for evaluating one bitflip moves.")]
  [StorableClass]
  public class KnapsackOneBitflipMoveEvaluator : KnapsackMoveEvaluator, IOneBitflipMoveOperator {
    public ILookupParameter<OneBitflipMove> OneBitflipMoveParameter {
      get { return (ILookupParameter<OneBitflipMove>)Parameters["OneBitflipMove"]; }
    }

    [StorableConstructor]
    protected KnapsackOneBitflipMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected KnapsackOneBitflipMoveEvaluator(KnapsackOneBitflipMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public KnapsackOneBitflipMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<OneBitflipMove>("OneBitflipMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KnapsackOneBitflipMoveEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      BinaryVector binaryVector = BinaryVectorParameter.ActualValue;
      OneBitflipMove move = OneBitflipMoveParameter.ActualValue;

      BinaryVector newSolution = new BinaryVector(binaryVector);
      newSolution[move.Index] = !newSolution[move.Index];

      DoubleValue quality = KnapsackEvaluator.Apply(newSolution,
        KnapsackCapacityParameter.ActualValue,
        PenaltyParameter.ActualValue,
        WeightsParameter.ActualValue,
        ValuesParameter.ActualValue).Quality;

      double moveQuality = quality.Value;

      if (MoveQualityParameter.ActualValue == null) MoveQualityParameter.ActualValue = new DoubleValue(moveQuality);
      else MoveQualityParameter.ActualValue.Value = moveQuality;
      return base.Apply();
    }
  }
}
