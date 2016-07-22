#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("ExhaustiveSwap2MoveGenerator", "Generates all possible swap-2 moves from a given permutation.")]
  [StorableClass]
  public class ExhaustiveSwap2MoveGenerator : Swap2MoveGenerator, IExhaustiveMoveGenerator {
    [StorableConstructor]
    protected ExhaustiveSwap2MoveGenerator(bool deserializing) : base(deserializing) { }
    protected ExhaustiveSwap2MoveGenerator(ExhaustiveSwap2MoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public ExhaustiveSwap2MoveGenerator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExhaustiveSwap2MoveGenerator(this, cloner);
    }

    public static IEnumerable<Swap2Move> Generate(Permutation permutation) {
      int length = permutation.Length;
      if (length == 1) throw new ArgumentException("ExhaustiveSwap2MoveGenerator: There cannot be an Swap move given a permutation of length 1.", "permutation");

      for (int i = 0; i < length - 1; i++)
        for (int j = i + 1; j < length; j++) {
          yield return new Swap2Move(i, j);
        }
    }

    public static Swap2Move[] Apply(Permutation permutation) {
      int length = permutation.Length;
      int totalMoves = (length) * (length - 1) / 2;
      Swap2Move[] moves = new Swap2Move[totalMoves];
      int count = 0;
      foreach (Swap2Move move in Generate(permutation)) {
        moves[count++] = move;
      }
      return moves;
    }

    protected override Swap2Move[] GenerateMoves(Permutation permutation) {
      return Apply(permutation);
    }
  }
}
