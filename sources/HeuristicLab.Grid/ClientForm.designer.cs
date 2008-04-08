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

namespace HeuristicLab.Grid {
  partial class ClientForm {
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
      this.label1 = new System.Windows.Forms.Label();
      this.stopButton = new System.Windows.Forms.Button();
      this.startButton = new System.Windows.Forms.Button();
      this.addressTextBox = new System.Windows.Forms.TextBox();
      this.statusLabel = new System.Windows.Forms.Label();
      this.statusTextBox = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.clientPort = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(9, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(81, 13);
      this.label1.TabIndex = 7;
      this.label1.Text = "&Server address:";
      // 
      // stopButton
      // 
      this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stopButton.Enabled = false;
      this.stopButton.Location = new System.Drawing.Point(99, 87);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(75, 23);
      this.stopButton.TabIndex = 6;
      this.stopButton.Text = "St&op";
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.startButton.Location = new System.Drawing.Point(15, 87);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(75, 23);
      this.startButton.TabIndex = 5;
      this.startButton.Text = "St&art";
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // addressTextBox
      // 
      this.addressTextBox.Location = new System.Drawing.Point(96, 6);
      this.addressTextBox.Name = "addressTextBox";
      this.addressTextBox.Size = new System.Drawing.Size(222, 20);
      this.addressTextBox.TabIndex = 4;
      // 
      // statusLabel
      // 
      this.statusLabel.AutoSize = true;
      this.statusLabel.Location = new System.Drawing.Point(9, 56);
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Size = new System.Drawing.Size(67, 13);
      this.statusLabel.TabIndex = 8;
      this.statusLabel.Text = "&Client status:";
      // 
      // statusTextBox
      // 
      this.statusTextBox.Location = new System.Drawing.Point(96, 53);
      this.statusTextBox.Name = "statusTextBox";
      this.statusTextBox.ReadOnly = true;
      this.statusTextBox.Size = new System.Drawing.Size(222, 20);
      this.statusTextBox.TabIndex = 9;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(9, 32);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(57, 13);
      this.label2.TabIndex = 10;
      this.label2.Text = "Client port:";
      // 
      // clientPort
      // 
      this.clientPort.Location = new System.Drawing.Point(96, 29);
      this.clientPort.Name = "clientPort";
      this.clientPort.Size = new System.Drawing.Size(222, 20);
      this.clientPort.TabIndex = 11;
      this.clientPort.Text = "8002";
      // 
      // ClientForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(330, 122);
      this.Controls.Add(this.clientPort);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.statusTextBox);
      this.Controls.Add(this.statusLabel);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.stopButton);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.addressTextBox);
      this.Name = "ClientForm";
      this.Text = "Grid Client";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.TextBox addressTextBox;
    private System.Windows.Forms.Label statusLabel;
    private System.Windows.Forms.TextBox statusTextBox;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox clientPort;
  }
}
