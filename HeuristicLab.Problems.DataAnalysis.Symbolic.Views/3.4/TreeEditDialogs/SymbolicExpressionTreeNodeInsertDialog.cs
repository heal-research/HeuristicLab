#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Data;
using HeuristicLab.Encodings.SymbolicExpressionTreeEncoding;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  public partial class InsertNodeDialog : Form {
    public InsertNodeDialog() {
      InitializeComponent();
    }

    public void SetAllowedSymbols(IEnumerable<ISymbol> symbols) {
      allowedSymbolsCombo.Items.Clear();
      foreach (var s in symbols) allowedSymbolsCombo.Items.Add(s);
      allowedSymbolsCombo.SelectedIndex = 0;
    }

    public ISymbol SelectedSymbol() {
      return (ISymbol)allowedSymbolsCombo.SelectedItem;
    }

    private void allowedSymbolsCombo_SelectedIndexChanged(object sender, EventArgs e) {
      var combo = (ComboBox)sender;
      var symbol = combo.Items[combo.SelectedIndex];
      if (symbol is Constant) {
        // add controls to the dialog for changing the constant value
        variableNameLabel.Visible = false;
        variableNamesCombo.Visible = false;
        variableWeightLabel.Visible = false;
        variableWeightTextBox.Visible = false;
        constantValueLabel.Visible = true;
        constantValueTextBox.Visible = true;
      } else if (symbol is Variable) {
        var variable = (Variable)symbol;
        foreach (var name in variable.VariableNames) variableNamesCombo.Items.Add(name);
        variableNamesCombo.SelectedIndex = 0;
        variableNameLabel.Visible = true;
        variableNamesCombo.Visible = true;
        variableWeightLabel.Visible = true;
        variableWeightTextBox.Visible = true;
        constantValueLabel.Visible = false;
        constantValueTextBox.Visible = false;
        // add controls to the dialog for changing the variable name or weight
      } else {
        variableNameLabel.Visible = false;
        variableNamesCombo.Visible = false;
        variableWeightLabel.Visible = false;
        variableWeightTextBox.Visible = false;
        constantValueLabel.Visible = false;
        constantValueTextBox.Visible = false;
      }
    }

    // validation
    private void variableWeightTextBox_Validating(object sender, CancelEventArgs e) {
      string errorMessage;
      if (ValidateDoubleValue(variableWeightTextBox.Text, out errorMessage)) return;
      e.Cancel = true;
      errorProvider.SetError(variableWeightTextBox, errorMessage);
      variableWeightTextBox.SelectAll();
    }

    private void constantValueTextBox_Validating(object sender, CancelEventArgs e) {
      string errorMessage;
      if (ValidateDoubleValue(constantValueTextBox.Text, out errorMessage)) return;
      e.Cancel = true;
      errorProvider.SetError(constantValueTextBox, errorMessage);
      constantValueTextBox.SelectAll();
    }

    private static bool ValidateDoubleValue(string value, out string errorMessage) {
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

    public event EventHandler DialogValidated;
    private void OnDialogValidated(object sender, EventArgs e) {
      DialogResult = DialogResult.OK;
      var dialogValidated = DialogValidated;
      if (dialogValidated != null)
        dialogValidated(sender, e);
    }

    private void childControl_KeyDown(object sender, KeyEventArgs e) {
      insertNodeDialog_KeyDown(sender, e);
    }

    private void insertNodeDialog_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return) {
        if (ValidateChildren()) {
          OnDialogValidated(this, e);
          Close();
        }
      }
    }

    private void okButton_Click(object sender, EventArgs e) {
      if (ValidateChildren()) {
        OnDialogValidated(this, e);
        Close();
      }
    }

    private void cancelButton_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
