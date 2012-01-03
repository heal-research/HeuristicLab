#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class OperatorTreeView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.graphTreeView = new System.Windows.Forms.TreeView();
      this.graphContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.breakpointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.graphContextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // graphTreeView
      // 
      this.graphTreeView.AllowDrop = true;
      this.graphTreeView.ContextMenuStrip = this.graphContextMenuStrip;
      this.graphTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.graphTreeView.HideSelection = false;
      this.graphTreeView.ImageIndex = 0;
      this.graphTreeView.ImageList = this.imageList;
      this.graphTreeView.Location = new System.Drawing.Point(0, 0);
      this.graphTreeView.Name = "graphTreeView";
      this.graphTreeView.SelectedImageIndex = 0;
      this.graphTreeView.ShowNodeToolTips = true;
      this.graphTreeView.Size = new System.Drawing.Size(475, 291);
      this.graphTreeView.TabIndex = 0;
      this.graphTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.graphTreeView_BeforeExpand);
      this.graphTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.graphTreeView_DragDrop);
      this.graphTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.graphTreeView_AfterSelect);
      this.graphTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphTreeView_MouseDown);
      this.graphTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.graphTreeView_DragEnterOver);
      this.graphTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.graphTreeView_KeyDown);
      this.graphTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.graphTreeView_ItemDrag);
      this.graphTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.graphTreeView_DragEnterOver);
      // 
      // graphContextMenuStrip
      // 
      this.graphContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.breakpointToolStripMenuItem,
            this.viewToolStripMenuItem});
      this.graphContextMenuStrip.Name = "graphContextMenuStrip";
      this.graphContextMenuStrip.Size = new System.Drawing.Size(132, 48);
      this.graphContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.graphContextMenuStrip_Opening);
      // 
      // breakpointToolStripMenuItem
      // 
      this.breakpointToolStripMenuItem.CheckOnClick = true;
      this.breakpointToolStripMenuItem.Name = "breakpointToolStripMenuItem";
      this.breakpointToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
      this.breakpointToolStripMenuItem.Text = "&Breakpoint";
      this.breakpointToolStripMenuItem.ToolTipText = "Halt engine execution after executing the operator";
      this.breakpointToolStripMenuItem.Click += new System.EventHandler(this.breakpointToolStripMenuItem_Click);
      // 
      // viewToolStripMenuItem
      // 
      this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
      this.viewToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
      this.viewToolStripMenuItem.Text = "&View...";
      this.viewToolStripMenuItem.ToolTipText = "View operator";
      this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
      // 
      // OperatorTreeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.graphTreeView);
      this.Name = "OperatorTreeView";
      this.Size = new System.Drawing.Size(475, 291);
      this.graphContextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView graphTreeView;
    private System.Windows.Forms.ContextMenuStrip graphContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem breakpointToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    private System.Windows.Forms.ImageList imageList;
  }
}
