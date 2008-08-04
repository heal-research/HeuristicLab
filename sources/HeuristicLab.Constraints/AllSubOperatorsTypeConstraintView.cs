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

namespace HeuristicLab.Constraints {
  public partial class AllSubOperatorsTypeConstraintView : ViewBase {
    private SubOperatorTypeConstraint constraint = new SubOperatorTypeConstraint();

    public SubOperatorTypeConstraint Constraint {
      get { return constraint; }
      set { 
        constraint = value;
        UpdateAllowedOperatorsList();
      }
    }

    public AllSubOperatorsTypeConstraintView() {
      InitializeComponent();
    }

    public AllSubOperatorsTypeConstraintView(SubOperatorTypeConstraint constraint) {
      this.constraint = constraint;
      InitializeComponent();
    }

    private void UpdateAllowedOperatorsList() {
      listView.Clear();
      foreach(IOperator op in constraint.AllowedSubOperators) {
        ListViewItem item = new ListViewItem();
        item.Name = op.Name;
        item.Text = op.Name;
        item.Tag = op;
        listView.Items.Add(item);
      }
      Refresh();
    }

    private void op_NameChanged(object sender, EventArgs e) {
      IOperator srcOp = (IOperator)sender;

      foreach(ListViewItem item in listView.Items) {
        if(item.Tag == srcOp) {
          item.Text = srcOp.Name;
          item.Name = srcOp.Name;
          break;
        }
      }
      Refresh();
    }

    #region Key Events
    private void listView_KeyDown(object sender, KeyEventArgs e) {
      if(e.KeyCode == Keys.Delete) {
        while(listView.SelectedIndices.Count > 0) {
          constraint.RemoveOperator((IOperator)listView.SelectedItems[0].Tag);
          listView.Items.RemoveAt(listView.SelectedIndices[0]);
        }
      }
    }
    #endregion

    #region Drag Events
    private void listView_DragDrop(object sender, DragEventArgs e) {
      if(e.Effect != DragDropEffects.None) {
        if(e.Data.GetDataPresent("IOperator")) {
          IOperator op = (IOperator)e.Data.GetData("IOperator");
          ListViewItem item = new ListViewItem();
          item.Tag = op;
          item.Name = op.Name;
          item.Text = op.Name;
          listView.Items.Add(item);
          constraint.AddOperator(op);
          op.NameChanged += new EventHandler(op_NameChanged);
          listView.SelectedIndices.Clear();
        }
      }
    }

    private void listView_DragEnter(object sender, DragEventArgs e) {
      this.FindForm().BringToFront();
      this.Focus();
      e.Effect = DragDropEffects.None;
      if(e.Data.GetDataPresent("IOperator")) {
        IOperator op = (IOperator)e.Data.GetData("IOperator");
        if(!ListContains(op)) {
          e.Effect = DragDropEffects.Copy;
        }
      }
    }

    private void listView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if(e.Data.GetDataPresent("IOperator")) {
        IOperator op = (IOperator)e.Data.GetData("IOperator");
        if(!ListContains(op)) {
          e.Effect = DragDropEffects.Copy;
        }
      }
    }
    #endregion

    #region button events
    private void removeButton_Click(object sender, EventArgs e) {
      while(listView.SelectedIndices.Count > 0) {
        constraint.RemoveOperator((IOperator)listView.SelectedItems[0].Tag);
        listView.Items.RemoveAt(listView.SelectedIndices[0]);
      }
    }
    #endregion

    private bool ListContains(IOperator op) {
      foreach(ListViewItem item in listView.Items) {
        if(item.Tag == op) return true;
      }
      return false;
    }

    private void listView_SelectedIndexChanged(object sender, EventArgs e) {
      if(listView.SelectedItems.Count > 0) {
        removeButton.Enabled = true;
      } else {
        removeButton.Enabled = false;
      }
    }
  }
}
