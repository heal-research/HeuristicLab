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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("InversionMoveTabuMaker", "Declares a given inversion move (2-opt) as tabu, by adding its attributes to the tabu list and also store the solution quality or the move quality (whichever is better).")]
  [StorableType("81928009-A474-418B-8FD6-834FE79B43C9")]
  public class InversionMoveTabuMaker : TabuMaker, IPermutationInversionMoveQualityOperator {
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<InversionMove> InversionMoveParameter {
      get { return (LookupParameter<InversionMove>)Parameters["InversionMove"]; }
    }

    [StorableConstructor]
    protected InversionMoveTabuMaker(StorableConstructorFlag _) : base(_) { }
    protected InversionMoveTabuMaker(InversionMoveTabuMaker original, Cloner cloner) : base(original, cloner) { }
    public InversionMoveTabuMaker()
      : base() {
      Parameters.Add(new LookupParameter<InversionMove>("InversionMove", "The move that was made."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new InversionMoveTabuMaker(this, cloner);
    }

    protected override IItem GetTabuAttribute(bool maximization, double quality, double moveQuality) {
      InversionMove move = InversionMoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      double baseQuality = moveQuality;
      if (maximization && quality > moveQuality || !maximization && quality < moveQuality) baseQuality = quality; // we make an uphill move, the lower bound is the solution quality
      if (permutation.PermutationType == PermutationTypes.Absolute)
        return new InversionMoveAbsoluteAttribute(move.Index1, permutation[move.Index1], move.Index2, permutation[move.Index2], baseQuality);
      else
        return new InversionMoveRelativeAttribute(permutation.GetCircular(move.Index1 - 1),
          permutation[move.Index1],
          permutation[move.Index2],
          permutation.GetCircular(move.Index2 + 1),
          baseQuality);
    }
  }
}
