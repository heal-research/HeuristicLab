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
      this.externalAddressTextBox = new System.Windows.Forms.TextBox();
      this.startButton = new System.Windows.Forms.Button();
      this.stopButton = new System.Windows.Forms.Button();
      this.externalAddressLabel = new System.Windows.Forms.Label();
      this.internalAddressLabel = new System.Windows.Forms.Label();
      this.internalAddressTextBox = new System.Windows.Forms.TextBox();
      this.runningJobsLabel = new System.Windows.Forms.Label();
      this.runningJobsTextBox = new System.Windows.Forms.TextBox();
      this.waitingJobsLabel = new System.Windows.Forms.Label();
      this.waitingJobsTextBox = new System.Windows.Forms.TextBox();
      this.waitingResultsLabel = new System.Windows.Forms.Label();
      this.waitingResultsTextBox = new System.Windows.Forms.TextBox();
      this.statusUpdateTimer = new System.Windows.Forms.Timer(this.components);
      this.SuspendLayout();
      // 
      // externalAddressTextBox
      // 
      this.externalAddressTextBox.Location = new System.Drawing.Point(106, 6);
      this.externalAddressTextBox.Name = "externalAddressTextBox";
      this.externalAddressTextBox.ReadOnly = true;
      this.externalAddressTextBox.Size = new System.Drawing.Size(229, 20);
      this.externalAddressTextBox.TabIndex = 0;
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.startButton.Location = new System.Drawing.Point(12, 160);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(75, 23);
      this.startButton.TabIndex = 1;
      this.startButton.Text = "St&art";
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // stopButton
      // 
      this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stopButton.Enabled = false;
      this.stopButton.Location = new System.Drawing.Point(99, 160);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(75, 23);
      this.stopButton.TabIndex = 2;
      this.stopButton.Text = "St&op";
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // externalAddressLabel
      // 
      this.externalAddressLabel.AutoSize = true;
      this.externalAddressLabel.Location = new System.Drawing.Point(12, 9);
      this.externalAddressLabel.Name = "externalAddressLabel";
      this.externalAddressLabel.Size = new System.Drawing.Size(88, 13);
      this.externalAddressLabel.TabIndex = 3;
      this.externalAddressLabel.Text = "&External address:";
      // 
      // internalAddressLabel
      // 
      this.internalAddressLabel.AutoSize = true;
      this.internalAddressLabel.Location = new System.Drawing.Point(12, 34);
      this.internalAddressLabel.Name = "internalAddressLabel";
      this.internalAddressLabel.Size = new System.Drawing.Size(85, 13);
      this.internalAddressLabel.TabIndex = 5;
      this.internalAddressLabel.Text = "&Internal address:";
      // 
      // internalAddressTextBox
      // 
      this.internalAddressTextBox.Location = new System.Drawing.Point(106, 31);
      this.internalAddressTextBox.Name = "internalAddressTextBox";
      this.internalAddressTextBox.ReadOnly = true;
      this.internalAddressTextBox.Size = new System.Drawing.Size(229, 20);
      this.internalAddressTextBox.TabIndex = 4;
      // 
      // runningJobsLabel
      // 
      this.runningJobsLabel.AutoSize = true;
      this.runningJobsLabel.Location = new System.Drawing.Point(12, 99);
      this.runningJobsLabel.Name = "runningJobsLabel";
      this.runningJobsLabel.Size = new System.Drawing.Size(72, 13);
      this.runningJobsLabel.TabIndex = 9;
      this.runningJobsLabel.Text = "&Running jobs:";
      // 
      // runningJobsTextBox
      // 
      this.runningJobsTextBox.Location = new System.Drawing.Point(106, 96);
      this.runningJobsTextBox.Name = "runningJobsTextBox";
      this.runningJobsTextBox.ReadOnly = true;
      this.runningJobsTextBox.Size = new System.Drawing.Size(90, 20);
      this.runningJobsTextBox.TabIndex = 8;
      this.runningJobsTextBox.Text = "0";
      this.runningJobsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // waitingJobsLabel
      // 
      this.waitingJobsLabel.AutoSize = true;
      this.waitingJobsLabel.Location = new System.Drawing.Point(12, 74);
      this.waitingJobsLabel.Name = "waitingJobsLabel";
      this.waitingJobsLabel.Size = new System.Drawing.Size(68, 13);
      this.waitingJobsLabel.TabIndex = 7;
      this.waitingJobsLabel.Text = "&Waiting jobs:";
      // 
      // waitingJobsTextBox
      // 
      this.waitingJobsTextBox.Location = new System.Drawing.Point(106, 71);
      this.waitingJobsTextBox.Name = "waitingJobsTextBox";
      this.waitingJobsTextBox.ReadOnly = true;
      this.waitingJobsTextBox.Size = new System.Drawing.Size(90, 20);
      this.waitingJobsTextBox.TabIndex = 6;
      this.waitingJobsTextBox.Text = "0";
      this.waitingJobsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // waitingResultsLabel
      // 
      this.waitingResultsLabel.AutoSize = true;
      this.waitingResultsLabel.Location = new System.Drawing.Point(12, 125);
      this.waitingResultsLabel.Name = "waitingResultsLabel";
      this.waitingResultsLabel.Size = new System.Drawing.Size(79, 13);
      this.waitingResultsLabel.TabIndex = 11;
      this.waitingResultsLabel.Text = "&Waiting results:";
      // 
      // waitingResultsTextBox
      // 
      this.waitingResultsTextBox.Location = new System.Drawing.Point(106, 122);
      this.waitingResultsTextBox.Name = "waitingResultsTextBox";
      this.waitingResultsTextBox.ReadOnly = true;
      this.waitingResultsTextBox.Size = new System.Drawing.Size(90, 20);
      this.waitingResultsTextBox.TabIndex = 10;
      this.waitingResultsTextBox.Text = "0";
      this.waitingResultsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
      // 
      // statusUpdateTimer
      // 
      this.statusUpdateTimer.Enabled = true;
      this.statusUpdateTimer.Interval = 1000;
      this.statusUpdateTimer.Tick += new System.EventHandler(this.statusUpdateTimer_Tick);
      // 
      // ServerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(347, 195);
      this.Controls.Add(this.waitingResultsLabel);
      this.Controls.Add(this.waitingResultsTextBox);
      this.Controls.Add(this.runningJobsLabel);
      this.Controls.Add(this.runningJobsTextBox);
      this.Controls.Add(this.waitingJobsLabel);
      this.Controls.Add(this.waitingJobsTextBox);
      this.Controls.Add(this.internalAddressLabel);
      this.Controls.Add(this.internalAddressTextBox);
      this.Controls.Add(this.externalAddressLabel);
      this.Controls.Add(this.stopButton);
      this.Controls.Add(this.startButton);
      this.Controls.Add(this.externalAddressTextBox);
      this.Name = "ServerForm";
      this.Text = "Grid Server";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox externalAddressTextBox;
    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.Label externalAddressLabel;
    private System.Windows.Forms.Label internalAddressLabel;
    private System.Windows.Forms.TextBox internalAddressTextBox;
    private System.Windows.Forms.Label runningJobsLabel;
    private System.Windows.Forms.TextBox runningJobsTextBox;
    private System.Windows.Forms.Label waitingJobsLabel;
    private System.Windows.Forms.TextBox waitingJobsTextBox;
    private System.Windows.Forms.Label waitingResultsLabel;
    private System.Windows.Forms.TextBox waitingResultsTextBox;
    private System.Windows.Forms.Timer statusUpdateTimer;
  }
}
