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
  [View("StringConvertibleArrayData View")]
  [Content(typeof(IStringConvertibleArrayData), true)]
  public partial class StringConvertibleArrayDataView : ContentView {
    public new IStringConvertibleArrayData Content {
      get { return (IStringConvertibleArrayData)base.Content; }
      set { base.Content = value; }
    }

    public StringConvertibleArrayDataView() {
      InitializeComponent();
      Caption = "StringConvertibleArrayData View";
      errorProvider.SetIconAlignment(rowsTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(rowsTextBox, 2);
    }
    public StringConvertibleArrayDataView(IStringConvertibleArrayData content)
      : this() {
      Content = content;
    }

    protected override void DeregisterContentEvents() {
      Content.ItemChanged -= new EventHandler<EventArgs<int>>(Content_ItemChanged);
      Content.Reset -= new EventHandler(Content_Reset);
      base.DeregisterContentEvents();
    }


    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ItemChanged += new EventHandler<EventArgs<int>>(Content_ItemChanged);
      Content.Reset += new EventHandler(Content_Reset);
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        Caption = "StringConvertibleArrayData View";
        rowsTextBox.Text = "";
        rowsTextBox.Enabled = false;
        dataGridView.Rows.Clear();
        dataGridView.Columns.Clear();
        dataGridView.Enabled = false;
      } else {
        Caption = "StringConvertibleArrayData (" + Content.GetType().Name + ")";
        UpdateData();
      }
    }

    private void UpdateData() {
      rowsTextBox.Text = Content.Rows.ToString();
      rowsTextBox.Enabled = true;
      dataGridView.Rows.Clear();
      dataGridView.Columns.Clear();
      if (Content.Rows > 0) {
        dataGridView.ColumnCount++;
        dataGridView.Columns[0].FillWeight = float.Epsilon;  // sum of all fill weights must not be larger than 65535
        dataGridView.RowCount = Content.Rows;
        for (int i = 0; i < Content.Rows; i++) {
          dataGridView.Rows[i].Cells[0].Value = Content.GetValue(i);
        }
        dataGridView.Columns[0].Width = dataGridView.Columns[0].GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells, true);
      }
      dataGridView.Enabled = true;
    }

    private void Content_ItemChanged(object sender, EventArgs<int> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<int>>(Content_ItemChanged), sender, e);
      else {
        dataGridView.Rows[e.Value].Cells[0].Value = Content.GetValue(e.Value);
        Size size = dataGridView.Rows[e.Value].Cells[0].PreferredSize;
        dataGridView.Columns[0].Width = Math.Max(dataGridView.Columns[0].Width, size.Width);
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
      e.ParsingApplied = Content.SetValue(value, e.RowIndex);
      if (e.ParsingApplied) e.Value = Content.GetValue(e.RowIndex);
    }
    private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
      dataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
    }
    #endregion
  }
}
