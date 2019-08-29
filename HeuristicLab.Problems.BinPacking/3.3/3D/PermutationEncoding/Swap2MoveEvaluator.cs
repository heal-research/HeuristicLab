#region License Information
/* HeuristicLab
 * Copyright (C) Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Encodings.PermutationEncoding;
using HEAL.Attic;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Swap2MoveEvaluator", "Move evaluator for 2-opt moves.")]
  [StorableType("CF5E678F-E420-4825-A3FE-AE48D9B9B7C0")]
  public sealed class Swap2MoveEvaluator : MoveEvaluatorBase<Permutation, Swap2Move>, IPermutationSwap2MoveOperator {
    public ILookupParameter<Swap2Move> Swap2MoveParameter {
      get { return MoveParameter; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return EncodedSolutionParameter; }
    }

    [StorableConstructor]
    private Swap2MoveEvaluator(StorableConstructorFlag _) : base(_) { }
    private Swap2MoveEvaluator(Swap2MoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public Swap2MoveEvaluator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Swap2MoveEvaluator(this, cloner);
    }

    public override double EvaluateMove(Permutation permutation, Swap2Move move, PackingShape binShape, ReadOnlyItemList<PackingItem> items, bool useStackingConstraints) {
      // uses full evaluation
      Swap2Manipulator.Apply(permutation, move.Index1, move.Index2);
      var solution = DecoderParameter.ActualValue.Decode(permutation, binShape, items, useStackingConstraints);

      return SolutionEvaluatorParameter.ActualValue.Evaluate(solution);
    }
  }
}
