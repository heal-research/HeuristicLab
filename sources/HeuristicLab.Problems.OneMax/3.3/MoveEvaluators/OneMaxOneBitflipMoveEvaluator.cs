#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.OneMax {
  /// <summary>
  /// An operator to evaluate one bitflip moves.
  /// </summary>
  [Item("OneMaxOneBitflipMoveEvaluator", "Base class for evaluating one bitflip moves.")]
  [StorableClass]
  public class OneMaxOneBitflipMoveEvaluator : OneMaxMoveEvaluator, IOneBitflipMoveOperator {
    public ILookupParameter<OneBitflipMove> OneBitflipMoveParameter {
      get { return (ILookupParameter<OneBitflipMove>)Parameters["OneBitflipMove"]; }
    }

    [StorableConstructor]
    protected OneMaxOneBitflipMoveEvaluator(bool deserializing) : base(deserializing) { }
    protected OneMaxOneBitflipMoveEvaluator(OneMaxOneBitflipMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public OneMaxOneBitflipMoveEvaluator()
      : base() {
      Parameters.Add(new LookupParameter<OneBitflipMove>("OneBitflipMove", "The move to evaluate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneMaxOneBitflipMoveEvaluator(this, cloner);
    }

    public override IOperation Apply() {
      BinaryVector binaryVector = BinaryVectorParameter.ActualValue;
      OneBitflipMove move = OneBitflipMoveParameter.ActualValue;
      double moveQuality = QualityParameter.ActualValue.Value;

      if (binaryVector[move.Index])
        moveQuality -= 1.0;
      else
        moveQuality += 1.0;

      if (MoveQualityParameter.ActualValue == null) MoveQualityParameter.ActualValue = new DoubleValue(moveQuality);
      else MoveQualityParameter.ActualValue.Value = moveQuality;
      return base.Apply();
    }
  }
}
