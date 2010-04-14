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

namespace HeuristicLab.MainForm.WindowsForms {
  public sealed partial class ViewHost : UserControl {
    private Dictionary<Type, IView> cachedViews;
    public ViewHost() {
      InitializeComponent();
      viewType = null;
      content = null;
      cachedViews = new Dictionary<Type, IView>();
      Initialize();
    }
    public ViewHost(bool readOnly)
      : this() {
      this.ReadOnly = readOnly;
    }

    private Type viewType;
    public Type ViewType {
      get { return this.viewType; }
      set {
        if (viewType != value) {
          if (value != null && !ViewCanShowContent(value, content))
            throw new ArgumentException(string.Format("View \"{0}\" cannot display content \"{1}\".",
                                                      value, content.GetType()));
          viewType = value;
          UpdateView();
        }
      }
    }

    private object content;
    public object Content {
      get { return content; }
      set {
        if (value != content) {
          content = value;
          viewContextMenuStrip.Item = content;
          cachedViews.Clear();
          Initialize();
        }
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
    private bool readOnly;
    public bool ReadOnly {
      get { return this.readOnly; }
      set {
        if (InvokeRequired) {
          Action<bool> action = delegate(bool b) { this.ReadOnly = b; };
          Invoke(action, value);
        } else {
          if (value != readOnly) {
            readOnly = value;
            OnReadOnlyChanged();
          }
        }
      }
    }

    public event EventHandler ReadOnlyChanged;
    private void OnReadOnlyChanged() {
      if (InvokeRequired)
        Invoke((MethodInvoker)OnReadOnlyChanged);
      else {
        EventHandler handler = ReadOnlyChanged;
        if (handler != null)
          handler(this, EventArgs.Empty);
        foreach (IView view in cachedViews.Values)
          view.ReadOnly = this.readOnly;
      }
    }

    private void Initialize() {
      viewsLabel.Visible = false;
      viewPanel.Visible = false;
      if (viewPanel.Controls.Count > 0) viewPanel.Controls[0].Dispose();
      viewPanel.Controls.Clear();

      if (Content != null) {
        if (viewContextMenuStrip.Items.Count == 0) 
          messageLabel.Visible = true;
         else 
          viewsLabel.Visible = true;

        if (!ViewCanShowContent(viewType, Content)) {
          viewType = MainFormManager.GetDefaultViewType(Content.GetType());
          if ((viewType == null) && (viewContextMenuStrip.Items.Count > 0))  // create first available view if default view is not available
            viewType = (Type)viewContextMenuStrip.Items[0].Tag;
        }
        UpdateView();
      }
    }

    private void UpdateView() {
      if (viewPanel.Controls.Count > 0) viewPanel.Controls[0].Dispose();
      viewPanel.Controls.Clear();

      if (viewType == null || content == null)
        return;

      if (!ViewCanShowContent(viewType, content))
        throw new InvalidOperationException(string.Format("View \"{0}\" cannot display content \"{1}\".",
                                                          viewType, Content.GetType()));

      UpdateActiveMenuItem();
      cachedViews.Clear();
      Control view = (Control)MainFormManager.CreateView(viewType, Content,ReadOnly);
      cachedViews.Add(viewType, ((IView)view));
      view.Dock = DockStyle.Fill;
      viewPanel.Controls.Add(view);
      viewPanel.Visible = true;
    }

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
      MainFormManager.CreateView(viewType, Content,ReadOnly).Show();
    }

    private void viewContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
      Type viewType = (Type)e.ClickedItem.Tag;
      ViewType = viewType;
    }
  }
}
