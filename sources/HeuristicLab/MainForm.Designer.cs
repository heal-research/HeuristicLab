#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab {
  partial class MainForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if(disposing && (components != null)) {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Plugin Management", System.Windows.Forms.HorizontalAlignment.Left);
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Applications", System.Windows.Forms.HorizontalAlignment.Left);
      this.startButton = new System.Windows.Forms.Button();
      this.largeImageList = new System.Windows.Forms.ImageList(this.components);
      this.applicationsListView = new System.Windows.Forms.ListView();
      this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.versionColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.descriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.smallImageList = new System.Windows.Forms.ImageList(this.components);
      this.detailsButton = new System.Windows.Forms.Button();
      this.listButton = new System.Windows.Forms.Button();
      this.largeIconsButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.startButton.Enabled = false;
      this.startButton.Location = new System.Drawing.Point(579, 511);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(75, 23);
      this.startButton.TabIndex = 1;
      this.startButton.Text = "Start";
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.applicationsListView_ItemActivate);
      // 
      // largeImageList
      // 
      this.largeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("largeImageList.ImageStream")));
      this.largeImageList.TransparentColor = System.Drawing.Color.Transparent;
      this.largeImageList.Images.SetKeyName(0, "HeuristicLab.ico");
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
      listViewGroup1.Header = "Plugin Management";
      listViewGroup1.Name = "Plugin Management";
      listViewGroup2.Header = "Applications";
      listViewGroup2.Name = "Applications";
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
      this.applicationsListView.TabIndex = 2;
      this.applicationsListView.UseCompatibleStateImageBehavior = false;
      this.applicationsListView.ItemActivate += new System.EventHandler(this.applicationsListView_ItemActivate);
      this.applicationsListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.applicationsListBox_SelectedIndexChanged);
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
      this.smallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallImageList.ImageStream")));
      this.smallImageList.TransparentColor = System.Drawing.Color.Transparent;
      this.smallImageList.Images.SetKeyName(0, "HeuristicLab.ico");
      // 
      // detailsButton
      // 
      this.detailsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.detailsButton.AutoSize = true;
      this.detailsButton.Image = global::HeuristicLab.Properties.Resources.Details;
      this.detailsButton.Location = new System.Drawing.Point(68, 511);
      this.detailsButton.Name = "detailsButton";
      this.detailsButton.Size = new System.Drawing.Size(22, 23);
      this.detailsButton.TabIndex = 5;
      this.detailsButton.UseVisualStyleBackColor = true;
      this.detailsButton.Click += new System.EventHandler(this.detailsButton_Click);
      // 
      // listButton
      // 
      this.listButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.listButton.AutoSize = true;
      this.listButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.listButton.Image = global::HeuristicLab.Properties.Resources.List;
      this.listButton.Location = new System.Drawing.Point(40, 511);
      this.listButton.Name = "listButton";
      this.listButton.Size = new System.Drawing.Size(22, 22);
      this.listButton.TabIndex = 4;
      this.listButton.UseVisualStyleBackColor = true;
      this.listButton.Click += new System.EventHandler(this.listButton_Click);
      // 
      // largeIconsButton
      // 
      this.largeIconsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.largeIconsButton.AutoSize = true;
      this.largeIconsButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.largeIconsButton.Image = global::HeuristicLab.Properties.Resources.LargeIcons;
      this.largeIconsButton.Location = new System.Drawing.Point(12, 511);
      this.largeIconsButton.Name = "largeIconsButton";
      this.largeIconsButton.Size = new System.Drawing.Size(22, 22);
      this.largeIconsButton.TabIndex = 3;
      this.largeIconsButton.UseVisualStyleBackColor = true;
      this.largeIconsButton.Click += new System.EventHandler(this.largeIconsButton_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(666, 546);
      this.Controls.Add(this.detailsButton);
      this.Controls.Add(this.listButton);
      this.Controls.Add(this.largeIconsButton);
      this.Controls.Add(this.applicationsListView);
      this.Controls.Add(this.startButton);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MainForm";
      this.Text = "HeuristicLab Starter";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.ImageList largeImageList;
    private System.Windows.Forms.ListView applicationsListView;
    private System.Windows.Forms.ColumnHeader nameColumnHeader;
    private System.Windows.Forms.ColumnHeader versionColumnHeader;
    private System.Windows.Forms.ColumnHeader descriptionColumnHeader;
    private System.Windows.Forms.Button largeIconsButton;
    private System.Windows.Forms.Button listButton;
    private System.Windows.Forms.Button detailsButton;
    private System.Windows.Forms.ImageList smallImageList;
  }
}
