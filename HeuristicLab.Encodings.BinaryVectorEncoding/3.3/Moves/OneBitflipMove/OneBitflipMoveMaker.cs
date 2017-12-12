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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("OneBitflipMoveMaker", "Peforms a one bitflip move on a given BitVector and updates the quality.")]
  [StorableClass]
  public class OneBitflipMoveMaker : SingleSuccessorOperator, IOneBitflipMoveOperator, IMoveMaker, ISingleObjectiveOperator {
    public override bool CanChangeName {
      get { return false; }
    }
    public ILookupParameter<DoubleValue> QualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["Quality"]; }
    }
    public ILookupParameter<DoubleValue> MoveQualityParameter {
      get { return (ILookupParameter<DoubleValue>)Parameters["MoveQuality"]; }
    }
    public ILookupParameter<OneBitflipMove> OneBitflipMoveParameter {
      get { return (ILookupParameter<OneBitflipMove>)Parameters["OneBitflipMove"]; }
    }
    public ILookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }

    [StorableConstructor]
    protected OneBitflipMoveMaker(bool deserializing) : base(deserializing) { }
    protected OneBitflipMoveMaker(OneBitflipMoveMaker original, Cloner cloner) : base(original, cloner) { }
    public OneBitflipMoveMaker()
      : base() {
      Parameters.Add(new LookupParameter<DoubleValue>("Quality", "The quality of the solution."));
      Parameters.Add(new LookupParameter<DoubleValue>("MoveQuality", "The relative quality of the move."));
      Parameters.Add(new LookupParameter<OneBitflipMove>("OneBitflipMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The solution as BinaryVector."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneBitflipMoveMaker(this, cloner);
    }

    public override IOperation Apply() {
      OneBitflipMove move = OneBitflipMoveParameter.ActualValue;
      BinaryVector binaryVector = BinaryVectorParameter.ActualValue;
      DoubleValue moveQuality = MoveQualityParameter.ActualValue;
      DoubleValue quality = QualityParameter.ActualValue;

      binaryVector[move.Index] = !binaryVector[move.Index];

      quality.Value = moveQuality.Value;

      return base.Apply();
    }
  }
}
