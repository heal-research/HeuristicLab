#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Optimization;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("StochasticSwap2SingleMoveGenerator", "Randomly samples a single from all possible swap-2 moves from a given lle grouping.")]
  [StorableClass]
  public class StochasticSwap2SingleMoveGenerator : Swap2MoveGenerator, IStochasticOperator, ISingleMoveGenerator {
    public ILookupParameter<IRandom> RandomParameter {
      get { return (ILookupParameter<IRandom>)Parameters["Random"]; }
    }

    [StorableConstructor]
    protected StochasticSwap2SingleMoveGenerator(bool deserializing) : base(deserializing) { }
    protected StochasticSwap2SingleMoveGenerator(StochasticSwap2SingleMoveGenerator original, Cloner cloner) : base(original, cloner) { }
    public StochasticSwap2SingleMoveGenerator()
      : base() {
      Parameters.Add(new LookupParameter<IRandom>("Random", "The random number generator."));
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new StochasticSwap2SingleMoveGenerator(this, cloner);
    }

    public static Swap2Move Apply(LinearLinkage lle, IRandom random) {
      int length = lle.Length;
      if (length < 2) throw new ArgumentException("StochasticSwap2SingleMoveGenerator: There cannot be a swap-2 move given only one item.", "lle");

      var groups = lle.GetGroups().ToList();
      if (groups.Count == 1) throw new InvalidOperationException("StochasticSwap2SingleMoveGenerator: Swap moves cannot be applied when there is only one group.");

      int index1 = random.Next(groups.Count), index2 = 0;
      do {
        index2 = random.Next(length);
      } while (index1 == index2);

      var item1 = random.Next(groups[index1].Count);
      var item2 = random.Next(groups[index2].Count);

      return new Swap2Move(groups[index1][item1], groups[index2][item2]);
    }

    protected override Swap2Move[] GenerateMoves(LinearLinkage lle) {
      IRandom random = RandomParameter.ActualValue;
      return new[] { Apply(lle, random) };
    }
  }
}
