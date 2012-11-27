#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Data;

namespace HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views {
  public partial class ValueChangeDialog : Form {
    public string Caption {
      get { return this.Text; }
      set {
        if (InvokeRequired)
          Invoke(new Action<string>(x => this.Caption = x), value);
        else
          this.Text = value;
      }
    }
    public string OriginalValue {
      get { return originalValueTextBox.Text; }
      set {
        if (InvokeRequired)
          Invoke(new Action<string>(x => this.NewValue = x), value);
        else
          originalValueTextBox.Text = value;
      }
    }
    public string NewValue {
      get { return newValueTextBox.Text; }
      set {
        if (InvokeRequired)
          Invoke(new Action<string>(x => this.NewValue = x), value);
        else
          newValueTextBox.Text = value;
      }
    }
    public TextBox NewValueTextBox {
      get { return newValueTextBox; }
    }

    public ValueChangeDialog() {
      InitializeComponent();
      originalValueTextBox.TabStop = false; // cannot receive focus using tab key
    }
    public ValueChangeDialog(string caption, string originalValue, string newValue)
      : this() {
      Caption = caption;
      OriginalValue = originalValue;
      NewValue = newValue;
    }

    protected virtual void okButton_Click(object sender, EventArgs e) {
      Close();
    }

    private void newValueTextBox_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
        newValueLabel.Select();  // select label to validate data

      if (e.KeyCode == Keys.Escape) {
        newValueTextBox.Text = String.Empty;
        newValueLabel.Select();  // select label to validate data
      }
    }
    private void newValueTextBox_Validating(object sender, CancelEventArgs e) {
      string errorMessage;
      if (!Validate(newValueTextBox.Text, out errorMessage)) {
        e.Cancel = true;
        errorProvider.SetError(newValueTextBox, errorMessage);
        newValueTextBox.SelectAll();
      }
    }
    private void newValueTextBox_Validated(object sender, EventArgs e) {
      errorProvider.SetError(newValueTextBox, string.Empty);
    }

    private static bool Validate(string value, out string errorMessage) {
      double val;
      bool valid = double.TryParse(value, out val);
      errorMessage = string.Empty;
      if (!valid) {
        var sb = new StringBuilder();
        sb.Append("Invalid Value (Valid Value Format: \"");
        sb.Append(FormatPatterns.GetDoubleFormatPattern());
        sb.Append("\")");
        errorMessage = sb.ToString();
      }
      return valid;
    }
  }
}
