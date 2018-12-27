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

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Encodings.BinaryVectorEncoding;
using HeuristicLab.Optimization.Operators;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Problems.Knapsack {
  /// <summary>
  /// An operator that relinks paths between knapsack solutions starting from both ends.
  /// </summary>
  /// <remarks>
  /// The operator incrementally assimilates the initiating solution into the guiding solution and vice versa by adding and removing elements as needed.
  /// </remarks>
  [Item("KnapsackSimultaneousPathRelinker", "An operator that relinks paths between knapsack solutions starting from both ends. The operator incrementally assimilates the initiating solution into the guiding solution and vice versa by adding and removing elements as needed.")]
  [StorableClass]
  public sealed class KnapsackSimultaneousPathRelinker : SingleObjectivePathRelinker {
    [StorableConstructor]
    private KnapsackSimultaneousPathRelinker(bool deserializing) : base(deserializing) { }
    private KnapsackSimultaneousPathRelinker(KnapsackSimultaneousPathRelinker original, Cloner cloner) : base(original, cloner) { }
    public KnapsackSimultaneousPathRelinker() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KnapsackSimultaneousPathRelinker(this, cloner);
    }

    public static ItemArray<IItem> Apply(IItem initiator, IItem guide, PercentValue n) {
      if (!(initiator is BinaryVector) || !(guide is BinaryVector))
        throw new ArgumentException("Cannot relink path because one of the provided solutions or both have the wrong type.");
      if (n.Value <= 0.0)
        throw new ArgumentException("RelinkingAccuracy must be greater than 0.");

      BinaryVector firstInitiator = initiator.Clone() as BinaryVector;
      BinaryVector firstGuide = guide.Clone() as BinaryVector;
      BinaryVector secondInitiator = firstGuide.Clone() as BinaryVector;
      BinaryVector secondGuide = firstInitiator.Clone() as BinaryVector;

      if (firstInitiator.Length != firstGuide.Length)
        throw new ArgumentException("The solutions are of different length.");

      IList<BinaryVector> solutions = new List<BinaryVector>();
      for (int i = 0; i < firstInitiator.Length / 2; i++) {
        if (firstInitiator[i] != firstGuide[i]) {
          firstInitiator[i] = firstGuide[i];
          solutions.Add(firstInitiator.Clone() as BinaryVector);
        }
        int j = secondInitiator.Length - 1 - i;
        if (secondInitiator[j] != secondGuide[j]) {
          secondInitiator[j] = secondGuide[j];
          solutions.Add(secondInitiator.Clone() as BinaryVector);
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
