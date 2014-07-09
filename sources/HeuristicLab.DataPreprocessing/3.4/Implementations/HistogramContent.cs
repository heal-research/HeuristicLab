#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.DataPreprocessing.Interfaces;

namespace HeuristicLab.DataPreprocessing {
  [Item("Histogram", "Represents the histogram grid.")]
  public class HistogramContent : PreprocessingChartContent {

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Statistics; }
    }
    private const int MAX_DISTINCT_VALUES_FOR_CLASSIFCATION = 20;

    private int classifierVariableIndex = 0;

    public int ClassifierVariableIndex {
      get { return this.classifierVariableIndex; }
      set { this.classifierVariableIndex = value; }
    }


    public HistogramContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
      AllInOneMode = false;
    }

    public HistogramContent(HistogramContent content, Cloner cloner)
      : base(content, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new HistogramContent(this, cloner);
    }

    public IEnumerable<string> GetVariableNamesForHistogramClassification() {
      List<string> doubleVariableNames = new List<string>();

      //only return variable names from type double
      for (int i = 0; i < PreprocessingData.Columns; ++i) {
        if (PreprocessingData.VariableHasType<double>(i)) {
          double distinctValueCount = PreprocessingData.GetValues<double>(i).GroupBy(x => x).Count();
          bool distinctValuesOk = distinctValueCount <= MAX_DISTINCT_VALUES_FOR_CLASSIFCATION;
          if (distinctValuesOk)
            doubleVariableNames.Add(PreprocessingData.GetVariableName(i));
        }
      }
      return doubleVariableNames;
    }

  }
}
