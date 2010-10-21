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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Analysis.Views {
  [View("AlleleFrequencyArray View")]
  [Content(typeof(AlleleFrequencyArray), true)]
  public partial class AlleleFrequencyArrayView : ItemView {
    private List<Series> invisibleSeries;

    public new AlleleFrequencyArray Content {
      get { return (AlleleFrequencyArray)base.Content; }
      set { base.Content = value; }
    }

    public AlleleFrequencyArrayView() {
      InitializeComponent();
      invisibleSeries = new List<Series>();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        chart.Series.Clear();
        chart.DataSource = null;
        invisibleSeries.Clear();
      } else {
        if (chart.Series.Count == 0) CreateSeries();
        chart.DataSource = Content.Select(x => new {
          Id = x.Id,
          BestKnownFrequency = x.ContainedInBestKnownSolution ? x.Frequency : 0,
          Frequency = !x.ContainedInBestKnownSolution ? x.Frequency : 0,
          Quality = x.AverageSolutionQuality,
          Impact = x.AverageImpact
        }).OrderBy(x => x.Impact).ToArray();
        UpdateSeries();
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      chart.Enabled = Content != null;
    }

    protected virtual void CreateSeries() {
      Series bestKnown = new Series("Alleles of Best Known Solution");
      bestKnown.ChartType = SeriesChartType.StackedColumn;
      bestKnown.XValueMember = "Id";
      bestKnown.XValueType = ChartValueType.String;
      bestKnown.YValueMembers = "BestKnownFrequency";
      bestKnown.YValueType = ChartValueType.Double;
      bestKnown.YAxisType = AxisType.Primary;
      bestKnown.ToolTip = "X = #LABEL, Y = #VAL";
      chart.Series.Add(bestKnown);

      Series others = new Series("Other Alleles");
      others.ChartType = SeriesChartType.StackedColumn;
      others.XValueMember = "Id";
      others.XValueType = ChartValueType.String;
      others.YValueMembers = "Frequency";
      others.YValueType = ChartValueType.Double;
      others.YAxisType = AxisType.Primary;
      others.ToolTip = "X = #LABEL, Y = #VAL";
      chart.Series.Add(others);

      Series qualities = new Series("Average Solution Qualities");
      qualities.ChartType = SeriesChartType.FastLine;
      qualities.XValueMember = "Id";
      qualities.XValueType = ChartValueType.String;
      qualities.YValueMembers = "Quality";
      qualities.YValueType = ChartValueType.Double;
      qualities.YAxisType = AxisType.Secondary;
      qualities.ToolTip = "X = #LABEL, Y = #VAL";
      chart.Series.Add(qualities);

      Series impacts = new Series("Average Impact");
      impacts.ChartType = SeriesChartType.FastLine;
      impacts.XValueMember = "Id";
      impacts.XValueType = ChartValueType.String;
      impacts.YValueMembers = "Impact";
      impacts.YValueType = ChartValueType.Double;
      impacts.YAxisType = AxisType.Secondary;
      impacts.ToolTip = "X = #LABEL, Y = #VAL";
      chart.Series.Add(impacts);
    }

    protected virtual void UpdateSeries() {
      chart.DataBind();

      if (invisibleSeries.Contains(chart.Series["Alleles of Best Known Solution"]))
        chart.Series["Alleles of Best Known Solution"].Points.Clear();
      chart.DataManipulator.Filter(CompareMethod.EqualTo, 0, chart.Series["Alleles of Best Known Solution"]);

      if (invisibleSeries.Contains(chart.Series["Other Alleles"]))
        chart.Series["Other Alleles"].Points.Clear();
      chart.DataManipulator.Filter(CompareMethod.EqualTo, 0, chart.Series["Other Alleles"]);

      if (invisibleSeries.Contains(chart.Series["Average Solution Qualities"]))
        chart.Series["Average Solution Qualities"].Points.Clear();
      chart.DataManipulator.Filter(CompareMethod.EqualTo, 0, chart.Series["Average Solution Qualities"]);

      if (invisibleSeries.Contains(chart.Series["Average Impact"]))
        chart.Series["Average Impact"].Points.Clear();
      chart.DataManipulator.Filter(CompareMethod.EqualTo, 0, chart.Series["Average Impact"]);
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
