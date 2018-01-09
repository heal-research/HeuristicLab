#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.DataPreprocessing {
  [Item("Histogram", "Represents the histogram grid.")]
  [StorableClass]
  public class HistogramContent : PreprocessingChartContent {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Statistics; }
    }

    [Storable]
    public string GroupingVariableName { get; set; }

    [Storable]
    public int Bins { get; set; }
    [Storable]
    public bool ExactBins { get; set; }

    [Storable]
    public LegendOrder Order { get; set; }

    #region Constructor, Cloning & Persistence
    public HistogramContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
      Bins = 10;
      ExactBins = false;
    }

    public HistogramContent(HistogramContent original, Cloner cloner)
      : base(original, cloner) {
      GroupingVariableName = original.GroupingVariableName;
      Bins = original.Bins;
      ExactBins = original.ExactBins;
      Order = original.Order;
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HistogramContent(this, cloner);
    }

    [StorableConstructor]
    protected HistogramContent(bool deserializing)
      : base(deserializing) { }
    #endregion

    public static DataTable CreateHistogram(IFilteredPreprocessingData preprocessingData, string variableName, string groupingVariableName, DataTableVisualProperties.DataTableHistogramAggregation aggregation, LegendOrder legendOrder = LegendOrder.Alphabetically) {
      var dataTable = new DataTable {
        VisualProperties = { Title = variableName, HistogramAggregation = aggregation },
      };

      if (string.IsNullOrEmpty(groupingVariableName)) {
        var row = PreprocessingChartContent.CreateDataRow(preprocessingData, variableName, DataRowVisualProperties.DataRowChartType.Histogram);
        row.VisualProperties.IsVisibleInLegend = false;
        dataTable.Rows.Add(row);
        return dataTable;
      }

      int variableIndex = preprocessingData.GetColumnIndex(variableName);
      var variableValues = preprocessingData.GetValues<double>(variableIndex);
      int groupVariableIndex = preprocessingData.GetColumnIndex(groupingVariableName);
      var groupingValues = Enumerable.Empty<string>();

      if (preprocessingData.VariableHasType<double>(groupVariableIndex)) {
        groupingValues = preprocessingData.GetValues<double>(groupVariableIndex).Select(x => x.ToString());
      } else if (preprocessingData.VariableHasType<string>(groupVariableIndex)) {
        groupingValues = preprocessingData.GetValues<string>(groupVariableIndex);
      } else if (preprocessingData.VariableHasType<DateTime>(groupVariableIndex)) {
        groupingValues = preprocessingData.GetValues<DateTime>(groupVariableIndex).Select(x => x.ToString());
      }

      var groups = groupingValues.Zip(variableValues, Tuple.Create).GroupBy(t => t.Item1, t => t.Item2);

      if (legendOrder == LegendOrder.Alphabetically)
        groups = groups.OrderBy(x => x.Key, new NaturalStringComparer());

      foreach (var group in groups) {
        var classRow = new DataRow {
          Name = group.Key,
          VisualProperties = {
              ChartType = DataRowVisualProperties.DataRowChartType.Histogram,
              IsVisibleInLegend = !string.IsNullOrEmpty(groupingVariableName)
            }
        };
        classRow.Values.AddRange(group);
        dataTable.Rows.Add(classRow);
      }
      return dataTable;
    }
  }
}
