#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.JobManager.ExtensionMethods;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;
using HeuristicLab.MainForm.WindowsForms;

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  [View("Hive Resource Selector View")]
  [Content(typeof(IItemList<Resource>), true)]
  public partial class HiveResourceSelector : ItemView, IDisposable {
    private const int slaveImageIndex = 0;
    private const int slaveGroupImageIndex = 1;
    private string currentSearchString;
    private ISet<TreeNode> mainTreeNodes;
    private ISet<TreeNode> filteredTreeNodes;
    private ISet<TreeNode> nodeStore;
    
    private ISet<Resource> selectedResources;
    public ISet<Resource> SelectedResources {
      get { return selectedResources; }
      set { selectedResources = value; }
    }

    public new IItemList<Resource> Content {
      get { return (IItemList<Resource>)base.Content; }
      set { base.Content = value; }
    }

    public HiveResourceSelector() {
      InitializeComponent();
      mainTreeNodes = new HashSet<TreeNode>();
      filteredTreeNodes = new HashSet<TreeNode>();
      nodeStore = new HashSet<TreeNode>();
      selectedResources = new HashSet<Resource>();
      imageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.MonitorLarge);
      imageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.NetworkCenterLarge);
    }
  
    public void StartProgressView() {
      if (InvokeRequired) {
        Invoke(new Action(StartProgressView));
      } else {
        var message = "Downloading resources. Please be patient.";
        MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().AddOperationProgressToView(this, message);
      }
    }

    public void FinishProgressView() {
      if (InvokeRequired) {
        Invoke(new Action(FinishProgressView));
      } else {
        MainFormManager.GetMainForm<HeuristicLab.MainForm.WindowsForms.MainForm>().RemoveOperationProgressFromView(this);
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();

      if (Content != null) {
        selectedResources = new HashSet<Resource>(Content.Where(x => selectedResources.Any(y => x.Id == y.Id)));
        UpdateMainTree();
        ExtractStatistics();
      } else {
        mainTreeNodes.Clear();
        UpdateFilteredTree();
      }
    }

    #region MainTree Methods
    private void UpdateMainTree() {
      mainTreeNodes.Clear();

      foreach (Resource g in Content) {
        if (g.GetType() == typeof(SlaveGroup)) {
          //root node
          if (g.ParentResourceId == null) {
            TreeNode tn = new TreeNode();
            tn.ImageIndex = slaveGroupImageIndex;
            tn.SelectedImageIndex = tn.ImageIndex;

            tn.Tag = g;
            tn.Text = g.Name;
            tn.Checked = selectedResources.Any(x => x.Id == g.Id);

            BuildMainTree(tn);
            mainTreeNodes.Add(tn);
          }
        }
      }
      UpdateFilteredTree();
    }

    private void BuildMainTree(TreeNode tn) {
      foreach (Resource r in Content.Where(s => s.ParentResourceId != null && s.ParentResourceId == ((Resource)tn.Tag).Id)) {
        TreeNode stn = new TreeNode(r.Name);
        if (r is Slave) stn.ImageIndex = slaveImageIndex;
        else if (r is SlaveGroup) stn.ImageIndex = slaveGroupImageIndex;
        stn.SelectedImageIndex = stn.ImageIndex;
        stn.Tag = r;
        stn.Checked = selectedResources.Any(x => x.Id == r.Id);
        tn.Nodes.Add(stn);
        mainTreeNodes.Add(stn);

        BuildMainTree(stn);
      }
    }
    #endregion

    #region FilteredTree Methods
    private void UpdateFilteredTree() {
      filteredTreeNodes.Clear();
      foreach (TreeNode n in mainTreeNodes) {
        n.BackColor = SystemColors.Window;
        if (currentSearchString == null || ((Resource)n.Tag).Name.ToLower().Contains(currentSearchString)) {
          n.BackColor = string.IsNullOrEmpty(currentSearchString) ? SystemColors.Window : Color.LightBlue;
          filteredTreeNodes.Add(n);
          TraverseParentNodes(n);
        }
      }
      UpdateResourceTree();
    }

    private void TraverseParentNodes(TreeNode node) {
      if (node != null) {
        for (TreeNode parent = node.Parent; parent != null; parent = parent.Parent)
          filteredTreeNodes.Add(parent);
      }
    }
    #endregion

    #region ResourceTree Methods
    private void UpdateResourceTree() {
      resourcesTreeView.Nodes.Clear();
      nodeStore.Clear();

      foreach (TreeNode node in filteredTreeNodes) {
        var clone = nodeStore.SingleOrDefault(x => ((Resource)x.Tag).Id == ((Resource)node.Tag).Id);
        if (clone == null) {
          clone = (TreeNode)node.Clone();
          nodeStore.Add(clone);
          clone.Nodes.Clear();
        }
        foreach (TreeNode child in node.Nodes)
          if (filteredTreeNodes.Any(x => ((Resource)x.Tag).Id == ((Resource)child.Tag).Id)) {
            var childClone = nodeStore.SingleOrDefault(x => ((Resource)x.Tag).Id == ((Resource)child.Tag).Id);
            if (childClone == null) {
              childClone = (TreeNode)child.Clone();
              nodeStore.Add(childClone);
              childClone.Nodes.Clear();
            }
            clone.Nodes.Add(childClone);
          }
      }
      resourcesTreeView.Nodes.AddRange(nodeStore.Where(x => ((Resource)x.Tag).ParentResourceId == null).ToArray());
      if (string.IsNullOrEmpty(currentSearchString)) ExpandSlaveGroupNodes();
      else resourcesTreeView.ExpandAll();
    }
    #endregion

    #region Events
    private void resourcesTreeView_AfterCheck(object sender, TreeViewEventArgs e) {
      if (e.Action != TreeViewAction.Unknown) {
        if (e.Node.Checked) {
          IncludeChildNodes(mainTreeNodes.SingleOrDefault(x => ((Resource)x.Tag).Id == ((Resource)e.Node.Tag).Id));
          IncludeParentNodes(mainTreeNodes.SingleOrDefault(x => ((Resource)x.Tag).Id == ((Resource)e.Node.Tag).Id));
        } else {
          ExcludeChildNodes(mainTreeNodes.SingleOrDefault(x => ((Resource)x.Tag).Id == ((Resource)e.Node.Tag).Id));
          ExcludeParentNodes(mainTreeNodes.SingleOrDefault(x => ((Resource)x.Tag).Id == ((Resource)e.Node.Tag).Id));
        }
        ExtractStatistics();
      }
    }

    private void resourcesTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      if (e.Action != TreeViewAction.Unknown) {
        ExtractStatistics(e.Node);
      }
    }

    private void searchTextBox_TextChanged(object sender, System.EventArgs e) {
      currentSearchString = searchTextBox.Text.ToLower();
      UpdateFilteredTree();
    }
    #endregion

    #region Helpers
    private void IncludeChildNodes(TreeNode node) {
      if (node != null) {
        node.Checked = true;
        selectedResources.Add((Resource)node.Tag);
        AdjustNodeCheckedState(node);
        foreach (TreeNode n in node.Nodes) IncludeChildNodes(n);
      }
    }

    private void IncludeParentNodes(TreeNode node) {
      if (node != null && node.Parent != null) {
        TreeNode parent = node.Parent;
        if (parent.Nodes.OfType<TreeNode>().All(x => x.Checked)) {
          parent.Checked = true;
          selectedResources.Add((Resource)parent.Tag);
          AdjustNodeCheckedState(parent);
          IncludeParentNodes(parent);
        }
      }
    }

    private void ExcludeChildNodes(TreeNode node) {
      if (node != null) {
        node.Checked = false;
        selectedResources.Remove((Resource)node.Tag);
        AdjustNodeCheckedState(node);
        foreach (TreeNode n in node.Nodes) ExcludeChildNodes(n);
      }
    }

    private void ExcludeParentNodes(TreeNode node) {
      if (node != null) {
        node.Checked = false;
        selectedResources.Remove((Resource)node.Tag);
        AdjustNodeCheckedState(node);
        ExcludeParentNodes(node.Parent);
      }
    }

    private void AdjustNodeCheckedState(TreeNode node) {
      var filterdNode = filteredTreeNodes.SingleOrDefault(x => ((Resource)x.Tag).Id == ((Resource)node.Tag).Id);
      var storedNode = nodeStore.SingleOrDefault(x => ((Resource)x.Tag).Id == ((Resource)node.Tag).Id);
      if (filterdNode != null) filterdNode.Checked = node.Checked;
      if (storedNode != null) storedNode.Checked = node.Checked;
    }

    private void ExpandSlaveGroupNodes() {
      foreach (TreeNode n in nodeStore.Where(x => x.Tag is SlaveGroup)) {
        TreeNode[] children = new TreeNode[n.Nodes.Count];
        n.Nodes.CopyTo(children, 0);
        if (children.Any(x => x.Tag is SlaveGroup)) n.Expand();
      }
    }

    private void ExtractStatistics(TreeNode treeNode = null) {
      StringBuilder sb = new StringBuilder();
      Resource resource = treeNode == null ? null : treeNode.Tag as Resource;
      ISet<Resource> resources = treeNode == null ? selectedResources : new HashSet<Resource>(treeNode.DescendantNodes().Select(x => x.Tag as Resource)); ;
      IEnumerable<SlaveGroup> slaveGroups = resources.OfType<SlaveGroup>();
      IEnumerable<Slave> slaves = resources.OfType<Slave>();
      int cpuSpeed = 0, cores = 0, freeCores = 0, memory = 0, freeMemory = 0;
      string contextString = treeNode == null ? "Selected" : "Included";

      if (resources.Any() || resource != null) {
        foreach (Slave s in slaves) {
          cpuSpeed += s.CpuSpeed.GetValueOrDefault();
          cores += s.Cores.GetValueOrDefault();
          freeCores += s.FreeCores.GetValueOrDefault();
          memory += s.Memory.GetValueOrDefault();
          freeMemory += s.FreeMemory.GetValueOrDefault();
        }
        if (resource != null) {
          if (resource is SlaveGroup) sb.Append("Slave group: ");
          else if (resource is Slave) {
            sb.Append("Slave: ");
            if (!resources.Any()) {
              Slave s = resource as Slave;
              cpuSpeed = s.CpuSpeed.GetValueOrDefault();
              cores = s.Cores.GetValueOrDefault();
              freeCores = s.FreeCores.GetValueOrDefault();
              memory = s.Memory.GetValueOrDefault();
              freeMemory = s.FreeMemory.GetValueOrDefault();
            }
          }
          sb.AppendLine(string.Format("{0}", resource.Name));
        }
        if (resource == null || resource is SlaveGroup) {
          if (resources.Any()) {
            sb.AppendFormat("{0} slave groups ({1}): ", contextString, slaveGroups.Count());
            foreach (SlaveGroup sg in slaveGroups) sb.AppendFormat("{0}; ", sg.Name);
            sb.AppendLine();
            sb.AppendFormat("{0} slaves ({1}): ", contextString, slaves.Count());
            foreach (Slave s in slaves) sb.AppendFormat("{0}; ", s.Name);
            sb.AppendLine();
          } else {
            sb.Append("The selection does not inlcude any further resources.");
          }
        }
        sb.AppendLine();
        sb.AppendLine(string.Format("CPU speed: {0} MHz", cpuSpeed));
        if (resources.Any()) sb.AppendLine(string.Format("Avg. CPU speed: {0:0.00} MHz", (double)cpuSpeed / resources.Count()));
        sb.AppendLine(string.Format("Cores: {0}", cores));
        sb.AppendLine(string.Format("Free cores: {0}", freeCores));
        if (resources.Any()) sb.AppendLine(string.Format("Avg. free cores: {0:0.00}", (double)freeCores / resources.Count()));
        sb.AppendLine(string.Format("Memory: {0} MB", memory));
        sb.AppendFormat("Free memory: {0} MB", freeMemory);
        if (resources.Any()) sb.Append(string.Format("{0}Avg. free memory: {1:0.00} MB", Environment.NewLine, (double)freeMemory / resources.Count()));
      } else {
        sb.Append("No resources selected.");
      }

      descriptionTextBox.Text = sb.ToString();
    }
    #endregion
  }
}