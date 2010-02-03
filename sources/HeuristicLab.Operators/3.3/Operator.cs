#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Xml;
using System.Drawing;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Core;
using HeuristicLab.Collections;

namespace HeuristicLab.Operators {
  /// <summary>
  /// The base class for all operators.
  /// </summary>
  [Item("Operator", "Base class for operators.")]
  public abstract class Operator : NamedItem, IOperator {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Method; }
    }
    public override bool CanChangeDescription {
      get { return false; }
    }

    private ParameterCollection parameters;
    [Storable]
    protected ParameterCollection Parameters {
      get { return parameters;}
      private set {
        if (parameters != null) parameters.Changed -= new ChangedEventHandler(Parameters_Changed);
        parameters = value;
        readOnlyParameters = null;
        if (parameters != null) parameters.Changed += new ChangedEventHandler(Parameters_Changed);
      }
    }
    private ReadOnlyObservableKeyedCollection<string, IParameter> readOnlyParameters;
    IObservableKeyedCollection<string, IParameter> IOperator.Parameters {
      get {
        if (readOnlyParameters == null) readOnlyParameters = parameters.AsReadOnly();
        return readOnlyParameters;
      }
    }

    /// <summary>
    /// Flag whether the current instance has been canceled.
    /// </summary>
    private bool canceled;
    /// <inheritdoc/>
    protected bool Canceled {
      get { return canceled; }
    }

    [Storable]
    private bool breakpoint;
    /// <inheritdoc/>
    /// <remarks>Calls <see cref="OnBreakpointChanged"/> in the setter.</remarks>
    public bool Breakpoint {
      get { return breakpoint; }
      set {
        if (value != breakpoint) {
          breakpoint = value;
          OnBreakpointChanged();
        }
      }
    }

    [Storable]
    private ExecutionContext executionContext;
    protected ExecutionContext ExecutionContext {
      get { return executionContext; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorBase"/> setting the breakpoint flag and 
    /// the canceled flag to <c>false</c> and the name of the operator to the type name. 
    /// </summary>
    protected Operator() {
      name = ItemName;
      Parameters = new ParameterCollection();
      readOnlyParameters = null;
      canceled = false;
      breakpoint = false;
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Clones also sub operators, variables and variable infos.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="OperatorBase"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      Operator clone = (Operator)base.Clone(cloner);
      clone.Parameters = (ParameterCollection)cloner.Clone(parameters);
      clone.canceled = canceled;
      clone.breakpoint = breakpoint;
      clone.executionContext = (ExecutionContext)cloner.Clone(executionContext);
      return clone;
    }

    /// <inheritdoc/>
    public virtual ExecutionContextCollection Execute(ExecutionContext context) {
      try {
        canceled = false;
        executionContext = context;
        foreach (IParameter param in Parameters)
          param.ExecutionContext = context;
        ExecutionContextCollection next = Apply();
        OnExecuted();
        return next;
      }
      finally {
        foreach (IParameter param in Parameters)
          param.ExecutionContext = null;
        context = null;
      }
    }
    /// <inheritdoc/>
    /// <remarks>Sets property <see cref="Canceled"/> to <c>true</c>.</remarks>
    public virtual void Abort() {
      canceled = true;
    }
    /// <summary>
    /// Performs the current operator on the specified <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope where to execute the operator</param>
    /// <returns><c>null</c>.</returns>
    public virtual ExecutionContextCollection Apply() {
      return new ExecutionContextCollection();
    }

    /// <inheritdoc/>
    public event EventHandler BreakpointChanged;
    /// <summary>
    /// Fires a new <c>BreakpointChanged</c> event.
    /// </summary>
    protected virtual void OnBreakpointChanged() {
      if (BreakpointChanged != null) {
        BreakpointChanged(this, new EventArgs());
      }
      OnChanged();
    }
    /// <inheritdoc/>
    public event EventHandler Executed;
    /// <summary>
    /// Fires a new <c>Executed</c> event.
    /// </summary>
    protected virtual void OnExecuted() {
      if (Executed != null) {
        Executed(this, new EventArgs());
      }
    }

    private void Parameters_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
  }
}
