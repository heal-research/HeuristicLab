#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.DataPreprocessing {
  [Item("PreprocessingChart", "Represents a preprocessing chart.")]
  [StorableClass]
  public class PreprocessingChartContent : PreprocessingContent, IViewShortcut {
    public enum LegendOrder {
      Alphabetically,
      Appearance
    }

    public static new Image StaticItemImage {
      get { return VSImageLibrary.PieChart; }
    }

    [Storable]
    private ICheckedItemList<StringValue> variableItemList;
    public ICheckedItemList<StringValue> VariableItemList {
      get {
        if (variableItemList == null)
          variableItemList = CreateVariableItemList(PreprocessingData);
        return variableItemList;
      }
    }

    public event DataPreprocessingChangedEventHandler Changed {
      add { PreprocessingData.Changed += value; }
      remove { PreprocessingData.Changed -= value; }
    }

    #region Constructor, Cloning & Persistence
    public PreprocessingChartContent(IFilteredPreprocessingData preprocessingData)
       : base(preprocessingData) {
    }

    public PreprocessingChartContent(PreprocessingChartContent original, Cloner cloner)
      : base(original, cloner) {
      variableItemList = cloner.Clone(original.variableItemList);
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new PreprocessingChartContent(this, cloner);
    }

    [StorableConstructor]
    protected PreprocessingChartContent(bool deserializing)
      : base(deserializing) { }
    #endregion

    public DataRow CreateDataRow(string variableName, DataRowVisualProperties.DataRowChartType chartType) {
      return CreateDataRow(PreprocessingData, variableName, chartType);
    }

    public static DataRow CreateDataRow(IFilteredPreprocessingData preprocessingData, string variableName, DataRowVisualProperties.DataRowChartType chartType) {
      var values = preprocessingData.GetValues<double>(preprocessingData.GetColumnIndex(variableName));
      var row = new DataRow(variableName, "", values) {
        VisualProperties = {
          ChartType = chartType,
          StartIndexZero = true
        }
      };
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
