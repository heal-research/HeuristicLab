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
using System.Linq;
using System.Text;
using System.Xml;
using HeuristicLab.Collections;
using HeuristicLab.Core;
using HeuristicLab.Parameters;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Operators {
  /// <summary>
  /// A base class for operators which have multiple successors.
  /// </summary>
  [Item("MultipleSuccessorsOperator", "A base class for operators which have multiple successors.")]
  [Creatable("Test")]
  [EmptyStorableClass]
  public abstract class MultipleSuccessorsOperator : Operator {
    protected IValueParameter<IOperator>[] SuccessorParameters {
      get {
        return (from p in Parameters
                where p is IValueParameter<IOperator>
                orderby p.Name ascending
                select (IValueParameter<IOperator>)p).ToArray();
      }
    }

    private OperatorList successors;
    public OperatorList Successors {
      get {
        if (successors == null) {
          successors = new OperatorList();
          var opParams = SuccessorParameters;
          foreach (IValueParameter<IOperator> opParam in opParams) {
            opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
            successors.Add(opParam.Value);
          }
          successors.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(successors_ItemsAdded);
          successors.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(successors_ItemsRemoved);
          successors.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(successors_ItemsReplaced);
          successors.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(successors_ItemsMoved);
          successors.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IOperator>>(successors_CollectionReset);
        }
        return successors;
      }
    }

    public MultipleSuccessorsOperator()
      : base() {
    }

    private void UpdateOperatorParameters() {
      var opParams = SuccessorParameters;
      foreach (IValueParameter<IOperator> opParam in opParams) {
        opParam.ValueChanged -= new EventHandler(opParam_ValueChanged);
        Parameters.Remove(opParam.Name);
      }
      for (int i = 0; i < Successors.Count; i++) {
        IValueParameter<IOperator> opParam = new OperatorParameter(i.ToString(), string.Empty, Successors[i]);
        opParam.ValueChanged += new EventHandler(opParam_ValueChanged);
        Parameters.Add(opParam);
      }
    }

    private void successors_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      UpdateOperatorParameters();
    }
    private void successors_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      UpdateOperatorParameters();
    }
    private void successors_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      foreach (IndexedItem<IOperator> item in e.Items)
        ((IValueParameter<IOperator>)Parameters[item.Index.ToString()]).Value = item.Value;
    }
    private void successors_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      foreach (IndexedItem<IOperator> item in e.Items)
        ((IValueParameter<IOperator>)Parameters[item.Index.ToString()]).Value = item.Value;
    }
    private void successors_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperator>> e) {
      UpdateOperatorParameters();
    }
    private void opParam_ValueChanged(object sender, EventArgs e) {
      IValueParameter<IOperator> opParam = (IValueParameter<IOperator>)sender;
      int index = int.Parse(opParam.Name);
      successors[index] = opParam.Value;
    }
  }
}
