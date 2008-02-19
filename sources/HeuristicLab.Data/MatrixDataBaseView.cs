#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  public partial class MatrixDataBaseView : ViewBase {
    public ArrayDataBase ArrayDataBase {
      get { return (ArrayDataBase)Item; }
      protected set { base.Item = value; }
    }

    public MatrixDataBaseView() {
      InitializeComponent();
    }

    protected override void RemoveItemEvents() {
      ArrayDataBase.Changed -= new EventHandler(ArrayDataBase_Changed);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ArrayDataBase.Changed += new EventHandler(ArrayDataBase_Changed);
    }

    protected virtual bool ValidateData(string element) {
      throw new InvalidOperationException("ValidateData has to be overridden in each inherited class");
    }
    protected virtual void SetArrayElement(int row, int column, string element) {
      throw new InvalidOperationException("SetArrayElement has to be overridden in each inherited class");
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ArrayDataBase != null) {
        int rows = ArrayDataBase.Data.GetLength(0);
        int columns = ArrayDataBase.Data.GetLength(1);

        rowsTextBox.Text = rows + "";
        columnsTextBox.Text = columns + "";
        dataGridView.ColumnCount = columns;
        dataGridView.RowCount = rows;
        for (int i = 0; i < rows; i++) {
          for (int j = 0; j < columns; j++) {
            dataGridView.Rows[i].Cells[j].Value = ArrayDataBase.Data.GetValue(i, j);
          }
        }
      } else {
        rowsTextBox.Text = "1";
        columnsTextBox.Text = "1";
        dataGridView.ColumnCount = 1;
        dataGridView.RowCount = 1;
      }
    }

    private void textBox_Validating(object sender, CancelEventArgs e) {
      int newValue;
      TextBox source = (TextBox)sender;
      if (int.TryParse(source.Text, out newValue)) {
        if (newValue > 0) {
          e.Cancel = false;
        } else {
          e.Cancel = true;
        }
      } else {
        e.Cancel = true;
      }
    }

    private void CreateAndCopyArray(int newRows, int newColumns) {
      Array newArray = Array.CreateInstance(ArrayDataBase.Data.GetType().GetElementType(), newRows, newColumns);
      Array.Copy(ArrayDataBase.Data, newArray, Math.Min(newArray.Length, ArrayDataBase.Data.Length));
      ArrayDataBase.Data = newArray;
    }

    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      if (ValidateData((string)e.FormattedValue)) {
        SetArrayElement(e.RowIndex, e.ColumnIndex, (string)e.FormattedValue);
        e.Cancel = false;
        Refresh();
      } else {
        e.Cancel = true;
      }
    }

    private void textBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
        e.SuppressKeyPress = true;
        dataGridView.Focus();
      }
    }

    private void textBox_Validated(object sender, EventArgs e) {
      int newRows;
      int newColumns;
      if (int.TryParse(columnsTextBox.Text, out newColumns) && int.TryParse(rowsTextBox.Text, out newRows)) {
        CreateAndCopyArray(newRows, newColumns);
      } else {
        throw new FormatException();
      }
    }

    #region ArrayDataBase Events
    private void ArrayDataBase_Changed(object sender, EventArgs e) {
      Refresh();
    }
    #endregion
  }
}
