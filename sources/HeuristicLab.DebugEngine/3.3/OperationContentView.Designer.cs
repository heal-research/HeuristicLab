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
      this.executionContextImageList = new System.Windows.Forms.ImageList(this.components);
      this.scopeGroupBox = new System.Windows.Forms.GroupBox();
      this.scopeTreeView = new System.Windows.Forms.TreeView();
      this.scopeImageList = new System.Windows.Forms.ImageList(this.components);
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.contextLabel = new System.Windows.Forms.Label();
      this.atomicLabel = new System.Windows.Forms.Label();
      this.collectionLabel = new System.Windows.Forms.Label();
      this.parametersImageList = new System.Windows.Forms.ImageList(this.components);
      this.executionContextConextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.showValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.groupBox.SuspendLayout();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.executionContextGroupBox.SuspendLayout();
      this.scopeGroupBox.SuspendLayout();
      this.executionContextConextMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox
      // 
      this.groupBox.Controls.Add(this.splitContainer1);
      this.groupBox.Controls.Add(this.nameTextBox);
      this.groupBox.Controls.Add(this.contextLabel);
      this.groupBox.Controls.Add(this.atomicLabel);
      this.groupBox.Controls.Add(this.collectionLabel);
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
      this.executionContextTreeView.ContextMenuStrip = this.executionContextConextMenu;
      this.executionContextTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.executionContextTreeView.ImageIndex = 0;
      this.executionContextTreeView.ImageList = this.executionContextImageList;
      this.executionContextTreeView.Location = new System.Drawing.Point(3, 16);
      this.executionContextTreeView.Name = "executionContextTreeView";
      this.executionContextTreeView.SelectedImageIndex = 0;
      this.executionContextTreeView.ShowNodeToolTips = true;
      this.executionContextTreeView.Size = new System.Drawing.Size(236, 342);
      this.executionContextTreeView.TabIndex = 0;
      this.executionContextTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.executionContextTreeView_NodeMouseClick);
      this.executionContextTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.executionContextTreeView_NodeMouseDoubleClick);
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
      this.scopeTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scopeTreeView.ImageIndex = 0;
      this.scopeTreeView.ImageList = this.scopeImageList;
      this.scopeTreeView.Location = new System.Drawing.Point(3, 16);
      this.scopeTreeView.Name = "scopeTreeView";
      this.scopeTreeView.SelectedImageIndex = 0;
      this.scopeTreeView.ShowNodeToolTips = true;
      this.scopeTreeView.Size = new System.Drawing.Size(299, 342);
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
      this.nameTextBox.Location = new System.Drawing.Point(6, 19);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.ReadOnly = true;
      this.nameTextBox.Size = new System.Drawing.Size(395, 20);
      this.nameTextBox.TabIndex = 3;
      this.nameTextBox.DoubleClick += new System.EventHandler(this.nameTextBox_DoubleClick);
      // 
      // contextLabel
      // 
      this.contextLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.contextLabel.AutoSize = true;
      this.contextLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
      this.contextLabel.Location = new System.Drawing.Point(407, 22);
      this.contextLabel.Name = "contextLabel";
      this.contextLabel.Size = new System.Drawing.Size(43, 13);
      this.contextLabel.TabIndex = 2;
      this.contextLabel.Text = "Context";
      // 
      // atomicLabel
      // 
      this.atomicLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.atomicLabel.AutoSize = true;
      this.atomicLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
      this.atomicLabel.Location = new System.Drawing.Point(456, 22);
      this.atomicLabel.Name = "atomicLabel";
      this.atomicLabel.Size = new System.Drawing.Size(39, 13);
      this.atomicLabel.TabIndex = 1;
      this.atomicLabel.Text = "Atomic";
      // 
      // collectionLabel
      // 
      this.collectionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.collectionLabel.AutoSize = true;
      this.collectionLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
      this.collectionLabel.Location = new System.Drawing.Point(501, 22);
      this.collectionLabel.Name = "collectionLabel";
      this.collectionLabel.Size = new System.Drawing.Size(53, 13);
      this.collectionLabel.TabIndex = 0;
      this.collectionLabel.Text = "Collection";
      // 
      // parametersImageList
      // 
      this.parametersImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.parametersImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.parametersImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // executionContextConextMenu
      // 
      this.executionContextConextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showValueToolStripMenuItem});
      this.executionContextConextMenu.Name = "executionContextConextMenu";
      this.executionContextConextMenu.Size = new System.Drawing.Size(169, 48);
      this.executionContextConextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.executionContextConextMenu_Opening);
      // 
      // showValueToolStripMenuItem
      // 
      this.showValueToolStripMenuItem.Name = "showValueToolStripMenuItem";
      this.showValueToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
      this.showValueToolStripMenuItem.Text = "show actual value";
      this.showValueToolStripMenuItem.ToolTipText = "Try to obtain the parameter\'s actual value in the current execution context and o" +
    "pen it in a new view.";
      this.showValueToolStripMenuItem.Click += new System.EventHandler(this.ShowValue_Click);
      // 
      // OperationContentView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox);
      this.Name = "OperationContentView";
      this.Size = new System.Drawing.Size(563, 412);
      this.groupBox.ResumeLayout(false);
      this.groupBox.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.executionContextGroupBox.ResumeLayout(false);
      this.scopeGroupBox.ResumeLayout(false);
      this.executionContextConextMenu.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.Label contextLabel;
    private System.Windows.Forms.Label atomicLabel;
    private System.Windows.Forms.Label collectionLabel;
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
  }
}
