#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Analysis.Views {
  partial class AlleleFrequencyView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.idLabel = new System.Windows.Forms.Label();
      this.idTextBox = new System.Windows.Forms.TextBox();
      this.frequencyTextBox = new System.Windows.Forms.TextBox();
      this.averageImpactTextBox = new System.Windows.Forms.TextBox();
      this.averageSolutionQualityTextBox = new System.Windows.Forms.TextBox();
      this.frequencyLabel = new System.Windows.Forms.Label();
      this.averageImpactLabel = new System.Windows.Forms.Label();
      this.averageSolutionQualityLabel = new System.Windows.Forms.Label();
      this.containedInBestKnownSolutionLabel = new System.Windows.Forms.Label();
      this.containedInBestSolutionLabel = new System.Windows.Forms.Label();
      this.containedInBestKnownSolutionCheckBox = new System.Windows.Forms.CheckBox();
      this.containedInBestSolutionCheckBox = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // idLabel
      // 
      this.idLabel.AutoSize = true;
      this.idLabel.Location = new System.Drawing.Point(3, 3);
      this.idLabel.Name = "idLabel";
      this.idLabel.Size = new System.Drawing.Size(19, 13);
      this.idLabel.TabIndex = 0;
      this.idLabel.Text = "&Id:";
      // 
      // idTextBox
      // 
      this.idTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.idTextBox.Location = new System.Drawing.Point(179, 0);
      this.idTextBox.Name = "idTextBox";
      this.idTextBox.ReadOnly = true;
      this.idTextBox.Size = new System.Drawing.Size(188, 20);
      this.idTextBox.TabIndex = 1;
      // 
      // frequencyTextBox
      // 
      this.frequencyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.frequencyTextBox.Location = new System.Drawing.Point(179, 26);
      this.frequencyTextBox.Name = "frequencyTextBox";
      this.frequencyTextBox.ReadOnly = true;
      this.frequencyTextBox.Size = new System.Drawing.Size(188, 20);
      this.frequencyTextBox.TabIndex = 3;
      // 
      // averageImpactTextBox
      // 
      this.averageImpactTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.averageImpactTextBox.Location = new System.Drawing.Point(179, 52);
      this.averageImpactTextBox.Name = "averageImpactTextBox";
      this.averageImpactTextBox.ReadOnly = true;
      this.averageImpactTextBox.Size = new System.Drawing.Size(188, 20);
      this.averageImpactTextBox.TabIndex = 5;
      // 
      // averageSolutionQualityTextBox
      // 
      this.averageSolutionQualityTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.averageSolutionQualityTextBox.Location = new System.Drawing.Point(179, 78);
      this.averageSolutionQualityTextBox.Name = "averageSolutionQualityTextBox";
      this.averageSolutionQualityTextBox.ReadOnly = true;
      this.averageSolutionQualityTextBox.Size = new System.Drawing.Size(188, 20);
      this.averageSolutionQualityTextBox.TabIndex = 7;
      // 
      // frequencyLabel
      // 
      this.frequencyLabel.AutoSize = true;
      this.frequencyLabel.Location = new System.Drawing.Point(3, 29);
      this.frequencyLabel.Name = "frequencyLabel";
      this.frequencyLabel.Size = new System.Drawing.Size(60, 13);
      this.frequencyLabel.TabIndex = 2;
      this.frequencyLabel.Text = "&Frequency:";
      // 
      // averageImpactLabel
      // 
      this.averageImpactLabel.AutoSize = true;
      this.averageImpactLabel.Location = new System.Drawing.Point(3, 55);
      this.averageImpactLabel.Name = "averageImpactLabel";
      this.averageImpactLabel.Size = new System.Drawing.Size(85, 13);
      this.averageImpactLabel.TabIndex = 4;
      this.averageImpactLabel.Text = "Average &Impact:";
      // 
      // averageSolutionQualityLabel
      // 
      this.averageSolutionQualityLabel.AutoSize = true;
      this.averageSolutionQualityLabel.Location = new System.Drawing.Point(3, 81);
      this.averageSolutionQualityLabel.Name = "averageSolutionQualityLabel";
      this.averageSolutionQualityLabel.Size = new System.Drawing.Size(126, 13);
      this.averageSolutionQualityLabel.TabIndex = 6;
      this.averageSolutionQualityLabel.Text = "Average Solution &Quality:";
      // 
      // containedInBestKnownSolutionLabel
      // 
      this.containedInBestKnownSolutionLabel.AutoSize = true;
      this.containedInBestKnownSolutionLabel.Location = new System.Drawing.Point(3, 104);
      this.containedInBestKnownSolutionLabel.Name = "containedInBestKnownSolutionLabel";
      this.containedInBestKnownSolutionLabel.Size = new System.Drawing.Size(170, 13);
      this.containedInBestKnownSolutionLabel.TabIndex = 8;
      this.containedInBestKnownSolutionLabel.Text = "Contained in Best &Known Solution:";
      // 
      // containedInBestSolutionLabel
      // 
      this.containedInBestSolutionLabel.AutoSize = true;
      this.containedInBestSolutionLabel.Location = new System.Drawing.Point(3, 124);
      this.containedInBestSolutionLabel.Name = "containedInBestSolutionLabel";
      this.containedInBestSolutionLabel.Size = new System.Drawing.Size(134, 13);
      this.containedInBestSolutionLabel.TabIndex = 10;
      this.containedInBestSolutionLabel.Text = "Contained in &Best Solution:";
      // 
      // containedInBestKnownSolutionCheckBox
      // 
      this.containedInBestKnownSolutionCheckBox.AutoSize = true;
      this.containedInBestKnownSolutionCheckBox.Enabled = false;
      this.containedInBestKnownSolutionCheckBox.Location = new System.Drawing.Point(179, 104);
      this.containedInBestKnownSolutionCheckBox.Name = "containedInBestKnownSolutionCheckBox";
      this.containedInBestKnownSolutionCheckBox.Size = new System.Drawing.Size(15, 14);
      this.containedInBestKnownSolutionCheckBox.TabIndex = 9;
      this.containedInBestKnownSolutionCheckBox.UseVisualStyleBackColor = true;
      // 
      // containedInBestSolutionCheckBox
      // 
      this.containedInBestSolutionCheckBox.AutoSize = true;
      this.containedInBestSolutionCheckBox.Enabled = false;
      this.containedInBestSolutionCheckBox.Location = new System.Drawing.Point(179, 124);
      this.containedInBestSolutionCheckBox.Name = "containedInBestSolutionCheckBox";
      this.containedInBestSolutionCheckBox.Size = new System.Drawing.Size(15, 14);
      this.containedInBestSolutionCheckBox.TabIndex = 11;
      this.containedInBestSolutionCheckBox.UseVisualStyleBackColor = true;
      // 
      // AlleleFrequencyView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.containedInBestSolutionCheckBox);
      this.Controls.Add(this.containedInBestKnownSolutionCheckBox);
      this.Controls.Add(this.averageSolutionQualityTextBox);
      this.Controls.Add(this.averageImpactTextBox);
      this.Controls.Add(this.frequencyTextBox);
      this.Controls.Add(this.idTextBox);
      this.Controls.Add(this.containedInBestSolutionLabel);
      this.Controls.Add(this.containedInBestKnownSolutionLabel);
      this.Controls.Add(this.averageSolutionQualityLabel);
      this.Controls.Add(this.averageImpactLabel);
      this.Controls.Add(this.frequencyLabel);
      this.Controls.Add(this.idLabel);
      this.Name = "AlleleFrequencyView";
      this.Size = new System.Drawing.Size(367, 151);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.Label idLabel;
    protected System.Windows.Forms.TextBox idTextBox;
    protected System.Windows.Forms.TextBox frequencyTextBox;
    protected System.Windows.Forms.TextBox averageImpactTextBox;
    protected System.Windows.Forms.TextBox averageSolutionQualityTextBox;
    protected System.Windows.Forms.Label frequencyLabel;
    protected System.Windows.Forms.Label averageImpactLabel;
    protected System.Windows.Forms.Label averageSolutionQualityLabel;
    protected System.Windows.Forms.Label containedInBestKnownSolutionLabel;
    protected System.Windows.Forms.Label containedInBestSolutionLabel;
    protected System.Windows.Forms.CheckBox containedInBestKnownSolutionCheckBox;
    protected System.Windows.Forms.CheckBox containedInBestSolutionCheckBox;
  }
}
