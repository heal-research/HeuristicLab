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
using System.Xml;
using System.Linq;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;
using HeuristicLab.Collections;

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents a graph of operators.
  /// </summary>
  [Item("OperatorGraph", "Represents a graph of operators.")]
  [Creatable("Algorithms")]
  public class OperatorGraph : Item {
    private OperatorSet operators;
    /// <summary>
    /// Gets all operators of the current instance.
    /// </summary>
    [Storable]
    public OperatorSet Operators {
      get { return operators; }
      private set {
        DeregisterOperatorsEvents();
        operators = value;
        RegisterOperatorsEvents();
      }
    }

    [Storable]
    private IOperator initialOperator;
    /// <summary>
    /// Gets or sets the initial operator (the starting one).
    /// </summary>
    /// <remarks>Calls <see cref="OnInitialOperatorChanged"/> in the setter.</remarks>
    public IOperator InitialOperator {
      get { return initialOperator; }
      set {
        if (initialOperator != value) {
          if (value != null) Operators.Add(value);
          initialOperator = value;
          OnInitialOperatorChanged();
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraph"/>.
    /// </summary>
    public OperatorGraph() {
      Operators = new OperatorSet();
      initialOperator = null;
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="cloner.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="OperatorGraph"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      OperatorGraph clone = new OperatorGraph();
      cloner.RegisterClonedObject(this, clone);
      clone.Operators = (OperatorSet)cloner.Clone(operators);
      clone.initialOperator = (IOperator)cloner.Clone(initialOperator);
      return clone;
    }

    /// <inheritdoc/>
    public event EventHandler InitialOperatorChanged;
    /// <summary>
    /// Fires a new <c>InitialOperatorChanged</c> event.
    /// </summary>
    protected virtual void OnInitialOperatorChanged() {
      if (InitialOperatorChanged != null)
        InitialOperatorChanged(this, EventArgs.Empty);
      OnChanged();
    }

    #region Operators Events
    private void AddOperator(IOperator op) {
      RegisterOperatorEvents(op);
      foreach (IParameter param in op.Parameters)
        AddParameter(param);
    }
    private void RemoveOperator(IOperator op) {
      foreach (IParameter param in op.Parameters)
        RemoveParameter(param);
      DeregisterOperatorEvents(op);

      // remove edges to removed operator
      var opParams = from o in Operators
                     from p in o.Parameters
                     where p is IValueParameter<IOperator>
                     where (((IValueParameter<IOperator>)p).Value != null) && (((IValueParameter<IOperator>)p).Value == op)
                     select (IValueParameter<IOperator>)p;
      foreach (IValueParameter<IOperator> opParam in opParams)
        opParam.Value = null;
    }
    private void AddParameter(IParameter param) {
      IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
      if (opParam != null) {
        RegisterOperatorParameterEvents(opParam);
        if (opParam.Value != null) Operators.Add(opParam.Value);
      }
    }
    private void RemoveParameter(IParameter param) {
      IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
      if (opParam != null) {
        DeregisterOperatorParameterEvents(opParam);
      }
    }

    private void RegisterOperatorsEvents() {
      if (operators != null) {
        operators.Changed += new ChangedEventHandler(Operators_Changed);
        operators.ItemsAdded += new CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsAdded);
        operators.ItemsRemoved += new CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsRemoved);
        operators.CollectionReset += new CollectionItemsChangedEventHandler<IOperator>(Operators_CollectionReset);
        foreach (IOperator op in operators) {
          RegisterOperatorEvents(op);
          foreach (IParameter param in op.Parameters) {
            IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
            if (opParam != null) RegisterOperatorParameterEvents(opParam);
          }
        }
      }
    }
    private void DeregisterOperatorsEvents() {
      if (operators != null) {
        foreach (IOperator op in operators) {
          foreach (IParameter param in op.Parameters) {
            IValueParameter<IOperator> opParam = param as IValueParameter<IOperator>;
            if (opParam != null) DeregisterOperatorParameterEvents(opParam);
          }
          DeregisterOperatorEvents(op);
        }
        operators.Changed -= new ChangedEventHandler(Operators_Changed);
        operators.ItemsAdded -= new CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsAdded);
        operators.ItemsRemoved -= new CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsRemoved);
        operators.CollectionReset -= new CollectionItemsChangedEventHandler<IOperator>(Operators_CollectionReset);
      }
    }
    private void RegisterOperatorEvents(IOperator op) {
      op.Parameters.ItemsAdded += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsAdded);
      op.Parameters.ItemsRemoved += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsRemoved);
      op.Parameters.ItemsReplaced += new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsReplaced);
      op.Parameters.CollectionReset += new CollectionItemsChangedEventHandler<IParameter>(Parameters_CollectionReset);
    }
    private void DeregisterOperatorEvents(IOperator op) {
      op.Parameters.ItemsAdded -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsAdded);
      op.Parameters.ItemsRemoved -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsRemoved);
      op.Parameters.ItemsReplaced -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_ItemsReplaced);
      op.Parameters.CollectionReset -= new CollectionItemsChangedEventHandler<IParameter>(Parameters_CollectionReset);
    }
    private void RegisterOperatorParameterEvents(IValueParameter<IOperator> opParam) {
      opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
    }
    private void DeregisterOperatorParameterEvents(IValueParameter<IOperator> opParam) {
      opParam.ValueChanged -= new EventHandler(opParam_ValueChanged);
    }

    private void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IOperator> e) {
      foreach (IOperator op in e.Items)
        AddOperator(op);
    }
    private void Operators_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IOperator> e) {
      foreach (IOperator op in e.Items)
        RemoveOperator(op);
      if (!Operators.Contains(InitialOperator)) InitialOperator = null;
    }
    private void Operators_CollectionReset(object sender, CollectionItemsChangedEventArgs<IOperator> e) {
      foreach (IOperator op in e.OldItems)
        RemoveOperator(op);
      foreach (IOperator op in e.Items)
        AddOperator(op);
      if (!Operators.Contains(InitialOperator)) InitialOperator = null;
    }
    private void Parameters_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      foreach (IParameter param in e.Items)
        AddParameter(param);
    }
    private void Parameters_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      foreach (IParameter param in e.Items)
        RemoveParameter(param);
    }
    private void Parameters_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      foreach (IParameter param in e.OldItems)
        RemoveParameter(param);
      foreach (IParameter param in e.Items)
        AddParameter(param);
    }
    private void Parameters_CollectionReset(object sender, CollectionItemsChangedEventArgs<IParameter> e) {
      foreach (IParameter param in e.OldItems)
        RemoveParameter(param);
      foreach (IParameter param in e.Items)
        AddParameter(param);
    }
    private void opParam_ValueChanged(object sender, EventArgs e) {
      IValueParameter<IOperator> opParam = (IValueParameter<IOperator>)sender;
      if (opParam.Value != null) Operators.Add(opParam.Value);
    }
    private void Operators_Changed(object sender, ChangedEventArgs e) {
      OnChanged(e);
    }
    #endregion
  }
}
