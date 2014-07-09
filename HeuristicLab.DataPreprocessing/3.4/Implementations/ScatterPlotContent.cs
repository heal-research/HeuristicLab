#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.DataPreprocessing.Interfaces;

namespace HeuristicLab.DataPreprocessing {

  [Item("ScatterPlot", "Represents a scatter plot.")]
  public class ScatterPlotContent : PreprocessingChartContent {

    public string SelectedXVariable { get; set; }
    public string SelectedYVariable { get; set; }

    public ScatterPlotContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
    }

    public ScatterPlotContent(ScatterPlotContent content, Cloner cloner)
      : base(content, cloner) {
      this.SelectedXVariable = content.SelectedXVariable;
      this.SelectedYVariable = content.SelectedYVariable;
    }

    public static new Image StaticItemImage
    {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Performance; }
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new ScatterPlotContent(this, cloner);
    }

    public ScatterPlot CreateScatterPlot(string variableNameX, string variableNameY) {
      ScatterPlot scatterPlot = new ScatterPlot();

      IList<double> xValues = PreprocessingData.GetValues<double>(PreprocessingData.GetColumnIndex(variableNameX));
      IList<double> yValues = PreprocessingData.GetValues<double>(PreprocessingData.GetColumnIndex(variableNameY));

      List<Point2D<double>> points = new List<Point2D<double>>();

      for (int i = 0; i < xValues.Count; i++) {
        Point2D<double> point = new Point2D<double>(xValues[i], yValues[i]);
        points.Add(point);
      }

      ScatterPlotDataRow scdr = new ScatterPlotDataRow(variableNameX + " - " + variableNameY, "", points);
      scatterPlot.Rows.Add(scdr);
      return scatterPlot;
    }
  }
}
