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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Core;
using System.Diagnostics;
using HeuristicLab.Data;
using HeuristicLab.Operators;
using HeuristicLab.Random;

namespace HeuristicLab.StructureIdentification {
  public partial class GPOperatorLibraryEditor : EditorBase {
    public IOperatorLibrary OperatorLibrary {
      get { return (IOperatorLibrary)Item; }
      set { base.Item = value; }
    }

    public GPOperatorLibraryEditor(GPOperatorLibrary library)
      : base() {
      InitializeComponent();
      OperatorLibrary = library;
      operatorLibraryEditor.OperatorLibrary = library;

      library.GPOperatorGroup.OperatorAdded += new EventHandler(GPOperatorLibraryView_OperatorAdded);
      library.GPOperatorGroup.OperatorRemoved += new EventHandler(GPOperatorGroup_OperatorRemoved);

      mutationVariableView.Enabled = false;
      initVariableView.Enabled = false;

      foreach(IOperator op in library.Group.Operators) {
        if(op.GetVariable(GPOperatorLibrary.MANIPULATION) != null) {
          ListViewItem item = new ListViewItem();
          item.Text = op.Name;
          item.Name = op.Name;
          item.Tag = op;
          mutationListView.Items.Add(item);
        }
        if(op.GetVariable(GPOperatorLibrary.INITIALIZATION) != null) {
          ListViewItem item = new ListViewItem();
          item.Name = op.Name;
          item.Text = op.Name;
          item.Tag = op;
          initListView.Items.Add(item);
        }
      }
    }


    private void GPOperatorLibraryView_OperatorAdded(object sender, EventArgs e) {
      IOperator op = ((OperatorEventArgs)e).op;
      if (op.GetVariable(GPOperatorLibrary.MANIPULATION) != null) {
        ListViewItem operatorMutationItem = new ListViewItem();
        operatorMutationItem.Name = op.Name;
        operatorMutationItem.Text = op.Name;
        operatorMutationItem.Tag = op;
        mutationListView.Items.Add(operatorMutationItem);
      }

      if (op.GetVariable(GPOperatorLibrary.INITIALIZATION) != null) {
        ListViewItem operatorInitItem = new ListViewItem();
        operatorInitItem.Name = op.Name;
        operatorInitItem.Text = op.Name;
        operatorInitItem.Tag = op;
        initListView.Items.Add(operatorInitItem);
      }

      op.NameChanged += new EventHandler(op_NameChanged);
      Refresh();
    }

    private void op_NameChanged(object sender, EventArgs e) {
      IOperator srcOp = (IOperator)sender;
      foreach(ListViewItem item in mutationListView.Items) {
        if(item.Tag == srcOp) {
          item.Name = srcOp.Name;
          item.Text = srcOp.Name;
          break;
        }
      }
      foreach(ListViewItem item in initListView.Items) {
        if(item.Tag == srcOp) {
          item.Name = srcOp.Name;
          item.Text = srcOp.Name;
          break;
        }
      }
    }


    private void GPOperatorGroup_OperatorRemoved(object sender, EventArgs e) {
      IOperator op = ((OperatorEventArgs)e).op;

      foreach(ListViewItem item in mutationListView.Items) {
        if(item.Tag == op) {
          mutationListView.Items.Remove(item);
          break;
        }
      }

      foreach(ListViewItem item in initListView.Items) {
        if(item.Tag == op) {
          initListView.Items.Remove(item);
          break;
        }
      }
    }

    private void mutationListView_SelectedIndexChanged(object sender, EventArgs e) {
      if(mutationListView.SelectedItems.Count>0 && mutationListView.SelectedItems[0].Tag != null) {
        IVariable variable = ((IOperator)mutationListView.SelectedItems[0].Tag).GetVariable(GPOperatorLibrary.MANIPULATION);
        mutationVariableView.Enabled = true;
        mutationVariableView.Variable = variable;
      } else {
        mutationVariableView.Enabled = false;
      }
    }

    private void initListView_SelectedIndexChanged(object sender, EventArgs e) {
      if(initListView.SelectedItems.Count>0 && initListView.SelectedItems[0].Tag != null) {
        IVariable variable = ((IOperator)initListView.SelectedItems[0].Tag).GetVariable(GPOperatorLibrary.INITIALIZATION);
        initVariableView.Enabled = true;
        initVariableView.Variable = variable;
      } else {
        initVariableView.Enabled = false;
      }
    }

    private void preprocessButton_Click(object sender, EventArgs e) {
      ((GPOperatorLibrary)OperatorLibrary).Prepare();
    }
  }
}
