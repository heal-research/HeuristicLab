#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimization.Views {
  public sealed partial class ExperimentTreeView : ItemView {
    private TypeSelectorDialog typeSelectorDialog;
    private Dictionary<INamedItem, List<TreeNode>> treeNodeTagMapping;

    public ExperimentTreeView() {
      InitializeComponent();
      treeNodeTagMapping = new Dictionary<INamedItem, List<TreeNode>>();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region necessary code to handle dock correctly regarding the expanded nodes
    bool[] expandendedState;
    protected override void OnHandleCreated(EventArgs e) {
      base.OnHandleCreated(e);
      if (expandendedState == null) return;
      var nodes = IterateTreeNodes().ToList();
      for (int i = 0; i < nodes.Count; i++)
        if (expandendedState[i]) nodes[i].Expand();
    }
    protected override void OnHandleDestroyed(EventArgs e) {
      base.OnHandleDestroyed(e);
      var nodes = IterateTreeNodes().ToList();
      expandendedState = new bool[nodes.Count];
      for (int i = 0; i < nodes.Count; i++)
        expandendedState[i] = nodes[i].IsExpanded;
    }
    #endregion

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
      foreach (var optimizer in treeNodeTagMapping.Keys)
        DeregisterNamedItemEvents(optimizer);
      base.DeregisterContentEvents();
    }

    private void RegisterNamedItemEvents(INamedItem namedItem) {
      namedItem.ToStringChanged += new EventHandler(namedItem_ToStringChanged);
      namedItem.ItemImageChanged += new EventHandler(namedItem_ItemImageChanged);

      var algorithm = namedItem as IAlgorithm;
      var batchRun = namedItem as BatchRun;
      var experiment = namedItem as Experiment;

      if (algorithm != null) {
        algorithm.Prepared += new EventHandler(algorithm_Prepared);
        algorithm.ProblemChanged += new EventHandler(algorithm_ProblemChanged);
      } else if (batchRun != null) {
        batchRun.OptimizerChanged += new EventHandler(batchRun_OptimizerChanged);
        batchRun.RepetetionsCounterChanged += new EventHandler(batchRun_RepetitionsCounterChanged);
        batchRun.RepetitionsChanged += new EventHandler(batchRun_RepetitionsChanged);
      } else if (experiment != null) {
        experiment.Optimizers.ItemsAdded += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsAdded);
        experiment.Optimizers.ItemsMoved += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsMoved);
        experiment.Optimizers.ItemsRemoved += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsRemoved);
        experiment.Optimizers.ItemsReplaced += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_ItemsReplaced);
        experiment.Optimizers.CollectionReset += new CollectionItemsChangedEventHandler<IndexedItem<IOptimizer>>(Optimizers_CollectionReset);
      }
    }

    private void DeregisterNamedItemEvents(INamedItem namedItem) {
      namedItem.ToStringChanged -= new EventHandler(namedItem_ToStringChanged);
      namedItem.ItemImageChanged -= new EventHandler(namedItem_ItemImageChanged);

      var algorithm = namedItem as IAlgorithm;
      var batchRun = namedItem as BatchRun;
      var experiment = namedItem as Experiment;

      if (algorithm != null) {
        algorithm.Prepared -= new EventHandler(algorithm_Prepared);
        algorithm.ProblemChanged -= new EventHandler(algorithm_ProblemChanged);
      } else if (batchRun != null) {
        batchRun.OptimizerChanged -= new EventHandler(batchRun_OptimizerChanged);
        batchRun.RepetetionsCounterChanged -= new EventHandler(batchRun_RepetitionsCounterChanged);
        batchRun.RepetitionsChanged += new EventHandler(batchRun_RepetitionsChanged);
      } else if (experiment != null) {
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
        treeView.Nodes.Clear();
      } else {
        UpdateOptimizerTreeView();
        treeView.ExpandAll();
      }
    }

    #region content events
    private void Content_ExecutionStateChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((Action<object, EventArgs>)Content_ExecutionStateChanged, sender, e);
        return;
      }
      SetEnabledStateOfControls();
    }

    private void algorithm_Prepared(object sender, EventArgs e) {
      var algorithm = (IAlgorithm)sender;
      foreach (TreeNode node in treeNodeTagMapping[algorithm]) {
        TreeNode resultsNode = node.Nodes.OfType<TreeNode>().Where(x => x.Tag is ResultCollection).Single();
        if (detailsViewHost.Content == resultsNode.Tag)
          detailsViewHost.Content = algorithm.Results;
        resultsNode.Tag = algorithm.Results;
      }
    }

    private void algorithm_ProblemChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((Action<object, EventArgs>)algorithm_ProblemChanged, sender, e);
        return;
      }

      var algorithm = (IAlgorithm)sender;
      foreach (TreeNode node in treeNodeTagMapping[algorithm]) {
        foreach (TreeNode childNode in node.Nodes.OfType<TreeNode>().ToList()) {
          DisposeTreeNode(childNode);
          childNode.Remove();
        }
        List<TreeNode> nodes;
        foreach (TreeNode childNode in CreateAlgorithmChildNodes(algorithm)) {
          node.Nodes.Add(childNode);
          INamedItem namedItem = childNode.Tag as INamedItem;
          if (namedItem != null) {
            if (!treeNodeTagMapping.TryGetValue(namedItem, out nodes)) {
              nodes = new List<TreeNode>();
              treeNodeTagMapping.Add(namedItem, nodes);
              RegisterNamedItemEvents(namedItem);
            }
            nodes.Add(childNode);
          }
        }

        node.Expand();
      }

      RebuildImageList();
      UpdateDetailsViewHost();
    }

    private void batchRun_OptimizerChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((Action<object, EventArgs>)batchRun_OptimizerChanged, sender, e);
        return;
      }
      var batchRun = (BatchRun)sender;
      foreach (TreeNode node in treeNodeTagMapping[batchRun]) {
        foreach (TreeNode childNode in node.Nodes.OfType<TreeNode>().ToList()) {
          DisposeTreeNode(childNode);
          childNode.Remove();
        }

        if (batchRun.Optimizer != null) {
          UpdateChildTreeNodes(node.Nodes, batchRun);
          node.Expand();
        }
      }
      RebuildImageList();
      UpdateDetailsViewHost();
    }

    private void batchRun_RepetitionsCounterChanged(object sender, EventArgs e) {
      namedItem_ToStringChanged(sender, e);
    }
    private void batchRun_RepetitionsChanged(object sender, EventArgs e) {
      namedItem_ToStringChanged(sender, e);
    }

    private void Optimizers_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      if (InvokeRequired) {
        Invoke((Action<object, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>>>)Optimizers_ItemsAdded, sender, e);
        return;
      }

      var optimizerList = (OptimizerList)sender;
      IEnumerable<TreeNodeCollection> parentNodes;
      if (optimizerList == Content.Optimizers) parentNodes = new List<TreeNodeCollection>() { treeView.Nodes };
      else {
        Experiment experiment = treeNodeTagMapping.Keys.OfType<Experiment>().Where(exp => exp.Optimizers == optimizerList).First();
        parentNodes = treeNodeTagMapping[experiment].Select(node => node.Nodes);
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
      if (optimizerList == Content.Optimizers) parentNodes = new List<TreeNodeCollection>() { treeView.Nodes };
      else {
        Experiment experiment = treeNodeTagMapping.Keys.OfType<Experiment>().Where(exp => exp.Optimizers == optimizerList).First();
        parentNodes = treeNodeTagMapping[experiment].Select(node => node.Nodes);
      }

      foreach (TreeNodeCollection parentNode in parentNodes) {
        var backup = parentNode.OfType<TreeNode>().ToList();
        foreach (var indexedItem in e.Items) {
          var node = backup.Where(n => n.Tag == indexedItem.Value).First();
          node.Remove();
          parentNode.Insert(indexedItem.Index, node);
        }
      }
    }
    private void Optimizers_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>> e) {
      if (InvokeRequired) {
        Invoke((Action<object, CollectionItemsChangedEventArgs<IndexedItem<IOptimizer>>>)Optimizers_ItemsRemoved, sender, e);
        return;
      }

      var optimizerList = (OptimizerList)sender;
      IEnumerable<TreeNodeCollection> parentNodes;
      if (optimizerList == Content.Optimizers) parentNodes = new List<TreeNodeCollection>() { treeView.Nodes };
      else {
        Experiment experiment = treeNodeTagMapping.Keys.OfType<Experiment>().Where(exp => exp.Optimizers == optimizerList).First();
        parentNodes = treeNodeTagMapping[experiment].Select(node => node.Nodes);
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
      if (optimizerList == Content.Optimizers) parentNodes = new List<TreeNodeCollection>() { treeView.Nodes };
      else {
        Experiment experiment = treeNodeTagMapping.Keys.OfType<Experiment>().Where(exp => exp.Optimizers == optimizerList).First();
        parentNodes = treeNodeTagMapping[experiment].Select(node => node.Nodes);
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
      if (optimizerList == Content.Optimizers) parentNodes = new List<TreeNodeCollection>() { treeView.Nodes };
      else {
        Experiment experiment = treeNodeTagMapping.Keys.OfType<Experiment>().Where(exp => exp.Optimizers == optimizerList).First();
        parentNodes = treeNodeTagMapping[experiment].Select(node => node.Nodes);
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

    private void namedItem_ToStringChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((Action<object, EventArgs>)namedItem_ToStringChanged, sender, e);
        return;
      }
      var namedItem = (INamedItem)sender;
      foreach (TreeNode node in treeNodeTagMapping[namedItem]) {
        node.Text = namedItem.ToString();
        var batchRun = namedItem as BatchRun;
        if (batchRun != null)
          node.Text += string.Format(" {0}/{1}", batchRun.RepetitionsCounter, batchRun.Repetitions);
      }
    }

    private void namedItem_ItemImageChanged(object sender, EventArgs e) {
      if (InvokeRequired) {
        Invoke((Action<object, EventArgs>)namedItem_ItemImageChanged, sender, e);
        return;
      }
      INamedItem namedItem = (INamedItem)sender;
      foreach (TreeNode node in treeNodeTagMapping[namedItem]) {
        treeView.ImageList.Images[node.ImageIndex] = namedItem.ItemImage;
        node.ImageIndex = node.ImageIndex;
      }
      SetEnabledStateOfControls();
    }
    #endregion

    protected override void PropagateStateChanges(Control control, Type type, System.Reflection.PropertyInfo propertyInfo) {
      return;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
      IOptimizer optimizer = null;
      IAlgorithm algorithm = null;
      BatchRun batchRun = null;
      Experiment experiment = null;

      IOptimizer parentOptimizer = null;
      Experiment parentExperiment = null;

      if (treeView.SelectedNode != null) {
        optimizer = treeView.SelectedNode.Tag as IOptimizer;
        algorithm = optimizer as IAlgorithm;
        batchRun = optimizer as BatchRun;
        experiment = optimizer as Experiment;

        if (treeView.SelectedNode.Parent != null) parentOptimizer = treeView.SelectedNode.Parent.Tag as IOptimizer;
        else parentOptimizer = Content;

        parentExperiment = parentOptimizer as Experiment;
      }

      treeView.Enabled = Content != null;
      if (parentOptimizer != null) {
        detailsViewHost.ReadOnly = parentOptimizer.ExecutionState == ExecutionState.Started;
        detailsViewHost.Locked = parentOptimizer.ExecutionState == ExecutionState.Started;
      }

      addButton.Enabled = Content != null && !Locked && !ReadOnly &&
        (treeView.SelectedNode == null || experiment != null || batchRun != null || algorithm != null);
      moveUpButton.Enabled = Content != null && !Locked && !ReadOnly &&
        treeView.SelectedNode != null && treeView.SelectedNode.PrevNode != null && parentExperiment != null;
      moveDownButton.Enabled = Content != null && !Locked && !ReadOnly &&
        treeView.SelectedNode != null && treeView.SelectedNode.NextNode != null && parentExperiment != null;
      removeButton.Enabled = Content != null && !Locked && !ReadOnly && optimizer != null;
    }

    private void UpdateOptimizerTreeView() {
      treeView.Nodes.Clear();
      UpdateChildTreeNodes(treeView.Nodes, Content);
      RebuildImageList();
    }

    private void UpdateChildTreeNodes(TreeNodeCollection collection, IOptimizer optimizer) {
      var batchRun = optimizer as BatchRun;
      var experiment = optimizer as Experiment;

      if (batchRun != null && batchRun.Optimizer != null) UpdateChildTreeNodes(collection, new List<IOptimizer>() { batchRun.Optimizer });
      else if (experiment != null) UpdateChildTreeNodes(collection, experiment.Optimizers);
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
      var item = (IItem)selectedNode.Tag;
      if (item == null) return;

      DataObject data = new DataObject();
      data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, item);
      validDragOperation = true;

      if (ReadOnly || !(item is IOptimizer)) {
        DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
      } else {
        DragDropEffects result = DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move);
        if ((result & DragDropEffects.Move) == DragDropEffects.Move) {
          var optimizer = (IOptimizer)item;
          if (selectedNode.Parent == null) Content.Optimizers.Remove(optimizer);
          else {
            var parentOptimizer = (IOptimizer)selectedNode.Parent.Tag;
            var parentBatchRun = parentOptimizer as BatchRun;
            var parentExperiment = parentOptimizer as Experiment;
            if (parentBatchRun != null) parentBatchRun.Optimizer = null;
            else if (parentExperiment != null) parentExperiment.Optimizers.Remove(optimizer);
            else throw new NotSupportedException("Handling for specific type not implemented" + parentOptimizer.GetType());
          }
        }
      }
    }

    private bool validDragOperation = false;
    private void optimizerTreeView_DragEnter(object sender, DragEventArgs e) {
      validDragOperation = false;
      if (!ReadOnly) {
        var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
        if (data is IOptimizer) validDragOperation = true;
        else if (data is IProblem) validDragOperation = true;
        else if (data is IEnumerable) {
          IEnumerable items = (IEnumerable)data;
          validDragOperation = true;
          foreach (object item in items)
            validDragOperation = validDragOperation && (item is IOptimizer);
        }
      }
    }
    private void optimizerTreeView_DragOver(object sender, DragEventArgs e) {
      e.Effect = DragDropEffects.None;
      if (!validDragOperation) return;
      Point coordinates = treeView.PointToClient(new Point(e.X, e.Y));
      TreeNode node = treeView.GetNodeAt(coordinates);
      Experiment experiment = null;
      BatchRun batchRun = null;
      Algorithm algorithm = null;

      if (node == null) experiment = Content;
      else {
        experiment = node.Tag as Experiment;
        batchRun = node.Tag as BatchRun;
        algorithm = node.Tag as Algorithm;
      }

      if (batchRun == null && experiment == null && algorithm == null) return;

      var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);

      if (algorithm != null) {
        var problem = data as IProblem;
        if (problem == null) return;
        if (!algorithm.ProblemType.IsAssignableFrom(problem.GetType())) return;
      } else if (batchRun != null) {
        var optimizer = data as IOptimizer;
        if (optimizer == null) return;
        if (batchRun == optimizer) return;
        if (optimizer.NestedOptimizers.Contains(batchRun)) return;
      } //do not allow recursive nesting of contents
      else if (experiment != null) {
        var optimizer = data as IOptimizer;
        IEnumerable<IOptimizer> optimizers = null;
        var enumerable = data as IEnumerable;
        if (enumerable != null) optimizers = enumerable.Cast<IOptimizer>();
        if (experiment == optimizer) return;
        if (optimizer != null && optimizer.NestedOptimizers.Contains(experiment)) return;
        if (optimizers != null && optimizers.Any(x => x.NestedOptimizers.Contains(experiment))) return;
      }

      if ((e.KeyState & 32) == 32 && e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;  // ALT key
      else if ((e.KeyState & 4) == 4 && e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;  // SHIFT key
      else if (e.AllowedEffect.HasFlag(DragDropEffects.Copy)) e.Effect = DragDropEffects.Copy;
      else if (e.AllowedEffect.HasFlag(DragDropEffects.Move)) e.Effect = DragDropEffects.Move;
      else if (e.AllowedEffect.HasFlag(DragDropEffects.Link)) e.Effect = DragDropEffects.Link;
    }

    private void optimizerTreeView_DragDrop(object sender, DragEventArgs e) {
      Point coordinates = treeView.PointToClient(new Point(e.X, e.Y));
      TreeNode node = treeView.GetNodeAt(coordinates);
      Algorithm algorithm = null;
      BatchRun batchRun = null;
      Experiment experiment = null;

      if (node == null) experiment = Content;
      else {
        algorithm = node.Tag as Algorithm;
        batchRun = node.Tag as BatchRun;
        experiment = node.Tag as Experiment;
      }

      var data = e.Data.GetData(HeuristicLab.Common.Constants.DragDropDataFormat);
      if (data is IProblem) {
        var problem = (IProblem)data;
        if (e.Effect.HasFlag(DragDropEffects.Copy)) problem = (IProblem)problem.Clone();
        algorithm.Problem = problem;
      } else if (data is IOptimizer) {
        IOptimizer optimizer = (IOptimizer)data;
        if (e.Effect.HasFlag(DragDropEffects.Copy)) optimizer = (IOptimizer)optimizer.Clone();
        if (batchRun != null) batchRun.Optimizer = optimizer;
        else if (experiment != null) experiment.Optimizers.Add(optimizer);
        else throw new NotSupportedException("Handling for specific type not implemented" + node.Tag.GetType());
      } else if (data is IEnumerable) {
        IEnumerable<IOptimizer> optimizers = ((IEnumerable)data).Cast<IOptimizer>();
        if (e.Effect.HasFlag(DragDropEffects.Copy)) {
          Cloner cloner = new Cloner();
          optimizers = optimizers.Select(o => cloner.Clone(o));
        }
        if (experiment != null) experiment.Optimizers.AddRange(optimizers);
        else throw new NotSupportedException("Handling for specific type not implemented" + node.Tag.GetType());
      }
    }
    #endregion

    #region control events
    private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
      if (rightClickOccured) return;
      if (e.X < e.Node.Bounds.Left - treeView.ImageList.Images[e.Node.ImageIndex].Width || e.X > e.Node.Bounds.Right) return;
      e.Node.Toggle();
      IContent optimizer = (IContent)e.Node.Tag;
      MainFormManager.MainForm.ShowContent(optimizer);
    }
    private void treeview_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
      if (e.X < e.Node.Bounds.Left - treeView.ImageList.Images[e.Node.ImageIndex].Width || e.X > e.Node.Bounds.Right) return;
      treeView.SelectedNode = e.Node;
      detailsViewHost.Content = (IContent)e.Node.Tag;
      SetEnabledStateOfControls();
    }
    private void treeView_MouseDown(object sender, MouseEventArgs e) {
      // enables deselection of treeNodes
      if (e.Button != MouseButtons.Right) rightClickOccured = false;
      if (treeView.SelectedNode == null) return;
      Point coordinates = new Point(e.X, e.Y);
      TreeNode node = treeView.GetNodeAt(coordinates);
      if (node == null || coordinates.X < node.Bounds.Left - treeView.ImageList.Images[node.ImageIndex].Width || coordinates.X > node.Bounds.Right) {
        treeView.SelectedNode = null;
        detailsViewHost.Content = null;
        SetEnabledStateOfControls();
      }
    }


    private void treeView_KeyDown(object sender, KeyEventArgs e) {
      if (Locked || ReadOnly) return;
      if (treeView.SelectedNode == null) return;
      if (!(treeView.SelectedNode.Tag is INamedItem)) return;

      var treeNode = treeView.SelectedNode;
      var namedItem = (INamedItem)treeNode.Tag;
      var optimizer = namedItem as IOptimizer;

      if (e.KeyCode == Keys.F2 && !treeNode.IsEditing) {
        treeNode.BeginEdit();
        return;
      }

      if (e.KeyCode == Keys.Delete && optimizer != null) {
        if (treeNode.Parent == null)
          Content.Optimizers.Remove(optimizer);
        else {
          var batchRun = treeNode.Parent.Tag as BatchRun;
          var experiment = treeNode.Parent.Tag as Experiment;
          if (batchRun != null) batchRun.Optimizer = null;
          else if (experiment != null) experiment.Optimizers.Remove(optimizer);
          else throw new NotSupportedException("Handling for specific type not implemented" + treeView.SelectedNode.Tag.GetType());
        }
        SetEnabledStateOfControls();
        UpdateDetailsViewHost();
        RebuildImageList();
      }
    }


    private void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
      if (e.Action == TreeViewAction.ByKeyboard) {
        UpdateDetailsViewHost();
        //NOTE: necessary because algorithm view steals the focus
        treeView.Focus();
      }
    }

    private bool rightClickOccured = true;
    private TreeNode toolStripMenuNode = null;
    private void treeView_RightClick(object sender, EventArgs e) {
      rightClickOccured = true;
      Point coordinates = treeView.PointToClient(Cursor.Position);
      toolStripMenuNode = treeView.GetNodeAt(coordinates);

      if (toolStripMenuNode != null && coordinates.X >= toolStripMenuNode.Bounds.Left && coordinates.X <= toolStripMenuNode.Bounds.Right) {
        treeView.SelectedNode = toolStripMenuNode;
        detailsViewHost.Content = (IContent)toolStripMenuNode.Tag;
        SetEnabledStateOfControls();

        ExpandToolStripMenuItem.Enabled = ExpandToolStripMenuItem.Visible = !toolStripMenuNode.IsExpanded && toolStripMenuNode.Nodes.Count > 0;
        CollapseToolStripMenuItem.Enabled = CollapseToolStripMenuItem.Visible = toolStripMenuNode.IsExpanded;
        EditNodeLabelToolStripMenuItem.Enabled = EditNodeLabelToolStripMenuItem.Visible = !Locked && !ReadOnly && toolStripMenuNode.Tag != null && toolStripMenuNode.Tag is INamedItem && ((INamedItem)toolStripMenuNode.Tag).CanChangeName;
      } else {
        ExpandToolStripMenuItem.Enabled = ExpandToolStripMenuItem.Visible = false;
        CollapseToolStripMenuItem.Enabled = CollapseToolStripMenuItem.Visible = false;
        EditNodeLabelToolStripMenuItem.Enabled = EditNodeLabelToolStripMenuItem.Visible = false;
      }
      ExpandAllToolStripMenuItem.Enabled = ExpandAllToolStripMenuItem.Visible = !treeView.Nodes.OfType<TreeNode>().All(x => TreeNodeIsFullyExpanded(x));
      CollapseAllToolStripMenuItem.Enabled = CollapseAllToolStripMenuItem.Visible = treeView.Nodes.OfType<TreeNode>().Any(x => x.IsExpanded);
      if (contextMenuStrip.Items.Cast<ToolStripMenuItem>().Any(item => item.Enabled))
        contextMenuStrip.Show(Cursor.Position);
    }

    private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e) {
      if (Locked) e.CancelEdit = true;
      if (ReadOnly) e.CancelEdit = true;
      if (e.Node.Tag == null) e.CancelEdit = true;
      var namedItem = e.Node.Tag as INamedItem;
      if (namedItem == null) e.CancelEdit = true;
      else if (!namedItem.CanChangeName) e.CancelEdit = true;
    }
    private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
      if (e.Label == null) return;
      e.Node.EndEdit(false);
      var namedItem = (INamedItem)e.Node.Tag;
      namedItem.Name = e.Label;
    }

    private void ExpandToolStripMenuItem_Click(object sender, EventArgs e) {
      if (toolStripMenuNode != null) toolStripMenuNode.ExpandAll();
    }
    private void ExpandAllToolStripMenuItem_Click(object sender, EventArgs e) {
      treeView.ExpandAll();
    }
    private void CollapseToolStripMenuItem_Click(object sender, EventArgs e) {
      if (toolStripMenuNode != null) toolStripMenuNode.Collapse();
    }
    private void CollapseAllToolStripMenuItem_Click(object sender, EventArgs e) {
      treeView.CollapseAll();
    }
    private void EditNodeLabelToolStripMenuItem_Click(object sender, EventArgs e) {
      if (toolStripMenuNode != null) {
        if (!toolStripMenuNode.IsEditing) toolStripMenuNode.BeginEdit();
      }
    }

    private void addButton_Click(object sender, System.EventArgs e) {
      if (typeSelectorDialog == null) typeSelectorDialog = new TypeSelectorDialog();

      IAlgorithm algorithm = null;
      if (treeView.SelectedNode != null && (treeView.SelectedNode.Tag is IAlgorithm))
        algorithm = (IAlgorithm)treeView.SelectedNode.Tag;

      if (algorithm == null) {
        typeSelectorDialog.Caption = "Select Optimizer";
        typeSelectorDialog.TypeSelector.Caption = "Available Optimizers";
        typeSelectorDialog.TypeSelector.Configure(typeof(IOptimizer), false, true);
      } else {
        typeSelectorDialog.Caption = "Select Problem";
        typeSelectorDialog.TypeSelector.Caption = "Available Problems";
        typeSelectorDialog.TypeSelector.Configure(algorithm.ProblemType, false, true);
      }

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        try {
          if (algorithm == null) {
            IOptimizer optimizer = (IOptimizer)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
            if (treeView.SelectedNode == null) Content.Optimizers.Add(optimizer);
            else {
              var batchRun = treeView.SelectedNode.Tag as BatchRun;
              var experiment = treeView.SelectedNode.Tag as Experiment;
              if (batchRun != null) batchRun.Optimizer = optimizer;
              else if (experiment != null) experiment.Optimizers.Add(optimizer);
              else throw new NotSupportedException("Handling for specific type not implemented" + treeView.SelectedNode.Tag.GetType());
            }
          } else {
            IProblem problem = (IProblem)typeSelectorDialog.TypeSelector.CreateInstanceOfSelectedType();
            algorithm.Problem = problem;
          }
        }
        catch (Exception ex) {
          ErrorHandling.ShowErrorDialog(this, ex);
        }
      }
    }

    private void moveUpButton_Click(object sender, EventArgs e) {
      var optimizer = (IOptimizer)treeView.SelectedNode.Tag;
      Experiment experiment = null;
      if (treeView.SelectedNode.Parent == null) experiment = Content;
      else experiment = (Experiment)treeView.SelectedNode.Parent.Tag;

      var selectedNode = treeView.SelectedNode;
      int index = treeView.SelectedNode.Index;
      experiment.Optimizers.Reverse(index - 1, 2);
      treeView.SelectedNode = selectedNode;
      SetEnabledStateOfControls();
    }
    private void moveDownButton_Click(object sender, EventArgs e) {
      var optimizer = (IOptimizer)treeView.SelectedNode.Tag;
      Experiment experiment = null;
      if (treeView.SelectedNode.Parent == null) experiment = Content;
      else experiment = (Experiment)treeView.SelectedNode.Parent.Tag;

      var selectedNode = treeView.SelectedNode;
      int index = treeView.SelectedNode.Index;
      experiment.Optimizers.Reverse(index, 2);
      treeView.SelectedNode = selectedNode;
      SetEnabledStateOfControls();
    }

    private void removeButton_Click(object sender, EventArgs e) {
      var treeNode = treeView.SelectedNode;
      var optimizer = treeNode.Tag as IOptimizer;

      if (treeNode.Parent == null)
        Content.Optimizers.Remove(optimizer);
      else {
        var batchRun = treeNode.Parent.Tag as BatchRun;
        var experiment = treeNode.Parent.Tag as Experiment;
        if (batchRun != null) batchRun.Optimizer = null;
        else if (experiment != null) experiment.Optimizers.Remove(optimizer);
        else throw new NotSupportedException("Handling for specific type not implemented" + treeView.SelectedNode.Tag.GetType());
      }
      SetEnabledStateOfControls();
      UpdateDetailsViewHost();
      RebuildImageList();
    }

    private void showDetailsCheckBox_CheckedChanged(object sender, System.EventArgs e) {
      if (showDetailsCheckBox.Checked) {
        splitContainer.Panel2Collapsed = false;
        detailsGroupBox.Enabled = treeView.SelectedNode != null;
        detailsViewHost.Content = treeView.SelectedNode != null ? (IContent)treeView.SelectedNode.Tag : null;
      } else {
        splitContainer.Panel2Collapsed = true;
        detailsViewHost.Content = null;
      }
    }
    #endregion

    #region helpers
    private void UpdateDetailsViewHost() {
      if (treeView.SelectedNode != null)
        detailsViewHost.Content = (IContent)treeView.SelectedNode.Tag;
      else
        detailsViewHost.Content = null;
    }

    private TreeNode CreateTreeNode(IOptimizer optimizer) {
      TreeNode node = new TreeNode(optimizer.ToString());
      node.Tag = optimizer;

      var algorithm = optimizer as IAlgorithm;
      if (algorithm != null) {
        foreach (TreeNode childNode in CreateAlgorithmChildNodes(algorithm))
          node.Nodes.Add(childNode);
      }
      var batchRun = optimizer as BatchRun;
      if (batchRun != null) {
        node.Text += string.Format(" {0}/{1}", batchRun.RepetitionsCounter, batchRun.Repetitions);
      }

      List<TreeNode> nodes;
      if (!treeNodeTagMapping.TryGetValue(optimizer, out nodes)) {
        nodes = new List<TreeNode>();
        treeNodeTagMapping.Add(optimizer, nodes);
        RegisterNamedItemEvents(optimizer);
      }
      nodes.Add(node);

      foreach (TreeNode childNode in node.Nodes) {
        INamedItem namedItem = childNode.Tag as INamedItem;
        if (namedItem != null) {
          if (!treeNodeTagMapping.TryGetValue(namedItem, out nodes)) {
            nodes = new List<TreeNode>();
            treeNodeTagMapping.Add(namedItem, nodes);
            RegisterNamedItemEvents(namedItem);
          }
          nodes.Add(childNode);
        }
      }
      return node;
    }

    private IEnumerable<TreeNode> CreateAlgorithmChildNodes(IAlgorithm algorithm) {
      TreeNode problemNode;
      if (algorithm.Problem != null) {
        problemNode = new TreeNode(algorithm.Problem.Name);
        problemNode.Tag = algorithm.Problem;
      } else {
        problemNode = new TreeNode("No Problem");
        problemNode.Tag = null;
      }
      TreeNode parametersNode = new TreeNode("Parameters");
      parametersNode.Tag = algorithm.Parameters;
      TreeNode resultsNode = new TreeNode("Results");
      resultsNode.Tag = algorithm.Results;

      yield return problemNode;
      yield return parametersNode;
      yield return resultsNode;
    }

    private void DisposeTreeNode(TreeNode node) {
      var namedItem = node.Tag as INamedItem;
      if (namedItem == null) return;

      List<TreeNode> nodes;
      if (!treeNodeTagMapping.TryGetValue(namedItem, out nodes))
        throw new ArgumentException();
      nodes.Remove(node);
      if (nodes.Count == 0) {
        treeNodeTagMapping.Remove(namedItem);
        DeregisterNamedItemEvents(namedItem);
      }
    }

    private IEnumerable<TreeNode> IterateTreeNodes(TreeNode node = null) {
      TreeNodeCollection nodes;
      if (node == null)
        nodes = treeView.Nodes;
      else {
        nodes = node.Nodes;
        yield return node;
      }

      foreach (var childNode in nodes.OfType<TreeNode>())
        foreach (var n in IterateTreeNodes(childNode))
          yield return n;
    }

    private bool TreeNodeIsFullyExpanded(TreeNode node) {
      return (node.Nodes.Count == 0) || (node.IsExpanded && node.Nodes.OfType<TreeNode>().All(x => TreeNodeIsFullyExpanded(x)));
    }

    private void RebuildImageList() {
      if (InvokeRequired) {
        Invoke((Action)RebuildImageList);
        return;
      }

      treeView.BeginUpdate();

      treeView.ImageList.Images.Clear();
      var topLevelNodes = treeView.Nodes.OfType<TreeNode>().ToArray();
      var nodes = IterateTreeNodes().ToList();
      var selectedNode = treeView.SelectedNode;
      treeView.Nodes.Clear();

      foreach (TreeNode treeNode in nodes) {
        var item = (IItem)treeNode.Tag;
        treeView.ImageList.Images.Add(item == null ? HeuristicLab.Common.Resources.VSImageLibrary.Nothing : item.ItemImage);
        treeNode.ImageIndex = treeView.ImageList.Images.Count - 1;
        treeNode.SelectedImageIndex = treeNode.ImageIndex;
      }
      treeView.Nodes.AddRange(topLevelNodes);
      treeView.SelectedNode = selectedNode;
      treeView.EndUpdate();
    }
    #endregion


    public sealed class CustomTreeView : System.Windows.Forms.TreeView {
      protected override void WndProc(ref System.Windows.Forms.Message m) {
        const int WM_RBUTTONDOWN = 0x204;
        if (m.Msg == WM_RBUTTONDOWN) {
          //Raise your custom event right click event to prevent node highlighting
          OnRightClick();
          return;
        }
        base.WndProc(ref m);
      }

      public event EventHandler RightClick;
      private void OnRightClick() {
        var handler = RightClick;
        if (handler != null) RightClick(this, EventArgs.Empty);
      }
    }
  }
}
