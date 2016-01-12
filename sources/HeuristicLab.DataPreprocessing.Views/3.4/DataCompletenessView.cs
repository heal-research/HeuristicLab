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
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.DataPreprocessing.Views {

  [View("Histogram View")]
  [Content(typeof(DataCompletenessChartContent), true)]
  public partial class DataCompletenessView : ItemView {

    //list of columns, bool indicates wether the cell is a missing value or not
    private List<List<bool>> matrix = new List<List<bool>>();
    //series colors
    private static Color colorNonMissingVal = Color.CornflowerBlue;
    private static Color colorMissingVal = Color.Orange;

    public new DataCompletenessChartContent Content {
      get { return (DataCompletenessChartContent)base.Content; }
      set { base.Content = value; }
    }


    public DataCompletenessView() {
      InitializeComponent();
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content != null) {
        InitData();
      }
    }

    private void InitData() {
      IDictionary<int, IList<int>> missingValueIndices = Content.SearchLogic.GetMissingValueIndices();
      for (int i = 0; i < Content.SearchLogic.Columns; i++) {
        //append column
        List<bool> column = new List<bool>();
        for (int j = 0; j < Content.SearchLogic.Rows; j++) {
          column.Add(missingValueIndices[i].Contains(j));
        }
        matrix.Add(column);
      }
      List<List<int>> yValuesPerColumn = ProcessMatrixForCharting(matrix, missingValueIndices);
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
      //prepare series
      int seriesCount = DetermineSeriesCount(yValuesPerColumn);
      for (int i = 0; i < seriesCount; i++) {
        chart.Series.Add(CreateSeriesName(i));
        Series series = chart.Series[CreateSeriesName(i)];
        series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedColumn;
        series.IsVisibleInLegend = false;
        series["PointWidth"] = "1.0";
        if (i % 2 == 0) {
          if (i == 0) //show legend for non missing values only once
            series.IsVisibleInLegend = true;
          series.Color = colorNonMissingVal;
        } else {
          if (i == 1) //show legend for missing values only once
            series.IsVisibleInLegend = true;
          series.Color = colorMissingVal;
        }
      }
      //fill series
      for (int i = 0; i < yValuesPerColumn.Count; i++) {
        List<int> column = yValuesPerColumn[i];
        for (int j = 0; j < seriesCount; j++) {
          if (column.Count - 1 < j) {
            chart.Series[CreateSeriesName(j)].Points.AddY(0);
          } else {
            chart.Series[CreateSeriesName(j)].Points.AddY(column[j]);
          }
        }
      }
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
      int highest = 0;
      foreach (List<int> values in yValuesPerColumn) {
        highest = Math.Max(values.Count, highest);
      }
      return highest;
    }

    private List<List<int>> ProcessMatrixForCharting(List<List<bool>> matrix, IDictionary<int, IList<int>> missingValueIndices) {
      List<List<int>> columnsYValues = new List<List<int>>();
      for (int i = 0; i < matrix.Count; i++) //column
      {
        List<int> yValues = new List<int>();
        List<bool> column = matrix[i];
        bool missingState = false;
        int valueCount = 0;
        for (int j = 0; j < column.Count; j++) {
          if (missingState == missingValueIndices[i].Contains(j)) {
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
        //yValues.Reverse();
        columnsYValues.Add(yValues);
      }
      return columnsYValues;
    }
    #endregion
  }
}
