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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.PluginInfrastructure;
using WeifenLuo.WinFormsUI.Docking;

namespace HeuristicLab.MainForm.WindowsForms {
  public partial class MainForm : Form, IMainForm {
    private bool initialized;
    private int appStartingCursors;
    private int waitingCursors;

    protected MainForm()
      : base() {
      InitializeComponent();
      this.views = new Dictionary<IView, Form>();
      this.userInterfaceItems = new List<IUserInterfaceItem>();
      this.initialized = false;
      this.showContentInViewHost = false;
      appStartingCursors = 0;
      waitingCursors = 0;
    }

    protected MainForm(Type userInterfaceItemType)
      : this() {
      this.userInterfaceItemType = userInterfaceItemType;
    }

    #region properties
    private bool showContentInViewHost;
    public bool ShowContentInViewHost {
      get { return this.showContentInViewHost; }
      set { this.showContentInViewHost = value; }
    }

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
    #endregion

    #region events
    public event EventHandler ActiveViewChanged;
    protected virtual void OnActiveViewChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnActiveViewChanged);
      else {
        EventHandler handler = ActiveViewChanged;
        if (handler != null)
          handler(this, EventArgs.Empty);
      }
    }

    public event EventHandler<ViewEventArgs> ViewClosed;
    protected virtual void OnViewClosed(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)OnViewClosed, view);
      else {
        EventHandler<ViewEventArgs> handler = ViewClosed;
        if (handler != null)
          handler(this, new ViewEventArgs(view));
      }
    }

    public event EventHandler<ViewShownEventArgs> ViewShown;
    protected virtual void OnViewShown(IView view, bool firstTimeShown) {
      if (InvokeRequired) Invoke((Action<IView, bool>)OnViewShown, view, firstTimeShown);
      else {
        EventHandler<ViewShownEventArgs> handler = ViewShown;
        if (handler != null)
          handler(this, new ViewShownEventArgs(view, firstTimeShown));
      }
    }

    public event EventHandler<ViewEventArgs> ViewHidden;
    protected virtual void OnViewHidden(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)OnViewHidden, view);
      else {
        EventHandler<ViewEventArgs> handler = ViewHidden;
        if (handler != null)
          handler(this, new ViewEventArgs(view));
      }
    }

    public event EventHandler Changed;
    protected void OnChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnChanged);
      else {
        EventHandler handler = Changed;
        if (handler != null)
          Changed(this, EventArgs.Empty);
      }
    }

    private void MainFormBase_Load(object sender, EventArgs e) {
      if (!DesignMode) {
        MainFormManager.RegisterMainForm(this);
        this.CreateGUI();
        if (!this.initialized) {
          this.initialized = true;
          this.OnInitialized(EventArgs.Empty);
        }
      }
    }

    protected virtual void OnInitialized(EventArgs e) { }

    public virtual void UpdateTitle() { }

    private void FormActivated(object sender, EventArgs e) {
      this.ActiveView = GetView((Form)sender);
    }

    protected override void OnFormClosing(FormClosingEventArgs e) {
      foreach (KeyValuePair<IView, Form> pair in this.views) {
        DockForm dockForm = pair.Value as DockForm;
        View view = pair.Key as View;
        if (view != null && dockForm != null && dockForm.DockState != DockState.Document) {
          view.CloseReason = CloseReason.ApplicationExitCall;
          view.OnClosingHelper(dockForm, e);
        }
      }
      base.OnFormClosing(e);
    }

    protected override void OnFormClosed(FormClosedEventArgs e) {
      foreach (KeyValuePair<IView, Form> pair in this.views.ToList()) {
        DockForm dockForm = pair.Value as DockForm;
        View view = pair.Key as View;
        if (view != null && dockForm != null && dockForm.DockState != DockState.Document) {
          view.CloseReason = CloseReason.ApplicationExitCall;
          view.OnClosedHelper(dockForm, e);
          dockForm.Close();
        }
      }
      base.OnFormClosed(e);
    }
    #endregion

    #region create, get, show, hide, close views
    protected virtual Form CreateForm(IView view) {
      throw new NotImplementedException("CreateForm must be implemented in subclasses of MainForm.");
    }

    internal Form GetForm(IView view) {
      if (views.ContainsKey(view))
        return views[view];
      return null;
    }
    protected IView GetView(Form form) {
      return views.Where(x => x.Value == form).Single().Key;
    }

    public IContentView ShowContent(IContent content) {
      if (content == null) throw new ArgumentNullException("Content cannot be null.");
      Type viewType = MainFormManager.GetDefaultViewType(content.GetType());
      if (viewType != null) return ShowContent(content, viewType);
      return null;
    }

    public IContentView ShowContent<T>(T content, bool reuseExistingView, IEqualityComparer<T> comparer = null) where T : class,IContent {
      if (content == null) throw new ArgumentNullException("Content cannot be null.");
      if (!reuseExistingView) return ShowContent(content);

      IContentView view = null;
      if (comparer == null) view = Views.OfType<IContentView>().Where(v => (v.Content as T) == content).FirstOrDefault();
      else view = Views.OfType<IContentView>().Where(v => comparer.Equals((v.Content as T), content)).FirstOrDefault();

      if (view == null) view = ShowContent(content);
      else view.Show();

      return view;
    }

    public IContentView ShowContent(IContent content, Type viewType) {
      if (InvokeRequired) return (IContentView)Invoke((Func<IContent, Type, IContentView>)ShowContent, content, viewType);
      else {
        if (content == null) throw new ArgumentNullException("Content cannot be null.");
        if (viewType == null) throw new ArgumentNullException("ViewType cannot be null.");

        IContentView view = null;
        if (ShowContentInViewHost) {
          ViewHost viewHost = new ViewHost();
          viewHost.ViewType = viewType;
          view = viewHost;

        } else view = MainFormManager.CreateView(viewType);

        view.Content = content;
        view.Show();
        return view;
      }
    }

    internal void ShowView(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)ShowView, view);
      else {
        Form form = GetForm(view);
        bool firstTimeShown = form == null;
        if (firstTimeShown) {
          form = CreateForm(view);
          form.Activated += new EventHandler(FormActivated);
          form.FormClosed += new FormClosedEventHandler(ChildFormClosed);
          view.Changed += new EventHandler(View_Changed);
          views[view] = form;
        }
        this.ShowView(view, firstTimeShown);
        this.OnViewShown(view, firstTimeShown);
      }
    }

    private void View_Changed(object sender, EventArgs e) {
      IView view = (IView)sender;
      if (view == this.ActiveView)
        this.OnActiveViewChanged();
      this.OnChanged();
    }

    protected virtual void ShowView(IView view, bool firstTimeShown) {
    }

    internal void HideView(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)HideView, view);
      else {
        this.Hide(view);
        if (this.activeView == view)
          this.ActiveView = null;
        this.OnViewHidden(view);
      }
    }

    protected virtual void Hide(IView view) {
    }

    private void ChildFormClosed(object sender, FormClosedEventArgs e) {
      Form form = (Form)sender;
      IView view = GetView(form);

      view.Changed -= new EventHandler(View_Changed);
      form.Activated -= new EventHandler(FormActivated);
      form.FormClosed -= new FormClosedEventHandler(ChildFormClosed);

      views.Remove(view);
      this.OnViewClosed(view);
      if (ActiveView == view)
        ActiveView = null;
    }

    internal void CloseView(IView view) {
      if (InvokeRequired) Invoke((Action<IView>)CloseView, view);
      else if (views.ContainsKey(view)) {
        this.views[view].Close();
        this.OnViewClosed(view);
      }
    }

    internal void CloseView(IView view, CloseReason closeReason) {
      if (InvokeRequired) Invoke((Action<IView>)CloseView, view);
      else if (views.ContainsKey(view)) {
        ((View)view).CloseReason = closeReason;
        this.CloseView(view);
      }
    }

    public void CloseAllViews() {
      foreach (IView view in views.Keys.ToArray())
        CloseView(view);
    }

    public void CloseAllViews(CloseReason closeReason) {
      foreach (IView view in views.Keys.ToArray())
        CloseView(view, closeReason);
    }
    #endregion

    #region create menu and toolbar
    private void CreateGUI() {
      IEnumerable<object> allUserInterfaceItems = ApplicationManager.Manager.GetInstances(userInterfaceItemType);

      IEnumerable<IPositionableUserInterfaceItem> toolStripMenuItems =
        from mi in allUserInterfaceItems
        where (mi is IPositionableUserInterfaceItem) &&
              (mi is IMenuItem || mi is IMenuSeparatorItem)
        orderby ((IPositionableUserInterfaceItem)mi).Position
        select (IPositionableUserInterfaceItem)mi;

      foreach (IPositionableUserInterfaceItem menuItem in toolStripMenuItems) {
        if (menuItem is IMenuItem)
          AddToolStripMenuItem((IMenuItem)menuItem);
        else if (menuItem is IMenuSeparatorItem)
          AddToolStripMenuItem((IMenuSeparatorItem)menuItem);
      }

      IEnumerable<IPositionableUserInterfaceItem> toolStripButtonItems =
        from bi in allUserInterfaceItems
        where (bi is IPositionableUserInterfaceItem) &&
              (bi is IToolBarItem || bi is IToolBarSeparatorItem)
        orderby ((IPositionableUserInterfaceItem)bi).Position
        select (IPositionableUserInterfaceItem)bi;

      foreach (IPositionableUserInterfaceItem toolStripButtonItem in toolStripButtonItems) {
        if (toolStripButtonItem is IToolBarItem)
          AddToolStripButtonItem((IToolBarItem)toolStripButtonItem);
        else if (toolStripButtonItem is IToolBarSeparatorItem)
          AddToolStripButtonItem((IToolBarSeparatorItem)toolStripButtonItem);
      }

      this.AdditionalCreationOfGuiElements();
    }

    protected virtual void AdditionalCreationOfGuiElements() {
    }

    private void AddToolStripMenuItem(IMenuItem menuItem) {
      ToolStripMenuItem item = new ToolStripMenuItem();
      this.SetToolStripItemProperties(item, menuItem);
      if (menuItem is MenuItem) {
        MenuItem menuItemBase = (MenuItem)menuItem;
        menuItemBase.ToolStripItem = item;
        item.ShortcutKeys = menuItemBase.ShortCutKeys;
        item.DisplayStyle = menuItemBase.ToolStripItemDisplayStyle;
      }
      this.InsertItem(menuItem.Structure, typeof(ToolStripMenuItem), item, menuStrip.Items);
    }

    private void AddToolStripMenuItem(IMenuSeparatorItem menuItem) {
      this.InsertItem(menuItem.Structure, typeof(ToolStripMenuItem), new ToolStripSeparator(), menuStrip.Items);
    }

    private void AddToolStripButtonItem(IToolBarItem buttonItem) {
      ToolStripItem item = new ToolStripButton();
      if (buttonItem is ToolBarItem) {
        ToolBarItem buttonItemBase = (ToolBarItem)buttonItem;
        if (buttonItemBase.IsDropDownButton)
          item = new ToolStripDropDownButton();

        item.DisplayStyle = buttonItemBase.ToolStripItemDisplayStyle;
        buttonItemBase.ToolStripItem = item;
      }

      this.SetToolStripItemProperties(item, buttonItem);
      this.InsertItem(buttonItem.Structure, typeof(ToolStripDropDownButton), item, toolStrip.Items);
    }

    private void AddToolStripButtonItem(IToolBarSeparatorItem buttonItem) {
      this.InsertItem(buttonItem.Structure, typeof(ToolStripDropDownButton), new ToolStripSeparator(), toolStrip.Items);
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

    private void SetToolStripItemProperties(ToolStripItem toolStripItem, IActionUserInterfaceItem userInterfaceItem) {
      toolStripItem.Name = userInterfaceItem.Name;
      toolStripItem.Text = userInterfaceItem.Name;
      toolStripItem.ToolTipText = userInterfaceItem.ToolTipText;
      toolStripItem.Tag = userInterfaceItem;
      toolStripItem.Image = userInterfaceItem.Image;
      toolStripItem.Click += new EventHandler(ToolStripItemClicked);
      this.userInterfaceItems.Add(userInterfaceItem);
    }

    private void ToolStripItemClicked(object sender, EventArgs e) {
      System.Windows.Forms.ToolStripItem item = (System.Windows.Forms.ToolStripItem)sender;
      try {
        ((IActionUserInterfaceItem)item.Tag).Execute();
      }
      catch (Exception ex) {
        ErrorHandling.ShowErrorDialog((Control)MainFormManager.MainForm, ex);
      }
    }
    #endregion

    #region Cursor Handling
    public void SetAppStartingCursor() {
      if (InvokeRequired)
        Invoke(new Action(SetAppStartingCursor));
      else {
        appStartingCursors++;
        SetCursor();
      }
    }
    public void ResetAppStartingCursor() {
      if (InvokeRequired)
        Invoke(new Action(ResetAppStartingCursor));
      else {
        appStartingCursors--;
        SetCursor();
      }
    }
    public void SetWaitCursor() {
      if (InvokeRequired)
        Invoke(new Action(SetWaitCursor));
      else {
        waitingCursors++;
        SetCursor();
      }
    }
    public void ResetWaitCursor() {
      if (InvokeRequired)
        Invoke(new Action(ResetWaitCursor));
      else {
        waitingCursors--;
        SetCursor();
      }
    }
    private void SetCursor() {
      if (waitingCursors > 0) Cursor = Cursors.WaitCursor;
      else if (appStartingCursors > 0) Cursor = Cursors.AppStarting;
      else Cursor = Cursors.Default;
    }
    #endregion
  }
}
