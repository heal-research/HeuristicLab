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

using System;
using System.Collections.Generic;
using System.Text;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  public class ExecutionContext : DeepCloneable, IExecutionSequence {
    [Storable]
    private ExecutionContext parent;
    public ExecutionContext Parent {
      get { return parent; }
    }

    [Storable]
    private IOperator op;
    public IOperator Operator {
      get { return op; }
    }

    [Storable]
    private IScope scope;
    public IScope Scope {
      get { return scope; }
    }

    private ExecutionContext() {
      parent = null;
      op = null;
      scope = null;
    }
    public ExecutionContext(ExecutionContext parent, IOperator op, IScope scope) {
      if ((op == null) || (scope == null)) throw new ArgumentNullException();
      this.parent = parent;
      this.op = op;
      this.scope = scope;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      ExecutionContext clone = new ExecutionContext();
      cloner.RegisterClonedObject(this, clone);
      clone.parent = (ExecutionContext)cloner.Clone(parent);
      clone.op = (IOperator)cloner.Clone(op);
      clone.scope = (IScope)cloner.Clone(scope);
      return clone;
    }
  }
}
