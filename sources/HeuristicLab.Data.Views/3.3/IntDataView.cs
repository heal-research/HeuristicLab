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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Data.Views {
  /// <summary>
  /// The visual representation of a <see cref="Variable"/>.
  /// </summary>
  [Content(typeof(IntData), true)]
  public partial class IntDataView : ItemViewBase {
    public IntData IntData {
      get { return (IntData)Item; }
      set { base.Item = value; }
    }

    public IntDataView() {
      InitializeComponent();
      Caption = "IntData";
    }
    public IntDataView(IntData intData)
      : this() {
      IntData = intData;
    }

    protected override void DeregisterObjectEvents() {
      IntData.ValueChanged -= new EventHandler(IntData_ValueChanged);
      base.DeregisterObjectEvents();
    }

    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      IntData.ValueChanged += new EventHandler(IntData_ValueChanged);
    }

    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      if (IntData == null) {
        Caption = "IntData";
        valueTextBox.Text = "-";
        valueTextBox.Enabled = false;
      } else {
        Caption = IntData.Value.ToString() + " (" + IntData.GetType().Name + ")";
        valueTextBox.Text = IntData.Value.ToString();
        valueTextBox.Enabled = true;
      }
    }

    private void IntData_ValueChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new EventHandler(IntData_ValueChanged), sender, e);
      else
        valueTextBox.Text = IntData.Value.ToString();
    }

    private void valueTextBox_Validating(object sender, CancelEventArgs e) {
      int oldValue = IntData.Value;
      int value;
      e.Cancel = !int.TryParse(valueTextBox.Text, out value);
      if (e.Cancel) {
        MessageBox.Show(this, "\"" + valueTextBox.Text + "\" is not a valid integer value.", "Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
        valueTextBox.Text = oldValue.ToString();
        valueTextBox.SelectAll();
        valueTextBox.Focus();
      }
    }
    private void valueTextBox_Validated(object sender, EventArgs e) {
      IntData.Value = int.Parse(valueTextBox.Text);
    }
  }
}
