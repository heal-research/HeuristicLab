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
  public partial class ConstrainedItemBaseView : ViewBase {
    private ChooseItemDialog chooseItemDialog;

    public IConstrainedItem ConstrainedItem {
      get { return (IConstrainedItem)Item; }
      set { base.Item = value; }
    }

    public ConstrainedItemBaseView() {
      InitializeComponent();
      constraintsListView.Columns[0].Width = Math.Max(0, constraintsListView.Width - 25);
      Caption = "Constrained Item";
    }
    public ConstrainedItemBaseView(IConstrainedItem constraintItem)
      : this() {
      ConstrainedItem = constraintItem;
    }

    protected override void RemoveItemEvents() {
      ConstrainedItem.ConstraintAdded -= new EventHandler<ConstraintEventArgs>(ConstrainedItemBase_ConstraintAdded);
      ConstrainedItem.ConstraintRemoved -= new EventHandler<ConstraintEventArgs>(ConstrainedItemBase_ConstraintRemoved);
      base.RemoveItemEvents();
    }
    protected override void AddItemEvents() {
      base.AddItemEvents();
      ConstrainedItem.ConstraintAdded += new EventHandler<ConstraintEventArgs>(ConstrainedItemBase_ConstraintAdded);
      ConstrainedItem.ConstraintRemoved += new EventHandler<ConstraintEventArgs>(ConstrainedItemBase_ConstraintRemoved);
    }

    protected override void UpdateControls() {
      base.UpdateControls();
      constraintDetailsGroupBox.Controls.Clear();
      if (ConstrainedItem == null) {
        Caption = "Constrained Item";
        constraintsListView.Enabled = false;
        constraintDetailsGroupBox.Enabled = false;
        removeConstraintButton.Enabled = false;
      } else {
        Caption = "Constrained Item (" + ConstrainedItem.GetType().Name + ")";
        constraintsListView.Enabled = true;
        constraintsListView.Items.Clear();
        foreach (IConstraint constraint in ConstrainedItem.Constraints) {
          ListViewItem item = new ListViewItem();
          item.Text = constraint.GetType().Name;
          item.Tag = constraint;
          constraintsListView.Items.Add(item);
        }
        if (constraintsListView.Items.Count > 0)
          constraintsListView.SelectedIndices.Add(0);
      }
    }

    private void constraintsListView_SelectedIndexChanged(object sender, EventArgs e) {
      if (constraintDetailsGroupBox.Controls.Count > 0)
        constraintDetailsGroupBox.Controls[0].Dispose();
      constraintDetailsGroupBox.Controls.Clear();
      constraintDetailsGroupBox.Enabled = false;
      removeConstraintButton.Enabled = false;
      if (constraintsListView.SelectedItems.Count > 0) {
        removeConstraintButton.Enabled = true;
      }
      if (constraintsListView.SelectedItems.Count == 1) {
        IConstraint constraint = (IConstraint)constraintsListView.SelectedItems[0].Tag;
        Control view = (Control)constraint.CreateView();
        if (view != null) {
          constraintDetailsGroupBox.Controls.Add(view);
          view.Dock = DockStyle.Fill;
          constraintDetailsGroupBox.Enabled = true;
        }
      }
    }

    #region Size Changed Events
    private void constraintsListView_SizeChanged(object sender, EventArgs e) {
      if (constraintsListView.Columns.Count > 0)
        constraintsListView.Columns[0].Width = Math.Max(0, constraintsListView.Width - 25);
    }
    #endregion

    #region Key Events
    private void constraintsListView_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Delete) {
        if (constraintsListView.SelectedItems.Count > 0) {
          foreach (ListViewItem item in constraintsListView.SelectedItems)
            ConstrainedItem.RemoveConstraint((IConstraint)item.Tag);
        }
      }
    }
    #endregion

    #region Button Events
    private void addConstraintButton_Click(object sender, EventArgs e) {
      if (chooseItemDialog == null) {
        chooseItemDialog = new ChooseItemDialog(typeof(IConstraint));
        chooseItemDialog.Caption = "Add Constraint";
      }
      if (chooseItemDialog.ShowDialog(this) == DialogResult.OK)
        ConstrainedItem.AddConstraint((IConstraint)chooseItemDialog.Item);
    }
    private void removeConstraintButton_Click(object sender, EventArgs e) {
      if (constraintsListView.SelectedItems.Count > 0) {
        foreach (ListViewItem item in constraintsListView.SelectedItems)
          ConstrainedItem.RemoveConstraint((IConstraint)item.Tag);
      }
    }
    #endregion

    #region ConstrainedItemBase Events
    private void ConstrainedItemBase_ConstraintAdded(object sender, ConstraintEventArgs e) {
      ListViewItem item = new ListViewItem();
      item.Text = e.Constraint.GetType().Name;
      item.Tag = e.Constraint;
      constraintsListView.Items.Add(item);
    }
    private void ConstrainedItemBase_ConstraintRemoved(object sender, ConstraintEventArgs e) {
      ListViewItem itemToDelete = null;
      foreach (ListViewItem item in constraintsListView.Items) {
        if (item.Tag == e.Constraint)
          itemToDelete = item;
      }
      constraintsListView.Items.Remove(itemToDelete);
    }
    #endregion
  }
}
