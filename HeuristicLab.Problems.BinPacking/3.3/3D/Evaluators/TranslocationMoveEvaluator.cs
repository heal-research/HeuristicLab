#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.BinPacking3D.Evaluators {
  [Item("TranslocationMoveEvaluator", "Move evaluator for insertion or translocation moves.")]
  [StorableClass]
  public sealed class TranslocationMoveEvaluator : MoveEvaluatorBase<Permutation, TranslocationMove>, IPermutationTranslocationMoveOperator {
    public ILookupParameter<TranslocationMove> TranslocationMoveParameter {
      get { return MoveParameter; }
    }
    public ILookupParameter<Permutation> PermutationParameter {
      get { return EncodedSolutionParameter; }
    }
    [StorableConstructor]
    private TranslocationMoveEvaluator(bool deserializing) : base(deserializing) { }
    private TranslocationMoveEvaluator(TranslocationMoveEvaluator original, Cloner cloner) : base(original, cloner) { }
    public TranslocationMoveEvaluator()
      : base() {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TranslocationMoveEvaluator(this, cloner);
    }

    public override double EvaluateMove(Permutation permutation, TranslocationMove move, PackingShape binShape,
      ReadOnlyItemList<PackingItem> items, bool useStackingConstraints) {

      // uses full evaluation
      TranslocationManipulator.Apply(permutation, move.Index1, move.Index2, move.Index3);
      var solution = DecoderParameter.ActualValue.Decode(permutation, binShape, items, useStackingConstraints);

      return SolutionEvaluatorParameter.ActualValue.Evaluate(solution);
    }
  }
}
