#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  /// An operator that relinks paths between knapsack solutions.
  /// </summary>
  /// <remarks>
  /// The operator incrementally assimilates the initiating solution into the guiding solution by adding and removing elements as needed.
  /// </remarks>
  [Item("KnapsackPathRelinker", "An operator that relinks paths between knapsack solutions. The operator incrementally assimilates the initiating solution into the guiding solution by adding and removing elements as needed.")]
  [StorableClass]
  public sealed class KnapsackPathRelinker : SingleObjectivePathRelinker {
    [StorableConstructor]
    private KnapsackPathRelinker(bool deserializing) : base(deserializing) { }
    private KnapsackPathRelinker(KnapsackPathRelinker original, Cloner cloner) : base(original, cloner) { }
    public KnapsackPathRelinker() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new KnapsackPathRelinker(this, cloner);
    }

    public static ItemArray<IItem> Apply(IItem initiator, IItem guide, PercentValue n) {
      if (!(initiator is BinaryVector) || !(guide is BinaryVector))
        throw new ArgumentException("Cannot relink path because one of the provided solutions or both have the wrong type.");
      if (n.Value <= 0.0)
        throw new ArgumentException("RelinkingAccuracy must be greater than 0.");

      BinaryVector v1 = initiator.Clone() as BinaryVector;
      BinaryVector v2 = guide as BinaryVector;

      if (v1.Length != v2.Length)
        throw new ArgumentException("The solutions are of different length.");

      IList<BinaryVector> solutions = new List<BinaryVector>();
      for (int i = 0; i < v1.Length; i++)
        if (v1[i] != v2[i]) {
          v1[i] = v2[i];
          solutions.Add(v1.Clone() as BinaryVector);
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
