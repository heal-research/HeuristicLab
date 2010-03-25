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
  [Item("ExhaustiveInsertionMoveGenerator", "Generates all possible insertion moves from a given permutation. Insertion is a 3-opt move.")]
  [StorableClass]
  public class ExhaustiveInsertionMoveGenerator : ThreeOptMoveGenerator, IExhaustiveMoveGenerator {
    public static ThreeOptMove[] Apply(Permutation permutation) {
      int length = permutation.Length;
      ThreeOptMove[] moves = new ThreeOptMove[length * (length - 1)];
      int count = 0;
      for (int i = 0; i < length; i++) {
        for (int j = 1; j <= length - 1; j++) {
          moves[count++] = new ThreeOptMove(i, i, (i + j) % length);
        }
      }
      return moves;
    }

    protected override ThreeOptMove[] GenerateMoves(Permutation permutation) {
      return Apply(permutation);
    }
  }
}
