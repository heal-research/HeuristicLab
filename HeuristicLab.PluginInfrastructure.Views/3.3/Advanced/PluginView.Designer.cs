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
  partial class PluginView {
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
      this.nameLabel = new System.Windows.Forms.Label();
      this.nameTextBox = new System.Windows.Forms.TextBox();
      this.versionTextBox = new System.Windows.Forms.TextBox();
      this.versionLabel = new System.Windows.Forms.Label();
      this.contactTextBox = new System.Windows.Forms.TextBox();
      this.contactInfoLabel = new System.Windows.Forms.Label();
      this.dependenciesGroupBox = new System.Windows.Forms.GroupBox();
      this.dependenciesListView = new System.Windows.Forms.ListView();
      this.pluginNameHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginVersionHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginDescriptionHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginsImageList = new System.Windows.Forms.ImageList(this.components);
      this.filesListView = new System.Windows.Forms.ListView();
      this.fileNameHeader = new System.Windows.Forms.ColumnHeader();
      this.fileTypeHeader = new System.Windows.Forms.ColumnHeader();
      this.filesImageList = new System.Windows.Forms.ImageList(this.components);
      this.filesGroupBox = new System.Windows.Forms.GroupBox();
      this.stateTextBox = new System.Windows.Forms.TextBox();
      this.stateLabel = new System.Windows.Forms.Label();
      this.errorLabel = new System.Windows.Forms.Label();
      this.descriptionLabel = new System.Windows.Forms.Label();
      this.descriptionTextBox = new System.Windows.Forms.TextBox();
      this.errorTextBox = new System.Windows.Forms.TextBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.showLicenseButton = new System.Windows.Forms.Button();
      this.dependenciesGroupBox.SuspendLayout();
      this.filesGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(9, 15);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 0;
      this.nameLabel.Text = "Name:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.nameTextBox.Location = new System.Drawing.Point(78, 12);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.ReadOnly = true;
      this.nameTextBox.Size = new System.Drawing.Size(594, 20);
      this.nameTextBox.TabIndex = 1;
      // 
      // versionTextBox
      // 
      this.versionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.versionTextBox.Location = new System.Drawing.Point(78, 38);
      this.versionTextBox.Name = "versionTextBox";
      this.versionTextBox.ReadOnly = true;
      this.versionTextBox.Size = new System.Drawing.Size(594, 20);
      this.versionTextBox.TabIndex = 3;
      // 
      // versionLabel
      // 
      this.versionLabel.AutoSize = true;
      this.versionLabel.Location = new System.Drawing.Point(9, 41);
      this.versionLabel.Name = "versionLabel";
      this.versionLabel.Size = new System.Drawing.Size(45, 13);
      this.versionLabel.TabIndex = 2;
      this.versionLabel.Text = "Version:";
      // 
      // contactTextBox
      // 
      this.contactTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.contactTextBox.Location = new System.Drawing.Point(78, 64);
      this.contactTextBox.Name = "contactTextBox";
      this.contactTextBox.ReadOnly = true;
      this.contactTextBox.Size = new System.Drawing.Size(594, 20);
      this.contactTextBox.TabIndex = 5;
      // 
      // contactInfoLabel
      // 
      this.contactInfoLabel.AutoSize = true;
      this.contactInfoLabel.Location = new System.Drawing.Point(9, 67);
      this.contactInfoLabel.Name = "contactInfoLabel";
      this.contactInfoLabel.Size = new System.Drawing.Size(47, 13);
      this.contactInfoLabel.TabIndex = 4;
      this.contactInfoLabel.Text = "Contact:";
      // 
      // dependenciesGroupBox
      // 
      this.dependenciesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dependenciesGroupBox.Controls.Add(this.dependenciesListView);
      this.dependenciesGroupBox.Location = new System.Drawing.Point(12, 324);
      this.dependenciesGroupBox.Name = "dependenciesGroupBox";
      this.dependenciesGroupBox.Size = new System.Drawing.Size(663, 198);
      this.dependenciesGroupBox.TabIndex = 1;
      this.dependenciesGroupBox.TabStop = false;
      this.dependenciesGroupBox.Text = "Dependencies";
      // 
      // dependenciesListView
      // 
      this.dependenciesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginNameHeader,
            this.pluginVersionHeader,
            this.pluginDescriptionHeader});
      this.dependenciesListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dependenciesListView.Location = new System.Drawing.Point(3, 16);
      this.dependenciesListView.Name = "dependenciesListView";
      this.dependenciesListView.Size = new System.Drawing.Size(657, 179);
      this.dependenciesListView.SmallImageList = this.pluginsImageList;
      this.dependenciesListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.dependenciesListView.TabIndex = 0;
      this.dependenciesListView.UseCompatibleStateImageBehavior = false;
      this.dependenciesListView.View = System.Windows.Forms.View.Details;
      this.dependenciesListView.ItemActivate += new System.EventHandler(this.dependenciesListView_ItemActivate);
      // 
      // pluginNameHeader
      // 
      this.pluginNameHeader.Text = "Name";
      this.pluginNameHeader.Width = 200;
      // 
      // pluginVersionHeader
      // 
      this.pluginVersionHeader.Text = "Version";
      this.pluginVersionHeader.Width = 120;
      // 
      // pluginDescriptionHeader
      // 
      this.pluginDescriptionHeader.Text = "Description";
      this.pluginDescriptionHeader.Width = 325;
      // 
      // pluginsImageList
      // 
      this.pluginsImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.pluginsImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.pluginsImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // filesListView
      // 
      this.filesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileNameHeader,
            this.fileTypeHeader});
      this.filesListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.filesListView.Location = new System.Drawing.Point(3, 16);
      this.filesListView.Name = "filesListView";
      this.filesListView.Size = new System.Drawing.Size(657, 131);
      this.filesListView.SmallImageList = this.filesImageList;
      this.filesListView.TabIndex = 0;
      this.filesListView.UseCompatibleStateImageBehavior = false;
      this.filesListView.View = System.Windows.Forms.View.Details;
      // 
      // fileNameHeader
      // 
      this.fileNameHeader.Text = "Name";
      this.fileNameHeader.Width = 200;
      // 
      // fileTypeHeader
      // 
      this.fileTypeHeader.Text = "Type";
      this.fileTypeHeader.Width = 120;
      // 
      // filesImageList
      // 
      this.filesImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.filesImageList.ImageSize = new System.Drawing.Size(16, 16);
      this.filesImageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // filesGroupBox
      // 
      this.filesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.filesGroupBox.Controls.Add(this.filesListView);
      this.filesGroupBox.Location = new System.Drawing.Point(12, 168);
      this.filesGroupBox.Name = "filesGroupBox";
      this.filesGroupBox.Size = new System.Drawing.Size(663, 150);
      this.filesGroupBox.TabIndex = 11;
      this.filesGroupBox.TabStop = false;
      this.filesGroupBox.Text = "Files";
      // 
      // stateTextBox
      // 
      this.stateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.stateTextBox.Location = new System.Drawing.Point(78, 116);
      this.stateTextBox.Name = "stateTextBox";
      this.stateTextBox.ReadOnly = true;
      this.stateTextBox.Size = new System.Drawing.Size(594, 20);
      this.stateTextBox.TabIndex = 13;
      // 
      // stateLabel
      // 
      this.stateLabel.AutoSize = true;
      this.stateLabel.Location = new System.Drawing.Point(9, 119);
      this.stateLabel.Name = "stateLabel";
      this.stateLabel.Size = new System.Drawing.Size(35, 13);
      this.stateLabel.TabIndex = 12;
      this.stateLabel.Text = "State:";
      // 
      // errorLabel
      // 
      this.errorLabel.AutoSize = true;
      this.errorLabel.Location = new System.Drawing.Point(9, 145);
      this.errorLabel.Name = "errorLabel";
      this.errorLabel.Size = new System.Drawing.Size(32, 13);
      this.errorLabel.TabIndex = 15;
      this.errorLabel.Text = "Error:";
      // 
      // descriptionLabel
      // 
      this.descriptionLabel.AutoSize = true;
      this.descriptionLabel.Location = new System.Drawing.Point(9, 93);
      this.descriptionLabel.Name = "descriptionLabel";
      this.descriptionLabel.Size = new System.Drawing.Size(63, 13);
      this.descriptionLabel.TabIndex = 17;
      this.descriptionLabel.Text = "Description:";
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.descriptionTextBox.Location = new System.Drawing.Point(78, 90);
      this.descriptionTextBox.Multiline = true;
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.ReadOnly = true;
      this.descriptionTextBox.Size = new System.Drawing.Size(594, 20);
      this.descriptionTextBox.TabIndex = 20;
      // 
      // errorTextBox
      // 
      this.errorTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.errorTextBox.Location = new System.Drawing.Point(78, 142);
      this.errorTextBox.Multiline = true;
      this.errorTextBox.Name = "errorTextBox";
      this.errorTextBox.ReadOnly = true;
      this.errorTextBox.Size = new System.Drawing.Size(594, 20);
      this.errorTextBox.TabIndex = 21;
      // 
      // showLicenseButton
      // 
      this.showLicenseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.showLicenseButton.Location = new System.Drawing.Point(12, 528);
      this.showLicenseButton.Name = "showLicenseButton";
      this.showLicenseButton.Size = new System.Drawing.Size(87, 23);
      this.showLicenseButton.TabIndex = 22;
      this.showLicenseButton.Text = "Show License";
      this.toolTip.SetToolTip(this.showLicenseButton, "Show Plugin License");
      this.showLicenseButton.UseVisualStyleBackColor = true;
      this.showLicenseButton.Click += new System.EventHandler(this.showLicenseButton_Click);
      // 
      // PluginView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.ClientSize = new System.Drawing.Size(687, 565);
      this.Controls.Add(this.showLicenseButton);
      this.Controls.Add(this.errorTextBox);
      this.Controls.Add(this.descriptionTextBox);
      this.Controls.Add(this.errorLabel);
      this.Controls.Add(this.descriptionLabel);
      this.Controls.Add(this.stateTextBox);
      this.Controls.Add(this.stateLabel);
      this.Controls.Add(this.dependenciesGroupBox);
      this.Controls.Add(this.filesGroupBox);
      this.Controls.Add(this.contactTextBox);
      this.Controls.Add(this.contactInfoLabel);
      this.Controls.Add(this.versionTextBox);
      this.Controls.Add(this.versionLabel);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.nameLabel);
      this.Icon = global::HeuristicLab.PluginInfrastructure.Resources.HeuristicLab;
      this.Name = "PluginView";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.dependenciesGroupBox.ResumeLayout(false);
      this.filesGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label nameLabel;
    protected System.Windows.Forms.TextBox nameTextBox;
    protected System.Windows.Forms.TextBox versionTextBox;
    protected System.Windows.Forms.Label versionLabel;
    protected System.Windows.Forms.TextBox contactTextBox;
    protected System.Windows.Forms.Label contactInfoLabel;
    protected System.Windows.Forms.GroupBox dependenciesGroupBox;
    private System.Windows.Forms.ColumnHeader pluginNameHeader;
    private System.Windows.Forms.ColumnHeader pluginVersionHeader;
    protected System.Windows.Forms.ListView dependenciesListView;
    private System.Windows.Forms.ListView filesListView;
    private System.Windows.Forms.GroupBox filesGroupBox;
    private System.Windows.Forms.ImageList pluginsImageList;
    private System.Windows.Forms.ColumnHeader fileNameHeader;
    private System.Windows.Forms.ColumnHeader fileTypeHeader;
    protected System.Windows.Forms.TextBox stateTextBox;
    protected System.Windows.Forms.Label stateLabel;
    protected System.Windows.Forms.Label errorLabel;
    protected System.Windows.Forms.Label descriptionLabel;
    protected System.Windows.Forms.TextBox descriptionTextBox;
    protected System.Windows.Forms.TextBox errorTextBox;
    private System.Windows.Forms.ImageList filesImageList;
    private System.Windows.Forms.ColumnHeader pluginDescriptionHeader;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Button showLicenseButton;

  }
}
