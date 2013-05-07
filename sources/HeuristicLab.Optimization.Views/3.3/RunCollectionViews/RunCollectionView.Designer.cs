#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization.Views {
  partial class RunCollectionView {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.showDetailsCheckBox = new System.Windows.Forms.CheckBox();
      this.clearButton = new System.Windows.Forms.Button();
      this.toolStrip = new System.Windows.Forms.ToolStrip();
      this.analyzeRunsToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
      this.itemsListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.removeButton = new System.Windows.Forms.Button();
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.itemsGroupBox = new System.Windows.Forms.GroupBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.tabControl = new HeuristicLab.MainForm.WindowsForms.DragOverTabControl();
      this.runPage = new System.Windows.Forms.TabPage();
      this.constraintPage = new System.Windows.Forms.TabPage();
      this.runCollectionConstraintCollectionView = new HeuristicLab.Optimization.Views.RunCollectionConstraintCollectionView();
      this.modifiersPage = new System.Windows.Forms.TabPage();
      this.runCollectionModifiersListView = new HeuristicLab.Optimization.Views.RunCollectionModifiersListView();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.toolStrip.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.itemsGroupBox.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.runPage.SuspendLayout();
      this.constraintPage.SuspendLayout();
      this.modifiersPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.splitContainer.Location = new System.Drawing.Point(3, 16);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.showDetailsCheckBox);
      this.splitContainer.Panel1.Controls.Add(this.clearButton);
      this.splitContainer.Panel1.Controls.Add(this.toolStrip);
      this.splitContainer.Panel1.Controls.Add(this.itemsListView);
      this.splitContainer.Panel1.Controls.Add(this.removeButton);
      this.splitContainer.Panel1MinSize = 100;
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(512, 332);
      this.splitContainer.SplitterDistance = 250;
      this.splitContainer.TabIndex = 0;
      // 
      // showDetailsCheckBox
      // 
      this.showDetailsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.showDetailsCheckBox.Checked = true;
      this.showDetailsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showDetailsCheckBox.Image = HeuristicLab.Common.Resources.VSImageLibrary.Properties;
      this.showDetailsCheckBox.Location = new System.Drawing.Point(87, 3);
      this.showDetailsCheckBox.Name = "showDetailsCheckBox";
      this.showDetailsCheckBox.Size = new System.Drawing.Size(24, 24);
      this.showDetailsCheckBox.TabIndex = 2;
      this.toolTip.SetToolTip(this.showDetailsCheckBox, "Show/Hide Details");
      this.showDetailsCheckBox.UseVisualStyleBackColor = true;
      this.showDetailsCheckBox.CheckedChanged += new System.EventHandler(this.showDetailsCheckBox_CheckedChanged);
      // 
      // clearButton
      // 
      this.clearButton.Enabled = false;
      this.clearButton.Location = new System.Drawing.Point(33, 3);
      this.clearButton.Name = "clearButton";
      this.clearButton.Size = new System.Drawing.Size(48, 24);
      this.clearButton.TabIndex = 1;
      this.clearButton.Text = "&Clear";
      this.toolTip.SetToolTip(this.clearButton, "Remove All Runs");
      this.clearButton.UseVisualStyleBackColor = true;
      this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
      // 
      // toolStrip
      // 
      this.toolStrip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.toolStrip.AutoSize = false;
      this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
      this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.analyzeRunsToolStripDropDownButton});
      this.toolStrip.Location = new System.Drawing.Point(114, 3);
      this.toolStrip.Name = "toolStrip";
      this.toolStrip.Size = new System.Drawing.Size(133, 24);
      this.toolStrip.TabIndex = 3;
      this.toolStrip.Text = "toolStrip1";
      // 
      // analyzeRunsToolStripDropDownButton
      // 
      this.analyzeRunsToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.analyzeRunsToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.analyzeRunsToolStripDropDownButton.Name = "analyzeRunsToolStripDropDownButton";
      this.analyzeRunsToolStripDropDownButton.Size = new System.Drawing.Size(99, 21);
      this.analyzeRunsToolStripDropDownButton.Text = "&Analyze Runs...";
      this.analyzeRunsToolStripDropDownButton.ToolTipText = "Show Run Analysis Views";
      // 
      // itemsListView
      // 
      this.itemsListView.AllowDrop = true;
      this.itemsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.itemsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
      this.itemsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.itemsListView.HideSelection = false;
      this.itemsListView.Location = new System.Drawing.Point(3, 33);
      this.itemsListView.Name = "itemsListView";
      this.itemsListView.ShowItemToolTips = true;
      this.itemsListView.Size = new System.Drawing.Size(244, 295);
      this.itemsListView.SmallImageList = this.imageList;
      this.itemsListView.TabIndex = 4;
      this.itemsListView.UseCompatibleStateImageBehavior = false;
      this.itemsListView.View = System.Windows.Forms.View.Details;
      this.itemsListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.itemsListView_ItemDrag);
      this.itemsListView.SelectedIndexChanged += new System.EventHandler(this.itemsListView_SelectedIndexChanged);
      this.itemsListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.itemsListView_DragDrop);
      this.itemsListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.itemsListView_DragEnter);
      this.itemsListView.DragOver += new System.Windows.Forms.DragEventHandler(this.itemsListView_DragOver);
      this.itemsListView.DoubleClick += new System.EventHandler(this.itemsListView_DoubleClick);
      this.itemsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemsListView_KeyDown);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // removeButton
      // 
      this.removeButton.Enabled = false;
      this.removeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      this.removeButton.Location = new System.Drawing.Point(3, 3);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(24, 24);
      this.removeButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.removeButton, "Remove");
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsGroupBox.Controls.Add(this.viewHost);
      this.detailsGroupBox.Location = new System.Drawing.Point(3, 27);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(252, 303);
      this.detailsGroupBox.TabIndex = 0;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Location = new System.Drawing.Point(6, 19);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(240, 278);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewType = null;
      // 
      // itemsGroupBox
      // 
      this.itemsGroupBox.Controls.Add(this.splitContainer);
      this.itemsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.itemsGroupBox.Location = new System.Drawing.Point(3, 3);
      this.itemsGroupBox.Name = "itemsGroupBox";
      this.itemsGroupBox.Size = new System.Drawing.Size(518, 351);
      this.itemsGroupBox.TabIndex = 0;
      this.itemsGroupBox.TabStop = false;
      this.itemsGroupBox.Text = "Items";
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.runPage);
      this.tabControl.Controls.Add(this.constraintPage);
      this.tabControl.Controls.Add(this.modifiersPage);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(532, 383);
      this.tabControl.TabIndex = 0;
      // 
      // runPage
      // 
      this.runPage.Controls.Add(this.itemsGroupBox);
      this.runPage.Location = new System.Drawing.Point(4, 22);
      this.runPage.Name = "runPage";
      this.runPage.Padding = new System.Windows.Forms.Padding(3);
      this.runPage.Size = new System.Drawing.Size(524, 357);
      this.runPage.TabIndex = 0;
      this.runPage.Text = "Runs";
      this.runPage.UseVisualStyleBackColor = true;
      // 
      // constraintPage
      // 
      this.constraintPage.Controls.Add(this.runCollectionConstraintCollectionView);
      this.constraintPage.Location = new System.Drawing.Point(4, 22);
      this.constraintPage.Name = "constraintPage";
      this.constraintPage.Padding = new System.Windows.Forms.Padding(3);
      this.constraintPage.Size = new System.Drawing.Size(524, 357);
      this.constraintPage.TabIndex = 1;
      this.constraintPage.Text = "Filtering";
      this.constraintPage.UseVisualStyleBackColor = true;
      // 
      // runCollectionConstraintCollectionView
      // 
      this.runCollectionConstraintCollectionView.Caption = "ConstraintCollection View";
      this.runCollectionConstraintCollectionView.Content = null;
      this.runCollectionConstraintCollectionView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.runCollectionConstraintCollectionView.Location = new System.Drawing.Point(3, 3);
      this.runCollectionConstraintCollectionView.Name = "runCollectionConstraintCollectionView";
      this.runCollectionConstraintCollectionView.ReadOnly = false;
      this.runCollectionConstraintCollectionView.Size = new System.Drawing.Size(518, 351);
      this.runCollectionConstraintCollectionView.TabIndex = 0;
      // 
      // modifiersPage
      // 
      this.modifiersPage.Controls.Add(this.runCollectionModifiersListView);
      this.modifiersPage.Location = new System.Drawing.Point(4, 22);
      this.modifiersPage.Name = "modifiersPage";
      this.modifiersPage.Padding = new System.Windows.Forms.Padding(3);
      this.modifiersPage.Size = new System.Drawing.Size(524, 357);
      this.modifiersPage.TabIndex = 2;
      this.modifiersPage.Text = "Modification";
      this.modifiersPage.UseVisualStyleBackColor = true;
      // 
      // runCollectionModifiersListView
      // 
      this.runCollectionModifiersListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.runCollectionModifiersListView.Caption = "Modifier View";
      this.runCollectionModifiersListView.Content = null;
      this.runCollectionModifiersListView.Evaluator = null;
      this.runCollectionModifiersListView.Location = new System.Drawing.Point(3, 3);
      this.runCollectionModifiersListView.Name = "runCollectionModifiersListView";
      this.runCollectionModifiersListView.ReadOnly = false;
      this.runCollectionModifiersListView.Size = new System.Drawing.Size(518, 351);
      this.runCollectionModifiersListView.TabIndex = 0;
      // 
      // RunCollectionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.tabControl);
      this.Name = "RunCollectionView";
      this.Size = new System.Drawing.Size(532, 383);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.toolStrip.ResumeLayout(false);
      this.toolStrip.PerformLayout();
      this.detailsGroupBox.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.runPage.ResumeLayout(false);
      this.constraintPage.ResumeLayout(false);
      this.modifiersPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private GroupBox itemsGroupBox;
    private ListView itemsListView;
    private GroupBox detailsGroupBox;
    private Button removeButton;
    private ToolTip toolTip;
    private ImageList imageList;
    private HeuristicLab.MainForm.WindowsForms.ViewHost viewHost;
    private ToolStrip toolStrip;
    private ToolStripDropDownButton analyzeRunsToolStripDropDownButton;
    private HeuristicLab.MainForm.WindowsForms.DragOverTabControl tabControl;
    private TabPage runPage;
    private TabPage constraintPage;
    private RunCollectionConstraintCollectionView runCollectionConstraintCollectionView;
    private Button clearButton;
    private CheckBox showDetailsCheckBox;
    private TabPage modifiersPage;
    private RunCollectionModifiersListView runCollectionModifiersListView;
  }
}
