#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Joseph Helm and Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;
using HeuristicLab.Problems.BinPacking;

namespace HeuristicLab.Problems.BinPacking2D {
  [Item("Bin Packing Solution (2d)", "Represents a solution for a 2D bin packing problem.")]
  [StorableType("9C014DEE-B1B3-447F-885E-5887E29DADF1")]
  public class Solution : PackingPlan<PackingPosition, PackingShape, PackingItem> {
    public Solution(PackingShape binShape) : this(binShape, false, false) { }
    public Solution(PackingShape binShape, bool useExtremePoints, bool stackingConstraints) : base(binShape, useExtremePoints, stackingConstraints) { }
    [StorableConstructor]
    protected Solution(StorableConstructorFlag _) : base(_) { }
    protected Solution(Solution original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Solution(this, cloner);
    }
  }
}