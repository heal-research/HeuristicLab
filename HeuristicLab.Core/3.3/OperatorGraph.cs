#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  /// <summary>
  /// Represents a graph of operators.
  /// </summary>
  [Item("Operator Graph", "Represents a graph of operators.")]
  [StorableClass]
  public class OperatorGraph : Item, IStorableContent {
    public string Filename { get; set; }

    [Storable]
    private OperatorSet operators;
    /// <summary>
    /// Gets all operators of the current instance.
    /// </summary>
    public OperatorSet Operators {
      get { return operators; }
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

    [Storable]
    private IDeepCloneable visualizationInfo;
    /// <summary>
    /// Gets or sets the visualizationInfo.
    /// </summary>
    /// /// <remarks>The VisualizationInfo can only be set once and fires afterwards and InvalidOperationException</remarks>
    public IDeepCloneable VisualizationInfo {
      get { return visualizationInfo; }
      set {
        if (visualizationInfo != null)
          throw new InvalidOperationException("The value of the property VisualizationInfo is already set and cannot be set again.");
        visualizationInfo = value;
      }
    }

    [StorableConstructor]
    protected OperatorGraph(bool deserializing) : base(deserializing) { }
    protected OperatorGraph(OperatorGraph original, Cloner cloner)
      : base(original, cloner) {
      operators = cloner.Clone(original.operators);
      initialOperator = cloner.Clone(original.initialOperator);
      visualizationInfo = cloner.Clone(original.visualizationInfo);
      Initialize();
    }
    /// <summary>
    /// Initializes a new instance of <see cref="OperatorGraph"/>.
    /// </summary>
    public OperatorGraph() {
      operators = new OperatorSet();
      initialOperator = null;
      visualizationInfo = null;
      Initialize();
    }

    //mkommend: IMPORTANT DO NOT REMOVE THIS EVENT
    //needed to register OperatorGraph events in GraphVisualizationInfo
    public event EventHandler DeserializationFinished;
    private void OnOperatorGraphDeserializationFinished() {
      EventHandler handler = DeserializationFinished;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
      OnOperatorGraphDeserializationFinished();
    }
    private void Initialize() {
      RegisterOperatorsEvents();
    }

    /// <summary>
    /// Clones the current instance (deep clone).
    /// </summary>
    /// <remarks>Deep clone through <see cref="cloner.Clone"/> method of helper class 
    /// <see cref="Auxiliary"/>.</remarks>
    /// <param name="clonedObjects">Dictionary of all already cloned objects. (Needed to avoid cycles.)</param>
    /// <returns>The cloned object as <see cref="OperatorGraph"/>.</returns>
    public override IDeepCloneable Clone(Cloner cloner) {
      return new OperatorGraph(this, cloner);
    }

    /// <inheritdoc/>
    public event EventHandler InitialOperatorChanged;
    /// <summary>
    /// Fires a new <c>InitialOperatorChanged</c> event.
    /// </summary>
    protected virtual void OnInitialOperatorChanged() {
      var handler = InitialOperatorChanged;
      if (handler != null) handler(this, EventArgs.Empty);
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
      IValueParameter[] opParams = (from o in Operators
                                    from p in o.Parameters
                                    where p is IValueParameter
                                    where typeof(IOperator).IsAssignableFrom(((IValueParameter)p).DataType)
                                    where (((IValueParameter)p).Value != null) && (((IValueParameter)p).Value == op)
                                    select (IValueParameter)p).ToArray();
      foreach (IValueParameter opParam in opParams)
        opParam.Value = null;
    }
    private void AddParameter(IParameter param) {
      IValueParameter valueParam = param as IValueParameter;
      if ((valueParam != null) && (typeof(IOperator).IsAssignableFrom(valueParam.DataType))) {
        RegisterOperatorParameterEvents(valueParam);
        if (valueParam.Value != null) Operators.Add((IOperator)valueParam.Value);
      }
    }
    private void RemoveParameter(IParameter param) {
      IValueParameter valueParam = param as IValueParameter;
      if ((valueParam != null) && (typeof(IOperator).IsAssignableFrom(valueParam.DataType))) {
        DeregisterOperatorParameterEvents(valueParam);
      }
    }

    private void RegisterOperatorsEvents() {
      if (operators != null) {
        operators.ItemsAdded += new CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsAdded);
        operators.ItemsRemoved += new CollectionItemsChangedEventHandler<IOperator>(Operators_ItemsRemoved);
        operators.CollectionReset += new CollectionItemsChangedEventHandler<IOperator>(Operators_CollectionReset);
        foreach (IOperator op in operators) {
          RegisterOperatorEvents(op);
          var opParams = from p in op.Parameters
                         where p is IValueParameter
                         where typeof(IOperator).IsAssignableFrom(((IValueParameter)p).DataType)
                         select (IValueParameter)p;
          foreach (IValueParameter opParam in opParams)
            RegisterOperatorParameterEvents(opParam);
        }
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
    private void RegisterOperatorParameterEvents(IValueParameter opParam) {
      opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
    }
    private void DeregisterOperatorParameterEvents(IValueParameter opParam) {
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
      IValueParameter opParam = (IValueParameter)sender;
      if (opParam.Value != null) Operators.Add((IOperator)opParam.Value);
    }
    #endregion

    // <summary>
    /// Iterates an operator graph so that it jumps from the intial operator to all other operators and yields each operator it touches.
    /// Cycles are detected and not iterated twice.
    /// </summary>
    /// <returns>An enumeration of all the operators that could be found.</returns>
    public virtual IEnumerable<IOperator> Iterate() {
      if (InitialOperator == null) yield break;

      var open = new Stack<IOperator>();
      var visited = new HashSet<IOperator>();
      open.Push(InitialOperator);

      while (open.Any()) {
        IOperator current = open.Pop();
        if (visited.Contains(current)) continue;
        visited.Add(current);

        IOperatorGraphOperator operatorGraphOperator = current as IOperatorGraphOperator;
        if (operatorGraphOperator != null) open.Push(operatorGraphOperator.OperatorGraph.InitialOperator);

        foreach (var parameter in current.Parameters.OfType<IValueParameter>()) {
          if (!typeof(IOperator).IsAssignableFrom(parameter.DataType)) continue;
          if (parameter.Value == null) continue;

          open.Push((IOperator)parameter.Value);
        }

        yield return current;
      }
    }
  }
}
