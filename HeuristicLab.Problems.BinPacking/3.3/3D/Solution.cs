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

using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Problems.BinPacking;
using System;

namespace HeuristicLab.Problems.BinPacking3D {
  [Item("Bin Packing Solution (3d)", "Represents a solution for a 3D bin packing problem.")]
  [StorableClass]
  public class Solution : PackingPlan<PackingPosition, PackingShape, PackingItem> {
    public Solution(PackingShape binShape) : this(binShape, false, false) { }
    public Solution(PackingShape binShape, bool useExtremePoints, bool stackingConstraints) : base(binShape, useExtremePoints, stackingConstraints) { }
    [StorableConstructor]
    protected Solution(bool deserializing) : base(deserializing) { }
    protected Solution(Solution original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new Solution(this, cloner);
    }

    public bool IsBetterThan(Solution other, IEvaluator evaluator, bool problemMaximization = true) {
      var evaluatedThis = evaluator.Evaluate1(this);

      if (double.IsInfinity(evaluatedThis.Item2) || double.IsNaN(evaluatedThis.Item2)) {
        return false;
      }

      if (other == null) {
        return true;
      }

      var evaluatedOther = evaluator.Evaluate1(other);
      if (evaluatedThis.Item1 < evaluatedOther.Item1) {
        return true;
      } else if (evaluatedThis.Item1 > evaluatedOther.Item1) {
        return false;
      }
      
      if (evaluatedThis.Item2 > evaluatedOther.Item2) {
        return true;
      }
      if (evaluatedThis.Item2 < evaluatedOther.Item2) {
        return false;
      }

      if (evaluatedThis.Item4 > evaluatedOther.Item4) {
        return false;
      }

      if (evaluatedThis.Item3 > evaluatedOther.Item3) {
        return false;
      }
      return true;

    }
  }
}