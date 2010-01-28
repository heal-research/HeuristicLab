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
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Data.Views {
  [Content(typeof(IStringConvertibleData), true)]
  public partial class StringConvertibleDataView : ObjectView {
    public IStringConvertibleData StringConvertibleData {
      get { return (IStringConvertibleData)Object; }
      set { base.Object = value; }
    }

    public StringConvertibleDataView() {
      InitializeComponent();
      Caption = "StringConvertibleData View";
      errorProvider.SetIconAlignment(valueTextBox, ErrorIconAlignment.MiddleLeft);
      errorProvider.SetIconPadding(valueTextBox, 2);
    }
    public StringConvertibleDataView(IStringConvertibleData stringConvertibleData)
      : this() {
      StringConvertibleData = stringConvertibleData;
    }

    protected override void DeregisterObjectEvents() {
      StringConvertibleData.Changed -= new ChangedEventHandler(StringConvertibleData_Changed);
      base.DeregisterObjectEvents();
    }

    protected override void RegisterObjectEvents() {
      base.RegisterObjectEvents();
      StringConvertibleData.Changed += new ChangedEventHandler(StringConvertibleData_Changed);
    }

    protected override void OnObjectChanged() {
      base.OnObjectChanged();
      if (StringConvertibleData == null) {
        Caption = "StringConvertibleData View";
        valueTextBox.Text = string.Empty;
        valueTextBox.Enabled = false;
      } else {
        Caption = StringConvertibleData.GetValue() + " (" + StringConvertibleData.GetType().Name + ")";
        valueTextBox.Text = StringConvertibleData.GetValue();
        valueTextBox.Enabled = true;
      }
    }

    private void StringConvertibleData_Changed(object sender, ChangedEventArgs e) {
      if (InvokeRequired)
        Invoke(new ChangedEventHandler(StringConvertibleData_Changed), sender, e);
      else
        valueTextBox.Text = StringConvertibleData.GetValue();
    }

    private void valueTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        valueLabel.Focus();  // set focus on label to validate data
      if (e.KeyCode == Keys.Escape) {
        valueTextBox.Text = StringConvertibleData.GetValue();
        valueLabel.Focus();  // set focus on label to validate data
      }
    }
    private void valueTextBox_Validating(object sender, CancelEventArgs e) {
      string errorMessage;
      if (!StringConvertibleData.Validate(valueTextBox.Text, out errorMessage)) {
        e.Cancel = true;
        errorProvider.SetError(valueTextBox, errorMessage);
        valueTextBox.SelectAll();
      }
    }
    private void valueTextBox_Validated(object sender, EventArgs e) {
      StringConvertibleData.SetValue(valueTextBox.Text);
      errorProvider.SetError(valueTextBox, string.Empty);
    }
  }
}
