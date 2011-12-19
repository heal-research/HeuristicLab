#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Parameters {
  /// <summary>
  /// A base class for parameters.
  /// </summary>
  [Item("Parameter", "A base class for parameters.")]
  [StorableClass]
  public abstract class Parameter : NamedItem, IParameter, IStatefulItem {
    public override Image ItemImage {
      get {
        if ((dataType != null) && (typeof(IOperator).IsAssignableFrom(dataType)))
          return HeuristicLab.Common.Resources.VSImageLibrary.Method;
        else
          return base.ItemImage;
      }
    }
    public override bool CanChangeName {
      get { return false; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    [Storable]
    private Type dataType;
    public Type DataType {
      get { return dataType; }
    }

    [Storable(DefaultValue = false)]
    private bool hidden;
    public bool Hidden {
      get { return hidden; }
      set {
        if (value != hidden) {
          hidden = value;
          OnHiddenChanged();
        }
      }
    }

    private Lazy<ThreadLocal<IItem>> cachedActualValues;
    public IItem ActualValue {
      get {
        if (cachedActualValues.Value.Value == null) cachedActualValues.Value.Value = GetActualValue();
        return cachedActualValues.Value.Value;
      }
      set {
        cachedActualValues.Value.Value = value;
        SetActualValue(value);
      }
    }
    private Lazy<ThreadLocal<IExecutionContext>> executionContexts;
    public IExecutionContext ExecutionContext {
      get { return executionContexts.Value.Value; }
      set {
        if (value != executionContexts.Value.Value) {
          executionContexts.Value.Value = value;
          cachedActualValues.Value.Value = null;
        }
      }
    }

    [StorableConstructor]
    protected Parameter(bool deserializing)
      : base(deserializing) {
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    protected Parameter(Parameter original, Cloner cloner)
      : base(original, cloner) {
      dataType = original.dataType;
      hidden = original.hidden;
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    protected Parameter()
      : base("Anonymous") {
      dataType = typeof(IItem);
      hidden = false;
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    protected Parameter(string name, Type dataType)
      : base(name) {
      if (dataType == null) throw new ArgumentNullException();
      this.dataType = dataType;
      hidden = false;
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    protected Parameter(string name, string description, Type dataType)
      : base(name, description) {
      if (dataType == null) throw new ArgumentNullException();
      this.dataType = dataType;
      hidden = false;
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public virtual void InitializeState() { }
    public virtual void ClearState() {
      cachedActualValues = new Lazy<ThreadLocal<IItem>>(() => { return new ThreadLocal<IItem>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public override string ToString() {
      return Name;
    }

    protected abstract IItem GetActualValue();
    protected abstract void SetActualValue(IItem value);

    public event EventHandler HiddenChanged;
    protected virtual void OnHiddenChanged() {
      EventHandler handler = HiddenChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }
}
