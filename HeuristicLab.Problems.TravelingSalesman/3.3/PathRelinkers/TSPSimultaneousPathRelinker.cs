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
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator that relinks paths between traveling salesman solutions starting from both ends.
  /// </summary>
  /// <remarks>
  /// The operator incrementally assimilates the initiating solution into the guiding solution and vice versa by correcting edges as needed.
  /// </remarks>
  [Item("TSPSimultaneousPathRelinker", "An operator that relinks paths between traveling salesman solutions starting from both ends. The operator incrementally assimilates the initiating solution into the guiding solution and vice versa by correcting edges as needed.")]
  [StorableClass]
  public sealed class TSPSimultaneousPathRelinker : SingleObjectivePathRelinker {
    [StorableConstructor]
    private TSPSimultaneousPathRelinker(bool deserializing) : base(deserializing) { }
    private TSPSimultaneousPathRelinker(TSPSimultaneousPathRelinker original, Cloner cloner) : base(original, cloner) { }
    public TSPSimultaneousPathRelinker() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPSimultaneousPathRelinker(this, cloner);
    }

    public static ItemArray<IItem> Apply(IItem initiator, IItem guide, PercentValue n) {
      if (!(initiator is Permutation) || !(guide is Permutation))
        throw new ArgumentException("Cannot relink path because one of the provided solutions or both have the wrong type.");
      if (n.Value <= 0.0)
        throw new ArgumentException("RelinkingAccuracy must be greater than 0.");

      Permutation firstInitiator = initiator.Clone() as Permutation;
      Permutation firstGuide = guide.Clone() as Permutation;
      Permutation secondInitiator = firstGuide.Clone() as Permutation;
      Permutation secondGuide = firstInitiator.Clone() as Permutation;

      if (firstInitiator.Length != firstGuide.Length)
        throw new ArgumentException("The solutions are of different length.");

      IList<Permutation> solutions = new List<Permutation>();
      for (int i = 0; i < firstInitiator.Length / 2; i++) {
        if (firstInitiator[i] != firstGuide[i]) {
          var target = firstInitiator.Select((x, index) => new { Value = x, ValueIndex = index }).First(x => x.Value == firstGuide[i]);
          // XOR swap
          firstInitiator[i] ^= firstInitiator[target.ValueIndex];
          firstInitiator[target.ValueIndex] ^= firstInitiator[i];
          firstInitiator[i] ^= firstInitiator[target.ValueIndex];
          solutions.Add(firstInitiator.Clone() as Permutation);
        }
        int j = secondInitiator.Length - 1 - i;
        if (secondInitiator[j] != secondGuide[j]) {
          var target = secondInitiator.Select((x, index) => new { Value = x, ValueIndex = index }).First(x => x.Value == secondGuide[j]);
          // XOR swap
          secondInitiator[j] ^= secondInitiator[target.ValueIndex];
          secondInitiator[target.ValueIndex] ^= secondInitiator[j];
          secondInitiator[j] ^= secondInitiator[target.ValueIndex];
          solutions.Add(secondInitiator.Clone() as Permutation);
        }
      }

      IList<IItem> selection = new List<IItem>();
      if (solutions.Count > 0) {
        int noSol = (int)(solutions.Count * n.Value);
        if (noSol <= 0) noSol++;
        double stepSize = (double)solutions.Count / (double)noSol;
        for (int i = 0; i < noSol; i++)
          selection.Add(solutions.ElementAt((int)((i + 1) * stepSize - stepSize * 0.5)));
      }

      return new ItemArray<IItem>(selection);
    }

    protected override ItemArray<IItem> Relink(ItemArray<IItem> parents, PercentValue n) {
      if (parents.Length != 2)
        throw new ArgumentException("The number of parents is not equal to 2.");
      return Apply(parents[0], parents[1], n);
    }
  }
}
