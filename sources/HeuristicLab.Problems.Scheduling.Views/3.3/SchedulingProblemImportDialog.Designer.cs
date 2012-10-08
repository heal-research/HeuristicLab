#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Scheduling.Views {
  partial class SchedulingProblemImportDialog {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchedulingProblemImportDialog));
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.openSchedulingProblemFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.spFileLabel = new System.Windows.Forms.Label();
      this.openSchedulingProblemFileButton = new System.Windows.Forms.Button();
      this.spFileTextBox = new System.Windows.Forms.TextBox();
      this.optimalScheduleFileLabel = new System.Windows.Forms.Label();
      this.openOptimalScheduleFileButton = new System.Windows.Forms.Button();
      this.optimalScheduleFileTextBox = new System.Windows.Forms.TextBox();
      this.openOptimalScheduleFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.clearOptimalScheduleButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Enabled = false;
      this.okButton.Location = new System.Drawing.Point(422, 69);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 9;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      // 
      // cancelButton
      // 
      this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancelButton.Location = new System.Drawing.Point(499, 69);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 10;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // openSchedulingProblemFileDialog
      // 
      this.openSchedulingProblemFileDialog.DefaultExt = "scp";
      this.openSchedulingProblemFileDialog.FileName = "scp";
      this.openSchedulingProblemFileDialog.Filter = "ORLib Format|*.txt";
      this.openSchedulingProblemFileDialog.Title = "Open Scheduling Problem File";
      // 
      // spFileLabel
      // 
      this.spFileLabel.AutoSize = true;
      this.spFileLabel.Location = new System.Drawing.Point(12, 15);
      this.spFileLabel.Name = "spFileLabel";
      this.spFileLabel.Size = new System.Drawing.Size(43, 13);
      this.spFileLabel.TabIndex = 0;
      this.spFileLabel.Text = "&SP File:";
      // 
      // openSchedulingProblemFileButton
      // 
      this.openSchedulingProblemFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.openSchedulingProblemFileButton.Image = ((System.Drawing.Image)(resources.GetObject("openSchedulingProblemFileButton.Image")));
      this.openSchedulingProblemFileButton.Location = new System.Drawing.Point(520, 10);
      this.openSchedulingProblemFileButton.Name = "openSchedulingProblemFileButton";
      this.openSchedulingProblemFileButton.Size = new System.Drawing.Size(24, 24);
      this.openSchedulingProblemFileButton.TabIndex = 2;
      this.openSchedulingProblemFileButton.UseVisualStyleBackColor = true;
      this.openSchedulingProblemFileButton.Click += new System.EventHandler(this.openSchedulingProblemFileButton_Click);
      // 
      // spFileTextBox
      // 
      this.spFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.spFileTextBox.Enabled = false;
      this.spFileTextBox.Location = new System.Drawing.Point(136, 12);
      this.spFileTextBox.Name = "spFileTextBox";
      this.spFileTextBox.ReadOnly = true;
      this.spFileTextBox.Size = new System.Drawing.Size(378, 20);
      this.spFileTextBox.TabIndex = 1;
      // 
      // optimalScheduleFileLabel
      // 
      this.optimalScheduleFileLabel.AutoSize = true;
      this.optimalScheduleFileLabel.Location = new System.Drawing.Point(12, 41);
      this.optimalScheduleFileLabel.Name = "optimalScheduleFileLabel";
      this.optimalScheduleFileLabel.Size = new System.Drawing.Size(112, 13);
      this.optimalScheduleFileLabel.TabIndex = 5;
      this.optimalScheduleFileLabel.Text = "&Optimal Schedule File:";
      // 
      // openOptimalScheduleFileButton
      // 
      this.openOptimalScheduleFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.openOptimalScheduleFileButton.Image = ((System.Drawing.Image)(resources.GetObject("openOptimalScheduleFileButton.Image")));
      this.openOptimalScheduleFileButton.Location = new System.Drawing.Point(520, 35);
      this.openOptimalScheduleFileButton.Name = "openOptimalScheduleFileButton";
      this.openOptimalScheduleFileButton.Size = new System.Drawing.Size(24, 24);
      this.openOptimalScheduleFileButton.TabIndex = 7;
      this.openOptimalScheduleFileButton.UseVisualStyleBackColor = true;
      this.openOptimalScheduleFileButton.Click += new System.EventHandler(this.openOptimalScheduleFileButton_Click);
      // 
      // optimalScheduleFileTextBox
      // 
      this.optimalScheduleFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.optimalScheduleFileTextBox.Enabled = false;
      this.optimalScheduleFileTextBox.Location = new System.Drawing.Point(136, 38);
      this.optimalScheduleFileTextBox.Name = "optimalScheduleFileTextBox";
      this.optimalScheduleFileTextBox.ReadOnly = true;
      this.optimalScheduleFileTextBox.Size = new System.Drawing.Size(378, 20);
      this.optimalScheduleFileTextBox.TabIndex = 6;
      // 
      // openOptimalScheduleFileDialog
      // 
      this.openOptimalScheduleFileDialog.DefaultExt = "opt";
      this.openOptimalScheduleFileDialog.FileName = "tour";
      this.openOptimalScheduleFileDialog.Filter = "Optimal Schedule Files|*.opt";
      this.openOptimalScheduleFileDialog.SupportMultiDottedExtensions = true;
      this.openOptimalScheduleFileDialog.Title = "Open Optimal Schedule File";
      // 
      // clearOptimalScheduleButton
      // 
      this.clearOptimalScheduleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.clearOptimalScheduleButton.Image = ((System.Drawing.Image)(resources.GetObject("clearOptimalScheduleButton.Image")));
      this.clearOptimalScheduleButton.Location = new System.Drawing.Point(550, 35);
      this.clearOptimalScheduleButton.Name = "clearOptimalScheduleButton";
      this.clearOptimalScheduleButton.Size = new System.Drawing.Size(24, 24);
      this.clearOptimalScheduleButton.TabIndex = 8;
      this.clearOptimalScheduleButton.UseVisualStyleBackColor = true;
      this.clearOptimalScheduleButton.Click += new System.EventHandler(this.clearTourFileButton_Click);
      // 
      // SchedulingProblemImportDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(586, 104);
      this.Controls.Add(this.optimalScheduleFileTextBox);
      this.Controls.Add(this.spFileTextBox);
      this.Controls.Add(this.clearOptimalScheduleButton);
      this.Controls.Add(this.openOptimalScheduleFileButton);
      this.Controls.Add(this.openSchedulingProblemFileButton);
      this.Controls.Add(this.optimalScheduleFileLabel);
      this.Controls.Add(this.spFileLabel);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "SchedulingProblemImportDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Import Scheduling Problem";
      this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.VRPImportDialog_HelpButtonClicked);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.OpenFileDialog openSchedulingProblemFileDialog;
    private System.Windows.Forms.Label spFileLabel;
    private System.Windows.Forms.Button openSchedulingProblemFileButton;
    private System.Windows.Forms.TextBox spFileTextBox;
    private System.Windows.Forms.Label optimalScheduleFileLabel;
    private System.Windows.Forms.Button openOptimalScheduleFileButton;
    private System.Windows.Forms.TextBox optimalScheduleFileTextBox;
    private System.Windows.Forms.OpenFileDialog openOptimalScheduleFileDialog;
    private System.Windows.Forms.Button clearOptimalScheduleButton;
  }
}