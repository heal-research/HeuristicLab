#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Generic;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.Optimization {
  [Item("Optimizer List", "Represents a list of optimizers.")]
  [StorableType("7C68A9CF-5FC6-46BF-BB75-B9CB8102539E")]
  public class OptimizerList : ItemList<IOptimizer> {
    public OptimizerList() : base() { }
    public OptimizerList(int capacity) : base(capacity) { }
    public OptimizerList(IEnumerable<IOptimizer> collection) : base(collection) { }
    [StorableConstructor]
    protected OptimizerList(StorableConstructorFlag _) : base(_) { }
    protected OptimizerList(OptimizerList original, Cloner cloner)
      : base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OptimizerList(this, cloner);
    }
  }
}
