#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.DebugEngine {
  partial class OperationContentView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
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
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.executionContextGroupBox = new System.Windows.Forms.GroupBox();
      this.executionContextTreeView = new System.Windows.Forms.TreeView();
      this.executionContextConextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.showValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.executionContextImageList = new System.Windows.Forms.ImageList(this.components);
      this.scopeGroupBox = new System.Windows.Forms.GroupBox();
      this.scopeTreeView = new System.Windows.Forms.TreeView();
      this.scopeImageList = new System.Windows.Forms.ImageList(this.components);
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.parametersImageList = new System.Windows.Forms.ImageList(this.components);
      this.iconBox = new System.Windows.Forms.PictureBox();
      this.groupBox.SuspendLayout();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.executionContextGroupBox.SuspendLayout();
      this.executionContextConextMenu.SuspendLayout();
      this.scopeGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.iconBox)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox
      // 
      this.groupBox.Controls.Add(this.iconBox);
      this.groupBox.Controls.Add(this.splitContainer1);
      this.groupBox.Controls.Add(this.nameTextBox);
      this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox.Location = new System.Drawing.Point(0, 0);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new System.Drawing.Size(563, 412);
      this.groupBox.TabIndex = 0;
      this.groupBox.TabStop = false;
      this.groupBox.Text = "Operation";
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.Location = new System.Drawing.Point(6, 45);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.executionContextGroupBox);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.scopeGroupBox);
      this.splitContainer1.Size = new System.Drawing.Size(551, 361);
      this.splitContainer1.SplitterDistance = 242;
      this.splitContainer1.TabIndex = 4;
      // 
      // executionContextGroupBox
      // 
      this.executionContextGroupBox.Controls.Add(this.executionContextTreeView);
      this.executionContextGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.executionContextGroupBox.Location = new System.Drawing.Point(0, 0);
      this.executionContextGroupBox.Name = "executionContextGroupBox";
      this.executionContextGroupBox.Size = new System.Drawing.Size(242, 361);
      this.executionContextGroupBox.TabIndex = 0;
      this.executionContextGroupBox.TabStop = false;
      this.executionContextGroupBox.Text = "Execution Context";
      // 
      // executionContextTreeView
      // 
      this.executionContextTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.executionContextTreeView.ContextMenuStrip = this.executionContextConextMenu;
      this.executionContextTreeView.ImageIndex = 0;
      this.executionContextTreeView.ImageList = this.executionContextImageList;
      this.executionContextTreeView.Location = new System.Drawing.Point(6, 19);
      this.executionContextTreeView.Name = "executionContextTreeView";
      this.executionContextTreeView.SelectedImageIndex = 0;
      this.executionContextTreeView.ShowNodeToolTips = true;
      this.executionContextTreeView.Size = new System.Drawing.Size(230, 336);
      this.executionContextTreeView.TabIndex = 0;
      this.executionContextTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.executionContextTreeView_NodeMouseClick);
      this.executionContextTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.executionContextTreeView_NodeMouseDoubleClick);
      this.executionContextTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.executionContextTreeView_MouseDown);
      // 
      // executionContextConextMenu
      // 
      this.executionContextConextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showValueToolStripMenuItem});
      this.executionContextConextMenu.Name = "executionContextConextMenu";
      this.executionContextConextMenu.Size = new System.Drawing.Size(173, 26);
      this.executionContextConextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.executionContextConextMenu_Opening);
      // 
      // showValueToolStripMenuItem
      // 
      this.showValueToolStripMenuItem.Name = "showValueToolStripMenuItem";
      this.showValueToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
      this.showValueToolStripMenuItem.Text = "Show Actual Value";
      this.showValueToolStripMenuItem.ToolTipText = "Try to obtain the parameter\'s actual value in the current execution context and o" +
    "pen it in a new view.";
      this.showValueToolStripMenuItem.Click += new System.EventHandler(this.ShowValue_Click);
      // 
      // executionContextImageList
      // 
      this.executionContextImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.executionContextImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.executionContextImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // scopeGroupBox
      // 
      this.scopeGroupBox.Controls.Add(this.scopeTreeView);
      this.scopeGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scopeGroupBox.Location = new System.Drawing.Point(0, 0);
      this.scopeGroupBox.Name = "scopeGroupBox";
      this.scopeGroupBox.Size = new System.Drawing.Size(305, 361);
      this.scopeGroupBox.TabIndex = 1;
      this.scopeGroupBox.TabStop = false;
      this.scopeGroupBox.Text = "Scope";
      // 
      // scopeTreeView
      // 
      this.scopeTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.scopeTreeView.ImageIndex = 0;
      this.scopeTreeView.ImageList = this.scopeImageList;
      this.scopeTreeView.Location = new System.Drawing.Point(6, 19);
      this.scopeTreeView.Name = "scopeTreeView";
      this.scopeTreeView.SelectedImageIndex = 0;
      this.scopeTreeView.ShowNodeToolTips = true;
      this.scopeTreeView.Size = new System.Drawing.Size(293, 336);
      this.scopeTreeView.TabIndex = 0;
      this.scopeTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.scopeTreeView_NodeMouseDoubleClick);
      // 
      // scopeImageList
      // 
      this.scopeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.scopeImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.scopeImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.nameTextBox.Cursor = System.Windows.Forms.Cursors.Default;
      this.nameTextBox.Location = new System.Drawing.Point(32, 19);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.ReadOnly = true;
      this.nameTextBox.Size = new System.Drawing.Size(525, 20);
      this.nameTextBox.TabIndex = 3;
      this.nameTextBox.DoubleClick += new System.EventHandler(this.nameTextBox_DoubleClick);
      // 
      // parametersImageList
      // 
      this.parametersImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.parametersImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.parametersImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // iconBox
      // 
      this.iconBox.Location = new System.Drawing.Point(6, 19);
      this.iconBox.Name = "iconBox";
      this.iconBox.Size = new System.Drawing.Size(20, 20);
      this.iconBox.TabIndex = 5;
      this.iconBox.TabStop = false;
      // 
      // OperationContentView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.groupBox);
      this.Name = "OperationContentView";
      this.Size = new System.Drawing.Size(563, 412);
      this.groupBox.ResumeLayout(false);
      this.groupBox.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.executionContextGroupBox.ResumeLayout(false);
      this.executionContextConextMenu.ResumeLayout(false);
      this.scopeGroupBox.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.iconBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TreeView scopeTreeView;
    private System.Windows.Forms.GroupBox scopeGroupBox;
    private System.Windows.Forms.ImageList executionContextImageList;
    private System.Windows.Forms.ImageList parametersImageList;
    private System.Windows.Forms.ImageList scopeImageList;
    private System.Windows.Forms.GroupBox executionContextGroupBox;
    private System.Windows.Forms.TreeView executionContextTreeView;
    private System.Windows.Forms.ContextMenuStrip executionContextConextMenu;
    private System.Windows.Forms.ToolStripMenuItem showValueToolStripMenuItem;
    private System.Windows.Forms.PictureBox iconBox;
  }
}
