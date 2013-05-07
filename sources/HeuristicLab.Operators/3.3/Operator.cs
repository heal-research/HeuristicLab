#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Threading;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// Base class for operators.
  /// </summary>
  [Item("Operator", "Base class for operators.")]
  [StorableClass]
  public abstract class Operator : ParameterizedNamedItem, IOperator, IStatefulItem {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Method; }
    }
    public override Image ItemImage {
      get {
        if (Breakpoint) return HeuristicLab.Common.Resources.VSImageLibrary.BreakpointActive;
        else return base.ItemImage;
      }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    private Lazy<ThreadLocal<IExecutionContext>> executionContexts;
    protected IExecutionContext ExecutionContext {
      get { return executionContexts.Value.Value; }
      private set {
        if (value != executionContexts.Value.Value) {
          executionContexts.Value.Value = value;
        }
      }
    }
    private CancellationToken cancellationToken;
    protected CancellationToken CancellationToken {
      get { return cancellationToken; }
    }

    [Storable]
    private bool breakpoint;
    public bool Breakpoint {
      get { return breakpoint; }
      set {
        if (value != breakpoint) {
          breakpoint = value;
          OnBreakpointChanged();
          OnItemImageChanged();
        }
      }
    }

    [StorableConstructor]
    protected Operator(bool deserializing)
      : base(deserializing) {
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }
    protected Operator(Operator original, Cloner cloner)
      : base(original, cloner) {
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      this.breakpoint = original.breakpoint;
    }
    protected Operator()
      : base() {
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      breakpoint = false;
    }
    protected Operator(string name)
      : base(name) {
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      breakpoint = false;
    }
    protected Operator(string name, ParameterCollection parameters)
      : base(name, parameters) {
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      breakpoint = false;
    }
    protected Operator(string name, string description)
      : base(name, description) {
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      breakpoint = false;
    }
    protected Operator(string name, string description, ParameterCollection parameters)
      : base(name, description, parameters) {
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
      breakpoint = false;
    }

    public virtual void InitializeState() { }
    public virtual void ClearState() {
      executionContexts = new Lazy<ThreadLocal<IExecutionContext>>(() => { return new ThreadLocal<IExecutionContext>(); }, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public virtual IOperation Execute(IExecutionContext context, CancellationToken cancellationToken) {
      try {
        ExecutionContext = context;
        this.cancellationToken = cancellationToken;
        foreach (ILookupParameter param in Parameters.OfType<ILookupParameter>())
          param.ExecutionContext = context;
        IOperation next = Apply();
        OnExecuted();
        return next;
      }
      finally {
        foreach (ILookupParameter param in Parameters.OfType<ILookupParameter>())
          param.ExecutionContext = null;
        ExecutionContext = null;
      }
    }
    public abstract IOperation Apply();

    public event EventHandler BreakpointChanged;
    protected virtual void OnBreakpointChanged() {
      if (BreakpointChanged != null) {
        BreakpointChanged(this, EventArgs.Empty);
      }
    }
    public event EventHandler Executed;
    protected virtual void OnExecuted() {
      if (Executed != null) {
        Executed(this, EventArgs.Empty);
      }
    }
  }
}
