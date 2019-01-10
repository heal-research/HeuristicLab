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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Collections;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("IndexedDataTable View")]
  [Content(typeof(IndexedDataTable<>), true)]
  public partial class IndexedDataTableView<T> : NamedItemView, IConfigureableView {
    protected List<Series> invisibleSeries;
    protected Dictionary<IObservableList<Tuple<T, double>>, IndexedDataRow<T>> valuesRowsTable;

    public new IndexedDataTable<T> Content {
      get { return (IndexedDataTable<T>)base.Content; }
      set { base.Content = value; }
    }

    public IndexedDataTableView() {
      InitializeComponent();
      valuesRowsTable = new Dictionary<IObservableList<Tuple<T, double>>, IndexedDataRow<T>>();
      invisibleSeries = new List<Series>();
      chart.CustomizeAllChartAreas();
      chart.ChartAreas[0].CursorX.Interval = 1;
      chart.SuppressExceptions = true;
      chart.ContextMenuStrip.Items.Add(configureToolStripMenuItem);
    }

    #region Event Handler Registration
    protected override void DeregisterContentEvents() {
      foreach (var row in Content.Rows)
        DeregisterDataRowEvents(row);
      Content.VisualPropertiesChanged -= new EventHandler(Content_VisualPropertiesChanged);
      Content.Rows.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_ItemsAdded);
      Content.Rows.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_ItemsRemoved);
      Content.Rows.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_ItemsReplaced);
      Content.Rows.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_CollectionReset);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.VisualPropertiesChanged += new EventHandler(Content_VisualPropertiesChanged);
      Content.Rows.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_ItemsAdded);
      Content.Rows.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_ItemsRemoved);
      Content.Rows.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_ItemsReplaced);
      Content.Rows.CollectionReset += new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_CollectionReset);
      foreach (var row in Content.Rows)
        RegisterDataRowEvents(row);
    }

    /// <summary>
    /// Automatically called for every existing data row and whenever a data row is added
    /// to the data table. Do not call this method directly.
    /// </summary>
    /// <param name="row">The DataRow that was added.</param>
    protected virtual void RegisterDataRowEvents(IndexedDataRow<T> row) {
      row.NameChanged += new EventHandler(Row_NameChanged);
      row.VisualPropertiesChanged += new EventHandler(Row_VisualPropertiesChanged);
      valuesRowsTable.Add(row.Values, row);
      row.Values.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsAdded);
      row.Values.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsRemoved);
      row.Values.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsReplaced);
      row.Values.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsMoved);
      row.Values.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_CollectionReset);
    }

    /// <summary>
    /// Automatically called for every data row that is removed from the DataTable. Do
    /// not directly call this method.
    /// </summary>
    /// <param name="row">The DataRow that was removed.</param>
    protected virtual void DeregisterDataRowEvents(IndexedDataRow<T> row) {
      row.Values.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsAdded);
      row.Values.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsRemoved);
      row.Values.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsReplaced);
      row.Values.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsMoved);
      row.Values.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_CollectionReset);
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
      chart.ChartAreas[0].AxisX.IsLogarithmic = false;
      chart.ChartAreas[0].AxisX2.IsLogarithmic = false;
      chart.ChartAreas[0].AxisY.IsLogarithmic = false;
      chart.ChartAreas[0].AxisY2.IsLogarithmic = false;
      chart.Series.Clear();
      if (Content != null) {
        chart.Titles[0].Text = Content.Name;
        foreach (var row in Content.Rows)
          AddDataRow(row);
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
        using (var dialog = new DataTableVisualPropertiesDialog<IndexedDataRow<T>>(Content)) {
          dialog.ShowDialog(this);
        }
      } else MessageBox.Show("Nothing to configure.");
    }

    /// <summary>
    /// Add the DataRow as a series to the chart.
    /// </summary>
    /// <param name="row">DataRow to add as series to the chart.</param>
    protected virtual void AddDataRow(IndexedDataRow<T> row) {
      if (row.Values.Count == 0) return;

      Series series = new Series(row.Name);
      if (row.VisualProperties.DisplayName.Trim() != String.Empty) series.LegendText = row.VisualProperties.DisplayName;
      else series.LegendText = row.Name;
      ConfigureSeries(series, row);
      FillSeriesWithRowValues(series, row);

      chart.Series.Add(series);
      ConfigureChartArea(chart.ChartAreas[0]);
      RecalculateAxesScale(chart.ChartAreas[0]);
      UpdateYCursorInterval();
    }

    private void ConfigureSeries(Series series, IndexedDataRow<T> row) {
      RemoveCustomPropertyIfExists(series, "PointWidth");
      series.BorderWidth = 1;
      series.BorderDashStyle = ChartDashStyle.Solid;
      series.BorderColor = Color.Empty;

      series.Color = row.VisualProperties.Color;
      series.IsVisibleInLegend = row.VisualProperties.IsVisibleInLegend;

      series.SmartLabelStyle.Enabled = true;
      series.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
      series.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.None;
      series.SmartLabelStyle.CalloutLineColor = series.Color;
      series.SmartLabelStyle.CalloutLineWidth = 2;
      series.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Underlined;
      series.SmartLabelStyle.IsOverlappedHidden = false;
      series.SmartLabelStyle.MaxMovingDistance = 200;

      switch (row.VisualProperties.ChartType) {
        case DataRowVisualProperties.DataRowChartType.Line:
          series.ChartType = SeriesChartType.FastLine;
          series.BorderWidth = row.VisualProperties.LineWidth;
          series.BorderDashStyle = ConvertLineStyle(row.VisualProperties.LineStyle);
          break;
        case DataRowVisualProperties.DataRowChartType.Bars:
          // Bar is incompatible with anything but Bar and StackedBar*
          if (!chart.Series.Any(x => x.ChartType != SeriesChartType.Bar && x.ChartType != SeriesChartType.StackedBar && x.ChartType != SeriesChartType.StackedBar100)) {
            series.ChartType = SeriesChartType.Bar;
            chart.ChartAreas[0].AxisX.Interval = 1;
          } else {
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
      series.ToolTip = series.LegendText + " X = #VALX, Y = #VALY";
    }

    private void ConfigureChartArea(ChartArea area) {
      if (Content.VisualProperties.TitleFont != null) chart.Titles[0].Font = Content.VisualProperties.TitleFont;
      if (!Content.VisualProperties.TitleColor.IsEmpty) chart.Titles[0].ForeColor = Content.VisualProperties.TitleColor;
      chart.Titles[0].Text = Content.VisualProperties.Title;
      chart.Titles[0].Visible = !string.IsNullOrEmpty(Content.VisualProperties.Title);

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

      if (typeof(T).Equals(typeof(DateTime))) {
        area.AxisX.IntervalType = DateTimeIntervalType.Hours;
        area.AxisX.LabelStyle.Format = "dd.MM.yyyy HH:mm";
        area.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
      }
    }

    private void RecalculateAxesScale(ChartArea area) {
      // Reset the axes bounds so that RecalculateAxesScale() will assign new bounds
      foreach (Axis a in area.Axes) {
        a.Minimum = double.NaN;
        a.Maximum = double.NaN;
      }

      if (chart.Series.Any(x => x.Enabled && x.XAxisType == AxisType.Primary && (x.Points.Count == 0 || x.Points.Any(y => y.XValue <= 0))))
        area.AxisX.IsLogarithmic = false;
      else area.AxisX.IsLogarithmic = Content.VisualProperties.XAxisLogScale;
      if (chart.Series.Any(x => x.Enabled && x.XAxisType == AxisType.Secondary && (x.Points.Count == 0 || x.Points.Any(y => y.XValue <= 0))))
        area.AxisX2.IsLogarithmic = false;
      else area.AxisX2.IsLogarithmic = Content.VisualProperties.SecondXAxisLogScale;

      if (chart.Series.Any(x => x.Enabled && x.YAxisType == AxisType.Primary && (x.Points.Count == 0 || x.Points.Any(y => y.YValues.Min() <= 0))))
        area.AxisY.IsLogarithmic = false;
      else area.AxisY.IsLogarithmic = Content.VisualProperties.YAxisLogScale;
      if (chart.Series.Any(x => x.Enabled && x.YAxisType == AxisType.Secondary && (x.Points.Count == 0 || x.Points.Any(y => y.YValues.Min() <= 0))))
        area.AxisY2.IsLogarithmic = false;
      else area.AxisY2.IsLogarithmic = Content.VisualProperties.SecondYAxisLogScale;

      area.RecalculateAxesScale();
      area.AxisX.IsMarginVisible = false;
      area.AxisX2.IsMarginVisible = false;

      area.AxisX.IsStartedFromZero = Content.Rows.Where(x => !x.VisualProperties.SecondXAxis).Any(x => x.VisualProperties.StartIndexZero);
      area.AxisX2.IsStartedFromZero = Content.Rows.Where(x => x.VisualProperties.SecondXAxis).Any(x => x.VisualProperties.StartIndexZero);

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

    /// <summary>
    /// Set the Y Cursor interval to visible points of enabled series.
    /// </summary>
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


    /// <summary>
    /// Remove the corresponding series for a certain DataRow.
    /// </summary>
    /// <param name="row">DataRow which series should be removed.</param>
    protected virtual void RemoveDataRow(IndexedDataRow<T> row) {
      if (chart.Series.All(x => x.Name != row.Name)) return;
      Series series = chart.Series[row.Name];
      chart.Series.Remove(series);
      if (invisibleSeries.Contains(series))
        invisibleSeries.Remove(series);
      RecalculateAxesScale(chart.ChartAreas[0]);
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
    private void Rows_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedDataRow<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_ItemsAdded), sender, e);
      else {
        foreach (var row in e.Items) {
          AddDataRow(row);
          RegisterDataRowEvents(row);
        }
      }
    }
    private void Rows_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedDataRow<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_ItemsRemoved), sender, e);
      else {
        foreach (var row in e.Items) {
          DeregisterDataRowEvents(row);
          RemoveDataRow(row);
        }
      }
    }
    private void Rows_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedDataRow<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_ItemsReplaced), sender, e);
      else {
        foreach (var row in e.OldItems) {
          DeregisterDataRowEvents(row);
          RemoveDataRow(row);
        }
        foreach (var row in e.Items) {
          AddDataRow(row);
          RegisterDataRowEvents(row);
        }
      }
    }
    private void Rows_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedDataRow<T>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedDataRow<T>>(Rows_CollectionReset), sender, e);
      else {
        foreach (var row in e.OldItems) {
          DeregisterDataRowEvents(row);
          RemoveDataRow(row);
        }
        foreach (var row in e.Items) {
          AddDataRow(row);
          RegisterDataRowEvents(row);
        }
      }
    }
    #endregion
    #region Row Event Handlers
    private void Row_VisualPropertiesChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Row_VisualPropertiesChanged), sender, e);
      else {
        var row = (IndexedDataRow<T>)sender;
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
        var row = (IndexedDataRow<T>)sender;
        chart.Series[row.Name].Name = row.Name;
      }
    }
    #endregion
    #region Values Event Handlers
    private void Values_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<Tuple<T, double>>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsAdded), sender, e);
      else {
        IndexedDataRow<T> row = null;
        valuesRowsTable.TryGetValue((IObservableList<Tuple<T, double>>)sender, out row);
        if (row != null) {
          Series rowSeries = chart.Series[row.Name];
          if (!invisibleSeries.Contains(rowSeries)) {
            rowSeries.Points.Clear();
            FillSeriesWithRowValues(rowSeries, row);
            RecalculateAxesScale(chart.ChartAreas[0]);
            UpdateYCursorInterval();
          }
        } else AddDataRow((IndexedDataRow<T>)sender);
      }
    }
    private void Values_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Tuple<T, double>>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsRemoved), sender, e);
      else {
        IndexedDataRow<T> row = null;
        valuesRowsTable.TryGetValue((IObservableList<Tuple<T, double>>)sender, out row);
        if (row != null) {
          if (row.Values.Count == 0) {
            RemoveDataRow(row);
          } else {
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
    }
    private void Values_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<Tuple<T, double>>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsReplaced), sender, e);
      else {
        IndexedDataRow<T> row = null;
        valuesRowsTable.TryGetValue((IObservableList<Tuple<T, double>>)sender, out row);
        if (row != null) {
          if (row.Values.Count == 0) {
            RemoveDataRow(row);
          } else {
            Series rowSeries = chart.Series[row.Name];
            if (!invisibleSeries.Contains(rowSeries)) {
              if (row.VisualProperties.ChartType == DataRowVisualProperties.DataRowChartType.Histogram) {
                rowSeries.Points.Clear();
                FillSeriesWithRowValues(rowSeries, row);
              } else {
                foreach (IndexedItem<Tuple<T, double>> item in e.Items) {
                  rowSeries.Points[item.Index].SetValueXY(item.Value.Item1, item.Value.Item2);
                }
              }
              RecalculateAxesScale(chart.ChartAreas[0]);
              UpdateYCursorInterval();
            }
          }
        }
      }
    }
    private void Values_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<Tuple<T, double>>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_ItemsMoved), sender, e);
      else {
        IndexedDataRow<T> row = null;
        valuesRowsTable.TryGetValue((IObservableList<Tuple<T, double>>)sender, out row);
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

    private void Values_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<Tuple<T, double>>> e) {
      if (InvokeRequired)
        Invoke(new CollectionItemsChangedEventHandler<IndexedItem<Tuple<T, double>>>(Values_CollectionReset), sender, e);
      else {
        IndexedDataRow<T> row = null;
        valuesRowsTable.TryGetValue((IObservableList<Tuple<T, double>>)sender, out row);
        if (row != null) {
          if (row.Values.Count == 0) {
            RemoveDataRow(row);
          } else {
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
    }
    #endregion
    private void configureToolStripMenuItem_Click(object sender, EventArgs e) {
      ShowConfiguration();
    }
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

    private void FillSeriesWithRowValues(Series series, IndexedDataRow<T> row) {
      for (int i = 0; i < row.Values.Count; i++) {
        var value = row.Values[i];
        if (IsInvalidValue(value.Item2)) continue;
        var point = new DataPoint();
        point.SetValueXY(value.Item1, value.Item2);
        if (i == row.Values.Count - 1) {
          point.Label = series.Name;
          point.MarkerStyle = MarkerStyle.Cross;
          point.MarkerSize = 15;
          point.MarkerBorderWidth = 1;
        }
        series.Points.Add(point);
      }
    }

    #region Helpers
    protected void RemoveCustomPropertyIfExists(Series series, string property) {
      if (series.IsCustomPropertySet(property)) series.DeleteCustomProperty(property);
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

    /// <summary>
    /// Determines whether a double value can be displayed (converted to Decimal and not an NaN).
    /// </summary>
    /// <param name="x">The number to check.</param>
    /// <returns><code>true</code> if the value can be safely shwon in the chart,
    /// <code>false</code> otherwise.</returns>
    protected static bool IsInvalidValue(double x) {
      return double.IsNaN(x) || x < (double)decimal.MinValue || x > (double)decimal.MaxValue;
    }
    #endregion
  }
}
