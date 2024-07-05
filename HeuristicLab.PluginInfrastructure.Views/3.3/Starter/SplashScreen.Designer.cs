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
      this.versionLabel = new System.Windows.Forms.Label();
      this.infoLabel = new System.Windows.Forms.Label();
      this.copyrightLabel = new System.Windows.Forms.Label();
      this.pictureBox = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // versionLabel
      // 
      this.versionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.versionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
      this.versionLabel.Location = new System.Drawing.Point(436, 147);
      this.versionLabel.Name = "versionLabel";
      this.versionLabel.Size = new System.Drawing.Size(150, 9);
      this.versionLabel.TabIndex = 1;
      this.versionLabel.Text = "Version ";
      this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // infoLabel
      // 
      this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.infoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.infoLabel.ForeColor = System.Drawing.Color.DarkOrange;
      this.infoLabel.Location = new System.Drawing.Point(12, 113);
      this.infoLabel.Name = "infoLabel";
      this.infoLabel.Size = new System.Drawing.Size(574, 30);
      this.infoLabel.TabIndex = 6;
      this.infoLabel.Text = "Startup Information";
      this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // copyrightLabel
      // 
      this.copyrightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.copyrightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
      this.copyrightLabel.Location = new System.Drawing.Point(12, 147);
      this.copyrightLabel.Name = "copyrightLabel";
      this.copyrightLabel.Size = new System.Drawing.Size(150, 9);
      this.copyrightLabel.TabIndex = 2;
      this.copyrightLabel.Text = "Copyright";
      this.copyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // pictureBox
      // 
      this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox.Image = global::HeuristicLab.PluginInfrastructure.Resources.HeuristicLabBanner;
      this.pictureBox.Location = new System.Drawing.Point(0, 0);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(598, 110);
      this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      // 
      // SplashScreen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(598, 165);
      this.ControlBox = false;
      this.Controls.Add(this.pictureBox);
      this.Controls.Add(this.versionLabel);
      this.Controls.Add(this.copyrightLabel);
      this.Controls.Add(this.infoLabel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = HeuristicLab.PluginInfrastructure.Resources.HeuristicLab;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SplashScreen";
      this.Opacity = 0.99D;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox;
    private System.Windows.Forms.Label versionLabel;
    private System.Windows.Forms.Label copyrightLabel;
    private System.Windows.Forms.Label infoLabel;
  }
}
