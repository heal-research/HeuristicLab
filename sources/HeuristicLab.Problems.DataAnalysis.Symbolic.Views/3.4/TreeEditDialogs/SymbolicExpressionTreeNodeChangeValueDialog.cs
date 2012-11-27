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
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  public partial class ValueChangeDialog : Form {
    private ISymbolicExpressionTreeNode content;
    public ISymbolicExpressionTreeNode Content {
      get { return content; }
      set {
        if (InvokeRequired)
          Invoke(new Action<SymbolicExpressionTreeNode>(x => content = x), value);
        else
          content = value;
      }
    }

    public ValueChangeDialog() {
      InitializeComponent();
      oldValueTextBox.TabStop = false; // cannot receive focus using tab key
    }

    public void SetContent(ISymbolicExpressionTreeNode content) {
      Content = content;
      if (Content is VariableTreeNode) {
        this.Text = "Change variable name or weight";
        var variable = Content as VariableTreeNode;
        newValueTextBox.Text = oldValueTextBox.Text = Math.Round(variable.Weight, 4).ToString();
        // add a dropbox containing all the available variable names
        variableNameLabel.Visible = true;
        variableNamesCombo.Visible = true;
        foreach (var name in variable.Symbol.VariableNames) variableNamesCombo.Items.Add(name);
        variableNamesCombo.SelectedIndex = variableNamesCombo.Items.IndexOf(variable.VariableName);
      } else if (Content is ConstantTreeNode) {
        this.Text = "Change constant value";
        var constant = Content as ConstantTreeNode;
        newValueTextBox.Text = oldValueTextBox.Text = Math.Round(constant.Value, 4).ToString();
      }
    }

    #region text box validation and events
    private void newValueTextBox_Validating(object sender, CancelEventArgs e) {
      string errorMessage;
      if (!ValidateNewValue(newValueTextBox.Text, out errorMessage)) {
        e.Cancel = true;
        errorProvider.SetError(newValueTextBox, errorMessage);
        newValueTextBox.SelectAll();
      }
    }

    private void newValueTextBox_Validated(object sender, EventArgs e) {
      errorProvider.SetError(newValueTextBox, string.Empty);
    }

    private static bool ValidateNewValue(string value, out string errorMessage) {
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
    #endregion

    #region combo box validation and events
    private void variableNamesCombo_Validating(object sender, CancelEventArgs e) {
      if (!(Content is VariableTreeNode)) return;
      if (variableNamesCombo.Items.Contains(variableNamesCombo.SelectedItem)) return;
      e.Cancel = true;
      errorProvider.SetError(variableNamesCombo, "Invalid variable name");
      variableNamesCombo.SelectAll();
    }

    private void variableNamesCombo_Validated(object sender, EventArgs e) {
      errorProvider.SetError(variableNamesCombo, String.Empty);
    }
    #endregion
    // proxy handler passing key strokes to the parent control
    private void childControl_KeyDown(object sender, KeyEventArgs e) {
      ValueChangeDialog_KeyDown(sender, e);
    }

    private void ValueChangeDialog_KeyDown(object sender, KeyEventArgs e) {
      if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return)) {
        if (!ValidateChildren()) return;
        OnDialogValidated(this, e); // emit validated effect
        Close();
      }
    }

    public event EventHandler DialogValidated;
    private void OnDialogValidated(object sender, EventArgs e) {
      var dialogValidated = DialogValidated;
      if (dialogValidated != null)
        dialogValidated(sender, e);
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      Close();
    }

    private void okButton_Click(object sender, EventArgs e) {
      if (ValidateChildren()) {
        OnDialogValidated(this, e);
        Close();
      }
    }

  }
}
