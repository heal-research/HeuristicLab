#region License Information

/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class FeatureCorrelationView {
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
      this.ignoreMissingValuesCheckbox = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.progressPanel.SuspendLayout();
      this.SuspendLayout();
      // 
      // minimumLabel
      // 
      this.minimumLabel.Location = new System.Drawing.Point(707, 434);
      // 
      // maximumLabel
      // 
      this.maximumLabel.Location = new System.Drawing.Point(707, 2);
      // 
      // pictureBox
      // 
      this.pictureBox.Location = new System.Drawing.Point(727, 30);
      this.pictureBox.Size = new System.Drawing.Size(35, 401);
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.ignoreMissingValuesCheckbox);
      this.splitContainer.Size = new System.Drawing.Size(695, 450);
      // 
      // dataView
      // 
      this.dataView.Size = new System.Drawing.Size(695, 421);
      // 
      // ignoreMissingValuesCheckbox
      // 
      this.ignoreMissingValuesCheckbox.AutoSize = true;
      this.ignoreMissingValuesCheckbox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.ignoreMissingValuesCheckbox.Location = new System.Drawing.Point(481, 6);
      this.ignoreMissingValuesCheckbox.Name = "ignoreMissingValuesCheckbox";
      this.ignoreMissingValuesCheckbox.Size = new System.Drawing.Size(122, 17);
      this.ignoreMissingValuesCheckbox.TabIndex = 11;
      this.ignoreMissingValuesCheckbox.Text = "Ignore missing values";
      this.ignoreMissingValuesCheckbox.UseVisualStyleBackColor = true;
      this.ignoreMissingValuesCheckbox.CheckedChanged += new System.EventHandler(this.ignoreMissingValuesCheckbox_CheckedChanged);
      // 
      // FeatureCorrelationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "FeatureCorrelationView";
      this.Size = new System.Drawing.Size(789, 456);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.progressPanel.ResumeLayout(false);
      this.progressPanel.PerformLayout();
      this.ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.CheckBox ignoreMissingValuesCheckbox;
  }
}
