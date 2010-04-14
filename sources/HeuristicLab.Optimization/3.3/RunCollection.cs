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
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Common;

namespace HeuristicLab.Optimization {
  [StorableClass]
  [Item("RunCollection", "Represents a collection of runs.")]
  public class RunCollection : ItemCollection<IRun>, IStringConvertibleMatrix {
    public RunCollection() : base() { Initialize(); }
    public RunCollection(int capacity) : base(capacity) { Initialize(); }
    public RunCollection(IEnumerable<IRun> collection) : base(collection) { Initialize(); this.OnItemsAdded(collection); }

    protected static Type[] viewableDataTypes = new Type[]{typeof(BoolValue), typeof(DoubleValue), typeof(IntValue),
      typeof(PercentValue), typeof(StringValue)};

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

    private void Initialize() {
      parameterNames = new List<string>();
      resultNames = new List<string>();
      this.ReadOnlyView = true;
    }

    private bool AddParameter(string name, IItem value) {
      if (value == null)
        return false;
      if (!parameterNames.Contains(name) &&
          viewableDataTypes.Any(x => x.IsAssignableFrom(value.GetType()))) {
        parameterNames.Add(name);
        return true;
      }
      return false;
    }
    private bool AddResult(string name, IItem value) {
      if (value == null)
        return false;
      if (!resultNames.Contains(name) &&
          viewableDataTypes.Any(x => x.IsAssignableFrom(value.GetType()))) {
        resultNames.Add(name);
        return true;
      }
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

    #region IStringConvertibleMatrix Members
    [Storable]
    private List<string> parameterNames;
    [Storable]
    private List<string> resultNames;
    public int Rows {
      get { return this.Count; }
      set { throw new System.NotImplementedException(); }
    }
    public int Columns {
      get { return parameterNames.Count + resultNames.Count; }
      set { throw new NotSupportedException(); }
    }
    public IEnumerable<string> ColumnNames {
      get {
        List<string> value = new List<string>(parameterNames);
        value.AddRange(resultNames);
        return value;
      }
      set { throw new NotSupportedException(); }
    }
    public IEnumerable<string> RowNames {
      get { return list.Select(x => x.Name).ToList(); }
      set { throw new NotSupportedException(); }
    }
    public bool SortableView {
      get { return true; }
      set { throw new NotSupportedException(); }
    }

    public string GetValue(int rowIndex, int columnIndex) {
      IRun run = this.list[rowIndex];
      string value = string.Empty;

      if (columnIndex < parameterNames.Count) {
        string parameterName = parameterNames[columnIndex];
        if (run.Parameters.ContainsKey(parameterName)) {
          IItem param = run.Parameters[parameterName];
          if (param != null) value = param.ToString();
        }
      } else if (columnIndex < parameterNames.Count + resultNames.Count) {
        string resultName = resultNames[columnIndex - parameterNames.Count];
        if (run.Results.ContainsKey(resultName)) {
          IItem result = run.Results[resultName];
          if (result != null) value = result.ToString();
        }
      }
      return value;
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
  }
}
