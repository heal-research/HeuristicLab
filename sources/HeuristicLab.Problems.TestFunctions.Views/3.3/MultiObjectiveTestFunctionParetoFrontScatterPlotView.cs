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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Problems.TestFunctions.MultiObjective {
  [View("Scatter Plot")]
  [Content(typeof(ScatterPlotContent))]
  public partial class MultiObjectiveTestFunctionParetoFrontScatterPlotView : ItemView {
    private const string QUALITIES = "Qualities";
    private const string PARETO_FRONT = "Best Known Pareto Front";
    private Series qualitySeries;
    private Series paretoSeries;
    private int xDim = 0;
    private int yDim = 1;
    int objectives = -1;

    public new ScatterPlotContent Content {
      get { return (ScatterPlotContent)base.Content; }
      set { base.Content = value; }
    }

    public MultiObjectiveTestFunctionParetoFrontScatterPlotView()
      : base() {
      InitializeComponent();

      BuildEmptySeries();

      //start with qualities toggled ON
      qualitySeries.Points.AddXY(0, 0);

      this.chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
      this.chart.AxisViewChanged += new EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(chart_AxisViewChanged);
      this.chart.GetToolTipText += new System.EventHandler<ToolTipEventArgs>(this.Chart_GetToolTipText);

      //configure axis 
      this.chart.CustomizeAllChartAreas();
      this.chart.ChartAreas[0].AxisX.Title = "Objective " + xDim;
      this.chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].CursorX.Interval = 1;
      this.chart.ChartAreas[0].CursorY.Interval = 1;

      this.chart.ChartAreas[0].AxisY.Title = "Objective " + yDim;
      this.chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      this.chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
      this.chart.ChartAreas[0].AxisY.IsStartedFromZero = true;
    }


    private void Chart_GetToolTipText(object sender, ToolTipEventArgs e) {
      if (e.HitTestResult.ChartElementType == ChartElementType.LegendItem) {
        if (e.HitTestResult.Series == paretoSeries && (Content.ParetoFront == null || Content.ParetoFront.Length == 0)) {
          e.Text = "No optimal pareto front is available for this problem with this number of objectives";
        }
        if (e.HitTestResult.Series == paretoSeries && (xDim >= Content.Objectives || yDim >= Content.Objectives)) {
          e.Text = "The optimal pareto front can only be displayed in  Objective Space";
        }
      }

      // Check selected chart element and set tooltip text
      if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint) {
        int i = e.HitTestResult.PointIndex;
        StringBuilder toolTippText = new StringBuilder();
        DataPoint qp = e.HitTestResult.Series.Points[i];
        toolTippText.Append("Objective " + xDim + " = " + qp.XValue + "\n");
        toolTippText.Append("Objective " + yDim + " = " + qp.YValues[0]);

        Series s = e.HitTestResult.Series;
        if (s.Equals(this.chart.Series[QUALITIES])) {
          double[] dp = Content.Solutions[i];
          toolTippText.Append("\nSolution: {");
          for (int j = 0; j < dp.Length; j++) {
            toolTippText.Append(dp[j]);
            toolTippText.Append(";");
          }
          toolTippText.Remove(toolTippText.Length - 1, 1);
          toolTippText.Append("}");
          e.Text = toolTippText.ToString();
        }


      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) return;
      if (objectives != Content.Objectives) {
        AddMenuItems();
        objectives = Content.Objectives;
      }
      if (Content.ParetoFront == null && chart.Series.Contains(paretoSeries)) {
        Series s = this.chart.Series[PARETO_FRONT];
        paretoSeries = null;
        this.chart.Series.Remove(s);

      } else if (Content.ParetoFront != null && !chart.Series.Contains(paretoSeries)) {
        this.chart.Series.Add(PARETO_FRONT);
        paretoSeries = this.chart.Series[PARETO_FRONT];
        this.chart.Series[PARETO_FRONT].LegendText = PARETO_FRONT;
        this.chart.Series[PARETO_FRONT].ChartType = SeriesChartType.FastPoint;
      }
      UpdateChart();
    }

    private void UpdateChart() {
      if (InvokeRequired) Invoke((Action)UpdateChart);
      else {
        if (Content != null) {
          this.UpdateSeries();
          if (!this.chart.Series.Any(s => s.Points.Count > 0))
            this.ClearChart();
        }
      }
    }

    private void UpdateCursorInterval() {
      var estimatedValues = this.chart.Series[QUALITIES].Points.Select(x => x.XValue).DefaultIfEmpty(1.0);
      var targetValues = this.chart.Series[QUALITIES].Points.Select(x => x.YValues[0]).DefaultIfEmpty(1.0);
      double estimatedValuesRange = estimatedValues.Max() - estimatedValues.Min();
      double targetValuesRange = targetValues.Max() - targetValues.Min();
      double interestingValuesRange = Math.Min(Math.Max(targetValuesRange, 1.0), Math.Max(estimatedValuesRange, 1.0));
      double digits = (int)Math.Log10(interestingValuesRange) - 3;
      double zoomInterval = Math.Max(Math.Pow(10, digits), 10E-5);
      this.chart.ChartAreas[0].CursorX.Interval = zoomInterval;
      this.chart.ChartAreas[0].CursorY.Interval = zoomInterval;

      this.chart.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = zoomInterval;
      this.chart.ChartAreas[0].AxisY.ScaleView.SmallScrollSize = zoomInterval;

      this.chart.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSizeType = DateTimeIntervalType.Number;
      this.chart.ChartAreas[0].AxisX.ScaleView.SmallScrollMinSize = zoomInterval;
      this.chart.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSizeType = DateTimeIntervalType.Number;
      this.chart.ChartAreas[0].AxisY.ScaleView.SmallScrollMinSize = zoomInterval;

      if (digits < 0) {
        this.chart.ChartAreas[0].AxisX.LabelStyle.Format = "F" + (int)Math.Abs(digits);
        this.chart.ChartAreas[0].AxisY.LabelStyle.Format = "F" + (int)Math.Abs(digits);
      } else {
        this.chart.ChartAreas[0].AxisX.LabelStyle.Format = "F0";
        this.chart.ChartAreas[0].AxisY.LabelStyle.Format = "F0";
      }
    }

    private void UpdateSeries() {
      if (InvokeRequired) Invoke((Action)UpdateSeries);
      else {

        if (this.chart.Series.Contains(qualitySeries) && qualitySeries.Points.Count() != 0) {
          FillSeries(Content.Qualities, Content.Solutions, qualitySeries);
        }
        if (this.chart.Series.Contains(paretoSeries) && paretoSeries.Points.Count() != 0) {
          FillSeries(Content.ParetoFront, null, paretoSeries);
        }


        double minX = Double.MaxValue;
        double maxX = Double.MinValue;
        double minY = Double.MaxValue;
        double maxY = Double.MinValue;
        foreach (Series s in this.chart.Series) {
          if (s.Points.Count == 0) continue;
          minX = Math.Min(minX, s.Points.Select(p => p.XValue).Min());
          maxX = Math.Max(maxX, s.Points.Select(p => p.XValue).Max());
          minY = Math.Min(minY, s.Points.Select(p => p.YValues.Min()).Min());
          maxY = Math.Max(maxY, s.Points.Select(p => p.YValues.Max()).Max());
        }

        maxX = maxX + 0.2 * Math.Abs(maxX);
        minX = minX - 0.2 * Math.Abs(minX);
        maxY = maxY + 0.2 * Math.Abs(maxY);
        minY = minY - 0.2 * Math.Abs(minY);

        double interestingValuesRangeX = maxX - minX;
        double interestingValuesRangeY = maxY - minY;

        int digitsX = Math.Max(0, 3 - (int)Math.Log10(interestingValuesRangeX));
        int digitsY = Math.Max(0, 3 - (int)Math.Log10(interestingValuesRangeY));


        maxX = Math.Round(maxX, digitsX);
        minX = Math.Round(minX, digitsX);
        maxY = Math.Round(maxY, digitsY);
        minY = Math.Round(minY, digitsY);
        if (minX > maxX) {
          minX = 0;
          maxX = 1;
        }
        if (minY > maxY) {
          minY = 0;
          maxY = 1;
        }


        this.chart.ChartAreas[0].AxisX.Maximum = maxX;
        this.chart.ChartAreas[0].AxisX.Minimum = minX;
        this.chart.ChartAreas[0].AxisY.Maximum = maxY;
        this.chart.ChartAreas[0].AxisY.Minimum = minY;
        UpdateCursorInterval();
      }
    }

    private void ClearChart() {
      if (chart.Series.Contains(qualitySeries)) chart.Series.Remove(qualitySeries);
      if (chart.Series.Contains(paretoSeries)) chart.Series.Remove(paretoSeries);
      BuildEmptySeries();
    }

    private void ToggleSeriesData(Series series) {
      if (series.Points.Count > 0) {  //checks if series is shown
        series.Points.Clear();
      } else if (Content != null) {
        switch (series.Name) {
          case PARETO_FRONT:
            FillSeries(Content.ParetoFront, null, this.chart.Series[PARETO_FRONT]);
            break;
          case QUALITIES:
            FillSeries(Content.Qualities, Content.Solutions, this.chart.Series[QUALITIES]);
            break;
        }
      }
    }

    private void chart_MouseDown(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem) {
        this.ToggleSeriesData(result.Series);
      }

    }

    private void chart_MouseMove(object sender, MouseEventArgs e) {
      HitTestResult result = chart.HitTest(e.X, e.Y);
      if (result.ChartElementType == ChartElementType.LegendItem)
        this.Cursor = Cursors.Hand;
      else
        this.Cursor = Cursors.Default;
    }

    private void chart_AxisViewChanged(object sender, System.Windows.Forms.DataVisualization.Charting.ViewEventArgs e) {
      this.chart.ChartAreas[0].AxisX.ScaleView.Size = e.NewSize;
      this.chart.ChartAreas[0].AxisY.ScaleView.Size = e.NewSize;
    }

    private void chart_CustomizeLegend(object sender, CustomizeLegendEventArgs e) {
      if (this.chart.Series.Contains(qualitySeries)) e.LegendItems[0].Cells[1].ForeColor = this.chart.Series[QUALITIES].Points.Count == 0 ? Color.Gray : Color.Black;
      if (this.chart.Series.Contains(paretoSeries)) e.LegendItems[1].Cells[1].ForeColor = this.chart.Series[PARETO_FRONT].Points.Count == 0 ? Color.Gray : Color.Black;
    }

    private void AddMenuItems() {
      chooseDimensionToolStripMenuItem.DropDownItems.Clear();
      chooseYDimensionToolStripMenuItem.DropDownItems.Clear();
      if (Content == null) { return; }
      int i = 0;
      for (; i < Content.Objectives; i++) {
        //add Menu Points
        ToolStripMenuItem xItem = MakeMenuItem("X", "Objective " + i, i);
        ToolStripMenuItem yItem = MakeMenuItem("Y", "Objective " + i, i);
        xItem.Click += new System.EventHandler(this.XMenu_Click);
        yItem.Click += new System.EventHandler(this.YMenu_Click);
        chooseDimensionToolStripMenuItem.DropDownItems.Add(xItem);
        chooseYDimensionToolStripMenuItem.DropDownItems.Add(yItem);
      }

      for (; i < Content.Solutions[0].Length + Content.Objectives; i++) {
        ToolStripMenuItem xItem = MakeMenuItem("X", "ProblemDimension " + (i - Content.Objectives), i);
        ToolStripMenuItem yItem = MakeMenuItem("Y", "ProblemDimension " + (i - Content.Objectives), i); ;
        xItem.Click += new System.EventHandler(this.XMenu_Click);
        yItem.Click += new System.EventHandler(this.YMenu_Click);
        chooseDimensionToolStripMenuItem.DropDownItems.Add(xItem);
        chooseYDimensionToolStripMenuItem.DropDownItems.Add(yItem);
      }
    }

    private ToolStripMenuItem MakeMenuItem(String axis, String label, int i) {
      ToolStripMenuItem xItem = new ToolStripMenuItem();
      xItem.Name = "obj" + i;
      xItem.Size = new System.Drawing.Size(269, 38);
      xItem.Text = label;
      return xItem;
    }

    private void YMenu_Click(object sender, EventArgs e) {
      ToolStripMenuItem item = (ToolStripMenuItem)sender;
      yDim = Int32.Parse(item.Name.Remove(0, 3));
      String label = item.Text;
      this.chooseYDimensionToolStripMenuItem.Text = label;
      this.chart.ChartAreas[0].AxisY.Title = label;
      UpdateChart();
    }

    private void XMenu_Click(object sender, EventArgs e) {
      ToolStripMenuItem item = (ToolStripMenuItem)sender;
      xDim = Int32.Parse(item.Name.Remove(0, 3));
      String label = item.Text;
      this.chooseDimensionToolStripMenuItem.Text = label;
      this.chart.ChartAreas[0].AxisX.Title = label;
      UpdateChart();
    }

    private void FillSeries(double[][] qualities, double[][] solutions, Series series) {
      series.Points.Clear();
      if (qualities == null || qualities.Length == 0) return;
      int jx = xDim - qualities[0].Length;
      int jy = yDim - qualities[0].Length;
      if ((jx >= 0 || jy >= 0) && solutions == null) {
        return;
      }
      for (int i = 0; i < qualities.Length; i++) {   //Assumtion: Columnwise
        double[] d = qualities[i];
        double[] q = null;
        if (jx >= 0 || jy >= 0) { q = solutions[i]; }
        series.Points.AddXY(jx < 0 ? d[xDim] : q[jx], jy < 0 ? d[yDim] : q[jy]);
      }
    }

    private void BuildEmptySeries() {

      this.chart.Series.Add(QUALITIES);
      qualitySeries = this.chart.Series[QUALITIES];

      this.chart.Series[QUALITIES].LegendText = QUALITIES;
      this.chart.Series[QUALITIES].ChartType = SeriesChartType.FastPoint;

      this.chart.Series.Add(PARETO_FRONT);
      paretoSeries = this.chart.Series[PARETO_FRONT];
      paretoSeries.Color = Color.FromArgb(100, Color.Orange);
      this.chart.Series[PARETO_FRONT].LegendText = PARETO_FRONT;
      this.chart.Series[PARETO_FRONT].ChartType = SeriesChartType.FastPoint;
    }
  }
}

