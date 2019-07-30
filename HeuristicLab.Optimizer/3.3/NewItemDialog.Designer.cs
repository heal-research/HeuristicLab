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

namespace HeuristicLab.Optimizer {
  partial class NewItemDialog {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewItemDialog));
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.searchLabel = new System.Windows.Forms.Label();
      this.searchTextBox = new System.Windows.Forms.TextBox();
      this.clearSearchButton = new System.Windows.Forms.Button();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.availableItemsGroupBox = new System.Windows.Forms.GroupBox();
      this.typesTreeView = new System.Windows.Forms.TreeView();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.descriptionSplitContainer = new System.Windows.Forms.SplitContainer();
      this.itemGroupBox = new System.Windows.Forms.GroupBox();
      this.itemDescriptionTextBox = new System.Windows.Forms.TextBox();
      this.pluginDescriptionGroupBox = new System.Windows.Forms.GroupBox();
      this.versionTextBox = new System.Windows.Forms.TextBox();
      this.pluginTextBox = new System.Windows.Forms.TextBox();
      this.pluginDescriptionTextBox = new System.Windows.Forms.TextBox();
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.expandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.collapseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.searchTextBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.availableItemsGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.descriptionSplitContainer)).BeginInit();
      this.descriptionSplitContainer.Panel1.SuspendLayout();
      this.descriptionSplitContainer.Panel2.SuspendLayout();
      this.descriptionSplitContainer.SuspendLayout();
      this.itemGroupBox.SuspendLayout();
      this.pluginDescriptionGroupBox.SuspendLayout();
      this.contextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Enabled = false;
      this.okButton.Location = new System.Drawing.Point(456, 587);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 1;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(537, 587);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 2;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // searchLabel
      // 
      this.searchLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Zoom;
      this.searchLabel.Location = new System.Drawing.Point(6, 17);
      this.searchLabel.Name = "searchLabel";
      this.searchLabel.Size = new System.Drawing.Size(20, 20);
      this.searchLabel.TabIndex = 0;
      this.toolTip.SetToolTip(this.searchLabel, "Enter string to search for items");
      // 
      // searchTextBox
      // 
      this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.searchTextBox.Controls.Add(this.clearSearchButton);
      this.searchTextBox.Location = new System.Drawing.Point(32, 17);
      this.searchTextBox.Name = "searchTextBox";
      this.searchTextBox.Size = new System.Drawing.Size(562, 20);
      this.searchTextBox.TabIndex = 1;
      this.toolTip.SetToolTip(this.searchTextBox, "Filters the available Items.\r\nThe search term is tokenized by space and a name ha" +
        "s to contain all tokens to be displayed.\r\n(E.g. \"Sym Reg\" matches \"Symbolic Regr" +
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
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
      this.splitContainer.Location = new System.Drawing.Point(12, 12);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.availableItemsGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.descriptionSplitContainer);
      this.splitContainer.Size = new System.Drawing.Size(600, 569);
      this.splitContainer.SplitterDistance = 373;
      this.splitContainer.TabIndex = 0;
      this.splitContainer.TabStop = false;
      // 
      // availableItemsGroupBox
      // 
      this.availableItemsGroupBox.Controls.Add(this.searchLabel);
      this.availableItemsGroupBox.Controls.Add(this.searchTextBox);
      this.availableItemsGroupBox.Controls.Add(this.typesTreeView);
      this.availableItemsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.availableItemsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.availableItemsGroupBox.Name = "availableItemsGroupBox";
      this.availableItemsGroupBox.Padding = new System.Windows.Forms.Padding(3, 15, 3, 3);
      this.availableItemsGroupBox.Size = new System.Drawing.Size(600, 373);
      this.availableItemsGroupBox.TabIndex = 5;
      this.availableItemsGroupBox.TabStop = false;
      this.availableItemsGroupBox.Text = "Available Items";
      // 
      // typesTreeView
      // 
      this.typesTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.typesTreeView.HideSelection = false;
      this.typesTreeView.ImageIndex = 0;
      this.typesTreeView.ImageList = this.imageList;
      this.typesTreeView.Location = new System.Drawing.Point(6, 41);
      this.typesTreeView.Name = "typesTreeView";
      this.typesTreeView.SelectedImageIndex = 0;
      this.typesTreeView.ShowNodeToolTips = true;
      this.typesTreeView.Size = new System.Drawing.Size(588, 326);
      this.typesTreeView.TabIndex = 2;
      this.typesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.itemsTreeView_AfterSelect);
      this.typesTreeView.VisibleChanged += new System.EventHandler(this.itemsTreeView_VisibleChanged);
      this.typesTreeView.DoubleClick += new System.EventHandler(this.itemTreeView_DoubleClick);
      this.typesTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.typesTreeView_MouseDown);
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // descriptionSplitContainer
      // 
      this.descriptionSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.descriptionSplitContainer.Location = new System.Drawing.Point(0, 0);
      this.descriptionSplitContainer.Name = "descriptionSplitContainer";
      this.descriptionSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // descriptionSplitContainer.Panel1
      // 
      this.descriptionSplitContainer.Panel1.Controls.Add(this.itemGroupBox);
      // 
      // descriptionSplitContainer.Panel2
      // 
      this.descriptionSplitContainer.Panel2.Controls.Add(this.pluginDescriptionGroupBox);
      this.descriptionSplitContainer.Size = new System.Drawing.Size(600, 192);
      this.descriptionSplitContainer.SplitterDistance = 81;
      this.descriptionSplitContainer.TabIndex = 1;
      this.descriptionSplitContainer.TabStop = false;
      // 
      // itemGroupBox
      // 
      this.itemGroupBox.Controls.Add(this.itemDescriptionTextBox);
      this.itemGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.itemGroupBox.Location = new System.Drawing.Point(0, 0);
      this.itemGroupBox.Name = "itemGroupBox";
      this.itemGroupBox.Size = new System.Drawing.Size(600, 81);
      this.itemGroupBox.TabIndex = 1;
      this.itemGroupBox.TabStop = false;
      this.itemGroupBox.Text = "Item";
      // 
      // itemDescriptionTextBox
      // 
      this.itemDescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.itemDescriptionTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.itemDescriptionTextBox.Location = new System.Drawing.Point(6, 19);
      this.itemDescriptionTextBox.Multiline = true;
      this.itemDescriptionTextBox.Name = "itemDescriptionTextBox";
      this.itemDescriptionTextBox.ReadOnly = true;
      this.itemDescriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.itemDescriptionTextBox.Size = new System.Drawing.Size(588, 56);
      this.itemDescriptionTextBox.TabIndex = 0;
      this.itemDescriptionTextBox.TabStop = false;
      // 
      // pluginDescriptionGroupBox
      // 
      this.pluginDescriptionGroupBox.Controls.Add(this.versionTextBox);
      this.pluginDescriptionGroupBox.Controls.Add(this.pluginTextBox);
      this.pluginDescriptionGroupBox.Controls.Add(this.pluginDescriptionTextBox);
      this.pluginDescriptionGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pluginDescriptionGroupBox.Location = new System.Drawing.Point(0, 0);
      this.pluginDescriptionGroupBox.Name = "pluginDescriptionGroupBox";
      this.pluginDescriptionGroupBox.Size = new System.Drawing.Size(600, 107);
      this.pluginDescriptionGroupBox.TabIndex = 1;
      this.pluginDescriptionGroupBox.TabStop = false;
      this.pluginDescriptionGroupBox.Text = "Plugin";
      // 
      // versionTextBox
      // 
      this.versionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.versionTextBox.Location = new System.Drawing.Point(444, 20);
      this.versionTextBox.Name = "versionTextBox";
      this.versionTextBox.ReadOnly = true;
      this.versionTextBox.Size = new System.Drawing.Size(150, 20);
      this.versionTextBox.TabIndex = 0;
      this.versionTextBox.TabStop = false;
      // 
      // pluginTextBox
      // 
      this.pluginTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginTextBox.Location = new System.Drawing.Point(6, 20);
      this.pluginTextBox.Name = "pluginTextBox";
      this.pluginTextBox.ReadOnly = true;
      this.pluginTextBox.Size = new System.Drawing.Size(432, 20);
      this.pluginTextBox.TabIndex = 0;
      this.pluginTextBox.TabStop = false;
      // 
      // pluginDescriptionTextBox
      // 
      this.pluginDescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginDescriptionTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.pluginDescriptionTextBox.Location = new System.Drawing.Point(6, 46);
      this.pluginDescriptionTextBox.Multiline = true;
      this.pluginDescriptionTextBox.Name = "pluginDescriptionTextBox";
      this.pluginDescriptionTextBox.ReadOnly = true;
      this.pluginDescriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.pluginDescriptionTextBox.Size = new System.Drawing.Size(588, 55);
      this.pluginDescriptionTextBox.TabIndex = 0;
      this.pluginDescriptionTextBox.TabStop = false;
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandToolStripMenuItem,
            this.expandAllToolStripMenuItem,
            this.collapseToolStripMenuItem,
            this.collapseAllToolStripMenuItem});
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(137, 92);
      // 
      // expandToolStripMenuItem
      // 
      this.expandToolStripMenuItem.Name = "expandToolStripMenuItem";
      this.expandToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
      this.expandToolStripMenuItem.Text = "Expand";
      this.expandToolStripMenuItem.Click += new System.EventHandler(this.expandToolStripMenuItem_Click);
      // 
      // expandAllToolStripMenuItem
      // 
      this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
      this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
      this.expandAllToolStripMenuItem.Text = "Expand All";
      this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
      // 
      // collapseToolStripMenuItem
      // 
      this.collapseToolStripMenuItem.Name = "collapseToolStripMenuItem";
      this.collapseToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
      this.collapseToolStripMenuItem.Text = "Collapse";
      this.collapseToolStripMenuItem.Click += new System.EventHandler(this.collapseToolStripMenuItem_Click);
      // 
      // collapseAllToolStripMenuItem
      // 
      this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
      this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
      this.collapseAllToolStripMenuItem.Text = "Collapse All";
      this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
      // 
      // NewItemDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(624, 622);
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "NewItemDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "New Item";
      this.TopMost = true;
      this.Load += new System.EventHandler(this.NewItemDialog_Load);
      this.Shown += new System.EventHandler(this.NewItemDialog_Shown);
      this.searchTextBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.availableItemsGroupBox.ResumeLayout(false);
      this.availableItemsGroupBox.PerformLayout();
      this.descriptionSplitContainer.Panel1.ResumeLayout(false);
      this.descriptionSplitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.descriptionSplitContainer)).EndInit();
      this.descriptionSplitContainer.ResumeLayout(false);
      this.itemGroupBox.ResumeLayout(false);
      this.itemGroupBox.PerformLayout();
      this.pluginDescriptionGroupBox.ResumeLayout(false);
      this.pluginDescriptionGroupBox.PerformLayout();
      this.contextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.TextBox pluginDescriptionTextBox;
    private System.Windows.Forms.Label searchLabel;
    private System.Windows.Forms.TextBox searchTextBox;
    private System.Windows.Forms.TreeView typesTreeView;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.GroupBox availableItemsGroupBox;
    private System.Windows.Forms.GroupBox pluginDescriptionGroupBox;
    private System.Windows.Forms.TextBox versionTextBox;
    private System.Windows.Forms.TextBox pluginTextBox;
    private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem expandToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem collapseToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
    private System.Windows.Forms.Button clearSearchButton;
    private System.Windows.Forms.GroupBox itemGroupBox;
    private System.Windows.Forms.TextBox itemDescriptionTextBox;
    private System.Windows.Forms.SplitContainer descriptionSplitContainer;
  }
}