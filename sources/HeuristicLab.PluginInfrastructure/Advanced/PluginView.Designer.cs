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
      this.licenseButton = new System.Windows.Forms.Button();
      this.dependenciesGroupBox = new System.Windows.Forms.GroupBox();
      this.dependenciesListView = new System.Windows.Forms.ListView();
      this.pluginNameHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginVersionHeader = new System.Windows.Forms.ColumnHeader();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.filesListView = new System.Windows.Forms.ListView();
      this.fileNameHeader = new System.Windows.Forms.ColumnHeader();
      this.fileTypeHeader = new System.Windows.Forms.ColumnHeader();
      this.filesGroupBox = new System.Windows.Forms.GroupBox();
      this.stateTextBox = new System.Windows.Forms.TextBox();
      this.stateLabel = new System.Windows.Forms.Label();
      this.errorLabel = new System.Windows.Forms.Label();
      this.descriptionLabel = new System.Windows.Forms.Label();
      this.errorTextBox = new System.Windows.Forms.RichTextBox();
      this.descriptionTextBox = new System.Windows.Forms.RichTextBox();
      this.dependenciesGroupBox.SuspendLayout();
      this.filesGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // nameLabel
      // 
      this.nameLabel.AutoSize = true;
      this.nameLabel.Location = new System.Drawing.Point(3, 6);
      this.nameLabel.Name = "nameLabel";
      this.nameLabel.Size = new System.Drawing.Size(38, 13);
      this.nameLabel.TabIndex = 0;
      this.nameLabel.Text = "Name:";
      // 
      // nameTextBox
      // 
      this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.nameTextBox.Location = new System.Drawing.Point(74, 3);
      this.nameTextBox.Name = "nameTextBox";
      this.nameTextBox.ReadOnly = true;
      this.nameTextBox.Size = new System.Drawing.Size(284, 20);
      this.nameTextBox.TabIndex = 1;
      // 
      // versionTextBox
      // 
      this.versionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.versionTextBox.Location = new System.Drawing.Point(74, 29);
      this.versionTextBox.Name = "versionTextBox";
      this.versionTextBox.ReadOnly = true;
      this.versionTextBox.Size = new System.Drawing.Size(284, 20);
      this.versionTextBox.TabIndex = 3;
      // 
      // versionLabel
      // 
      this.versionLabel.AutoSize = true;
      this.versionLabel.Location = new System.Drawing.Point(3, 32);
      this.versionLabel.Name = "versionLabel";
      this.versionLabel.Size = new System.Drawing.Size(45, 13);
      this.versionLabel.TabIndex = 2;
      this.versionLabel.Text = "Version:";
      // 
      // contactTextBox
      // 
      this.contactTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.contactTextBox.Location = new System.Drawing.Point(74, 55);
      this.contactTextBox.Name = "contactTextBox";
      this.contactTextBox.ReadOnly = true;
      this.contactTextBox.Size = new System.Drawing.Size(284, 20);
      this.contactTextBox.TabIndex = 5;
      // 
      // contactInfoLabel
      // 
      this.contactInfoLabel.AutoSize = true;
      this.contactInfoLabel.Location = new System.Drawing.Point(3, 58);
      this.contactInfoLabel.Name = "contactInfoLabel";
      this.contactInfoLabel.Size = new System.Drawing.Size(47, 13);
      this.contactInfoLabel.TabIndex = 4;
      this.contactInfoLabel.Text = "Contact:";
      // 
      // licenseButton
      // 
      this.licenseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.licenseButton.Location = new System.Drawing.Point(3, 586);
      this.licenseButton.Name = "licenseButton";
      this.licenseButton.Size = new System.Drawing.Size(103, 23);
      this.licenseButton.TabIndex = 10;
      this.licenseButton.Text = "Show license";
      this.licenseButton.UseVisualStyleBackColor = true;
      this.licenseButton.Click += new System.EventHandler(this.licenseButton_Click);
      // 
      // dependenciesGroupBox
      // 
      this.dependenciesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dependenciesGroupBox.Controls.Add(this.dependenciesListView);
      this.dependenciesGroupBox.Location = new System.Drawing.Point(6, 408);
      this.dependenciesGroupBox.Name = "dependenciesGroupBox";
      this.dependenciesGroupBox.Size = new System.Drawing.Size(355, 172);
      this.dependenciesGroupBox.TabIndex = 1;
      this.dependenciesGroupBox.TabStop = false;
      this.dependenciesGroupBox.Text = "Dependencies";
      // 
      // dependenciesListView
      // 
      this.dependenciesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginNameHeader,
            this.pluginVersionHeader});
      this.dependenciesListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dependenciesListView.Location = new System.Drawing.Point(3, 16);
      this.dependenciesListView.Name = "dependenciesListView";
      this.dependenciesListView.Size = new System.Drawing.Size(349, 153);
      this.dependenciesListView.SmallImageList = this.imageList;
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
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // filesListView
      // 
      this.filesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileNameHeader,
            this.fileTypeHeader});
      this.filesListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.filesListView.Location = new System.Drawing.Point(3, 16);
      this.filesListView.Name = "filesListView";
      this.filesListView.Size = new System.Drawing.Size(346, 140);
      this.filesListView.SmallImageList = this.imageList;
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
      // filesGroupBox
      // 
      this.filesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.filesGroupBox.Controls.Add(this.filesListView);
      this.filesGroupBox.Location = new System.Drawing.Point(9, 243);
      this.filesGroupBox.Name = "filesGroupBox";
      this.filesGroupBox.Size = new System.Drawing.Size(352, 159);
      this.filesGroupBox.TabIndex = 11;
      this.filesGroupBox.TabStop = false;
      this.filesGroupBox.Text = "Files";
      // 
      // stateTextBox
      // 
      this.stateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.stateTextBox.Location = new System.Drawing.Point(74, 145);
      this.stateTextBox.Name = "stateTextBox";
      this.stateTextBox.ReadOnly = true;
      this.stateTextBox.Size = new System.Drawing.Size(284, 20);
      this.stateTextBox.TabIndex = 13;
      // 
      // stateLabel
      // 
      this.stateLabel.AutoSize = true;
      this.stateLabel.Location = new System.Drawing.Point(3, 148);
      this.stateLabel.Name = "stateLabel";
      this.stateLabel.Size = new System.Drawing.Size(35, 13);
      this.stateLabel.TabIndex = 12;
      this.stateLabel.Text = "State:";
      // 
      // errorLabel
      // 
      this.errorLabel.AutoSize = true;
      this.errorLabel.Location = new System.Drawing.Point(3, 174);
      this.errorLabel.Name = "errorLabel";
      this.errorLabel.Size = new System.Drawing.Size(32, 13);
      this.errorLabel.TabIndex = 15;
      this.errorLabel.Text = "Error:";
      // 
      // descriptionLabel
      // 
      this.descriptionLabel.AutoSize = true;
      this.descriptionLabel.Location = new System.Drawing.Point(3, 84);
      this.descriptionLabel.Name = "descriptionLabel";
      this.descriptionLabel.Size = new System.Drawing.Size(63, 13);
      this.descriptionLabel.TabIndex = 17;
      this.descriptionLabel.Text = "Description:";
      // 
      // errorTextBox
      // 
      this.errorTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.errorTextBox.Location = new System.Drawing.Point(74, 171);
      this.errorTextBox.Name = "errorTextBox";
      this.errorTextBox.ReadOnly = true;
      this.errorTextBox.Size = new System.Drawing.Size(284, 66);
      this.errorTextBox.TabIndex = 18;
      this.errorTextBox.Text = "";
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.descriptionTextBox.Location = new System.Drawing.Point(74, 81);
      this.descriptionTextBox.Name = "descriptionTextBox";
      this.descriptionTextBox.ReadOnly = true;
      this.descriptionTextBox.Size = new System.Drawing.Size(284, 58);
      this.descriptionTextBox.TabIndex = 19;
      this.descriptionTextBox.Text = "";
      // 
      // PluginView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(345, 576);
      this.Controls.Add(this.descriptionTextBox);
      this.Controls.Add(this.errorTextBox);
      this.Controls.Add(this.errorLabel);
      this.Controls.Add(this.descriptionLabel);
      this.Controls.Add(this.stateTextBox);
      this.Controls.Add(this.stateLabel);
      this.Controls.Add(this.dependenciesGroupBox);
      this.Controls.Add(this.filesGroupBox);
      this.Controls.Add(this.licenseButton);
      this.Controls.Add(this.contactTextBox);
      this.Controls.Add(this.contactInfoLabel);
      this.Controls.Add(this.versionTextBox);
      this.Controls.Add(this.versionLabel);
      this.Controls.Add(this.nameTextBox);
      this.Controls.Add(this.nameLabel);
      this.Name = "PluginView";
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
    protected System.Windows.Forms.Button licenseButton;
    protected System.Windows.Forms.GroupBox dependenciesGroupBox;
    private System.Windows.Forms.ColumnHeader pluginNameHeader;
    private System.Windows.Forms.ColumnHeader pluginVersionHeader;
    protected System.Windows.Forms.ListView dependenciesListView;
    private System.Windows.Forms.ListView filesListView;
    private System.Windows.Forms.GroupBox filesGroupBox;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.ColumnHeader fileNameHeader;
    private System.Windows.Forms.ColumnHeader fileTypeHeader;
    protected System.Windows.Forms.TextBox stateTextBox;
    protected System.Windows.Forms.Label stateLabel;
    protected System.Windows.Forms.Label errorLabel;
    protected System.Windows.Forms.Label descriptionLabel;
    private System.Windows.Forms.RichTextBox errorTextBox;
    private System.Windows.Forms.RichTextBox descriptionTextBox;

  }
}
