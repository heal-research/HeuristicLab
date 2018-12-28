#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.Encodings.RealVectorEncoding {
  [Item("AdditiveMoveTabuMaker", "Sets the move tabu.")]
  [StorableType("D495C70E-BE42-4FFB-B0E2-8422B7C8A1E9")]
  public class AdditiveMoveTabuMaker : TabuMaker, IAdditiveRealVectorMoveOperator {
    public ILookupParameter<AdditiveMove> AdditiveMoveParameter {
      get { return (ILookupParameter<AdditiveMove>)Parameters["AdditiveMove"]; }
    }
    public ILookupParameter<RealVector> RealVectorParameter {
      get { return (ILookupParameter<RealVector>)Parameters["RealVector"]; }
    }

    [StorableConstructor]
    protected AdditiveMoveTabuMaker(StorableConstructorFlag _) : base(_) { }
    protected AdditiveMoveTabuMaker(AdditiveMoveTabuMaker original, Cloner cloner) : base(original, cloner) { }
    public AdditiveMoveTabuMaker()
      : base() {
      Parameters.Add(new LookupParameter<AdditiveMove>("AdditiveMove", "The move to evaluate."));
      Parameters.Add(new LookupParameter<RealVector>("RealVector", "The solution as permutation."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new AdditiveMoveTabuMaker(this, cloner);
    }

    protected override IItem GetTabuAttribute(bool maximization, double quality, double moveQuality) {
      AdditiveMove move = AdditiveMoveParameter.ActualValue;
      RealVector vector = RealVectorParameter.ActualValue;
      double baseQuality = moveQuality;
      if (maximization && quality > moveQuality || !maximization && quality < moveQuality) baseQuality = quality;
      return new AdditiveMoveTabuAttribute(move.Dimension, vector[move.Dimension], vector[move.Dimension] + move.MoveDistance, baseQuality);
    }
  }
}
