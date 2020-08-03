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
  partial class CVRPTWEvaluationView {
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
      this.tardinessLabel = new System.Windows.Forms.Label();
      this.tardinessTextBox = new System.Windows.Forms.TextBox();
      this.travelTimeLabel = new System.Windows.Forms.Label();
      this.travelTimeTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // overloadLabel
      // 
      this.overloadLabel.TabIndex = 13;
      // 
      // overloadTextBox
      // 
      this.overloadTextBox.Location = new System.Drawing.Point(71, 127);
      this.overloadTextBox.Size = new System.Drawing.Size(149, 20);
      // 
      // qualityLabel
      // 
      this.qualityLabel.TabIndex = 8;
      // 
      // qualityTextBox
      // 
      this.qualityTextBox.Location = new System.Drawing.Point(71, 3);
      this.qualityTextBox.Size = new System.Drawing.Size(149, 20);
      // 
      // vehiclesTextBox
      // 
      this.vehiclesTextBox.Location = new System.Drawing.Point(71, 55);
      this.vehiclesTextBox.Size = new System.Drawing.Size(149, 20);
      // 
      // distanceLabel
      // 
      this.distanceLabel.TabIndex = 9;
      // 
      // vehiclesLabel
      // 
      this.vehiclesLabel.TabIndex = 10;
      // 
      // penaltyLabel
      // 
      this.penaltyLabel.TabIndex = 11;
      // 
      // feasibleLabel
      // 
      this.feasibleLabel.TabIndex = 12;
      // 
      // distanceTextBox
      // 
      this.distanceTextBox.Location = new System.Drawing.Point(71, 29);
      this.distanceTextBox.Size = new System.Drawing.Size(149, 20);
      // 
      // penaltyTextBox
      // 
      this.penaltyTextBox.Location = new System.Drawing.Point(71, 81);
      this.penaltyTextBox.Size = new System.Drawing.Size(149, 20);
      // 
      // isFeasibleCcheckBox
      // 
      this.isFeasibleCcheckBox.Location = new System.Drawing.Point(71, 107);
      // 
      // tardinessLabel
      // 
      this.tardinessLabel.AutoSize = true;
      this.tardinessLabel.Location = new System.Drawing.Point(3, 156);
      this.tardinessLabel.Name = "tardinessLabel";
      this.tardinessLabel.Size = new System.Drawing.Size(56, 13);
      this.tardinessLabel.TabIndex = 14;
      this.tardinessLabel.Text = "Tardiness:";
      // 
      // tardinessTextBox
      // 
      this.tardinessTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tardinessTextBox.Location = new System.Drawing.Point(71, 153);
      this.tardinessTextBox.Name = "tardinessTextBox";
      this.tardinessTextBox.ReadOnly = true;
      this.tardinessTextBox.Size = new System.Drawing.Size(149, 20);
      this.tardinessTextBox.TabIndex = 6;
      // 
      // travelTimeLabel
      // 
      this.travelTimeLabel.AutoSize = true;
      this.travelTimeLabel.Location = new System.Drawing.Point(3, 182);
      this.travelTimeLabel.Name = "travelTimeLabel";
      this.travelTimeLabel.Size = new System.Drawing.Size(62, 13);
      this.travelTimeLabel.TabIndex = 15;
      this.travelTimeLabel.Text = "Travel time:";
      // 
      // travelTimeTextBox
      // 
      this.travelTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.travelTimeTextBox.Location = new System.Drawing.Point(71, 179);
      this.travelTimeTextBox.Name = "travelTimeTextBox";
      this.travelTimeTextBox.ReadOnly = true;
      this.travelTimeTextBox.Size = new System.Drawing.Size(149, 20);
      this.travelTimeTextBox.TabIndex = 7;
      // 
      // CVRPTWEvaluationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.travelTimeTextBox);
      this.Controls.Add(this.travelTimeLabel);
      this.Controls.Add(this.tardinessTextBox);
      this.Controls.Add(this.tardinessLabel);
      this.Name = "CVRPTWEvaluationView";
      this.Size = new System.Drawing.Size(230, 211);
      this.Controls.SetChildIndex(this.overloadLabel, 0);
      this.Controls.SetChildIndex(this.overloadTextBox, 0);
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
      this.Controls.SetChildIndex(this.tardinessLabel, 0);
      this.Controls.SetChildIndex(this.tardinessTextBox, 0);
      this.Controls.SetChildIndex(this.travelTimeLabel, 0);
      this.Controls.SetChildIndex(this.travelTimeTextBox, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label tardinessLabel;
    protected System.Windows.Forms.TextBox tardinessTextBox;
    protected System.Windows.Forms.Label travelTimeLabel;
    protected System.Windows.Forms.TextBox travelTimeTextBox;
  }
}
