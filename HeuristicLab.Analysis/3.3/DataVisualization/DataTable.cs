#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
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
  public class DataTable : NamedItem, IStringConvertibleMatrix {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Performance; }
    }

    private DataTableVisualProperties visualProperties;
    public DataTableVisualProperties VisualProperties {
      get { return visualProperties; }
      set {
        if (visualProperties != value) {
          if (value == null) throw new ArgumentNullException("VisualProperties");
          if (visualProperties != null) visualProperties.PropertyChanged -= new PropertyChangedEventHandler(VisualProperties_PropertyChanged);
          visualProperties = value;
          visualProperties.PropertyChanged += new PropertyChangedEventHandler(VisualProperties_PropertyChanged);
          OnVisualPropertiesChanged();
        }
      }
    }

    private NamedItemCollection<DataRow> rows;
    public NamedItemCollection<DataRow> Rows {
      get { return rows; }
      private set {
        if (rows != null) throw new InvalidOperationException("Rows already set");
        rows = value;
        if (rows != null) RegisterRowsEvents();
      }
    }

    #region Persistence Properties
    [Storable(Name = "VisualProperties")]
    private DataTableVisualProperties StorableVisualProperties {
      get { return visualProperties; }
      set {
        visualProperties = value;
        visualProperties.PropertyChanged += new PropertyChangedEventHandler(VisualProperties_PropertyChanged);
      }
    }
    [Storable(Name = "rows")]
    private IEnumerable<DataRow> StorableRows {
      get { return rows; }
      set { Rows = new NamedItemCollection<DataRow>(value); }
    }
    #endregion

    [StorableConstructor]
    protected DataTable(bool deserializing) : base(deserializing) { }
    protected DataTable(DataTable original, Cloner cloner)
      : base(original, cloner) {
      VisualProperties = (DataTableVisualProperties)cloner.Clone(original.visualProperties);
      Rows = cloner.Clone(original.rows);
    }
    public DataTable()
      : base() {
      Name = "DataTable";
      VisualProperties = new DataTableVisualProperties();
      Rows = new NamedItemCollection<DataRow>();
    }
    public DataTable(string name)
      : base(name) {
      VisualProperties = new DataTableVisualProperties(name);
      Rows = new NamedItemCollection<DataRow>();
    }
    public DataTable(string name, string description)
      : base(name, description) {
      VisualProperties = new DataTableVisualProperties(name);
      Rows = new NamedItemCollection<DataRow>();
    }

    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (VisualProperties == null) VisualProperties = new DataTableVisualProperties(name);
      if (VisualProperties.Title == null) VisualProperties.Title = name;

      #region Backwards Compatability Histogram Visual Properties 
      var rowProperties = Rows.Select(r => r.VisualProperties).ToList();
      if (rowProperties.Any(r => r.Bins.HasValue))
        VisualProperties.HistogramBins = rowProperties.Where(r => r.Bins.HasValue).Max(r => r.Bins.Value);
      if (rowProperties.Any(r => r.ExactBins.HasValue))
        VisualProperties.HistogramExactBins = rowProperties.Where(r => r.ExactBins.HasValue).Any(r => r.ExactBins.Value);
      if (rowProperties.Any(r => r.Aggregation.HasValue)) {
        var maxOccurrence = rowProperties
          .Where(r => r.Aggregation.HasValue).Select(r => r.Aggregation.Value)
          .GroupBy(x => x).OrderByDescending(x => x.Count())
          .First().Key;
        VisualProperties.HistogramAggregation = (DataTableVisualProperties.DataTableHistogramAggregation)maxOccurrence;
      }
      #endregion
    }
    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataTable(this, cloner);
    }

    public event EventHandler VisualPropertiesChanged;
    protected virtual void OnVisualPropertiesChanged() {
      EventHandler handler = VisualPropertiesChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    private void VisualProperties_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      OnVisualPropertiesChanged();
    }

    protected virtual void RegisterRowsEvents() {
      rows.ItemsAdded += new CollectionItemsChangedEventHandler<DataRow>(rows_ItemsAdded);
      rows.ItemsRemoved += new CollectionItemsChangedEventHandler<DataRow>(rows_ItemsRemoved);
      rows.ItemsReplaced += new CollectionItemsChangedEventHandler<DataRow>(rows_ItemsReplaced);
      rows.CollectionReset += new CollectionItemsChangedEventHandler<DataRow>(rows_CollectionReset);
    }
    private void rows_ItemsAdded(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      foreach (DataRow row in e.Items)
        this.RegisterRowEvents(row);

      this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    private void rows_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      foreach (DataRow row in e.Items)
        this.DeregisterRowEvents(row);

      this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    private void rows_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      foreach (DataRow row in e.OldItems)
        this.DeregisterRowEvents(row);
      foreach (DataRow row in e.Items)
        this.RegisterRowEvents(row);

      this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    private void rows_CollectionReset(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      foreach (DataRow row in e.OldItems)
        this.DeregisterRowEvents(row);
      foreach (DataRow row in e.Items)
        this.RegisterRowEvents(row);

      if (e.OldItems.Count() != e.Items.Count())
        this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }

    protected virtual void RegisterRowEvents(DataRow row) {
      row.Values.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsAdded);
      row.Values.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsMoved);
      row.Values.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsRemoved);
      row.Values.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsReplaced);
      row.Values.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_CollectionReset);
    }
    protected virtual void DeregisterRowEvents(DataRow row) {
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
      get { return rows.Count == 0 ? 0 : rows.Max(r => r.Values.Count); }
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
      get { return Enumerable.Empty<string>(); }
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
    protected virtual void OnItemChanged(int rowIndex, int columnIndex) {
      var handler = ItemChanged;
      if (handler != null) handler(this, new EventArgs<int, int>(rowIndex, columnIndex));
      OnToStringChanged();
    }
    public event EventHandler Reset;
    protected virtual void OnReset() {
      var handler = Reset;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ColumnsChanged;
    protected virtual void OnColumnsChanged() {
      var handler = ColumnsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler RowsChanged;
    protected virtual void OnRowsChanged() {
      var handler = RowsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ColumnNamesChanged;
    protected virtual void OnColumnNamesChanged() {
      var handler = ColumnNamesChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler RowNamesChanged;
    protected virtual void OnRowNamesChanged() {
      var handler = RowNamesChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler SortableViewChanged;
    protected virtual void OnSortableViewChanged() {
      var handler = SortableViewChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    #endregion
  }
}
