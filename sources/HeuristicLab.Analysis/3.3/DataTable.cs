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
using System.Drawing;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Analysis {
  /// <summary>
  /// A table of data values.
  /// </summary>
  [Item("DataTable", "A table of data values.")]
  [StorableClass]
  public sealed class DataTable : NamedItem, IStringConvertibleMatrix {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Performance; }
    }

    [Storable]
    private NamedItemCollection<DataRow> rows;
    public NamedItemCollection<DataRow> Rows {
      get { return rows; }
    }

    #region Storing & Cloning
    [StorableConstructor]
    private DataTable(bool deserializing) : base(deserializing) { }
    private DataTable(DataTable original, Cloner cloner)
      : base(original, cloner) {
      this.rows = cloner.Clone(original.rows);
      this.RegisterRowsEvents();
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataTable(this, cloner);
    }
    #endregion
    public DataTable()
      : base() {
      rows = new NamedItemCollection<DataRow>();
      this.RegisterRowsEvents();
    }
    public DataTable(string name)
      : base(name) {
      rows = new NamedItemCollection<DataRow>();
      this.RegisterRowsEvents();
    }
    public DataTable(string name, string description)
      : base(name, description) {
      rows = new NamedItemCollection<DataRow>();
      this.RegisterRowsEvents();
    }

    private void RegisterRowsEvents() {
      rows.ItemsAdded += new CollectionItemsChangedEventHandler<DataRow>(rows_ItemsAdded);
      rows.ItemsRemoved += new CollectionItemsChangedEventHandler<DataRow>(rows_ItemsRemoved);
      rows.ItemsReplaced += new CollectionItemsChangedEventHandler<DataRow>(rows_ItemsReplaced);
      rows.CollectionReset += new CollectionItemsChangedEventHandler<DataRow>(rows_CollectionReset);
    }
    private void rows_ItemsAdded(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      foreach (DataRow row in e.Items)
        this.RegisterRowEvents(row);
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    private void rows_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      foreach (DataRow row in e.Items)
        this.DeregisterRowEvents(row);
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    private void rows_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      foreach (DataRow row in e.OldItems)
        this.DeregisterRowEvents(row);
      foreach (DataRow row in e.Items)
        this.RegisterRowEvents(row);
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    private void rows_CollectionReset(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      foreach (DataRow row in e.OldItems)
        this.DeregisterRowEvents(row);
      foreach (DataRow row in e.Items)
        this.RegisterRowEvents(row);
      this.OnColumnNamesChanged();
      this.OnReset();
    }

    private void RegisterRowEvents(DataRow row) {
      row.Values.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsAdded);
      row.Values.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsMoved);
      row.Values.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsRemoved);
      row.Values.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsReplaced);
      row.Values.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_CollectionReset);
    }
    private void DeregisterRowEvents(DataRow row) {
      row.Values.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsAdded);
      row.Values.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsMoved);
      row.Values.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsRemoved);
      row.Values.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsReplaced);
      row.Values.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_CollectionReset);
    }

    private void Values_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      this.OnReset();
    }
    private void Values_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      this.OnReset();
    }
    private void Values_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      this.OnReset();
    }
    private void Values_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      this.OnReset();
    }
    private void Values_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      this.OnReset();
    }

    #region IStringConvertibleMatrix Members

    int IStringConvertibleMatrix.Rows {
      get { return rows.Max(r => r.Values.Count); }
      set { throw new NotSupportedException(); }
    }
    int IStringConvertibleMatrix.Columns {
      get { return rows.Count; }
      set { throw new NotSupportedException(); }
    }
    IEnumerable<string> IStringConvertibleMatrix.ColumnNames {
      get { return rows.Select(r => r.Name); }
      set { throw new NotSupportedException(); }
    }
    IEnumerable<string> IStringConvertibleMatrix.RowNames {
      get { return new List<string>(); }
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
      if (columnIndex < rows.Count) {
        string columnName = ((IStringConvertibleMatrix)this).ColumnNames.ElementAt(columnIndex);
        if (rows.ContainsKey(columnName) && rowIndex < rows[columnName].Values.Count)
          return rows[columnName].Values[rowIndex].ToString();
      }
      return string.Empty;
    }

    bool IStringConvertibleMatrix.Validate(string value, out string errorMessage) {
      throw new NotSupportedException();
    }
    bool IStringConvertibleMatrix.SetValue(string value, int rowIndex, int columnIndex) {
      throw new NotSupportedException();
    }

    public event EventHandler<EventArgs<int, int>> ItemChanged;
    private void OnItemChanged(int rowIndex, int columnIndex) {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs<int, int>(rowIndex, columnIndex));
      OnToStringChanged();
    }
    public event EventHandler Reset;
    private void OnReset() {
      if (Reset != null)
        Reset(this, EventArgs.Empty);
    }
    public event EventHandler ColumnNamesChanged;
    private void OnColumnNamesChanged() {
      EventHandler handler = ColumnNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler RowNamesChanged;
    private void OnRowNamesChanged() {
      EventHandler handler = RowNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler SortableViewChanged;
    private void OnSortableViewChanged() {
      EventHandler handler = SortableViewChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
