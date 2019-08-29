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

namespace HeuristicLab.Clients.OKB.RunCreation.Views {
  partial class SingleObjectiveOKBSolutionView {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.refreshButton = new System.Windows.Forms.Button();
      this.uploadButton = new System.Windows.Forms.Button();
      this.solutionGroupBox = new System.Windows.Forms.GroupBox();
      this.qualityTextBox = new System.Windows.Forms.TextBox();
      this.qualityLabel = new System.Windows.Forms.Label();
      this.solutionViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.solutionGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // refreshButton
      // 
      this.refreshButton.Location = new System.Drawing.Point(3, 3);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(26, 23);
      this.refreshButton.TabIndex = 0;
      this.refreshButton.Text = "Refresh";
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // uploadButton
      // 
      this.uploadButton.Location = new System.Drawing.Point(35, 3);
      this.uploadButton.Name = "uploadButton";
      this.uploadButton.Size = new System.Drawing.Size(26, 23);
      this.uploadButton.TabIndex = 0;
      this.uploadButton.Text = "Upload";
      this.uploadButton.UseVisualStyleBackColor = true;
      this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
      // 
      // solutionGroupBox
      // 
      this.solutionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.solutionGroupBox.Controls.Add(this.solutionViewHost);
      this.solutionGroupBox.Controls.Add(this.qualityTextBox);
      this.solutionGroupBox.Controls.Add(this.qualityLabel);
      this.solutionGroupBox.Location = new System.Drawing.Point(3, 32);
      this.solutionGroupBox.Name = "solutionGroupBox";
      this.solutionGroupBox.Size = new System.Drawing.Size(703, 452);
      this.solutionGroupBox.TabIndex = 3;
      this.solutionGroupBox.TabStop = false;
      this.solutionGroupBox.Text = "Solution";
      // 
      // qualityTextBox
      // 
      this.qualityTextBox.Location = new System.Drawing.Point(58, 19);
      this.qualityTextBox.Name = "qualityTextBox";
      this.qualityTextBox.ReadOnly = true;
      this.qualityTextBox.Size = new System.Drawing.Size(229, 20);
      this.qualityTextBox.TabIndex = 4;
      // 
      // qualityLabel
      // 
      this.qualityLabel.AutoSize = true;
      this.qualityLabel.Location = new System.Drawing.Point(10, 22);
      this.qualityLabel.Name = "qualityLabel";
      this.qualityLabel.Size = new System.Drawing.Size(42, 13);
      this.qualityLabel.TabIndex = 3;
      this.qualityLabel.Text = "Quality:";
      // 
      // solutionViewHost
      // 
      this.solutionViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.solutionViewHost.Caption = "View";
      this.solutionViewHost.Content = null;
      this.solutionViewHost.Enabled = false;
      this.solutionViewHost.Location = new System.Drawing.Point(6, 45);
      this.solutionViewHost.Name = "solutionViewHost";
      this.solutionViewHost.ReadOnly = false;
      this.solutionViewHost.Size = new System.Drawing.Size(691, 401);
      this.solutionViewHost.TabIndex = 5;
      this.solutionViewHost.ViewsLabelVisible = true;
      this.solutionViewHost.ViewType = null;
      // 
      // SingleObjectiveOKBSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.solutionGroupBox);
      this.Controls.Add(this.uploadButton);
      this.Controls.Add(this.refreshButton);
      this.Name = "SingleObjectiveOKBSolutionView";
      this.Size = new System.Drawing.Size(709, 487);
      this.solutionGroupBox.ResumeLayout(false);
      this.solutionGroupBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Button uploadButton;
    private System.Windows.Forms.GroupBox solutionGroupBox;
    private MainForm.WindowsForms.ViewHost solutionViewHost;
    private System.Windows.Forms.TextBox qualityTextBox;
    private System.Windows.Forms.Label qualityLabel;
  }
}
