#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.DataPreprocessing.Views {

  [View("Histogram View")]
  [Content(typeof(DataCompletenessChartContent), true)]
  public partial class DataCompletenessView : ItemView {
    //series colors
    private static readonly Color colorNonMissingValue = Color.CornflowerBlue;
    private static readonly Color colorMissingValue = Color.Orange;

    public new DataCompletenessChartContent Content {
      get { return (DataCompletenessChartContent)base.Content; }
      set { base.Content = value; }
    }

    public DataCompletenessView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) return;
      InitData();
    }

    private void InitData() {
      IDictionary<int, IList<int>> missingValueIndices = Content.SearchLogic.GetMissingValueIndices();

      bool[,] valueMissing = new bool[Content.SearchLogic.Rows, Content.SearchLogic.Columns];
      foreach (var columnMissingValues in missingValueIndices) {
        var column = columnMissingValues.Key;
        foreach (var missingValueIndex in columnMissingValues.Value)
          valueMissing[missingValueIndex, column] = true;
      }

      var yValuesPerColumn = ProcessMatrixForCharting(valueMissing);
      PrepareChart();
      CreateSeries(yValuesPerColumn);
    }

    private void PrepareChart() {
      chart.Titles.Add("DataCompletenessChart");
      chart.EnableDoubleClickResetsZoom = true;
      chart.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
      chart.ChartAreas[0].AxisY.MajorGrid.LineWidth = 0;
      chart.ChartAreas[0].AxisX.IsMarginVisible = false;
      chart.ChartAreas[0].AxisY.IsMarginVisible = false;
      chart.ChartAreas[0].CursorX.IsUserEnabled = true;
      chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      chart.ChartAreas[0].CursorY.IsUserEnabled = true;
      chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      //custom x axis label
      double from = 0.5;
      foreach (String columnName in Content.SearchLogic.VariableNames) {
        double to = from + 1;
        chart.ChartAreas[0].AxisX.CustomLabels.Add(from, to, columnName);
        from = to;
      }
      //custom y axis label
      chart.ChartAreas[0].AxisY.IsReversed = true;
    }

    private void CreateSeries(List<List<int>> yValuesPerColumn) {
      chart.Series.SuspendUpdates();
      //prepare series
      int seriesCount = DetermineSeriesCount(yValuesPerColumn);
      for (int i = 0; i < seriesCount; i++) {
        Series series = new Series(CreateSeriesName(i));
        series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
        series.IsVisibleInLegend = false;
        series["PointWidth"] = "1.0";
        series.IsVisibleInLegend = i < 2; //only first two series are visible, non-missing and missing values
        series.Color = i % 2 == 0 ? colorNonMissingValue : colorMissingValue;

        var values = yValuesPerColumn.Select(y => i < y.Count ? y[i] : 0).ToArray();
        series.Points.DataBindY(values);
        chart.Series.Add(series);
      }
      chart.Series.ResumeUpdates();
    }

    private String CreateSeriesName(int index) {
      if (index == 0)
        return "non-missing value";
      else if (index == 1)
        return "missing value";
      return "series" + index;
    }

    #region data_preparation_for_chartseries
    private int DetermineSeriesCount(List<List<int>> yValuesPerColumn) {
      return yValuesPerColumn.Max(values => values.Count);
    }

    private List<List<int>> ProcessMatrixForCharting(bool[,] matrix) {
      var columnsYValues = new List<List<int>>();
      for (int column = 0; column < matrix.GetLength(1); column++) {
        var yValues = new List<int>();
        bool missingState = false;
        int valueCount = 0;

        for (int row = 0; row < matrix.GetLength(0); row++) {
          if (missingState == matrix[row, column]) {
            valueCount++;
          } else {
            yValues.Add(valueCount);
            valueCount = 1;
            missingState = !missingState;
          }
        }
        yValues.Add(valueCount);
        if (missingState) //handle last missing
        {
          yValues.Add(0);
        }
        columnsYValues.Add(yValues);
      }
      return columnsYValues;
    }
    #endregion
  }
}
