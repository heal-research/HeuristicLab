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

namespace HeuristicLab.Core {
  partial class OperatorGraphView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (chooseOperatorDialog != null) chooseOperatorDialog.Dispose();
      if (graphTreeView.Nodes.Count > 0) {
        RemoveTreeNode(graphTreeView.Nodes[0]);
        graphTreeView.Nodes.Clear();
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OperatorGraphView));
      this.operatorsListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
      this.operatorsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.initialOperatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.addOperatorButton = new System.Windows.Forms.Button();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.operatorsGroupBox = new System.Windows.Forms.GroupBox();
      this.graphGroupBox = new System.Windows.Forms.GroupBox();
      this.graphTreeView = new System.Windows.Forms.TreeView();
      this.graphContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.breakpointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.viewToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.removeButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.operatorsContextMenuStrip.SuspendLayout();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.operatorsGroupBox.SuspendLayout();
      this.graphGroupBox.SuspendLayout();
      this.graphContextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // operatorsListView
      // 
      this.operatorsListView.AllowDrop = true;
      this.operatorsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
      this.operatorsListView.ContextMenuStrip = this.operatorsContextMenuStrip;
      this.operatorsListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.operatorsListView.HideSelection = false;
      this.operatorsListView.LabelEdit = true;
      this.operatorsListView.Location = new System.Drawing.Point(3, 16);
      this.operatorsListView.Name = "operatorsListView";
      this.operatorsListView.Size = new System.Drawing.Size(169, 243);
      this.operatorsListView.SmallImageList = this.imageList;
      this.operatorsListView.TabIndex = 0;
      this.operatorsListView.UseCompatibleStateImageBehavior = false;
      this.operatorsListView.View = System.Windows.Forms.View.Details;
      this.operatorsListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.operatorsListView_AfterLabelEdit);
      this.operatorsListView.SelectedIndexChanged += new System.EventHandler(this.operatorsListView_SelectedIndexChanged);
      this.operatorsListView.SizeChanged += new System.EventHandler(this.operatorsListView_SizeChanged);
      this.operatorsListView.DoubleClick += new System.EventHandler(this.operatorsListView_DoubleClick);
      this.operatorsListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.operatorsListView_DragDrop);
      this.operatorsListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.operatorsListView_DragEnter);
      this.operatorsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.operatorsListView_KeyDown);
      this.operatorsListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.operatorsListView_ItemDrag);
      this.operatorsListView.DragOver += new System.Windows.Forms.DragEventHandler(this.operatorsListView_DragOver);
      // 
      // operatorsContextMenuStrip
      // 
      this.operatorsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initialOperatorToolStripMenuItem,
            this.viewToolStripMenuItem});
      this.operatorsContextMenuStrip.Name = "operatorsContextMenuStrip";
      this.operatorsContextMenuStrip.Size = new System.Drawing.Size(159, 48);
      this.operatorsContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.operatorsContextMenuStrip_Opening);
      // 
      // initialOperatorToolStripMenuItem
      // 
      this.initialOperatorToolStripMenuItem.CheckOnClick = true;
      this.initialOperatorToolStripMenuItem.Name = "initialOperatorToolStripMenuItem";
      this.initialOperatorToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
      this.initialOperatorToolStripMenuItem.Text = "&Initial Operator";
      this.initialOperatorToolStripMenuItem.ToolTipText = "Set as initial operator";
      this.initialOperatorToolStripMenuItem.Click += new System.EventHandler(this.initialOperatorToolStripMenuItem_Click);
      // 
      // viewToolStripMenuItem
      // 
      this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
      this.viewToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
      this.viewToolStripMenuItem.Text = "&View...";
      this.viewToolStripMenuItem.ToolTipText = "View operator";
      this.viewToolStripMenuItem.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
      // 
      // imageList
      // 
      this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
      this.imageList.TransparentColor = System.Drawing.Color.Magenta;
      this.imageList.Images.SetKeyName(0, "Operator.bmp");
      this.imageList.Images.SetKeyName(1, "CombinedOperator.bmp");
      this.imageList.Images.SetKeyName(2, "ProgrammableOperator.bmp");
      // 
      // addOperatorButton
      // 
      this.addOperatorButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addOperatorButton.Location = new System.Drawing.Point(0, 268);
      this.addOperatorButton.Name = "addOperatorButton";
      this.addOperatorButton.Size = new System.Drawing.Size(75, 23);
      this.addOperatorButton.TabIndex = 0;
      this.addOperatorButton.Text = "&Add...";
      this.addOperatorButton.UseVisualStyleBackColor = true;
      this.addOperatorButton.Click += new System.EventHandler(this.addOperatorButton_Click);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.operatorsGroupBox);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.graphGroupBox);
      this.splitContainer1.Size = new System.Drawing.Size(475, 262);
      this.splitContainer1.SplitterDistance = 175;
      this.splitContainer1.TabIndex = 4;
      // 
      // operatorsGroupBox
      // 
      this.operatorsGroupBox.Controls.Add(this.operatorsListView);
      this.operatorsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.operatorsGroupBox.Name = "operatorsGroupBox";
      this.operatorsGroupBox.Size = new System.Drawing.Size(175, 262);
      this.operatorsGroupBox.TabIndex = 3;
      this.operatorsGroupBox.TabStop = false;
      this.operatorsGroupBox.Text = "&Operators";
      // 
      // graphGroupBox
      // 
      this.graphGroupBox.Controls.Add(this.graphTreeView);
      this.graphGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.graphGroupBox.Location = new System.Drawing.Point(0, 0);
      this.graphGroupBox.Name = "graphGroupBox";
      this.graphGroupBox.Size = new System.Drawing.Size(296, 262);
      this.graphGroupBox.TabIndex = 0;
      this.graphGroupBox.TabStop = false;
      this.graphGroupBox.Text = "&Graph";
      // 
      // graphTreeView
      // 
      this.graphTreeView.AllowDrop = true;
      this.graphTreeView.ContextMenuStrip = this.graphContextMenuStrip;
      this.graphTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.graphTreeView.HideSelection = false;
      this.graphTreeView.Location = new System.Drawing.Point(3, 16);
      this.graphTreeView.Name = "graphTreeView";
      this.graphTreeView.ShowNodeToolTips = true;
      this.graphTreeView.Size = new System.Drawing.Size(290, 243);
      this.graphTreeView.TabIndex = 0;
      this.graphTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.graphTreeView_BeforeExpand);
      this.graphTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.graphTreeView_DragDrop);
      this.graphTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.graphTreeView_AfterSelect);
      this.graphTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.graphTreeView_MouseDown);
      this.graphTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.graphTreeView_DragEnter);
      this.graphTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.graphTreeView_KeyDown);
      this.graphTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.graphTreeView_ItemDrag);
      this.graphTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.graphTreeView_DragOver);
      // 
      // graphContextMenuStrip
      // 
      this.graphContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.breakpointToolStripMenuItem,
            this.viewToolStripMenuItem1});
      this.graphContextMenuStrip.Name = "graphContextMenuStrip";
      this.graphContextMenuStrip.Size = new System.Drawing.Size(137, 48);
      this.graphContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.graphContextMenuStrip_Opening);
      // 
      // breakpointToolStripMenuItem
      // 
      this.breakpointToolStripMenuItem.CheckOnClick = true;
      this.breakpointToolStripMenuItem.Name = "breakpointToolStripMenuItem";
      this.breakpointToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
      this.breakpointToolStripMenuItem.Text = "&Breakpoint";
      this.breakpointToolStripMenuItem.ToolTipText = "Halt engine execution after executing the operator";
      this.breakpointToolStripMenuItem.Click += new System.EventHandler(this.breakpointToolStripMenuItem_Click);
      // 
      // viewToolStripMenuItem1
      // 
      this.viewToolStripMenuItem1.Name = "viewToolStripMenuItem1";
      this.viewToolStripMenuItem1.Size = new System.Drawing.Size(136, 22);
      this.viewToolStripMenuItem1.Text = "&View...";
      this.viewToolStripMenuItem1.ToolTipText = "View operator";
      this.viewToolStripMenuItem1.Click += new System.EventHandler(this.viewToolStripMenuItem_Click);
      // 
      // removeButton
      // 
      this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeButton.Location = new System.Drawing.Point(81, 268);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(75, 23);
      this.removeButton.TabIndex = 1;
      this.removeButton.Text = "&Remove";
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // OperatorGraphView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.addOperatorButton);
      this.Controls.Add(this.removeButton);
      this.Name = "OperatorGraphView";
      this.Size = new System.Drawing.Size(475, 291);
      this.operatorsContextMenuStrip.ResumeLayout(false);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.operatorsGroupBox.ResumeLayout(false);
      this.graphGroupBox.ResumeLayout(false);
      this.graphContextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button addOperatorButton;
    private System.Windows.Forms.ListView operatorsListView;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.GroupBox operatorsGroupBox;
    private System.Windows.Forms.Button removeButton;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ContextMenuStrip operatorsContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem initialOperatorToolStripMenuItem;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.GroupBox graphGroupBox;
    private System.Windows.Forms.TreeView graphTreeView;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.ContextMenuStrip graphContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem breakpointToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem1;
  }
}
