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

using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Attic;

namespace HeuristicLab.Problems.NK {
  [Item("RandomInteractionsInitializer", "Randomly assignes interactions across all bits")]
  [StorableType("ACB3BE45-DB9E-4200-95BF-D22C726B4DAE")]
  public sealed class RandomInteractionsInitializer : ParameterizedNamedItem, IInteractionInitializer {
    [StorableConstructor]
    private RandomInteractionsInitializer(StorableConstructorFlag _) : base(_) { }
    private RandomInteractionsInitializer(RandomInteractionsInitializer original, Cloner cloner) : base(original, cloner) { }
    public RandomInteractionsInitializer() { }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new RandomInteractionsInitializer(this, cloner);
    }

    public BoolMatrix InitializeInterations(int length, int nComponents, int nInteractions, IRandom random) {
      BoolMatrix m = new BoolMatrix(length, nComponents);
      for (int c = 0; c < m.Columns; c++) {
        var indices = Enumerable.Range(0, length).ToList();
        if (indices.Count > c) {
          indices.RemoveAt(c);
          m[c, c] = true;
        }
        while (indices.Count > nInteractions) {
          indices.RemoveAt(random.Next(indices.Count));
        }
        foreach (var i in indices) {
          m[i, c] = true;
        }
      }
      return m;
    }
  }
}
