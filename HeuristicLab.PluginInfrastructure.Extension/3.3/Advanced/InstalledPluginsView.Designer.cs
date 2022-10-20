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
  partial class InstalledPluginsView {
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
      System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Active Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Disabled Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      this.localPluginsListView = new HeuristicLab.PluginInfrastructure.Advanced.MultiSelectListView();
      this.nameHeader = new System.Windows.Forms.ColumnHeader();
      this.versionHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptionHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginImageList = new System.Windows.Forms.ImageList(this.components);
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.refreshButton = new System.Windows.Forms.Button();
      this.removeButton = new System.Windows.Forms.Button();
      this.updateSelectedButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // localPluginsListView
      // 
      this.localPluginsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.localPluginsListView.CheckBoxes = true;
      this.localPluginsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameHeader,
            this.versionHeader,
            this.descriptionHeader});
      listViewGroup3.Header = "Active Plugins";
      listViewGroup3.Name = "activePluginsGroup";
      listViewGroup4.Header = "Disabled Plugins";
      listViewGroup4.Name = "disabledPluginsGroup";
      this.localPluginsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup3,
            listViewGroup4});
      this.localPluginsListView.Location = new System.Drawing.Point(0, 0);
      this.localPluginsListView.Name = "localPluginsListView";
      this.localPluginsListView.Size = new System.Drawing.Size(539, 477);
      this.localPluginsListView.SmallImageList = this.pluginImageList;
      this.localPluginsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.localPluginsListView.SuppressItemCheckedEvents = false;
      this.localPluginsListView.TabIndex = 13;
      this.localPluginsListView.UseCompatibleStateImageBehavior = false;
      this.localPluginsListView.View = System.Windows.Forms.View.Details;
      this.localPluginsListView.ItemActivate += new System.EventHandler(this.localPluginsListView_ItemActivate);
      this.localPluginsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.pluginsListView_ItemChecked);
      // 
      // nameHeader
      // 
      this.nameHeader.Text = "Name";
      this.nameHeader.Width = 199;
      // 
      // versionHeader
      // 
      this.versionHeader.Text = "Version";
      this.versionHeader.Width = 84;
      // 
      // descriptionHeader
      // 
      this.descriptionHeader.Text = "Description";
      this.descriptionHeader.Width = 245;
      // 
      // pluginImageList
      // 
      this.pluginImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.pluginImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.pluginImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // refreshButton
      // 
      this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.refreshButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.Repeat;
      this.refreshButton.Location = new System.Drawing.Point(0, 483);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(96, 25);
      this.refreshButton.TabIndex = 14;
      this.refreshButton.Text = "Refresh List";
      this.refreshButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh List of Installed Plugins");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // removeButton
      // 
      this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.removeButton.Enabled = false;
      this.removeButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.Remove;
      this.removeButton.Location = new System.Drawing.Point(222, 483);
      this.removeButton.Name = "removeButton";
      this.removeButton.Size = new System.Drawing.Size(110, 25);
      this.removeButton.TabIndex = 12;
      this.removeButton.Text = "Delete Selected";
      this.removeButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.removeButton, "Delete all selected plugins from disk");
      this.removeButton.UseVisualStyleBackColor = true;
      this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
      // 
      // updateSelectedButton
      // 
      this.updateSelectedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.updateSelectedButton.Enabled = false;
      this.updateSelectedButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.Internet;
      this.updateSelectedButton.Location = new System.Drawing.Point(102, 483);
      this.updateSelectedButton.Name = "updateSelectedButton";
      this.updateSelectedButton.Size = new System.Drawing.Size(114, 25);
      this.updateSelectedButton.TabIndex = 6;
      this.updateSelectedButton.Text = "Update Selected";
      this.updateSelectedButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.updateSelectedButton, "Update all selected plugins");
      this.updateSelectedButton.UseVisualStyleBackColor = true;
      this.updateSelectedButton.Click += new System.EventHandler(this.updateSelectedButton_Click);
      // 
      // LocalPluginsView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.refreshButton);
      this.Controls.Add(this.localPluginsListView);
      this.Controls.Add(this.removeButton);
      this.Controls.Add(this.updateSelectedButton);
      this.Name = "LocalPluginsView";
      this.Size = new System.Drawing.Size(539, 508);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button updateSelectedButton;
    private System.Windows.Forms.Button removeButton;
    private MultiSelectListView localPluginsListView;
    private System.Windows.Forms.ColumnHeader nameHeader;
    private System.Windows.Forms.ColumnHeader versionHeader;
    private System.Windows.Forms.ColumnHeader descriptionHeader;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.ImageList pluginImageList;
    private System.Windows.Forms.Button refreshButton;
    // private LocalPluginManagerView localPluginManagerView;
  }
}
