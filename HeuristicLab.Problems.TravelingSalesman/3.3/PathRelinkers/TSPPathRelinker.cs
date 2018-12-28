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
using HeuristicLab.Encodings.PermutationEncoding;
using HeuristicLab.Optimization.Operators;
using HEAL.Fossil;

namespace HeuristicLab.Problems.TravelingSalesman {
  /// <summary>
  /// An operator that relinks paths between traveling salesman solutions.
  /// </summary>
  /// <remarks>
  /// The operator incrementally assimilates the initiating solution into the guiding solution by correcting edges as needed.
  /// </remarks>
  [Item("TSPPathRelinker", "An operator that relinks paths between traveling salesman solutions. The operator incrementally assimilates the initiating solution into the guiding solution by correcting edges as needed.")]
  [StorableType("0997A24C-0592-4CE4-A681-70C97890D3F7")]
  public sealed class TSPPathRelinker : SingleObjectivePathRelinker {
    [StorableConstructor]
    private TSPPathRelinker(StorableConstructorFlag _) : base(_) { }
    private TSPPathRelinker(TSPPathRelinker original, Cloner cloner) : base(original, cloner) { }
    public TSPPathRelinker() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new TSPPathRelinker(this, cloner);
    }

    public static ItemArray<IItem> Apply(IItem initiator, IItem guide, PercentValue n) {
      if (!(initiator is Permutation) || !(guide is Permutation))
        throw new ArgumentException("Cannot relink path because one of the provided solutions or both have the wrong type.");
      if (n.Value <= 0.0)
        throw new ArgumentException("RelinkingAccuracy must be greater than 0.");

      Permutation v1 = initiator.Clone() as Permutation;
      Permutation v2 = guide as Permutation;

      if (v1.Length != v2.Length)
        throw new ArgumentException("The solutions are of different length.");

      IList<Permutation> solutions = new List<Permutation>();
      for (int i = 0; i < v1.Length; i++)
        if (v1[i] != v2[i]) {
          var target = v1.Select((x, index) => new { Value = x, ValueIndex = index }).First(x => x.Value == v2[i]);
          if (v1[i] != v1[target.ValueIndex]) {
            // XOR swap
            v1[i] ^= v1[target.ValueIndex];
            v1[target.ValueIndex] ^= v1[i];
            v1[i] ^= v1[target.ValueIndex];
            solutions.Add(v1.Clone() as Permutation);
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
