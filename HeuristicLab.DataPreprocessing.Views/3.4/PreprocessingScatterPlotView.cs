#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Analysis;
using HeuristicLab.Analysis.Views;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {
  [View("Preprocessing ScatterPlot View")]
  [Content(typeof(ScatterPlot), false)]
  public partial class PreprocessingScatterPlotView : ItemView, IConfigureableView {
    protected List<Series> invisibleSeries;
    protected Dictionary<IObservableList<Point2D<double>>, ScatterPlotDataRow> pointsRowsTable;
    private event EventHandler chartDoubleClick;

    public new ScatterPlot Content {
      get { return (ScatterPlot)base.Content; }
      set { base.Content = value; }
    }

    public PreprocessingScatterPlotView() {
      InitializeComponent();
      pointsRowsTable = new Dictionary<IObservableList<Point2D<double>>, ScatterPlotDataRow>();
      invisibleSeries = new List<Series>();
      chart.CustomizeAllChartAreas();
      chart.ChartAreas[0].CursorX.Interval = 1;
    }

    #region Event Handler Registration
    protected override void DeregisterContentEvents() {
      foreach (ScatterPlotDataRow row in Content.Rows)
        DeregisterScatterPlotDataRowEvents(row);
      Content.VisualPropertiesChanged -= new EventHandler(Content_VisualPropertiesChanged);
      Content.Rows.ItemsAdded -= new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_ItemsAdded);
      Content.Rows.ItemsRemoved -= new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_ItemsRemoved);
      Content.Rows.ItemsReplaced -= new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_ItemsReplaced);
      Content.Rows.CollectionReset -= new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_CollectionReset);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.VisualPropertiesChanged += new EventHandler(Content_VisualPropertiesChanged);
      Content.Rows.ItemsAdded += new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_ItemsAdded);
      Content.Rows.ItemsRemoved += new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_ItemsRemoved);
      Content.Rows.ItemsReplaced += new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_ItemsReplaced);
      Content.Rows.CollectionReset += new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_CollectionReset);
    }

    protected virtual void RegisterScatterPlotDataRowEvents(ScatterPlotDataRow row) {
      row.NameChanged += new EventHandler(Row_NameChanged);
      row.VisualPropertiesChanged += new EventHandler(Row_VisualPropertiesChanged);
      pointsRowsTable.Add(row.Points, row);
      row.Points.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsAdded);
      row.Points.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsRemoved);
      row.Points.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsReplaced);
      row.Points.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_CollectionReset);
    }
    protected virtual void DeregisterScatterPlotDataRowEvents(ScatterPlotDataRow row) {
      row.Points.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsAdded);
      row.Points.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsRemoved);
      row.Points.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsReplaced);
      row.Points.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_CollectionReset);
      pointsRowsTable.Remove(row.Points);
      row.VisualPropertiesChanged -= new EventHandler(Row_VisualPropertiesChanged);
      row.NameChanged -= new EventHandler(Row_NameChanged);
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      invisibleSeries.Clear();
      chart.ChartAreas[0].AxisX.Title = string.Empty;
      chart.ChartAreas[0].AxisY.Title = string.Empty;
      chart.Series.Clear();
      if (Content != null) {
        AddScatterPlotDataRows(Content.Rows);
        ConfigureChartArea(chart.ChartAreas[0]);
        RecalculateAxesScale(chart.ChartAreas[0]);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      chart.Enabled = Content != null;
    }

    public void ShowConfiguration() {
      if (Content != null) {
        using (ScatterPlotVisualPropertiesDialog dialog = new ScatterPlotVisualPropertiesDialog(Content)) {
          dialog.ShowDialog(this);
        }
      } else MessageBox.Show("Nothing to configure.");
    }

    protected virtual void AddScatterPlotDataRows(IEnumerable<ScatterPlotDataRow> rows) {
      foreach (var row in rows) {
        RegisterScatterPlotDataRowEvents(row);
        Series series = new Series(row.Name);
        if (row.VisualProperties.DisplayName.Trim() != String.Empty) series.LegendText = row.VisualProperties.DisplayName;
        else series.LegendText = row.Name;
        ConfigureSeries(series, row);
        FillSeriesWithRowValues(series, row);
        chart.Series.Add(series);
      }
      ConfigureChartArea(chart.ChartAreas[0]);
      RecalculateAxesScale(chart.ChartAreas[0]);
      UpdateYCursorInterval();
    }

    protected virtual void RemoveScatterPlotDataRows(IEnumerable<ScatterPlotDataRow> rows) {
      foreach (var row in rows) {
        DeregisterScatterPlotDataRowEvents(row);
        Series series = chart.Series[row.Name];
        chart.Series.Remove(series);
        if (invisibleSeries.Contains(series))
          invisibleSeries.Remove(series);
      }
      RecalculateAxesScale(chart.ChartAreas[0]);
    }

    private void ConfigureSeries(Series series, ScatterPlotDataRow row) {
      series.BorderWidth = 1;
      series.BorderDashStyle = ChartDashStyle.Solid;
      series.BorderColor = Color.Empty;

      if (row.VisualProperties.Color != Color.Empty)
        series.Color = row.VisualProperties.Color;
      else series.Color = Color.Empty;
      series.IsVisibleInLegend = row.VisualProperties.IsVisibleInLegend;
      series.ChartType = SeriesChartType.FastPoint;
      series.MarkerSize = row.VisualProperties.PointSize;
      series.MarkerStyle = ConvertPointStyle(row.VisualProperties.PointStyle);
      series.XAxisType = AxisType.Primary;
      series.YAxisType = AxisType.Primary;

      if (row.VisualProperties.DisplayName.Trim() != String.Empty) series.LegendText = row.VisualProperties.DisplayName;
      else series.LegendText = row.Name;

      string xAxisTitle = string.IsNullOrEmpty(Content.VisualProperties.XAxisTitle)
                      ? "X"
                      : Content.VisualProperties.XAxisTitle;
      string yAxisTitle = string.IsNullOrEmpty(Content.VisualProperties.YAxisTitle)
                            ? "Y"
                            : Content.VisualProperties.YAxisTitle;
      series.ToolTip =
        series.LegendText + Environment.NewLine +
        xAxisTitle + " = " + "#VALX," + Environment.NewLine +
        yAxisTitle + " = " + "#VAL";
    }

    private void ConfigureChartArea(ChartArea area) {

      if (Content.VisualProperties.AxisTitleFont != null) area.AxisX.TitleFont = Content.VisualProperties.AxisTitleFont;
      if (!Content.VisualProperties.AxisTitleColor.IsEmpty) area.AxisX.TitleForeColor = Content.VisualProperties.AxisTitleColor;
      area.AxisX.Title = Content.VisualProperties.XAxisTitle;
      area.AxisX.MajorGrid.Enabled = Content.VisualProperties.XAxisGrid;

      if (Content.VisualProperties.AxisTitleFont != null) area.AxisY.TitleFont = Content.VisualProperties.AxisTitleFont;
      if (!Content.VisualProperties.AxisTitleColor.IsEmpty) area.AxisY.TitleForeColor = Content.VisualProperties.AxisTitleColor;
      area.AxisY.Title = Content.VisualProperties.YAxisTitle;
      area.AxisY.MajorGrid.Enabled = Content.VisualProperties.YAxisGrid;
    }

    private void RecalculateAxesScale(ChartArea area) {
      // Reset the axes bounds so that RecalculateAxesScale() will assign new bounds
      foreach (Axis a in area.Axes) {
        a.Minimum = double.NaN;
        a.Maximum = double.NaN;
      }
      area.RecalculateAxesScale();
      area.AxisX.IsMarginVisible = false;

      if (!Content.VisualProperties.XAxisMinimumAuto && !double.IsNaN(Content.VisualProperties.XAxisMinimumFixedValue)) area.AxisX.Minimum = Content.VisualProperties.XAxisMinimumFixedValue;
      if (!Content.VisualProperties.XAxisMaximumAuto && !double.IsNaN(Content.VisualProperties.XAxisMaximumFixedValue)) area.AxisX.Maximum = Content.VisualProperties.XAxisMaximumFixedValue;
      if (!Content.VisualProperties.YAxisMinimumAuto && !double.IsNaN(Content.VisualProperties.YAxisMinimumFixedValue)) area.AxisY.Minimum = Content.VisualProperties.YAxisMinimumFixedValue;
      if (!Content.VisualProperties.YAxisMaximumAuto && !double.IsNaN(Content.VisualProperties.YAxisMaximumFixedValue)) area.AxisY.Maximum = Content.VisualProperties.YAxisMaximumFixedValue;
      if (area.AxisX.Minimum >= area.AxisX.Maximum) area.AxisX.Maximum = area.AxisX.Minimum + 1;
      if (area.AxisY.Minimum >= area.AxisY.Maximum) area.AxisY.Maximum = area.AxisY.Minimum + 1;
    }

    protected virtual void UpdateYCursorInterval() {
      double interestingValuesRange = (
        from series in chart.Series
        where series.Enabled
        let values = (from point in series.Points
                      where !point.IsEmpty
                      select point.YValues[0]).DefaultIfEmpty(1.0)
        let range = values.Max() - values.Min()
        where range > 0.0
        select range
        ).DefaultIfEmpty(1.0).Min();

      double digits = (int)Math.Log10(interestingValuesRange) - 3;
      double yZoomInterval = Math.Pow(10, digits);
      this.chart.ChartAreas[0].CursorY.Interval = yZoomInterval;
    }


    public event EventHandler ChartDoubleClick {
      add { chartDoubleClick += value; }
      remove { chartDoubleClick -= value; }
    }

    #region Event Handlers
    #region Content Event Handlers

    private void Content_VisualPropertiesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_VisualPropertiesChanged), sender, e);
      else {
        ConfigureChartArea(chart.ChartAreas[0]);
        RecalculateAxesScale(chart.ChartAreas[0]); // axes min/max could have changed
      }
    }
    #endregion
    #region Rows Event Handlers
    private void Rows_ItemsAdded(object sender, CollectionItemsChangedEventArgs<ScatterPlotDataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_ItemsAdded), sender, e);
      else {
        AddScatterPlotDataRows(e.Items);
      }
    }
    private void Rows_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<ScatterPlotDataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_ItemsRemoved), sender, e);
      else {
        RemoveScatterPlotDataRows(e.Items);
      }
    }
    private void Rows_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<ScatterPlotDataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_ItemsReplaced), sender, e);
      else {
        RemoveScatterPlotDataRows(e.OldItems);
        AddScatterPlotDataRows(e.Items);
      }
    }
    private void Rows_CollectionReset(object sender, CollectionItemsChangedEventArgs<ScatterPlotDataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<ScatterPlotDataRow>(Rows_CollectionReset), sender, e);
      else {
        RemoveScatterPlotDataRows(e.OldItems);
        AddScatterPlotDataRows(e.Items);
      }
    }
    #endregion
    #region Row Event Handlers
    private void Row_VisualPropertiesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Row_VisualPropertiesChanged), sender, e);
      else {
        ScatterPlotDataRow row = (ScatterPlotDataRow)sender;
        Series series = chart.Series[row.Name];
        series.Points.Clear();
        ConfigureSeries(series, row);
        FillSeriesWithRowValues(series, row);
        RecalculateAxesScale(chart.ChartAreas[0]);
      }
    }
    private void Row_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Row_NameChanged), sender, e);
      else {
        ScatterPlotDataRow row = (ScatterPlotDataRow)sender;
        chart.Series[row.Name].Name = row.Name;
      }
    }
    #endregion
    #region Points Event Handlers
    private void Points_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<Point2D<double>>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsAdded), sender, e);
      else {
        ScatterPlotDataRow row = null;
        pointsRowsTable.TryGetValue((IObservableList<Point2D<double>>)sender, out row);
        if (row != null) {
          Series rowSeries = chart.Series[row.Name];
          if (!invisibleSeries.Contains(rowSeries)) {
            rowSeries.Points.Clear();
            FillSeriesWithRowValues(rowSeries, row);
            RecalculateAxesScale(chart.ChartAreas[0]);
            UpdateYCursorInterval();
          }
        }
      }
    }
    private void Points_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Point2D<double>>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsRemoved), sender, e);
      else {
        ScatterPlotDataRow row = null;
        pointsRowsTable.TryGetValue((IObservableList<Point2D<double>>)sender, out row);
        if (row != null) {
          Series rowSeries = chart.Series[row.Name];
          if (!invisibleSeries.Contains(rowSeries)) {
            rowSeries.Points.Clear();
            FillSeriesWithRowValues(rowSeries, row);
            RecalculateAxesScale(chart.ChartAreas[0]);
            UpdateYCursorInterval();
          }
        }
      }
    }
    private void Points_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<Point2D<double>>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_ItemsReplaced), sender, e);
      else {
        ScatterPlotDataRow row = null;
        pointsRowsTable.TryGetValue((IObservableList<Point2D<double>>)sender, out row);
        if (row != null) {
          Series rowSeries = chart.Series[row.Name];
          if (!invisibleSeries.Contains(rowSeries)) {
            rowSeries.Points.Clear();
            FillSeriesWithRowValues(rowSeries, row);
            RecalculateAxesScale(chart.ChartAreas[0]);
            UpdateYCursorInterval();
          }
        }
      }
    }
    private void Points_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<Point2D<double>>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Point2D<double>>>(Points_CollectionReset), sender, e);
      else {
        ScatterPlotDataRow row = null;
        pointsRowsTable.TryGetValue((IObservableList<Point2D<double>>)sender, out row);
        if (row != null) {
          Series rowSeries = chart.Series[row.Name];
          if (!invisibleSeries.Contains(rowSeries)) {
            rowSeries.Points.Clear();
            FillSeriesWithRowValues(rowSeries, row);
            RecalculateAxesScale(chart.ChartAreas[0]);
            UpdateYCursorInterval();
          }
        }
      }
    }
    #endregion
    #endregion

    #region Chart Event Handlers
    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        ToggleSeriesVisible(result.Series);
      }
    }
    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem)
        this.Cursor = Cursors.Hand;
      else
        this.Cursor = Cursors.Default;
    }
    private void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e) {
      foreach (LegendItem legendItem in e.LegendItems) {
        var series = chart.Series[legendItem.SeriesName];
        if (series != null) {
          bool seriesIsInvisible = invisibleSeries.Contains(series);
          foreach (LegendCell cell in legendItem.Cells) {
            cell.ForeColor = seriesIsInvisible ? Color.Gray : Color.Black;
          }
        }
      }
    }
    #endregion

    private void ToggleSeriesVisible(Series series) {
      if (!invisibleSeries.Contains(series)) {
        series.Points.Clear();
        invisibleSeries.Add(series);
      } else {
        invisibleSeries.Remove(series);
        if (Content != null) {

          var row = (from r in Content.Rows
                     where r.Name == series.Name
                     select r).Single();
          FillSeriesWithRowValues(series, row);
          this.chart.Legends[series.Legend].ForeColor = Color.Black;
          RecalculateAxesScale(chart.ChartAreas[0]);
          UpdateYCursorInterval();
        }
      }
    }

    private void FillSeriesWithRowValues(Series series, ScatterPlotDataRow row) {
      for (int i = 0; i < row.Points.Count; i++) {
        var value = row.Points[i];
        DataPoint point = new DataPoint();
        if (IsInvalidValue(value.X) || IsInvalidValue(value.Y))
          point.IsEmpty = true;
        else {
          point.XValue = value.X;
          point.YValues = new double[] { value.Y };
        }
        series.Points.Add(point);
      }
    }

    #region Helpers
    private MarkerStyle ConvertPointStyle(ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle pointStyle) {
      switch (pointStyle) {
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Circle:
          return MarkerStyle.Circle;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Cross:
          return MarkerStyle.Cross;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Diamond:
          return MarkerStyle.Diamond;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Square:
          return MarkerStyle.Square;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Star4:
          return MarkerStyle.Star4;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Star5:
          return MarkerStyle.Star5;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Star6:
          return MarkerStyle.Star6;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Star10:
          return MarkerStyle.Star10;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowPointStyle.Triangle:
          return MarkerStyle.Triangle;
        default:
          return MarkerStyle.None;
      }
    }

    protected static bool IsInvalidValue(double x) {
      return double.IsNaN(x) || x < (double)decimal.MinValue || x > (double)decimal.MaxValue;
    }
    #endregion

    //bubble double click event with scatter plot view as sender
    private void chart_DoubleClick(object sender, EventArgs e) {
      if (chartDoubleClick != null)
        chartDoubleClick(this, e);
    } 
  }
}
