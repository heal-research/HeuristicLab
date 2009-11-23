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

namespace HeuristicLab.Core.Views {
  partial class ScopeView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (scopesTreeView.Nodes.Count > 0) {
        RemoveTreeNode(scopesTreeView.Nodes[0]);
        scopesTreeView.Nodes.Clear();
      }
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.scopesTreeView = new System.Windows.Forms.TreeView();
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.automaticUpdatingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.variablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.contextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // scopesTreeView
      // 
      this.scopesTreeView.ContextMenuStrip = this.contextMenuStrip;
      this.scopesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scopesTreeView.HideSelection = false;
      this.scopesTreeView.Location = new System.Drawing.Point(0, 0);
      this.scopesTreeView.Name = "scopesTreeView";
      this.scopesTreeView.ShowNodeToolTips = true;
      this.scopesTreeView.Size = new System.Drawing.Size(400, 400);
      this.scopesTreeView.TabIndex = 0;
      this.scopesTreeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.scopesTreeView_AfterCollapse);
      this.scopesTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.scopesTreeView_BeforeExpand);
      this.scopesTreeView.DoubleClick += new System.EventHandler(this.scopesTreeView_DoubleClick);
      this.scopesTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.scopesTreeView_MouseDown);
      this.scopesTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.scopesTreeView_ItemDrag);
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.automaticUpdatingToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.toolStripMenuItem1,
            this.variablesToolStripMenuItem,
            this.viewToolStripMenuItem});
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(180, 98);
      this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
      // 
      // automaticUpdatingToolStripMenuItem
      // 
      this.automaticUpdatingToolStripMenuItem.CheckOnClick = true;
      this.automaticUpdatingToolStripMenuItem.Name = "automaticUpdatingToolStripMenuItem";
      this.automaticUpdatingToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
      this.automaticUpdatingToolStripMenuItem.Text = "Automatic &Updating";
      this.automaticUpdatingToolStripMenuItem.ToolTipText = "Automatically update scope editor during execution";
      this.automaticUpdatingToolStripMenuItem.Click += new System.EventHandler(this.automaticUpdatingToolStripMenuItem_Click);
      // 
      // refreshToolStripMenuItem
      // 
      this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
      this.refreshToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
      this.refreshToolStripMenuItem.Text = "&Refresh";
      this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(176, 6);
      // 
      // viewToolStripMenuItem
      // 
      this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
      this.viewToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
      this.viewToolStripMenuItem.Text = "&View";
      // 
      // variablesToolStripMenuItem
      // 
      this.variablesToolStripMenuItem.Name = "variablesToolStripMenuItem";
      this.variablesToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
      this.variablesToolStripMenuItem.Text = "&Variables...";
      this.variablesToolStripMenuItem.Click += new System.EventHandler(this.variablesToolStripMenuItem_Click);
      // 
      // ScopeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.scopesTreeView);
      this.Name = "ScopeView";
      this.Size = new System.Drawing.Size(400, 400);
      this.contextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView scopesTreeView;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem automaticUpdatingToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem variablesToolStripMenuItem;
  }
}
