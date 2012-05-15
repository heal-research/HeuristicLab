#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  [Item("ScatterPlot", "A scatter plot of 2D data")]
  [StorableClass]
  public class ScatterPlot : NamedItem, IStringConvertibleMatrix {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Performance; }
    }

    [Storable]
    private ObservableList<PointF> points;
    public ObservableList<PointF> Points {
      get { return points; }
    }

    [Storable]
    private string xAxisName;
    public string XAxisName {
      get { return xAxisName; }
      set {
        if (value == xAxisName) return;
        xAxisName = value;
        OnAxesNameChanged();
      }
    }

    [Storable]
    private string yAxisName;
    public string YAxisName {
      get { return yAxisName; }
      set {
        if (value == yAxisName) return;
        yAxisName = value;
        OnAxesNameChanged();
      }
    }

    [StorableConstructor]
    protected ScatterPlot(bool deserializing) : base(deserializing) { }
    protected ScatterPlot(ScatterPlot original, Cloner cloner)
      : base(original, cloner) {
      points = new ObservableList<PointF>(original.points);
      xAxisName = original.xAxisName;
      yAxisName = original.yAxisName;
    }
    public ScatterPlot()
      : base() {
      this.points = new ObservableList<PointF>();
    }
    public ScatterPlot(string name)
      : base(name) {
      this.points = new ObservableList<PointF>();
    }
    public ScatterPlot(string name, string description)
      : base(name, description) {
      this.points = new ObservableList<PointF>();
    }
    public ScatterPlot(string name, string description, string xAxisName, string yAxisName)
      : base(name, description) {
      this.points = new ObservableList<PointF>();
      this.xAxisName = xAxisName;
      this.yAxisName = yAxisName;
    }
    public ScatterPlot(IEnumerable<PointF> points)
      : base() {
      this.points = new ObservableList<PointF>(points);
    }
    public ScatterPlot(IEnumerable<PointF> points, string name)
      : base(name) {
      this.points = new ObservableList<PointF>(points);
    }
    public ScatterPlot(IEnumerable<PointF> points, string name, string description)
      : base(name, description) {
      this.points = new ObservableList<PointF>(points);
    }
    public ScatterPlot(IEnumerable<PointF> points, string name, string description, string xAxisName, string yAxisName)
      : base(name, description) {
      this.points = new ObservableList<PointF>(points);
      this.xAxisName = xAxisName;
      this.yAxisName = yAxisName;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterPlot(this, cloner);
    }

    public event EventHandler AxisNameChanged;
    protected virtual void OnAxesNameChanged() {
      EventHandler handler = AxisNameChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    #region IStringConvertibleMatrix Members

    int IStringConvertibleMatrix.Rows {
      get { return points.Count; }
      set { throw new NotSupportedException(); }
    }
    int IStringConvertibleMatrix.Columns {
      get { return 2; }
      set { throw new NotSupportedException(); }
    }
    IEnumerable<string> IStringConvertibleMatrix.ColumnNames {
      get { return new string[] { xAxisName, yAxisName }; }
      set { throw new NotSupportedException(); }
    }
    IEnumerable<string> IStringConvertibleMatrix.RowNames {
      get { return Enumerable.Empty<string>(); }
      set { throw new NotSupportedException(); }
    }

    bool IStringConvertibleMatrix.SortableView {
      get { return false; }
      set { throw new NotSupportedException(); }
    }
    bool IStringConvertibleMatrix.ReadOnly {
      get { return true; }
    }

    string IStringConvertibleMatrix.GetValue(int rowIndex, int columnIndex) {
      if (rowIndex < points.Count && columnIndex < 2) {
        return columnIndex == 0 ? points[rowIndex].X.ToString() : points[rowIndex].Y.ToString();
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
