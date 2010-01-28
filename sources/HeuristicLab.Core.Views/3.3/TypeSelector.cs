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
using System.Linq;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views { 
  public partial class TypeSelector : UserControl {
    protected Type baseType;
    public Type BaseType {
      get { return baseType; }
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
      selectedType = null;
    }

    public virtual void Configure(Type baseType, bool showNotInstantiableTypes, bool showGenericTypes) {
      if (baseType == null) throw new ArgumentNullException();
      if (InvokeRequired)
        Invoke(new Action<Type, bool, bool>(Configure), baseType, showNotInstantiableTypes, showGenericTypes);
      else {
        this.baseType = baseType;
        this.showNotInstantiableTypes = showNotInstantiableTypes;
        this.showGenericTypes = showGenericTypes;

        typesTreeView.Nodes.Clear();
        imageList.Images.Clear();
        imageList.Images.Add(HeuristicLab.Common.Resources.VS2008ImageLibrary.Class);      // default icon
        imageList.Images.Add(HeuristicLab.Common.Resources.VS2008ImageLibrary.Namespace);  // plugins
        imageList.Images.Add(HeuristicLab.Common.Resources.VS2008ImageLibrary.Interface);  // interfaces
        imageList.Images.Add(HeuristicLab.Common.Resources.VS2008ImageLibrary.OrgChart);   // abstract types
        imageList.Images.Add(HeuristicLab.Common.Resources.VS2008ImageLibrary.Template);   // generic types

        TreeNode selectedNode = null;
        var plugins = from p in ApplicationManager.Manager.Plugins
                      orderby p.Name, p.Version ascending
                      select p;
        foreach (IPluginDescription plugin in plugins) {
          TreeNode pluginNode = new TreeNode();
          pluginNode.Text = plugin.Name;
          pluginNode.ImageIndex = 1;
          pluginNode.SelectedImageIndex = pluginNode.ImageIndex;
          pluginNode.Tag = plugin;

          var types = from t in ApplicationManager.Manager.GetTypes(BaseType, plugin, false)
                      orderby t.Name ascending
                      select t;
          foreach (Type type in types) {
            bool valid = true;
            valid = valid && (ShowGenericTypes || !type.ContainsGenericParameters);
            valid = valid && (ShowNotInstantiableTypes || !type.IsAbstract);
            valid = valid && (ShowNotInstantiableTypes || !type.IsInterface);
            valid = valid && (ShowNotInstantiableTypes || !type.HasElementType);
            if (valid) {
              TreeNode typeNode = new TreeNode();
              string name = ItemAttribute.GetName(type);
              typeNode.Text = name != null ? name : type.Name;
              typeNode.ImageIndex = 0;
              if (type.IsInterface) typeNode.ImageIndex = 2;
              else if (type.IsAbstract) typeNode.ImageIndex = 3;
              else if (type.ContainsGenericParameters) typeNode.ImageIndex = 4;
              else if (imageList.Images.ContainsKey(type.FullName)) typeNode.ImageIndex = imageList.Images.IndexOfKey(type.FullName);
              else if (typeof(IItem).IsAssignableFrom(type)) {
                try {
                  IItem item = (IItem)Activator.CreateInstance(type);
                  imageList.Images.Add(type.FullName, item.ItemImage);
                  typeNode.ImageIndex = imageList.Images.IndexOfKey(type.FullName);
                }
                catch (Exception) {
                }
              }
              typeNode.SelectedImageIndex = typeNode.ImageIndex;
              typeNode.Tag = type;
              if (type == SelectedType) selectedNode = typeNode;
              pluginNode.Nodes.Add(typeNode);
            }
          }
          if (pluginNode.Nodes.Count > 0)
            typesTreeView.Nodes.Add(pluginNode);
        }
        if (typesTreeView.Nodes.Count == 0) {
          typesTreeView.Nodes.Add("No types of base type \"" + BaseType.Name + "\" found");
          typesTreeView.Enabled = false;
        }
        if (selectedNode != null) {
          typesTreeView.SelectedNode = selectedNode;
          selectedNode.EnsureVisible();
        } else {
          SelectedType = null;
        }
      }
    }

    public virtual object CreateInstanceOfSelectedType(params object[] args) {
      if (SelectedType != null) {
      try {
        return Activator.CreateInstance(SelectedType, args);
      } catch(Exception) { }
      }
      return null;
    }

    public event EventHandler SelectedTypeChanged;
    protected virtual void OnSelectedTypeChanged() {
      if (SelectedTypeChanged != null)
        SelectedTypeChanged(this, EventArgs.Empty);
    }

    protected virtual void UpdateDescription() {
      descriptionTextBox.Text = string.Empty;

      if (typesTreeView.SelectedNode != null) {
        IPluginDescription plugin = typesTreeView.SelectedNode.Tag as IPluginDescription;
        if (plugin != null) {
          StringBuilder sb = new StringBuilder();
          sb.Append("Plugin: ").Append(plugin.Name).Append(Environment.NewLine);
          sb.Append("Version: ").Append(plugin.Version.ToString()).Append(Environment.NewLine);
          sb.Append("Build Date: ").Append(plugin.BuildDate.ToString());
          descriptionTextBox.Text = sb.ToString();
        }
        Type type = typesTreeView.SelectedNode.Tag as Type;
        if (type != null) {
          string description = ItemAttribute.GetDescription(type);
          if (description != null)
            descriptionTextBox.Text = description;
        }
      }
    }

    protected virtual void typesTreeView_AfterSelect(object sender, TreeViewEventArgs e) {
      if (typesTreeView.SelectedNode != null) {
        SelectedType = typesTreeView.SelectedNode.Tag as Type;
      }
      UpdateDescription();
    }

    protected virtual void typesTreeView_ItemDrag(object sender, ItemDragEventArgs e) {
      TreeNode node = (TreeNode)e.Item;
      Type type = node.Tag as Type;
      if (type != null) {
        try {
          object o = Activator.CreateInstance(type);
          DataObject data = new DataObject();
          data.SetData("Type", type);
          data.SetData("Value", o);
          DoDragDrop(data, DragDropEffects.Copy);
        } catch (Exception) {
        }
      }
    }
  }
}
