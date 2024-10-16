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
      components = new System.ComponentModel.Container();
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Active Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Disabled Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      localPluginsListView = new MultiSelectListView();
      nameHeader = new System.Windows.Forms.ColumnHeader();
      versionHeader = new System.Windows.Forms.ColumnHeader();
      descriptionHeader = new System.Windows.Forms.ColumnHeader();
      pluginImageList = new System.Windows.Forms.ImageList(components);
      toolTip = new System.Windows.Forms.ToolTip(components);
      SuspendLayout();
      // 
      // localPluginsListView
      // 
      localPluginsListView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
      localPluginsListView.CheckBoxes = true;
      localPluginsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { nameHeader, versionHeader, descriptionHeader });
      listViewGroup1.Header = "Active Plugins";
      listViewGroup1.Name = "activePluginsGroup";
      listViewGroup2.Header = "Disabled Plugins";
      listViewGroup2.Name = "disabledPluginsGroup";
      localPluginsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] { listViewGroup1, listViewGroup2 });
      localPluginsListView.Location = new System.Drawing.Point(0, 0);
      localPluginsListView.Name = "localPluginsListView";
      localPluginsListView.Size = new System.Drawing.Size(539, 505);
      localPluginsListView.SmallImageList = pluginImageList;
      localPluginsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      localPluginsListView.SuppressItemCheckedEvents = false;
      localPluginsListView.TabIndex = 13;
      localPluginsListView.UseCompatibleStateImageBehavior = false;
      localPluginsListView.View = System.Windows.Forms.View.Details;
      localPluginsListView.ItemActivate += localPluginsListView_ItemActivate;
      localPluginsListView.ItemChecked += pluginsListView_ItemChecked;
      // 
      // nameHeader
      // 
      nameHeader.Text = "Name";
      nameHeader.Width = 199;
      // 
      // versionHeader
      // 
      versionHeader.Text = "Version";
      versionHeader.Width = 84;
      // 
      // descriptionHeader
      // 
      descriptionHeader.Text = "Description";
      descriptionHeader.Width = 245;
      // 
      // pluginImageList
      // 
      pluginImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      pluginImageList.ImageSize = new System.Drawing.Size(16, 16);
      pluginImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // InstalledPluginsView
      // 
      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      Controls.Add(localPluginsListView);
      Name = "InstalledPluginsView";
      Size = new System.Drawing.Size(539, 508);
      ResumeLayout(false);
    }

    #endregion
    private MultiSelectListView localPluginsListView;
    private System.Windows.Forms.ColumnHeader nameHeader;
    private System.Windows.Forms.ColumnHeader versionHeader;
    private System.Windows.Forms.ColumnHeader descriptionHeader;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.ImageList pluginImageList;
    // private LocalPluginManagerView localPluginManagerView;
  }
}
