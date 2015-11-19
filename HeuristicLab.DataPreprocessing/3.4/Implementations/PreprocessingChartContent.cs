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
using HeuristicLab.Analysis;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;
using HeuristicLab.DataPreprocessing.Interfaces;

namespace HeuristicLab.DataPreprocessing {
  [Item("PreprocessingChart", "Represents a preprocessing chart.")]
  public class PreprocessingChartContent : Item, IViewChartShortcut {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.PieChart; }
    }

    private bool allInOneMode = true;
    public bool AllInOneMode {
      get { return this.allInOneMode; }
      set { this.allInOneMode = value; }
    }

    private ICheckedItemList<StringValue> variableItemList = null;
    public ICheckedItemList<StringValue> VariableItemList {
      get { return this.variableItemList; }
      set { this.variableItemList = value; }
    }

    public IFilteredPreprocessingData PreprocessingData { get; private set; }

    public PreprocessingChartContent(IFilteredPreprocessingData preprocessingData) {
      PreprocessingData = preprocessingData;
    }

    public PreprocessingChartContent(PreprocessingChartContent content, Cloner cloner)
      : base(content, cloner) {
      this.allInOneMode = content.allInOneMode;
      this.PreprocessingData = content.PreprocessingData;
      this.variableItemList = cloner.Clone<ICheckedItemList<StringValue>>(variableItemList);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PreprocessingChartContent(this, cloner);
    }


    public DataRow CreateDataRow(string variableName, DataRowVisualProperties.DataRowChartType chartType) {
      IList<double> values = PreprocessingData.GetValues<double>(PreprocessingData.GetColumnIndex(variableName));
      DataRow row = new DataRow(variableName, "", values);
      row.VisualProperties.ChartType = chartType;
      return row;
    }

    public List<DataRow> CreateAllDataRows(DataRowVisualProperties.DataRowChartType chartType) {
      List<DataRow> dataRows = new List<DataRow>();
      foreach (var name in PreprocessingData.GetDoubleVariableNames())
        dataRows.Add(CreateDataRow(name, chartType));
      return dataRows;
    }

    public DataRow CreateSelectedDataRow(string variableName, DataRowVisualProperties.DataRowChartType chartType) {

      IDictionary<int, IList<int>> selection = PreprocessingData.Selection;
      int variableIndex = PreprocessingData.GetColumnIndex(variableName);

      if (selection.Keys.Contains(variableIndex)) {
        List<int> selectedIndices = new List<int>(selection[variableIndex]);
        //need selection with more than 1 value
        if (selectedIndices.Count < 2)
          return null;

        selectedIndices.Sort();
        int start = selectedIndices[0];
        int end = selectedIndices[selectedIndices.Count - 1];

        DataRow rowSelect = CreateDataRowRange(variableName, start, end, chartType);
        return rowSelect;
      } else
        return null;
    }

    public DataRow CreateDataRowRange(string variableName, int start, int end, DataRowVisualProperties.DataRowChartType chartType) {
      IList<double> values = PreprocessingData.GetValues<double>(PreprocessingData.GetColumnIndex(variableName));
      IList<double> valuesRange = new List<double>();
      for (int i = 0; i < values.Count; i++) {
        if (i >= start && i <= end)
          valuesRange.Add(values[i]);
        else
          valuesRange.Add(Double.NaN);
      }

      DataRow row = new DataRow(variableName, "", valuesRange);
      row.VisualProperties.ChartType = chartType;
      return row;
    }

    public List<DataRow> CreateAllSelectedDataRows(DataRowVisualProperties.DataRowChartType chartType) {
      List<DataRow> dataRows = new List<DataRow>();
      foreach (var name in PreprocessingData.GetDoubleVariableNames()) {
        DataRow row = CreateSelectedDataRow(name, chartType);
        if (row != null)
          dataRows.Add(row);
      }
      return dataRows;
    }


    public ICheckedItemList<StringValue> CreateVariableItemList(IEnumerable<string> checkedItems = null) {
      ICheckedItemList<StringValue> itemList = new CheckedItemList<StringValue>();
      foreach (string name in PreprocessingData.GetDoubleVariableNames()) {
        var n = new StringValue(name);
        itemList.Add(n, (checkedItems == null) ? true : checkedItems.Contains(name));
      }
      return new ReadOnlyCheckedItemList<StringValue>(itemList);
    }

    public event DataPreprocessingChangedEventHandler Changed {
      add { PreprocessingData.Changed += value; }
      remove { PreprocessingData.Changed -= value; }
    }

  }
}
