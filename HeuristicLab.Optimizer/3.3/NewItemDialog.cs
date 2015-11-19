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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Common.Resources;
using HeuristicLab.Core;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Optimizer {
  internal partial class NewItemDialog : Form {
    private bool isInitialized;

    private readonly List<TreeNode> treeNodes;
    private string currentSearchString;

    private Type selectedType;
    public Type SelectedType {
      get { return selectedType; }
      private set {
        if (value != selectedType) {
          selectedType = value;
          OnSelectedTypeChanged();
        }
      }
    }

    private IItem item;
    public IItem Item {
      get { return item; }
    }

    public NewItemDialog() {
      InitializeComponent();
      treeNodes = new List<TreeNode>();
      currentSearchString = string.Empty;
      item = null;
      SelectedTypeChanged += this_SelectedTypeChanged;
    }

    private void NewItemDialog_Load(object sender, EventArgs e) {
      if (isInitialized) return;

      // Sorted by hasOrdering to create category nodes first with concrete ordering.
      // Items with categoryname without ordering are inserted afterwards correctly
      var categories =
        from type in ApplicationManager.Manager.GetTypes(typeof(IItem))
        where CreatableAttribute.IsCreatable(type)
        let category = CreatableAttribute.GetCategory(type)
        let hasOrdering = category.Contains(CreatableAttribute.Categories.OrderToken)
        let name = ItemAttribute.GetName(type)
        let priority = CreatableAttribute.GetPriority(type)
        let version = ItemAttribute.GetVersion(type)
        orderby category, hasOrdering descending, priority, name, version ascending
        group type by category into categoryGroup
        select categoryGroup;

      var rootNode = CreateCategoryTree(categories);
      CreateItemNodes(rootNode, categories);

      foreach (TreeNode topNode in rootNode.Nodes)
        treeNodes.Add(topNode);
      foreach (var node in treeNodes)
        typesTreeView.Nodes.Add((TreeNode)node.Clone());

      typesTreeView.TreeViewNodeSorter = new ItemTreeNodeComparer();
      typesTreeView.Sort();

      isInitialized = true;
    }

    private TreeNode CreateCategoryTree(IEnumerable<IGrouping<string, Type>> categories) {
      imageList.Images.Add(VSImageLibrary.Class);      // default icon
      imageList.Images.Add(VSImageLibrary.Namespace);  // plugins

      var rootNode = new TreeNode();

      // CategoryNode
      // Tag: raw string, used for sorting, e.g. 1$$$Algorithms###2$$$Single Solution
      // Name: full name = combined category name with parent categories, used for finding nodes in tree, e.g. Algorithms###Single Solution
      // Text: category name, used for displaying on node itself, e.g. Single Solution

      foreach (var category in categories) {
        var rawName = category.Key;
        string fullName = CreatableAttribute.Categories.GetFullName(rawName);
        string name = CreatableAttribute.Categories.GetName(rawName);

        // Skip categories with same full name because the raw name can still be different (missing order)
        if (rootNode.Nodes.Find(fullName, true).Length > 0)
          continue;

        var categoryNode = new TreeNode(name, 1, 1) {
          Name = fullName,
          Tag = rawName
        };

        var parents = CreatableAttribute.Categories.GetParentRawNames(rawName);
        var parentNode = FindOrCreateParentNode(rootNode, parents);
        if (parentNode != null)
          parentNode.Nodes.Add(categoryNode);
        else
          rootNode.Nodes.Add(categoryNode);
      }

      return rootNode;
    }
    private TreeNode FindOrCreateParentNode(TreeNode node, IEnumerable<string> rawParentNames) {
      TreeNode parentNode = null;
      string rawName = null;
      foreach (string rawParentName in rawParentNames) {
        rawName = rawName == null ? rawParentName : rawName + CreatableAttribute.Categories.SplitToken + rawParentName;
        var fullName = CreatableAttribute.Categories.GetFullName(rawName);
        parentNode = node.Nodes.Find(fullName, false).SingleOrDefault();
        if (parentNode == null) {
          var name = CreatableAttribute.Categories.GetName(rawName);
          parentNode = new TreeNode(name, 1, 1) {
            Name = fullName,
            Tag = rawName
          };
          node.Nodes.Add(parentNode);
        }
        node = parentNode;
      }
      return parentNode;
    }
    private void CreateItemNodes(TreeNode node, IEnumerable<IGrouping<string, Type>> categories) {
      foreach (var category in categories) {
        var fullName = CreatableAttribute.Categories.GetFullName(category.Key);
        var categoryNode = node.Nodes.Find(fullName, true).Single();
        foreach (var creatable in category) {
          var itemNode = CreateItemNode(creatable);
          itemNode.Name = itemNode.Name + ":" + fullName;
          categoryNode.Nodes.Add(itemNode);
        }
      }
    }
    private TreeNode CreateItemNode(Type creatable) {
      string name = ItemAttribute.GetName(creatable);

      var itemNode = new TreeNode(name) {
        ImageIndex = 0,
        Tag = creatable,
        Name = name
      };

      var image = ItemAttribute.GetImage(creatable);
      if (image != null) {
        imageList.Images.Add(image);
        itemNode.ImageIndex = imageList.Images.Count - 1;
      }
      itemNode.SelectedImageIndex = itemNode.ImageIndex;

      return itemNode;
    }

    private void NewItemDialog_Shown(object sender, EventArgs e) {
      searchTextBox.Text = string.Empty;
      searchTextBox.Focus();
      SelectedType = null;
      typesTreeView.SelectedNode = null;
      UpdateDescription();

      foreach (TreeNode node in typesTreeView.Nodes)
        node.Expand();
      typesTreeView.Nodes[0].EnsureVisible();
    }

    public virtual void Filter(string searchString) {
      if (InvokeRequired) {
        Invoke(new Action<string>(Filter), searchString);
      } else {
        searchString = searchString.ToLower();
        var selectedNode = typesTreeView.SelectedNode;

        if (!searchString.Contains(currentSearchString)) {
          typesTreeView.BeginUpdate();
          // expand search -> restore all tree nodes
          typesTreeView.Nodes.Clear();
          foreach (TreeNode node in treeNodes)
            typesTreeView.Nodes.Add((TreeNode)node.Clone());
          typesTreeView.EndUpdate();
        }

        // remove nodes
        typesTreeView.BeginUpdate();
        var searchTokens = searchString.Split(' ');
        int i = 0;
        while (i < typesTreeView.Nodes.Count) {
          var categoryNode = typesTreeView.Nodes[i];
          bool remove = FilterNode(categoryNode, searchTokens);
          if (remove)
            typesTreeView.Nodes.RemoveAt(i);
          else i++;
        }
        typesTreeView.EndUpdate();
        currentSearchString = searchString;

        // select first item
        typesTreeView.BeginUpdate();
        var firstNode = FirstVisibleNode;
        while (firstNode != null && !(firstNode.Tag is Type))
          firstNode = firstNode.NextVisibleNode;
        if (firstNode != null)
          typesTreeView.SelectedNode = firstNode;

        if (typesTreeView.Nodes.Count == 0) {
          SelectedType = null;
          typesTreeView.Enabled = false;
        } else {
          SetTreeNodeVisibility();
          typesTreeView.Enabled = true;
        }
        typesTreeView.EndUpdate();

        RestoreSelectedNode(selectedNode);
        UpdateDescription();
      }
    }

    private bool FilterNode(TreeNode node, string[] searchTokens) {
      if (node.Tag is string) { // Category node
        int i = 0;
        while (i < node.Nodes.Count) {
          bool remove = FilterNode(node.Nodes[i], searchTokens);
          if (remove)
            node.Nodes.RemoveAt(i);
          else i++;
        }
        return node.Nodes.Count == 0;
      } if (node.Tag is Type) { // Type node
        var text = node.Text;
        if (searchTokens.Any(searchToken => !text.ToLower().Contains(searchToken))) {
          var typeTag = (Type)node.Tag;
          if (typeTag == SelectedType) {
            SelectedType = null;
            typesTreeView.SelectedNode = null;
          }
          return true;
        }
        return false;
      }
      throw new InvalidOperationException("Encountered neither a category nor a creatable node during tree traversal.");
    }

    protected virtual void UpdateDescription() {
      itemDescriptionTextBox.Text = string.Empty;
      pluginDescriptionTextBox.Text = string.Empty;
      pluginTextBox.Text = string.Empty;
      versionTextBox.Text = string.Empty;

      if (typesTreeView.SelectedNode != null) {
        var node = typesTreeView.SelectedNode;
        string category = node.Tag as string;
        if (category != null) {
          itemDescriptionTextBox.Text = string.Join(" - ", node.Name.Split(new[] { CreatableAttribute.Categories.SplitToken }, StringSplitOptions.RemoveEmptyEntries));
        }
        Type type = node.Tag as Type;
        if (type != null) {
          string description = ItemAttribute.GetDescription(type);
          var version = ItemAttribute.GetVersion(type);
          var plugin = ApplicationManager.Manager.GetDeclaringPlugin(type);
          if (description != null)
            itemDescriptionTextBox.Text = description;
          if (plugin != null) {
            pluginTextBox.Text = plugin.Name;
            pluginDescriptionTextBox.Text = plugin.Description;
          }
          if (version != null)
            versionTextBox.Text = version.ToString();
        }
      } else if (typesTreeView.Nodes.Count == 0) {
        itemDescriptionTextBox.Text = "No types found";
      }
    }

    #region Events
    public event EventHandler SelectedTypeChanged;
    protected virtual void OnSelectedTypeChanged() {
      if (SelectedTypeChanged != null)
        SelectedTypeChanged(this, EventArgs.Empty);
    }
    #endregion

    #region Control Events
    protected virtual void searchTextBox_TextChanged(object sender, EventArgs e) {
      Filter(searchTextBox.Text);
    }

    protected virtual void itemsTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      if (typesTreeView.SelectedNode == null) SelectedType = null;
      else SelectedType = typesTreeView.SelectedNode.Tag as Type;
      UpdateDescription();
    }

    protected virtual void itemsTreeView_VisibleChanged(object sender, EventArgs e) {
      if (Visible) SetTreeNodeVisibility();
    }
    #endregion

    #region Helpers
    private void RestoreSelectedNode(TreeNode selectedNode) {
      if (selectedNode != null) {
        var node = typesTreeView.Nodes.Find(selectedNode.Name, true).SingleOrDefault();
        if (node != null)
          typesTreeView.SelectedNode = node;
        if (typesTreeView.SelectedNode == null)
          SelectedType = null;
      }
    }
    private void SetTreeNodeVisibility() {
      TreeNode selectedNode = typesTreeView.SelectedNode;
      if (string.IsNullOrEmpty(currentSearchString) && (typesTreeView.Nodes.Count > 1)) {
        typesTreeView.CollapseAll();
        if (selectedNode != null) typesTreeView.SelectedNode = selectedNode;
      } else {
        typesTreeView.ExpandAll();
      }
      if (selectedNode != null) selectedNode.EnsureVisible();
    }
    #endregion

    private void okButton_Click(object sender, EventArgs e) {
      if (SelectedType != null) {
        item = (IItem)Activator.CreateInstance(SelectedType);
        DialogResult = DialogResult.OK;
        Close();
      }
    }
    private void itemTreeView_DoubleClick(object sender, EventArgs e) {
      if (SelectedType != null) {
        item = (IItem)Activator.CreateInstance(SelectedType);
        DialogResult = DialogResult.OK;
        Close();
      }
    }
    private void this_SelectedTypeChanged(object sender, EventArgs e) {
      okButton.Enabled = SelectedType != null;
    }

    private TreeNode toolStripMenuNode;
    private void typesTreeView_MouseDown(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Right) {
        Point coordinates = typesTreeView.PointToClient(Cursor.Position);
        toolStripMenuNode = typesTreeView.GetNodeAt(coordinates);

        if (toolStripMenuNode != null && coordinates.X >= toolStripMenuNode.Bounds.Left &&
            coordinates.X <= toolStripMenuNode.Bounds.Right) {
          typesTreeView.SelectedNode = toolStripMenuNode;

          expandToolStripMenuItem.Enabled =
            expandToolStripMenuItem.Visible = !toolStripMenuNode.IsExpanded && toolStripMenuNode.Nodes.Count > 0;
          collapseToolStripMenuItem.Enabled = collapseToolStripMenuItem.Visible = toolStripMenuNode.IsExpanded;
        } else {
          expandToolStripMenuItem.Enabled = expandToolStripMenuItem.Visible = false;
          collapseToolStripMenuItem.Enabled = collapseToolStripMenuItem.Visible = false;
        }
        expandAllToolStripMenuItem.Enabled =
          expandAllToolStripMenuItem.Visible =
            !typesTreeView.Nodes.OfType<TreeNode>().All(x => TreeNodeIsFullyExpanded(x));
        collapseAllToolStripMenuItem.Enabled =
          collapseAllToolStripMenuItem.Visible = typesTreeView.Nodes.OfType<TreeNode>().Any(x => x.IsExpanded);
        if (contextMenuStrip.Items.Cast<ToolStripMenuItem>().Any(item => item.Enabled))
          contextMenuStrip.Show(Cursor.Position);
      }
    }
    private bool TreeNodeIsFullyExpanded(TreeNode node) {
      return (node.Nodes.Count == 0) || (node.IsExpanded && node.Nodes.OfType<TreeNode>().All(x => TreeNodeIsFullyExpanded(x)));
    }

    private void expandToolStripMenuItem_Click(object sender, EventArgs e) {
      typesTreeView.BeginUpdate();
      if (toolStripMenuNode != null) toolStripMenuNode.ExpandAll();
      typesTreeView.EndUpdate();
    }
    private void expandAllToolStripMenuItem_Click(object sender, EventArgs e) {
      typesTreeView.BeginUpdate();
      typesTreeView.ExpandAll();
      typesTreeView.EndUpdate();
    }
    private void collapseToolStripMenuItem_Click(object sender, EventArgs e) {
      typesTreeView.BeginUpdate();
      if (toolStripMenuNode != null) toolStripMenuNode.Collapse();
      typesTreeView.EndUpdate();
    }
    private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e) {
      typesTreeView.BeginUpdate();
      typesTreeView.CollapseAll();
      typesTreeView.EndUpdate();
    }

    private void clearSearchButton_Click(object sender, EventArgs e) {
      searchTextBox.Text = string.Empty;
      searchTextBox.Focus();
    }

    private void searchTextBox_KeyDown(object sender, KeyEventArgs e) {
      if (typesTreeView.Nodes.Count == 0)
        return;

      if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) {
        var selectedNode = typesTreeView.SelectedNode;

        if (selectedNode == null) { // nothing selected => select first
          if (e.KeyCode == Keys.Down) typesTreeView.SelectedNode = FirstVisibleNode;
        } else {
          if (e.KeyCode == Keys.Down && selectedNode.NextVisibleNode != null)
            typesTreeView.SelectedNode = selectedNode.NextVisibleNode;
          if (e.KeyCode == Keys.Up && selectedNode.PrevVisibleNode != null)
            typesTreeView.SelectedNode = selectedNode.PrevVisibleNode;
        }
        e.Handled = true;
      }
    }

    private TreeNode FirstVisibleNode {
      get {
        return typesTreeView.Nodes.Count > 0 ? typesTreeView.Nodes[0] : null;
      }
    }

    private class ItemTreeNodeComparer : IComparer {
      private static readonly IComparer<string> Comparer = new NaturalStringComparer();
      public int Compare(object x, object y) {
        var lhs = (TreeNode)x;
        var rhs = (TreeNode)y;

        if (lhs.Tag is string && rhs.Tag is string) {
          return Comparer.Compare((string)lhs.Tag, (string)rhs.Tag);
        }
        if (lhs.Tag is string) {
          return -1;
        }
        return 1;
      }
    }
  }
}
