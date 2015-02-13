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
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// A base class for operators which apply arbitrary many other operators of a specific type.
  /// </summary>
  [Item("MultiOperator", "A base class for operators which apply arbitrary many other operators of a specific type.")]
  [StorableClass]
  public abstract class MultiOperator<T> : InstrumentedOperator, IMultiOperator<T> where T : class, IOperator {
    private List<IValueParameter<T>> operatorParameters;
    protected IEnumerable<IValueParameter<T>> OperatorParameters { get { return operatorParameters; } }

    [Storable]
    private IItemList<T> operators;
    public IItemList<T> Operators {
      get { return operators; }
      protected set {
        if (operators != value) {
          if (value == null) throw new ArgumentException();
          DeregisterOperatorsEvents();
          operators = value;
          RegisterOperatorsEvents();
          UpdateOperatorParameters();
        }
      }
    }

    [StorableConstructor]
    protected MultiOperator(bool deserializing) : base(deserializing) { }
    protected MultiOperator(MultiOperator<T> original, Cloner cloner)
      : base(original, cloner) {
      this.operators = cloner.Clone<IItemList<T>>(original.operators);
      Initialize();
    }
    public MultiOperator()
      : base() {
      this.operators = new ItemList<T>();
      Initialize();
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      Initialize();
    }

    private void Initialize() {
      if (operators != null) RegisterOperatorsEvents();
      operatorParameters = new List<IValueParameter<T>>();
      for (int i = 0; i < Operators.Count; i++) {
        IValueParameter<T> opParam = (IValueParameter<T>)Parameters[i.ToString()];
        operatorParameters.Add(opParam);
        opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
      }
    }

    private void UpdateOperatorParameters() {
      foreach (IValueParameter<T> opParam in operatorParameters) {
        opParam.ValueChanged -= new EventHandler(opParam_ValueChanged);
        Parameters.Remove(opParam.Name);
      }
      operatorParameters.Clear();
      for (int i = 0; i < Operators.Count; i++) {
        IValueParameter<T> opParam = new OptionalValueParameter<T>(i.ToString(), string.Empty, Operators[i]);
        opParam.Hidden = true;
        opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
        Parameters.Add(opParam);
        operatorParameters.Add(opParam);
      }
    }

    #region Events
    private void RegisterOperatorsEvents() {
      operators.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Operators_ItemsAdded);
      operators.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Operators_ItemsRemoved);
      operators.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Operators_ItemsReplaced);
      operators.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Operators_ItemsMoved);
      operators.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<T>>(Operators_CollectionReset);
    }
    private void DeregisterOperatorsEvents() {
      operators.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Operators_ItemsAdded);
      operators.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Operators_ItemsRemoved);
      operators.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Operators_ItemsReplaced);
      operators.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Operators_ItemsMoved);
      operators.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<T>>(Operators_CollectionReset);
    }
    protected virtual void Operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      UpdateOperatorParameters();
    }
    protected virtual void Operators_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      UpdateOperatorParameters();
    }
    protected virtual void Operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      foreach (IndexedItem<T> item in e.Items)
        operatorParameters[item.Index].Value = item.Value;
    }
    protected virtual void Operators_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      foreach (IndexedItem<T> item in e.Items)
        operatorParameters[item.Index].Value = item.Value;
    }
    protected virtual void Operators_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<T>> e) {
      UpdateOperatorParameters();
    }
    private void opParam_ValueChanged(object sender, EventArgs e) {
      IValueParameter<T> opParam = (IValueParameter<T>)sender;
      if (opParam.Value == null)
        operators.RemoveAt(operatorParameters.IndexOf(opParam));
      else
        operators[operatorParameters.IndexOf(opParam)] = opParam.Value;
    }
    #endregion
  }
}
