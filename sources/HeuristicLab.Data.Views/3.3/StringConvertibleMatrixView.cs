#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Data.Views {
  [View("StringConvertibleMatrix View")]
  [Content(typeof(IStringConvertibleMatrix), true)]
  public partial class StringConvertibleMatrixView : ContentView {
    public new IStringConvertibleMatrix Content {
      get { return (IStringConvertibleMatrix)base.Content; }
      set { base.Content = value; }
    }

    public StringConvertibleMatrixView() {
      InitializeComponent();
      Caption = "StringConvertibleMatrix View";
      errorProvider.SetIconAlignment(rowsTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(rowsTextBox, 2);
      errorProvider.SetIconAlignment(columnsTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(columnsTextBox, 2);
    }
    public StringConvertibleMatrixView(IStringConvertibleMatrix content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.ItemChanged -= new EventHandler<EventArgs<int, int>>(Content_ItemChanged);
      Content.Reset -= new EventHandler(Content_Reset);
      base.DeregisterContentEvents();
    }


    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemChanged += new EventHandler<EventArgs<int, int>>(Content_ItemChanged);
      Content.Reset += new EventHandler(Content_Reset);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "StringConvertibleMatrix View";
        rowsTextBox.Text = "";
        rowsTextBox.Enabled = false;
        columnsTextBox.Text = "";
        columnsTextBox.Enabled = false;
        dataGridView.Rows.Clear();
        dataGridView.Columns.Clear();
        dataGridView.Enabled = false;
      } else {
        Caption = "StringConvertibleMatrix (" + Content.GetType().Name + ")";
        UpdateData();
      }
    }

    private void UpdateData() {
      rowsTextBox.Text = Content.Rows.ToString();
      rowsTextBox.Enabled = true;
      columnsTextBox.Text = Content.Columns.ToString();
      columnsTextBox.Enabled = true;
      dataGridView.Rows.Clear();
      dataGridView.Columns.Clear();
      if ((Content.Rows > 0) && (Content.Columns > 0)) {
        for (int i = 0; i < Content.Columns; i++) {
          dataGridView.ColumnCount++;
          dataGridView.Columns[dataGridView.ColumnCount - 1].FillWeight = float.Epsilon;  // sum of all fill weights must not be larger than 65535
        }
        dataGridView.RowCount = Content.Rows;
        for (int i = 0; i < Content.Rows; i++) {
          for (int j = 0; j < Content.Columns; j++)
            dataGridView.Rows[i].Cells[j].Value = Content.GetValue(i, j);
        }
        for (int i = 0; i < Content.Columns; i++)
          dataGridView.Columns[i].Width = dataGridView.Columns[i].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
      }
      dataGridView.Enabled = true;
    }

    private void Content_ItemChanged(object sender, EventArgs<int, int> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<int, int>>(Content_ItemChanged), sender, e);
      else {
        dataGridView.Rows[e.Value].Cells[e.Value2].Value = Content.GetValue(e.Value, e.Value2);
        Size size = dataGridView.Rows[e.Value].Cells[e.Value2].PreferredSize;
        dataGridView.Columns[e.Value2].Width = Math.Max(dataGridView.Columns[e.Value2].Width, size.Width);
      }
    }
    private void Content_Reset(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(Content_Reset), sender, e);
      else
        UpdateData();
    }

    #region TextBox Events
    private void rowsTextBox_Validating(object sender, CancelEventArgs e) {
      int i = 0;
      if (!int.TryParse(rowsTextBox.Text, out i) || (i < 0)) {
        e.Cancel = true;
        errorProvider.SetError(rowsTextBox, "Invalid Number of Rows (Valid Values: Positive Integers Larger or Equal to 0)");
        rowsTextBox.SelectAll();
      }
    }
    private void rowsTextBox_Validated(object sender, EventArgs e) {
      Content.Rows = int.Parse(rowsTextBox.Text);
      errorProvider.SetError(rowsTextBox, string.Empty);
    }
    private void rowsTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
        rowsLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        rowsTextBox.Text = Content.Rows.ToString();
        rowsLabel.Focus();  // set focus on label to validate data
      }
    }
    private void columnsTextBox_Validating(object sender, CancelEventArgs e) {
      int i = 0;
      if (!int.TryParse(columnsTextBox.Text, out i) || (i < 0)) {
        e.Cancel = true;
        errorProvider.SetError(columnsTextBox, "Invalid Number of Columns (Valid Values: Positive Integers Larger or Equal to 0)");
        columnsTextBox.SelectAll();
      }
    }
    private void columnsTextBox_Validated(object sender, EventArgs e) {
      Content.Columns = int.Parse(columnsTextBox.Text);
      errorProvider.SetError(columnsTextBox, string.Empty);
    }
    private void columnsTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
        columnsLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        columnsTextBox.Text = Content.Columns.ToString();
        columnsLabel.Focus();  // set focus on label to validate data
      }
    }
    #endregion

    #region DataGridView Events
    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      string errorMessage;
      if (!Content.Validate(e.FormattedValue.ToString(), out errorMessage)) {
        e.Cancel = true;
        dataGridView.Rows[e.RowIndex].ErrorText = errorMessage;
      }
    }
    private void dataGridView_CellParsing(object sender, DataGridViewCellParsingEventArgs e) {
      string value = e.Value.ToString();
      e.ParsingApplied = Content.SetValue(value, e.RowIndex, e.ColumnIndex);
      if (e.ParsingApplied) e.Value = Content.GetValue(e.RowIndex, e.ColumnIndex);
    }
    private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
      dataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
    }
    #endregion
  }
}
