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

using System;
using System.Windows.Forms;

namespace HeuristicLab.Core.Views {
  partial class ItemArrayView<T> {
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
        foreach (ListViewItem listViewItem in itemsListView.Items) {
          T item = listViewItem.Tag as T;
          if (item != null) item.ToStringChanged -= new EventHandler(Item_ToStringChanged);
        }
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
      this.moveUpButton = new System.Windows.Forms.Button();
      this.moveDownButton = new System.Windows.Forms.Button();
      this.itemsListView = new System.Windows.Forms.ListView();
      this.listViewColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.itemsGroupBox = new System.Windows.Forms.GroupBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
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
      this.splitContainer.Panel1.Controls.Add(this.moveUpButton);
      this.splitContainer.Panel1.Controls.Add(this.moveDownButton);
      this.splitContainer.Panel1.Controls.Add(this.itemsListView);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(493, 323);
      this.splitContainer.SplitterDistance = 200;
      this.splitContainer.TabIndex = 0;
      // 
      // moveUpButton
      // 
      this.moveUpButton.Enabled = false;
      this.moveUpButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.ArrowUp;
      this.moveUpButton.Location = new System.Drawing.Point(3, 3);
      this.moveUpButton.Name = "moveUpButton";
      this.moveUpButton.Size = new System.Drawing.Size(24, 24);
      this.moveUpButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.moveUpButton, "Move Up");
      this.moveUpButton.UseVisualStyleBackColor = true;
      this.moveUpButton.Click += new System.EventHandler(this.moveUpButton_Click);
      // 
      // moveDownButton
      // 
      this.moveDownButton.Enabled = false;
      this.moveDownButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.ArrowDown;
      this.moveDownButton.Location = new System.Drawing.Point(33, 3);
      this.moveDownButton.Name = "moveDownButton";
      this.moveDownButton.Size = new System.Drawing.Size(24, 24);
      this.moveDownButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.moveDownButton, "Move Down");
      this.moveDownButton.UseVisualStyleBackColor = true;
      this.moveDownButton.Click += new System.EventHandler(this.moveDownButton_Click);
      // 
      // itemsListView
      // 
      this.itemsListView.AllowDrop = true;
      this.itemsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.itemsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.listViewColumnHeader});
      this.itemsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.itemsListView.HideSelection = false;
      this.itemsListView.Location = new System.Drawing.Point(3, 33);
      this.itemsListView.Name = "itemsListView";
      this.itemsListView.ShowItemToolTips = true;
      this.itemsListView.Size = new System.Drawing.Size(194, 286);
      this.itemsListView.SmallImageList = this.imageList;
      this.itemsListView.TabIndex = 2;
      this.itemsListView.UseCompatibleStateImageBehavior = false;
      this.itemsListView.View = System.Windows.Forms.View.Details;
      this.itemsListView.SelectedIndexChanged += new System.EventHandler(this.itemsListView_SelectedIndexChanged);
      this.itemsListView.SizeChanged += new System.EventHandler(this.itemsListView_SizeChanged);
      this.itemsListView.DoubleClick += new System.EventHandler(this.itemsListView_DoubleClick);
      this.itemsListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.itemsListView_DragDrop);
      this.itemsListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.itemsListView_DragEnterOver);
      this.itemsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.itemsListView_KeyDown);
      this.itemsListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.itemsListView_ItemDrag);
      this.itemsListView.DragOver += new System.Windows.Forms.DragEventHandler(this.itemsListView_DragEnterOver);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsGroupBox.Controls.Add(this.viewHost);
      this.detailsGroupBox.Location = new System.Drawing.Point(3, 27);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(283, 294);
      this.detailsGroupBox.TabIndex = 0;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Content = null;
      this.viewHost.Location = new System.Drawing.Point(6, 19);
      this.viewHost.Name = "viewHost";
      this.viewHost.Size = new System.Drawing.Size(271, 269);
      this.viewHost.TabIndex = 0;
      // 
      // itemsGroupBox
      // 
      this.itemsGroupBox.Controls.Add(this.splitContainer);
      this.itemsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.itemsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.itemsGroupBox.Name = "itemsGroupBox";
      this.itemsGroupBox.Size = new System.Drawing.Size(499, 342);
      this.itemsGroupBox.TabIndex = 0;
      this.itemsGroupBox.TabStop = false;
      this.itemsGroupBox.Text = "Items";
      // 
      // ItemArrayView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.itemsGroupBox);
      this.Name = "ItemArrayView";
      this.Size = new System.Drawing.Size(499, 342);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.SplitContainer splitContainer;
    protected System.Windows.Forms.ColumnHeader listViewColumnHeader;
    protected GroupBox itemsGroupBox;
    protected ListView itemsListView;
    protected GroupBox detailsGroupBox;
    protected Button moveUpButton;
    protected Button moveDownButton;
    protected ToolTip toolTip;
    protected ImageList imageList;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost viewHost;
  }
}
