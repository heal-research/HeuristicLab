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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HeuristicLab.Core {
  public partial class VariablesScopeView : ViewBase {
    private ChooseItemDialog chooseItemDialog;

    public IScope Scope {
      get { return (IScope)Item; }
      set { base.Item = value; }
    }

    public VariablesScopeView() {
      InitializeComponent();
      Caption = "Variables Scope View";
    }
    public VariablesScopeView(IScope scope)
      : this() {
      Scope = scope;
    }

    protected override void RemoveItemEvents() {
      Scope.VariableAdded -= new EventHandler<VariableEventArgs>(Scope_VariableAdded);
      Scope.VariableRemoved -= new EventHandler<VariableEventArgs>(Scope_VariableRemoved);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      Scope.VariableAdded += new EventHandler<VariableEventArgs>(Scope_VariableAdded);
      Scope.VariableRemoved += new EventHandler<VariableEventArgs>(Scope_VariableRemoved);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      detailsGroupBox.Controls.Clear();
      detailsGroupBox.Enabled = false;
      removeButton.Enabled = false;
      if (Scope == null) {
        Caption = "Variables Scope View";
        variablesListView.Enabled = false;
      } else {
        Caption = "Variables Scope View of " + Scope.Name + " (" + Scope.GetType().Name + ")";
        variablesListView.Enabled = true;
        foreach (ListViewItem item in variablesListView.Items) {
          ((IVariable)item.Tag).NameChanged -= new EventHandler(Variable_NameChanged);
        }
        variablesListView.Items.Clear();
        foreach (IVariable variable in Scope.Variables) {
          ListViewItem item = new ListViewItem();
          item.Text = variable.Name;
          item.Tag = variable;
          variablesListView.Items.Add(item);
          variable.NameChanged += new EventHandler(Variable_NameChanged);
        }
      }
    }

    private void variablesListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (detailsGroupBox.Controls.Count > 0)
        detailsGroupBox.Controls[0].Dispose();
      detailsGroupBox.Controls.Clear();
      detailsGroupBox.Enabled = false;
      removeButton.Enabled = false;
      if (variablesListView.SelectedItems.Count > 0) {
        removeButton.Enabled = true;
      }
      if (variablesListView.SelectedItems.Count == 1) {
        IVariable variable = (IVariable)variablesListView.SelectedItems[0].Tag;
        Control control = (Control)new VariableView(variable);
        detailsGroupBox.Controls.Add(control);
        control.Dock = DockStyle.Fill;
        detailsGroupBox.Enabled = true;
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
            Scope.RemoveVariable(((IVariable)item.Tag).Name);
        }
      }
    }
    #endregion

    #region Button Events
    private void addButton_Click(object sender, EventArgs e) {
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
          foreach (IVariable existingVariable in Scope.Variables) {
            if (existingVariable.Name == name) {
              valid = false;
              index++;
            }
          }
        } while (!valid);
        newVariable.Name = name;
        Scope.AddVariable(newVariable);
      }
    }
    private void removeButton_Click(object sender, EventArgs e) {
      if (variablesListView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in variablesListView.SelectedItems)
          Scope.RemoveVariable(((IVariable)item.Tag).Name);
      }
    }
    #endregion

    #region Scope Events
    private delegate void OnVariableEventDelegate(object sender, VariableEventArgs e);
    private void Scope_VariableAdded(object sender, VariableEventArgs e) {
      if (InvokeRequired)
        Invoke(new OnVariableEventDelegate(Scope_VariableAdded), sender, e);
      else {
        ListViewItem item = new ListViewItem();
        item.Text = e.Variable.Name;
        item.Tag = e.Variable;
        variablesListView.Items.Add(item);
        e.Variable.NameChanged += new EventHandler(Variable_NameChanged);
      }
    }
    private void Scope_VariableRemoved(object sender, VariableEventArgs e) {
      if (InvokeRequired)
        Invoke(new OnVariableEventDelegate(Scope_VariableRemoved), sender, e);
      else {
        ListViewItem itemToDelete = null;
        foreach (ListViewItem item in variablesListView.Items) {
          if (item.Tag == e.Variable)
            itemToDelete = item;
        }
        e.Variable.NameChanged -= new EventHandler(Variable_NameChanged);
        variablesListView.Items.Remove(itemToDelete);
      }
    }
    #endregion

    #region Variable Events
    private delegate void OnEventDelegate(object sender, EventArgs e);
    private void Variable_NameChanged(object sender, EventArgs e) {
      if (InvokeRequired)
        Invoke(new OnEventDelegate(Variable_NameChanged), sender, e);
      else {
        IVariable variable = (IVariable)sender;
        foreach (ListViewItem item in variablesListView.Items) {
          if (item.Tag == variable)
            item.Text = variable.Name;
        }
      }
    }
    #endregion
  }
}

