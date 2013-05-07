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
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("OneBitflipMoveTabuMaker", "Declares a given one bitflip move as tabu, by adding its attributes to the tabu list. It also removes the oldest entry in the tabu list when its size is greater than tenure.")]
  [StorableClass]
  public class OneBitflipMoveTabuMaker : TabuMaker, IOneBitflipMoveOperator {
    public ILookupParameter<BinaryVector> BinaryVectorParameter {
      get { return (ILookupParameter<BinaryVector>)Parameters["BinaryVector"]; }
    }
    public ILookupParameter<OneBitflipMove> OneBitflipMoveParameter {
      get { return (LookupParameter<OneBitflipMove>)Parameters["OneBitflipMove"]; }
    }

    [StorableConstructor]
    protected OneBitflipMoveTabuMaker(bool deserializing) : base(deserializing) { }
    protected OneBitflipMoveTabuMaker(OneBitflipMoveTabuMaker original, Cloner cloner) : base(original, cloner) { }
    public OneBitflipMoveTabuMaker()
      : base() {
      Parameters.Add(new LookupParameter<BinaryVector>("BinaryVector", "The solution as BinaryVector."));
      Parameters.Add(new LookupParameter<OneBitflipMove>("OneBitflipMove", "The move that was made."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new OneBitflipMoveTabuMaker(this, cloner);
    }

    protected override IItem GetTabuAttribute(bool maximization, double quality, double moveQuality) {
      double baseQuality = moveQuality;
      if (maximization && quality > moveQuality || !maximization && quality < moveQuality) baseQuality = quality;
      return new OneBitflipMoveAttribute(OneBitflipMoveParameter.ActualValue.Index, baseQuality);
    }
  }
}
