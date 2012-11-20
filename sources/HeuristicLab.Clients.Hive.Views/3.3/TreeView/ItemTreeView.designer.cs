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

using HeuristicLab.Common.Resources;
namespace HeuristicLab.Clients.Hive.Views {
  partial class ItemTreeView<T> {
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.showDetailsCheckBox = new System.Windows.Forms.CheckBox();
      this.removeButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.treeView = new System.Windows.Forms.TreeView();
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.detailsViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.contextMenuStrip.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.showDetailsCheckBox);
      this.splitContainer.Panel1.Controls.Add(this.removeButton);
      this.splitContainer.Panel1.Controls.Add(this.addButton);
      this.splitContainer.Panel1.Controls.Add(this.treeView);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(624, 463);
      this.splitContainer.SplitterDistance = 200;
      this.splitContainer.TabIndex = 0;
      // 
      // showDetailsCheckBox
      // 
      this.showDetailsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.showDetailsCheckBox.Checked = true;
      this.showDetailsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showDetailsCheckBox.Image = VSImageLibrary.Properties;
      this.showDetailsCheckBox.Location = new System.Drawing.Point(63, 3);
      this.showDetailsCheckBox.Name = "showDetailsCheckBox";
      this.showDetailsCheckBox.Size = new System.Drawing.Size(24, 24);
      this.showDetailsCheckBox.TabIndex = 5;
      this.showDetailsCheckBox.UseVisualStyleBackColor = true;
      this.showDetailsCheckBox.CheckedChanged += new System.EventHandler(this.showDetailsCheckBox_CheckedChanged);
      // 
      // removeButton
      // 
      this.removeButton.Enabled = false;
      this.removeButton.Image = VSImageLibrary.Remove;
      this.removeButton.Location = new System.Drawing.Point(33, 3);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(24, 24);
      this.removeButton.TabIndex = 4;
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // addButton
      // 
      this.addButton.Image = VSImageLibrary.Add;
      this.addButton.Location = new System.Drawing.Point(3, 3);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(24, 24);
      this.addButton.TabIndex = 1;
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
      // 
      // treeView
      // 
      this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.treeView.ContextMenuStrip = this.contextMenuStrip;
      this.treeView.HideSelection = false;
      this.treeView.ImageIndex = 0;
      this.treeView.ImageList = this.imageList;
      this.treeView.Location = new System.Drawing.Point(0, 33);
      this.treeView.Name = "treeView";
      this.treeView.SelectedImageIndex = 0;
      this.treeView.Size = new System.Drawing.Size(200, 430);
      this.treeView.TabIndex = 0;
      this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
      this.treeView.Click += new System.EventHandler(this.treeView_Click);
      this.treeView.DoubleClick += new System.EventHandler(this.treeView_DoubleClick);
      this.treeView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseClick);
      this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDown);
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(153, 70);
      this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Controls.Add(this.detailsViewHost);
      this.detailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(420, 463);
      this.detailsGroupBox.TabIndex = 1;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // detailsViewHost
      // 
      this.detailsViewHost.Caption = "View";
      this.detailsViewHost.Content = null;
      this.detailsViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailsViewHost.Enabled = false;
      this.detailsViewHost.Location = new System.Drawing.Point(3, 16);
      this.detailsViewHost.Name = "detailsViewHost";
      this.detailsViewHost.ReadOnly = false;
      this.detailsViewHost.Size = new System.Drawing.Size(414, 444);
      this.detailsViewHost.TabIndex = 0;
      this.detailsViewHost.ViewsLabelVisible = true;
      this.detailsViewHost.ViewType = null;
      // 
      // ItemTreeView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.splitContainer);
      this.Name = "ItemTreeView";
      this.Size = new System.Drawing.Size(624, 463);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.contextMenuStrip.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    protected System.Windows.Forms.TreeView treeView;
    private MainForm.WindowsForms.ViewHost detailsViewHost;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    protected System.Windows.Forms.CheckBox showDetailsCheckBox;
    protected System.Windows.Forms.Button removeButton;
    protected System.Windows.Forms.Button addButton;
  }
}
