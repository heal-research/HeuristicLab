#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Collections;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  public sealed partial class ExperimentTreeView : ItemView {
    private TypeSelectorDialog typeSelectorDialog;
    private Dictionary<IOptimizer, List<TreeNode>> optimizerTreeViewMapping;

    public ExperimentTreeView() {
      InitializeComponent();
      optimizerTreeViewMapping = new Dictionary<IOptimizer, List<TreeNode>>();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    public new Experiment Content {
      get { return (Experiment)base.Content; }
      set { base.Content = value; }
    }

    #region events registration
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.ExecutionStateChanged += new EventHandler(Content_ExecutionStateChanged);
      Content.Optimizers.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsAdded);
      Content.Optimizers.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsMoved);
      Content.Optimizers.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsRemoved);
      Content.Optimizers.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsReplaced);
      Content.Optimizers.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_CollectionReset);
    }

    protected override void DeregisterContentEvents() {
      Content.ExecutionStateChanged -= new EventHandler(Content_ExecutionStateChanged);
      Content.Optimizers.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsAdded);
      Content.Optimizers.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsMoved);
      Content.Optimizers.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsRemoved);
      Content.Optimizers.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsReplaced);
      Content.Optimizers.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_CollectionReset);
      foreach (var optimizer in optimizerTreeViewMapping.Keys)
        DeregisterOptimizerEvents(optimizer);
      base.DeregisterContentEvents();
    }

    private void RegisterOptimizerEvents(IOptimizer optimizer) {
      optimizer.ToStringChanged += new EventHandler(optimizer_ToStringChanged);
      optimizer.ExecutionStateChanged += new EventHandler(optimizer_ExecutionStateChanged);

      var batchRun = optimizer as BatchRun;
      var experiment = optimizer as Experiment;
      if (batchRun != null) {
        batchRun.OptimizerChanged += new EventHandler(batchRun_OptimizerChanged);
      }
      if (experiment != null) {
        experiment.Optimizers.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsAdded);
        experiment.Optimizers.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsMoved);
        experiment.Optimizers.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsRemoved);
        experiment.Optimizers.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsReplaced);
        experiment.Optimizers.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_CollectionReset);
      }
    }

    private void DeregisterOptimizerEvents(IOptimizer optimizer) {
      optimizer.ToStringChanged -= new EventHandler(optimizer_ToStringChanged);
      optimizer.ExecutionStateChanged -= new EventHandler(optimizer_ExecutionStateChanged);

      var batchRun = optimizer as BatchRun;
      var experiment = optimizer as Experiment;
      if (batchRun != null) {
        batchRun.OptimizerChanged -= new EventHandler(batchRun_OptimizerChanged);
      }
      if (experiment != null) {
        experiment.Optimizers.ItemsAdded -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsAdded);
        experiment.Optimizers.ItemsMoved -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsMoved);
        experiment.Optimizers.ItemsRemoved -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsRemoved);
        experiment.Optimizers.ItemsReplaced -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsReplaced);
        experiment.Optimizers.CollectionReset -= new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_CollectionReset);
      }
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      if (Content == null) {
        optimizerTreeView.Nodes.Clear();
      } else {
        UpdateOptimizerTreeView();
        optimizerTreeView.ExpandAll();
      }
    }

    #region content events
    private void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((Action<object, EventArgs>)Content_ExecutionStateChanged, sender, e);
        return;
      }
      RebuildImageList();
      SetEnabledStateOfControls();
    }

    private void optimizer_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((Action<object, EventArgs>)optimizer_ExecutionStateChanged, sender, e);
        return;
      }
      RebuildImageList();
    }

    private void batchRun_OptimizerChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((Action<object, EventArgs>)batchRun_OptimizerChanged, sender, e);
        return;
      }
      var batchRun = (BatchRun)sender;
      foreach (TreeNode node in optimizerTreeViewMapping[batchRun]) {
        foreach (TreeNode childNode in node.Nodes) {
          DisposeTreeNode(childNode);
          childNode.Remove();
        }

        if (batchRun.Optimizer != null) {
          TreeNode childNode = CreateTreeNode(batchRun.Optimizer);
          UpdateChildTreeNodes(childNode.Nodes, batchRun.Optimizer);
          node.Nodes.Add(childNode);
          node.Expand();
        }
      }
      RebuildImageList();
      UpdateDetailsViewHost();
    }

    private void Optimizers_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      if (InvokeRequired) {
        Invoke((Action<object, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>>>)Optimizers_ItemsAdded, sender, e);
        return;
      }

      var optimizerList = (OptimizerList)sender;
      IEnumerable<TreeNodeCollection> parentNodes;
      if (optimizerList == Content.Optimizers) parentNodes = new List<TreeNodeCollection>() { optimizerTreeView.Nodes };
      else {
        Experiment experiment = optimizerTreeViewMapping.Keys.OfType<Experiment>().Where(exp => exp.Optimizers == optimizerList).First();
        parentNodes = optimizerTreeViewMapping[experiment].Select(node => node.Nodes);
      }

      foreach (TreeNodeCollection parentNode in parentNodes) {
        foreach (var childOptimizer in e.Items) {
          TreeNode childNode = CreateTreeNode(childOptimizer.Value);
          UpdateChildTreeNodes(childNode.Nodes, childOptimizer.Value);
          parentNode.Insert(childOptimizer.Index, childNode);
          childNode.ExpandAll();
          if (childNode.Parent != null) childNode.Parent.ExpandAll();
        }
      }
      RebuildImageList();
    }
    private void Optimizers_ItemsMoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      if (InvokeRequired) {
        Invoke((Action<object, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>>>)Optimizers_ItemsMoved, sender, e);
        return;
      }

      var optimizerList = (OptimizerList)sender;
      IEnumerable<TreeNodeCollection> parentNodes;
      if (optimizerList == Content.Optimizers) parentNodes = new List<TreeNodeCollection>() { optimizerTreeView.Nodes };
      else {
        Experiment experiment = optimizerTreeViewMapping.Keys.OfType<Experiment>().Where(exp => exp.Optimizers == optimizerList).First();
        parentNodes = optimizerTreeViewMapping[experiment].Select(node => node.Nodes);
      }

      foreach (TreeNodeCollection parentNode in parentNodes) {
        //get all effected child nodes
        foreach (TreeNode childNode in parentNode.OfType<TreeNode>()
          .Where(n => e.OldItems.Select(x => x.Value).Contains((IOptimizer)n.Tag)).ToList()) {
          DisposeTreeNode(childNode);
          childNode.Remove();
        }

        foreach (var childOptimizer in e.Items) {
          TreeNode childNode = CreateTreeNode(childOptimizer.Value);
          UpdateChildTreeNodes(childNode.Nodes, childOptimizer.Value);
          childNode.ExpandAll();
          parentNode.Insert(childOptimizer.Index, childNode);
        }
      }
      RebuildImageList();
      UpdateDetailsViewHost();
    }
    private void Optimizers_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      if (InvokeRequired) {
        Invoke((Action<object, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>>>)Optimizers_ItemsRemoved, sender, e);
        return;
      }

      var optimizerList = (OptimizerList)sender;
      IEnumerable<TreeNodeCollection> parentNodes;
      if (optimizerList == Content.Optimizers) parentNodes = new List<TreeNodeCollection>() { optimizerTreeView.Nodes };
      else {
        Experiment experiment = optimizerTreeViewMapping.Keys.OfType<Experiment>().Where(exp => exp.Optimizers == optimizerList).First();
        parentNodes = optimizerTreeViewMapping[experiment].Select(node => node.Nodes);
      }

      foreach (TreeNodeCollection parentNode in parentNodes) {
        foreach (var childOptimizer in e.Items) {
          TreeNode childNode = parentNode[childOptimizer.Index];
          DisposeTreeNode(childNode);
          childNode.Remove();
        }
      }
      RebuildImageList();
      UpdateDetailsViewHost();
    }
    private void Optimizers_ItemsReplaced(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      if (InvokeRequired) {
        Invoke((Action<object, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>>>)Optimizers_ItemsReplaced, sender, e);
        return;
      }

      var optimizerList = (OptimizerList)sender;
      IEnumerable<TreeNodeCollection> parentNodes;
      if (optimizerList == Content.Optimizers) parentNodes = new List<TreeNodeCollection>() { optimizerTreeView.Nodes };
      else {
        Experiment experiment = optimizerTreeViewMapping.Keys.OfType<Experiment>().Where(exp => exp.Optimizers == optimizerList).First();
        parentNodes = optimizerTreeViewMapping[experiment].Select(node => node.Nodes);
      }

      foreach (TreeNodeCollection parentNode in parentNodes) {
        foreach (var childOptimizer in e.OldItems) {
          TreeNode childNode = parentNode.Cast<TreeNode>().Where(n => n.Tag == childOptimizer.Value && n.Index == childOptimizer.Index).First();
          DisposeTreeNode(childNode);
          childNode.Remove();
        }
        foreach (var childOptimizer in e.Items) {
          TreeNode childNode = CreateTreeNode(childOptimizer.Value);
          UpdateChildTreeNodes(childNode.Nodes, childOptimizer.Value);
          parentNode.Insert(childOptimizer.Index, childNode);
        }
      }
      RebuildImageList();
      UpdateDetailsViewHost();
    }
    private void Optimizers_CollectionReset(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      if (InvokeRequired) {
        Invoke((Action<object, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>>>)Optimizers_CollectionReset, sender, e);
        return;
      }

      var optimizerList = (OptimizerList)sender;
      IEnumerable<TreeNodeCollection> parentNodes;
      if (optimizerList == Content.Optimizers) parentNodes = new List<TreeNodeCollection>() { optimizerTreeView.Nodes };
      else {
        Experiment experiment = optimizerTreeViewMapping.Keys.OfType<Experiment>().Where(exp => exp.Optimizers == optimizerList).First();
        parentNodes = optimizerTreeViewMapping[experiment].Select(node => node.Nodes);
      }

      foreach (TreeNodeCollection parentNode in parentNodes) {
        foreach (var childOptimizer in e.OldItems) {
          TreeNode childNode = parentNode.Cast<TreeNode>().Where(n => n.Tag == childOptimizer.Value && n.Index == childOptimizer.Index).First();
          DisposeTreeNode(childNode);
          childNode.Remove();
        }
        foreach (var childOptimizer in e.Items) {
          TreeNode childNode = CreateTreeNode(childOptimizer.Value);
          UpdateChildTreeNodes(childNode.Nodes, childOptimizer.Value);
          parentNode.Insert(childOptimizer.Index, childNode);
        }
      }
      RebuildImageList();
      UpdateDetailsViewHost();
    }

    private void optimizer_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((Action<object, EventArgs>)optimizer_ToStringChanged, sender, e);
        return;
      }
      var optimizer = (IOptimizer)sender;
      foreach (TreeNode node in optimizerTreeViewMapping[optimizer])
        node.Text = optimizer.ToString();
    }
    #endregion

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      BatchRun batchRun = null;
      BatchRun parentBatchRun = null;
      Experiment experiment = null;
      if (optimizerTreeView.SelectedNode != null) {
        batchRun = optimizerTreeView.SelectedNode.Tag as BatchRun;
        experiment = optimizerTreeView.SelectedNode.Tag as Experiment;
        if (optimizerTreeView.SelectedNode.Parent != null)
          parentBatchRun = optimizerTreeView.SelectedNode.Parent.Tag as BatchRun;
      }

      optimizerTreeView.Enabled = Content != null;
      detailsViewHost.Enabled = Content != null && optimizerTreeView.SelectedNode != null;

      addButton.Enabled = Content != null && !Locked && !ReadOnly &&
        (optimizerTreeView.SelectedNode == null || experiment != null || (batchRun != null && batchRun.Optimizer == null));
      moveUpButton.Enabled = Content != null && !Locked && !ReadOnly &&
        optimizerTreeView.SelectedNode != null && optimizerTreeView.SelectedNode.PrevNode != null && parentBatchRun == null;
      moveDownButton.Enabled = Content != null && !Locked && !ReadOnly &&
        optimizerTreeView.SelectedNode != null && optimizerTreeView.SelectedNode.NextNode != null && parentBatchRun == null;
      removeButton.Enabled = Content != null && !Locked && !ReadOnly &&
        optimizerTreeView.SelectedNode != null;
    }

    private void UpdateOptimizerTreeView() {
      optimizerTreeView.Nodes.Clear();
      UpdateChildTreeNodes(optimizerTreeView.Nodes, Content);
      RebuildImageList();
    }


    private void UpdateChildTreeNodes(TreeNodeCollection collection, IOptimizer optimizer) {
      var batchRun = optimizer as BatchRun;
      var experiment = optimizer as Experiment;
      if (experiment != null) UpdateChildTreeNodes(collection, experiment.Optimizers);
      else if (batchRun != null && batchRun.Optimizer != null) UpdateChildTreeNodes(collection, new List<IOptimizer>() { batchRun.Optimizer });
    }
    private void UpdateChildTreeNodes(TreeNodeCollection collection, IEnumerable<IOptimizer> optimizers) {
      foreach (IOptimizer optimizer in optimizers) {
        var node = CreateTreeNode(optimizer);
        collection.Add(node);
        UpdateChildTreeNodes(node.Nodes, optimizer);
      }
    }


    #region drag & drop
    private void optimizerTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      if (Locked) return;

      TreeNode selectedNode = (TreeNode)e.Item;
      var optimizer = (IOptimizer)selectedNode.Tag;
      DataObject data = new DataObject();
      data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, optimizer);
      validDragOperation = true;

      if (ReadOnly) {
        DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
      } else {
        DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
        if ((result & DragDropEffects.Move) == DragDropEffects.Move) {
          if (selectedNode.Parent == null) Content.Optimizers.Remove(optimizer);
          else {
            var parentOptimizer = (IOptimizer)selectedNode.Parent.Tag;
            var parentBatchRun = parentOptimizer as BatchRun;
            var parentExperiment = parentOptimizer as Experiment;
            if (parentBatchRun != null) parentBatchRun.Optimizer = null;
            else if (parentExperiment != null) parentExperiment.Optimizers.Remove(optimizer);
            else throw new NotSupportedException("Handling for specific type not implemented" + parentOptimizer.GetType());
          }
          SetEnabledStateOfControls();
          UpdateDetailsViewHost();
          RebuildImageList();
        }
      }
    }

    private bool validDragOperation = false;
    private void optimizerTreeView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (!ReadOnly) {
        if ((e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IOptimizer)) validDragOperation = true;
        else if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IEnumerable) {
          validDragOperation = true;
          IEnumerable items = (IEnumerable)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
          foreach (object item in items)
            validDragOperation = validDragOperation && (item is IOptimizer);
        }
      }
    }
    private void optimizerTreeView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (validDragOperation) {
        Point coordinates = optimizerTreeView.PointToClient(new Point(e.X, e.Y));
        TreeNode node = optimizerTreeView.GetNodeAt(coordinates);
        Experiment experiment = null;
        BatchRun batchRun = null;

        if (node == null) experiment = Content;
        else {
          experiment = node.Tag as Experiment;
          batchRun = node.Tag as BatchRun;
        }

        if (batchRun == null && experiment == null) return;
        if (batchRun != null) {
          var optimizer = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IOptimizer;
          if (optimizer == null) return;
          if (batchRun.Optimizer != null) return;
          if (optimizer.GetObjectGraphObjects().OfType<IOptimizer>().Contains(batchRun)) return;
        }

        //do not allow recursive nesting of contents
        if (experiment != null) {
          var optimizer = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IOptimizer;
          IEnumerable<IOptimizer> optimizers = null;
          var enumerable = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) as IEnumerable;
          if (enumerable != null) optimizers = enumerable.Cast<IOptimizer>();

          if (optimizer != null && optimizer.GetObjectGraphObjects().OfType<IOptimizer>().Contains(experiment)) return;
          if (optimizers != null && optimizers.GetObjectGraphObjects().OfType<IOptimizer>().Contains(experiment)) return;
        }

        if ((e.KeyState & 32) == 32) e.Effect = DragDropEffects.Link;  // ALT key
        else if ((e.KeyState & 4) == 4) e.Effect = DragDropEffects.Move;  // SHIFT key
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
        else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
      }
    }
    private void optimizerTreeView_DragDrop(object sender, DragEventArgs e) {
      Point coordinates = optimizerTreeView.PointToClient(new Point(e.X, e.Y));
      TreeNode node = optimizerTreeView.GetNodeAt(coordinates);
      Experiment experiment = null;
      BatchRun batchRun = null;

      if (node == null) experiment = Content;
      else {
        experiment = node.Tag as Experiment;
        batchRun = node.Tag as BatchRun;
      }

      if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IOptimizer) {
        IOptimizer optimizer = (IOptimizer)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
        if (e.Effect.HasFlag(DragDropEffects.Copy)) optimizer = (IOptimizer)optimizer.Clone();
        if (batchRun != null) batchRun.Optimizer = optimizer;
        else if (experiment != null) experiment.Optimizers.Add(optimizer);
        else throw new NotSupportedException("Handling for specific type not implemented" + node.Tag.GetType());
      } else if (e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat) is IEnumerable) {
        IEnumerable<IOptimizer> optimizers = ((IEnumerable)e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat)).Cast<IOptimizer>();
        if (e.Effect.HasFlag(DragDropEffects.Copy)) {
          Cloner cloner = new Cloner();
          optimizers = optimizers.Select(o => (IOptimizer)o.Clone(cloner));
        }
        if (experiment != null) experiment.Optimizers.AddRange(optimizers);
        else throw new NotSupportedException("Handling for specific type not implemented" + node.Tag.GetType());
      }
    }
    #endregion

    #region control events
    private void optimizerTreeview_MouseClick(object sender, MouseEventArgs e) {
      Point coordinates = new Point(e.X, e.Y);
      TreeNode selectedNode = optimizerTreeView.GetNodeAt(coordinates);
      if (selectedNode != null) {
        optimizerTreeView.SelectedNode = selectedNode;
        detailsViewHost.Content = (IOptimizer)selectedNode.Tag;
        SetEnabledStateOfControls();
      }
    }
    private void optimizerTreeView_MouseDown(object sender, MouseEventArgs e) {
      // enables deselection of treeNodes
      if (optimizerTreeView.SelectedNode == null) return;
      Point coordinates = new Point(e.X, e.Y);
      if (e.Button == System.Windows.Forms.MouseButtons.Left && optimizerTreeView.GetNodeAt(coordinates) == null) {
        optimizerTreeView.SelectedNode = null;
        detailsViewHost.Content = null;
        SetEnabledStateOfControls();
      }
    }

    private void optimizerTreeView_KeyDown(object sender, KeyEventArgs e) {
      if (ReadOnly) return;
      if (optimizerTreeView.SelectedNode == null) return;
      if (e.KeyCode != Keys.Delete) return;

      var treeNode = optimizerTreeView.SelectedNode;
      var optimizer = (IOptimizer)treeNode.Tag;

      if (treeNode.Parent == null)
        Content.Optimizers.Remove(optimizer);
      else {
        var batchRun = treeNode.Parent.Tag as BatchRun;
        var experiment = treeNode.Parent.Tag as Experiment;
        if (batchRun != null) batchRun.Optimizer = null;
        else if (experiment != null) experiment.Optimizers.Remove(optimizer);
        else throw new NotSupportedException("Handling for specific type not implemented" + optimizerTreeView.SelectedNode.Tag.GetType());
      }
      SetEnabledStateOfControls();
      UpdateDetailsViewHost();
      RebuildImageList();
    }

    private void addButton_Click(object sender, System.EventArgs e) {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Optimizer";
        typeSelectorDialog.TypeSelector.Caption = "Available Optimizers";
        typeSelectorDialog.TypeSelector.Configure(typeof(IOptimizer), false, true);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          IOptimizer optimizer = (IOptimizer)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
          if (optimizerTreeView.SelectedNode == null) Content.Optimizers.Add(optimizer);
          else {
            var batchRun = optimizerTreeView.SelectedNode.Tag as BatchRun;
            var experiment = optimizerTreeView.SelectedNode.Tag as Experiment;
            if (batchRun != null) batchRun.Optimizer = optimizer;
            else if (experiment != null) experiment.Optimizers.Add(optimizer);
            else throw new NotSupportedException("Handling for specific type not implemented" + optimizerTreeView.SelectedNode.Tag.GetType());
          }
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    private void moveUpButton_Click(object sender, EventArgs e) {
      var optimizer = optimizerTreeView.SelectedNode.Tag as IOptimizer;
      Experiment experiment = null;
      if (optimizerTreeView.SelectedNode.Parent == null) experiment = Content;
      else experiment = (Experiment)optimizerTreeView.SelectedNode.Parent.Tag;

      int index = optimizerTreeView.SelectedNode.Index;
      experiment.Optimizers.Reverse(index - 1, 2);
      optimizerTreeView.SelectedNode = optimizerTreeViewMapping[optimizer].First();
      SetEnabledStateOfControls();
      UpdateDetailsViewHost();
      RebuildImageList();
    }
    private void moveDownButton_Click(object sender, EventArgs e) {
      var optimizer = optimizerTreeView.SelectedNode.Tag as IOptimizer;
      Experiment experiment = null;
      if (optimizerTreeView.SelectedNode.Parent == null) experiment = Content;
      else experiment = (Experiment)optimizerTreeView.SelectedNode.Parent.Tag;

      int index = optimizerTreeView.SelectedNode.Index;
      experiment.Optimizers.Reverse(index, 2);
      optimizerTreeView.SelectedNode = optimizerTreeViewMapping[optimizer].First();
      SetEnabledStateOfControls();
      UpdateDetailsViewHost();
      RebuildImageList();
    }

    private void removeButton_Click(object sender, EventArgs e) {
      var treeNode = optimizerTreeView.SelectedNode;
      var optimizer = (IOptimizer)treeNode.Tag;

      if (treeNode.Parent == null)
        Content.Optimizers.Remove(optimizer);
      else {
        var batchRun = treeNode.Parent.Tag as BatchRun;
        var experiment = treeNode.Parent.Tag as Experiment;
        if (batchRun != null) batchRun.Optimizer = null;
        else if (experiment != null) experiment.Optimizers.Remove(optimizer);
        else throw new NotSupportedException("Handling for specific type not implemented" + optimizerTreeView.SelectedNode.Tag.GetType());
      }
      SetEnabledStateOfControls();
      UpdateDetailsViewHost();
      RebuildImageList();
    }

    private void showDetailsCheckBox_CheckedChanged(object sender, System.EventArgs e) {
      if (showDetailsCheckBox.Checked) {
        splitContainer.Panel2Collapsed = false;
        detailsGroupBox.Enabled = optimizerTreeView.SelectedNode != null;
        detailsViewHost.Content = optimizerTreeView.SelectedNode != null ? (IOptimizer)optimizerTreeView.SelectedNode.Tag : null;
      } else {
        splitContainer.Panel2Collapsed = true;
        detailsViewHost.Content = null;
      }
    }
    #endregion

    #region helpers
    private void UpdateDetailsViewHost() {
      if (optimizerTreeView.SelectedNode != null)
        detailsViewHost.Content = (IOptimizer)optimizerTreeView.SelectedNode.Tag;
      else
        detailsViewHost.Content = null;
    }

    private TreeNode CreateTreeNode(IOptimizer optimizer) {
      TreeNode node = new TreeNode(optimizer.ToString());
      node.Tag = optimizer;
      List<TreeNode> nodes;
      if (!optimizerTreeViewMapping.TryGetValue(optimizer, out nodes)) {
        nodes = new List<TreeNode>();
        optimizerTreeViewMapping.Add(optimizer, nodes);
        RegisterOptimizerEvents(optimizer);
      }
      nodes.Add(node);
      return node;
    }

    private void DisposeTreeNode(TreeNode node) {
      var optimizer = (IOptimizer)node.Tag;
      List<TreeNode> nodes;
      if (!optimizerTreeViewMapping.TryGetValue(optimizer, out nodes))
        throw new ArgumentException();
      nodes.Remove(node);
      if (nodes.Count == 0) {
        optimizerTreeViewMapping.Remove(optimizer);
        DeregisterOptimizerEvents(optimizer);
      }
    }

    private IEnumerable<TreeNode> IterateTreeNodes(TreeNode node = null) {
      TreeNodeCollection nodes;
      if (node == null)
        nodes = optimizerTreeView.Nodes;
      else {
        nodes = node.Nodes;
        yield return node;
      }

      foreach (var childNode in nodes.OfType<TreeNode>())
        foreach (var n in IterateTreeNodes(childNode))
          yield return n;
    }

    private void RebuildImageList() {
      if (InvokeRequired) {
        Invoke((Action)RebuildImageList);
        return;
      }

      optimizerTreeView.ImageList.Images.Clear();
      foreach (TreeNode treeNode in IterateTreeNodes()) {
        var optimizer = (IOptimizer)treeNode.Tag;
        optimizerTreeView.ImageList.Images.Add(optimizer == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : optimizer.ItemImage);
        treeNode.ImageIndex = optimizerTreeView.ImageList.Images.Count - 1;
        treeNode.SelectedImageIndex = treeNode.ImageIndex;
      }
    }
    #endregion
  }
}
