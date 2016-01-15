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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Data;

namespace HeuristicLab.DataPreprocessing {

  [Item("DataGrid", "Represents a data grid.")]
  public class DataGridContent : Item, IStringConvertibleMatrix, IViewShortcut {

    public ITransactionalPreprocessingData PreProcessingData { get; private set; }

    public static new Image StaticItemImage {
      get { return HeuristicLab.Common.Resources.VSImageLibrary.Table; }
    }

    public ManipulationLogic ManipulationLogic { get; private set; }
    public FilterLogic FilterLogic { get; private set; }

    public int Rows {
      get {
        return PreProcessingData.Rows;
      }
      set {
        //does nothing
      }
    }

    public int Columns {
      get {
        return PreProcessingData.Columns;
      }
      set {
        //does nothing
      }
    }

    public IEnumerable<string> ColumnNames {
      get {
        return PreProcessingData.VariableNames;
      }
      set {

      }
    }

    public IEnumerable<string> RowNames {
      get {
        return Enumerable.Range(1, Rows).Select(n => n.ToString());
      }
      set {
        throw new NotSupportedException();
      }
    }

    public bool SortableView {
      get {
        return true;
      }
      set {
        throw new NotSupportedException();
      }
    }

    public bool ReadOnly {
      get { return false; }
    }


    public IDictionary<int, IList<int>> Selection {
      get { return PreProcessingData.Selection; }
      set { PreProcessingData.Selection = value; }
    }


    public DataGridContent(ITransactionalPreprocessingData preProcessingData, ManipulationLogic theManipulationLogic, FilterLogic theFilterLogic) {
      ManipulationLogic = theManipulationLogic;
      FilterLogic = theFilterLogic;
      PreProcessingData = preProcessingData;
    }

    public DataGridContent(DataGridContent dataGridContent, Cloner cloner)
      : base(dataGridContent, cloner) {

    }
    public override IDeepCloneable Clone(Cloner cloner) {
      return new DataGridContent(this, cloner);
    }

    public void DeleteRows(IEnumerable<int> rows) {
      PreProcessingData.DeleteRowsWithIndices(rows);
    }

    public void DeleteColumn(int column) {
      PreProcessingData.DeleteColumn(column);
    }

    public bool Validate(string value, out string errorMessage, int columnIndex) {
      return PreProcessingData.Validate(value, out errorMessage, columnIndex);
    }

    public string GetValue(int rowIndex, int columnIndex) {
      return PreProcessingData.GetCellAsString(columnIndex, rowIndex);
    }

    public bool SetValue(string value, int rowIndex, int columnIndex) {
      return PreProcessingData.SetValue(value, columnIndex, rowIndex);
    }

    public event DataPreprocessingChangedEventHandler Changed {
      add { PreProcessingData.Changed += value; }
      remove { PreProcessingData.Changed -= value; }
    }


    #region unused stuff/not implemented but necessary due to IStringConvertibleMatrix
#pragma warning disable 0067
    // Is not used since DataGridContentView overrides dataGridView_CellValidating and uses 
    // DataGridLogic#Validate(string value, out string errorMessage, int columnIndex)
    public bool Validate(string value, out string errorMessage) {
      errorMessage = string.Empty;
      return true;
    }

    public event EventHandler ColumnsChanged;
    public event EventHandler RowsChanged;
    public event EventHandler ColumnNamesChanged;
    public event EventHandler RowNamesChanged;
    public event EventHandler SortableViewChanged;
    public event EventHandler<EventArgs<int, int>> ItemChanged;
    public event EventHandler Reset;

#pragma warning restore 0067
    #endregion

  }
}
