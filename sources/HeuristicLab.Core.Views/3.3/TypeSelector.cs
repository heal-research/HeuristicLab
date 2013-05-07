#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Core.Views {
  public partial class TypeSelector : UserControl {
    protected List<TreeNode> treeNodes;
    protected string currentSearchString;
    protected TypeSelectorDialog typeSelectorDialog;

    protected IEnumerable<Type> baseTypes;
    public IEnumerable<Type> BaseTypes {
      get { return baseTypes; }
    }
    protected bool showNotInstantiableTypes;
    public bool ShowNotInstantiableTypes {
      get { return showNotInstantiableTypes; }
    }
    protected bool showGenericTypes;
    public bool ShowGenericTypes {
      get { return showGenericTypes; }
    }
    public string Caption {
      get { return typesGroupBox.Text; }
      set {
        if (InvokeRequired)
          Invoke(new Action<string>(delegate(string s) { Caption = s; }), value);
        else
          typesGroupBox.Text = value;
      }
    }
    public TreeView TypesTreeView {
      get { return typesTreeView; }
    }
    protected Type selectedType;
    public Type SelectedType {
      get { return selectedType; }
      private set {
        if (value != selectedType) {
          selectedType = value;
          OnSelectedTypeChanged();
        }
      }
    }

    public TypeSelector() {
      InitializeComponent();
      treeNodes = new List<TreeNode>();
      currentSearchString = string.Empty;
      selectedType = null;
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    public virtual void Configure(Type baseType, bool showNotInstantiableTypes, bool showGenericTypes) {
      Configure(baseType, showNotInstantiableTypes, showGenericTypes, (t) => true);
    }

    public virtual void Configure(Type baseType, bool showNotInstantiableTypes, bool showGenericTypes, Func<Type, bool> typeCondition) {
      Configure(new List<Type>() { baseType }, showNotInstantiableTypes, showGenericTypes, true, typeCondition);
    }

    public virtual void Configure(IEnumerable<Type> baseTypes, bool showNotInstantiableTypes, bool showGenericTypes, bool assignableToAllTypes) {
      Configure(baseTypes, showNotInstantiableTypes, showGenericTypes, assignableToAllTypes, (t) => { return true; });
    }

    public virtual void Configure(IEnumerable<Type> baseTypes, bool showNotInstantiableTypes, bool showGenericTypes, bool assignableToAllTypes, Func<Type, bool> typeCondition) {
      if (baseTypes == null) throw new ArgumentNullException();
      if (InvokeRequired)
        Invoke(new Action<IEnumerable<Type>, bool, bool, bool, Func<Type, bool>>(Configure), baseTypes, showNotInstantiableTypes, showGenericTypes, assignableToAllTypes, typeCondition);
      else {
        this.baseTypes = baseTypes;
        this.showNotInstantiableTypes = showNotInstantiableTypes;
        this.showGenericTypes = showGenericTypes;
        bool selectedTypeFound = false;

        typeParametersSplitContainer.Panel2Collapsed = !showGenericTypes;

        TreeNode selectedNode = typesTreeView.SelectedNode;
        typesTreeView.Nodes.Clear();
        treeNodes.Clear();
        imageList.Images.Clear();
        imageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.Class);      // default icon
        imageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.Namespace);  // plugins
        imageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.Interface);  // interfaces
        imageList.Images.Add(HeuristicLab.Common.Resources.VSImageLibrary.Template);   // generic types

        var plugins = from p in ApplicationManager.Manager.Plugins
                      orderby p.Name, p.Version ascending
                      select p;
        foreach (IPluginDescription plugin in plugins) {
          TreeNode pluginNode = new TreeNode();
          pluginNode.Text = string.Format("{0} {1}.{2}", plugin.Name, plugin.Version.Major, plugin.Version.Minor);
          pluginNode.ImageIndex = 1;
          pluginNode.SelectedImageIndex = pluginNode.ImageIndex;
          pluginNode.Tag = plugin;

          var types = from t in ApplicationManager.Manager.GetTypes(BaseTypes, plugin, !ShowNotInstantiableTypes, ShowGenericTypes, assignableToAllTypes)
                      where typeCondition(t)
                      orderby t.Name ascending
                      select t;
          foreach (Type type in types) {
            bool valid = (ShowNotInstantiableTypes || type.GetConstructor(Type.EmptyTypes) != null); //check for public default ctor
            if (valid) {
              TreeNode typeNode = new TreeNode();
              string name = ItemAttribute.GetName(type);
              typeNode.Text = name != null ? name : type.GetPrettyName();
              typeNode.ImageIndex = 0;
              if (type.IsInterface) typeNode.ImageIndex = 2;
              else if (type.ContainsGenericParameters) typeNode.ImageIndex = 3;
              else if (imageList.Images.ContainsKey(type.FullName)) typeNode.ImageIndex = imageList.Images.IndexOfKey(type.FullName);
              else {
                var image = ItemAttribute.GetImage(type);
                if (image != null) {
                  imageList.Images.Add(type.FullName, image);
                  typeNode.ImageIndex = imageList.Images.IndexOfKey(type.FullName);
                }
              }
              typeNode.SelectedImageIndex = typeNode.ImageIndex;
              typeNode.Tag = type;
              pluginNode.Nodes.Add(typeNode);
              if (type.Equals(selectedType)) selectedTypeFound = true;
            }
          }
          if (pluginNode.Nodes.Count > 0)
            treeNodes.Add(pluginNode);
        }
        if (!selectedTypeFound) SelectedType = null;
        foreach (TreeNode node in treeNodes)
          typesTreeView.Nodes.Add((TreeNode)node.Clone());
        RestoreSelectedNode(selectedNode);
        Filter(searchTextBox.Text);

        UpdateTypeParameters();
      }
    }

    public virtual void Filter(string searchString) {
      if (InvokeRequired)
        Invoke(new Action<string>(Filter), searchString);
      else {
        searchString = searchString.ToLower();

        if (!searchString.Contains(currentSearchString)) {
          typesTreeView.BeginUpdate();
          // expand search -> restore all tree nodes
          TreeNode selectedNode = typesTreeView.SelectedNode;
          typesTreeView.Nodes.Clear();
          foreach (TreeNode node in treeNodes)
            typesTreeView.Nodes.Add((TreeNode)node.Clone());
          RestoreSelectedNode(selectedNode);
          typesTreeView.EndUpdate();
        }


        // remove nodes
        typesTreeView.BeginUpdate();
        int i = 0;
        while (i < typesTreeView.Nodes.Count) {
          int j = 0;
          while (j < typesTreeView.Nodes[i].Nodes.Count) {
            if (!typesTreeView.Nodes[i].Nodes[j].Text.ToLower().Contains(searchString)) {
              if ((typesTreeView.Nodes[i].Nodes[j].Tag as Type).Equals(selectedType))
                SelectedType = null;
              typesTreeView.Nodes[i].Nodes[j].Remove();
            } else
              j++;
          }
          if (typesTreeView.Nodes[i].Nodes.Count == 0)
            typesTreeView.Nodes[i].Remove();
          else
            i++;
        }
        typesTreeView.EndUpdate();
        currentSearchString = searchString;

        // if there is just one type node left, select by default
        if (typesTreeView.Nodes.Count == 1) {
          if (typesTreeView.Nodes[0].Nodes.Count == 1) {
            typesTreeView.SelectedNode = typesTreeView.Nodes[0].Nodes[0];
          }
        }

        if (typesTreeView.Nodes.Count == 0) {
          SelectedType = null;
          typesTreeView.Enabled = false;
        } else {
          SetTreeNodeVisibility();
          typesTreeView.Enabled = true;
        }
        UpdateDescription();
      }
    }

    public virtual object CreateInstanceOfSelectedType(params object[] args) {
      if (SelectedType == null)
        throw new InvalidOperationException("No type selected.");
      else
        return Activator.CreateInstance(SelectedType, args);
    }

    protected virtual void UpdateTypeParameters() {
      typeParametersListView.Items.Clear();
      if ((SelectedType == null) || !SelectedType.ContainsGenericParameters) {
        typeParametersGroupBox.Enabled = false;
        typeParametersSplitContainer.Panel2Collapsed = true;
      } else {
        typeParametersGroupBox.Enabled = true;
        typeParametersSplitContainer.Panel2Collapsed = false;
        setTypeParameterButton.Enabled = false;

        foreach (Type param in SelectedType.GetGenericArguments()) {
          if (param.IsGenericParameter) {
            ListViewItem item = new ListViewItem();
            item.Text = param.Name;

            item.ToolTipText = "Constraints:";
            Type[] constraints = param.GetGenericParameterConstraints();
            if (constraints.Length == 0) {
              item.ToolTipText += " none";
            } else {
              foreach (Type constraint in constraints)
                item.ToolTipText += " " + constraint.GetPrettyName();
            }

            item.Tag = param;
            typeParametersListView.Items.Add(item);
          }
        }
        typeParametersListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      }
    }

    protected virtual void SetTypeParameter() {
      if (typeSelectorDialog == null) {
        typeSelectorDialog = new TypeSelectorDialog();
        typeSelectorDialog.Caption = "Select Type of Generic Type Parameter";
      }
      Type param = typeParametersListView.SelectedItems[0].Tag as Type;
      Type[] constraints = param.GetGenericParameterConstraints();
      bool showNotInstantiableTypes = !param.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint);
      typeSelectorDialog.TypeSelector.Configure(constraints, showNotInstantiableTypes, true, true);

      if (typeSelectorDialog.ShowDialog(this) == DialogResult.OK) {
        Type selected = typeSelectorDialog.TypeSelector.SelectedType;
        Type[] parameters = SelectedType.GetGenericArguments();
        parameters[param.GenericParameterPosition] = selected;
        SelectedType = SelectedType.GetGenericTypeDefinition().MakeGenericType(parameters);

        typeParametersListView.SelectedItems[0].Text = param.Name + ": " + selected.GetPrettyName();
        typeParametersListView.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      }
    }

    protected virtual void UpdateDescription() {
      descriptionTextBox.Text = string.Empty;

      if (typesTreeView.SelectedNode != null) {
        IPluginDescription plugin = typesTreeView.SelectedNode.Tag as IPluginDescription;
        if (plugin != null) {
          StringBuilder sb = new StringBuilder();
          sb.Append("Plugin: ").Append(plugin.Name).Append(Environment.NewLine);
          sb.Append("Version: ").Append(plugin.Version.ToString()).Append(Environment.NewLine);
          descriptionTextBox.Text = sb.ToString();
        }
        Type type = typesTreeView.SelectedNode.Tag as Type;
        if (type != null) {
          string description = ItemAttribute.GetDescription(type);
          if (description != null)
            descriptionTextBox.Text = description;
        }
      } else if (typesTreeView.Nodes.Count == 0) {
        descriptionTextBox.Text = "No types found";
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
    protected virtual void searchTextBox_TextChanged(object sender, System.EventArgs e) {
      Filter(searchTextBox.Text);
    }

    protected virtual void typesTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      if (typesTreeView.SelectedNode == null) SelectedType = null;
      else SelectedType = typesTreeView.SelectedNode.Tag as Type;
      UpdateTypeParameters();
      UpdateDescription();
    }
    protected virtual void typesTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      TreeNode node = (TreeNode)e.Item;
      Type type = node.Tag as Type;
      if ((type != null) && (!type.IsInterface) && (!type.IsAbstract) && (!type.HasElementType) && (!type.ContainsGenericParameters)) {
        object o = Activator.CreateInstance(type);
        DataObject data = new DataObject();
        data.SetData(HeuristicLab.Common.Constants.DragDropDataFormat, o);
        DoDragDrop(data, DragDropEffects.Copy);
      }
    }
    protected virtual void typesTreeView_VisibleChanged(object sender, EventArgs e) {
      if (Visible) SetTreeNodeVisibility();
    }

    protected virtual void typeParametersListView_SelectedIndexChanged(object sender, EventArgs e) {
      setTypeParameterButton.Enabled = typeParametersListView.SelectedItems.Count == 1;
    }
    protected virtual void typeParametersListView_DoubleClick(object sender, EventArgs e) {
      if (typeParametersListView.SelectedItems.Count == 1)
        SetTypeParameter();
    }

    protected virtual void setTypeParameterButton_Click(object sender, EventArgs e) {
      SetTypeParameter();
    }
    #endregion

    #region Helpers
    private void RestoreSelectedNode(TreeNode selectedNode) {
      if (selectedNode != null) {
        foreach (TreeNode node in typesTreeView.Nodes) {
          if (node.Text.Equals(selectedNode.Text)) typesTreeView.SelectedNode = node;
          foreach (TreeNode child in node.Nodes) {
            if ((child.Text.Equals(selectedNode.Text)) && (child.Tag == selectedNode.Tag))
              typesTreeView.SelectedNode = child;
          }
        }
        if (typesTreeView.SelectedNode == null) SelectedType = null;
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
  }
}
