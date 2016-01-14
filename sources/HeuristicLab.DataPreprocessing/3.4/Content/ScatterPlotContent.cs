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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.DataPreprocessing {

  [Item("ScatterPlot", "Represents a scatter plot.")]
  public class ScatterPlotContent : PreprocessingChartContent {

    public string SelectedXVariable { get; set; }
    public string SelectedYVariable { get; set; }
    public string SelectedColorVariable { get; set; }

    public ScatterPlotContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
    }

    public ScatterPlotContent(ScatterPlotContent content, Cloner cloner)
      : base(content, cloner) {
      this.SelectedXVariable = content.SelectedXVariable;
      this.SelectedYVariable = content.SelectedYVariable;
      this.SelectedColorVariable = content.SelectedColorVariable;
    }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Performance; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterPlotContent(this, cloner);
    }

    public ScatterPlot CreateScatterPlot(string variableNameX, string variableNameY, string variableNameColor = "-") {
      ScatterPlot scatterPlot = new ScatterPlot();

      IList<double> xValues = PreprocessingData.GetValues<double>(PreprocessingData.GetColumnIndex(variableNameX));
      IList<double> yValues = PreprocessingData.GetValues<double>(PreprocessingData.GetColumnIndex(variableNameY));
      if (variableNameColor == null || variableNameColor == "-") {
        List<Point2D<double>> points = new List<Point2D<double>>();

        for (int i = 0; i < xValues.Count; i++) {
          Point2D<double> point = new Point2D<double>(xValues[i], yValues[i]);
          points.Add(point);
        }

        ScatterPlotDataRow scdr = new ScatterPlotDataRow(variableNameX + " - " + variableNameY, "", points);
        scatterPlot.Rows.Add(scdr);

      } else {
        var colorValues = PreprocessingData.GetValues<double>(PreprocessingData.GetColumnIndex(variableNameColor));
        var data = xValues.Zip(yValues, (x, y) => new { x, y }).Zip(colorValues, (v, c) => new { v.x, v.y, c }).ToList();
        var gradients = ColorGradient.Colors;
        int curGradient = 0;
        int numColors = colorValues.Distinct().Count();
        foreach (var colorValue in colorValues.Distinct()) {
          var values = data.Where(x => x.c == colorValue);
          var row = new ScatterPlotDataRow(
            variableNameX + " - " + variableNameY + " (" + colorValue + ")",
            "",
            values.Select(v => new Point2D<double>(v.x, v.y)),
            new ScatterPlotDataRowVisualProperties() { Color = gradients[curGradient] });
          curGradient += gradients.Count / numColors;
          scatterPlot.Rows.Add(row);
        }
      }
      return scatterPlot;
    }
  }
}
