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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views {
  public sealed partial class ViewHost : UserControl {
    private Dictionary<Type, ToolStripMenuItem> typeMenuItemTable;

    public IEnumerable<Type> AvailableViewTypes {
      get { return typeMenuItemTable.Keys; }
    }

    private Type viewType;
    public Type ViewType {
      get { return this.viewType; }
      set {
        if (!ViewCanShowContent(value, content))
          throw new ArgumentException(string.Format("View \"{0}\" cannot display content \"{1}\".",
                                                    value.GetPrettyName(),
                                                    content.GetType().GetPrettyName()));
        viewType = value;
        UpdateView();
      }
    }

    private object content;
    public object Content {
      get { return content; }
      set {
        if (value != content) {
          content = value;
          Initialize();
        }
      }
    }

    public ViewHost() {
      typeMenuItemTable = new Dictionary<Type, ToolStripMenuItem>();
      viewType = null;
      content = null;
      InitializeComponent();
      Initialize();
    }

    private void Initialize() {
      viewsLabel.Enabled = false;
      viewsLabel.Visible = false;
      typeMenuItemTable.Clear();
      contextMenuStrip.Items.Clear();
      contextMenuStrip.Enabled = false;
      messageLabel.Visible = false;
      if (viewPanel.Controls.Count > 0) viewPanel.Controls[0].Dispose();
      viewPanel.Controls.Clear();
      viewPanel.Enabled = false;
      viewPanel.Visible = false;

      if (Content != null) {
        var viewTypes = from t in MainFormManager.GetViewTypes(Content.GetType())
                        orderby t.Name ascending
                        select t;
        foreach (Type v in viewTypes) {
          ToolStripMenuItem item = new ToolStripMenuItem(v.GetPrettyName());
          item.Name = v.FullName;
          item.ToolTipText = v.GetPrettyName(true);
          item.Tag = v;
          contextMenuStrip.Items.Add(item);
          typeMenuItemTable.Add(v, item);
        }
        if (contextMenuStrip.Items.Count == 0) {
          messageLabel.Visible = true;
        } else {
          viewsLabel.Enabled = true;
          viewsLabel.Visible = true;
          contextMenuStrip.Enabled = true;
          messageLabel.Visible = false;
        }

        if (!ViewCanShowContent(viewType, Content)) {
          viewType = MainFormManager.GetDefaultViewType(Content.GetType());
          if ((viewType == null) && (contextMenuStrip.Items.Count > 0))  // create first available view if default view is not available
            viewType = (Type)contextMenuStrip.Items[0].Tag;
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
        throw new ArgumentException(string.Format("View \"{0}\" cannot display content \"{1}\".",
                                                  viewType.GetPrettyName(),
                                                  Content.GetType().GetPrettyName()));

      foreach (ToolStripMenuItem item in typeMenuItemTable.Values) {
        item.Checked = false;
        item.Enabled = true;
      }
      typeMenuItemTable[viewType].Checked = true;
      typeMenuItemTable[viewType].Enabled = false;

      Control view = (Control)MainFormManager.CreateView(viewType, Content);
      viewPanel.Controls.Add(view);
      viewPanel.Tag = view;
      view.Dock = DockStyle.Fill;
      viewPanel.Enabled = true;
      viewPanel.Visible = true;
    }

    private bool ViewCanShowContent(Type viewType, object content) {
      if (content == null) // every view can display null
        return true;
      if (viewType == null)
        return false;
      return ContentAttribute.CanViewType(viewType, Content.GetType()) && typeMenuItemTable.ContainsKey(viewType);
    }

    private void viewsLabel_DoubleClick(object sender, EventArgs e) {
      MainFormManager.CreateView(viewPanel.Tag.GetType(), Content).Show();
    }
    private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
      Type viewType = (Type)e.ClickedItem.Tag;
      ViewType = viewType;
    }
  }
}
