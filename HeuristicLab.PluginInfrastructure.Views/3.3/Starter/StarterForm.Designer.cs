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

namespace HeuristicLab.PluginInfrastructure.Starter {
  partial class StarterForm {
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
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Applications", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Plugin Management", System.Windows.Forms.HorizontalAlignment.Left);
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StarterForm));
      this.startButton = new System.Windows.Forms.Button();
      this.largeImageList = new System.Windows.Forms.ImageList(this.components);
      this.applicationsListView = new System.Windows.Forms.ListView();
      this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.versionColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.smallImageList = new System.Windows.Forms.ImageList(this.components);
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.showLargeIconsButton = new System.Windows.Forms.RadioButton();
      this.showDetailsButton = new System.Windows.Forms.RadioButton();
      this.aboutButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.startButton.Enabled = false;
      this.startButton.Location = new System.Drawing.Point(498, 511);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(75, 23);
      this.startButton.TabIndex = 1;
      this.startButton.Text = "&Start";
      this.toolTip.SetToolTip(this.startButton, "Start Selected Application");
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.applicationsListView_ItemActivate);
      // 
      // largeImageList
      // 
      this.largeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.largeImageList.ImageSize = new System.Drawing.Size(32, 32);
      this.largeImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // applicationsListView
      // 
      this.applicationsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.applicationsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.versionColumnHeader,
            this.descriptionColumnHeader});
      listViewGroup1.Header = "Applications";
      listViewGroup1.Name = "Applications";
      listViewGroup2.Header = "Plugin Management";
      listViewGroup2.Name = "Plugin Management";
      this.applicationsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
      this.applicationsListView.LargeImageList = this.largeImageList;
      this.applicationsListView.Location = new System.Drawing.Point(12, 12);
      this.applicationsListView.MultiSelect = false;
      this.applicationsListView.Name = "applicationsListView";
      this.applicationsListView.ShowItemToolTips = true;
      this.applicationsListView.Size = new System.Drawing.Size(642, 493);
      this.applicationsListView.SmallImageList = this.smallImageList;
      this.applicationsListView.TabIndex = 0;
      this.applicationsListView.UseCompatibleStateImageBehavior = false;
      this.applicationsListView.ItemActivate += new System.EventHandler(this.applicationsListView_ItemActivate);
      this.applicationsListView.SelectedIndexChanged += new System.EventHandler(this.applicationsListView_SelectedIndexChanged);
      // 
      // nameColumnHeader
      // 
      this.nameColumnHeader.Text = "Name";
      this.nameColumnHeader.Width = 125;
      // 
      // versionColumnHeader
      // 
      this.versionColumnHeader.Text = "Version";
      // 
      // descriptionColumnHeader
      // 
      this.descriptionColumnHeader.Text = "Description";
      this.descriptionColumnHeader.Width = 453;
      // 
      // smallImageList
      // 
      this.smallImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.smallImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.smallImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // showLargeIconsButton
      // 
      this.showLargeIconsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.showLargeIconsButton.Appearance = System.Windows.Forms.Appearance.Button;
      this.showLargeIconsButton.Checked = true;
      this.showLargeIconsButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.ShowIcons;
      this.showLargeIconsButton.Location = new System.Drawing.Point(12, 511);
      this.showLargeIconsButton.Name = "showLargeIconsButton";
      this.showLargeIconsButton.Size = new System.Drawing.Size(23, 23);
      this.showLargeIconsButton.TabIndex = 2;
      this.showLargeIconsButton.TabStop = true;
      this.toolTip.SetToolTip(this.showLargeIconsButton, "Show Large Icons");
      this.showLargeIconsButton.UseVisualStyleBackColor = false;
      this.showLargeIconsButton.Click += new System.EventHandler(this.largeIconsButton_Click);
      // 
      // showDetailsButton
      // 
      this.showDetailsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.showDetailsButton.Appearance = System.Windows.Forms.Appearance.Button;
      this.showDetailsButton.Image = global::HeuristicLab.PluginInfrastructure.Resources.ShowDetails;
      this.showDetailsButton.Location = new System.Drawing.Point(41, 511);
      this.showDetailsButton.Name = "showDetailsButton";
      this.showDetailsButton.Size = new System.Drawing.Size(23, 23);
      this.showDetailsButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.showDetailsButton, "Show Details");
      this.showDetailsButton.UseVisualStyleBackColor = false;
      this.showDetailsButton.Click += new System.EventHandler(this.detailsButton_Click);
      // 
      // aboutButton
      // 
      this.aboutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.aboutButton.Location = new System.Drawing.Point(579, 511);
      this.aboutButton.Name = "aboutButton";
      this.aboutButton.Size = new System.Drawing.Size(75, 23);
      this.aboutButton.TabIndex = 4;
      this.aboutButton.Text = "&About...";
      this.aboutButton.UseVisualStyleBackColor = true;
      this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
      // 
      // StarterForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.ClientSize = new System.Drawing.Size(666, 546);
      this.Controls.Add(this.aboutButton);
      this.Controls.Add(this.showDetailsButton);
      this.Controls.Add(this.showLargeIconsButton);
      this.Controls.Add(this.applicationsListView);
      this.Controls.Add(this.startButton);
      this.Icon = HeuristicLab.PluginInfrastructure.Resources.HeuristicLab;
      this.Name = "StarterForm";
      this.Text = "HeuristicLab Starter";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StarterForm_FormClosing);
      this.Load += new System.EventHandler(this.StarterForm_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.ImageList largeImageList;
    private System.Windows.Forms.ListView applicationsListView;
    private System.Windows.Forms.ColumnHeader nameColumnHeader;
    private System.Windows.Forms.ColumnHeader versionColumnHeader;
    private System.Windows.Forms.ColumnHeader descriptionColumnHeader;
    private System.Windows.Forms.ImageList smallImageList;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.RadioButton showLargeIconsButton;
    private System.Windows.Forms.RadioButton showDetailsButton;
    private System.Windows.Forms.Button aboutButton;
  }
}
