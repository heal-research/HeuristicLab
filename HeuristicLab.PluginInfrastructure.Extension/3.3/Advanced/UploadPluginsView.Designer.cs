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
  partial class UploadPluginsView {
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
      this.uploadButton = new System.Windows.Forms.Button();
      this.refreshButton = new System.Windows.Forms.Button();
      this.listView = new HeuristicLab.PluginInfrastructure.Advanced.MultiSelectListView();
      this.pluginNameHeader = new System.Windows.Forms.ColumnHeader();
      this.localVersionHeader = new System.Windows.Forms.ColumnHeader();
      this.serverVersionHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptionHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginImageList = new System.Windows.Forms.ImageList(this.components);
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.SuspendLayout();
      // 
      // uploadButton
      // 
      this.uploadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.uploadButton.Enabled = false;
      this.uploadButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.PublishToWeb;
      this.uploadButton.Location = new System.Drawing.Point(78, 482);
      this.uploadButton.Name = "uploadButton";
      this.uploadButton.Size = new System.Drawing.Size(114, 26);
      this.uploadButton.TabIndex = 7;
      this.uploadButton.Text = "Upload Selected";
      this.uploadButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.uploadButton, "Upload selected plugins to the server");
      this.uploadButton.UseVisualStyleBackColor = true;
      this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
      // 
      // refreshButton
      // 
      this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.refreshButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.Internet;
      this.refreshButton.Location = new System.Drawing.Point(0, 482);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(72, 26);
      this.refreshButton.TabIndex = 6;
      this.refreshButton.Text = "Refresh";
      this.refreshButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.refreshButton, "Update list of plugins from the server");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // listView
      // 
      this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.listView.CheckBoxes = true;
      this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginNameHeader,
            this.localVersionHeader,
            this.serverVersionHeader,
            this.descriptionHeader});
      this.listView.Location = new System.Drawing.Point(0, 0);
      this.listView.Name = "listView";
      this.listView.Size = new System.Drawing.Size(539, 476);
      this.listView.SmallImageList = this.pluginImageList;
      this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.listView.SuppressItemCheckedEvents = false;
      this.listView.TabIndex = 8;
      this.listView.UseCompatibleStateImageBehavior = false;
      this.listView.View = System.Windows.Forms.View.Details;
      this.listView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_ItemChecked);
      // 
      // pluginNameHeader
      // 
      this.pluginNameHeader.Text = "Name";
      this.pluginNameHeader.Width = 40;
      // 
      // localVersionHeader
      // 
      this.localVersionHeader.Text = "Local Version";
      this.localVersionHeader.Width = 76;
      // 
      // serverVersionHeader
      // 
      this.serverVersionHeader.Text = "Server Version";
      this.serverVersionHeader.Width = 81;
      // 
      // descriptionHeader
      // 
      this.descriptionHeader.Text = "Description";
      this.descriptionHeader.Width = 335;
      // 
      // pluginImageList
      // 
      this.pluginImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.pluginImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.pluginImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // PluginEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.listView);
      this.Controls.Add(this.uploadButton);
      this.Controls.Add(this.refreshButton);
      this.Name = "PluginEditor";
      this.Size = new System.Drawing.Size(539, 508);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button uploadButton;
    private MultiSelectListView listView;
    private System.Windows.Forms.ColumnHeader pluginNameHeader;
    private System.Windows.Forms.ColumnHeader localVersionHeader;
    private System.Windows.Forms.ColumnHeader serverVersionHeader;
    private System.Windows.Forms.ColumnHeader descriptionHeader;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.ImageList pluginImageList;
  }
}
