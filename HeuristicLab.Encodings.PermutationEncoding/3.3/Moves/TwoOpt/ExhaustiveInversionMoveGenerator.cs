#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("ExhaustiveInversionMoveGenerator", "Generates all possible inversion moves (2-opt) from a given permutation.")]
  [StorableClass]
  public class ExhaustiveInversionMoveGenerator : InversionMoveGenerator, IExhaustiveMoveGenerator {
    [StorableConstructor]
    protected ExhaustiveInversionMoveGenerator(bool deserializing) : base(deserializing) { }
    protected ExhaustiveInversionMoveGenerator(ExhaustiveInversionMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public ExhaustiveInversionMoveGenerator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExhaustiveInversionMoveGenerator(this, cloner);
    }

    public static InversionMove[] Apply(Permutation permutation) {
      int length = permutation.Length;
      if (length == 1) throw new ArgumentException("ExhaustiveInversionMoveGenerator: There cannot be an inversion move given a permutation of length 1.", "permutation");
      int totalMoves = (length) * (length - 1) / 2;
      InversionMove[] moves = null;
      int count = 0;

      if (permutation.PermutationType == PermutationTypes.RelativeUndirected) {
        if (totalMoves - 3 > 0) {
          moves = new InversionMove[totalMoves - 3];
          for (int i = 0; i < length - 1; i++) {
            for (int j = i + 1; j < length; j++) {
              // doesn't make sense to inverse the whole permutation or the whole but one in case of relative undirected permutations
              if (j - i >= length - 2) continue;
              moves[count++] = new InversionMove(i, j);
            }
          }
        } else { // when length is 3 or less, there's actually no difference, but for the sake of not crashing the algorithm create a dummy move
          moves = new InversionMove[1];
          moves[0] = new InversionMove(0, 1);
        }
      } else {
        moves = new InversionMove[totalMoves];
        for (int i = 0; i < length - 1; i++)
          for (int j = i + 1; j < length; j++) {
            moves[count++] = new InversionMove(i, j);
          }
      }
      return moves;
    }

    protected override InversionMove[] GenerateMoves(Permutation permutation) {
      return Apply(permutation);
    }
  }
}
