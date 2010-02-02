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
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Core.Views { 
  public partial class ViewHost : UserControl {
    private Dictionary<Type, ToolStripMenuItem> typeMenuItemTable;

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
      InitializeComponent();
      Initialize();
    }

    protected virtual void Initialize() {
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
        foreach (Type viewType in viewTypes) {
          ToolStripMenuItem item = new ToolStripMenuItem(viewType.GetPrettyName());
          item.Name = viewType.FullName;
          item.ToolTipText = viewType.GetPrettyName(true);
          item.Tag = viewType;
          contextMenuStrip.Items.Add(item);
          typeMenuItemTable.Add(viewType, item);
        }
        if (contextMenuStrip.Items.Count == 0) {
          messageLabel.Visible = true;
        } else {
          viewsLabel.Enabled = true;
          viewsLabel.Visible = true;
          contextMenuStrip.Enabled = true;
          messageLabel.Visible = false;
        }

        Control view = (Control)MainFormManager.CreateDefaultView(Content);
        if ((view == null) && (contextMenuStrip.Items.Count > 0))  // create first available view if default view is not available
          view = (Control)MainFormManager.CreateView((Type)contextMenuStrip.Items[0].Tag, Content);
        if (view != null) {
          viewPanel.Controls.Add(view);
          viewPanel.Tag = view;
          view.Dock = DockStyle.Fill;
          viewPanel.Enabled = true;
          viewPanel.Visible = true;
          typeMenuItemTable[view.GetType()].Checked = true;
          typeMenuItemTable[view.GetType()].Enabled = false;
        }
      }
    }

    private void viewsLabel_DoubleClick(object sender, EventArgs e) {
      MainFormManager.CreateView(viewPanel.Tag.GetType(), Content).Show();
    }
    protected void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
      Type viewType = (Type)e.ClickedItem.Tag;
      foreach (ToolStripMenuItem item in typeMenuItemTable.Values) {
        item.Checked = false;
        item.Enabled = true;
      }
      typeMenuItemTable[viewType].Checked = true;
      typeMenuItemTable[viewType].Enabled = false;

      if (viewPanel.Controls.Count > 0) viewPanel.Controls[0].Dispose();
      viewPanel.Controls.Clear();
      Control view = (Control)MainFormManager.CreateView(viewType, Content);
      viewPanel.Controls.Add(view);
      viewPanel.Tag = view;
      view.Dock = DockStyle.Fill;
    }
  }
}
