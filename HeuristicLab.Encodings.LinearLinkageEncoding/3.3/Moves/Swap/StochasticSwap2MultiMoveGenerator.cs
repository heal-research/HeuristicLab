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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("StochasticSwap2MultiMoveGenerator", "Randomly samples n from all possible swap-2 moves from a given lle grouping.")]
  [StorableClass]
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
    protected StochasticSwap2MultiMoveGenerator(bool deserializing) : base(deserializing) { }
    protected StochasticSwap2MultiMoveGenerator(StochasticSwap2MultiMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticSwap2MultiMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
      Parameters.Add(new ValueLookupParameter<IntValue>("SampleSize", "The number of moves to generate."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticSwap2MultiMoveGenerator(this, cloner);
    }

    public static Swap2Move[] Apply(LinearLinkage lle, IRandom random, int sampleSize) {
      int length = lle.Length;
      Swap2Move[] moves = new Swap2Move[sampleSize];
      for (int i = 0; i < sampleSize; i++) {
        moves[i] = StochasticSwap2SingleMoveGenerator.Apply(lle, random);
      }
      return moves;
    }

    protected override Swap2Move[] GenerateMoves(LinearLinkage lle) {
      IRandom random = RandomParameter.ActualValue;
      return Apply(lle, random, SampleSizeParameter.ActualValue.Value);
    }
  }
}
