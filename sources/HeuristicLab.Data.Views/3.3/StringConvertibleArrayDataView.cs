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
  [Content(typeof(IStringConvertibleArrayData), true)]
  public partial class StringConvertibleArrayDataView : ObjectView {
    public IStringConvertibleArrayData StringConvertibleArrayData {
      get { return (IStringConvertibleArrayData)Object; }
      set { base.Object = value; }
    }

    public StringConvertibleArrayDataView() {
      InitializeComponent();
      Caption = "StringConvertibleArrayDataView View";
      errorProvider.SetIconAlignment(sizeTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(sizeTextBox, 2);
    }
    public StringConvertibleArrayDataView(IStringConvertibleArrayData stringConvertibleArrayData)
      : this() {
      StringConvertibleArrayData = stringConvertibleArrayData;
    }

    protected override void DeregisterObjectEvents() {
      StringConvertibleArrayData.ItemChanged -= new EventHandler<EventArgs<int>>(StringConvertibleArrayData_ItemChanged);
      StringConvertibleArrayData.Reset -= new EventHandler(StringConvertibleArrayData_Reset);
      base.DeregisterObjectEvents();
    }


    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      StringConvertibleArrayData.ItemChanged += new EventHandler<EventArgs<int>>(StringConvertibleArrayData_ItemChanged);
      StringConvertibleArrayData.Reset += new EventHandler(StringConvertibleArrayData_Reset);
    }

    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      if (StringConvertibleArrayData == null) {
        Caption = "StringConvertibleData View";
        sizeTextBox.Text = "";
        sizeTextBox.Enabled = false;
        dataGridView.Rows.Clear();
        dataGridView.Enabled = false;
      } else {
        Caption = "StringConvertibleArrayData (" + StringConvertibleArrayData.GetType().Name + ")";
        UpdateContent();
      }
    }

    private void UpdateContent() {
      sizeTextBox.Text = StringConvertibleArrayData.Length.ToString();
      sizeTextBox.Enabled = true;
      dataGridView.Rows.Clear();
      dataGridView.ColumnCount = 1;
      dataGridView.RowCount = StringConvertibleArrayData.Length;
      for (int i = 0; i < StringConvertibleArrayData.Length; i++)
        dataGridView.Rows[i].Cells[0].Value = StringConvertibleArrayData.GetValue(i);
      dataGridView.Enabled = true;
    }

    private void StringConvertibleArrayData_ItemChanged(object sender, EventArgs<int> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<EventArgs<int>>(StringConvertibleArrayData_ItemChanged), sender, e);
      else
        dataGridView.Rows[e.Value].Cells[0].Value = StringConvertibleArrayData.GetValue(e.Value);
    }
    private void StringConvertibleArrayData_Reset(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(StringConvertibleArrayData_Reset), sender, e);
      else
        UpdateContent();
    }

    private void sizeTextBox_Validating(object sender, CancelEventArgs e) {
      int i = 0;
      if (!int.TryParse(sizeTextBox.Text, out i) || (i < 0)) {
        e.Cancel = true;
        errorProvider.SetError(sizeTextBox, "Invalid Array Length");
        sizeTextBox.SelectAll();
      }
    }
    private void sizeTextBox_Validated(object sender, EventArgs e) {
      StringConvertibleArrayData.Length = int.Parse(sizeTextBox.Text);
      errorProvider.SetError(sizeTextBox, string.Empty);
    }
    private void sizeTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
        sizeLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        sizeTextBox.Text = StringConvertibleArrayData.Length.ToString();
        sizeLabel.Focus();  // set focus on label to validate data
      }
    }
    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      if (!StringConvertibleArrayData.Validate(e.FormattedValue.ToString())) {
        e.Cancel = true;
        dataGridView.Rows[e.RowIndex].ErrorText = "Invalid Value";
      }
    }
    private void dataGridView_CellParsing(object sender, DataGridViewCellParsingEventArgs e) {
      string value = e.Value.ToString();
      e.ParsingApplied = StringConvertibleArrayData.SetValue(value, e.RowIndex);
      if (e.ParsingApplied) e.Value = StringConvertibleArrayData.GetValue(e.RowIndex);
    }
    private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
      dataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
    }
  }
}
