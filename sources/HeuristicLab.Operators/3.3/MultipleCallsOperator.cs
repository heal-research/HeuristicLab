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
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// An operator which applies multiple operators sequentially on the current scope.
  /// </summary>
  [Item("MultipleCallsOperator", "An operator which applies multiple operators sequentially on the current scope.")]
  [Creatable("Test")]
  public class MultipleCallsOperator : SingleSuccessorOperator {
    protected IValueParameter<IOperator>[] OperatorParameters {
      get {
        return (from p in Parameters
                where p is IValueParameter<IOperator>
                where Operators.Contains(((IValueParameter<IOperator>)p).Value)
                orderby p.Name ascending
                select (IValueParameter<IOperator>)p).ToArray();
      }
    }

    private OperatorList operators;
    [Storable]
    public OperatorList Operators {
      get { return operators; }
      private set {
        operators = value;
        operators.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsAdded);
        operators.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsRemoved);
        operators.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsReplaced);
        operators.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_ItemsMoved);
        operators.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(operators_CollectionReset);
        var opParams = OperatorParameters;
        foreach (IValueParameter<IOperator> opParam in opParams)
          opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
      }
    }

    public MultipleCallsOperator()
      : base() {
      Operators = new OperatorList();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      MultipleCallsOperator clone = (MultipleCallsOperator)base.Clone(cloner);
      clone.Operators = (OperatorList)cloner.Clone(operators);
      return clone;
    }

    private void UpdateOperatorParameters() {
      var opParams = OperatorParameters;
      foreach (IValueParameter<IOperator> opParam in opParams) {
        opParam.ValueChanged -= new EventHandler(opParam_ValueChanged);
        Parameters.Remove(opParam.Name);
      }
      for (int i = 0; i < Operators.Count; i++) {
        IValueParameter<IOperator> opParam = new OperatorParameter(i.ToString(), string.Empty, Operators[i]);
        opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
        Parameters.Add(opParam);
      }
    }
    private void operators_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      UpdateOperatorParameters();
    }
    private void operators_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      UpdateOperatorParameters();
    }
    private void operators_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      foreach (IndexedItem<IOperator> item in e.Items)
        ((IValueParameter<IOperator>)Parameters[item.Index.ToString()]).Value = item.Value;
    }
    private void operators_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      foreach (IndexedItem<IOperator> item in e.Items)
        ((IValueParameter<IOperator>)Parameters[item.Index.ToString()]).Value = item.Value;
    }
    private void operators_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      UpdateOperatorParameters();
    }
    private void opParam_ValueChanged(object sender, EventArgs e) {
      IValueParameter<IOperator> opParam = (IValueParameter<IOperator>)sender;
      int index = int.Parse(opParam.Name);
      operators[index] = opParam.Value;
    }

    public override IExecutionSequence Apply() {
      ExecutionContextCollection next = new ExecutionContextCollection(base.Apply());
      for (int i = Operators.Count - 1; i >= 0; i--)
        next.Insert(0, ExecutionContext.CreateContext(Operators[i]));
      return next;
    }
  }
}
