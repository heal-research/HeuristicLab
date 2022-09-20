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

namespace HeuristicLab.PluginInfrastructure.Advanced {
  partial class EditProductsView {
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
      this.refreshButton = new System.Windows.Forms.Button();
      this.uploadButton = new System.Windows.Forms.Button();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.productsGroupBox = new System.Windows.Forms.GroupBox();
      this.deleteProductButton = new System.Windows.Forms.Button();
      this.newProductButton = new System.Windows.Forms.Button();
      this.productsListView = new System.Windows.Forms.ListView();
      this.productNameHeader = new System.Windows.Forms.ColumnHeader();
      this.productVersionHeader = new System.Windows.Forms.ColumnHeader();
      this.productImageList = new System.Windows.Forms.ImageList(this.components);
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.pluginsGroupBox = new System.Windows.Forms.GroupBox();
      this.pluginListView = new HeuristicLab.PluginInfrastructure.Advanced.MultiSelectListView();
      this.pluginNameHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginVersionHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginDescriptionHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginImageList = new System.Windows.Forms.ImageList(this.components);
      this.versionTextBox = new System.Windows.Forms.TextBox();
      this.nameLabel = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.versionLabel = new System.Windows.Forms.Label();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.productsGroupBox.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.pluginsGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // refreshButton
      // 
      this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.refreshButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.Internet;
      this.refreshButton.Location = new System.Drawing.Point(6, 394);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(73, 26);
      this.refreshButton.TabIndex = 1;
      this.refreshButton.Text = "Refresh";
      this.refreshButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh list of plugins and products from server");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // uploadButton
      // 
      this.uploadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.uploadButton.Enabled = false;
      this.uploadButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.PublishToWeb;
      this.uploadButton.Location = new System.Drawing.Point(85, 394);
      this.uploadButton.Name = "uploadButton";
      this.uploadButton.Size = new System.Drawing.Size(120, 26);
      this.uploadButton.TabIndex = 2;
      this.uploadButton.Text = "Upload Products";
      this.uploadButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.uploadButton, "Upload changed products to server");
      this.uploadButton.UseVisualStyleBackColor = true;
      this.uploadButton.Click += new System.EventHandler(this.saveButton_Click);
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.productsGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(665, 426);
      this.splitContainer.SplitterDistance = 321;
      this.splitContainer.TabIndex = 4;
      // 
      // productsGroupBox
      // 
      this.productsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.productsGroupBox.Controls.Add(this.deleteProductButton);
      this.productsGroupBox.Controls.Add(this.uploadButton);
      this.productsGroupBox.Controls.Add(this.newProductButton);
      this.productsGroupBox.Controls.Add(this.productsListView);
      this.productsGroupBox.Controls.Add(this.refreshButton);
      this.productsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.productsGroupBox.Name = "productsGroupBox";
      this.productsGroupBox.Size = new System.Drawing.Size(321, 426);
      this.productsGroupBox.TabIndex = 5;
      this.productsGroupBox.TabStop = false;
      this.productsGroupBox.Text = "Products";
      // 
      // deleteProductButton
      // 
      this.deleteProductButton.Enabled = false;
      this.deleteProductButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.Remove;
      this.deleteProductButton.Location = new System.Drawing.Point(116, 19);
      this.deleteProductButton.Name = "deleteProductButton";
      this.deleteProductButton.Size = new System.Drawing.Size(104, 26);
      this.deleteProductButton.TabIndex = 5;
      this.deleteProductButton.Text = "Delete Product";
      this.deleteProductButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.deleteProductButton, "Deletes the selected product");
      this.deleteProductButton.UseVisualStyleBackColor = true;
      this.deleteProductButton.Click += new System.EventHandler(this.deleteProductButton_Click);
      // 
      // newProductButton
      // 
      this.newProductButton.Enabled = false;
      this.newProductButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.Add;
      this.newProductButton.Location = new System.Drawing.Point(6, 19);
      this.newProductButton.Name = "newProductButton";
      this.newProductButton.Size = new System.Drawing.Size(104, 26);
      this.newProductButton.TabIndex = 3;
      this.newProductButton.Text = "Create Product";
      this.newProductButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.newProductButton, "Create a new product");
      this.newProductButton.UseVisualStyleBackColor = true;
      this.newProductButton.Click += new System.EventHandler(this.newProductButton_Click);
      // 
      // productsListView
      // 
      this.productsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.productsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.productNameHeader,
            this.productVersionHeader});
      this.productsListView.Enabled = false;
      this.productsListView.FullRowSelect = true;
      this.productsListView.HideSelection = false;
      this.productsListView.Location = new System.Drawing.Point(6, 51);
      this.productsListView.MultiSelect = false;
      this.productsListView.Name = "productsListView";
      this.productsListView.Size = new System.Drawing.Size(309, 337);
      this.productsListView.SmallImageList = this.productImageList;
      this.productsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.productsListView.TabIndex = 4;
      this.productsListView.UseCompatibleStateImageBehavior = false;
      this.productsListView.View = System.Windows.Forms.View.Details;
      this.productsListView.SelectedIndexChanged += new System.EventHandler(this.productsListBox_SelectedIndexChanged);
      // 
      // productNameHeader
      // 
      this.productNameHeader.Text = "Name";
      this.productNameHeader.Width = 40;
      // 
      // productVersionHeader
      // 
      this.productVersionHeader.Text = "Version";
      this.productVersionHeader.Width = 265;
      // 
      // productImageList
      // 
      this.productImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.productImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.productImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsGroupBox.Controls.Add(this.pluginsGroupBox);
      this.detailsGroupBox.Controls.Add(this.versionTextBox);
      this.detailsGroupBox.Controls.Add(this.nameLabel);
      this.detailsGroupBox.Controls.Add(this.nameTextBox);
      this.detailsGroupBox.Controls.Add(this.versionLabel);
      this.detailsGroupBox.Enabled = false;
      this.detailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(340, 426);
      this.detailsGroupBox.TabIndex = 8;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // pluginsGroupBox
      // 
      this.pluginsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginsGroupBox.Controls.Add(this.pluginListView);
      this.pluginsGroupBox.Location = new System.Drawing.Point(6, 71);
      this.pluginsGroupBox.Name = "pluginsGroupBox";
      this.pluginsGroupBox.Size = new System.Drawing.Size(328, 349);
      this.pluginsGroupBox.TabIndex = 6;
      this.pluginsGroupBox.TabStop = false;
      this.pluginsGroupBox.Text = "Plugins";
      // 
      // pluginListView
      // 
      this.pluginListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginListView.CheckBoxes = true;
      this.pluginListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginNameHeader,
            this.pluginVersionHeader,
            this.pluginDescriptionHeader});
      this.pluginListView.Location = new System.Drawing.Point(3, 16);
      this.pluginListView.Name = "pluginListView";
      this.pluginListView.Size = new System.Drawing.Size(322, 330);
      this.pluginListView.SmallImageList = this.pluginImageList;
      this.pluginListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.pluginListView.SuppressItemCheckedEvents = false;
      this.pluginListView.TabIndex = 7;
      this.pluginListView.UseCompatibleStateImageBehavior = false;
      this.pluginListView.View = System.Windows.Forms.View.Details;
      this.pluginListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_ItemChecked);
      // 
      // pluginNameHeader
      // 
      this.pluginNameHeader.Text = "Name";
      // 
      // pluginVersionHeader
      // 
      this.pluginVersionHeader.Text = "Version";
      // 
      // pluginDescriptionHeader
      // 
      this.pluginDescriptionHeader.Text = "Description";
      // 
      // pluginImageList
      // 
      this.pluginImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.pluginImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.pluginImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // versionTextBox
      // 
      this.versionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.versionTextBox.Location = new System.Drawing.Point(57, 45);
      this.versionTextBox.Name = "versionTextBox";
      this.versionTextBox.Size = new System.Drawing.Size(243, 20);
      this.versionTextBox.TabIndex = 5;
      this.versionTextBox.TextChanged += new System.EventHandler(this.versionTextBox_TextChanged);
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(13, 22);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 2;
      this.nameLabel.Text = "Name:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.nameTextBox.Location = new System.Drawing.Point(57, 19);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.Size = new System.Drawing.Size(243, 20);
      this.nameTextBox.TabIndex = 3;
      this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
      // 
      // versionLabel
      // 
      this.versionLabel.AutoSize = true;
      this.versionLabel.Location = new System.Drawing.Point(6, 48);
      this.versionLabel.Name = "versionLabel";
      this.versionLabel.Size = new System.Drawing.Size(45, 13);
      this.versionLabel.TabIndex = 4;
      this.versionLabel.Text = "Version:";
      // 
      // errorProvider
      // 
      this.errorProvider.ContainerControl = this;
      // 
      // ProductEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer);
      this.Name = "ProductEditor";
      this.Size = new System.Drawing.Size(665, 426);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.productsGroupBox.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.detailsGroupBox.PerformLayout();
      this.pluginsGroupBox.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button uploadButton;
    private System.Windows.Forms.Button newProductButton;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.TextBox versionTextBox;
    private System.Windows.Forms.Label versionLabel;
    private System.Windows.Forms.TextBox nameTextBox;
    private System.Windows.Forms.Label nameLabel;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.ListView productsListView;
    private System.Windows.Forms.ColumnHeader productNameHeader;
    private System.Windows.Forms.ColumnHeader productVersionHeader;
    private System.Windows.Forms.ImageList productImageList;
    private System.Windows.Forms.ImageList pluginImageList;
    private MultiSelectListView pluginListView;
    private System.Windows.Forms.GroupBox productsGroupBox;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private System.Windows.Forms.GroupBox pluginsGroupBox;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Button deleteProductButton;
    private System.Windows.Forms.ColumnHeader pluginNameHeader;
    private System.Windows.Forms.ColumnHeader pluginVersionHeader;
    private System.Windows.Forms.ColumnHeader pluginDescriptionHeader;

  }
}
