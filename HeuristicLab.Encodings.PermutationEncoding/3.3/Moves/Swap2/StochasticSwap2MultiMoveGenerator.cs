#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HEAL.Attic;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("StochasticSwap2MultiMoveGenerator", "Randomly samples n from all possible swap-2 moves from a given permutation.")]
  [StorableType("07B2AFDA-01BB-4317-9BFB-7D596CE43D1A")]
  public class StochasticSwap2MultiMoveGenerator : Swap2MoveGenerator, IMultiMoveGenerator, IStochasticOperator {
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
    protected StochasticSwap2MultiMoveGenerator(StorableConstructorFlag _) : base(_) { }
    protected StochasticSwap2MultiMoveGenerator(StochasticSwap2MultiMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticSwap2MultiMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticSwap2MultiMoveGenerator(this, cloner);
    }

    public static Swap2Move[] Apply(Permutation permutation, IRandom random, int sampleSize) {
      int length = permutation.Length;
      Swap2Move[] moves = new Swap2Move[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        moves[i] = StochasticSwap2SingleMoveGenerator.Apply(permutation, random);
      }
      return moves;
    }

    protected override Swap2Move[] GenerateMoves(Permutation permutation) {
      IRandom random = RandomParameter.ActualValue;
      return Apply(permutation, random, SampleSizeParameter.ActualValue.Value);
    }
  }
}
