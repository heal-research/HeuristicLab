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

namespace HeuristicLab.GP.StructureIdentification {
  partial class PredictorView {
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
      this.lowerPredictionLimit = new System.Windows.Forms.Label();
      this.lowerLimitTextbox = new System.Windows.Forms.TextBox();
      this.upperLimitTextbox = new System.Windows.Forms.TextBox();
      this.upperPredictionLimit = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // sizeLabel
      // 
      this.sizeLabel.Location = new System.Drawing.Point(3, 58);
      // 
      // sizeTextBox
      // 
      this.sizeTextBox.Location = new System.Drawing.Point(134, 55);
      // 
      // heightLabel
      // 
      this.heightLabel.Location = new System.Drawing.Point(3, 84);
      // 
      // heightTextBox
      // 
      this.heightTextBox.Location = new System.Drawing.Point(134, 81);
      // 
      // funTreeView
      // 
      this.funTreeView.Location = new System.Drawing.Point(0, 107);
      this.funTreeView.Size = new System.Drawing.Size(417, 304);
      // 
      // lowerPredictionLimit
      // 
      this.lowerPredictionLimit.AutoSize = true;
      this.lowerPredictionLimit.Location = new System.Drawing.Point(3, 6);
      this.lowerPredictionLimit.Name = "lowerPredictionLimit";
      this.lowerPredictionLimit.Size = new System.Drawing.Size(113, 13);
      this.lowerPredictionLimit.TabIndex = 13;
      this.lowerPredictionLimit.Text = "Lower Prediction Limit:";
      // 
      // lowerLimitTextbox
      // 
      this.lowerLimitTextbox.Location = new System.Drawing.Point(134, 3);
      this.lowerLimitTextbox.Name = "lowerLimitTextbox";
      this.lowerLimitTextbox.ReadOnly = true;
      this.lowerLimitTextbox.Size = new System.Drawing.Size(100, 20);
      this.lowerLimitTextbox.TabIndex = 14;
      // 
      // upperLimitTextbox
      // 
      this.upperLimitTextbox.Location = new System.Drawing.Point(134, 29);
      this.upperLimitTextbox.Name = "upperLimitTextbox";
      this.upperLimitTextbox.ReadOnly = true;
      this.upperLimitTextbox.Size = new System.Drawing.Size(100, 20);
      this.upperLimitTextbox.TabIndex = 16;
      // 
      // upperPredictionLimit
      // 
      this.upperPredictionLimit.AutoSize = true;
      this.upperPredictionLimit.Location = new System.Drawing.Point(3, 32);
      this.upperPredictionLimit.Name = "upperPredictionLimit";
      this.upperPredictionLimit.Size = new System.Drawing.Size(113, 13);
      this.upperPredictionLimit.TabIndex = 15;
      this.upperPredictionLimit.Text = "Upper Prediction Limit:";
      // 
      // PredictorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.upperLimitTextbox);
      this.Controls.Add(this.upperPredictionLimit);
      this.Controls.Add(this.lowerLimitTextbox);
      this.Controls.Add(this.lowerPredictionLimit);
      this.Name = "PredictorView";
      this.Size = new System.Drawing.Size(420, 414);
      this.Controls.SetChildIndex(this.sizeTextBox, 0);
      this.Controls.SetChildIndex(this.sizeLabel, 0);
      this.Controls.SetChildIndex(this.heightTextBox, 0);
      this.Controls.SetChildIndex(this.heightLabel, 0);
      this.Controls.SetChildIndex(this.funTreeView, 0);
      this.Controls.SetChildIndex(this.lowerPredictionLimit, 0);
      this.Controls.SetChildIndex(this.lowerLimitTextbox, 0);
      this.Controls.SetChildIndex(this.upperPredictionLimit, 0);
      this.Controls.SetChildIndex(this.upperLimitTextbox, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lowerPredictionLimit;
    private System.Windows.Forms.TextBox lowerLimitTextbox;
    private System.Windows.Forms.TextBox upperLimitTextbox;
    private System.Windows.Forms.Label upperPredictionLimit;

  }
}
