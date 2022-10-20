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
  partial class ErrorDialog {
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
      this.okButton = new System.Windows.Forms.Button();
      this.iconLabel = new System.Windows.Forms.Label();
      this.messageTextBox = new System.Windows.Forms.TextBox();
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.detailsTextBox = new System.Windows.Forms.TextBox();
      this.supportLabel = new System.Windows.Forms.Label();
      this.supportLinkLabel = new System.Windows.Forms.LinkLabel();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(497, 327);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 0;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      // 
      // iconLabel
      // 
      this.iconLabel.Image = global::HeuristicLab.PluginInfrastructure.Resources.Error;
      this.iconLabel.Location = new System.Drawing.Point(12, 9);
      this.iconLabel.Name = "iconLabel";
      this.iconLabel.Size = new System.Drawing.Size(50, 60);
      this.iconLabel.TabIndex = 1;
      // 
      // messageTextBox
      // 
      this.messageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.messageTextBox.BackColor = System.Drawing.SystemColors.Control;
      this.messageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.messageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.messageTextBox.Location = new System.Drawing.Point(68, 12);
      this.messageTextBox.Multiline = true;
      this.messageTextBox.Name = "messageTextBox";
      this.messageTextBox.ReadOnly = true;
      this.messageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.messageTextBox.Size = new System.Drawing.Size(504, 57);
      this.messageTextBox.TabIndex = 2;
      this.messageTextBox.Text = "Error Message";
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsGroupBox.Controls.Add(this.detailsTextBox);
      this.detailsGroupBox.Location = new System.Drawing.Point(12, 75);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(560, 246);
      this.detailsGroupBox.TabIndex = 3;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // detailsTextBox
      // 
      this.detailsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsTextBox.BackColor = System.Drawing.SystemColors.Control;
      this.detailsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.detailsTextBox.Location = new System.Drawing.Point(6, 19);
      this.detailsTextBox.Multiline = true;
      this.detailsTextBox.Name = "detailsTextBox";
      this.detailsTextBox.ReadOnly = true;
      this.detailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.detailsTextBox.Size = new System.Drawing.Size(548, 221);
      this.detailsTextBox.TabIndex = 0;
      this.detailsTextBox.Text = "Stack Trace";
      this.detailsTextBox.WordWrap = false;
      // 
      // supportLabel
      // 
      this.supportLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.supportLabel.AutoSize = true;
      this.supportLabel.Location = new System.Drawing.Point(12, 332);
      this.supportLabel.Name = "supportLabel";
      this.supportLabel.Size = new System.Drawing.Size(264, 13);
      this.supportLabel.TabIndex = 4;
      this.supportLabel.Text = "If you have any problems or questions, please contact:";
      // 
      // supportLinkLabel
      // 
      this.supportLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.supportLinkLabel.AutoSize = true;
      this.supportLinkLabel.Location = new System.Drawing.Point(298, 332);
      this.supportLinkLabel.Name = "supportLinkLabel";
      this.supportLinkLabel.Size = new System.Drawing.Size(129, 13);
      this.supportLinkLabel.TabIndex = 5;
      this.supportLinkLabel.TabStop = true;
      this.supportLinkLabel.Text = "support@heuristiclab.com";
      this.supportLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.supportLinkLabel_LinkClicked);
      // 
      // ErrorDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.okButton;
      this.ClientSize = new System.Drawing.Size(584, 362);
      this.Controls.Add(this.supportLinkLabel);
      this.Controls.Add(this.supportLabel);
      this.Controls.Add(this.detailsGroupBox);
      this.Controls.Add(this.messageTextBox);
      this.Controls.Add(this.iconLabel);
      this.Controls.Add(this.okButton);
      this.Icon = global::HeuristicLab.PluginInfrastructure.Resources.ErrorIcon;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ErrorDialog";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Error";
      this.TopMost = true;
      this.detailsGroupBox.ResumeLayout(false);
      this.detailsGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Label iconLabel;
    private System.Windows.Forms.TextBox messageTextBox;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private System.Windows.Forms.TextBox detailsTextBox;
    private System.Windows.Forms.Label supportLabel;
    private System.Windows.Forms.LinkLabel supportLinkLabel;
  }
}