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
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Optimization {
  [Item("Run Collection", "Represents a collection of runs.")]
  [Creatable("Testing & Analysis")]
  [StorableClass]
  public class RunCollection : ItemCollection<IRun>, IStringConvertibleMatrix {
    [StorableConstructor]
    protected RunCollection(bool deserializing) : base(deserializing) { }
    public RunCollection() : base() { Initialize(); }
    public RunCollection(int capacity) : base(capacity) { Initialize(); }
    public RunCollection(IEnumerable<IRun> collection) : base(collection) { Initialize(); this.OnItemsAdded(collection); }
    private void Initialize() {
      parameterNames = new List<string>();
      resultNames = new List<string>();
      dataTypes = new Dictionary<string, HashSet<Type>>();
      constraints = new RunCollectionConstraintCollection();
      RegisterConstraintsEvents();
    }

    [Storable]
    private Dictionary<string, HashSet<Type>> dataTypes;
    public IEnumerable<Type> GetDataType(string columnName) {
      if (!dataTypes.ContainsKey(columnName))
        return new Type[0];
      return dataTypes[columnName];
    }

    [Storable]
    private RunCollectionConstraintCollection constraints;
    public RunCollectionConstraintCollection Constraints {
      get { return constraints; }
    }

    protected override void OnCollectionReset(IEnumerable<IRun> items, IEnumerable<IRun> oldItems) {
      parameterNames.Clear();
      resultNames.Clear();
      foreach (IRun run in items) {
        foreach (KeyValuePair<string, IItem> parameter in run.Parameters)
          AddParameter(parameter.Key, parameter.Value);
        foreach (KeyValuePair<string, IItem> result in run.Results)
          AddResult(result.Key, result.Value);
      }
      base.OnCollectionReset(items, oldItems);
      OnReset();
      OnColumnNamesChanged();
      OnRowNamesChanged();
    }
    protected override void OnItemsAdded(IEnumerable<IRun> items) {
      bool columnNamesChanged = false;
      foreach (IRun run in items) {
        foreach (KeyValuePair<string, IItem> parameter in run.Parameters)
          columnNamesChanged |= AddParameter(parameter.Key, parameter.Value);
        foreach (KeyValuePair<string, IItem> result in run.Results)
          columnNamesChanged |= AddResult(result.Key, result.Value);
      }
      base.OnItemsAdded(items);
      OnReset();
      if (columnNamesChanged)
        OnColumnNamesChanged();
      OnRowNamesChanged();
      this.UpdateFiltering(false);
    }
    protected override void OnItemsRemoved(IEnumerable<IRun> items) {
      bool columnNamesChanged = false;
      foreach (IRun run in items) {
        foreach (string parameterName in run.Parameters.Keys)
          columnNamesChanged |= RemoveParameterName(parameterName);
        foreach (string resultName in run.Results.Keys)
          columnNamesChanged |= RemoveResultName(resultName);
      }
      base.OnItemsRemoved(items);
      OnReset();
      if (columnNamesChanged)
        OnColumnNamesChanged();
      OnRowNamesChanged();
    }

    private bool AddParameter(string name, IItem value) {
      if (value == null)
        return false;
      if (!parameterNames.Contains(name)) {
        parameterNames.Add(name);
        dataTypes[name] = new HashSet<Type>();
        dataTypes[name].Add(value.GetType());
        return true;
      }
      dataTypes[name].Add(value.GetType());
      return false;
    }
    private bool AddResult(string name, IItem value) {
      if (value == null)
        return false;
      if (!resultNames.Contains(name)) {
        resultNames.Add(name);
        dataTypes[name] = new HashSet<Type>();
        dataTypes[name].Add(value.GetType());
        return true;
      }
      dataTypes[name].Add(value.GetType());
      return false;
    }
    private bool RemoveParameterName(string name) {
      if (!list.Any(x => x.Parameters.ContainsKey(name))) {
        parameterNames.Remove(name);
        return true;
      }
      return false;
    }
    private bool RemoveResultName(string name) {
      if (!list.Any(x => x.Results.ContainsKey(name))) {
        resultNames.Remove(name);
        return true;
      }
      return false;
    }

    public IItem GetValue(int rowIndex, int columnIndex) {
      IRun run = this.list[rowIndex];
      return GetValue(run, columnIndex);
    }

    public IItem GetValue(IRun run, int columnIndex) {
      string name = ((IStringConvertibleMatrix)this).ColumnNames.ElementAt(columnIndex);
      return GetValue(run, name);
    }

    public IItem GetValue(IRun run, string columnName) {
      IItem value = null;
      if (run.Parameters.ContainsKey(columnName))
        value = run.Parameters[columnName];
      else if (run.Results.ContainsKey(columnName))
        value = run.Results[columnName];
      return value;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserializationHook() {
      if (constraints == null) constraints = new RunCollectionConstraintCollection();
      RegisterConstraintsEvents();
      UpdateFiltering(true);
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      RunCollection clone = (RunCollection)base.Clone(cloner);
      clone.resultNames = new List<string>(this.resultNames);
      clone.parameterNames = new List<string>(this.parameterNames);
      clone.dataTypes = new Dictionary<string, HashSet<Type>>();
      foreach (string s in this.dataTypes.Keys)
        clone.dataTypes[s] = new HashSet<Type>(this.dataTypes[s]);

      clone.constraints = new RunCollectionConstraintCollection(this.constraints.Select(x => (IRunCollectionConstraint)cloner.Clone(x)));
      foreach (IRunCollectionConstraint constraint in clone.constraints)
        constraint.ConstrainedValue = clone;
      clone.RegisterConstraintsEvents();
      clone.RegisterConstraintEvents(clone.constraints);

      clone.UpdateFiltering(true);
      return clone;
    }

    #region IStringConvertibleMatrix Members
    [Storable]
    private List<string> parameterNames;
    public IEnumerable<string> ParameterNames {
      get { return this.parameterNames; }
    }
    [Storable]
    private List<string> resultNames;
    public IEnumerable<string> ResultNames {
      get { return this.resultNames; }
    }
    int IStringConvertibleMatrix.Rows {
      get { return this.Count; }
      set { throw new NotSupportedException(); }
    }
    int IStringConvertibleMatrix.Columns {
      get { return parameterNames.Count + resultNames.Count; }
      set { throw new NotSupportedException(); }
    }
    IEnumerable<string> IStringConvertibleMatrix.ColumnNames {
      get {
        List<string> value = new List<string>(parameterNames);
        value.AddRange(resultNames);
        value.Sort();
        return value;
      }
      set { throw new NotSupportedException(); }
    }
    IEnumerable<string> IStringConvertibleMatrix.RowNames {
      get { return list.Select(x => x.Name).ToList(); }
      set { throw new NotSupportedException(); }
    }
    bool IStringConvertibleMatrix.SortableView {
      get { return true; }
      set { throw new NotSupportedException(); }
    }
    bool IStringConvertibleMatrix.ReadOnly {
      get { return true; }
    }

    string IStringConvertibleMatrix.GetValue(int rowIndex, int columnIndex) {
      IItem value = GetValue(rowIndex, columnIndex);
      if (value == null)
        return string.Empty;
      return value.ToString();
    }

    public event EventHandler<EventArgs<int, int>> ItemChanged;
    protected virtual void OnItemChanged(int rowIndex, int columnIndex) {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs<int, int>(rowIndex, columnIndex));
      OnToStringChanged();
    }
    public event EventHandler Reset;
    protected virtual void OnReset() {
      if (Reset != null)
        Reset(this, EventArgs.Empty);
      OnToStringChanged();
    }
    public event EventHandler ColumnNamesChanged;
    protected virtual void OnColumnNamesChanged() {
      EventHandler handler = ColumnNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler RowNamesChanged;
    protected virtual void OnRowNamesChanged() {
      EventHandler handler = RowNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler SortableViewChanged;
    protected virtual void OnSortableViewChanged() {
      EventHandler handler = SortableViewChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    public bool Validate(string value, out string errorMessage) { throw new NotSupportedException(); }
    public bool SetValue(string value, int rowIndex, int columnIndex) { throw new NotSupportedException(); }
    #endregion

    #region filtering
    private void UpdateFiltering(bool reset) {
      if (reset)
        list.ForEach(r => r.Visible = true);
      foreach (IRunCollectionConstraint constraint in this.constraints)
        constraint.Check();
    }

    private void RegisterConstraintsEvents() {
      constraints.ItemsAdded += new CollectionItemsChangedEventHandler<IRunCollectionConstraint>(Constraints_ItemsAdded);
      constraints.ItemsRemoved += new CollectionItemsChangedEventHandler<IRunCollectionConstraint>(Constraints_ItemsRemoved);
      constraints.CollectionReset += new CollectionItemsChangedEventHandler<IRunCollectionConstraint>(Constraints_CollectionReset);
    }

    protected virtual void RegisterConstraintEvents(IEnumerable<IRunCollectionConstraint> constraints) {
      foreach (IRunCollectionConstraint constraint in constraints) {
        constraint.ActiveChanged += new EventHandler(Constraint_ActiveChanged);
        constraint.ConstrainedValueChanged += new EventHandler(Constraint_ConstrainedValueChanged);
        constraint.ConstraintOperationChanged += new EventHandler(Constraint_ConstraintOperationChanged);
        constraint.ConstraintDataChanged += new EventHandler(Constraint_ConstraintDataChanged);
      }
    }
    protected virtual void DeregisterConstraintEvents(IEnumerable<IRunCollectionConstraint> constraints) {
      foreach (IRunCollectionConstraint constraint in constraints) {
        constraint.ActiveChanged -= new EventHandler(Constraint_ActiveChanged);
        constraint.ConstrainedValueChanged -= new EventHandler(Constraint_ConstrainedValueChanged);
        constraint.ConstraintOperationChanged -= new EventHandler(Constraint_ConstraintOperationChanged);
        constraint.ConstraintDataChanged -= new EventHandler(Constraint_ConstraintDataChanged);
      }
    }

    protected virtual void Constraints_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRunCollectionConstraint> e) {
      DeregisterConstraintEvents(e.OldItems);
      RegisterConstraintEvents(e.Items);
      this.UpdateFiltering(true);
    }
    protected virtual void Constraints_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRunCollectionConstraint> e) {
      RegisterConstraintEvents(e.Items);
      foreach (IRunCollectionConstraint constraint in e.Items)
        constraint.ConstrainedValue = this;
      this.UpdateFiltering(false);
    }
    protected virtual void Constraints_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRunCollectionConstraint> e) {
      DeregisterConstraintEvents(e.Items);
      this.UpdateFiltering(true);
    }
    protected virtual void Constraint_ActiveChanged(object sender, EventArgs e) {
      IRunCollectionConstraint constraint = (IRunCollectionConstraint)sender;
      this.UpdateFiltering(!constraint.Active);
    }
    protected virtual void Constraint_ConstrainedValueChanged(object sender, EventArgs e) {
      //mkommend: this method is intentionally left empty, because the constrainedValue is set in the ItemsAdded method
    }
    protected virtual void Constraint_ConstraintOperationChanged(object sender, EventArgs e) {
      IRunCollectionConstraint constraint = (IRunCollectionConstraint)sender;
      if (constraint.Active)
        this.UpdateFiltering(true);
    }
    protected virtual void Constraint_ConstraintDataChanged(object sender, EventArgs e) {
      IRunCollectionConstraint constraint = (IRunCollectionConstraint)sender;
      if (constraint.Active)
        this.UpdateFiltering(true);
    }
    #endregion
  }
}
