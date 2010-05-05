#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using HeuristicLab.MainForm;
using HeuristicLab.Common;

namespace HeuristicLab.MainForm.WindowsForms {
  [Content(typeof(IContent))]
  public sealed partial class ViewHost : AsynchronousContentView {
    public ViewHost() {
      InitializeComponent();
      cachedViews = new Dictionary<Type, IContentView>();
      viewType = null;
      startDragAndDrop = false;
      viewContextMenuStrip.IgnoredViewTypes = new List<Type>() { typeof(ViewHost) };
      activeView = null;
      Content = null;
      OnContentChanged();
    }

    private bool viewShown;
    private Dictionary<Type, IContentView> cachedViews;
    public IEnumerable<IContentView> Views {
      get { return cachedViews.Values; }
    }

    private IContentView activeView;
    public IContentView ActiveView {
      get { return this.activeView; }
      private set {
        if (activeView != value) {
          if (activeView != null) {
            DeregisterActiveViewEvents();
            View view = activeView as View;
            if (view != null)
              view.OnHidden(EventArgs.Empty);
          }
          activeView = value;
          if (activeView != null) {
            RegisterActiveViewEvents();
            View view = activeView as View;
            if (view != null)
              view.OnShown(new ViewShownEventArgs(view, false));
          }
          ActiveViewChanged();
          OnViewTypeChanged();
        }
      }
    }
    private Type viewType;
    public Type ViewType {
      get { return this.viewType; }
      set {
        if (viewType != value) {
          if (value != null && !ViewCanShowContent(value, Content))
            throw new ArgumentException(string.Format("View \"{0}\" cannot display content \"{1}\".",
                                                      value, Content.GetType()));
          viewType = value;
          OnViewTypeChanged();
        }
      }
    }
    public new IContent Content {
      get { return base.Content; }
      set {
        if (value == null || this.Content == null || value.GetType() != this.Content.GetType())
          cachedViews.Clear();

        base.Content = value;
      }
    }

    public new bool Enabled {
      get { return base.Enabled; }
      set {
        this.SuspendRepaint();
        base.Enabled = value;
        this.viewsLabel.Enabled = value;
        this.ResumeRepaint(true);
      }
    }

    protected override void OnContentChanged() {
      messageLabel.Visible = false;
      viewsLabel.Visible = false;
      viewPanel.Visible = false;
      viewContextMenuStrip.Item = Content;

      if (Content != null) {
        if (viewContextMenuStrip.Items.Count == 0)
          messageLabel.Visible = true;
        else {
          viewsLabel.Visible = true;
          viewPanel.Visible = true;
        }

        if (!ViewCanShowContent(viewType, Content)) {
          ViewType = MainFormManager.GetDefaultViewType(Content.GetType());
          if ((viewType == null) && (viewContextMenuStrip.Items.Count > 0))  // create first available view if default view is not available
            ViewType = (Type)viewContextMenuStrip.Items[0].Tag;
        }
        UpdateActiveMenuItem();
        foreach (IContentView view in cachedViews.Values)
          view.Content = this.Content;
      } else {
        if (viewPanel.Controls.Count > 0) viewPanel.Controls[0].Dispose();
        viewPanel.Controls.Clear();
        cachedViews.Clear();
      }
    }

    private void OnViewTypeChanged() {
      viewPanel.Controls.Clear();
      if (viewType == null || Content == null)
        return;
      if (!ViewCanShowContent(viewType, Content))
        throw new InvalidOperationException(string.Format("View \"{0}\" cannot display content \"{1}\".",
                                                          viewType, Content.GetType()));
      if (viewPanel.Height <= 10 || viewPanel.Width <= 10) {
        viewShown = false;
        return;
      }

      viewShown = true;
      UpdateActiveMenuItem();
      IContentView view;
      if (cachedViews.ContainsKey(ViewType))
        view = cachedViews[ViewType];
      else {
        view = MainFormManager.CreateView(viewType);
        view.ReadOnly = this.ReadOnly;
        view.Locked = this.Locked;
        cachedViews.Add(viewType, view);
      }
      this.ActiveView = view;
      Control control = (Control)view;
      control.Dock = DockStyle.Fill;
      viewPanel.Controls.Add(control);
      viewPanel.Visible = true;
      view.Content = Content;
    }

    private void viewPanel_Resize(object sender, EventArgs e) {
      if (!viewShown)
        this.OnViewTypeChanged();
    }

    private void RegisterActiveViewEvents() {
      activeView.Changed += new EventHandler(activeView_Changed);
      activeView.CaptionChanged += new EventHandler(activeView_CaptionChanged);
    }
    private void DeregisterActiveViewEvents() {
      activeView.Changed -= new EventHandler(activeView_Changed);
      activeView.CaptionChanged -= new EventHandler(activeView_CaptionChanged);
    }
    private void activeView_CaptionChanged(object sender, EventArgs e) {
      this.ActiveViewChanged();
    }
    private void activeView_Changed(object sender, EventArgs e) {
      this.ActiveViewChanged();
    }
    private void ActiveViewChanged() {
      if (ActiveView != null) {
        this.Caption = this.ActiveView.Caption;
        this.Locked = this.ActiveView.Locked;
      }
    }
    #region forwarding of view events
    protected override void OnReadOnlyChanged() {
      base.OnReadOnlyChanged();
      foreach (IContentView view in cachedViews.Values)
        view.ReadOnly = this.ReadOnly;
    }
    protected override void OnLockedChanged() {
      base.OnLockedChanged();
      foreach (IContentView view in cachedViews.Values)
        view.Locked = this.Locked;
    }
    internal protected override void OnShown(ViewShownEventArgs e) {
      base.OnShown(e);
      View view = this.ActiveView as View;
      if (view != null)
        view.OnShown(e);
    }
    internal protected override void OnHidden(EventArgs e) {
      base.OnHidden(e);
      View view = this.ActiveView as View;
      if (view != null)
        view.OnHidden(e);
    }
    internal protected override void OnClosing(FormClosingEventArgs e) {
      base.OnClosing(e);
      foreach (View view in this.Views.OfType<View>())
        view.OnClosing(e);
    }
    internal protected override void OnClosed(FormClosedEventArgs e) {
      base.OnClosed(e);
      foreach (View view in this.Views.OfType<View>())
        view.OnClosed(e);
    }
    #endregion

    #region GUI actions
    private void UpdateActiveMenuItem() {
      foreach (KeyValuePair<Type, ToolStripMenuItem> item in viewContextMenuStrip.MenuItems) {
        if (item.Key == viewType) {
          item.Value.Checked = true;
          item.Value.Enabled = false;
        } else {
          item.Value.Checked = false;
          item.Value.Enabled = true;
        }
      }
    }

    private bool ViewCanShowContent(Type viewType, object content) {
      if (content == null) // every view can display null
        return true;
      if (viewType == null)
        return false;
      return ContentAttribute.CanViewType(viewType, Content.GetType()) && viewContextMenuStrip.MenuItems.Any(item => item.Key == viewType);
    }

    private void viewsLabel_DoubleClick(object sender, EventArgs e) {
      IContentView view = MainFormManager.MainForm.ShowContent(this.Content);
      view.ReadOnly = this.ReadOnly;
      view.Locked = this.Locked;
    }
    private void viewContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
      Type viewType = (Type)e.ClickedItem.Tag;
      ViewType = viewType;
    }

    private bool startDragAndDrop;
    private void viewsLabel_MouseDown(object sender, MouseEventArgs e) {
      if (!Locked) {
        startDragAndDrop = true;
        viewsLabel.Capture = false;
        viewsLabel.Focus();
      }
    }
    private void viewsLabel_MouseLeave(object sender, EventArgs e) {
      if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left && startDragAndDrop) {
        DataObject data = new DataObject();
        data.SetData("Type", Content.GetType());
        data.SetData("Value", Content);
        DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
      } else
        startDragAndDrop = false;
    }
    #endregion
  }
}
