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
      this.components = new System.ComponentModel.Container();
      this.addressTextBox = new System.Windows.Forms.TextBox();
      this.externalAddressLabel = new System.Windows.Forms.Label();
      this.gridAddressLabel = new System.Windows.Forms.Label();
      this.address = new System.Windows.Forms.TextBox();
      this.connectButton = new System.Windows.Forms.Button();
      this.listBox = new System.Windows.Forms.ListBox();
      this.refreshTimer = new System.Windows.Forms.Timer(this.components);
      this.maxActiveJobsUpDown = new System.Windows.Forms.NumericUpDown();
      this.activeJobsLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.maxActiveJobsUpDown)).BeginInit();
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
      // gridAddressLabel
      // 
      this.gridAddressLabel.AutoSize = true;
      this.gridAddressLabel.Location = new System.Drawing.Point(12, 35);
      this.gridAddressLabel.Name = "gridAddressLabel";
      this.gridAddressLabel.Size = new System.Drawing.Size(69, 13);
      this.gridAddressLabel.TabIndex = 9;
      this.gridAddressLabel.Text = "&Grid address:";
      // 
      // address
      // 
      this.address.Location = new System.Drawing.Point(106, 32);
      this.address.Name = "address";
      this.address.Size = new System.Drawing.Size(160, 20);
      this.address.TabIndex = 8;
      // 
      // connectButton
      // 
      this.connectButton.Location = new System.Drawing.Point(272, 30);
      this.connectButton.Name = "connectButton";
      this.connectButton.Size = new System.Drawing.Size(75, 23);
      this.connectButton.TabIndex = 10;
      this.connectButton.Text = "Connect";
      this.connectButton.UseVisualStyleBackColor = true;
      this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
      // 
      // listBox
      // 
      this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.listBox.FormattingEnabled = true;
      this.listBox.Location = new System.Drawing.Point(12, 84);
      this.listBox.Name = "listBox";
      this.listBox.Size = new System.Drawing.Size(350, 251);
      this.listBox.TabIndex = 11;
      // 
      // refreshTimer
      // 
      this.refreshTimer.Interval = 1000;
      this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
      // 
      // maxActiveJobsUpDown
      // 
      this.maxActiveJobsUpDown.Enabled = false;
      this.maxActiveJobsUpDown.Location = new System.Drawing.Point(106, 59);
      this.maxActiveJobsUpDown.Maximum = new decimal(new int[] {
            64,
            0,
            0,
            0});
      this.maxActiveJobsUpDown.Name = "maxActiveJobsUpDown";
      this.maxActiveJobsUpDown.Size = new System.Drawing.Size(120, 20);
      this.maxActiveJobsUpDown.TabIndex = 12;
      this.maxActiveJobsUpDown.ValueChanged += new System.EventHandler(this.maxActiveJobsUpDown_ValueChanged);
      // 
      // activeJobsLabel
      // 
      this.activeJobsLabel.AutoSize = true;
      this.activeJobsLabel.Enabled = false;
      this.activeJobsLabel.Location = new System.Drawing.Point(12, 61);
      this.activeJobsLabel.Name = "activeJobsLabel";
      this.activeJobsLabel.Size = new System.Drawing.Size(84, 13);
      this.activeJobsLabel.TabIndex = 13;
      this.activeJobsLabel.Text = "&Max active jobs:";
      // 
      // ServerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(374, 342);
      this.Controls.Add(this.activeJobsLabel);
      this.Controls.Add(this.maxActiveJobsUpDown);
      this.Controls.Add(this.listBox);
      this.Controls.Add(this.connectButton);
      this.Controls.Add(this.gridAddressLabel);
      this.Controls.Add(this.address);
      this.Controls.Add(this.externalAddressLabel);
      this.Controls.Add(this.addressTextBox);
      this.Name = "ServerForm";
      this.Text = "CEDMA Server";
      ((System.ComponentModel.ISupportInitialize)(this.maxActiveJobsUpDown)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox addressTextBox;
    private System.Windows.Forms.Label externalAddressLabel;
    private System.Windows.Forms.Label gridAddressLabel;
    private System.Windows.Forms.TextBox address;
    private System.Windows.Forms.Button connectButton;
    private System.Windows.Forms.ListBox listBox;
    private System.Windows.Forms.Timer refreshTimer;
    private System.Windows.Forms.NumericUpDown maxActiveJobsUpDown;
    private System.Windows.Forms.Label activeJobsLabel;
  }
}
