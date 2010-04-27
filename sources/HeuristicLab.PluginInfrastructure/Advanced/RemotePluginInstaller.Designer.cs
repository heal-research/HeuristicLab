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
namespace HeuristicLab.PluginInfrastructure.Advanced {
  partial class RemotePluginInstallerView {
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
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Products", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("New Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("All Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      this.remotePluginsListView = new HeuristicLab.PluginInfrastructure.Advanced.MultiSelectListView();
      this.nameHeader = new System.Windows.Forms.ColumnHeader();
      this.versionHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptionHeader = new System.Windows.Forms.ColumnHeader();
      this.refreshButton = new System.Windows.Forms.Button();
      this.installButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // remotePluginsListView
      // 
      this.remotePluginsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.remotePluginsListView.CheckBoxes = true;
      this.remotePluginsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameHeader,
            this.versionHeader,
            this.descriptionHeader});
      listViewGroup1.Header = "Products";
      listViewGroup1.Name = "productsGroup";
      listViewGroup2.Header = "New Plugins";
      listViewGroup2.Name = "newPluginsGroup";
      listViewGroup3.Header = "All Plugins";
      listViewGroup3.Name = "allPluginsGroup";
      this.remotePluginsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
      this.remotePluginsListView.Location = new System.Drawing.Point(0, 0);
      this.remotePluginsListView.Name = "remotePluginsListView";
      this.remotePluginsListView.Size = new System.Drawing.Size(533, 527);
      this.remotePluginsListView.SuppressItemCheckedEvents = false;
      this.remotePluginsListView.TabIndex = 0;
      this.remotePluginsListView.UseCompatibleStateImageBehavior = false;
      this.remotePluginsListView.View = System.Windows.Forms.View.Details;
      this.remotePluginsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.remotePluginsListView_ItemChecked);
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
      // refreshButton
      // 
      this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.refreshButton.Image = global::HeuristicLab.PluginInfrastructure.Properties.Resources.VS2008ImageLibrary_Objects_Internet;
      this.refreshButton.Location = new System.Drawing.Point(0, 533);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(72, 25);
      this.refreshButton.TabIndex = 16;
      this.refreshButton.Text = "Refresh";
      this.refreshButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshRemoteButton_Click);
      // 
      // installButton
      // 
      this.installButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.installButton.Enabled = false;
      this.installButton.Image = global::HeuristicLab.PluginInfrastructure.Properties.Resources.VS2008ImageLibrary_Objects_Install;
      this.installButton.Location = new System.Drawing.Point(78, 533);
      this.installButton.Name = "installButton";
      this.installButton.Size = new System.Drawing.Size(140, 25);
      this.installButton.TabIndex = 17;
      this.installButton.Text = "Install Selected Items";
      this.installButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.installButton.UseVisualStyleBackColor = true;
      this.installButton.Click += new System.EventHandler(this.installButton_Click);
      // 
      // RemotePluginInstallerView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.refreshButton);
      this.Controls.Add(this.installButton);
      this.Controls.Add(this.remotePluginsListView);
      this.Name = "RemotePluginInstallerView";
      this.Size = new System.Drawing.Size(533, 558);
      this.ResumeLayout(false);

    }

    #endregion

    private MultiSelectListView remotePluginsListView;
    private System.Windows.Forms.ColumnHeader nameHeader;
    private System.Windows.Forms.ColumnHeader versionHeader;
    private System.Windows.Forms.ColumnHeader descriptionHeader;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button installButton;
  }
}
