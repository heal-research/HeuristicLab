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
  public partial class OperatorBaseVariablesView : ViewBase {
    private ChooseItemDialog chooseItemDialog;

    public IOperator Operator {
      get { return (IOperator)Item; }
      set { base.Item = value; }
    }

    public OperatorBaseVariablesView() {
      InitializeComponent();
      variablesListView.Columns[0].Width = Math.Max(0, variablesListView.Width - 25);
      Caption = "Operator";
    }
    public OperatorBaseVariablesView(IOperator op)
      : this() {
      Operator = op;
    }

    protected override void RemoveItemEvents() {
      Operator.VariableAdded -= new EventHandler<VariableEventArgs>(OperatorBase_VariableAdded);
      Operator.VariableRemoved -= new EventHandler<VariableEventArgs>(OperatorBase_VariableRemoved);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      Operator.VariableAdded += new EventHandler<VariableEventArgs>(OperatorBase_VariableAdded);
      Operator.VariableRemoved += new EventHandler<VariableEventArgs>(OperatorBase_VariableRemoved);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      variableDetailsGroupBox.Controls.Clear();
      if (Operator == null) {
        Caption = "Operator";
        variablesListView.Enabled = false;
        variableDetailsGroupBox.Enabled = false;
        removeVariableButton.Enabled = false;
      } else {
        Caption = Operator.Name + " (" + Operator.GetType().Name + ")";
        variablesListView.Enabled = true;
        foreach (ListViewItem item in variablesListView.Items) {
          ((IVariable)item.Tag).NameChanged -= new EventHandler(Variable_NameChanged);
        }
        variablesListView.Items.Clear();
        foreach (IVariable variable in Operator.Variables) {
          ListViewItem item = new ListViewItem();
          item.Text = variable.Name;
          item.Tag = variable;
          variablesListView.Items.Add(item);
          variable.NameChanged += new EventHandler(Variable_NameChanged);
        }
        if (variablesListView.Items.Count > 0)
          variablesListView.SelectedIndices.Add(0);
      }
    }

    private void variablesListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (variableDetailsGroupBox.Controls.Count > 0)
        variableDetailsGroupBox.Controls[0].Dispose();
      variableDetailsGroupBox.Controls.Clear();
      variableDetailsGroupBox.Enabled = false;
      removeVariableButton.Enabled = false;
      if (variablesListView.SelectedItems.Count > 0) {
        removeVariableButton.Enabled = true;
      }
      if (variablesListView.SelectedItems.Count == 1) {
        IVariable variable = (IVariable)variablesListView.SelectedItems[0].Tag;
        Control editor = (Control)variable.CreateView();
        variableDetailsGroupBox.Controls.Add(editor);
        editor.Dock = DockStyle.Fill;
        variableDetailsGroupBox.Enabled = true;
      }
    }

    #region Size Changed Events
    private void variablesListView_SizeChanged(object sender, EventArgs e) {
      if (variablesListView.Columns.Count > 0)
        variablesListView.Columns[0].Width = Math.Max(0, variablesListView.Width - 25);
    }
    #endregion

    #region Key Events
    private void variablesListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        if (variablesListView.SelectedItems.Count > 0) {
          foreach (ListViewItem item in variablesListView.SelectedItems)
            Operator.RemoveVariable(((IVariable)item.Tag).Name);
        }
      }
    }
    #endregion

    #region Button Events
    private void addVariableButton_Click(object sender, EventArgs e) {
      if (chooseItemDialog == null) {
        chooseItemDialog = new ChooseItemDialog();
        chooseItemDialog.Caption = "Add Variable";
      }
      if (chooseItemDialog.ShowDialog(this) == DialogResult.OK) {
        IVariable newVariable = new Variable(chooseItemDialog.Item.GetType().Name, chooseItemDialog.Item);
        int index = 1;
        bool valid = true;
        string name = null;
        do {
          valid = true;
          name = newVariable.Name + " (" + index.ToString() + ")";
          foreach (IVariable existingVariable in Operator.Variables) {
            if (existingVariable.Name == name) {
              valid = false;
              index++;
            }
          }
        } while (!valid);
        newVariable.Name = name;
        Operator.AddVariable(newVariable);
      }
    }
    private void removeVariableButton_Click(object sender, EventArgs e) {
      if (variablesListView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in variablesListView.SelectedItems)
          Operator.RemoveVariable(((IVariable)item.Tag).Name);
      }
    }
    #endregion

    #region Variable Events
    private void Variable_NameChanged(object sender, EventArgs e) {
      IVariable variable = (IVariable)sender;
      foreach (ListViewItem item in variablesListView.Items) {
        if (item.Tag == variable)
          item.Text = variable.Name;
      }
    }
    #endregion

    #region OperatorBase Events
    private void OperatorBase_VariableAdded(object sender, VariableEventArgs e) {
      ListViewItem item = new ListViewItem();
      item.Text = e.Variable.Name;
      item.Tag = e.Variable;
      variablesListView.Items.Add(item);
      // select the new item
      foreach(ListViewItem oldSelected in variablesListView.SelectedItems) {
        oldSelected.Selected = false;
      }
      item.Selected = true;
      e.Variable.NameChanged += new EventHandler(Variable_NameChanged);
    }
    private void OperatorBase_VariableRemoved(object sender, VariableEventArgs e) {
      ListViewItem itemToDelete = null;
      foreach (ListViewItem item in variablesListView.Items) {
        if (item.Tag == e.Variable)
          itemToDelete = item;
      }
      e.Variable.NameChanged -= new EventHandler(Variable_NameChanged);
      variablesListView.Items.Remove(itemToDelete);
    }
    #endregion
  }
}
