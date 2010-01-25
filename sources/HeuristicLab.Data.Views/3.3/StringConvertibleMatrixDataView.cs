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
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Data.Views {
  [Content(typeof(IStringConvertibleMatrixData), true)]
  public partial class StringConvertibleMatrixDataView : ObjectView {
    public IStringConvertibleMatrixData StringConvertibleMatrixData {
      get { return (IStringConvertibleMatrixData)Object; }
      set { base.Object = value; }
    }

    public StringConvertibleMatrixDataView() {
      InitializeComponent();
      Caption = "StringConvertibleMatrixDataView View";
      errorProvider.SetIconAlignment(rowsTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(rowsTextBox, 2);
      errorProvider.SetIconAlignment(columnsTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(columnsTextBox, 2);
    }
    public StringConvertibleMatrixDataView(IStringConvertibleMatrixData stringConvertibleArrayData)
      : this() {
      StringConvertibleMatrixData = stringConvertibleArrayData;
    }

    protected override void DeregisterObjectEvents() {
      StringConvertibleMatrixData.ItemChanged -= new EventHandler<EventArgs<int, int>>(StringConvertibleMatrixData_ItemChanged);
      StringConvertibleMatrixData.Reset -= new EventHandler(StringConvertibleMatrixData_Reset);
      base.DeregisterObjectEvents();
    }


    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      StringConvertibleMatrixData.ItemChanged += new EventHandler<EventArgs<int, int>>(StringConvertibleMatrixData_ItemChanged);
      StringConvertibleMatrixData.Reset += new EventHandler(StringConvertibleMatrixData_Reset);
    }

    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      if (StringConvertibleMatrixData == null) {
        Caption = "StringConvertibleMatrixData View";
        rowsTextBox.Text = "";
        rowsTextBox.Enabled = false;
        columnsTextBox.Text = "";
        columnsTextBox.Enabled = false;
        dataGridView.Rows.Clear();
        dataGridView.Columns.Clear();
        dataGridView.Enabled = false;
      } else {
        Caption = "StringConvertibleMatrixData (" + StringConvertibleMatrixData.GetType().Name + ")";
        UpdateContent();
      }
    }

    private void UpdateContent() {
      rowsTextBox.Text = StringConvertibleMatrixData.Rows.ToString();
      rowsTextBox.Enabled = (StringConvertibleMatrixData.Dimensions & StringConvertibleArrayDataDimensions.Rows) == StringConvertibleArrayDataDimensions.Rows;
      columnsTextBox.Text = StringConvertibleMatrixData.Columns.ToString();
      columnsTextBox.Enabled = (StringConvertibleMatrixData.Dimensions & StringConvertibleArrayDataDimensions.Columns) == StringConvertibleArrayDataDimensions.Columns;
      dataGridView.Rows.Clear();
      dataGridView.Columns.Clear();
      if ((StringConvertibleMatrixData.Rows > 0) && (StringConvertibleMatrixData.Columns > 0)) {
        for (int i = 0; i < StringConvertibleMatrixData.Columns; i++) {
          dataGridView.ColumnCount++;
          dataGridView.Columns[dataGridView.ColumnCount - 1].FillWeight = float.Epsilon;  // sum of all fill weights must not be larger than 65535
        }
        dataGridView.RowCount = StringConvertibleMatrixData.Rows;
        for (int i = 0; i < StringConvertibleMatrixData.Rows; i++) {
          for (int j = 0; j < StringConvertibleMatrixData.Columns; j++)
            dataGridView.Rows[i].Cells[j].Value = StringConvertibleMatrixData.GetValue(i, j);
        }
        for (int i = 0; i < StringConvertibleMatrixData.Columns; i++)
          dataGridView.Columns[i].Width = dataGridView.Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
      }
      dataGridView.Enabled = true;
    }

    private void StringConvertibleMatrixData_ItemChanged(object sender, EventArgs<int, int> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<int, int>>(StringConvertibleMatrixData_ItemChanged), sender, e);
      else {
        dataGridView.Rows[e.Value].Cells[e.Value2].Value = StringConvertibleMatrixData.GetValue(e.Value, e.Value2);
        Size size = dataGridView.Rows[e.Value].Cells[e.Value2].PreferredSize;
        dataGridView.Columns[e.Value2].Width = Math.Max(dataGridView.Columns[e.Value2].Width, size.Width);
      }
    }
    private void StringConvertibleMatrixData_Reset(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(StringConvertibleMatrixData_Reset), sender, e);
      else
        UpdateContent();
    }

    #region TextBox Events
    private void rowsTextBox_Validating(object sender, CancelEventArgs e) {
      int i = 0;
      if (!int.TryParse(rowsTextBox.Text, out i) || (i < 0)) {
        e.Cancel = true;
        errorProvider.SetError(rowsTextBox, "Invalid Number of Rows");
        rowsTextBox.SelectAll();
      }
    }
    private void rowsTextBox_Validated(object sender, EventArgs e) {
      StringConvertibleMatrixData.Rows = int.Parse(rowsTextBox.Text);
      errorProvider.SetError(rowsTextBox, string.Empty);
    }
    private void rowsTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
        rowsLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        rowsTextBox.Text = StringConvertibleMatrixData.Columns.ToString();
        rowsLabel.Focus();  // set focus on label to validate data
      }
    }
    private void columnsTextBox_Validating(object sender, CancelEventArgs e) {
      int i = 0;
      if (!int.TryParse(columnsTextBox.Text, out i) || (i < 0)) {
        e.Cancel = true;
        errorProvider.SetError(columnsTextBox, "Invalid Number of Columns");
        columnsTextBox.SelectAll();
      }
    }
    private void columnsTextBox_Validated(object sender, EventArgs e) {
      StringConvertibleMatrixData.Columns = int.Parse(columnsTextBox.Text);
      errorProvider.SetError(columnsTextBox, string.Empty);
    }
    private void columnsTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
        columnsLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        columnsTextBox.Text = StringConvertibleMatrixData.Columns.ToString();
        columnsLabel.Focus();  // set focus on label to validate data
      }
    }
    #endregion

    #region DataGridView Events
    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      if (!StringConvertibleMatrixData.Validate(e.FormattedValue.ToString())) {
        e.Cancel = true;
        dataGridView.Rows[e.RowIndex].ErrorText = "Invalid Value";
      }
    }
    private void dataGridView_CellParsing(object sender, DataGridViewCellParsingEventArgs e) {
      string value = e.Value.ToString();
      e.ParsingApplied = StringConvertibleMatrixData.SetValue(value, e.RowIndex, e.ColumnIndex);
      if (e.ParsingApplied) e.Value = StringConvertibleMatrixData.GetValue(e.RowIndex, e.ColumnIndex);
    }
    private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
      dataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
    }
    #endregion
  }
}
