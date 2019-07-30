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

namespace HeuristicLab.MainForm.WindowsForms {
  partial class ProgressView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;


    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.messageLabel = new System.Windows.Forms.Label();
      this.borderPanel = new System.Windows.Forms.Panel();
      this.panel = new System.Windows.Forms.Panel();
      this.stopButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.borderPanel.SuspendLayout();
      this.panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // progressBar
      // 
      this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar.Location = new System.Drawing.Point(3, 3);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(366, 23);
      this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      this.progressBar.TabIndex = 0;
      // 
      // messageLabel
      // 
      this.messageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.messageLabel.Location = new System.Drawing.Point(0, 0);
      this.messageLabel.Name = "messageLabel";
      this.messageLabel.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
      this.messageLabel.Size = new System.Drawing.Size(217, 23);
      this.messageLabel.TabIndex = 1;
      this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // borderPanel
      // 
      this.borderPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.borderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.borderPanel.Controls.Add(this.panel);
      this.borderPanel.Controls.Add(this.progressBar);
      this.borderPanel.Location = new System.Drawing.Point(0, 0);
      this.borderPanel.Name = "borderPanel";
      this.borderPanel.Size = new System.Drawing.Size(374, 62);
      this.borderPanel.TabIndex = 3;
      // 
      // panel
      // 
      this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel.Controls.Add(this.messageLabel);
      this.panel.Controls.Add(this.stopButton);
      this.panel.Controls.Add(this.cancelButton);
      this.panel.Location = new System.Drawing.Point(3, 32);
      this.panel.Name = "panel";
      this.panel.Size = new System.Drawing.Size(367, 23);
      this.panel.TabIndex = 4;
      // 
      // stopButton
      // 
      this.stopButton.Dock = System.Windows.Forms.DockStyle.Right;
      this.stopButton.Location = new System.Drawing.Point(217, 0);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(75, 23);
      this.stopButton.TabIndex = 3;
      this.stopButton.Text = "Stop";
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // cancelButton
      // 
      this.cancelButton.Dock = System.Windows.Forms.DockStyle.Right;
      this.cancelButton.Location = new System.Drawing.Point(292, 0);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 2;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // ProgressView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.borderPanel);
      this.Name = "ProgressView";
      this.Size = new System.Drawing.Size(374, 62);
      this.borderPanel.ResumeLayout(false);
      this.panel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.Label messageLabel;
    private System.Windows.Forms.Panel borderPanel;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.Panel panel;
    private System.Windows.Forms.Button cancelButton;
  }
}
