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
  partial class RemotePluginInstaller {
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
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Products", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("New Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("All Plugins", System.Windows.Forms.HorizontalAlignment.Left);
      this.imageListForRemoteItems = new System.Windows.Forms.ImageList(this.components);
      this.remotePluginsListView = new HeuristicLab.PluginInfrastructure.Advanced.MultiSelectListView();
      this.nameHeader = new System.Windows.Forms.ColumnHeader();
      this.versionHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptionHeader = new System.Windows.Forms.ColumnHeader();
      this.radioButton1 = new System.Windows.Forms.RadioButton();
      this.advancedViewButton = new System.Windows.Forms.RadioButton();
      this.simpleViewButton = new System.Windows.Forms.RadioButton();
      this.viewButtonGroupBox = new System.Windows.Forms.GroupBox();
      this.viewButtonGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // imageListForRemoteItems
      // 
      this.imageListForRemoteItems.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
      this.imageListForRemoteItems.ImageSize = new System.Drawing.Size(13, 13);
      this.imageListForRemoteItems.TransparentColor = System.Drawing.Color.Transparent;
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
      this.remotePluginsListView.Location = new System.Drawing.Point(0, 50);
      this.remotePluginsListView.Name = "remotePluginsListView";
      this.remotePluginsListView.Size = new System.Drawing.Size(533, 508);
      this.remotePluginsListView.StateImageList = this.imageListForRemoteItems;
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
      // radioButton1
      // 
      this.radioButton1.AutoSize = true;
      this.radioButton1.Location = new System.Drawing.Point(-15, -15);
      this.radioButton1.Name = "radioButton1";
      this.radioButton1.Size = new System.Drawing.Size(85, 17);
      this.radioButton1.TabIndex = 2;
      this.radioButton1.TabStop = true;
      this.radioButton1.Text = "radioButton1";
      this.radioButton1.UseVisualStyleBackColor = true;
      // 
      // advancedViewButton
      // 
      this.advancedViewButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)));
      this.advancedViewButton.Appearance = System.Windows.Forms.Appearance.Button;
      this.advancedViewButton.AutoSize = true;
      this.advancedViewButton.Location = new System.Drawing.Point(104, 9);
      this.advancedViewButton.Name = "advancedViewButton";
      this.advancedViewButton.Size = new System.Drawing.Size(92, 23);
      this.advancedViewButton.TabIndex = 3;
      this.advancedViewButton.Text = "Advanced View";
      this.advancedViewButton.UseVisualStyleBackColor = true;
      this.advancedViewButton.CheckedChanged += new System.EventHandler(this.advancedViewButton_CheckedChanged);
      // 
      // simpleViewButton
      // 
      this.simpleViewButton.Appearance = System.Windows.Forms.Appearance.Button;
      this.simpleViewButton.Checked = true;
      this.simpleViewButton.Location = new System.Drawing.Point(6, 9);
      this.simpleViewButton.Name = "simpleViewButton";
      this.simpleViewButton.Size = new System.Drawing.Size(92, 23);
      this.simpleViewButton.TabIndex = 3;
      this.simpleViewButton.TabStop = true;
      this.simpleViewButton.Text = "Simple View";
      this.simpleViewButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.simpleViewButton.UseVisualStyleBackColor = true;
      this.simpleViewButton.CheckedChanged += new System.EventHandler(this.advancedViewButton_CheckedChanged);
      // 
      // viewButtonGroupBox
      // 
      this.viewButtonGroupBox.Controls.Add(this.advancedViewButton);
      this.viewButtonGroupBox.Controls.Add(this.simpleViewButton);
      this.viewButtonGroupBox.Location = new System.Drawing.Point(3, 8);
      this.viewButtonGroupBox.Name = "viewButtonGroupBox";
      this.viewButtonGroupBox.Size = new System.Drawing.Size(204, 36);
      this.viewButtonGroupBox.TabIndex = 5;
      this.viewButtonGroupBox.TabStop = false;
      // 
      // RemotePluginInstaller
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.viewButtonGroupBox);
      this.Controls.Add(this.radioButton1);
      this.Controls.Add(this.remotePluginsListView);
      this.Name = "RemotePluginInstaller";
      this.Size = new System.Drawing.Size(533, 558);
      this.viewButtonGroupBox.ResumeLayout(false);
      this.viewButtonGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ImageList imageListForRemoteItems;
    private MultiSelectListView remotePluginsListView;
    private System.Windows.Forms.ColumnHeader nameHeader;
    private System.Windows.Forms.ColumnHeader versionHeader;
    private System.Windows.Forms.ColumnHeader descriptionHeader;
    private System.Windows.Forms.RadioButton radioButton1;
    private System.Windows.Forms.RadioButton advancedViewButton;
    private System.Windows.Forms.RadioButton simpleViewButton;
    private System.Windows.Forms.GroupBox viewButtonGroupBox;
  }
}
