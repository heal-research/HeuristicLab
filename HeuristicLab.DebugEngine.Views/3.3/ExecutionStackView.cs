#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.DebugEngine {

  [View("Execution Stack View")]
  [Content(typeof(ExecutionStack), IsDefaultView = true)]
  public partial class ExecutionStackView : AsynchronousContentView {
    public new ExecutionStack Content {
      get { return (ExecutionStack)base.Content; }
      set { base.Content = value; }
    }

    public ExecutionStackView() {
      InitializeComponent();
    }

    protected override void DeregisterContentEvents() {
      Content.CollectionReset -= Content_Changed;
      Content.ItemsAdded -= Content_Changed;
      Content.ItemsRemoved -= Content_Changed;
      Content.ItemsMoved -= Content_Changed;
      Content.ItemsReplaced -= Content_Changed;
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CollectionReset += Content_Changed;
      Content.ItemsAdded += Content_Changed;
      Content.ItemsRemoved += Content_Changed;
      Content.ItemsMoved += Content_Changed;
      Content.ItemsReplaced += Content_Changed;
    }

    #region Event Handlers (Content)
    void Content_Changed(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOperation>> e) {
      if (InvokeRequired)
        Invoke(new EventHandler<CollectionItemsChangedEventArgs<IndexedItem<IOperation>>>(Content_Changed), sender, e);
      else
        UpdateExecutionStack();
    }


    private void UpdateExecutionStack() {
      treeView.BeginUpdate();
      treeView.Nodes.Clear();
      treeView.ImageList.Images.Clear();
      treeView.ImageList.Images.Add(VSImageLibrary.Method);
      treeView.ImageList.Images.Add(VSImageLibrary.Module);
      treeView.ImageList.Images.Add(VSImageLibrary.BreakpointActive);
      int totalNodes = AddStackOperations(treeView.Nodes, ((IEnumerable<IOperation>)Content).Reverse());
      if (treeView.Nodes.Count > 0)
        treeView.TopNode = treeView.Nodes[0];
      treeView.ExpandAll();
      treeView.EndUpdate();
      groupBox.Text = string.Format("Execution Stack ({0})", totalNodes);
    }

    private int AddStackOperations(TreeNodeCollection nodes, IEnumerable<IOperation> operations) {
      int count = 0;
      foreach (IOperation op in operations) {
        if (op is IAtomicOperation) {
          IAtomicOperation atom = op as IAtomicOperation;
          TreeNode node = nodes.Add(atom.Operator.Name ?? atom.Operator.ItemName);
          node.Tag = atom;
          node.ToolTipText = string.Format("{0}{1}{1}{2}",
            Utils.TypeName(atom.Operator), Environment.NewLine,
            Utils.Wrap(atom.Operator.Description ?? atom.Operator.ItemDescription, 60));
          if (atom.Operator.Breakpoint) {
            node.ForeColor = Color.Red;
            node.ImageIndex = 2;
            node.SelectedImageIndex = 2;
          } else {
            node.ImageIndex = 0;
            node.SelectedImageIndex = 0;
          }
          count++;
        } else if (op is OperationCollection) {
          OperationCollection ops = op as OperationCollection;
          TreeNode node = nodes.Add(
            string.Format("{0} {2}Operation{1}",
            ops.Count,
            ops.Count == 1 ? string.Empty : "s",
            ops.Parallel ? "Parallel " : string.Empty
            ));
          node.Tag = op;
          node.ToolTipText = Utils.TypeName(ops);
          node.ImageIndex = 1;
          node.SelectedImageIndex = 1;
          count += AddStackOperations(node.Nodes, ops);
        }
      }
      return count;
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        treeView.Nodes.Clear();
      } else {
        UpdateExecutionStack();
      }
    }


    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    #region Event Handlers (child controls)
    private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
      if (e.Node != null) {
        IAtomicOperation op = e.Node.Tag as IAtomicOperation;
        if (op != null)
          MainFormManager.MainForm.ShowContent(op.Operator);
      }
    }
    #endregion
  }
}
