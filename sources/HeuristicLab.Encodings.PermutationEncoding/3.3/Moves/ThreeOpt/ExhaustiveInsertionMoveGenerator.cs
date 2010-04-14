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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using System.Collections.Generic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("ExhaustiveInsertionMoveGenerator", "Generates all possible insertion moves (3-opt) from a given permutation.")]
  [StorableClass]
  public class ExhaustiveInsertionMoveGenerator : TranslocationMoveGenerator, IExhaustiveMoveGenerator {
    public static TranslocationMove[] Apply(Permutation permutation) {
      int length = permutation.Length;
      TranslocationMove[] moves = null;
      int count = 0;
      if (permutation.PermutationType == PermutationTypes.Absolute) {
        moves = new TranslocationMove[length * (length - 1)];
        for (int i = 0; i < length; i++) {
          for (int j = 1; j <= length - 1; j++) {
            moves[count++] = new TranslocationMove(i, i, (i + j) % length);
          }
        }
      } else {
        moves = new TranslocationMove[length * (length - 1) - 2];
        for (int i = 0; i < length; i++) {
          for (int j = 1; j <= length - 1; j++) {
            if (i == 0 && j == length - 1
              || i == length - 1 && j == 1) continue;
            moves[count++] = new TranslocationMove(i, i, (i + j) % length);
          }
        }
      }
      System.Diagnostics.Debug.Assert(count == moves.Length);
      return moves;
    }

    protected override TranslocationMove[] GenerateMoves(Permutation permutation) {
      return Apply(permutation);
    }
  }
}
