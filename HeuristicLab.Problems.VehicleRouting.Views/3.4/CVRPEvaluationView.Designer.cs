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

namespace HeuristicLab.Problems.VehicleRouting.Views {
  partial class CVRPEvaluationView {
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
      this.overloadLabel = new System.Windows.Forms.Label();
      this.overloadTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // qualityLabel
      // 
      this.qualityLabel.TabIndex = 6;
      // 
      // distanceLabel
      // 
      this.distanceLabel.TabIndex = 7;
      // 
      // vehiclesLabel
      // 
      this.vehiclesLabel.TabIndex = 8;
      // 
      // penaltyLabel
      // 
      this.penaltyLabel.TabIndex = 9;
      // 
      // feasibleLabel
      // 
      this.feasibleLabel.TabIndex = 10;
      // 
      // overloadLabel
      // 
      this.overloadLabel.AutoSize = true;
      this.overloadLabel.Location = new System.Drawing.Point(3, 130);
      this.overloadLabel.Name = "overloadLabel";
      this.overloadLabel.Size = new System.Drawing.Size(53, 13);
      this.overloadLabel.TabIndex = 11;
      this.overloadLabel.Text = "Overload:";
      // 
      // overloadTextBox
      // 
      this.overloadTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.overloadTextBox.Location = new System.Drawing.Point(61, 127);
      this.overloadTextBox.Name = "overloadTextBox";
      this.overloadTextBox.ReadOnly = true;
      this.overloadTextBox.Size = new System.Drawing.Size(159, 20);
      this.overloadTextBox.TabIndex = 5;
      // 
      // CVRPEvaluationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.overloadTextBox);
      this.Controls.Add(this.overloadLabel);
      this.Name = "CVRPEvaluationView";
      this.Size = new System.Drawing.Size(230, 151);
      this.Controls.SetChildIndex(this.qualityLabel, 0);
      this.Controls.SetChildIndex(this.distanceLabel, 0);
      this.Controls.SetChildIndex(this.vehiclesLabel, 0);
      this.Controls.SetChildIndex(this.penaltyLabel, 0);
      this.Controls.SetChildIndex(this.feasibleLabel, 0);
      this.Controls.SetChildIndex(this.qualityTextBox, 0);
      this.Controls.SetChildIndex(this.vehiclesTextBox, 0);
      this.Controls.SetChildIndex(this.distanceTextBox, 0);
      this.Controls.SetChildIndex(this.penaltyTextBox, 0);
      this.Controls.SetChildIndex(this.isFeasibleCcheckBox, 0);
      this.Controls.SetChildIndex(this.overloadLabel, 0);
      this.Controls.SetChildIndex(this.overloadTextBox, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label overloadLabel;
    protected System.Windows.Forms.TextBox overloadTextBox;
  }
}
