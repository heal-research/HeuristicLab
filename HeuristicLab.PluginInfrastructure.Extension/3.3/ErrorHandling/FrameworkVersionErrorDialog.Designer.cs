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

namespace HeuristicLab.PluginInfrastructure {
  partial class FrameworkVersionErrorDialog {
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
      this.iconLabel = new System.Windows.Forms.Label();
      this.closeButton = new System.Windows.Forms.Button();
      this.linkLabel = new System.Windows.Forms.LinkLabel();
      this.label = new System.Windows.Forms.Label();
      this.linkLabelMono = new System.Windows.Forms.LinkLabel();
      this.SuspendLayout();
      // 
      // iconLabel
      // 
      this.iconLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.iconLabel.Image = global::HeuristicLab.PluginInfrastructure.Resources.Error;
      this.iconLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
      this.iconLabel.Location = new System.Drawing.Point(12, 9);
      this.iconLabel.Name = "iconLabel";
      this.iconLabel.Size = new System.Drawing.Size(50, 121);
      this.iconLabel.TabIndex = 0;
      // 
      // closeButton
      // 
      this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.closeButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.closeButton.Location = new System.Drawing.Point(294, 104);
      this.closeButton.Name = "closeButton";
      this.closeButton.Size = new System.Drawing.Size(75, 23);
      this.closeButton.TabIndex = 3;
      this.closeButton.Text = "&Close";
      this.closeButton.UseVisualStyleBackColor = true;
      this.closeButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // linkLabel
      // 
      this.linkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.linkLabel.Location = new System.Drawing.Point(68, 52);
      this.linkLabel.Name = "linkLabel";
      this.linkLabel.Size = new System.Drawing.Size(301, 49);
      this.linkLabel.TabIndex = 2;
      this.linkLabel.TabStop = true;
      this.linkLabel.Text = "Download Microsoft .NET Framework 4.5";
      this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
      // 
      // label
      // 
      this.label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.label.Location = new System.Drawing.Point(68, 9);
      this.label.Name = "label";
      this.label.Size = new System.Drawing.Size(301, 43);
      this.label.TabIndex = 1;
      this.label.Text = "To run HeuristicLab you need at least the Microsoft .NET Framework 4.5 or Mono ve" +
    "rsion 3.6.0 or higher. Please download and install it.";
      // 
      // linkLabelMono
      // 
      this.linkLabelMono.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.linkLabelMono.Location = new System.Drawing.Point(68, 72);
      this.linkLabelMono.Name = "linkLabelMono";
      this.linkLabelMono.Size = new System.Drawing.Size(301, 16);
      this.linkLabelMono.TabIndex = 4;
      this.linkLabelMono.TabStop = true;
      this.linkLabelMono.Text = "Download Mono";
      this.linkLabelMono.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelMono_LinkClicked);
      // 
      // FrameworkVersionErrorDialog
      // 
      this.AcceptButton = this.closeButton;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.closeButton;
      this.ClientSize = new System.Drawing.Size(381, 139);
      this.Controls.Add(this.linkLabelMono);
      this.Controls.Add(this.label);
      this.Controls.Add(this.linkLabel);
      this.Controls.Add(this.closeButton);
      this.Controls.Add(this.iconLabel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = global::HeuristicLab.PluginInfrastructure.Resources.HeuristicLab;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FrameworkVersionErrorDialog";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Error";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label iconLabel;
    private System.Windows.Forms.Button closeButton;
    private System.Windows.Forms.LinkLabel linkLabel;
    private System.Windows.Forms.Label label;
    private System.Windows.Forms.LinkLabel linkLabelMono;
  }
}