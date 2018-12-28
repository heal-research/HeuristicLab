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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.Encodings.LinearLinkageEncoding {
  [Item("Single Point Crossover", "The single point crossover concatenates two linkage strings at a randomly chosen point.")]
  [StorableType("1F1A83F4-659C-4A88-9A0C-4507257A3741")]
  public sealed class SinglePointCrossover : LinearLinkageCrossover {

    [StorableConstructor]
    private SinglePointCrossover(StorableConstructorFlag _) : base(_) { }
    private SinglePointCrossover(SinglePointCrossover original, Cloner cloner) : base(original, cloner) { }
    public SinglePointCrossover() { }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new SinglePointCrossover(this, cloner);
    }

    public static LinearLinkage Apply(IRandom random, LinearLinkage p1, LinearLinkage p2) {
      var length = p1.Length;
      var child = LinearLinkage.SingleElementGroups(length);
      var bp = random.Next(length - 1);
      for (var i = 0; i <= bp; i++) child[i] = p1[i];
      for (var i = bp + 1; i < length; i++) child[i] = p2[i];
      child.LinearizeTreeStructures();
      return child;
    }

    protected override LinearLinkage Cross(IRandom random, ItemArray<LinearLinkage> parents) {
      if (parents.Length != 2) throw new InvalidOperationException(Name + ": Can only cross exactly two parents.");
      return Apply(random, parents[0], parents[1]);
    }
  }
}
