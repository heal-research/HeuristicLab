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
  [Item("TranslocationMoveTabuMaker", "Declares a given translocation or insertion move (3-opt) as tabu, by adding its attributes to the tabu list.")]
  [StorableType("A5B9AC7B-3A83-4BF4-914A-A0FD17AE8A94")]
  public class TranslocationMoveTabuMaker : TabuMaker, IPermutationTranslocationMoveQualityOperator {
    public ILookupParameter<Permutation> PermutationParameter {
      get { return (ILookupParameter<Permutation>)Parameters["Permutation"]; }
    }
    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return (LookupParameter<TranslocationMove>)Parameters["TranslocationMove"]; }
    }

    [StorableConstructor]
    protected TranslocationMoveTabuMaker(StorableConstructorFlag _) : base(_) { }
    protected TranslocationMoveTabuMaker(TranslocationMoveTabuMaker original, Cloner cloner) : base(original, cloner) { }
    public TranslocationMoveTabuMaker()
      : base() {
      Parameters.Add(new LookupParameter<TranslocationMove>("TranslocationMove", "The move that was made."));
      Parameters.Add(new LookupParameter<Permutation>("Permutation", "The solution as permutation."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TranslocationMoveTabuMaker(this, cloner);
    }

    protected override IItem GetTabuAttribute(bool maximization, double quality, double moveQuality) {
      TranslocationMove move = TranslocationMoveParameter.ActualValue;
      Permutation permutation = PermutationParameter.ActualValue;
      double baseQuality = moveQuality;
      if (maximization && quality > moveQuality || !maximization && quality < moveQuality) baseQuality = quality; // we make an uphill move, the lower bound is the solution quality
      if (permutation.PermutationType == PermutationTypes.Absolute) {
        int[] numbers = new int[move.Index2 - move.Index1 + 1];
        for (int i = 0; i < numbers.Length; i++) {
          numbers[i] = permutation[i + move.Index1];
        }
        return new TranslocationMoveAbsoluteAttribute(numbers, move.Index1, move.Index3, baseQuality); ;
      } else {
        if (move.Index3 > move.Index1)
          return new TranslocationMoveRelativeAttribute(permutation.GetCircular(move.Index1 - 1),
          permutation[move.Index1],
          permutation[move.Index2],
          permutation.GetCircular(move.Index2 + 1),
          permutation.GetCircular(move.Index3 + move.Index2 - move.Index1),
          permutation.GetCircular(move.Index3 + move.Index2 - move.Index1 + 1),
          baseQuality);
        else
          return new TranslocationMoveRelativeAttribute(permutation.GetCircular(move.Index1 - 1),
          permutation[move.Index1],
          permutation[move.Index2],
          permutation.GetCircular(move.Index2 + 1),
          permutation.GetCircular(move.Index3 - 1),
          permutation.GetCircular(move.Index3),
          baseQuality);
      }
    }
  }
}
