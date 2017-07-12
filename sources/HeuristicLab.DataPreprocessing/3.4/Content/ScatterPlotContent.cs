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
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Visualization.ChartControlsExtensions;

namespace HeuristicLab.DataPreprocessing {

  public abstract class ScatterPlotContent : PreprocessingChartContent {
    public string GroupingVariable { get; set; }

    protected ScatterPlotContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
    }

    protected ScatterPlotContent(ScatterPlotContent content, Cloner cloner)
      : base(content, cloner) {
    }

    public static ScatterPlot CreateScatterPlot(IFilteredPreprocessingData preprocessingData, string variableNameX, string variableNameY, string variableNameGroup = "-", LegendOrder legendOrder = LegendOrder.Alphabetically) {
      ScatterPlot scatterPlot = new ScatterPlot();

      IList<double> xValues = preprocessingData.GetValues<double>(preprocessingData.GetColumnIndex(variableNameX));
      IList<double> yValues = preprocessingData.GetValues<double>(preprocessingData.GetColumnIndex(variableNameY));

      var points = xValues.Zip(yValues, (x, y) => new Point2D<double>(x, y)).ToList();
      var validPoints = points.Where(p => !double.IsNaN(p.X) && !double.IsNaN(p.Y) && !double.IsInfinity(p.X) && !double.IsInfinity(p.Y)).ToList();
      if (validPoints.Any()) {
        try {
          double axisMin, axisMax, axisInterval;
          ChartUtil.CalculateOptimalAxisInterval(validPoints.Min(p => p.X), validPoints.Max(p => p.X), out axisMin, out axisMax, out axisInterval);
          scatterPlot.VisualProperties.XAxisMinimumAuto = false;
          scatterPlot.VisualProperties.XAxisMaximumAuto = false;
          scatterPlot.VisualProperties.XAxisMinimumFixedValue = axisMin;
          scatterPlot.VisualProperties.XAxisMaximumFixedValue = axisMax;
        } catch (ArgumentOutOfRangeException) { } // error during CalculateOptimalAxisInterval 
        try {
          double axisMin, axisMax, axisInterval;
          ChartUtil.CalculateOptimalAxisInterval(validPoints.Min(p => p.Y), validPoints.Max(p => p.Y), out axisMin, out axisMax, out axisInterval);
          scatterPlot.VisualProperties.YAxisMinimumAuto = false;
          scatterPlot.VisualProperties.YAxisMaximumAuto = false;
          scatterPlot.VisualProperties.YAxisMinimumFixedValue = axisMin;
          scatterPlot.VisualProperties.YAxisMaximumFixedValue = axisMax;
        } catch (ArgumentOutOfRangeException) { } // error during CalculateOptimalAxisInterval 
      }


      //No Grouping
      if (string.IsNullOrEmpty(variableNameGroup) || variableNameGroup == "-") {
        ScatterPlotDataRow scdr = new ScatterPlotDataRow(variableNameX + " - " + variableNameY, "", validPoints);
        scdr.VisualProperties.IsVisibleInLegend = false;
        scatterPlot.Rows.Add(scdr);
        return scatterPlot;
      }

      //Grouping
      int groupVariableIndex = preprocessingData.GetColumnIndex(variableNameGroup);
      var groupingValues = Enumerable.Empty<string>();

      if (preprocessingData.VariableHasType<double>(groupVariableIndex)) {
        groupingValues = preprocessingData.GetValues<double>(groupVariableIndex).Select(x => x.ToString());
      } else if (preprocessingData.VariableHasType<string>(groupVariableIndex)) {
        groupingValues = preprocessingData.GetValues<string>(groupVariableIndex);
      } else if (preprocessingData.VariableHasType<DateTime>(groupVariableIndex)) {
        groupingValues = preprocessingData.GetValues<DateTime>(groupVariableIndex).Select(x => x.ToString());
      }
      var groups = groupingValues.Zip(validPoints, Tuple.Create).GroupBy(t => t.Item1, t => t.Item2);

      if (legendOrder == LegendOrder.Alphabetically)
        groups = groups.OrderBy(x => x.Key, new NaturalStringComparer());

      foreach (var group in groups) {
        var scdr = new ScatterPlotDataRow {
          Name = group.Key,
          VisualProperties = {
            IsVisibleInLegend = true,
            PointSize = 6
          }
        };
        scdr.Points.AddRange(group);
        scatterPlot.Rows.Add(scdr);
      }
      return scatterPlot;
    }
  }
}
