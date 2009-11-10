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

namespace HeuristicLab.Core {
  /// <summary>
  /// The visual representation of the variables of an operator.
  /// </summary>
  public partial class OperatorBaseVariablesView : ViewBase {
    private ChooseItemDialog chooseItemDialog;

    /// <summary>
    /// Gets or sets the operator whose variables to represent.
    /// </summary>
    /// <remarks>Uses property <see cref="ViewBase.Item"/> of base class <see cref="ViewBase"/>.
    /// No own data storage present.</remarks>
    public IOperator Operator {
      get { return (IOperator)Item; }
      set { base.Item = value; }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="OperatorBaseVariablesView"/> with caption "Operator".
    /// </summary>
    public OperatorBaseVariablesView() {
      InitializeComponent();
      variablesListView.Columns[0].Width = Math.Max(0, variablesListView.Width - 25);
      Caption = "Operator";
    }
    /// <summary>
    /// Initializes a new instance of <see cref="OperatorBaseVariablesView"/> with the given 
    /// operator <paramref name="op"/>.
    /// </summary>
    /// <remarks>Calls <see cref="OperatorBaseVariablesView"/>.</remarks>
    /// <param name="op">The operator whose variables should be represented visually.</param>
    public OperatorBaseVariablesView(IOperator op)
      : this() {
      Operator = op;
    }

    /// <summary>
    /// Removes the eventhandlers from the unterlying <see cref="IOperator"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.RemoveItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void RemoveItemEvents() {
      Operator.VariableAdded -= new EventHandler<EventArgs<IVariable>>(OperatorBase_VariableAdded);
      Operator.VariableRemoved -= new EventHandler<EventArgs<IVariable>>(OperatorBase_VariableRemoved);
      base.RemoveItemEvents();
    }
    /// <summary>
    /// Adds eventhandlers to the underlying <see cref="IOperator"/>.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.AddItemEvents"/> of base class <see cref="ViewBase"/>.</remarks>
    protected override void AddItemEvents() {
      base.AddItemEvents();
      Operator.VariableAdded += new EventHandler<EventArgs<IVariable>>(OperatorBase_VariableAdded);
      Operator.VariableRemoved += new EventHandler<EventArgs<IVariable>>(OperatorBase_VariableRemoved);
    }

    /// <summary>
    /// Updates all controls with the latest data of the model.
    /// </summary>
    /// <remarks>Calls <see cref="ViewBase.UpdateControls"/> of base class <see cref="ViewBase"/>.</remarks>
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
    private void OperatorBase_VariableAdded(object sender, EventArgs<IVariable> e) {
      ListViewItem item = new ListViewItem();
      item.Text = e.Value.Name;
      item.Tag = e.Value;
      variablesListView.Items.Add(item);
      // select the new item
      foreach(ListViewItem oldSelected in variablesListView.SelectedItems) {
        oldSelected.Selected = false;
      }
      item.Selected = true;
      e.Value.NameChanged += new EventHandler(Variable_NameChanged);
    }
    private void OperatorBase_VariableRemoved(object sender, EventArgs<IVariable> e) {
      ListViewItem itemToDelete = null;
      foreach (ListViewItem item in variablesListView.Items) {
        if (item.Tag == e.Value)
          itemToDelete = item;
      }
      e.Value.NameChanged -= new EventHandler(Variable_NameChanged);
      variablesListView.Items.Remove(itemToDelete);
    }
    #endregion
  }
}
