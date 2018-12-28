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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HEAL.Fossil;

namespace HeuristicLab.DataPreprocessing {
  [Item("Manipulation", "Represents the available manipulations on a data set.")]
  [StorableType("42EE7A94-807F-482D-BE64-F98B05896B16")]
  public class ManipulationContent : PreprocessingContent, IViewShortcut {
    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Method; }
    }

    #region Constructor, Cloning & Persistence
    public ManipulationContent(IFilteredPreprocessingData preprocessingData)
      : base(preprocessingData) {
    }

    public ManipulationContent(ManipulationContent original, Cloner cloner) :
      base(original, cloner) {
    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new ManipulationContent(this, cloner);
    }

    [StorableConstructor]
    protected ManipulationContent(StorableConstructorFlag _) : base(_) { }
    #endregion

    public List<int> RowsWithMissingValuesGreater(double percent) {
      List<int> rows = new List<int>();

      for (int i = 0; i < PreprocessingData.Rows; ++i) {
        int missingCount = PreprocessingData.GetRowMissingValueCount(i);
        if (100f / PreprocessingData.Columns * missingCount > percent) {
          rows.Add(i);
        }
      }

      return rows;
    }

    public List<int> ColumnsWithMissingValuesGreater(double percent) {
      List<int> columns = new List<int>();
      for (int i = 0; i < PreprocessingData.Columns; ++i) {
        int missingCount = PreprocessingData.GetMissingValueCount(i);
        if (100f / PreprocessingData.Rows * missingCount > percent) {
          columns.Add(i);
        }
      }

      return columns;
    }

    public List<int> ColumnsWithVarianceSmaller(double variance) {
      List<int> columns = new List<int>();
      for (int i = 0; i < PreprocessingData.Columns; ++i) {
        if (PreprocessingData.VariableHasType<double>(i)) {
          double columnVariance = PreprocessingData.GetVariance<double>(i);
          if (columnVariance < variance) {
            columns.Add(i);
          }
        } else if (PreprocessingData.VariableHasType<DateTime>(i)) {
          double columnVariance = (double)PreprocessingData.GetVariance<DateTime>(i).Ticks / TimeSpan.TicksPerSecond;
          if (columnVariance < variance) {
            columns.Add(i);
          }
        }
      }
      return columns;
    }

    public void DeleteRowsWithMissingValuesGreater(double percent) {
      DeleteRows(RowsWithMissingValuesGreater(percent));
    }

    public void DeleteColumnsWithMissingValuesGreater(double percent) {
      DeleteColumns(ColumnsWithMissingValuesGreater(percent));
    }

    public void DeleteColumnsWithVarianceSmaller(double variance) {
      DeleteColumns(ColumnsWithVarianceSmaller(variance));
    }

    private void DeleteRows(List<int> rows) {
      PreprocessingData.InTransaction(() => {
        foreach (int row in rows.OrderByDescending(x => x)) {
          PreprocessingData.DeleteRow(row);
        }
      });
    }

    private void DeleteColumns(List<int> columns) {
      PreprocessingData.InTransaction(() => {
        foreach (int column in columns.OrderByDescending(x => x)) {
          PreprocessingData.DeleteColumn(column);
        }
      });
    }
  }
}
