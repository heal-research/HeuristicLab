#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.Permutation {
  [Item("ExhaustiveTwoOptMoveGenerator", "Generates all possible 2-opt moves from a given permutation.")]
  [StorableClass]
  public class ExhaustiveTwoOptMoveGenerator : TwoOptMoveGenerator {
    public static TwoOptMove[] Apply(Permutation permutation) {
      int length = permutation.Length;
      int totalMoves = (length) * (length - 1) / 2 - 3;
      TwoOptMove[] moves = new TwoOptMove[totalMoves];
      int count = 0;
      for (int i = 0; i < length - 1; i++)
        for (int j = i + 1; j < length; j++) {
          // doesn't make sense to inverse the whole permutation or the whole but one
          if (i == 0 && j >= length - 2) continue;
          else if (i == 1 && j >= length - 1) continue;
          moves[count++] = new TwoOptMove(i, j);
        }
      return moves;
    }

    protected override TwoOptMove[] GenerateMoves(Permutation permutation) {
      return Apply(permutation);
    }
  }
}
