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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.DataPreprocessing {
  [Item("PreprocessingChart", "Represents a preprocessing chart.")]
  public class PreprocessingChartContent : Item, IViewShortcut {
    public enum LegendOrder {
      Alphabetically,
      Appearance
    }

    public static new Image StaticItemImage {
      get { return VSImageLibrary.PieChart; }
    }

    private ICheckedItemList<StringValue> variableItemList = null;
    public ICheckedItemList<StringValue> VariableItemList {
      get {
        if (variableItemList == null)
          variableItemList = CreateVariableItemList(PreprocessingData);
        return this.variableItemList;
      }
    }

    public IFilteredPreprocessingData PreprocessingData { get; private set; }
    public event DataPreprocessingChangedEventHandler Changed {
      add { PreprocessingData.Changed += value; }
      remove { PreprocessingData.Changed -= value; }
    }

    public PreprocessingChartContent(IFilteredPreprocessingData preprocessingData) {
      PreprocessingData = preprocessingData;
    }

    public PreprocessingChartContent(PreprocessingChartContent content, Cloner cloner)
      : base(content, cloner) {
      this.PreprocessingData = content.PreprocessingData;
      this.variableItemList = cloner.Clone<ICheckedItemList<StringValue>>(variableItemList);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PreprocessingChartContent(this, cloner);
    }

    public DataRow CreateDataRow(string variableName, DataRowVisualProperties.DataRowChartType chartType) {
      return CreateDataRow(PreprocessingData, variableName, chartType);
    }

    public static DataRow CreateDataRow(IFilteredPreprocessingData preprocessingData, string variableName, DataRowVisualProperties.DataRowChartType chartType) {
      IList<double> values = preprocessingData.GetValues<double>(preprocessingData.GetColumnIndex(variableName));
      DataRow row = new DataRow(variableName, "", values);
      row.VisualProperties.ChartType = chartType;
      return row;
    }

    private static ICheckedItemList<StringValue> CreateVariableItemList(IPreprocessingData preprocessingData) {
      ICheckedItemList<StringValue> itemList = new CheckedItemList<StringValue>();
      foreach (string name in preprocessingData.GetDoubleVariableNames()) {
        var n = new StringValue(name);
        bool isInputTarget = preprocessingData.InputVariables.Contains(name) || preprocessingData.TargetVariable == name;
        itemList.Add(n, isInputTarget);
      }
      return new ReadOnlyCheckedItemList<StringValue>(itemList);
    }

    public static IEnumerable<string> GetVariableNamesForGrouping(IPreprocessingData preprocessingData, int maxDistinctValues = 20) {
      var variableNames = new List<string>();

      for (int i = 0; i < preprocessingData.Columns; ++i) {
        int distinctValues = Int32.MaxValue;
        if (preprocessingData.VariableHasType<double>(i))
          distinctValues = preprocessingData.GetValues<double>(i).GroupBy(x => x).Count();
        else if (preprocessingData.VariableHasType<string>(i))
          distinctValues = preprocessingData.GetValues<string>(i).GroupBy(x => x).Count();
        else if (preprocessingData.VariableHasType<DateTime>(i))
          distinctValues = preprocessingData.GetValues<DateTime>(i).GroupBy(x => x).Count();

        if (distinctValues <= maxDistinctValues)
          variableNames.Add(preprocessingData.GetVariableName(i));
      }
      return variableNames;
    }
  }
}
