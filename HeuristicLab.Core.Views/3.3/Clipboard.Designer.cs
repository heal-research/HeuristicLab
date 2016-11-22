#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


namespace HeuristicLab.Core.Views {
  partial class Clipboard<T> {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.listView = new System.Windows.Forms.ListView();
      this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.infoPanel = new System.Windows.Forms.Panel();
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.infoLabel = new System.Windows.Forms.Label();
      this.sortDescendingButton = new System.Windows.Forms.Button();
      this.sortAscendingButton = new System.Windows.Forms.Button();
      this.removeButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.saveButton = new System.Windows.Forms.Button();
      this.infoPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // listView
      // 
      this.listView.AllowDrop = true;
      this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
      this.listView.FullRowSelect = true;
      this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.listView.HideSelection = false;
      this.listView.Location = new System.Drawing.Point(0, 30);
      this.listView.Name = "listView";
      this.listView.ShowItemToolTips = true;
      this.listView.Size = new System.Drawing.Size(179, 432);
      this.listView.SmallImageList = this.imageList;
      this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.listView.TabIndex = 0;
      this.listView.UseCompatibleStateImageBehavior = false;
      this.listView.View = System.Windows.Forms.View.Details;
      this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
      this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
      this.listView.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView_DragDrop);
      this.listView.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView_DragEnter);
      this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView_KeyDown);
      this.listView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listView_ItemDrag);
      this.listView.DragOver += new System.Windows.Forms.DragEventHandler(this.listView_DragOver);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // infoPanel
      // 
      this.infoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.infoPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.infoPanel.Controls.Add(this.progressBar);
      this.infoPanel.Controls.Add(this.infoLabel);
      this.infoPanel.Enabled = false;
      this.infoPanel.Location = new System.Drawing.Point(13, 202);
      this.infoPanel.Name = "infoPanel";
      this.infoPanel.Size = new System.Drawing.Size(154, 59);
      this.infoPanel.TabIndex = 6;
      // 
      // progressBar
      // 
      this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar.Location = new System.Drawing.Point(3, 30);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(146, 23);
      this.progressBar.Step = 1;
      this.progressBar.TabIndex = 1;
      // 
      // infoLabel
      // 
      this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.infoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.infoLabel.Location = new System.Drawing.Point(3, 4);
      this.infoLabel.Name = "infoLabel";
      this.infoLabel.Size = new System.Drawing.Size(146, 23);
      this.infoLabel.TabIndex = 0;
      this.infoLabel.Text = "Loading ...";
      this.infoLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
      // 
      // sortDescendingButton
      // 
      this.sortDescendingButton.Enabled = false;
      this.sortDescendingButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.SortUp;
      this.sortDescendingButton.Location = new System.Drawing.Point(30, 0);
      this.sortDescendingButton.Name = "sortDescendingButton";
      this.sortDescendingButton.Size = new System.Drawing.Size(24, 24);
      this.sortDescendingButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.sortDescendingButton, "Sort Descending");
      this.sortDescendingButton.UseVisualStyleBackColor = true;
      this.sortDescendingButton.Click += new System.EventHandler(this.sortDescendingButton_Click);
      // 
      // sortAscendingButton
      // 
      this.sortAscendingButton.Enabled = false;
      this.sortAscendingButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Sort;
      this.sortAscendingButton.Location = new System.Drawing.Point(60, 0);
      this.sortAscendingButton.Name = "sortAscendingButton";
      this.sortAscendingButton.Size = new System.Drawing.Size(24, 24);
      this.sortAscendingButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.sortAscendingButton, "Sort Ascending");
      this.sortAscendingButton.UseVisualStyleBackColor = true;
      this.sortAscendingButton.Click += new System.EventHandler(this.sortAscendingButton_Click);
      // 
      // removeButton
      // 
      this.removeButton.Enabled = false;
      this.removeButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      this.removeButton.Location = new System.Drawing.Point(90, 0);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(24, 24);
      this.removeButton.TabIndex = 4;
      this.toolTip.SetToolTip(this.removeButton, "Remove Item");
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // addButton
      // 
      this.addButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Add;
      this.addButton.Location = new System.Drawing.Point(0, 0);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(24, 24);
      this.addButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.addButton, "Add Item");
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
      // 
      // saveButton
      // 
      this.saveButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Save;
      this.saveButton.Location = new System.Drawing.Point(120, 0);
      this.saveButton.Name = "saveButton";
      this.saveButton.Size = new System.Drawing.Size(24, 24);
      this.saveButton.TabIndex = 5;
      this.toolTip.SetToolTip(this.saveButton, "Save All Items");
      this.saveButton.UseVisualStyleBackColor = true;
      this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
      // 
      // Clipboard
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.sortDescendingButton);
      this.Controls.Add(this.sortAscendingButton);
      this.Controls.Add(this.saveButton);
      this.Controls.Add(this.removeButton);
      this.Controls.Add(this.addButton);
      this.Controls.Add(this.infoPanel);
      this.Controls.Add(this.listView);
      this.Name = "Clipboard";
      this.Size = new System.Drawing.Size(179, 462);
      this.infoPanel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListView listView;
    private System.Windows.Forms.Panel infoPanel;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.Label infoLabel;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.Button sortDescendingButton;
    private System.Windows.Forms.Button sortAscendingButton;
    private System.Windows.Forms.Button removeButton;
    private System.Windows.Forms.Button addButton;
    private System.Windows.Forms.ColumnHeader columnHeader1;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Button saveButton;

  }
}
