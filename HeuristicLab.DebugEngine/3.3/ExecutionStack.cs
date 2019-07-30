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
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Attic;

namespace HeuristicLab.DebugEngine {

  [StorableType("FE481B4B-BA25-4889-AAC7-6C88D852D971")]
  public class ExecutionStack : ObservableList<IOperation>, IContent, IDeepCloneable {

    public ExecutionStack() : base() { }
    public ExecutionStack(int capacity) : base(capacity) { }
    public ExecutionStack(IEnumerable<IOperation> collection) : base(collection) { }

    [StorableConstructor]
    protected ExecutionStack(StorableConstructorFlag _) : base(_) { }
    protected ExecutionStack(ExecutionStack original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      AddRange(original.Select(op => cloner.Clone(op)));
    }
    public object Clone() {
      return Clone(new Cloner());
    }
    public virtual IDeepCloneable Clone(Cloner cloner) {
      return new ExecutionStack(this, cloner);
    }
  }
}
