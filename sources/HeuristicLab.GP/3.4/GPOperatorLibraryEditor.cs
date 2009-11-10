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
using HeuristicLab.Common;

namespace HeuristicLab.GP {
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

      library.GPOperatorGroup.OperatorAdded += new EventHandler<EventArgs<IOperator>>(GPOperatorLibraryView_OperatorAdded);
      library.GPOperatorGroup.OperatorRemoved += new EventHandler<EventArgs<IOperator>>(GPOperatorGroup_OperatorRemoved);

      mutationVariableView.Enabled = false;
      initVariableView.Enabled = false;

      foreach(IOperator op in library.Group.Operators) {
        if(op.GetVariable(FunctionBase.MANIPULATION) != null) {
          ListViewItem item = new ListViewItem();
          item.Text = op.Name;
          item.Name = op.Name;
          item.Tag = op;
          mutationListView.Items.Add(item);
        }
        if(op.GetVariable(FunctionBase.INITIALIZATION) != null) {
          ListViewItem item = new ListViewItem();
          item.Name = op.Name;
          item.Text = op.Name;
          item.Tag = op;
          initListView.Items.Add(item);
        }
      }
    }


    private void GPOperatorLibraryView_OperatorAdded(object sender, EventArgs<IOperator> e) {
      IOperator op = e.Value;
      if(op.GetVariable(FunctionBase.MANIPULATION) != null) {
        ListViewItem operatorMutationItem = new ListViewItem();
        operatorMutationItem.Name = op.Name;
        operatorMutationItem.Text = op.Name;
        operatorMutationItem.Tag = op;
        mutationListView.Items.Add(operatorMutationItem);
      }

      if(op.GetVariable(FunctionBase.INITIALIZATION) != null) {
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


    private void GPOperatorGroup_OperatorRemoved(object sender, EventArgs<IOperator> e) {
      IOperator op = e.Value;

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
        IVariable variable = ((IOperator)mutationListView.SelectedItems[0].Tag).GetVariable(FunctionBase.MANIPULATION);
        mutationVariableView.Enabled = true;
        mutationVariableView.Variable = variable;
      } else {
        mutationVariableView.Enabled = false;
      }
    }

    private void initListView_SelectedIndexChanged(object sender, EventArgs e) {
      if(initListView.SelectedItems.Count>0 && initListView.SelectedItems[0].Tag != null) {
        IVariable variable = ((IOperator)initListView.SelectedItems[0].Tag).GetVariable(FunctionBase.INITIALIZATION);
        initVariableView.Enabled = true;
        initVariableView.Variable = variable;
      } else {
        initVariableView.Enabled = false;
      }
    }
  }
}
