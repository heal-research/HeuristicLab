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
    }
    public StringConvertibleArrayDataView(IStringConvertibleArrayData stringConvertibleArrayData)
      : this() {
      StringConvertibleArrayData = stringConvertibleArrayData;
    }

    protected override void DeregisterObjectEvents() {
      StringConvertibleArrayData.ItemChanged -= new EventHandler<EventArgs<int>>(StringConvertibleArrayData_ItemChanged);
      StringConvertibleArrayData.Changed -= new ChangedEventHandler(StringConvertibleArrayData_Changed);
      base.DeregisterObjectEvents();
    }


    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      StringConvertibleArrayData.ItemChanged += new EventHandler<EventArgs<int>>(StringConvertibleArrayData_ItemChanged);
      StringConvertibleArrayData.Changed += new ChangedEventHandler(StringConvertibleArrayData_Changed);
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
      if (StringConvertibleArrayData.Length > 0) {
        dataGridView.ColumnCount = 1;
        dataGridView.Rows.Add(StringConvertibleArrayData.Length);
      }
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
    private void StringConvertibleArrayData_Changed(object sender, ChangedEventArgs e) {
      if (InvokeRequired)
        Invoke(new ChangedEventHandler(StringConvertibleArrayData_Changed), sender, e);
      else
        UpdateContent();
    }

    private void sizeTextBox_Validating(object sender, CancelEventArgs e) {
      int i = 0;
      e.Cancel = e.Cancel || !int.TryParse(sizeTextBox.Text, out i);
      e.Cancel = e.Cancel || (i < 0);
      if (e.Cancel) {
        MessageBox.Show(this, "\"" + sizeTextBox.Text + "\" is not a valid array length.", "Invalid Array Length", MessageBoxButtons.OK, MessageBoxIcon.Error);
        sizeTextBox.SelectAll();
        sizeTextBox.Focus();
      }
    }
    private void sizeTextBox_Validated(object sender, EventArgs e) {
      StringConvertibleArrayData.Length = int.Parse(sizeTextBox.Text);
    }
    private void sizeTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
        sizeLabel.Focus();
    }
    private void dataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
      e.Cancel = e.Cancel || !StringConvertibleArrayData.SetValue(e.FormattedValue.ToString(), e.RowIndex);
      if (e.Cancel)
        MessageBox.Show(this, "\"" + e.FormattedValue.ToString() + "\" is not a valid value.", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    private void dataGridView_CellValidated(object sender, DataGridViewCellEventArgs e) {
      dataGridView.Rows[e.RowIndex].Cells[0].Value = StringConvertibleArrayData.GetValue(e.RowIndex);
    }
  }
}
