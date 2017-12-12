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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.PTSP {
  [Item("Exhaustive 2.5-MoveGenerator", "Generates all possible inversion and shift moves (2.5-opt) from a given permutation.")]
  [StorableClass]
  public sealed class ExhaustiveTwoPointFiveMoveGenerator : TwoPointFiveMoveGenerator, IExhaustiveMoveGenerator {

    [StorableConstructor]
    private ExhaustiveTwoPointFiveMoveGenerator(bool deserializing) : base(deserializing) { }
    private ExhaustiveTwoPointFiveMoveGenerator(ExhaustiveTwoPointFiveMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public ExhaustiveTwoPointFiveMoveGenerator() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExhaustiveTwoPointFiveMoveGenerator(this, cloner);
    }

    public static TwoPointFiveMove[] Apply(Permutation permutation) {
      return Generate(permutation).ToArray();
    }

    public static IEnumerable<TwoPointFiveMove> Generate(Permutation permutation) {
      int length = permutation.Length;
      if (length == 1) throw new ArgumentException("Exhaustive 2.5-MoveGenerator: There cannot be a 2.5 move given a permutation of length 1.", "permutation");
      int totalMoves = (length) * (length - 1) / 2;

      if (permutation.PermutationType == PermutationTypes.RelativeUndirected) {
        if (totalMoves - 3 > 0) {
          for (int i = 0; i < length - 1; i++) {
            for (int j = i + 1; j < length; j++) {
              // doesn't make sense to inverse the whole permutation or the whole but one in case of relative undirected permutations
              if (j - i >= length - 2) continue;
              yield return new TwoPointFiveMove(i, j, true);
              yield return new TwoPointFiveMove(i, j, false);
            }
          }
        } else { // when length is 3 or less, there's actually no difference, but for the sake of not crashing the algorithm create a dummy move
          yield return new TwoPointFiveMove(0, 1, true);
        }
      } else {
        for (int i = 0; i < length - 1; i++)
          for (int j = i + 1; j < length; j++) {
            yield return new TwoPointFiveMove(i, j, true);
            yield return new TwoPointFiveMove(i, j, false);
          }
      }
    }

    protected override TwoPointFiveMove[] GenerateMoves(Permutation permutation) {
      return Apply(permutation);
    }
  }
}
