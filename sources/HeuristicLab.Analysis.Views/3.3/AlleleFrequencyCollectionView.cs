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
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("AlleleFrequencyCollection View")]
  [Content(typeof(AlleleFrequencyCollection), true)]
  public partial class AlleleFrequencyCollectionView : ItemView {
    private List<Series> invisibleSeries;

    public new AlleleFrequencyCollection Content {
      get { return (AlleleFrequencyCollection)base.Content; }
      set { base.Content = value; }
    }

    public AlleleFrequencyCollectionView() {
      InitializeComponent();
      invisibleSeries = new List<Series>();
      chart.CustomizeAllChartAreas();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        chart.Series.Clear();
        invisibleSeries.Clear();
      } else {
        if (chart.Series.Count == 0) CreateSeries();
        UpdateSeries();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      chart.Enabled = Content != null;
    }

    protected virtual void CreateSeries() {
      Series bestKnown = new Series("Alleles of Best Known Solution");
      bestKnown.ChartType = SeriesChartType.Column;
      bestKnown.XValueType = ChartValueType.String;
      bestKnown.YValueType = ChartValueType.Double;
      bestKnown.YAxisType = AxisType.Primary;
      chart.Series.Add(bestKnown);

      Series others = new Series("Other Alleles");
      others.ChartType = SeriesChartType.Column;
      others.XValueType = ChartValueType.String;
      others.YValueType = ChartValueType.Double;
      others.YAxisType = AxisType.Primary;
      chart.Series.Add(others);
      invisibleSeries.Add(others);

      Series qualities = new Series("Average Solution Qualities");
      qualities.ChartType = SeriesChartType.FastPoint;
      qualities.XValueType = ChartValueType.String;
      qualities.YValueType = ChartValueType.Double;
      qualities.YAxisType = AxisType.Secondary;
      chart.Series.Add(qualities);

      Series impacts = new Series("Average Impact");
      impacts.ChartType = SeriesChartType.FastPoint;
      impacts.XValueType = ChartValueType.String;
      impacts.YValueType = ChartValueType.Double;
      impacts.YAxisType = AxisType.Secondary;
      chart.Series.Add(impacts);
    }

    protected virtual void UpdateSeries() {
      int index = 1;
      Series bestKnown = chart.Series["Alleles of Best Known Solution"];
      Series others = chart.Series["Other Alleles"];
      Series qualities = chart.Series["Average Solution Qualities"];
      Series impacts = chart.Series["Average Impact"];
      bestKnown.Points.Clear();
      others.Points.Clear();
      qualities.Points.Clear();
      impacts.Points.Clear();

      if (!invisibleSeries.Contains(qualities) && !invisibleSeries.Contains(impacts))
        chart.ChartAreas["Default"].AxisY2.Title = "Average Solution Quality / Average Impact";
      else if (!invisibleSeries.Contains(qualities))
        chart.ChartAreas["Default"].AxisY2.Title = "Average Solution Quality";
      else if (!invisibleSeries.Contains(impacts))
        chart.ChartAreas["Default"].AxisY2.Title = "Average Impact";

      if (!invisibleSeries.Contains(bestKnown)) {
        foreach (AlleleFrequency af in Content.Where(x => x.ContainedInBestKnownSolution).OrderBy(x => x.Id)) {
          bestKnown.Points.Add(CreateDataPoint(index, af.Frequency, af));
          if (!invisibleSeries.Contains(qualities)) qualities.Points.Add(CreateDataPoint(index, af.AverageSolutionQuality, af));
          if (!invisibleSeries.Contains(impacts)) impacts.Points.Add(CreateDataPoint(index, af.AverageImpact, af));
          index++;
        }
      }
      if (!invisibleSeries.Contains(others)) {
        foreach (AlleleFrequency af in Content.Where(x => !x.ContainedInBestKnownSolution).OrderBy(x => x.Id)) {
          others.Points.Add(CreateDataPoint(index, af.Frequency, af));
          if (!invisibleSeries.Contains(qualities)) qualities.Points.Add(CreateDataPoint(index, af.AverageSolutionQuality, af));
          if (!invisibleSeries.Contains(impacts)) impacts.Points.Add(CreateDataPoint(index, af.AverageImpact, af));
          index++;
        }
      }
    }

    protected virtual DataPoint CreateDataPoint(int index, double value, AlleleFrequency af) {
      string nl = Environment.NewLine;
      DataPoint p = new DataPoint(index, value);
      p.ToolTip = string.Format("Id: {0}" + nl +
                                "Relative Frequency: {1}" + nl +
                                "Average Solution Quality: {2}" + nl +
                                "Average Impact: {3}" + nl +
                                "Contained in Best Known Solution: {4}" + nl +
                                "Contained in Best Solution: {5}",
                                af.Id, af.Frequency, af.AverageSolutionQuality, af.AverageImpact, af.ContainedInBestKnownSolution, af.ContainedInBestSolution);
      p.IsEmpty = value == 0;
      return p;
    }

    #region Chart Events
    protected virtual void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        ToggleSeriesVisible(result.Series);
      }
    }

    protected virtual void ToggleSeriesVisible(Series series) {
      if (!invisibleSeries.Contains(series))
        invisibleSeries.Add(series);
      else
        invisibleSeries.Remove(series);
      UpdateSeries();
    }

    protected virtual void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem)
        this.Cursor = Cursors.Hand;
      else
        this.Cursor = Cursors.Default;
    }

    protected virtual void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e) {
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
  }
}
