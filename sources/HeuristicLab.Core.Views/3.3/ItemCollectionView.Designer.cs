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

using System.Windows.Forms;

namespace HeuristicLab.Core.Views {
  partial class ItemCollectionView<T> {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.itemsListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.sortDescendingButton = new System.Windows.Forms.Button();
      this.sortAscendingButton = new System.Windows.Forms.Button();
      this.removeButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.itemsGroupBox = new System.Windows.Forms.GroupBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.showDetailsCheckBox = new System.Windows.Forms.CheckBox();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.itemsGroupBox.SuspendLayout();
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
      this.splitContainer.Panel1.Controls.Add(this.itemsListView);
      this.splitContainer.Panel1.Controls.Add(this.sortDescendingButton);
      this.splitContainer.Panel1.Controls.Add(this.sortAscendingButton);
      this.splitContainer.Panel1.Controls.Add(this.removeButton);
      this.splitContainer.Panel1.Controls.Add(this.addButton);
      this.splitContainer.Panel1MinSize = 100;
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(526, 364);
      this.splitContainer.SplitterDistance = 250;
      this.splitContainer.TabIndex = 0;
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
      this.itemsListView.Size = new System.Drawing.Size(244, 327);
      this.itemsListView.SmallImageList = this.imageList;
      this.itemsListView.TabIndex = 5;
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
      // sortDescendingButton
      // 
      this.sortDescendingButton.Enabled = false;
      this.sortDescendingButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.SortUp;
      this.sortDescendingButton.Location = new System.Drawing.Point(33, 3);
      this.sortDescendingButton.Name = "sortDescendingButton";
      this.sortDescendingButton.Size = new System.Drawing.Size(24, 24);
      this.sortDescendingButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.sortDescendingButton, "Sort Descending");
      this.sortDescendingButton.UseVisualStyleBackColor = true;
      this.sortDescendingButton.Click += new System.EventHandler(this.sortDescendingButton_Click);
      // 
      // sortAscendingButton
      // 
      this.sortAscendingButton.Enabled = false;
      this.sortAscendingButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Sort;
      this.sortAscendingButton.Location = new System.Drawing.Point(63, 3);
      this.sortAscendingButton.Name = "sortAscendingButton";
      this.sortAscendingButton.Size = new System.Drawing.Size(24, 24);
      this.sortAscendingButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.sortAscendingButton, "Sort Ascending");
      this.sortAscendingButton.UseVisualStyleBackColor = true;
      this.sortAscendingButton.Click += new System.EventHandler(this.sortAscendingButton_Click);
      // 
      // removeButton
      // 
      this.removeButton.Enabled = false;
      this.removeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      this.removeButton.Location = new System.Drawing.Point(93, 3);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(24, 24);
      this.removeButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.removeButton, "Remove");
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // addButton
      // 
      this.addButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Add;
      this.addButton.Location = new System.Drawing.Point(3, 3);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(24, 24);
      this.addButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.addButton, "Add");
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsGroupBox.Controls.Add(this.viewHost);
      this.detailsGroupBox.Location = new System.Drawing.Point(3, 27);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(266, 335);
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
      this.viewHost.Size = new System.Drawing.Size(254, 310);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewType = null;
      // 
      // itemsGroupBox
      // 
      this.itemsGroupBox.Controls.Add(this.splitContainer);
      this.itemsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.itemsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.itemsGroupBox.Name = "itemsGroupBox";
      this.itemsGroupBox.Size = new System.Drawing.Size(532, 383);
      this.itemsGroupBox.TabIndex = 0;
      this.itemsGroupBox.TabStop = false;
      this.itemsGroupBox.Text = "Items";
      // 
      // showDetailsCheckBox
      // 
      this.showDetailsCheckBox.Appearance = System.Windows.Forms.Appearance.Button;
      this.showDetailsCheckBox.Checked = true;
      this.showDetailsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.showDetailsCheckBox.Image = HeuristicLab.Common.Resources.VSImageLibrary.Properties;
      this.showDetailsCheckBox.Location = new System.Drawing.Point(123, 3);
      this.showDetailsCheckBox.Name = "showDetailsCheckBox";
      this.showDetailsCheckBox.Size = new System.Drawing.Size(24, 24);
      this.showDetailsCheckBox.TabIndex = 4;
      this.toolTip.SetToolTip(this.showDetailsCheckBox, "Show/Hide Details");
      this.showDetailsCheckBox.UseVisualStyleBackColor = true;
      this.showDetailsCheckBox.CheckedChanged += new System.EventHandler(this.showDetailsCheckBox_CheckedChanged);
      // 
      // ItemCollectionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.itemsGroupBox);
      this.Name = "ItemCollectionView";
      this.Size = new System.Drawing.Size(532, 383);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.SplitContainer splitContainer;
    protected System.Windows.Forms.ColumnHeader columnHeader1;
    protected GroupBox itemsGroupBox;
    protected ListView itemsListView;
    protected GroupBox detailsGroupBox;
    protected Button addButton;
    protected Button removeButton;
    protected ToolTip toolTip;
    protected ImageList imageList;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost viewHost;
    protected Button sortAscendingButton;
    protected Button sortDescendingButton;
    protected CheckBox showDetailsCheckBox;
  }
}
