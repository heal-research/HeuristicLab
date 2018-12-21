#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Windows.Forms;

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  partial class HiveProjectSelector {
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
      this.projectsGroupBox = new System.Windows.Forms.GroupBox();
      this.projectsImageList = new System.Windows.Forms.ImageList(this.components);
      this.resourcesImageList = new System.Windows.Forms.ImageList(this.components);
      this.searchLabel = new System.Windows.Forms.Label();
      this.searchTextBox = new System.Windows.Forms.TextBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.projectsTreeView = new System.Windows.Forms.TreeView();
      this.resourcesTreeView = new HeuristicLab.Clients.Hive.Views.TreeView.NoDoubleClickTreeView();
      this.summaryGroupBox = new System.Windows.Forms.GroupBox();
      this.coresLabel = new System.Windows.Forms.Label();
      this.coresSummaryLabel = new System.Windows.Forms.Label();
      this.memoryLabel = new System.Windows.Forms.Label();
      this.memorySummaryLabel = new System.Windows.Forms.Label();
      this.projectsGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.summaryGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // projectsGroupBox
      // 
      this.projectsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.projectsGroupBox.Controls.Add(this.searchLabel);
      this.projectsGroupBox.Controls.Add(this.splitContainer2);
      this.projectsGroupBox.Controls.Add(this.searchTextBox);
      this.projectsGroupBox.Location = new System.Drawing.Point(3, 3);
      this.projectsGroupBox.Name = "projectsGroupBox";
      this.projectsGroupBox.Size = new System.Drawing.Size(426, 461);
      this.projectsGroupBox.TabIndex = 0;
      this.projectsGroupBox.TabStop = false;
      this.projectsGroupBox.Text = "Available Projects";
      // 
      // projectsImageList
      // 
      this.projectsImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.projectsImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.projectsImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // resourcesImageList
      // 
      this.resourcesImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.resourcesImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.resourcesImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // searchLabel
      // 
      this.searchLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Zoom;
      this.searchLabel.Location = new System.Drawing.Point(6, 19);
      this.searchLabel.Name = "searchLabel";
      this.searchLabel.Size = new System.Drawing.Size(20, 20);
      this.searchLabel.TabIndex = 0;
      this.toolTip.SetToolTip(this.searchLabel, "Enter string to search for resources");
      // 
      // searchTextBox
      // 
      this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.searchTextBox.Location = new System.Drawing.Point(32, 19);
      this.searchTextBox.Name = "searchTextBox";
      this.searchTextBox.Size = new System.Drawing.Size(388, 20);
      this.searchTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.searchTextBox, "Enter string to search for resources");
      this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
      this.searchTextBox.MouseDown += new MouseEventHandler(this.searchTextBox_MouseDown);
      // 
      // splitContainer2
      // 
      this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer2.Location = new System.Drawing.Point(6, 45);
      this.splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.projectsTreeView);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.resourcesTreeView);
      this.splitContainer2.Size = new System.Drawing.Size(414, 410);
      this.splitContainer2.SplitterDistance = 204;
      this.splitContainer2.TabIndex = 4;
      // 
      // projectsTreeView
      // 
      this.projectsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.projectsTreeView.HideSelection = false;
      this.projectsTreeView.ImageIndex = 0;
      this.projectsTreeView.ImageList = this.projectsImageList;
      this.projectsTreeView.Location = new System.Drawing.Point(0, 0);
      this.projectsTreeView.Name = "projectsTreeView";
      this.projectsTreeView.SelectedImageIndex = 0;
      this.projectsTreeView.ShowNodeToolTips = true;
      this.projectsTreeView.Size = new System.Drawing.Size(204, 410);
      this.projectsTreeView.TabIndex = 3;
      this.projectsTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.projectsTreeView_MouseDown);
      this.projectsTreeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.projectsTreeView_MouseDoubleClick);
      this.projectsTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.projectsTreeView_BeforeSelect);
      this.projectsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.projectsTreeView_AfterSelect);
      // 
      // resourcesTreeView
      // 
      this.resourcesTreeView.CheckBoxes = true;
      this.resourcesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resourcesTreeView.ImageIndex = 0;
      this.resourcesTreeView.ImageList = this.resourcesImageList;
      this.resourcesTreeView.Location = new System.Drawing.Point(0, 0);
      this.resourcesTreeView.Name = "resourcesTreeView";
      this.resourcesTreeView.SelectedImageIndex = 0;
      this.resourcesTreeView.Size = new System.Drawing.Size(206, 410);
      this.resourcesTreeView.TabIndex = 0;
      this.resourcesTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.resourcesTreeView_NodeMouseDoubleClick);
      this.resourcesTreeView.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.resourcesTreeView_BeforeCheck);
      this.resourcesTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.resourcesTreeView_AfterCheck);
      this.resourcesTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.resourcesTreeView_MouseDown);
      this.resourcesTreeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.resourcesTreeView_BeforeSelect);
      // 
      // summaryGroupBox
      // 
      this.summaryGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.summaryGroupBox.Controls.Add(this.memorySummaryLabel);
      this.summaryGroupBox.Controls.Add(this.memoryLabel);
      this.summaryGroupBox.Controls.Add(this.coresSummaryLabel);
      this.summaryGroupBox.Controls.Add(this.coresLabel);
      this.summaryGroupBox.Location = new System.Drawing.Point(3, 470);
      this.summaryGroupBox.Name = "summaryGroupBox";
      this.summaryGroupBox.Size = new System.Drawing.Size(426, 71);
      this.summaryGroupBox.TabIndex = 1;
      this.summaryGroupBox.TabStop = false;
      this.summaryGroupBox.Text = "Computing Resources";
      // 
      // coresLabel
      // 
      this.coresLabel.AutoSize = true;
      this.coresLabel.Location = new System.Drawing.Point(6, 22);
      this.coresLabel.Name = "coresLabel";
      this.coresLabel.Size = new System.Drawing.Size(37, 13);
      this.coresLabel.TabIndex = 1;
      this.coresLabel.Text = "Cores:";
      // 
      // coresSummaryLabel
      // 
      this.coresSummaryLabel.AutoSize = true;
      this.coresSummaryLabel.Location = new System.Drawing.Point(59, 22);
      this.coresSummaryLabel.Name = "coresSummaryLabel";
      this.coresSummaryLabel.Size = new System.Drawing.Size(124, 13);
      this.coresSummaryLabel.TabIndex = 5;
      this.coresSummaryLabel.Text = "0 Total (0 Free / 0 Used)";
      // 
      // memoryLabel
      // 
      this.memoryLabel.AutoSize = true;
      this.memoryLabel.Location = new System.Drawing.Point(6, 48);
      this.memoryLabel.Name = "memoryLabel";
      this.memoryLabel.Size = new System.Drawing.Size(47, 13);
      this.memoryLabel.TabIndex = 7;
      this.memoryLabel.Text = "Memory:";
      // 
      // memorySummaryLabel
      // 
      this.memorySummaryLabel.AutoSize = true;
      this.memorySummaryLabel.Location = new System.Drawing.Point(59, 48);
      this.memorySummaryLabel.Name = "memorySummaryLabel";
      this.memorySummaryLabel.Size = new System.Drawing.Size(223, 13);
      this.memorySummaryLabel.TabIndex = 8;
      this.memorySummaryLabel.Text = "0.00 GB Total (0.00 GB Free / 0.00 GB Used)";
      // 
      // HiveProjectSelector
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.summaryGroupBox);
      this.Controls.Add(this.projectsGroupBox);
      this.Name = "HiveProjectSelector";
      this.Size = new System.Drawing.Size(432, 544);
      this.Load += new System.EventHandler(this.HiveProjectSelector_Load);
      this.projectsGroupBox.ResumeLayout(false);
      this.projectsGroupBox.PerformLayout();
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
      this.splitContainer2.ResumeLayout(false);
      this.summaryGroupBox.ResumeLayout(false);
      this.summaryGroupBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.GroupBox projectsGroupBox;
    protected System.Windows.Forms.Label searchLabel;
    protected System.Windows.Forms.TextBox searchTextBox;
    protected System.Windows.Forms.ImageList projectsImageList;
    protected System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.ImageList resourcesImageList;
    private System.Windows.Forms.SplitContainer splitContainer2;
    protected System.Windows.Forms.TreeView projectsTreeView;
    protected HeuristicLab.Clients.Hive.Views.TreeView.NoDoubleClickTreeView resourcesTreeView;
    private System.Windows.Forms.GroupBox summaryGroupBox;
    protected System.Windows.Forms.Label coresLabel;
    protected System.Windows.Forms.Label memorySummaryLabel;
    protected System.Windows.Forms.Label memoryLabel;
    protected System.Windows.Forms.Label coresSummaryLabel;
  }
}
