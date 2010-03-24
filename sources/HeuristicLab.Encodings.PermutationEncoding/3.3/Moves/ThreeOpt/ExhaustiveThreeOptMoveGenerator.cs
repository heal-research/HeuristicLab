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

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("ExhaustiveThreeOptMoveGenerator", "Generates all possible 3-opt moves from a given permutation.")]
  [StorableClass]
  public class ExhaustiveThreeOptMoveGenerator : ThreeOptMoveGenerator, IExhaustiveMoveGenerator {
    public static ThreeOptMove[] Apply(Permutation permutation) {
      int length = permutation.Length;
      ThreeOptMove[] moves = new ThreeOptMove[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        int index1, index2, index3;
        index1 = random.Next(length - 1);
        do {
          index2 = random.Next(index1 + 1, length);
        } while (index2 - index1 >= length - 2);
        do {
          index3 = random.Next(length - index2 + index1 - 1);
        } while (index3 == index1);
        
        moves[i] = new ThreeOptMove(index1, index2, index3);
      }
      return moves;
    }

    protected override ThreeOptMove[] GenerateMoves(Permutation permutation) {
      return Apply(permutation);
    }
  }
}
