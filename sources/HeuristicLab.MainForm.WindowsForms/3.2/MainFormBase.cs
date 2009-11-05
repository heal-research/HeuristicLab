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

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class MainFormBase : Form, IMainForm {
    protected MainFormBase()
      : base() {
      InitializeComponent();
      this.views = new Dictionary<IView, Form>();
      this.userInterfaceItems = new List<IUserInterfaceItem>();
    }

    protected MainFormBase(Type userInterfaceItemType)
      : this() {
      this.userInterfaceItemType = userInterfaceItemType;
      CreateGUI();
    }

    private void MainFormBase_Load(object sender, EventArgs e) {
      if (!DesignMode)
        MainFormManager.RegisterMainForm(this);
    }

    #region IMainForm Members
    public string Title {
      get { return this.Text; }
      set {
        if (InvokeRequired) {
          Action<string> action = delegate(string s) { this.Title = s; };
          Invoke(action, value);
        } else
          this.Text = value;
      }
    }

    public override Cursor Cursor {
      get { return base.Cursor; }
      set {
        if (InvokeRequired) {
          Action<Cursor> action = delegate(Cursor c) { this.Cursor = c; };
          Invoke(action, value);
        } else
          base.Cursor = value;
      }
    }

    private Type userInterfaceItemType;
    public Type UserInterfaceItemType {
      get { return this.userInterfaceItemType; }
    }

    private Dictionary<IView, Form> views;
    public IEnumerable<IView> Views {
      get { return views.Keys; }
    }

    public Form GetForm(IView view) {
      if (views.ContainsKey(view))
        return views[view];
      return null;
    }

    private IView activeView;
    public IView ActiveView {
      get { return this.activeView; }
      protected set {
        if (this.activeView != value) {
          if (InvokeRequired) {
            Action<IView> action = delegate(IView activeView) { this.ActiveView = activeView; };
            Invoke(action, value);
          } else {
            this.activeView = value;
            OnActiveViewChanged();
          }
        }
      }
    }

    private List<IUserInterfaceItem> userInterfaceItems;
    protected IEnumerable<IUserInterfaceItem> UserInterfaceItems {
      get { return this.userInterfaceItems; }
    }

    public event EventHandler ActiveViewChanged;
    protected virtual void OnActiveViewChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnActiveViewChanged);
      else if (ActiveViewChanged != null)
        ActiveViewChanged(this, new EventArgs());
    }

    public event EventHandler Changed;
    public void FireMainFormChanged() {
      OnMainFormChanged();
    }
    protected virtual void OnMainFormChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnMainFormChanged);
      else if (Changed != null)
        Changed(this, new EventArgs());
    }

    protected virtual Form CreateForm(IView view) {
      throw new NotImplementedException("CreateForm must be implemented in subclasses of MainFormBase.");
    }

    public virtual bool ShowView(IView view) {
      if (InvokeRequired) return (bool)Invoke((Func<IView, bool>)ShowView, view);
      else {
        if (!views.Keys.Contains(view)) {
          Form form = CreateForm(view);
          views[view] = form;
          form.Activated += new EventHandler(FormActivated);
          form.GotFocus += new EventHandler(FormActivated);
          form.FormClosing += new FormClosingEventHandler(view.OnClosing);
          form.FormClosed += new FormClosedEventHandler(view.OnClosed);
          form.FormClosed += new FormClosedEventHandler(ChildFormClosed);
          foreach (IUserInterfaceItem item in UserInterfaceItems)
            view.Changed += new EventHandler(item.ViewChanged);
          return true;
        } else
          return false;
      }
    }

    public virtual void HideView(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)HideView, view);
      else {
        if (views.ContainsKey(view))
          views[view].Hide();
      }
    }

    public void CloseView(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)CloseView, view);
      else {
        if (views.ContainsKey(view))
          views[view].Close();
      }
    }

    public virtual void CloseAllViews() {
      foreach (IView view in views.Keys.ToArray())
        CloseView(view);
    }
    #endregion

    #region events
    private void ChildFormClosed(object sender, FormClosedEventArgs e) {
      Form form = (Form)sender;
      IView view = GetViewForForm(form);

      form.Activated -= new EventHandler(FormActivated);
      form.GotFocus -= new EventHandler(FormActivated);
      form.FormClosing -= new FormClosingEventHandler(view.OnClosing);
      form.FormClosed -= new FormClosedEventHandler(view.OnClosed);
      form.FormClosed -= new FormClosedEventHandler(ChildFormClosed);
      foreach (IUserInterfaceItem item in UserInterfaceItems)
        view.Changed -= new EventHandler(item.ViewChanged);

      views.Remove(view);
      if (ActiveView == view)
        ActiveView = null;
    }

    private void FormActivated(object sender, EventArgs e) {
      this.ActiveView = GetViewForForm((Form)sender);
    }

    private IView GetViewForForm(Form form) {
      return views.Where(x => x.Value == form).Single().Key;
    }
    #endregion

    #region create menu and toolbar
    protected virtual void CreateGUI() {
      DiscoveryService ds = new DiscoveryService();

      object[] items = ds.GetInstances(userInterfaceItemType);
      IEnumerable<MenuItemBase> toolStripMenuItems =
        from mi in items
        where mi is MenuItemBase
        orderby ((MenuItemBase)mi).Position
        select (MenuItemBase)mi;
      foreach (MenuItemBase menuItem in toolStripMenuItems)
        AddToolStripMenuItem(menuItem);

      items = ds.GetInstances(userInterfaceItemType);
      IEnumerable<ToolBarItemBase> toolStripButtonItems =
        from bi in items
        where bi is ToolBarItemBase
        orderby ((ToolBarItemBase)bi).Position
        select (ToolBarItemBase)bi;
      foreach (ToolBarItemBase toolStripButtonItem in toolStripButtonItems)
        AddToolStripButtonItem(toolStripButtonItem);
    }

    private void AddToolStripMenuItem(MenuItemBase menuItem) {
      ToolStripMenuItem item = new ToolStripMenuItem();
      SetToolStripItemProperties(item, menuItem);
      menuItem.ToolStripItem = item;
      item.ShortcutKeys = menuItem.ShortCutKeys;
      item.DisplayStyle = menuItem.ToolStripItemDisplayStyle;
      this.InsertItem(menuItem.Structure, typeof(ToolStripMenuItem), item, menuStrip.Items);
    }

    private void AddToolStripButtonItem(ToolBarItemBase buttonItem) {
      ToolStripItem item;
      if (buttonItem.IsDropDownButton)
        item = new ToolStripDropDownButton();
      else
        item = new ToolStripButton();

      SetToolStripItemProperties(item, buttonItem);
      item.DisplayStyle = buttonItem.ToolStripItemDisplayStyle;
      buttonItem.ToolStripItem = item;
      this.InsertItem(buttonItem.Structure, typeof(ToolStripDropDownButton), item, toolStrip.Items);
    }

    private void InsertItem(IEnumerable<string> structure, Type t, ToolStripItem item, ToolStripItemCollection parentItems) {
      ToolStripDropDownItem parent = null;
      foreach (string s in structure) {
        if (parentItems.ContainsKey(s))
          parent = (ToolStripDropDownItem)parentItems[s];
        else {
          parent = (ToolStripDropDownItem)Activator.CreateInstance(t, s, null, null, s); ;
          parentItems.Add(parent);
        }
        parentItems = parent.DropDownItems;
      }
      parentItems.Add(item);
    }

    private void SetToolStripItemProperties(ToolStripItem toolStripItem, IUserInterfaceItem userInterfaceItem) {
      toolStripItem.Name = userInterfaceItem.Name;
      toolStripItem.Text = userInterfaceItem.Name;
      toolStripItem.ToolTipText = userInterfaceItem.ToolTipText;
      toolStripItem.Tag = userInterfaceItem;
      toolStripItem.Image = userInterfaceItem.Image;
      this.ActiveViewChanged += new EventHandler(userInterfaceItem.ActiveViewChanged);
      this.Changed += new EventHandler(userInterfaceItem.MainFormChanged);
      toolStripItem.Click += new EventHandler(ToolStripItemClicked);
      this.userInterfaceItems.Add(userInterfaceItem);
    }

    private void ToolStripItemClicked(object sender, EventArgs e) {
      System.Windows.Forms.ToolStripItem item = (System.Windows.Forms.ToolStripItem)sender;
      ((IUserInterfaceItem)item.Tag).Execute();
    }
    #endregion
  }
}
