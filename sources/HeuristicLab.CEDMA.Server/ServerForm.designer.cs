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

namespace HeuristicLab.CEDMA.Server {
  partial class ServerForm {
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
      this.addressTextBox = new System.Windows.Forms.TextBox();
      this.externalAddressLabel = new System.Windows.Forms.Label();
      this.activeAgentsLabel = new System.Windows.Forms.Label();
      this.activeAgentsTextBox = new System.Windows.Forms.TextBox();
      this.gridAddressLabel = new System.Windows.Forms.Label();
      this.gridAddress = new System.Windows.Forms.TextBox();
      this.startButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // addressTextBox
      // 
      this.addressTextBox.Location = new System.Drawing.Point(106, 6);
      this.addressTextBox.Name = "addressTextBox";
      this.addressTextBox.ReadOnly = true;
      this.addressTextBox.Size = new System.Drawing.Size(229, 20);
      this.addressTextBox.TabIndex = 0;
      // 
      // externalAddressLabel
      // 
      this.externalAddressLabel.AutoSize = true;
      this.externalAddressLabel.Location = new System.Drawing.Point(12, 9);
      this.externalAddressLabel.Name = "externalAddressLabel";
      this.externalAddressLabel.Size = new System.Drawing.Size(48, 13);
      this.externalAddressLabel.TabIndex = 3;
      this.externalAddressLabel.Text = "&Address:";
      // 
      // activeAgentsLabel
      // 
      this.activeAgentsLabel.AutoSize = true;
      this.activeAgentsLabel.Location = new System.Drawing.Point(12, 61);
      this.activeAgentsLabel.Name = "activeAgentsLabel";
      this.activeAgentsLabel.Size = new System.Drawing.Size(75, 13);
      this.activeAgentsLabel.TabIndex = 7;
      this.activeAgentsLabel.Text = "A&ctive agents:";
      // 
      // activeAgentsTextBox
      // 
      this.activeAgentsTextBox.Location = new System.Drawing.Point(106, 58);
      this.activeAgentsTextBox.Name = "activeAgentsTextBox";
      this.activeAgentsTextBox.ReadOnly = true;
      this.activeAgentsTextBox.Size = new System.Drawing.Size(90, 20);
      this.activeAgentsTextBox.TabIndex = 6;
      this.activeAgentsTextBox.Text = "0";
      this.activeAgentsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // gridAddressLabel
      // 
      this.gridAddressLabel.AutoSize = true;
      this.gridAddressLabel.Location = new System.Drawing.Point(12, 35);
      this.gridAddressLabel.Name = "gridAddressLabel";
      this.gridAddressLabel.Size = new System.Drawing.Size(69, 13);
      this.gridAddressLabel.TabIndex = 9;
      this.gridAddressLabel.Text = "&Grid address:";
      // 
      // gridAddress
      // 
      this.gridAddress.Location = new System.Drawing.Point(106, 32);
      this.gridAddress.Name = "gridAddress";
      this.gridAddress.Size = new System.Drawing.Size(229, 20);
      this.gridAddress.TabIndex = 8;
      // 
      // startButton
      // 
      this.startButton.Location = new System.Drawing.Point(15, 84);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(75, 23);
      this.startButton.TabIndex = 10;
      this.startButton.Text = "Start";
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // ServerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(347, 119);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.gridAddressLabel);
      this.Controls.Add(this.gridAddress);
      this.Controls.Add(this.activeAgentsLabel);
      this.Controls.Add(this.activeAgentsTextBox);
      this.Controls.Add(this.externalAddressLabel);
      this.Controls.Add(this.addressTextBox);
      this.Name = "ServerForm";
      this.Text = "Agent Server";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox addressTextBox;
    private System.Windows.Forms.Label externalAddressLabel;
    private System.Windows.Forms.Label activeAgentsLabel;
    private System.Windows.Forms.TextBox activeAgentsTextBox;
    private System.Windows.Forms.Label gridAddressLabel;
    private System.Windows.Forms.TextBox gridAddress;
    private System.Windows.Forms.Button startButton;
  }
}
