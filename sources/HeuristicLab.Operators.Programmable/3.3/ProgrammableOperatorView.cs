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
using HeuristicLab.Operators;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Operators.Programmable {
  [Content(typeof(ProgrammableOperator), true)]
  public partial class ProgrammableOperatorView : ViewBase {
    public ProgrammableOperator ProgrammableOperator {
      get { return (ProgrammableOperator)Item; }
      set { base.Item = value; }
    }

    public ProgrammableOperatorView() {
      InitializeComponent();
    }
    public ProgrammableOperatorView(ProgrammableOperator programmableOperator)
      : this() {
      ProgrammableOperator = programmableOperator;
    }

    protected override void RemoveItemEvents() {
      operatorBaseVariableInfosView.Operator = null;
      operatorBaseVariablesView.Operator = null;
      ProgrammableOperator.CodeChanged -= new EventHandler(ProgrammableOperator_CodeChanged);
      ProgrammableOperator.DescriptionChanged -= new EventHandler(ProgrammableOperator_DescriptionChanged);
      base.RemoveItemEvents();
    }

    protected override void AddItemEvents() {
      base.AddItemEvents();
      operatorBaseVariableInfosView.Operator = ProgrammableOperator;
      operatorBaseVariablesView.Operator = ProgrammableOperator;
      ProgrammableOperator.CodeChanged += new EventHandler(ProgrammableOperator_CodeChanged);
      ProgrammableOperator.DescriptionChanged += new EventHandler(ProgrammableOperator_DescriptionChanged);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      if (ProgrammableOperator == null) {
        codeTextBox.Text = "";
        codeTextBox.Enabled = false;
        addVariableInfoButton.Enabled = false;
        removeVariableInfoButton.Enabled = false;
        descriptionTextBox.Text = "";
        descriptionTextBox.Enabled = false;
      } else {
        codeTextBox.Text = ProgrammableOperator.Code;
        codeTextBox.Enabled = true;
        addVariableInfoButton.Enabled = true;
        removeVariableInfoButton.Enabled = operatorBaseVariableInfosView.SelectedVariableInfos.Count > 0;
        descriptionTextBox.Text = ProgrammableOperator.Description;
        descriptionTextBox.Enabled = true;
      }
    }

    private void operatorBaseVariableInfosView_SelectedVariableInfosChanged(object sender, EventArgs e) {
      removeVariableInfoButton.Enabled = operatorBaseVariableInfosView.SelectedVariableInfos.Count > 0;
    }

    #region Validated Events
    private void codeTextBox_Validated(object sender, EventArgs e) {
      ProgrammableOperator.Code = codeTextBox.Text;
    }
    private void descriptionTextBox_Validated(object sender, EventArgs e) {
      ProgrammableOperator.SetDescription(descriptionTextBox.Text);
    }
    #endregion

    #region Click Events
    private void compileButton_Click(object sender, EventArgs e) {
      try {
        ProgrammableOperator.Compile();
        MessageBox.Show("Compilation successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex) {
        HeuristicLab.Core.Views.Auxiliary.ShowErrorMessageBox(ex);
      }
    }
    private void addVariableInfoButton_Click(object sender, EventArgs e) {
      AddVariableInfoDialog dialog = new AddVariableInfoDialog();
      if (dialog.ShowDialog(this) == DialogResult.OK) {
        if (ProgrammableOperator.GetVariableInfo(dialog.VariableInfo.FormalName) != null)
          HeuristicLab.Core.Views.Auxiliary.ShowErrorMessageBox("A variable info with the same formal name already exists.");
        else
          ProgrammableOperator.AddVariableInfo(dialog.VariableInfo);
      }
      dialog.Dispose();
    }
    private void removeVariableInfoButton_Click(object sender, EventArgs e) {
      IVariableInfo[] selected = new IVariableInfo[operatorBaseVariableInfosView.SelectedVariableInfos.Count];
      operatorBaseVariableInfosView.SelectedVariableInfos.CopyTo(selected, 0);
      for (int i = 0; i < selected.Length; i++)
        ProgrammableOperator.RemoveVariableInfo(selected[i].FormalName);
    }
    #endregion

    #region ProgrammableOperator Events
    private void ProgrammableOperator_CodeChanged(object sender, EventArgs e) {
      codeTextBox.Text = ProgrammableOperator.Code;
    }
    private void ProgrammableOperator_DescriptionChanged(object sender, EventArgs e) {
      descriptionTextBox.Text = ProgrammableOperator.Description;
    }
    #endregion
  }
}
