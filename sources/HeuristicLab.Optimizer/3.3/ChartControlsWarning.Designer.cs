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
namespace HeuristicLab.Optimizer {
  partial class ChartControlsWarning {
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
      this.messageTextBox = new System.Windows.Forms.TextBox();
      this.iconLabel = new System.Windows.Forms.Label();
      this.okButton = new System.Windows.Forms.Button();
      this.linkLabel = new System.Windows.Forms.LinkLabel();
      this.SuspendLayout();
      // 
      // messageTextBox
      // 
      this.messageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.messageTextBox.BackColor = System.Drawing.SystemColors.Control;
      this.messageTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.messageTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.messageTextBox.Location = new System.Drawing.Point(39, 12);
      this.messageTextBox.Multiline = true;
      this.messageTextBox.Name = "messageTextBox";
      this.messageTextBox.ReadOnly = true;
      this.messageTextBox.Size = new System.Drawing.Size(284, 27);
      this.messageTextBox.TabIndex = 3;
      this.messageTextBox.Text = "Microsoft Chart Controls are not properly installed. Please download and install " +
          "them.";
      // 
      // iconLabel
      // 
      this.iconLabel.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Warning;
      this.iconLabel.Location = new System.Drawing.Point(12, 17);
      this.iconLabel.Name = "iconLabel";
      this.iconLabel.Size = new System.Drawing.Size(21, 22);
      this.iconLabel.TabIndex = 1;
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(248, 41);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 6;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      // 
      // linkLabel
      // 
      this.linkLabel.AutoSize = true;
      this.linkLabel.Location = new System.Drawing.Point(36, 42);
      this.linkLabel.Name = "linkLabel";
      this.linkLabel.Size = new System.Drawing.Size(141, 13);
      this.linkLabel.TabIndex = 9;
      this.linkLabel.TabStop = true;
      this.linkLabel.Text = "MS Chart Controls download";
      this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
      // 
      // ChartControlsWarning
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(335, 70);
      this.Controls.Add(this.linkLabel);
      this.Controls.Add(this.okButton);
      this.Controls.Add(this.iconLabel);
      this.Controls.Add(this.messageTextBox);
      this.Icon = HeuristicLab.Common.Resources.HeuristicLab.Icon;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ChartControlsWarning";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Warning";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox messageTextBox;
    private System.Windows.Forms.Label iconLabel;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.LinkLabel linkLabel;
  }
}