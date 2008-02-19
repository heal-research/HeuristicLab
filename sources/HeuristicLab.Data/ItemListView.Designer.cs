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

using System;
using System.Windows.Forms;
using HeuristicLab.Core;

namespace HeuristicLab.Data {
  partial class ItemListView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (chooseItemDialog != null) chooseItemDialog.Dispose();
      foreach (ListViewItem item in itemsListView.Items)
        ((IItem)item.Tag).Changed -= new EventHandler(Item_Changed);
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
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.itemsGroupBox = new System.Windows.Forms.GroupBox();
      this.removeButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.itemsListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.itemTypeLabel = new System.Windows.Forms.Label();
      this.setTypeButton = new System.Windows.Forms.Button();
      this.typeTextBox = new System.Windows.Forms.TextBox();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.itemsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Enabled = false;
      this.splitContainer.Location = new System.Drawing.Point(0, 27);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.itemsGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(423, 246);
      this.splitContainer.SplitterDistance = 198;
      this.splitContainer.TabIndex = 3;
      // 
      // itemsGroupBox
      // 
      this.itemsGroupBox.Controls.Add(this.removeButton);
      this.itemsGroupBox.Controls.Add(this.addButton);
      this.itemsGroupBox.Controls.Add(this.itemsListView);
      this.itemsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.itemsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.itemsGroupBox.Name = "itemsGroupBox";
      this.itemsGroupBox.Size = new System.Drawing.Size(198, 246);
      this.itemsGroupBox.TabIndex = 0;
      this.itemsGroupBox.TabStop = false;
      this.itemsGroupBox.Text = "&Items:";
      // 
      // removeButton
      // 
      this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeButton.Location = new System.Drawing.Point(87, 217);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(75, 23);
      this.removeButton.TabIndex = 2;
      this.removeButton.Text = "&Remove";
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // addButton
      // 
      this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.addButton.Location = new System.Drawing.Point(6, 217);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(75, 23);
      this.addButton.TabIndex = 1;
      this.addButton.Text = "&Add...";
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
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
      this.itemsListView.Location = new System.Drawing.Point(6, 19);
      this.itemsListView.Name = "itemsListView";
      this.itemsListView.Size = new System.Drawing.Size(186, 192);
      this.itemsListView.TabIndex = 0;
      this.itemsListView.UseCompatibleStateImageBehavior = false;
      this.itemsListView.View = System.Windows.Forms.View.Details;
      this.itemsListView.SelectedIndexChanged += new System.EventHandler(this.elementsListView_SelectedIndexChanged);
      this.itemsListView.SizeChanged += new System.EventHandler(this.elementsListView_SizeChanged);
      this.itemsListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.elementsListView_DragDrop);
      this.itemsListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.elementsListView_DragEnter);
      this.itemsListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.elementsListView_KeyDown);
      this.itemsListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.elementsListView_ItemDrag);
      this.itemsListView.DragOver += new System.Windows.Forms.DragEventHandler(this.elementsListView_DragOver);
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailsGroupBox.Enabled = false;
      this.detailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(221, 246);
      this.detailsGroupBox.TabIndex = 0;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "&Details:";
      // 
      // itemTypeLabel
      // 
      this.itemTypeLabel.AutoSize = true;
      this.itemTypeLabel.Location = new System.Drawing.Point(3, 3);
      this.itemTypeLabel.Name = "itemTypeLabel";
      this.itemTypeLabel.Size = new System.Drawing.Size(57, 13);
      this.itemTypeLabel.TabIndex = 0;
      this.itemTypeLabel.Text = "&Item Type:";
      // 
      // setTypeButton
      // 
      this.setTypeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.setTypeButton.Location = new System.Drawing.Point(380, 0);
      this.setTypeButton.Name = "setTypeButton";
      this.setTypeButton.Size = new System.Drawing.Size(43, 20);
      this.setTypeButton.TabIndex = 1;
      this.setTypeButton.Text = "&Set...";
      this.setTypeButton.UseVisualStyleBackColor = true;
      this.setTypeButton.Click += new System.EventHandler(this.setTypeButton_Click);
      // 
      // typeTextBox
      // 
      this.typeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.typeTextBox.Location = new System.Drawing.Point(66, 0);
      this.typeTextBox.Name = "typeTextBox";
      this.typeTextBox.ReadOnly = true;
      this.typeTextBox.Size = new System.Drawing.Size(308, 20);
      this.typeTextBox.TabIndex = 2;
      // 
      // ItemListView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.typeTextBox);
      this.Controls.Add(this.setTypeButton);
      this.Controls.Add(this.itemTypeLabel);
      this.Controls.Add(this.splitContainer);
      this.Name = "ItemListView";
      this.Size = new System.Drawing.Size(423, 273);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.GroupBox itemsGroupBox;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private System.Windows.Forms.Button removeButton;
    private System.Windows.Forms.Button addButton;
    private System.Windows.Forms.ListView itemsListView;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private Label itemTypeLabel;
    private Button setTypeButton;
    private TextBox typeTextBox;

  }
}
