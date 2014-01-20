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

namespace HeuristicLab.HLScript.Views {
  partial class VariableStoreView {
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
      this.variableListView = new System.Windows.Forms.ListView();
      this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.valueColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.typeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.sortDescendingButton = new System.Windows.Forms.Button();
      this.sortAscendingButton = new System.Windows.Forms.Button();
      this.removeButton = new System.Windows.Forms.Button();
      this.addButton = new System.Windows.Forms.Button();
      this.variablesGroupBox = new System.Windows.Forms.GroupBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.variablesGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // variableListView
      // 
      this.variableListView.AllowDrop = true;
      this.variableListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.variableListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.valueColumnHeader,
            this.typeColumnHeader});
      this.variableListView.FullRowSelect = true;
      this.variableListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.variableListView.HideSelection = false;
      this.variableListView.LabelEdit = true;
      this.variableListView.Location = new System.Drawing.Point(6, 49);
      this.variableListView.MultiSelect = false;
      this.variableListView.Name = "variableListView";
      this.variableListView.ShowItemToolTips = true;
      this.variableListView.Size = new System.Drawing.Size(224, 395);
      this.variableListView.SmallImageList = this.imageList;
      this.variableListView.TabIndex = 5;
      this.variableListView.UseCompatibleStateImageBehavior = false;
      this.variableListView.View = System.Windows.Forms.View.Details;
      this.variableListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.variableListView_AfterLabelEdit);
      this.variableListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.variableListView_ItemDrag);
      this.variableListView.SelectedIndexChanged += new System.EventHandler(this.variableListView_SelectedIndexChanged);
      this.variableListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.variableListView_DragDrop);
      this.variableListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.variableListView_DragEnter);
      this.variableListView.DragOver += new System.Windows.Forms.DragEventHandler(this.variableListView_DragOver);
      this.variableListView.DoubleClick += new System.EventHandler(this.variableListView_DoubleClick);
      this.variableListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.variableListView_KeyDown);
      // 
      // nameColumnHeader
      // 
      this.nameColumnHeader.Text = "Name";
      // 
      // valueColumnHeader
      // 
      this.valueColumnHeader.Text = "Value";
      // 
      // typeColumnHeader
      // 
      this.typeColumnHeader.Text = "Type";
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
      this.sortDescendingButton.Location = new System.Drawing.Point(36, 19);
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
      this.sortAscendingButton.Location = new System.Drawing.Point(66, 19);
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
      this.removeButton.Location = new System.Drawing.Point(96, 19);
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
      this.addButton.Location = new System.Drawing.Point(6, 19);
      this.addButton.Name = "addButton";
      this.addButton.Size = new System.Drawing.Size(24, 24);
      this.addButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.addButton, "Add");
      this.addButton.UseVisualStyleBackColor = true;
      this.addButton.Click += new System.EventHandler(this.addButton_Click);
      // 
      // variablesGroupBox
      // 
      this.variablesGroupBox.Controls.Add(this.variableListView);
      this.variablesGroupBox.Controls.Add(this.addButton);
      this.variablesGroupBox.Controls.Add(this.sortDescendingButton);
      this.variablesGroupBox.Controls.Add(this.removeButton);
      this.variablesGroupBox.Controls.Add(this.sortAscendingButton);
      this.variablesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.variablesGroupBox.Location = new System.Drawing.Point(0, 0);
      this.variablesGroupBox.Name = "variablesGroupBox";
      this.variablesGroupBox.Size = new System.Drawing.Size(236, 450);
      this.variablesGroupBox.TabIndex = 0;
      this.variablesGroupBox.TabStop = false;
      this.variablesGroupBox.Text = "Variables";
      // 
      // VariableStoreView
      // 
      this.Controls.Add(this.variablesGroupBox);
      this.Name = "VariableStoreView";
      this.Size = new System.Drawing.Size(236, 450);
      this.variablesGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.ColumnHeader nameColumnHeader;
    protected GroupBox variablesGroupBox;
    protected ListView variableListView;
    protected Button addButton;
    protected Button removeButton;
    protected ToolTip toolTip;
    protected ImageList imageList;
    protected Button sortAscendingButton;
    protected Button sortDescendingButton;
    private ColumnHeader valueColumnHeader;
    private ColumnHeader typeColumnHeader;
  }
}
