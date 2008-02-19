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

namespace HeuristicLab.Core {
  public partial class VariableView : ViewBase {
    private ChooseItemDialog chooseItemDialog;

    public IVariable Variable {
      get { return (IVariable)Item; }
      set { base.Item = value; }
    }

    public VariableView() {
      InitializeComponent();
      Caption = "Variable";
    }
    public VariableView(IVariable variable)
      : this() {
      Variable = variable;
    }

    protected override void RemoveItemEvents() {
      Variable.NameChanged -= new EventHandler(Variable_NameChanged);
      Variable.ValueChanged -= new EventHandler(Variable_ValueChanged);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      Variable.NameChanged += new EventHandler(Variable_NameChanged);
      Variable.ValueChanged += new EventHandler(Variable_ValueChanged);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      valueTextBox.Text = "-";
      valueTextBox.Enabled = false;
      editorGroupBox.Controls.Clear();
      editorGroupBox.Enabled = false;
      if (Variable == null) {
        Caption = "Variable";
        nameTextBox.Enabled = false;
      } else {
        Caption = Variable.Name + " (" + Variable.GetType().Name + ")";
        nameTextBox.Text = Variable.Name;
        nameTextBox.Enabled = true;
        if (Variable.Value != null) {
          valueTextBox.Text = Variable.Value.GetType().FullName;
          valueTextBox.Enabled = true;
          Control editor = (Control)Variable.Value.CreateView();
          if (editor != null) {
            editorGroupBox.Controls.Add(editor);
            editor.Dock = DockStyle.Fill;
            editorGroupBox.Enabled = true;
          }
        }
      }
    }

    private void Variable_NameChanged(object sender, EventArgs e) {
      UpdateText();
    }
    private void UpdateText() {
      if (InvokeRequired)
        Invoke(new MethodInvoker(UpdateText));
      else
        nameTextBox.Text = Variable.Name;
    }
    private void Variable_ValueChanged(object sender, EventArgs e) {
      Refresh();
    }

    private void nameTextBox_Validating(object sender, CancelEventArgs e) {
      Variable.Name = nameTextBox.Text;

      // check if variable name was set successfully
      e.Cancel = Variable.Name != nameTextBox.Text;
    }

    private void setVariableValueButton_Click(object sender, EventArgs e) {
      if (chooseItemDialog == null) {
        chooseItemDialog = new ChooseItemDialog();
        chooseItemDialog.Caption = "Set Value";
      }
      if (chooseItemDialog.ShowDialog(this) == DialogResult.OK)
        Variable.Value = chooseItemDialog.Item;
    }
  }
}
