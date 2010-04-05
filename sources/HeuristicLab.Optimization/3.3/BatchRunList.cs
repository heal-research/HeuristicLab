#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [StorableClass]
  [Item("BatchRunList", "Represents a list of batch runs.")]
  public sealed class BatchRunList : ItemList<BatchRun> {
    public BatchRunList() : base() { }
    public BatchRunList(int capacity) : base(capacity) { }
    public BatchRunList(IEnumerable<BatchRun> collection) : base(collection) { }

    public override IDeepCloneable Clone(Cloner cloner) {
      BatchRunList clone = new BatchRunList(this.Select(x => (BatchRun)cloner.Clone(x)));
      cloner.RegisterClonedObject(this, clone);
      return clone;
    }
  }
}
