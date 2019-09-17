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

using System;
using System.Threading;
using HEAL.Attic;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Parameters {
  [Item("ContextParameter", "")]
  [StorableType("b4f6a624-19b8-46aa-b89b-6bba2f2e2491")]
  public abstract class ContextParameter : Parameter, IContextParameter, IStatefulItem {

    protected Lazy<ThreadLocal<IItem>> cachedActualValues;
    protected virtual IItem CachedActualValue {
      get { return cachedActualValues.Value.Value; }
      set { cachedActualValues.Value.Value = value; }
    }

    protected Lazy<ThreadLocal<IExecutionContext>> executionContexts;
    public virtual IExecutionContext ExecutionContext {
      get { return executionContexts.Value.Value; }
      set {
        if (value != executionContexts.Value.Value) {
          executionContexts.Value.Value = value;
          cachedActualValues.Value.Value = null;
        }
      }
    }

    [StorableConstructor]
    protected ContextParameter(StorableConstructorFlag _) : base(_) {
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    protected ContextParameter(ContextParameter original, Cloner cloner)
      : base(original, cloner) {
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    protected ContextParameter() : this("Anonymous", string.Empty, typeof(IItem)) { }
    protected ContextParameter(string name, Type dataType) : this(name, string.Empty, dataType) { }
    protected ContextParameter(string name, string description, Type dataType)
      : base(name, description, dataType) {
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    protected override IItem GetActualValue() {
      if (CachedActualValue != null) return CachedActualValue;
      CachedActualValue = GetActualValueFromContext();
      return CachedActualValue;
    }

    protected abstract IItem GetActualValueFromContext();

    public virtual void InitializeState() {
    }
    public virtual void ClearState() {
      if (cachedActualValues.IsValueCreated) {
        cachedActualValues.Value.Dispose();
        cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      }
      if (executionContexts.IsValueCreated) {
        executionContexts.Value.Dispose();
        executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      }
    }
  }
}
