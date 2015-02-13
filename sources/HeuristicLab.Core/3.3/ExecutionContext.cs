#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [StorableClass]
  public sealed class ExecutionContext : DeepCloneable, IExecutionContext, IAtomicOperation {
    [Storable]
    private IParameterizedItem parameterizedItem;

    [Storable]
    private IExecutionContext parent;
    public IExecutionContext Parent {
      get { return parent; }
    }

    public IKeyedItemCollection<string, IParameter> Parameters {
      get { return parameterizedItem.Parameters; }
    }

    public IOperator Operator {
      get { return parameterizedItem as IOperator; }
    }

    [Storable]
    private IScope scope;
    public IScope Scope {
      get { return scope; }
    }

    [StorableConstructor]
    private ExecutionContext(bool deserializing) { }
    private ExecutionContext(ExecutionContext original, Cloner cloner)
      : base(original, cloner) {
      parent = cloner.Clone(original.parent);
      parameterizedItem = cloner.Clone(original.parameterizedItem);
      scope = cloner.Clone(original.scope);
    }
    private ExecutionContext() {
      parent = null;
      parameterizedItem = null;
      scope = null;
    }
    public ExecutionContext(IExecutionContext parent, IParameterizedItem parameterizedItem, IScope scope) {
      if ((parameterizedItem == null) || (scope == null)) throw new ArgumentNullException();
      this.parent = parent;
      this.parameterizedItem = parameterizedItem;
      this.scope = scope;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ExecutionContext(this, cloner);
    }

    public IAtomicOperation CreateOperation(IOperator op) {
      return new ExecutionContext(parent, op, scope);
    }
    public IAtomicOperation CreateOperation(IOperator op, IScope scope) {
      return new ExecutionContext(parent, op, scope);
    }
    public IAtomicOperation CreateChildOperation(IOperator op) {
      return new ExecutionContext(this, op, scope);
    }
    public IAtomicOperation CreateChildOperation(IOperator op, IScope scope) {
      return new ExecutionContext(this, op, scope);
    }
  }
}
