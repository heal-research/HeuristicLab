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
  partial class AboutDialog {
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
      this.okButton = new System.Windows.Forms.Button();
      this.pluginListView = new System.Windows.Forms.ListView();
      this.pluginNameColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginVersionColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.pluginDescriptionColumnHeader = new System.Windows.Forms.ColumnHeader();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.label = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.productTextBox = new System.Windows.Forms.TextBox();
      this.versionTextBox = new System.Windows.Forms.TextBox();
      this.copyrightTextBox = new System.Windows.Forms.TextBox();
      this.pluginsGroupBox = new System.Windows.Forms.GroupBox();
      this.licenseTextBox = new System.Windows.Forms.RichTextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.label4 = new System.Windows.Forms.Label();
      this.webLinkLabel = new System.Windows.Forms.LinkLabel();
      this.mailLinkLabel = new System.Windows.Forms.LinkLabel();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.pluginsGroupBox.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(538, 13);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 0;
      this.okButton.Text = "&Close";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // pluginListView
      // 
      this.pluginListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pluginNameColumnHeader,
            this.pluginVersionColumnHeader,
            this.pluginDescriptionColumnHeader});
      this.pluginListView.Location = new System.Drawing.Point(6, 19);
      this.pluginListView.Name = "pluginListView";
      this.pluginListView.ShowGroups = false;
      this.pluginListView.Size = new System.Drawing.Size(589, 201);
      this.pluginListView.SmallImageList = this.imageList;
      this.pluginListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.pluginListView.TabIndex = 0;
      this.pluginListView.UseCompatibleStateImageBehavior = false;
      this.pluginListView.View = System.Windows.Forms.View.Details;
      this.pluginListView.ItemActivate += new System.EventHandler(this.pluginListView_ItemActivate);
      // 
      // pluginNameColumnHeader
      // 
      this.pluginNameColumnHeader.Text = "Name";
      // 
      // pluginVersionColumnHeader
      // 
      this.pluginVersionColumnHeader.Text = "Version";
      // 
      // pluginDescriptionColumnHeader
      // 
      this.pluginDescriptionColumnHeader.Text = "Description";
      // 
      // imageList
      // 
      this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
      this.imageList.ImageSize = new System.Drawing.Size(16, 16);
      this.imageList.TransparentColor = System.Drawing.Color.Transparent;
      // 
      // pictureBox
      // 
      this.pictureBox.Location = new System.Drawing.Point(12, 12);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(160, 180);
      this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.pictureBox.TabIndex = 2;
      this.pictureBox.TabStop = false;
      // 
      // label
      // 
      this.label.AutoSize = true;
      this.label.Location = new System.Drawing.Point(183, 12);
      this.label.Name = "label";
      this.label.Size = new System.Drawing.Size(47, 13);
      this.label.TabIndex = 1;
      this.label.Text = "Product:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(183, 31);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(45, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Version:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(183, 50);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(54, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "Copyright:";
      // 
      // productTextBox
      // 
      this.productTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.productTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.productTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.productTextBox.Location = new System.Drawing.Point(258, 12);
      this.productTextBox.Name = "productTextBox";
      this.productTextBox.ReadOnly = true;
      this.productTextBox.Size = new System.Drawing.Size(355, 13);
      this.productTextBox.TabIndex = 2;
      this.productTextBox.Text = "HeuristicLab";
      // 
      // versionTextBox
      // 
      this.versionTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.versionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.versionTextBox.Location = new System.Drawing.Point(258, 31);
      this.versionTextBox.Name = "versionTextBox";
      this.versionTextBox.ReadOnly = true;
      this.versionTextBox.Size = new System.Drawing.Size(355, 13);
      this.versionTextBox.TabIndex = 4;
      this.versionTextBox.Text = "1.0";
      // 
      // copyrightTextBox
      // 
      this.copyrightTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.copyrightTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.copyrightTextBox.Location = new System.Drawing.Point(258, 50);
      this.copyrightTextBox.Name = "copyrightTextBox";
      this.copyrightTextBox.ReadOnly = true;
      this.copyrightTextBox.Size = new System.Drawing.Size(355, 13);
      this.copyrightTextBox.TabIndex = 6;
      this.copyrightTextBox.Text = "(C)";
      // 
      // pluginsGroupBox
      // 
      this.pluginsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pluginsGroupBox.Controls.Add(this.pluginListView);
      this.pluginsGroupBox.Location = new System.Drawing.Point(12, 306);
      this.pluginsGroupBox.Name = "pluginsGroupBox";
      this.pluginsGroupBox.Size = new System.Drawing.Size(601, 226);
      this.pluginsGroupBox.TabIndex = 12;
      this.pluginsGroupBox.TabStop = false;
      this.pluginsGroupBox.Text = "Plugins";
      // 
      // licenseTextBox
      // 
      this.licenseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.licenseTextBox.BackColor = System.Drawing.SystemColors.HighlightText;
      this.licenseTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.licenseTextBox.Location = new System.Drawing.Point(186, 115);
      this.licenseTextBox.Name = "licenseTextBox";
      this.licenseTextBox.ReadOnly = true;
      this.licenseTextBox.Size = new System.Drawing.Size(427, 185);
      this.licenseTextBox.TabIndex = 11;
      this.licenseTextBox.Text = "License Text";
      this.licenseTextBox.WordWrap = false;
      this.licenseTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.licenseTextBox_LinkClicked);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(183, 69);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(49, 13);
      this.label2.TabIndex = 7;
      this.label2.Text = "Website:";
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.SystemColors.Control;
      this.panel1.Controls.Add(this.okButton);
      this.panel1.Location = new System.Drawing.Point(0, 538);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(626, 48);
      this.panel1.TabIndex = 0;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(183, 88);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(47, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "Contact:";
      // 
      // webLinkLabel
      // 
      this.webLinkLabel.AutoSize = true;
      this.webLinkLabel.Location = new System.Drawing.Point(255, 69);
      this.webLinkLabel.Name = "webLinkLabel";
      this.webLinkLabel.Size = new System.Drawing.Size(135, 13);
      this.webLinkLabel.TabIndex = 8;
      this.webLinkLabel.TabStop = true;
      this.webLinkLabel.Text = "http://dev.heuristiclab.com";
      this.webLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.webLinkLabel_LinkClicked);
      // 
      // mailLinkLabel
      // 
      this.mailLinkLabel.AutoSize = true;
      this.mailLinkLabel.Location = new System.Drawing.Point(255, 88);
      this.mailLinkLabel.Name = "mailLinkLabel";
      this.mailLinkLabel.Size = new System.Drawing.Size(129, 13);
      this.mailLinkLabel.TabIndex = 10;
      this.mailLinkLabel.TabStop = true;
      this.mailLinkLabel.Text = "support@heuristiclab.com";
      this.mailLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.mailLinkLabel_LinkClicked);
      // 
      // AboutDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.SystemColors.Window;
      this.CancelButton = this.okButton;
      this.ClientSize = new System.Drawing.Size(625, 586);
      this.Controls.Add(this.mailLinkLabel);
      this.Controls.Add(this.webLinkLabel);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.licenseTextBox);
      this.Controls.Add(this.pluginsGroupBox);
      this.Controls.Add(this.copyrightTextBox);
      this.Controls.Add(this.versionTextBox);
      this.Controls.Add(this.productTextBox);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label);
      this.Controls.Add(this.pictureBox);
      this.Icon = global::HeuristicLab.PluginInfrastructure.Resources.HeuristicLab;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "About HeuristicLab";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.pluginsGroupBox.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.ListView pluginListView;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.ColumnHeader pluginNameColumnHeader;
    private System.Windows.Forms.ColumnHeader pluginVersionColumnHeader;
    private System.Windows.Forms.ColumnHeader pluginDescriptionColumnHeader;
    private System.Windows.Forms.PictureBox pictureBox;
    private System.Windows.Forms.Label label;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox productTextBox;
    private System.Windows.Forms.TextBox versionTextBox;
    private System.Windows.Forms.TextBox copyrightTextBox;
    private System.Windows.Forms.GroupBox pluginsGroupBox;
    private System.Windows.Forms.RichTextBox licenseTextBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.LinkLabel webLinkLabel;
    private System.Windows.Forms.LinkLabel mailLinkLabel;
  }
}