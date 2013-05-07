#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.Instances.TSPLIB.Views {
  partial class TSPLIBImportDialog {
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
      this.okButton = new System.Windows.Forms.Button();
      this.cancelButton = new System.Windows.Forms.Button();
      this.openTSPFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.tspFileLabel = new System.Windows.Forms.Label();
      this.openTSPFileButton = new System.Windows.Forms.Button();
      this.tspFileTextBox = new System.Windows.Forms.TextBox();
      this.tourFileLabel = new System.Windows.Forms.Label();
      this.openTourFileButton = new System.Windows.Forms.Button();
      this.tourFileTextBox = new System.Windows.Forms.TextBox();
      this.qualityLabel = new System.Windows.Forms.Label();
      this.qualityTextBox = new System.Windows.Forms.TextBox();
      this.openTourFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
      this.clearTourFileButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Enabled = false;
      this.okButton.Location = new System.Drawing.Point(418, 97);
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
      this.cancelButton.Location = new System.Drawing.Point(499, 97);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 10;
      this.cancelButton.Text = "&Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      // 
      // openTSPFileDialog
      // 
      this.openTSPFileDialog.DefaultExt = "tsp";
      this.openTSPFileDialog.FileName = "tsp";
      this.openTSPFileDialog.Filter = "TSP Files|*.tsp|All Files|*.*";
      this.openTSPFileDialog.Title = "Open TSP File";
      // 
      // tspFileLabel
      // 
      this.tspFileLabel.AutoSize = true;
      this.tspFileLabel.Location = new System.Drawing.Point(12, 15);
      this.tspFileLabel.Name = "tspFileLabel";
      this.tspFileLabel.Size = new System.Drawing.Size(50, 13);
      this.tspFileLabel.TabIndex = 0;
      this.tspFileLabel.Text = "&TSP File:";
      // 
      // openTSPFileButton
      // 
      this.openTSPFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.openTSPFileButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.openTSPFileButton.Location = new System.Drawing.Point(520, 10);
      this.openTSPFileButton.Name = "openTSPFileButton";
      this.openTSPFileButton.Size = new System.Drawing.Size(24, 24);
      this.openTSPFileButton.TabIndex = 2;
      this.openTSPFileButton.UseVisualStyleBackColor = true;
      this.openTSPFileButton.Click += new System.EventHandler(this.openTSPFileButton_Click);
      // 
      // tspFileTextBox
      // 
      this.tspFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tspFileTextBox.Enabled = false;
      this.tspFileTextBox.Location = new System.Drawing.Point(136, 12);
      this.tspFileTextBox.Name = "tspFileTextBox";
      this.tspFileTextBox.ReadOnly = true;
      this.tspFileTextBox.Size = new System.Drawing.Size(378, 20);
      this.tspFileTextBox.TabIndex = 1;
      // 
      // tourFileLabel
      // 
      this.tourFileLabel.AutoSize = true;
      this.tourFileLabel.Location = new System.Drawing.Point(12, 67);
      this.tourFileLabel.Name = "tourFileLabel";
      this.tourFileLabel.Size = new System.Drawing.Size(113, 13);
      this.tourFileLabel.TabIndex = 5;
      this.tourFileLabel.Text = "&Optimal TSP Tour File:";
      // 
      // openTourFileButton
      // 
      this.openTourFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.openTourFileButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Open;
      this.openTourFileButton.Location = new System.Drawing.Point(520, 62);
      this.openTourFileButton.Name = "openTourFileButton";
      this.openTourFileButton.Size = new System.Drawing.Size(24, 24);
      this.openTourFileButton.TabIndex = 7;
      this.openTourFileButton.UseVisualStyleBackColor = true;
      this.openTourFileButton.Click += new System.EventHandler(this.openTourFileButton_Click);
      // 
      // tourFileTextBox
      // 
      this.tourFileTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tourFileTextBox.Enabled = false;
      this.tourFileTextBox.Location = new System.Drawing.Point(136, 64);
      this.tourFileTextBox.Name = "tourFileTextBox";
      this.tourFileTextBox.ReadOnly = true;
      this.tourFileTextBox.Size = new System.Drawing.Size(378, 20);
      this.tourFileTextBox.TabIndex = 6;
      // 
      // qualityLabel
      // 
      this.qualityLabel.AutoSize = true;
      this.qualityLabel.Location = new System.Drawing.Point(12, 41);
      this.qualityLabel.Name = "qualityLabel";
      this.qualityLabel.Size = new System.Drawing.Size(102, 13);
      this.qualityLabel.TabIndex = 3;
      this.qualityLabel.Text = "&Best Known Quality:";
      // 
      // qualityTextBox
      // 
      this.qualityTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.qualityTextBox.Location = new System.Drawing.Point(136, 38);
      this.qualityTextBox.Name = "qualityTextBox";
      this.qualityTextBox.Size = new System.Drawing.Size(378, 20);
      this.qualityTextBox.TabIndex = 4;
      this.qualityTextBox.Validated += new System.EventHandler(this.qualityTextBox_Validated);
      this.qualityTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.qualityTextBox_KeyDown);
      this.qualityTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.qualityTextBox_Validating);
      // 
      // openTourFileDialog
      // 
      this.openTourFileDialog.DefaultExt = "opt.tour";
      this.openTourFileDialog.FileName = "tour";
      this.openTourFileDialog.Filter = "Optimal TSP Tour Files|*.opt.tour|All Files|*.*";
      this.openTourFileDialog.SupportMultiDottedExtensions = true;
      this.openTourFileDialog.Title = "Open Optimal TSP Tour File";
      // 
      // errorProvider
      // 
      this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
      this.errorProvider.ContainerControl = this;
      // 
      // clearTourFileButton
      // 
      this.clearTourFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.clearTourFileButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Remove;
      this.clearTourFileButton.Location = new System.Drawing.Point(550, 62);
      this.clearTourFileButton.Name = "clearTourFileButton";
      this.clearTourFileButton.Size = new System.Drawing.Size(24, 24);
      this.clearTourFileButton.TabIndex = 8;
      this.clearTourFileButton.UseVisualStyleBackColor = true;
      this.clearTourFileButton.Click += new System.EventHandler(this.clearTourFileButton_Click);
      // 
      // TSPLIBImportDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.cancelButton;
      this.ClientSize = new System.Drawing.Size(586, 132);
      this.Controls.Add(this.qualityTextBox);
      this.Controls.Add(this.tourFileTextBox);
      this.Controls.Add(this.tspFileTextBox);
      this.Controls.Add(this.clearTourFileButton);
      this.Controls.Add(this.openTourFileButton);
      this.Controls.Add(this.openTSPFileButton);
      this.Controls.Add(this.qualityLabel);
      this.Controls.Add(this.tourFileLabel);
      this.Controls.Add(this.tspFileLabel);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.okButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "TSPLIBImportDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Import TSP from TSPLIB";
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.OpenFileDialog openTSPFileDialog;
    private System.Windows.Forms.Label tspFileLabel;
    private System.Windows.Forms.Button openTSPFileButton;
    private System.Windows.Forms.TextBox tspFileTextBox;
    private System.Windows.Forms.Label tourFileLabel;
    private System.Windows.Forms.Button openTourFileButton;
    private System.Windows.Forms.TextBox tourFileTextBox;
    private System.Windows.Forms.Label qualityLabel;
    private System.Windows.Forms.TextBox qualityTextBox;
    private System.Windows.Forms.OpenFileDialog openTourFileDialog;
    private System.Windows.Forms.ErrorProvider errorProvider;
    private System.Windows.Forms.Button clearTourFileButton;
  }
}