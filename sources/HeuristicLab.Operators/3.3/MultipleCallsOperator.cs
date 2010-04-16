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
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// A base class for operators which apply multiple user-defined operators.
  /// </summary>
  [Item("MultipleCallsOperator", "A base class for operators which apply multiple user-defined operators.")]
  [StorableClass]
  public abstract class MultipleCallsOperator : SingleSuccessorOperator {
    private List<IValueParameter<IOperator>> operatorParameters;

    [Storable]
    private OperatorList operators;
    public OperatorList Operators {
      get { return operators; }
    }

    public MultipleCallsOperator()
      : base() {
      operators = new OperatorList();
      Initialize();
    }
    [StorableConstructor]
    protected MultipleCallsOperator(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      if (operators != null) RegisterOperatorsEvents();
      operatorParameters = new List<IValueParameter<IOperator>>();
      for (int i = 0; i < Operators.Count; i++) {
        IValueParameter<IOperator> opParam = (IValueParameter<IOperator>)Parameters[i.ToString()];
        operatorParameters.Add(opParam);
        opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
      }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      MultipleCallsOperator clone = (MultipleCallsOperator)base.Clone(cloner);
      clone.operators = (OperatorList)cloner.Clone(operators);
      clone.Initialize();
      return clone;
    }

    private void UpdateOperatorParameters() {
      foreach (IValueParameter<IOperator> opParam in operatorParameters) {
        opParam.ValueChanged -= new EventHandler(opParam_ValueChanged);
        Parameters.Remove(opParam.Name);
      }
      operatorParameters.Clear();
      for (int i = 0; i < Operators.Count; i++) {
        IValueParameter<IOperator> opParam = new ValueParameter<IOperator>(i.ToString(), string.Empty, Operators[i]);
        opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
        Parameters.Add(opParam);
        operatorParameters.Add(opParam);
      }
    }

    #region Events
    private void RegisterOperatorsEvents() {
      operators.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsAdded);
      operators.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsRemoved);
      operators.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsReplaced);
      operators.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsMoved);
      operators.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_CollectionReset);
    }
    private void DeregisterOperatorsEvents() {
      operators.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsAdded);
      operators.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsRemoved);
      operators.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsReplaced);
      operators.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsMoved);
      operators.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_CollectionReset);
    }
    private void operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      UpdateOperatorParameters();
    }
    private void operators_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      UpdateOperatorParameters();
    }
    private void operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      foreach (IndexedItem<IOperator> item in e.Items)
        operatorParameters[item.Index].Value = item.Value;
    }
    private void operators_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      foreach (IndexedItem<IOperator> item in e.Items)
        operatorParameters[item.Index].Value = item.Value;
    }
    private void operators_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      UpdateOperatorParameters();
    }
    private void opParam_ValueChanged(object sender, EventArgs e) {
      IValueParameter<IOperator> opParam = (IValueParameter<IOperator>)sender;
      operators[operatorParameters.IndexOf(opParam)] = opParam.Value;
    }
    #endregion
  }
}
