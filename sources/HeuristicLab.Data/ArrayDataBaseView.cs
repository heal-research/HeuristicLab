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
  public partial class ArrayDataBaseView : ViewBase {
    public ArrayDataBase ArrayDataBase {
      get { return (ArrayDataBase)Item; }
      protected set { base.Item = value; }
    }

    public ArrayDataBaseView() {
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
    protected virtual void SetArrayElement(int index, string element) {
      throw new InvalidOperationException("SetArrayElement has to be overridden in each inherited class");
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ArrayDataBase != null) {
        int length = ArrayDataBase.Data.Length;
        lengthTextBox.Text = length + "";
        dataGridView.ColumnCount = 1;
        dataGridView.RowCount = length;
        for (int i = 0; i < length; i++) {
          dataGridView.Rows[i].Cells[0].Value = ArrayDataBase.Data.GetValue(i);
        }
      } else {
        lengthTextBox.Text = "0";
        dataGridView.ColumnCount = 1;
        dataGridView.RowCount = 0;
      }
    }

    private void lengthTextBox_Validating(object sender, CancelEventArgs e) {
      int newLength;
      if (int.TryParse(lengthTextBox.Text, out newLength)) {
        if (newLength > 0) {
          e.Cancel = false;
          if (newLength != ArrayDataBase.Data.Length) {
            CreateAndCopyArray(newLength);
          }
        } else {
          // only allow values greater than 0
          e.Cancel = true;
        }
      } else {
        e.Cancel = true;
      }
    }

    private void CreateAndCopyArray(int newLength) {
      Array newArray = Array.CreateInstance(ArrayDataBase.Data.GetType().GetElementType(), newLength);
      Array.Copy(ArrayDataBase.Data, newArray, Math.Min(newLength, ArrayDataBase.Data.Length));
      ArrayDataBase.Data = newArray;
    }

    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      if (ValidateData((string)e.FormattedValue)) {
        SetArrayElement(e.RowIndex, (string)e.FormattedValue);
        e.Cancel = false;
      } else {
        e.Cancel = true;
      }
    }

    private void lengthTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
        e.SuppressKeyPress = true;
        dataGridView.Focus();
      }
    }

    #region ArrayDataBase Events
    private void ArrayDataBase_Changed(object sender, EventArgs e) {
      Refresh();
    }
    #endregion
  }
}
