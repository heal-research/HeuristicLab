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
  partial class ServerView {
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
      this.gridAddressLabel = new System.Windows.Forms.Label();
      this.address = new System.Windows.Forms.TextBox();
      this.connectButton = new System.Windows.Forms.Button();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.executerTabPage = new System.Windows.Forms.TabPage();
      this.dispatcherTabPage = new System.Windows.Forms.TabPage();
      this.problemPage = new System.Windows.Forms.TabPage();
      this.tabControl.SuspendLayout();
      this.SuspendLayout();
      // 
      // gridAddressLabel
      // 
      this.gridAddressLabel.AutoSize = true;
      this.gridAddressLabel.Location = new System.Drawing.Point(2, 8);
      this.gridAddressLabel.Name = "gridAddressLabel";
      this.gridAddressLabel.Size = new System.Drawing.Size(69, 13);
      this.gridAddressLabel.TabIndex = 9;
      this.gridAddressLabel.Text = "&Grid address:";
      // 
      // address
      // 
      this.address.Location = new System.Drawing.Point(96, 5);
      this.address.Name = "address";
      this.address.Size = new System.Drawing.Size(160, 20);
      this.address.TabIndex = 8;
      // 
      // connectButton
      // 
      this.connectButton.Location = new System.Drawing.Point(262, 3);
      this.connectButton.Name = "connectButton";
      this.connectButton.Size = new System.Drawing.Size(75, 23);
      this.connectButton.TabIndex = 10;
      this.connectButton.Text = "Connect";
      this.connectButton.UseVisualStyleBackColor = true;
      this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.executerTabPage);
      this.tabControl.Controls.Add(this.dispatcherTabPage);
      this.tabControl.Controls.Add(this.problemPage);
      this.tabControl.Location = new System.Drawing.Point(3, 31);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(573, 570);
      this.tabControl.TabIndex = 14;
      // 
      // executerTabPage
      // 
      this.executerTabPage.Location = new System.Drawing.Point(4, 22);
      this.executerTabPage.Name = "executerTabPage";
      this.executerTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.executerTabPage.Size = new System.Drawing.Size(565, 544);
      this.executerTabPage.TabIndex = 0;
      this.executerTabPage.Text = "Executer";
      this.executerTabPage.UseVisualStyleBackColor = true;
      // 
      // dispatcherTabPage
      // 
      this.dispatcherTabPage.Location = new System.Drawing.Point(4, 22);
      this.dispatcherTabPage.Name = "dispatcherTabPage";
      this.dispatcherTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.dispatcherTabPage.Size = new System.Drawing.Size(565, 517);
      this.dispatcherTabPage.TabIndex = 1;
      this.dispatcherTabPage.Text = "Dispatcher";
      this.dispatcherTabPage.UseVisualStyleBackColor = true;
      // 
      // problemPage
      // 
      this.problemPage.Location = new System.Drawing.Point(4, 22);
      this.problemPage.Name = "problemPage";
      this.problemPage.Padding = new System.Windows.Forms.Padding(3);
      this.problemPage.Size = new System.Drawing.Size(565, 517);
      this.problemPage.TabIndex = 2;
      this.problemPage.Text = "Problem";
      this.problemPage.UseVisualStyleBackColor = true;
      // 
      // ServerView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.connectButton);
      this.Controls.Add(this.gridAddressLabel);
      this.Controls.Add(this.address);
      this.Name = "ServerView";
      this.Size = new System.Drawing.Size(579, 604);
      this.tabControl.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label gridAddressLabel;
    private System.Windows.Forms.TextBox address;
    private System.Windows.Forms.Button connectButton;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage executerTabPage;
    private System.Windows.Forms.TabPage dispatcherTabPage;
    private System.Windows.Forms.TabPage problemPage;
  }
}
