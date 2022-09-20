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
  partial class AvailablePluginsView {
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
      this.pluginsListView = new HeuristicLab.PluginInfrastructure.Advanced.MultiSelectListView();
      this.nameHeader = new System.Windows.Forms.ColumnHeader();
      this.versionHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptionHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginsImageList = new System.Windows.Forms.ImageList(this.components);
      this.productsListView = new System.Windows.Forms.ListView();
      this.productNameHeader = new System.Windows.Forms.ColumnHeader();
      this.productVersionHeader = new System.Windows.Forms.ColumnHeader();
      this.productLargeImageList = new System.Windows.Forms.ImageList(this.components);
      this.productImageList = new System.Windows.Forms.ImageList(this.components);
      this.productsGroupBox = new System.Windows.Forms.GroupBox();
      this.showDetailsButton = new System.Windows.Forms.RadioButton();
      this.showLargeIconsButton = new System.Windows.Forms.RadioButton();
      this.installProductsButton = new System.Windows.Forms.Button();
      this.refreshButton = new System.Windows.Forms.Button();
      this.pluginsGroupBox = new System.Windows.Forms.GroupBox();
      this.installPluginsButton = new System.Windows.Forms.Button();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.productsGroupBox.SuspendLayout();
      this.pluginsGroupBox.SuspendLayout();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // pluginsListView
      // 
      this.pluginsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginsListView.CheckBoxes = true;
      this.pluginsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameHeader,
            this.versionHeader,
            this.descriptionHeader});
      this.pluginsListView.Location = new System.Drawing.Point(6, 19);
      this.pluginsListView.Name = "pluginsListView";
      this.pluginsListView.ShowGroups = false;
      this.pluginsListView.Size = new System.Drawing.Size(266, 502);
      this.pluginsListView.SmallImageList = this.pluginsImageList;
      this.pluginsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.pluginsListView.SuppressItemCheckedEvents = false;
      this.pluginsListView.TabIndex = 0;
      this.pluginsListView.UseCompatibleStateImageBehavior = false;
      this.pluginsListView.View = System.Windows.Forms.View.Details;
      this.pluginsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.remotePluginsListView_ItemChecked);
      // 
      // nameHeader
      // 
      this.nameHeader.Text = "Name";
      this.nameHeader.Width = 185;
      // 
      // versionHeader
      // 
      this.versionHeader.Text = "Version";
      this.versionHeader.Width = 93;
      // 
      // descriptionHeader
      // 
      this.descriptionHeader.Text = "Description";
      this.descriptionHeader.Width = 250;
      // 
      // pluginsImageList
      // 
      this.pluginsImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.pluginsImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.pluginsImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // productsListView
      // 
      this.productsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.productsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.productNameHeader,
            this.productVersionHeader});
      this.productsListView.HideSelection = false;
      this.productsListView.LargeImageList = this.productLargeImageList;
      this.productsListView.Location = new System.Drawing.Point(6, 50);
      this.productsListView.MultiSelect = false;
      this.productsListView.Name = "productsListView";
      this.productsListView.ShowGroups = false;
      this.productsListView.Size = new System.Drawing.Size(240, 471);
      this.productsListView.SmallImageList = this.productImageList;
      this.productsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.productsListView.TabIndex = 18;
      this.productsListView.UseCompatibleStateImageBehavior = false;
      this.productsListView.View = System.Windows.Forms.View.Details;
      this.productsListView.SelectedIndexChanged += new System.EventHandler(this.productsListView_SelectedIndexChanged);
      // 
      // productNameHeader
      // 
      this.productNameHeader.Text = "Name";
      // 
      // productVersionHeader
      // 
      this.productVersionHeader.Text = "Version";
      // 
      // productLargeImageList
      // 
      this.productLargeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.productLargeImageList.ImageSize = new System.Drawing.Size(32, 32);
      this.productLargeImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // productImageList
      // 
      this.productImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.productImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.productImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // productsGroupBox
      // 
      this.productsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.productsGroupBox.Controls.Add(this.showDetailsButton);
      this.productsGroupBox.Controls.Add(this.showLargeIconsButton);
      this.productsGroupBox.Controls.Add(this.installProductsButton);
      this.productsGroupBox.Controls.Add(this.refreshButton);
      this.productsGroupBox.Controls.Add(this.productsListView);
      this.productsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.productsGroupBox.Name = "productsGroupBox";
      this.productsGroupBox.Size = new System.Drawing.Size(252, 558);
      this.productsGroupBox.TabIndex = 19;
      this.productsGroupBox.TabStop = false;
      this.productsGroupBox.Text = "Products";
      // 
      // showDetailsButton
      // 
      this.showDetailsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.showDetailsButton.Appearance = System.Windows.Forms.Appearance.Button;
      this.showDetailsButton.Checked = true;
      this.showDetailsButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.ShowDetails;
      this.showDetailsButton.Location = new System.Drawing.Point(221, 19);
      this.showDetailsButton.Name = "showDetailsButton";
      this.showDetailsButton.Size = new System.Drawing.Size(25, 25);
      this.showDetailsButton.TabIndex = 22;
      this.showDetailsButton.TabStop = true;
      this.toolTip.SetToolTip(this.showDetailsButton, "Show Details");
      this.showDetailsButton.UseVisualStyleBackColor = true;
      this.showDetailsButton.CheckedChanged += new System.EventHandler(this.showDetailsButton_CheckedChanged);
      // 
      // showLargeIconsButton
      // 
      this.showLargeIconsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.showLargeIconsButton.Appearance = System.Windows.Forms.Appearance.Button;
      this.showLargeIconsButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.ShowIcons;
      this.showLargeIconsButton.Location = new System.Drawing.Point(190, 19);
      this.showLargeIconsButton.Name = "showLargeIconsButton";
      this.showLargeIconsButton.Size = new System.Drawing.Size(25, 25);
      this.showLargeIconsButton.TabIndex = 21;
      this.toolTip.SetToolTip(this.showLargeIconsButton, "Show Large Icons");
      this.showLargeIconsButton.UseVisualStyleBackColor = true;
      this.showLargeIconsButton.CheckedChanged += new System.EventHandler(this.showLargeIconsButton_CheckedChanged);
      // 
      // installProductsButton
      // 
      this.installProductsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.installProductsButton.Enabled = false;
      this.installProductsButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.Install;
      this.installProductsButton.Location = new System.Drawing.Point(6, 527);
      this.installProductsButton.Name = "installProductsButton";
      this.installProductsButton.Size = new System.Drawing.Size(146, 25);
      this.installProductsButton.TabIndex = 20;
      this.installProductsButton.Text = "Install Selected Product";
      this.installProductsButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.installProductsButton, "Install all plugins for the selected product");
      this.installProductsButton.UseVisualStyleBackColor = true;
      this.installProductsButton.Click += new System.EventHandler(this.installProductsButton_Click);
      // 
      // refreshButton
      // 
      this.refreshButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.Internet;
      this.refreshButton.Location = new System.Drawing.Point(6, 19);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(72, 25);
      this.refreshButton.TabIndex = 16;
      this.refreshButton.Text = "Refresh";
      this.refreshButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh available products from HeuristicLab deployment service");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshRemoteButton_Click);
      // 
      // pluginsGroupBox
      // 
      this.pluginsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginsGroupBox.Controls.Add(this.pluginsListView);
      this.pluginsGroupBox.Controls.Add(this.installPluginsButton);
      this.pluginsGroupBox.Location = new System.Drawing.Point(-1, 0);
      this.pluginsGroupBox.Name = "pluginsGroupBox";
      this.pluginsGroupBox.Size = new System.Drawing.Size(278, 558);
      this.pluginsGroupBox.TabIndex = 20;
      this.pluginsGroupBox.TabStop = false;
      this.pluginsGroupBox.Text = "Plugins";
      // 
      // installPluginsButton
      // 
      this.installPluginsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.installPluginsButton.Enabled = false;
      this.installPluginsButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.Install;
      this.installPluginsButton.Location = new System.Drawing.Point(6, 527);
      this.installPluginsButton.Name = "installPluginsButton";
      this.installPluginsButton.Size = new System.Drawing.Size(140, 25);
      this.installPluginsButton.TabIndex = 17;
      this.installPluginsButton.Text = "Install Selected Plugins";
      this.installPluginsButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.installPluginsButton, "Install only checked plugins");
      this.installPluginsButton.UseVisualStyleBackColor = true;
      this.installPluginsButton.Click += new System.EventHandler(this.installPluginsButton_Click);
      // 
      // splitContainer
      // 
      this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.productsGroupBox);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.pluginsGroupBox);
      this.splitContainer.Size = new System.Drawing.Size(533, 558);
      this.splitContainer.SplitterDistance = 252;
      this.splitContainer.TabIndex = 21;
      // 
      // RemotePluginInstallerView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer);
      this.Name = "RemotePluginInstallerView";
      this.Size = new System.Drawing.Size(533, 558);
      this.productsGroupBox.ResumeLayout(false);
      this.pluginsGroupBox.ResumeLayout(false);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private MultiSelectListView pluginsListView;
    private System.Windows.Forms.ColumnHeader nameHeader;
    private System.Windows.Forms.ColumnHeader versionHeader;
    private System.Windows.Forms.ColumnHeader descriptionHeader;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button installPluginsButton;
    private System.Windows.Forms.ListView productsListView;
    private System.Windows.Forms.GroupBox productsGroupBox;
    private System.Windows.Forms.GroupBox pluginsGroupBox;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.Button installProductsButton;
    private System.Windows.Forms.ColumnHeader productNameHeader;
    private System.Windows.Forms.ColumnHeader productVersionHeader;
    private System.Windows.Forms.ImageList productImageList;
    private System.Windows.Forms.ImageList pluginsImageList;
    private System.Windows.Forms.RadioButton showDetailsButton;
    private System.Windows.Forms.RadioButton showLargeIconsButton;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.ImageList productLargeImageList;
  }
}
