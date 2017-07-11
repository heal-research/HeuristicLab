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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("ScatterPlot View")]
  [Content(typeof(ScatterPlot), true)]
  public partial class ScatterPlotView : NamedItemView, IConfigureableView {
    protected List<Series> invisibleSeries;
    protected Dictionary<IObservableList<Point2D<double>>, ScatterPlotDataRow> pointsRowsTable;
    protected Dictionary<Series, Series> seriesToRegressionSeriesTable;
    private double xMin, xMax, yMin, yMax;

    public new ScatterPlot Content {
      get { return (ScatterPlot)base.Content; }
      set { base.Content = value; }
    }

    public bool ShowName {
      get { return nameTextBox.Visible; }
      set {
        if (nameTextBox.Visible != value) {
          foreach (Control c in Controls) {
            if (c == chart) continue;
            c.Visible = value;
          }
          chart.Dock = value ? DockStyle.None : DockStyle.Fill;
        }
      }
    }

    public ScatterPlotView() {
      InitializeComponent();
      pointsRowsTable = new Dictionary<IObservableList<Point2D<double>>, ScatterPlotDataRow>();
      seriesToRegressionSeriesTable = new Dictionary<Series, Series>();
      invisibleSeries = new List<Series>();
      chart.CustomizeAllChartAreas();
      chart.ChartAreas[0].CursorX.Interval = 1;
      chart.ContextMenuStrip.Items.Add(configureToolStripMenuItem);
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
      chart.Titles[0].Text = string.Empty;
      chart.ChartAreas[0].AxisX.Title = string.Empty;
      chart.ChartAreas[0].AxisY.Title = string.Empty;
      chart.Series.Clear();
      if (Content != null) {
        chart.Titles[0].Text = Content.Name;
        chart.Titles[0].Visible = !string.IsNullOrEmpty(Content.Name);
        AddScatterPlotDataRows(Content.Rows);
        ConfigureChartArea(chart.ChartAreas[0]);
        RecalculateMinMaxPointValues();
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
        Series series = new Series(row.Name) { Tag = row };

        if (row.VisualProperties.DisplayName.Trim() != String.Empty) series.LegendText = row.VisualProperties.DisplayName;
        else series.LegendText = row.Name;

        var regressionSeries = new Series(row.Name + "_Regression") {
          Tag = row,
          ChartType = SeriesChartType.Line,
          BorderDashStyle = ChartDashStyle.Dot,
          IsVisibleInLegend = false,
          Color = Color.Transparent // to avoid auto color assignment via color palette 
        };

        seriesToRegressionSeriesTable.Add(series, regressionSeries);
        ConfigureSeries(series, regressionSeries, row);
        FillSeriesWithRowValues(series, row);

        chart.Series.Add(series);
        chart.Series.Add(regressionSeries);
        FillRegressionSeries(regressionSeries, row);
      }
      ConfigureChartArea(chart.ChartAreas[0]);
      RecalculateMinMaxPointValues();
      RecalculateAxesScale(chart.ChartAreas[0]);
      UpdateYCursorInterval();
      UpdateRegressionSeriesColors();
    }

    protected virtual void RemoveScatterPlotDataRows(IEnumerable<ScatterPlotDataRow> rows) {
      foreach (var row in rows) {
        DeregisterScatterPlotDataRowEvents(row);
        Series series = chart.Series[row.Name];
        chart.Series.Remove(series);
        if (invisibleSeries.Contains(series))
          invisibleSeries.Remove(series);
        chart.Series.Remove(seriesToRegressionSeriesTable[series]);
        seriesToRegressionSeriesTable.Remove(series);
      }
      RecalculateMinMaxPointValues();
      RecalculateAxesScale(chart.ChartAreas[0]);
    }

    private void ConfigureSeries(Series series, Series regressionSeries, ScatterPlotDataRow row) {
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

      regressionSeries.BorderWidth = Math.Max(1, row.VisualProperties.PointSize / 2);
      regressionSeries.IsVisibleInLegend = row.VisualProperties.IsRegressionVisibleInLegend &&
        row.VisualProperties.RegressionType != ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.None;
      regressionSeries.LegendText = string.IsNullOrEmpty(row.VisualProperties.RegressionDisplayName)
        ? string.Format("{0}({1})", row.VisualProperties.RegressionType, row.Name)
        : row.VisualProperties.RegressionDisplayName;
    }

    private void ConfigureChartArea(ChartArea area) {
      if (Content.VisualProperties.TitleFont != null) chart.Titles[0].Font = Content.VisualProperties.TitleFont;
      if (!Content.VisualProperties.TitleColor.IsEmpty) chart.Titles[0].ForeColor = Content.VisualProperties.TitleColor;
      chart.Titles[0].Text = Content.VisualProperties.Title;
      chart.Titles[0].Visible = !string.IsNullOrEmpty(Content.VisualProperties.Title);

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
      area.AxisX.Minimum = CalculateMinBound(xMin);
      area.AxisX.Maximum = CalculateMaxBound(xMax);
      if (area.AxisX.Minimum == area.AxisX.Maximum) {
        area.AxisX.Minimum = xMin - 0.5;
        area.AxisX.Maximum = xMax + 0.5;
      }
      area.AxisY.Minimum = CalculateMinBound(yMin);
      area.AxisY.Maximum = CalculateMaxBound(yMax);
      if (area.AxisY.Minimum == area.AxisY.Maximum) {
        area.AxisY.Minimum = yMin - 0.5;
        area.AxisY.Maximum = yMax + 0.5;
      }
      if (xMax - xMin > 0) area.CursorX.Interval = Math.Pow(10, Math.Floor(Math.Log10(area.AxisX.Maximum - area.AxisX.Minimum) - 3));
      else area.CursorX.Interval = 1;
      area.AxisX.IsMarginVisible = false;

      if (!Content.VisualProperties.XAxisMinimumAuto && !double.IsNaN(Content.VisualProperties.XAxisMinimumFixedValue)) area.AxisX.Minimum = Content.VisualProperties.XAxisMinimumFixedValue;
      if (!Content.VisualProperties.XAxisMaximumAuto && !double.IsNaN(Content.VisualProperties.XAxisMaximumFixedValue)) area.AxisX.Maximum = Content.VisualProperties.XAxisMaximumFixedValue;
      if (!Content.VisualProperties.YAxisMinimumAuto && !double.IsNaN(Content.VisualProperties.YAxisMinimumFixedValue)) area.AxisY.Minimum = Content.VisualProperties.YAxisMinimumFixedValue;
      if (!Content.VisualProperties.YAxisMaximumAuto && !double.IsNaN(Content.VisualProperties.YAxisMaximumFixedValue)) area.AxisY.Maximum = Content.VisualProperties.YAxisMaximumFixedValue;
    }

    private static double CalculateMinBound(double min) {
      if (min == 0) return 0;
      var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(min))));
      return scale * (Math.Floor(min / scale));
    }

    private static double CalculateMaxBound(double max) {
      if (max == 0) return 0;
      var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(max))));
      return scale * (Math.Ceiling(max / scale));
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

    protected void UpdateRegressionSeriesColors() {
      chart.ApplyPaletteColors();
      foreach (var row in Content.Rows) {
        var series = chart.Series[row.Name];
        var regressionSeries = seriesToRegressionSeriesTable[series];
        regressionSeries.Color = series.Color;
      }
    }

    #region Event Handlers
    #region Content Event Handlers
    protected override void Content_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_NameChanged), sender, e);
      else {
        Content.VisualProperties.Title = Content.Name;
        base.Content_NameChanged(sender, e);
      }
    }
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
        Series regressionSeries = seriesToRegressionSeriesTable[series];
        series.Points.Clear();
        regressionSeries.Points.Clear();
        ConfigureSeries(series, regressionSeries, row);
        FillSeriesWithRowValues(series, row);
        FillRegressionSeries(regressionSeries, row);
        RecalculateMinMaxPointValues();
        RecalculateAxesScale(chart.ChartAreas[0]);
        UpdateRegressionSeriesColors();
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
          Series regressionSeries = seriesToRegressionSeriesTable[rowSeries];
          if (!invisibleSeries.Contains(rowSeries)) {
            rowSeries.Points.Clear();
            regressionSeries.Points.Clear();
            FillSeriesWithRowValues(rowSeries, row);
            FillRegressionSeries(regressionSeries, row);
            RecalculateMinMaxPointValues();
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
          Series regressionSeries = seriesToRegressionSeriesTable[rowSeries];
          if (!invisibleSeries.Contains(rowSeries)) {
            rowSeries.Points.Clear();
            regressionSeries.Points.Clear();
            FillSeriesWithRowValues(rowSeries, row);
            FillRegressionSeries(regressionSeries, row);
            RecalculateMinMaxPointValues();
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
          Series regressionSeries = seriesToRegressionSeriesTable[rowSeries];
          if (!invisibleSeries.Contains(rowSeries)) {
            rowSeries.Points.Clear();
            regressionSeries.Points.Clear();
            FillSeriesWithRowValues(rowSeries, row);
            FillRegressionSeries(regressionSeries, row);
            RecalculateMinMaxPointValues();
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
          Series regressionSeries = seriesToRegressionSeriesTable[rowSeries];
          if (!invisibleSeries.Contains(rowSeries)) {
            rowSeries.Points.Clear();
            regressionSeries.Points.Clear();
            FillSeriesWithRowValues(rowSeries, row);
            FillRegressionSeries(regressionSeries, row);
            RecalculateMinMaxPointValues();
            RecalculateAxesScale(chart.ChartAreas[0]);
            UpdateYCursorInterval();
          }
        }
      }
    }
    #endregion
    private void configureToolStripMenuItem_Click(object sender, System.EventArgs e) {
      ShowConfiguration();
    }
    #endregion

    #region Chart Event Handlers
    private void chart_MouseDoubleClick(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y, ChartElementType.DataPoint);
      if (result.ChartElementType != ChartElementType.DataPoint) return;

      var series = result.Series;
      var dataPoint = series.Points[result.PointIndex];
      var tag = dataPoint.Tag;
      var content = tag as IContent;

      if (tag == null) return;
      if (content == null) return;

      MainFormManager.MainForm.ShowContent(content);
    }

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
        RecalculateMinMaxPointValues();
      } else {
        invisibleSeries.Remove(series);
        if (Content != null) {
          var row = (ScatterPlotDataRow)series.Tag;
          if (seriesToRegressionSeriesTable.ContainsKey(series))
            FillSeriesWithRowValues(series, row);
          else
            FillRegressionSeries(series, row);
          RecalculateMinMaxPointValues();
          this.chart.Legends[series.Legend].ForeColor = Color.Black;
          RecalculateAxesScale(chart.ChartAreas[0]);
          UpdateYCursorInterval();
        }
      }
    }

    private void RecalculateMinMaxPointValues() {
      yMin = xMin = double.MaxValue;
      yMax = xMax = double.MinValue;
      foreach (var s in chart.Series.Where(x => x.Enabled)) {
        foreach (var p in s.Points) {
          double x = p.XValue, y = p.YValues[0];
          if (xMin > x) xMin = x;
          if (xMax < x) xMax = x;
          if (yMin > y) yMin = y;
          if (yMax < y) yMax = y;
        }
      }
    }

    private void FillSeriesWithRowValues(Series series, ScatterPlotDataRow row) {
      bool zerosOnly = true;
      for (int i = 0; i < row.Points.Count; i++) {
        var value = row.Points[i];
        DataPoint point = new DataPoint();
        if (IsInvalidValue(value.X) || IsInvalidValue(value.Y))
          point.IsEmpty = true;
        else {
          point.XValue = value.X;
          point.YValues = new double[] { value.Y };
        }
        point.Tag = value.Tag;
        series.Points.Add(point);
        if (value.X != 0.0f)
          zerosOnly = false;
      }
      if (zerosOnly) // if all x-values are zero, the x-values are interpreted as 1, 2, 3, ...
        series.Points.Add(new DataPoint(1, 1) { IsEmpty = true });
      double correlation = Correlation(row.Points);
      series.LegendToolTip = string.Format("Correlation (R²) = {0:G4}", correlation * correlation);
    }

    private void FillRegressionSeries(Series regressionSeries, ScatterPlotDataRow row) {
      if (row.VisualProperties.RegressionType == ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.None
        || invisibleSeries.Contains(regressionSeries))
        return;

      double[] coefficients;
      if (!Fitting(row, out coefficients)) {
        regressionSeries.LegendToolTip = "Could not calculate regression.";
        return;
      }

      // Fill regrssion series
      double min = row.Points.Min(p => p.X), max = row.Points.Max(p => p.X);
      double range = max - min, delta = range / Math.Max(row.Points.Count - 1, 50);
      if (range > double.Epsilon) {
        for (double x = min; x <= max; x += delta) {
          regressionSeries.Points.AddXY(x, Estimate(x, row, coefficients));
        }
      }

      // Correlation
      var data = row.Points.Select(p => new Point2D<double>(p.Y, Estimate(p.X, row, coefficients)));
      double correlation = Correlation(data.ToList());
      regressionSeries.LegendToolTip = GetStringFormula(row, coefficients) + Environment.NewLine +
                                       string.Format("Correlation (R²) = {0:G4}", correlation * correlation);
      regressionSeries.ToolTip = GetStringFormula(row, coefficients);
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

    #region Correlation and Fitting Helper
    protected static double Correlation(IList<Point2D<double>> values) {
      // sums of x, y, x squared etc.
      double sx = 0.0;
      double sy = 0.0;
      double sxx = 0.0;
      double syy = 0.0;
      double sxy = 0.0;

      int n = 0;
      for (int i = 0; i < values.Count; i++) {
        double x = values[i].X;
        double y = values[i].Y;
        if (IsInvalidValue(x) || IsInvalidValue(y))
          continue;

        sx += x;
        sy += y;
        sxx += x * x;
        syy += y * y;
        sxy += x * y;
        n++;
      }

      // covariation
      double cov = sxy / n - sx * sy / n / n;
      // standard error of x
      double sigmaX = Math.Sqrt(sxx / n - sx * sx / n / n);
      // standard error of y
      double sigmaY = Math.Sqrt(syy / n - sy * sy / n / n);

      // correlation
      return cov / sigmaX / sigmaY;
    }

    protected static bool Fitting(ScatterPlotDataRow row, out double[] coefficients) {
      if (!IsValidRegressionData(row)) {
        coefficients = new double[0];
        return false;
      }

      var xs = row.Points.Select(p => p.X).ToList();
      var ys = row.Points.Select(p => p.Y).ToList();

      // Input transformations
      double[,] matrix;
      int nRows;
      switch (row.VisualProperties.RegressionType) {
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Linear:
          matrix = CreateMatrix(out nRows, ys, xs);
          break;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Polynomial:
          var xss = Enumerable.Range(1, row.VisualProperties.PolynomialRegressionOrder)
            .Select(o => xs.Select(x => Math.Pow(x, o)).ToList())
            .Reverse(); // higher order first
          matrix = CreateMatrix(out nRows, ys, xss.ToArray());
          break;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Exponential:
          matrix = CreateMatrix(out nRows, ys.Select(y => Math.Log(y)).ToList(), xs);
          break;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Power:
          matrix = CreateMatrix(out nRows, ys.Select(y => Math.Log(y)).ToList(), xs.Select(x => Math.Log(x)).ToList());
          break;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Logarithmic:
          matrix = CreateMatrix(out nRows, ys, xs.Select(x => Math.Log(x)).ToList());
          break;
        default:
          throw new ArgumentException("Unknown RegressionType: " + row.VisualProperties.RegressionType);
      }

      // Linear fitting
      bool success = LinearFitting(matrix, nRows, out coefficients);
      if (!success) return false;

      // Output transformation
      switch (row.VisualProperties.RegressionType) {
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Exponential:
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Power:
          coefficients[1] = Math.Exp(coefficients[1]);
          break;
      }

      return true;
    }
    protected static bool IsValidRegressionData(ScatterPlotDataRow row) {
      // No invalid values allowed
      for (int i = 0; i < row.Points.Count; i++) {
        if (IsInvalidValue(row.Points[i].X) || IsInvalidValue(row.Points[i].Y))
          return false;
      }
      // Exp, Power and Log Regression do not work with negative values
      switch (row.VisualProperties.RegressionType) {
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Exponential:
          for (int i = 0; i < row.Points.Count; i++) {
            if (row.Points[i].Y <= 0)
              return false;
          }
          break;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Power:
          for (int i = 0; i < row.Points.Count; i++) {
            if (row.Points[i].X <= 0 || row.Points[i].Y <= 0)
              return false;
          }
          break;
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Logarithmic:
          for (int i = 0; i < row.Points.Count; i++) {
            if (row.Points[i].X <= 0)
              return false;
          }
          break;
      }
      return true;
    }
    protected static double[,] CreateMatrix(out int nRows, IList<double> ys, params IList<double>[] xss) {
      var matrix = new double[ys.Count, xss.Length + 1];
      int rowIdx = 0;
      for (int i = 0; i < ys.Count; i++) {
        if (IsInvalidValue(ys[i]) || xss.Any(xs => IsInvalidValue(xs[i])))
          continue;
        for (int j = 0; j < xss.Length; j++) {
          matrix[rowIdx, j] = xss[j][i];
        }
        matrix[rowIdx, xss.Length] = ys[i];
        rowIdx++;
      }
      nRows = rowIdx;
      return matrix;
    }

    protected static bool LinearFitting(double[,] xsy, int nRows, out double[] coefficients) {
      int nFeatures = xsy.GetLength(1) - 1;

      alglib.linearmodel lm;
      alglib.lrreport ar;
      int retVal;
      alglib.lrbuild(xsy, nRows, nFeatures, out retVal, out lm, out ar);
      if (retVal != 1) {
        coefficients = new double[0];
        return false;
      }

      alglib.lrunpack(lm, out coefficients, out nFeatures);
      return true;
    }

    protected static double Estimate(double x, ScatterPlotDataRow row, double[] coefficients) {
      switch (row.VisualProperties.RegressionType) {
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Linear:
          return coefficients[0] * x + coefficients[1];
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Polynomial:
          return coefficients
            .Reverse() // to match index and order
            .Select((c, o) => c * Math.Pow(x, o))
            .Sum();
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Exponential:
          return coefficients[1] * Math.Exp(coefficients[0] * x);
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Power:
          return coefficients[1] * Math.Pow(x, coefficients[0]);
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Logarithmic:
          return coefficients[0] * Math.Log(x) + coefficients[1];
        default:
          throw new ArgumentException("Unknown RegressionType: " + row.VisualProperties.RegressionType);
      }
    }

    protected static string GetStringFormula(ScatterPlotDataRow row, double[] coefficients) {
      switch (row.VisualProperties.RegressionType) {
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Linear:
          return string.Format("{0:G4} x {1} {2:G4}", coefficients[0], Sign(coefficients[1]), Math.Abs(coefficients[1]));
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Polynomial:
          var sb = new StringBuilder();
          sb.AppendFormat("{0:G4}{1}", coefficients[0], PolyFactor(coefficients.Length - 1));
          foreach (var x in coefficients
            .Reverse() // match index and order
            .Select((c, o) => new { c, o })
            .Reverse() // higher order first
            .Skip(1)) // highest order poly already added
            sb.AppendFormat(" {0} {1:G4}{2}", Sign(x.c), Math.Abs(x.c), PolyFactor(x.o));
          return sb.ToString();
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Exponential:
          return string.Format("{0:G4} e^({1:G4} x)", coefficients[1], coefficients[0]);
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Power:
          return string.Format("{0:G4} x^({1:G4})", coefficients[1], coefficients[0]);
        case ScatterPlotDataRowVisualProperties.ScatterPlotDataRowRegressionType.Logarithmic:
          return string.Format("{0:G4} ln(x) {1} {2:G4}", coefficients[0], Sign(coefficients[1]), Math.Abs(coefficients[1]));
        default:
          throw new ArgumentException("Unknown RegressionType: " + row.VisualProperties.RegressionType);
      }
    }
    private static string Sign(double value) {
      return value >= 0 ? "+" : "-";
    }
    private static string PolyFactor(int order) {
      if (order == 0) return "";
      if (order == 1) return " x";
      if (order == 2) return " x²";
      if (order == 3) return " x³";
      return " x^" + order;
    }
    #endregion
  }
}
