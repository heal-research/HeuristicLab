#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HEAL.Fossil;

namespace HeuristicLab.DebugEngine {

  [StorableType("F694C6E8-39EC-4705-AB77-D00147A4D542")]
  public class OperatorTrace : ObservableList<IOperator>, IContent, IDeepCloneable {

    #region fields

    [Storable]
    protected Dictionary<IAtomicOperation, IAtomicOperation> parents;

    [Storable]
    protected bool isEnabled;
    #endregion

    #region events
    public event EventHandler IsEnabledChanged;
    protected virtual void OnIsEnabledChanged() {
      EventHandler handler = IsEnabledChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    #endregion

    #region Constructors & Cloning

    public OperatorTrace() {
      parents = new Dictionary<IAtomicOperation, IAtomicOperation>();
    }

    public OperatorTrace(int capacity)
      : base(capacity) {
      parents = new Dictionary<IAtomicOperation, IAtomicOperation>();
    }

    public OperatorTrace(IEnumerable<IOperator> collection)
      : base(collection) {
      parents = new Dictionary<IAtomicOperation, IAtomicOperation>();
    }

    [StorableConstructor]
    protected OperatorTrace(StorableConstructorFlag _) : base(_) { }

    protected OperatorTrace(OperatorTrace original, Cloner cloner) {
      cloner.RegisterClonedObject(original, this);
      AddRange(original.Select(op => cloner.Clone(op)));
      parents = original.parents.ToDictionary(kvp => cloner.Clone(kvp.Key), kvp => cloner.Clone(kvp.Value));
    }

    public object Clone() {
      return Clone(new Cloner());
    }

    public virtual IDeepCloneable Clone(Cloner cloner) {
      return new OperatorTrace(this, cloner);
    }
    #endregion

    #region Additional List Modifiers

    public virtual void ReplaceAll(IEnumerable<IOperator> operators) {
      var oldList = list;
      list = new List<IOperator>(operators);
      if (oldList.Count != list.Count)
        OnPropertyChanged("Count");
      OnPropertyChanged("Item[]");
      OnCollectionReset(
        list.Select((op, i) => new IndexedItem<IOperator>(i, op)),
        oldList.Select((op, i) => new IndexedItem<IOperator>(i, op)));
    }

    #endregion

    #region Parent Tracing

    public virtual void RegisterParenthood(IAtomicOperation parent, IOperation children) {
      if (!isEnabled)
        return;
      OperationCollection operations = children as OperationCollection;
      if (operations != null)
        foreach (var op in operations)
          RegisterParenthood(parent, op);
      IAtomicOperation atomicOperation = children as IAtomicOperation;
      if (atomicOperation != null && atomicOperation.Operator != null && !parents.ContainsKey(atomicOperation))
        parents[atomicOperation] = parent;
    }

    public virtual void Reset() {
      Clear();
      parents.Clear();
    }

    public virtual void Regenerate(IAtomicOperation operation) {
      if (!isEnabled) {
        Reset();
        return;
      }
      if (operation == null)
        return;
      Stack<IOperator> trace = new Stack<IOperator>();
      while (operation != null) {
        trace.Push(operation.Operator);
        IAtomicOperation parent = null;
        parents.TryGetValue(operation, out parent);
        operation = parent;
      }
      ReplaceAll(trace);
    }

    public bool IsEnabled {
      get { return isEnabled; }
      set {
        if (isEnabled == value)
          return;
        isEnabled = value;
        if (!isEnabled)
          Reset();
        OnIsEnabledChanged();
      }
    }

    #endregion
  }
}
