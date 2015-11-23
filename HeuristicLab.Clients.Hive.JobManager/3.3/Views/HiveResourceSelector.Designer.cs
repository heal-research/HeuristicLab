#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Clients.Hive.JobManager.Views {
  partial class HiveResourceSelector {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HiveResourceSelector));
      this.resourcesGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.resourcesTreeView = new System.Windows.Forms.TreeView();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.searchLabel = new System.Windows.Forms.Label();
      this.searchTextBox = new System.Windows.Forms.TextBox();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.resourcesGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // resourcesGroupBox
      // 
      this.resourcesGroupBox.Controls.Add(this.splitContainer);
      this.resourcesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resourcesGroupBox.Location = new System.Drawing.Point(0, 0);
      this.resourcesGroupBox.Name = "resourcesGroupBox";
      this.resourcesGroupBox.Size = new System.Drawing.Size(308, 365);
      this.resourcesGroupBox.TabIndex = 0;
      this.resourcesGroupBox.TabStop = false;
      this.resourcesGroupBox.Text = "Available Resources";
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(3, 16);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.resourcesTreeView);
      this.splitContainer.Panel1.Controls.Add(this.searchLabel);
      this.splitContainer.Panel1.Controls.Add(this.searchTextBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.descriptionTextBox);
      this.splitContainer.Size = new System.Drawing.Size(302, 346);
      this.splitContainer.SplitterDistance = 219;
      this.splitContainer.TabIndex = 0;
      // 
      // resourcesTreeView
      // 
      this.resourcesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.resourcesTreeView.CheckBoxes = true;
      this.resourcesTreeView.HideSelection = false;
      this.resourcesTreeView.ImageIndex = 0;
      this.resourcesTreeView.ImageList = this.imageList;
      this.resourcesTreeView.Location = new System.Drawing.Point(3, 29);
      this.resourcesTreeView.Name = "resourcesTreeView";
      this.resourcesTreeView.SelectedImageIndex = 0;
      this.resourcesTreeView.ShowNodeToolTips = true;
      this.resourcesTreeView.Size = new System.Drawing.Size(296, 196);
      this.resourcesTreeView.TabIndex = 2;
      this.resourcesTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.resourcesTreeView_AfterCheck);
      this.resourcesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.resourcesTreeView_AfterSelect);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // searchLabel
      // 
      this.searchLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Zoom;
      this.searchLabel.Location = new System.Drawing.Point(3, 3);
      this.searchLabel.Name = "searchLabel";
      this.searchLabel.Size = new System.Drawing.Size(20, 20);
      this.searchLabel.TabIndex = 0;
      this.toolTip.SetToolTip(this.searchLabel, "Enter string to search for resources");
      // 
      // searchTextBox
      // 
      this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.searchTextBox.Location = new System.Drawing.Point(29, 3);
      this.searchTextBox.Name = "searchTextBox";
      this.searchTextBox.Size = new System.Drawing.Size(270, 20);
      this.searchTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.searchTextBox, "Enter string to search for resources");
      this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.descriptionTextBox.Font = new System.Drawing.Font("Courier New", 8.25F);
      this.descriptionTextBox.Location = new System.Drawing.Point(3, 3);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.ReadOnly = true;
      this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.descriptionTextBox.Size = new System.Drawing.Size(296, 108);
      this.descriptionTextBox.TabIndex = 0;
      this.descriptionTextBox.WordWrap = false;
      // 
      // HiveResourceSelector
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.resourcesGroupBox);
      this.Name = "HiveResourceSelector";
      this.Size = new System.Drawing.Size(308, 365);
      this.resourcesGroupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.GroupBox resourcesGroupBox;
    protected System.Windows.Forms.SplitContainer splitContainer;
    protected System.Windows.Forms.Label searchLabel;
    protected System.Windows.Forms.TextBox searchTextBox;
    protected System.Windows.Forms.TreeView resourcesTreeView;
    protected System.Windows.Forms.TextBox descriptionTextBox;
    protected System.Windows.Forms.ImageList imageList;
    protected System.Windows.Forms.ToolTip toolTip;

  }
}
