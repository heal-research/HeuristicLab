#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
namespace HeuristicLab.Clients.Hive.Administrator.Views {
  partial class ResourcesView {
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
      this.imageListSlaveGroups = new System.Windows.Forms.ImageList(this.components);
      this.splitSlaves = new System.Windows.Forms.SplitContainer();
      this.btnRefresh = new System.Windows.Forms.Button();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnRemoveGroup = new System.Windows.Forms.Button();
      this.btnAddGroup = new System.Windows.Forms.Button();
      this.treeView = new Hive.Views.TreeView.NoDoubleClickTreeView();
      this.tabSlaveGroup = new System.Windows.Forms.TabControl();
      this.tabDetails = new System.Windows.Forms.TabPage();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabSchedule = new System.Windows.Forms.TabPage();
      this.scheduleView = new HeuristicLab.Clients.Hive.Administrator.Views.ScheduleView();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.splitSlaves)).BeginInit();
      this.splitSlaves.Panel1.SuspendLayout();
      this.splitSlaves.Panel2.SuspendLayout();
      this.splitSlaves.SuspendLayout();
      this.tabSlaveGroup.SuspendLayout();
      this.tabDetails.SuspendLayout();
      this.tabSchedule.SuspendLayout();
      this.SuspendLayout();
      // 
      // imageListSlaveGroups
      // 
      this.imageListSlaveGroups.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imageListSlaveGroups.ImageSize = new System.Drawing.Size(16, 16);
      this.imageListSlaveGroups.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // splitSlaves
      // 
      this.splitSlaves.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitSlaves.Location = new System.Drawing.Point(3, 3);
      this.splitSlaves.Name = "splitSlaves";
      // 
      // splitSlaves.Panel1
      // 
      this.splitSlaves.Panel1.Controls.Add(this.btnRefresh);
      this.splitSlaves.Panel1.Controls.Add(this.btnSave);
      this.splitSlaves.Panel1.Controls.Add(this.btnRemoveGroup);
      this.splitSlaves.Panel1.Controls.Add(this.btnAddGroup);
      this.splitSlaves.Panel1.Controls.Add(this.treeView);
      // 
      // splitSlaves.Panel2
      // 
      this.splitSlaves.Panel2.Controls.Add(this.tabSlaveGroup);
      this.splitSlaves.Size = new System.Drawing.Size(847, 547);
      this.splitSlaves.SplitterDistance = 249;
      this.splitSlaves.TabIndex = 3;
      // 
      // btnRefresh
      // 
      this.btnRefresh.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.btnRefresh.Location = new System.Drawing.Point(3, 3);
      this.btnRefresh.Name = "btnRefresh";
      this.btnRefresh.Size = new System.Drawing.Size(24, 24);
      this.btnRefresh.TabIndex = 8;
      this.toolTip.SetToolTip(this.btnRefresh, "Fetch list from server.");
      this.btnRefresh.UseVisualStyleBackColor = true;
      this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
      // 
      // btnSave
      // 
      this.btnSave.Image = HeuristicLab.Common.Resources.VSImageLibrary.Save;
      this.btnSave.Location = new System.Drawing.Point(93, 3);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(24, 24);
      this.btnSave.TabIndex = 5;
      this.toolTip.SetToolTip(this.btnSave, "Store slave and group configuration on the server.");
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnRemoveGroup
      // 
      this.btnRemoveGroup.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      this.btnRemoveGroup.Location = new System.Drawing.Point(63, 3);
      this.btnRemoveGroup.Name = "btnRemoveGroup";
      this.btnRemoveGroup.Size = new System.Drawing.Size(24, 24);
      this.btnRemoveGroup.TabIndex = 2;
      this.toolTip.SetToolTip(this.btnRemoveGroup, "Remove a slave or a group.");
      this.btnRemoveGroup.UseVisualStyleBackColor = true;
      this.btnRemoveGroup.Click += new System.EventHandler(this.btnRemoveGroup_Click);
      // 
      // btnAddGroup
      // 
      this.btnAddGroup.Image = HeuristicLab.Common.Resources.VSImageLibrary.Add;
      this.btnAddGroup.Location = new System.Drawing.Point(33, 3);
      this.btnAddGroup.Name = "btnAddGroup";
      this.btnAddGroup.Size = new System.Drawing.Size(24, 24);
      this.btnAddGroup.TabIndex = 1;
      this.toolTip.SetToolTip(this.btnAddGroup, "Add a new group.");
      this.btnAddGroup.UseVisualStyleBackColor = true;
      this.btnAddGroup.Click += new System.EventHandler(this.btnAddGroup_Click);
      // 
      // treeView
      // 
      this.treeView.AllowDrop = true;
      this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.treeView.CheckBoxes = true;
      this.treeView.ImageIndex = 0;
      this.treeView.ImageList = this.imageListSlaveGroups;
      this.treeView.Location = new System.Drawing.Point(3, 33);
      this.treeView.Name = "treeView";
      this.treeView.SelectedImageIndex = 0;
      this.treeView.Size = new System.Drawing.Size(243, 511);
      this.treeView.TabIndex = 0;
      this.treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeSlaveGroup_ItemDrag);
      this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeSlaveGroup_MouseDown);
      this.treeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeSlaveGroup_BeforeSelect);
      this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeSlaveGroup_DragDrop);
      this.treeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeSlaveGroup_DragEnterOver);
      this.treeView.DragOver += new System.Windows.Forms.DragEventHandler(this.treeSlaveGroup_DragEnterOver);
      this.treeView.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeSlaveGroup_BeforeCheck);
      this.treeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeSlaveGroup_AfterCheck);
      // 
      // tabSlaveGroup
      // 
      this.tabSlaveGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabSlaveGroup.Controls.Add(this.tabDetails);
      this.tabSlaveGroup.Controls.Add(this.tabSchedule);
      this.tabSlaveGroup.Location = new System.Drawing.Point(3, 3);
      this.tabSlaveGroup.Name = "tabSlaveGroup";
      this.tabSlaveGroup.SelectedIndex = 0;
      this.tabSlaveGroup.Size = new System.Drawing.Size(585, 541);
      this.tabSlaveGroup.TabIndex = 1;
      //this.tabSlaveGroup.TabIndexChanged += TabSlaveGroup_TabIndexChanged;
      this.tabSlaveGroup.Selected += TabSlaveGroup_Selected;
      // 
      // tabDetails
      // 
      this.tabDetails.Controls.Add(this.viewHost);
      this.tabDetails.Location = new System.Drawing.Point(4, 22);
      this.tabDetails.Name = "tabDetails";
      this.tabDetails.Padding = new System.Windows.Forms.Padding(3);
      this.tabDetails.Size = new System.Drawing.Size(577, 515);
      this.tabDetails.TabIndex = 0;
      this.tabDetails.Text = "Details";
      this.tabDetails.UseVisualStyleBackColor = true;
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Enabled = true;
      this.viewHost.Location = new System.Drawing.Point(6, 6);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(565, 503);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // tabSchedule
      // 
      this.tabSchedule.Controls.Add(this.scheduleView);
      this.tabSchedule.Location = new System.Drawing.Point(4, 22);
      this.tabSchedule.Name = "tabSchedule";
      this.tabSchedule.Padding = new System.Windows.Forms.Padding(3);
      this.tabSchedule.Size = new System.Drawing.Size(577, 515);
      this.tabSchedule.TabIndex = 1;
      this.tabSchedule.Text = "Schedule";
      this.tabSchedule.UseVisualStyleBackColor = true;
      // 
      // scheduleView
      // 
      this.scheduleView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.scheduleView.Caption = "ScheduleView";
      this.scheduleView.Content = null;
      this.scheduleView.Location = new System.Drawing.Point(6, 6);
      this.scheduleView.Name = "scheduleView";
      this.scheduleView.ReadOnly = false;
      this.scheduleView.Size = new System.Drawing.Size(565, 503);
      this.scheduleView.TabIndex = 0;
      // 
      // ResourcesView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitSlaves);
      this.Name = "ResourcesView";
      this.Size = new System.Drawing.Size(853, 553);
      this.Load += new System.EventHandler(this.ResourcesView_Load);
      this.Disposed += new System.EventHandler(this.ResourcesView_Disposed);
      this.splitSlaves.Panel1.ResumeLayout(false);
      this.splitSlaves.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitSlaves)).EndInit();
      this.splitSlaves.ResumeLayout(false);
      this.tabSlaveGroup.ResumeLayout(false);
      this.tabDetails.ResumeLayout(false);
      this.tabSchedule.ResumeLayout(false);
      this.ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.SplitContainer splitSlaves;
    private System.Windows.Forms.Button btnRemoveGroup;
    private System.Windows.Forms.Button btnAddGroup;
    private HeuristicLab.Clients.Hive.Views.TreeView.NoDoubleClickTreeView treeView;
    private System.Windows.Forms.TabControl tabSlaveGroup;
    private System.Windows.Forms.TabPage tabDetails;
    private System.Windows.Forms.TabPage tabSchedule;
    private System.Windows.Forms.ImageList imageListSlaveGroups;
    private ScheduleView scheduleView;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnRefresh;
    private System.Windows.Forms.ToolTip toolTip;
    private MainForm.WindowsForms.ViewHost viewHost;
  }
}
