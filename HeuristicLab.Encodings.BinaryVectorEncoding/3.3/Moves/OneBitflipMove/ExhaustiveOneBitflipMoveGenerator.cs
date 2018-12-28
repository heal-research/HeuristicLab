#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.BinaryVectorEncoding {
  [Item("ExhaustiveBitflipMoveGenerator", "Generates all possible one bitflip moves from a given binaryVector.")]
  [StorableType("15F17361-2334-434A-904D-EF9426DEF32F")]
  public sealed class ExhaustiveOneBitflipMoveGenerator : OneBitflipMoveGenerator, IExhaustiveMoveGenerator {
    [StorableConstructor]
    private ExhaustiveOneBitflipMoveGenerator(StorableConstructorFlag _) : base(_) { }
    private ExhaustiveOneBitflipMoveGenerator(ExhaustiveOneBitflipMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public ExhaustiveOneBitflipMoveGenerator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExhaustiveOneBitflipMoveGenerator(this, cloner);
    }

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
