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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HEAL.Fossil;

namespace HeuristicLab.Analysis {
  [Item("IndexedDataTable", "A data table where the points are also given with a certain index.")]
  [StorableType("1453C842-6312-4931-9B05-20399A0528D6")]
  public class IndexedDataTable<T> : NamedItem, IStringConvertibleMatrix, IDataTable<IndexedDataRow<T>> {
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

    private NamedItemCollection<IndexedDataRow<T>> rows;
    public NamedItemCollection<IndexedDataRow<T>> Rows {
      get { return rows; }
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
    private IEnumerable<IndexedDataRow<T>> StorableRows {
      get { return rows; }
      set { rows = new NamedItemCollection<IndexedDataRow<T>>(value); }
    }
    #endregion

    [StorableConstructor]
    protected IndexedDataTable(StorableConstructorFlag _) : base(_) { }
    protected IndexedDataTable(IndexedDataTable<T> original, Cloner cloner)
      : base(original, cloner) {
      VisualProperties = (DataTableVisualProperties)cloner.Clone(original.visualProperties);
      rows = cloner.Clone(original.rows);
      this.RegisterRowsEvents();
    }
    public IndexedDataTable()
      : base() {
      VisualProperties = new DataTableVisualProperties();
      rows = new NamedItemCollection<IndexedDataRow<T>>();
      this.RegisterRowsEvents();
    }
    public IndexedDataTable(string name)
      : base(name) {
      VisualProperties = new DataTableVisualProperties(name);
      rows = new NamedItemCollection<IndexedDataRow<T>>();
      this.RegisterRowsEvents();
    }
    public IndexedDataTable(string name, string description)
      : base(name, description) {
      VisualProperties = new DataTableVisualProperties(name);
      rows = new NamedItemCollection<IndexedDataRow<T>>();
      this.RegisterRowsEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new IndexedDataTable<T>(this, cloner);
    }

    #region BackwardsCompatibility3.3
    // Using the name as title is the old style
    [Storable(DefaultValue = true)]
    private bool useNameAsTitle = false;
    #endregion

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      // BackwardsCompatibility3.3
      #region Backwards compatible code, remove with 3.4
      // Previously, the Name of the IndexedDataTable was used as Title
      if (useNameAsTitle && string.IsNullOrEmpty(VisualProperties.Title)) {
        VisualProperties.Title = Name;
        useNameAsTitle = false;
      }
      #endregion
    }

    public event EventHandler VisualPropertiesChanged;
    protected virtual void OnVisualPropertiesChanged() {
      var handler = VisualPropertiesChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    private void VisualProperties_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      OnVisualPropertiesChanged();
    }

    private void RegisterRowsEvents() {
      rows.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(RowsOnItemsAdded);
      rows.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(RowsOnItemsRemoved);
      rows.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(RowsOnItemsReplaced);
      rows.CollectionReset += new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(RowsOnCollectionReset);
      foreach (var row in Rows) RegisterRowEvents(row);
    }
    protected virtual void RowsOnItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedDataRow<T>> e) {
      foreach (var row in e.Items)
        this.RegisterRowEvents(row);

      this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    protected virtual void RowsOnItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedDataRow<T>> e) {
      foreach (var row in e.Items)
        this.DeregisterRowEvents(row);

      this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    protected virtual void RowsOnItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedDataRow<T>> e) {
      foreach (var row in e.OldItems)
        this.DeregisterRowEvents(row);
      foreach (var row in e.Items)
        this.RegisterRowEvents(row);

      this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    protected virtual void RowsOnCollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedDataRow<T>> e) {
      foreach (var row in e.OldItems)
        this.DeregisterRowEvents(row);
      foreach (var row in e.Items)
        this.RegisterRowEvents(row);

      if (e.OldItems.Count() != e.Items.Count())
        this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }

    private void RegisterRowEvents(IndexedDataRow<T> row) {
      row.Values.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(ValuesOnItemsAdded);
      row.Values.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(ValuesOnItemsMoved);
      row.Values.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(ValuesOnItemsRemoved);
      row.Values.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(ValuesOnItemsReplaced);
      row.Values.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(ValuesOnCollectionReset);
    }
    private void DeregisterRowEvents(IndexedDataRow<T> row) {
      row.Values.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(ValuesOnItemsAdded);
      row.Values.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(ValuesOnItemsMoved);
      row.Values.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(ValuesOnItemsRemoved);
      row.Values.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(ValuesOnItemsReplaced);
      row.Values.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(ValuesOnCollectionReset);
    }

    protected virtual void ValuesOnItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<Tuple<T, double>>> e) {
      this.OnReset();
    }
    protected virtual void ValuesOnItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Tuple<T, double>>> e) {
      this.OnReset();
    }
    protected virtual void ValuesOnItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Tuple<T, double>>> e) {
      this.OnReset();
    }
    protected virtual void ValuesOnItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<Tuple<T, double>>> e) {
      this.OnReset();
    }
    protected virtual void ValuesOnCollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<Tuple<T, double>>> e) {
      this.OnReset();
    }

    #region IStringConvertibleMatrix Members

    int IStringConvertibleMatrix.Rows {
      get { return rows.Count == 0 ? 0 : rows.Max(r => r.Values.Count); }
      set { throw new NotSupportedException(); }
    }
    int IStringConvertibleMatrix.Columns {
      get { return rows.Count * 2; }
      set { throw new NotSupportedException(); }
    }
    IEnumerable<string> IStringConvertibleMatrix.ColumnNames {
      get { return rows.Select(r => new string[] { r.Name + " Index", r.Name }).SelectMany(x => x); }
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
      if (columnIndex < rows.Count * 2) {
        string columnName = ((IStringConvertibleMatrix)this).ColumnNames.ElementAt(columnIndex);
        if (!rows.ContainsKey(columnName) && columnName.Length > " Index".Length) {
          columnName = columnName.Substring(0, columnName.Length - " Index".Length);
          if (rows.ContainsKey(columnName) && rowIndex < rows[columnName].Values.Count)
            return rows[columnName].Values[rowIndex].Item1.ToString();
          else return string.Empty;
        } else if (rows.ContainsKey(columnName) && rowIndex < rows[columnName].Values.Count)
          return rows[columnName].Values[rowIndex].Item2.ToString();
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
