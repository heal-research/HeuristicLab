#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("StochasticScrambleMultiMoveGenerator", "Randomly samples n from all possible scramble moves from a given permutation.")]
  [StorableClass]
  public class StochasticScrambleMultiMoveGenerator : ScrambleMoveGenerator, IMultiMoveGenerator, IStochasticOperator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }
    public IValueLookupParameter<IntValue> SampleSizeParameter {
      get { return (IValueLookupParameter<IntValue>)Parameters["SampleSize"]; }
    }

    public IntValue SampleSize {
      get { return SampleSizeParameter.Value; }
      set { SampleSizeParameter.Value = value; }
    }

    [StorableConstructor]
    protected StochasticScrambleMultiMoveGenerator(bool deserializing) : base(deserializing) { }
    protected StochasticScrambleMultiMoveGenerator(StochasticScrambleMultiMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticScrambleMultiMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticScrambleMultiMoveGenerator(this, cloner);
    }

    public static ScrambleMove GenerateRandomMove(Permutation permutation, IRandom random) {
      int breakPoint1, breakPoint2;
      int[] scrambledIndices;

      breakPoint1 = random.Next(permutation.Length);
      do {
        breakPoint2 = random.Next(permutation.Length);
      } while (Math.Abs(breakPoint2 - breakPoint1) <= 1);
      if (breakPoint2 < breakPoint1) { int h = breakPoint1; breakPoint1 = breakPoint2; breakPoint2 = h; }

      scrambledIndices = new int[breakPoint2 - breakPoint1 + 1];
      for (int i = 0; i < scrambledIndices.Length; i++)
        scrambledIndices[i] = i;
      bool[] moved = new bool[scrambledIndices.Length];
      bool changed = false;
      do {
        for (int i = scrambledIndices.Length - 1; i > 0; i--) {
          int j = random.Next(i + 1);
          int t = scrambledIndices[j];
          scrambledIndices[j] = scrambledIndices[i];
          scrambledIndices[i] = t;
          if (scrambledIndices[j] == j) moved[j] = false;
          else moved[j] = true;
          if (scrambledIndices[i] == i) moved[i] = false;
          else moved[i] = true;
        }
        changed = moved.Any(x => x);
      } while (!changed);

      return new ScrambleMove(breakPoint1, scrambledIndices);
    }

    public static ScrambleMove[] Apply(Permutation permutation, IRandom random, int sampleSize) {
      int length = permutation.Length;
      ScrambleMove[] moves = new ScrambleMove[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        moves[i] = GenerateRandomMove(permutation, random);
      }
      return moves;
    }

    protected override ScrambleMove[] GenerateMoves(Permutation permutation) {
      IRandom random = RandomParameter.ActualValue;
      return Apply(permutation, random, SampleSizeParameter.ActualValue.Value);
    }
  }
}
