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

using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HeuristicLab.Analysis.Views {
  [View("DataTable View")]
  [Content(typeof(DataTable), true)]
  public partial class DataTableView : NamedItemView, IConfigureableView {
    protected List<Series> invisibleSeries;
    protected Dictionary<IObservableList<double>, DataRow> valuesRowsTable;

    public new DataTable Content {
      get { return (DataTable)base.Content; }
      set { base.Content = value; }
    }

    public DataTableView() {
      InitializeComponent();
      valuesRowsTable = new Dictionary<IObservableList<double>, DataRow>();
      invisibleSeries = new List<Series>();
      chart.CustomizeAllChartAreas();
      chart.ChartAreas[0].CursorX.Interval = 1;
    }

    #region Event Handler Registration
    protected override void DeregisterContentEvents() {
      foreach (DataRow row in Content.Rows)
        DeregisterDataRowEvents(row);
      Content.VisualPropertiesChanged -= new EventHandler(Content_VisualPropertiesChanged);
      Content.Rows.ItemsAdded -= new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsAdded);
      Content.Rows.ItemsRemoved -= new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsRemoved);
      Content.Rows.ItemsReplaced -= new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsReplaced);
      Content.Rows.CollectionReset -= new CollectionItemsChangedEventHandler<DataRow>(Rows_CollectionReset);
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.VisualPropertiesChanged += new EventHandler(Content_VisualPropertiesChanged);
      Content.Rows.ItemsAdded += new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsAdded);
      Content.Rows.ItemsRemoved += new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsRemoved);
      Content.Rows.ItemsReplaced += new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsReplaced);
      Content.Rows.CollectionReset += new CollectionItemsChangedEventHandler<DataRow>(Rows_CollectionReset);
    }

    protected virtual void RegisterDataRowEvents(DataRow row) {
      row.NameChanged += new EventHandler(Row_NameChanged);
      row.VisualPropertiesChanged += new EventHandler(Row_VisualPropertiesChanged);
      valuesRowsTable.Add(row.Values, row);
      row.Values.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsAdded);
      row.Values.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsRemoved);
      row.Values.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsReplaced);
      row.Values.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsMoved);
      row.Values.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_CollectionReset);
    }
    protected virtual void DeregisterDataRowEvents(DataRow row) {
      row.Values.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsAdded);
      row.Values.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsRemoved);
      row.Values.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsReplaced);
      row.Values.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsMoved);
      row.Values.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_CollectionReset);
      valuesRowsTable.Remove(row.Values);
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
      chart.ChartAreas[0].AxisY2.Title = string.Empty;
      chart.Series.Clear();
      if (Content != null) {
        chart.Titles[0].Text = Content.Name;
        AddDataRows(Content.Rows);
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
        using (var dialog = new DataTableVisualPropertiesDialog(Content)) {
          dialog.ShowDialog(this);
        }
      } else MessageBox.Show("Nothing to configure.");
    }
    protected virtual void AddDataRows(IEnumerable<DataRow> rows) {
      foreach (var row in rows) {
        RegisterDataRowEvents(row);
        var series = new Series(row.Name);
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

    protected virtual void RemoveDataRows(IEnumerable<DataRow> rows) {
      foreach (var row in rows) {
        DeregisterDataRowEvents(row);
        Series series = chart.Series[row.Name];
        chart.Series.Remove(series);
        if (invisibleSeries.Contains(series))
          invisibleSeries.Remove(series);
      }
      RecalculateAxesScale(chart.ChartAreas[0]);
    }

    private void ConfigureSeries(Series series, DataRow row) {
      RemoveCustomPropertyIfExists(series, "PointWidth");
      series.BorderWidth = 1;
      series.BorderDashStyle = ChartDashStyle.Solid;
      series.BorderColor = Color.Empty;

      if (row.VisualProperties.Color != Color.Empty)
        series.Color = row.VisualProperties.Color;
      else series.Color = Color.Empty;
      series.IsVisibleInLegend = row.VisualProperties.IsVisibleInLegend;

      switch (row.VisualProperties.ChartType) {
        case DataRowVisualProperties.DataRowChartType.Line:
          series.ChartType = SeriesChartType.FastLine;
          series.BorderWidth = row.VisualProperties.LineWidth;
          series.BorderDashStyle = ConvertLineStyle(row.VisualProperties.LineStyle);
          break;
        case DataRowVisualProperties.DataRowChartType.Bars:
          // Bar is incompatible with anything but Bar and StackedBar*
          if (!chart.Series.Any(x => x.ChartType != SeriesChartType.Bar && x.ChartType != SeriesChartType.StackedBar && x.ChartType != SeriesChartType.StackedBar100))
            series.ChartType = SeriesChartType.Bar;
          else {
            series.ChartType = SeriesChartType.FastPoint; //default
            row.VisualProperties.ChartType = DataRowVisualProperties.DataRowChartType.Points;
          }
          break;
        case DataRowVisualProperties.DataRowChartType.Columns:
          series.ChartType = SeriesChartType.Column;
          break;
        case DataRowVisualProperties.DataRowChartType.Points:
          series.ChartType = SeriesChartType.FastPoint;
          break;
        case DataRowVisualProperties.DataRowChartType.Histogram:
          series.ChartType = SeriesChartType.Column;
          series.SetCustomProperty("PointWidth", "1");
          if (!series.Color.IsEmpty && series.Color.GetBrightness() < 0.25)
            series.BorderColor = Color.White;
          else series.BorderColor = Color.Black;
          break;
        case DataRowVisualProperties.DataRowChartType.StepLine:
          series.ChartType = SeriesChartType.StepLine;
          series.BorderWidth = row.VisualProperties.LineWidth;
          series.BorderDashStyle = ConvertLineStyle(row.VisualProperties.LineStyle);
          break;
        default:
          series.ChartType = SeriesChartType.FastPoint;
          break;
      }
      series.YAxisType = row.VisualProperties.SecondYAxis ? AxisType.Secondary : AxisType.Primary;
      series.XAxisType = row.VisualProperties.SecondXAxis ? AxisType.Secondary : AxisType.Primary;
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
        xAxisTitle + " = " + "#INDEX," + Environment.NewLine +
        yAxisTitle + " = " + "#VAL";
    }

    private void ConfigureChartArea(ChartArea area) {
      if (Content.VisualProperties.TitleFont != null) chart.Titles[0].Font = Content.VisualProperties.TitleFont;
      if (!Content.VisualProperties.TitleColor.IsEmpty) chart.Titles[0].ForeColor = Content.VisualProperties.TitleColor;
      chart.Titles[0].Text = Content.VisualProperties.Title;

      if (Content.VisualProperties.AxisTitleFont != null) area.AxisX.TitleFont = Content.VisualProperties.AxisTitleFont;
      if (!Content.VisualProperties.AxisTitleColor.IsEmpty) area.AxisX.TitleForeColor = Content.VisualProperties.AxisTitleColor;
      area.AxisX.Title = Content.VisualProperties.XAxisTitle;

      if (Content.VisualProperties.AxisTitleFont != null) area.AxisX2.TitleFont = Content.VisualProperties.AxisTitleFont;
      if (!Content.VisualProperties.AxisTitleColor.IsEmpty) area.AxisX2.TitleForeColor = Content.VisualProperties.AxisTitleColor;
      area.AxisX2.Title = Content.VisualProperties.SecondXAxisTitle;

      if (Content.VisualProperties.AxisTitleFont != null) area.AxisY.TitleFont = Content.VisualProperties.AxisTitleFont;
      if (!Content.VisualProperties.AxisTitleColor.IsEmpty) area.AxisY.TitleForeColor = Content.VisualProperties.AxisTitleColor;
      area.AxisY.Title = Content.VisualProperties.YAxisTitle;

      if (Content.VisualProperties.AxisTitleFont != null) area.AxisY2.TitleFont = Content.VisualProperties.AxisTitleFont;
      if (!Content.VisualProperties.AxisTitleColor.IsEmpty) area.AxisY2.TitleForeColor = Content.VisualProperties.AxisTitleColor;
      area.AxisY2.Title = Content.VisualProperties.SecondYAxisTitle;

      area.AxisX.IsLogarithmic = Content.VisualProperties.XAxisLogScale;
      area.AxisX2.IsLogarithmic = Content.VisualProperties.SecondXAxisLogScale;
      area.AxisY.IsLogarithmic = Content.VisualProperties.YAxisLogScale;
      area.AxisY2.IsLogarithmic = Content.VisualProperties.SecondYAxisLogScale;
    }

    private void RecalculateAxesScale(ChartArea area) {
      // Reset the axes bounds so that RecalculateAxesScale() will assign new bounds
      foreach (Axis a in area.Axes) {
        a.Minimum = double.NaN;
        a.Maximum = double.NaN;
      }
      area.RecalculateAxesScale();
      area.AxisX.IsMarginVisible = false;
      area.AxisX2.IsMarginVisible = false;

      if (!Content.VisualProperties.XAxisMinimumAuto && !double.IsNaN(Content.VisualProperties.XAxisMinimumFixedValue)) area.AxisX.Minimum = Content.VisualProperties.XAxisMinimumFixedValue;
      if (!Content.VisualProperties.XAxisMaximumAuto && !double.IsNaN(Content.VisualProperties.XAxisMaximumFixedValue)) area.AxisX.Maximum = Content.VisualProperties.XAxisMaximumFixedValue;
      if (!Content.VisualProperties.SecondXAxisMinimumAuto && !double.IsNaN(Content.VisualProperties.SecondXAxisMinimumFixedValue)) area.AxisX2.Minimum = Content.VisualProperties.SecondXAxisMinimumFixedValue;
      if (!Content.VisualProperties.SecondXAxisMaximumAuto && !double.IsNaN(Content.VisualProperties.SecondXAxisMaximumFixedValue)) area.AxisX2.Maximum = Content.VisualProperties.SecondXAxisMaximumFixedValue;
      if (!Content.VisualProperties.YAxisMinimumAuto && !double.IsNaN(Content.VisualProperties.YAxisMinimumFixedValue)) area.AxisY.Minimum = Content.VisualProperties.YAxisMinimumFixedValue;
      if (!Content.VisualProperties.YAxisMaximumAuto && !double.IsNaN(Content.VisualProperties.YAxisMaximumFixedValue)) area.AxisY.Maximum = Content.VisualProperties.YAxisMaximumFixedValue;
      if (!Content.VisualProperties.SecondYAxisMinimumAuto && !double.IsNaN(Content.VisualProperties.SecondYAxisMinimumFixedValue)) area.AxisY2.Minimum = Content.VisualProperties.SecondYAxisMinimumFixedValue;
      if (!Content.VisualProperties.SecondYAxisMaximumAuto && !double.IsNaN(Content.VisualProperties.SecondYAxisMaximumFixedValue)) area.AxisY2.Maximum = Content.VisualProperties.SecondYAxisMaximumFixedValue;
      if (area.AxisX.Minimum >= area.AxisX.Maximum) area.AxisX.Maximum = area.AxisX.Minimum + 1;
      if (area.AxisX2.Minimum >= area.AxisX2.Maximum) area.AxisX2.Maximum = area.AxisX2.Minimum + 1;
      if (area.AxisY.Minimum >= area.AxisY.Maximum) area.AxisY.Maximum = area.AxisY.Minimum + 1;
      if (area.AxisY2.Minimum >= area.AxisY2.Maximum) area.AxisY2.Maximum = area.AxisY2.Minimum + 1;
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

    #region Event Handlers
    #region Content Event Handlers
    protected override void Content_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_NameChanged), sender, e);
      else {
        chart.Titles[0].Text = Content.Name;
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
    private void Rows_ItemsAdded(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsAdded), sender, e);
      else {
        AddDataRows(e.Items);
      }
    }
    private void Rows_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsRemoved), sender, e);
      else {
        RemoveDataRows(e.Items);
      }
    }
    private void Rows_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<DataRow>(Rows_ItemsReplaced), sender, e);
      else {
        RemoveDataRows(e.OldItems);
        AddDataRows(e.Items);
      }
    }
    private void Rows_CollectionReset(object sender, CollectionItemsChangedEventArgs<DataRow> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<DataRow>(Rows_CollectionReset), sender, e);
      else {
        RemoveDataRows(e.OldItems);
        AddDataRows(e.Items);
      }
    }
    #endregion
    #region Row Event Handlers
    private void Row_VisualPropertiesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Row_VisualPropertiesChanged), sender, e);
      else {
        DataRow row = (DataRow)sender;
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
        DataRow row = (DataRow)sender;
        chart.Series[row.Name].Name = row.Name;
      }
    }
    #endregion
    #region Values Event Handlers
    private void Values_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsAdded), sender, e);
      else {
        DataRow row = null;
        valuesRowsTable.TryGetValue((IObservableList<double>)sender, out row);
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
    private void Values_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsRemoved), sender, e);
      else {
        DataRow row = null;
        valuesRowsTable.TryGetValue((IObservableList<double>)sender, out row);
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
    private void Values_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsReplaced), sender, e);
      else {
        DataRow row = null;
        valuesRowsTable.TryGetValue((IObservableList<double>)sender, out row);
        if (row != null) {
          Series rowSeries = chart.Series[row.Name];
          if (!invisibleSeries.Contains(rowSeries)) {
            if (row.VisualProperties.ChartType == DataRowVisualProperties.DataRowChartType.Histogram) {
              rowSeries.Points.Clear();
              FillSeriesWithRowValues(rowSeries, row);
            } else {
              foreach (IndexedItem<double> item in e.Items) {
                if (IsInvalidValue(item.Value))
                  rowSeries.Points[item.Index].IsEmpty = true;
                else {
                  rowSeries.Points[item.Index].YValues = new double[] { item.Value };
                  rowSeries.Points[item.Index].IsEmpty = false;
                }
              }
            }
            RecalculateAxesScale(chart.ChartAreas[0]);
            UpdateYCursorInterval();
          }
        }
      }
    }
    private void Values_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_ItemsMoved), sender, e);
      else {
        DataRow row = null;
        valuesRowsTable.TryGetValue((IObservableList<double>)sender, out row);
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

    private void Values_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<double>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<double>>(Values_CollectionReset), sender, e);
      else {
        DataRow row = null;
        valuesRowsTable.TryGetValue((IObservableList<double>)sender, out row);
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

    private void FillSeriesWithRowValues(Series series, DataRow row) {
      switch (row.VisualProperties.ChartType) {
        case DataRowVisualProperties.DataRowChartType.Histogram:
          CalculateHistogram(series, row);
          break;
        default: {
            bool yLogarithmic = series.YAxisType == AxisType.Primary
                                  ? Content.VisualProperties.YAxisLogScale
                                  : Content.VisualProperties.SecondYAxisLogScale;
            bool xLogarithmic = series.XAxisType == AxisType.Primary
                                  ? Content.VisualProperties.XAxisLogScale
                                  : Content.VisualProperties.SecondXAxisLogScale;
            for (int i = 0; i < row.Values.Count; i++) {
              var value = row.Values[i];
              var point = new DataPoint();
              point.XValue = row.VisualProperties.StartIndexZero && !xLogarithmic ? i : i + 1;
              if (IsInvalidValue(value) || (yLogarithmic && value <= 0))
                point.IsEmpty = true;
              else
                point.YValues = new double[] { value };
              series.Points.Add(point);
            }
          }
          break;
      }
    }

    protected virtual void CalculateHistogram(Series series, DataRow row) {
      series.Points.Clear();
      if (!row.Values.Any()) return;
      int bins = row.VisualProperties.Bins;

      double minValue = row.Values.Min();
      double maxValue = row.Values.Max();
      double intervalWidth = (maxValue - minValue) / bins;
      if (intervalWidth < 0) return;
      if (intervalWidth == 0) {
        series.Points.AddXY(minValue, row.Values.Count);
        return;
      }

      if (!row.VisualProperties.ExactBins) {
        intervalWidth = HumanRoundRange(intervalWidth);
        minValue = Math.Floor(minValue / intervalWidth) * intervalWidth;
        maxValue = Math.Ceiling(maxValue / intervalWidth) * intervalWidth;
      }

      double intervalCenter = intervalWidth / 2;

      double min = 0.0, max = 0.0;
      if (!Double.IsNaN(Content.VisualProperties.XAxisMinimumFixedValue) && !Content.VisualProperties.XAxisMinimumAuto)
        min = Content.VisualProperties.XAxisMinimumFixedValue;
      else min = minValue;
      if (!Double.IsNaN(Content.VisualProperties.XAxisMaximumFixedValue) && !Content.VisualProperties.XAxisMaximumAuto)
        max = Content.VisualProperties.XAxisMaximumFixedValue;
      else max = maxValue + intervalWidth;

      double axisInterval = intervalWidth / row.VisualProperties.ScaleFactor;

      var area = chart.ChartAreas[0];
      area.AxisX.Interval = axisInterval;

      series.SetCustomProperty("PointWidth", "1"); // 0.8 is the default value

      // get the range or intervals which define the grouping of the frequency values
      var doubleRange = DoubleRange(min, max, intervalWidth).Skip(1).ToList();

      // aggregate the row values by unique key and frequency value
      var valueFrequencies = (from v in row.Values
                              where !IsInvalidValue(v)
                              orderby v
                              group v by v into g
                              select new Tuple<double, double>(g.First(), g.Count())).ToList();

      // shift the chart to the left so the bars are placed on the intervals
      if (valueFrequencies.First().Item1 < doubleRange.First())
        series.Points.Add(new DataPoint(min - intervalWidth, 0));

      // add data points
      int j = 0;
      foreach (var d in doubleRange) {
        double sum = 0.0;
        // sum the frequency values that fall within the same interval
        while (j < valueFrequencies.Count && valueFrequencies[j].Item1 < d) {
          sum += valueFrequencies[j].Item2;
          ++j;
        }
        string xAxisTitle = string.IsNullOrEmpty(Content.VisualProperties.XAxisTitle)
                              ? "X"
                              : Content.VisualProperties.XAxisTitle;
        string yAxisTitle = string.IsNullOrEmpty(Content.VisualProperties.YAxisTitle)
                              ? "Y"
                              : Content.VisualProperties.YAxisTitle;
        series.Points.Add(new DataPoint(d - intervalCenter, sum) {
          ToolTip =
            xAxisTitle + ": [" + (d - intervalWidth) + "-" + d + ")" + Environment.NewLine +
            yAxisTitle + ": " + sum
        });
      }
    }

    #region Helpers
    public static IEnumerable<double> DoubleRange(double min, double max, double step) {
      double i;
      for (i = min; i <= max; i += step)
        yield return i;

      if (i != max + step)
        yield return i;
    }

    protected void RemoveCustomPropertyIfExists(Series series, string property) {
      if (series.IsCustomPropertySet(property)) series.DeleteCustomProperty(property);
    }

    private double HumanRoundRange(double range) {
      double base10 = Math.Pow(10.0, Math.Floor(Math.Log10(range)));
      double rounding = range / base10;
      if (rounding <= 1.5) rounding = 1;
      else if (rounding <= 2.25) rounding = 2;
      else if (rounding <= 3.75) rounding = 2.5;
      else if (rounding <= 7.5) rounding = 5;
      else rounding = 10;
      return rounding * base10;
    }

    private double HumanRoundMax(double max) {
      double base10;
      if (max > 0) base10 = Math.Pow(10.0, Math.Floor(Math.Log10(max)));
      else base10 = Math.Pow(10.0, Math.Ceiling(Math.Log10(-max)));
      double rounding = (max > 0) ? base10 : -base10;
      while (rounding < max) rounding += base10;
      return rounding;
    }

    private ChartDashStyle ConvertLineStyle(DataRowVisualProperties.DataRowLineStyle dataRowLineStyle) {
      switch (dataRowLineStyle) {
        case DataRowVisualProperties.DataRowLineStyle.Dash:
          return ChartDashStyle.Dash;
        case DataRowVisualProperties.DataRowLineStyle.DashDot:
          return ChartDashStyle.DashDot;
        case DataRowVisualProperties.DataRowLineStyle.DashDotDot:
          return ChartDashStyle.DashDotDot;
        case DataRowVisualProperties.DataRowLineStyle.Dot:
          return ChartDashStyle.Dot;
        case DataRowVisualProperties.DataRowLineStyle.NotSet:
          return ChartDashStyle.NotSet;
        case DataRowVisualProperties.DataRowLineStyle.Solid:
          return ChartDashStyle.Solid;
        default:
          return ChartDashStyle.NotSet;
      }
    }

    protected static bool IsInvalidValue(double x) {
      return double.IsNaN(x) || x < (double)decimal.MinValue || x > (double)decimal.MaxValue;
    }
    #endregion
  }
}
