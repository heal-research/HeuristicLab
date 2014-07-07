#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("ScatterPlot", "A scatter plot of 2D data")]
  [StorableClass]
  public class ScatterPlot : NamedItem, IStringConvertibleMatrix {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Performance; }
    }

    private ScatterPlotVisualProperties visualProperties;
    public ScatterPlotVisualProperties VisualProperties {
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

    private NamedItemCollection<ScatterPlotDataRow> rows;
    public NamedItemCollection<ScatterPlotDataRow> Rows {
      get { return rows; }
      private set {
        if (rows != null) throw new InvalidOperationException("Rows already set");
        rows = value;
        if (rows != null) RegisterRowsEvents();
      }
    }

    #region Persistence Properties
    [Storable(Name = "VisualProperties")]
    private ScatterPlotVisualProperties StorableVisualProperties {
      get { return visualProperties; }
      set {
        visualProperties = value;
        visualProperties.PropertyChanged += new PropertyChangedEventHandler(VisualProperties_PropertyChanged);
      }
    }
    [Storable(Name = "Rows")]
    private IEnumerable<ScatterPlotDataRow> StorableRows {
      get { return rows; }
      set { Rows = new NamedItemCollection<ScatterPlotDataRow>(value); }
    }
    #endregion

    [StorableConstructor]
    protected ScatterPlot(bool deserializing) : base(deserializing) { }
    protected ScatterPlot(ScatterPlot original, Cloner cloner)
      : base(original, cloner) {
      VisualProperties = cloner.Clone(original.visualProperties);
      Rows = cloner.Clone(original.rows);
    }
    public ScatterPlot()
      : base() {
      this.Name = ItemName;
      this.Description = ItemDescription;
      VisualProperties = new ScatterPlotVisualProperties();
      Rows = new NamedItemCollection<ScatterPlotDataRow>();
    }
    public ScatterPlot(string name, string description)
      : base(name, description) {
      VisualProperties = new ScatterPlotVisualProperties(name);
      Rows = new NamedItemCollection<ScatterPlotDataRow>();
    }
    public ScatterPlot(string name, string description, ScatterPlotVisualProperties visualProperties)
      : base(name, description) {
      VisualProperties = visualProperties;
      Rows = new NamedItemCollection<ScatterPlotDataRow>();
    }

    // BackwardsCompatibility3.3
    #region Backwards compatible code, remove with 3.4
    private ObservableList<PointF> points;
    [Storable(Name = "points", AllowOneWay = true)]
    private ObservableList<PointF> StorablePoints {
      set { points = value; }
    }
    private string xAxisName;
    [Storable(Name = "xAxisName", AllowOneWay = true)]
    private string StorableXAxisName {
      set { xAxisName = value; }
    }
    private string yAxisName;
    [Storable(Name = "yAxisName", AllowOneWay = true)]
    private string StorableYAxisName {
      set { yAxisName = value; }
    }
    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() {
      if (VisualProperties == null) VisualProperties = new ScatterPlotVisualProperties(name);
      if (string.IsNullOrEmpty(VisualProperties.XAxisTitle) && !string.IsNullOrEmpty(xAxisName)) VisualProperties.XAxisTitle = xAxisName;
      if (string.IsNullOrEmpty(VisualProperties.YAxisTitle) && !string.IsNullOrEmpty(yAxisName)) VisualProperties.YAxisTitle = yAxisName;
      if (rows == null) Rows = new NamedItemCollection<ScatterPlotDataRow>();
      if ((Rows.Count == 0) && (points != null)) Rows.Add(new ScatterPlotDataRow(name, null, points.Select(p => new Point2D<double>(p.X, p.Y))));
      if (string.IsNullOrEmpty(this.name)) this.name = ItemName;
      if (string.IsNullOrEmpty(this.description)) this.description = ItemDescription;
    }
    #endregion

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterPlot(this, cloner);
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
      rows.ItemsAdded += new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(rows_ItemsAdded);
      rows.ItemsRemoved += new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(rows_ItemsRemoved);
      rows.ItemsReplaced += new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(rows_ItemsReplaced);
      rows.CollectionReset += new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(rows_CollectionReset);
    }
    private void rows_ItemsAdded(object sender, CollectionItemsChangedEventArgs<ScatterPlotDataRow> e) {
      foreach (ScatterPlotDataRow row in e.Items)
        this.RegisterRowEvents(row);

      this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    private void rows_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<ScatterPlotDataRow> e) {
      foreach (ScatterPlotDataRow row in e.Items)
        this.DeregisterRowEvents(row);

      this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    private void rows_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<ScatterPlotDataRow> e) {
      foreach (ScatterPlotDataRow row in e.OldItems)
        this.DeregisterRowEvents(row);
      foreach (ScatterPlotDataRow row in e.Items)
        this.RegisterRowEvents(row);

      this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }
    private void rows_CollectionReset(object sender, CollectionItemsChangedEventArgs<ScatterPlotDataRow> e) {
      foreach (ScatterPlotDataRow row in e.OldItems)
        this.DeregisterRowEvents(row);
      foreach (ScatterPlotDataRow row in e.Items)
        this.RegisterRowEvents(row);

      if (e.OldItems.Count() != e.Items.Count())
        this.OnColumnsChanged();
      this.OnColumnNamesChanged();
      this.OnReset();
    }

    protected virtual void RegisterRowEvents(ScatterPlotDataRow row) {
      row.Points.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsAdded);
      row.Points.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsMoved);
      row.Points.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsRemoved);
      row.Points.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsReplaced);
      row.Points.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_CollectionReset);
    }
    protected virtual void DeregisterRowEvents(ScatterPlotDataRow row) {
      row.Points.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsAdded);
      row.Points.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsMoved);
      row.Points.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsRemoved);
      row.Points.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsReplaced);
      row.Points.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_CollectionReset);
    }

    private void Points_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<Point2D<double>>> e) {
      this.OnReset();
    }
    private void Points_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Point2D<double>>> e) {
      this.OnReset();
    }
    private void Points_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Point2D<double>>> e) {
      this.OnReset();
    }
    private void Points_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<Point2D<double>>> e) {
      this.OnReset();
    }
    private void Points_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<Point2D<double>>> e) {
      this.OnReset();
    }

    #region IStringConvertibleMatrix Members
    int IStringConvertibleMatrix.Rows {
      get { return rows.Count == 0 ? 0 : rows.Max(r => r.Points.Count); }
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
        if (rows.ContainsKey(columnName) && rowIndex < rows[columnName].Points.Count)
          return string.Format("{0};{1}", rows[columnName].Points[rowIndex].X, rows[columnName].Points[rowIndex].Y);
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
