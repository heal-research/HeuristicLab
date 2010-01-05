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

using HeuristicLab.PluginInfrastructure;
namespace HeuristicLab.PluginInfrastructure.Starter {
  partial class SplashScreen {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
      this.panel = new System.Windows.Forms.Panel();
      this.closeButton = new System.Windows.Forms.Button();
      this.imageList = new System.Windows.Forms.ImageList(this.components);
      this.companyLabel = new System.Windows.Forms.Label();
      this.userNameLabel = new System.Windows.Forms.Label();
      this.licensedToLabel = new System.Windows.Forms.Label();
      this.titleLabel = new System.Windows.Forms.Label();
      this.versionLabel = new System.Windows.Forms.Label();
      this.infoLabel = new System.Windows.Forms.Label();
      this.copyrightLabel = new System.Windows.Forms.Label();
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.panel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // panel
      // 
      this.panel.BackColor = System.Drawing.SystemColors.Window;
      this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel.Controls.Add(this.closeButton);
      this.panel.Controls.Add(this.companyLabel);
      this.panel.Controls.Add(this.userNameLabel);
      this.panel.Controls.Add(this.licensedToLabel);
      this.panel.Controls.Add(this.titleLabel);
      this.panel.Controls.Add(this.versionLabel);
      this.panel.Controls.Add(this.infoLabel);
      this.panel.Controls.Add(this.copyrightLabel);
      this.panel.Controls.Add(this.pictureBox);
      this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel.Location = new System.Drawing.Point(0, 0);
      this.panel.Name = "panel";
      this.panel.Size = new System.Drawing.Size(625, 161);
      this.panel.TabIndex = 0;
      // 
      // closeButton
      // 
      this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.closeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.White;
      this.closeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
      this.closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
      this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.closeButton.ForeColor = System.Drawing.Color.White;
      this.closeButton.ImageIndex = 0;
      this.closeButton.ImageList = this.imageList;
      this.closeButton.Location = new System.Drawing.Point(604, 3);
      this.closeButton.Name = "closeButton";
      this.closeButton.Size = new System.Drawing.Size(16, 16);
      this.closeButton.TabIndex = 7;
      this.closeButton.UseVisualStyleBackColor = true;
      this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
      // 
      // imageList
      // 
      this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
      this.imageList.TransparentColor = System.Drawing.Color.Magenta;
      this.imageList.Images.SetKeyName(0, "BuilderDialog_delete.bmp");
      // 
      // companyLabel
      // 
      this.companyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.companyLabel.Location = new System.Drawing.Point(248, 88);
      this.companyLabel.Name = "companyLabel";
      this.companyLabel.Size = new System.Drawing.Size(364, 16);
      this.companyLabel.TabIndex = 5;
      this.companyLabel.Text = "Company";
      this.companyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // userNameLabel
      // 
      this.userNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.userNameLabel.BackColor = System.Drawing.SystemColors.Window;
      this.userNameLabel.Location = new System.Drawing.Point(248, 72);
      this.userNameLabel.Name = "userNameLabel";
      this.userNameLabel.Size = new System.Drawing.Size(364, 16);
      this.userNameLabel.TabIndex = 4;
      this.userNameLabel.Text = "User";
      this.userNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // licensedToLabel
      // 
      this.licensedToLabel.Location = new System.Drawing.Point(170, 72);
      this.licensedToLabel.Name = "licensedToLabel";
      this.licensedToLabel.Size = new System.Drawing.Size(72, 16);
      this.licensedToLabel.TabIndex = 3;
      this.licensedToLabel.Text = "Licensed to:";
      this.licensedToLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // titleLabel
      // 
      this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.titleLabel.Location = new System.Drawing.Point(170, 8);
      this.titleLabel.Name = "titleLabel";
      this.titleLabel.Size = new System.Drawing.Size(442, 16);
      this.titleLabel.TabIndex = 0;
      this.titleLabel.Text = "Title";
      this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // versionLabel
      // 
      this.versionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.versionLabel.Location = new System.Drawing.Point(170, 24);
      this.versionLabel.Name = "versionLabel";
      this.versionLabel.Size = new System.Drawing.Size(442, 16);
      this.versionLabel.TabIndex = 1;
      this.versionLabel.Text = "Version ";
      this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // infoLabel
      // 
      this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.infoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.infoLabel.ForeColor = System.Drawing.SystemColors.GradientActiveCaption;
      this.infoLabel.Location = new System.Drawing.Point(170, 104);
      this.infoLabel.Name = "infoLabel";
      this.infoLabel.Size = new System.Drawing.Size(442, 47);
      this.infoLabel.TabIndex = 6;
      this.infoLabel.Text = "Startup Information";
      this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // copyrightLabel
      // 
      this.copyrightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.copyrightLabel.Location = new System.Drawing.Point(170, 56);
      this.copyrightLabel.Name = "copyrightLabel";
      this.copyrightLabel.Size = new System.Drawing.Size(442, 16);
      this.copyrightLabel.TabIndex = 2;
      this.copyrightLabel.Text = "Copyright";
      this.copyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // pictureBox
      // 
      this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)));
      this.pictureBox.Image = global::HeuristicLab.PluginInfrastructure.Properties.Resources.Logo_white;
      this.pictureBox.Location = new System.Drawing.Point(-1, -1);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(165, 161);
      this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      // 
      // SplashScreen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(625, 161);
      this.ControlBox = false;
      this.Controls.Add(this.panel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SplashScreen";
      this.Opacity = 0.99;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.TopMost = true;
      this.panel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel;
    private System.Windows.Forms.PictureBox pictureBox;
    private System.Windows.Forms.Label companyLabel;
    private System.Windows.Forms.Label userNameLabel;
    private System.Windows.Forms.Label licensedToLabel;
    private System.Windows.Forms.Label titleLabel;
    private System.Windows.Forms.Label versionLabel;
    private System.Windows.Forms.Label copyrightLabel;
    private System.Windows.Forms.Label infoLabel;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.Button closeButton;
  }
}
