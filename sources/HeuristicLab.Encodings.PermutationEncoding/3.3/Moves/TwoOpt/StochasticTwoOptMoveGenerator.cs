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
  [Item("StochasticTwoOptMoveGenerator", "Randomly samples n from all possible 2-opt moves from a given permutation.")]
  [StorableClass]
  public class StochasticTwoOptMoveGenerator : TwoOptMoveGenerator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    private ValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (ValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    public StochasticTwoOptMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    public static TwoOptMove[] Apply(Permutation permutation, IRandom random, int sampleSize) {
      int length = permutation.Length;
      int totalMoves = (length) * (length - 1) / 2 - 3;
      if (sampleSize >= totalMoves) throw new InvalidOperationException("StochasticTwoOptMoveGenerator: Sample size (" + sampleSize + ") is larger than the set of all possible moves (" + totalMoves + "), use the ExhaustiveTwoOptMoveGenerator instead.");
      TwoOptMove[] moves = new TwoOptMove[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        int index1 = random.Next(length - 1);
        int index2 = random.Next(index1 + 1, length);
        moves[i] = new TwoOptMove(index1, index2);
      }
      return moves;
    }

    protected override TwoOptMove[] GenerateMoves(Permutation permutation) {
      IRandom random = RandomParameter.ActualValue;
      return Apply(permutation, random, SampleSizeParameter.ActualValue.Value);
    }
  }
}
