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
      views = new List<IView>();
      viewChangeToolStripItems = new List<IToolStripItem>();
      this.userInterfaceItemType = userInterfaceItemType;
      CreateGUI();
      OnActiveViewChanged();
    }

    #region IMainForm Members
    public string Title {
      get { return this.Text; }
      set {
        if (InvokeRequired) {
          Action<string> action = delegate(string s) { this.Title = s; };
          Invoke(action, new object[] { value });
        } else
          this.Text = value;
      }
    }

    public string StatusStripText {
      get { return this.statusStripLabel.Text; }
      set {
        if (InvokeRequired) {
          Action<string> action = delegate(string s) { this.statusStripLabel.Text = s; };
          Invoke(action, new object[] { value });
        } else
          this.statusStripLabel.Text = value;
      }
    }

    private Type userInterfaceItemType;
    public Type UserInterfaceItemType {
      get { return this.userInterfaceItemType; }
      protected set { this.userInterfaceItemType = value; }
    }

    private IView activeView;
    public IView ActiveView {
      get { return this.activeView; }
      protected set {
        if (this.activeView != value) {
          this.activeView = value;
          OnActiveViewChanged();
        }
      }
    }

    private List<IToolStripItem> viewChangeToolStripItems;
    protected IEnumerable<IToolStripItem> ViewChangedToolStripItems {
      get { return this.viewChangeToolStripItems; }
    }

    public event EventHandler ActiveViewChanged;
    protected virtual void OnActiveViewChanged() {
      if (ActiveViewChanged != null)
        ActiveViewChanged(this, new EventArgs());
    }

    protected List<IView> views;
    public IEnumerable<IView> Views {
      get { return views; }
    }

    public virtual void ShowView(IView view) {
      view.MainForm = this;
      views.Add(view);
      ActiveView = view;
    }
    #endregion

    #region create menu and toolbar
    private void CreateGUI() {
      DiscoveryService ds = new DiscoveryService();

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

      ToolStripDropDownItem parent = FindParent(menuItem, menuStrip.Items);
      if (parent == null)
        menuStrip.Items.Add(item);
      else
        parent.DropDownItems.Add(item);
    }

    private void AddToolStripButtonItem(IToolStripButtonItem buttonItem) {
      ToolStripItem item;
      if (buttonItem.IsDropDownButton)
        item = new ToolStripDropDownButton();
      else
        item = new ToolStripButton();

      SetToolStripItemProperties(item, buttonItem);
      ToolStripDropDownItem parent = FindParent(buttonItem, toolStrip.Items);
      if (parent == null)
        toolStrip.Items.Add(item);
      else
        parent.DropDownItems.Add(item);
    }

    private ToolStripDropDownItem FindParent(IToolStripItem item, ToolStripItemCollection parentItems) {
      ToolStripDropDownItem parent = null;
      foreach (string structure in item.Structure.Split(item.StructureSeparator)) {
        if (parentItems.ContainsKey(structure))
          parent = (ToolStripDropDownItem)parentItems[structure];
        else
          break;
        parentItems = parent.DropDownItems;
      }
      return parent;
    }

    private void SetToolStripItemProperties(ToolStripItem toolStripItem, IToolStripItem iToolStripItem) {
      toolStripItem.Text = iToolStripItem.Name;
      toolStripItem.Name = iToolStripItem.Name;
      toolStripItem.Tag = iToolStripItem;
      toolStripItem.Image = iToolStripItem.Image;
      toolStripItem.DisplayStyle = iToolStripItem.DisplayStyle;
      if (iToolStripItem.ListenActiveViewChanged)
        this.ActiveViewChanged += new EventHandler(iToolStripItem.ActiveViewChanged);
      if (iToolStripItem.ListenViewChanged)
        this.viewChangeToolStripItems.Add(iToolStripItem);
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
