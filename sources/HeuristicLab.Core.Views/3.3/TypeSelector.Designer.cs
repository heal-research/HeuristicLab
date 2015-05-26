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

namespace HeuristicLab.Core.Views {
  partial class TypeSelector {
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
      this.typesTreeView = new System.Windows.Forms.TreeView();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.typesGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.typeParametersSplitContainer = new System.Windows.Forms.SplitContainer();
      this.searchLabel = new System.Windows.Forms.Label();
      this.searchTextBox = new System.Windows.Forms.TextBox();
      this.clearSearchButton = new System.Windows.Forms.Button();
      this.typeParametersGroupBox = new System.Windows.Forms.GroupBox();
      this.setTypeParameterButton = new System.Windows.Forms.Button();
      this.typeParametersListView = new System.Windows.Forms.ListView();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.typesGroupBox.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.typeParametersSplitContainer.Panel1.SuspendLayout();
      this.typeParametersSplitContainer.Panel2.SuspendLayout();
      this.typeParametersSplitContainer.SuspendLayout();
      this.typeParametersGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // typesTreeView
      // 
      this.typesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.typesTreeView.HideSelection = false;
      this.typesTreeView.ImageIndex = 0;
      this.typesTreeView.ImageList = this.imageList;
      this.typesTreeView.Location = new System.Drawing.Point(3, 29);
      this.typesTreeView.Name = "typesTreeView";
      this.typesTreeView.SelectedImageIndex = 0;
      this.typesTreeView.ShowNodeToolTips = true;
      this.typesTreeView.Size = new System.Drawing.Size(219, 287);
      this.typesTreeView.TabIndex = 2;
      this.typesTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.typesTreeView_ItemDrag);
      this.typesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.typesTreeView_AfterSelect);
      this.typesTreeView.VisibleChanged += new System.EventHandler(this.typesTreeView_VisibleChanged);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // typesGroupBox
      // 
      this.typesGroupBox.Controls.Add(this.splitContainer);
      this.typesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.typesGroupBox.Location = new System.Drawing.Point(0, 0);
      this.typesGroupBox.Name = "typesGroupBox";
      this.typesGroupBox.Size = new System.Drawing.Size(231, 614);
      this.typesGroupBox.TabIndex = 0;
      this.typesGroupBox.TabStop = false;
      this.typesGroupBox.Text = "Available Types";
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
      this.splitContainer.Panel1.Controls.Add(this.typeParametersSplitContainer);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.descriptionTextBox);
      this.splitContainer.Size = new System.Drawing.Size(225, 595);
      this.splitContainer.SplitterDistance = 471;
      this.splitContainer.TabIndex = 0;
      // 
      // typeParametersSplitContainer
      // 
      this.typeParametersSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.typeParametersSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.typeParametersSplitContainer.Name = "typeParametersSplitContainer";
      this.typeParametersSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // typeParametersSplitContainer.Panel1
      // 
      this.typeParametersSplitContainer.Panel1.Controls.Add(this.typesTreeView);
      this.typeParametersSplitContainer.Panel1.Controls.Add(this.searchLabel);
      this.typeParametersSplitContainer.Panel1.Controls.Add(this.searchTextBox);
      // 
      // typeParametersSplitContainer.Panel2
      // 
      this.typeParametersSplitContainer.Panel2.Controls.Add(this.typeParametersGroupBox);
      this.typeParametersSplitContainer.Size = new System.Drawing.Size(225, 472);
      this.typeParametersSplitContainer.SplitterDistance = 319;
      this.typeParametersSplitContainer.TabIndex = 0;
      // 
      // searchLabel
      // 
      this.searchLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Zoom;
      this.searchLabel.Location = new System.Drawing.Point(3, 3);
      this.searchLabel.Name = "searchLabel";
      this.searchLabel.Size = new System.Drawing.Size(20, 20);
      this.searchLabel.TabIndex = 0;
      this.toolTip.SetToolTip(this.searchLabel, "Enter string to search for types");
      // 
      // searchTextBox
      // 
      this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.searchTextBox.Controls.Add(this.clearSearchButton);
      this.searchTextBox.Location = new System.Drawing.Point(29, 3);
      this.searchTextBox.Name = "searchTextBox";
      this.searchTextBox.Size = new System.Drawing.Size(193, 20);
      this.searchTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.searchTextBox, "Filters the available Types.\r\nThe search term is tokenized by space and a name ha" +
        "s to contain all tokens to be displayed.\r\n(E.g. \"Sym Reg\" matches \"SymbolicRegr" +
        "ession\")");
      this.searchTextBox.TextChanged += new System.EventHandler(this.searchTextBox_TextChanged);
      this.searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchTextBox_KeyDown);
      // 
      // clearSearchButton
      // 
      this.clearSearchButton.BackColor = System.Drawing.Color.Transparent;
      this.clearSearchButton.Cursor = System.Windows.Forms.Cursors.Default;
      this.clearSearchButton.Dock = System.Windows.Forms.DockStyle.Right;
      this.clearSearchButton.FlatAppearance.BorderSize = 0;
      this.clearSearchButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.clearSearchButton.ForeColor = System.Drawing.Color.Transparent;
      this.clearSearchButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Delete;
      this.clearSearchButton.Location = new System.Drawing.Point(543, 0);
      this.clearSearchButton.Margin = new System.Windows.Forms.Padding(0);
      this.clearSearchButton.Name = "clearSearchButton";
      this.clearSearchButton.Size = new System.Drawing.Size(15, 16);
      this.clearSearchButton.TabIndex = 0;
      this.clearSearchButton.TabStop = false;
      this.clearSearchButton.UseVisualStyleBackColor = false;
      this.clearSearchButton.Click += new System.EventHandler(this.clearSearchButton_Click);
      // 
      // typeParametersGroupBox
      // 
      this.typeParametersGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.typeParametersGroupBox.Controls.Add(this.setTypeParameterButton);
      this.typeParametersGroupBox.Controls.Add(this.typeParametersListView);
      this.typeParametersGroupBox.Location = new System.Drawing.Point(3, 3);
      this.typeParametersGroupBox.Name = "typeParametersGroupBox";
      this.typeParametersGroupBox.Size = new System.Drawing.Size(219, 143);
      this.typeParametersGroupBox.TabIndex = 0;
      this.typeParametersGroupBox.TabStop = false;
      this.typeParametersGroupBox.Text = "Type Parameters";
      // 
      // setTypeParameterButton
      // 
      this.setTypeParameterButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.setTypeParameterButton.Enabled = false;
      this.setTypeParameterButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Edit;
      this.setTypeParameterButton.Location = new System.Drawing.Point(189, 19);
      this.setTypeParameterButton.Name = "setTypeParameterButton";
      this.setTypeParameterButton.Size = new System.Drawing.Size(24, 24);
      this.setTypeParameterButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.setTypeParameterButton, "Set Type Parameter");
      this.setTypeParameterButton.UseVisualStyleBackColor = true;
      this.setTypeParameterButton.Click += new System.EventHandler(this.setTypeParameterButton_Click);
      // 
      // typeParametersListView
      // 
      this.typeParametersListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.typeParametersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
      this.typeParametersListView.FullRowSelect = true;
      this.typeParametersListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.typeParametersListView.Location = new System.Drawing.Point(6, 19);
      this.typeParametersListView.MultiSelect = false;
      this.typeParametersListView.Name = "typeParametersListView";
      this.typeParametersListView.ShowItemToolTips = true;
      this.typeParametersListView.Size = new System.Drawing.Size(177, 118);
      this.typeParametersListView.TabIndex = 0;
      this.typeParametersListView.UseCompatibleStateImageBehavior = false;
      this.typeParametersListView.View = System.Windows.Forms.View.Details;
      this.typeParametersListView.SelectedIndexChanged += new System.EventHandler(this.typeParametersListView_SelectedIndexChanged);
      this.typeParametersListView.DoubleClick += new System.EventHandler(this.typeParametersListView_DoubleClick);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.descriptionTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.descriptionTextBox.Location = new System.Drawing.Point(3, 3);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.ReadOnly = true;
      this.descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.descriptionTextBox.Size = new System.Drawing.Size(219, 114);
      this.descriptionTextBox.TabIndex = 0;
      // 
      // TypeSelector
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.typesGroupBox);
      this.Name = "TypeSelector";
      this.Size = new System.Drawing.Size(231, 614);
      this.typesGroupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      this.splitContainer.ResumeLayout(false);
      this.typeParametersSplitContainer.Panel1.ResumeLayout(false);
      this.typeParametersSplitContainer.Panel1.PerformLayout();
      this.typeParametersSplitContainer.Panel2.ResumeLayout(false);
      this.typeParametersSplitContainer.ResumeLayout(false);
      this.typeParametersGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.GroupBox typesGroupBox;
    protected System.Windows.Forms.TextBox descriptionTextBox;
    protected System.Windows.Forms.ImageList imageList;
    protected System.Windows.Forms.TreeView typesTreeView;
    protected System.Windows.Forms.SplitContainer splitContainer;
    protected System.Windows.Forms.Label searchLabel;
    protected System.Windows.Forms.TextBox searchTextBox;
    protected System.Windows.Forms.Button clearSearchButton;
    protected System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.SplitContainer typeParametersSplitContainer;
    protected System.Windows.Forms.GroupBox typeParametersGroupBox;
    protected System.Windows.Forms.ListView typeParametersListView;
    protected System.Windows.Forms.Button setTypeParameterButton;
    protected System.Windows.Forms.ColumnHeader columnHeader1;

  }
}
