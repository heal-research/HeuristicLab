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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.MainForm {
  public abstract partial class MainFormBase : Form, IMainForm {
    protected MainFormBase(Type userInterfaceItemType)
      : base() {
      InitializeComponent();
      openViews = new List<IView>();
      this.userInterfaceItemType = userInterfaceItemType;
      CreateGUI();
    }

    #region IMainForm Members
    public string Title {
      get { return this.Text; }
      set { this.Text = value; }
    }

    public string StatusStripText {
      get { return this.statusStripLabel.Text; }
      set { this.statusStripLabel.Text = value; }
    }

    protected Type userInterfaceItemType;
    public Type UserInterfaceItemType {
      get { return this.userInterfaceItemType; }
    }

    protected IView activeView;
    public IView ActiveView {
      get { return this.activeView; }
    }

    protected List<IView> openViews;
    public IEnumerable<IView> OpenViews {
      get { return openViews; }
    }

    public virtual void ShowView(IView view) {
      view.MainForm = this;
      activeView = view;
      openViews.Add(view);
    }
    #endregion

    #region create menu and toolbar
    private void CreateGUI() {
      DiscoveryService ds = new DiscoveryService();
      Type[] userInterfaceTypes = ds.GetTypes(userInterfaceItemType);

      object[] items = ds.GetInstances(userInterfaceItemType);
      IEnumerable<IToolStripItem> toolStripItems = items.Where(mi => mi as IToolStripMenuItem != null).Cast<IToolStripItem>();
      toolStripItems = toolStripItems.OrderBy(x => x.Position);
      foreach (IToolStripMenuItem menuItem in toolStripItems) {
        AddToolStripMenuItem(menuItem);
      }

      items = ds.GetInstances(userInterfaceItemType);
      toolStripItems = items.Where(mi => mi as IToolStripButtonItem != null).Cast<IToolStripItem>();
      toolStripItems = toolStripItems.OrderBy(x => x.Position);
      foreach (IToolStripButtonItem toolStripButtonItem in toolStripItems) {
        AddToolStripButtonItem(toolStripButtonItem);
      }
    }

    private void AddToolStripMenuItem(IToolStripMenuItem menuItem) {
      ToolStripMenuItem item = new ToolStripMenuItem();
      SetToolStripItemProperties(item, menuItem);    
      item.ShortcutKeys = menuItem.ShortCutKeys;

      ToolStripMenuItem parent = null;
      if (!String.IsNullOrEmpty(menuItem.MenuStructure)) {
        ToolStripItemCollection parentItems = menuStrip.Items;
        foreach (string structure in menuItem.MenuStructure.Split(menuItem.MenuStructureSeparator)) {
          if (parentItems.ContainsKey(structure))
            parent = (ToolStripMenuItem)parentItems[structure];
          else {
            parent = new ToolStripMenuItem(structure);
            parent.Name = structure;
            parentItems.Add(parent);
          }
          parentItems = parent.DropDownItems;
        }
      }

      if (parent == null)
        menuStrip.Items.Add(item);
      else
        parent.DropDownItems.Add(item);
    }

    private void AddToolStripButtonItem(IToolStripButtonItem buttonItem) {
      ToolStripButton item = new ToolStripButton();
      SetToolStripItemProperties(item, buttonItem);
      toolStrip.Items.Add(item);
    }

    private void SetToolStripItemProperties(ToolStripItem toolStripItem, IToolStripItem iToolStripItem) {
      toolStripItem.Text = iToolStripItem.Name;
      toolStripItem.Name = iToolStripItem.Name;
      toolStripItem.Tag = iToolStripItem;
      toolStripItem.Image = iToolStripItem.Image;
      toolStripItem.DisplayStyle = iToolStripItem.DisplayStyle;
      toolStripItem.Click += new EventHandler(ToolStripItemClicked);
      iToolStripItem.ToolStripItem = toolStripItem;
    }

    private void ToolStripItemClicked(object sender, EventArgs e) {
      System.Windows.Forms.ToolStripItem item = (System.Windows.Forms.ToolStripItem)sender;
      ((IAction)item.Tag).Execute(this);
    }
    #endregion
  }
}
