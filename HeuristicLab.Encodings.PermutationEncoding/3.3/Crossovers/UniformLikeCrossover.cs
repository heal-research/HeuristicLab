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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.PermutationEncoding {
  [Item("UniformLikeCrossover", "The ULX crossover tries to maintain the position in the permutation. It randomly chooses from left to right one of its parents' alleles at each position. Missing entries are then filled randomly later. It is described in Tate, D. M. and Smith, A. E. 1995. A genetic approach to the quadratic assignment problem. Computers & Operations Research, vol. 22, pp. 73-83.")]
  [StorableType("5FCF5C48-80A1-4A08-9561-F01FDCB8D20D")]
  public sealed class UniformLikeCrossover : PermutationCrossover {
    [StorableConstructor]
    private UniformLikeCrossover(StorableConstructorFlag _) : base(_) { }
    private UniformLikeCrossover(UniformLikeCrossover original, Cloner cloner) : base(original, cloner) { }
    public UniformLikeCrossover() : base() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new UniformLikeCrossover(this, cloner);
    }

    public static Permutation Apply(IRandom random, Permutation parent1, Permutation parent2) {
      if (parent1.Length != parent2.Length) throw new ArgumentException("parent1 and parent2 are of different length.");
      int length = parent1.Length;
      int[] child = new int[length];
      bool[] numberCopied = new bool[length];
      List<int> unoccupied = new List<int>();

      for (int i = 0; i < length; i++) {
        bool copied = true;
        if (parent1[i] == parent2[i]) {
          child[i] = parent1[i];
        } else if (!numberCopied[parent1[i]] && !numberCopied[parent2[i]]) {
          child[i] = (random.NextDouble() < 0.5) ? parent1[i] : parent2[i];
        } else if (!numberCopied[parent1[i]]) {
          child[i] = parent1[i];
        } else if (!numberCopied[parent2[i]]) {
          child[i] = parent2[i];
        } else {
          copied = false;
          unoccupied.Add(i);
        }
        if (copied) numberCopied[child[i]] = true;
      }

      if (unoccupied.Count > 0) {
        for (int i = 0; i < length; i++) {
          if (!numberCopied[i]) {
            int r = random.Next(unoccupied.Count);
            child[unoccupied[r]] = i;
            unoccupied.RemoveAt(r);
          }
          if (unoccupied.Count == 0) break;
        }
      }

      return new Permutation(parent1.PermutationType, child);
    }

    protected override Permutation Cross(IRandom random, ItemArray<Permutation> parents) {
      if (parents.Length != 2) throw new InvalidOperationException("UniformLikeCrossover: The number of parents is not equal to 2.");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
