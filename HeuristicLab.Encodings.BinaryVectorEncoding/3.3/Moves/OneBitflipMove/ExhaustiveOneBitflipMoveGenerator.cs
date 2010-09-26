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

using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("ExhaustiveBitflipMoveGenerator", "Generates all possible one bitflip moves from a given binaryVector.")]
  [StorableClass]
  public class ExhaustiveOneBitflipMoveGenerator : OneBitflipMoveGenerator, IExhaustiveMoveGenerator {
    public static OneBitflipMove[] Apply(BinaryVector binaryVector) {
      int length = binaryVector.Length;
      int totalMoves = length;
      OneBitflipMove[] moves = new OneBitflipMove[totalMoves];
      for (int i = 0; i < length; i++)
        moves[i] = new OneBitflipMove(i);

      return moves;
    }

    protected override OneBitflipMove[] GenerateMoves(BinaryVector binaryVector) {
      return Apply(binaryVector);
    }
  }
}
