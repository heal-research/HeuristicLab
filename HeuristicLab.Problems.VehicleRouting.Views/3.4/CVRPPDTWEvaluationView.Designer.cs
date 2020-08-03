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
  partial class CVRPPDTWEvaluationView {
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
      this.pickupViolationsLabel = new System.Windows.Forms.Label();
      this.pickupViolationsTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // tardinessLabel
      // 
      this.tardinessLabel.TabIndex = 15;
      // 
      // travelTimeLabel
      // 
      this.travelTimeLabel.TabIndex = 16;
      // 
      // overloadLabel
      // 
      this.overloadLabel.TabIndex = 14;
      // 
      // qualityLabel
      // 
      this.qualityLabel.TabIndex = 9;
      // 
      // distanceLabel
      // 
      this.distanceLabel.TabIndex = 10;
      // 
      // vehiclesLabel
      // 
      this.vehiclesLabel.TabIndex = 11;
      // 
      // penaltyLabel
      // 
      this.penaltyLabel.TabIndex = 12;
      // 
      // feasibleLabel
      // 
      this.feasibleLabel.TabIndex = 13;
      // 
      // pickupViolationsLabel
      // 
      this.pickupViolationsLabel.AutoSize = true;
      this.pickupViolationsLabel.Location = new System.Drawing.Point(3, 199);
      this.pickupViolationsLabel.Name = "pickupViolationsLabel";
      this.pickupViolationsLabel.Size = new System.Drawing.Size(55, 26);
      this.pickupViolationsLabel.TabIndex = 17;
      this.pickupViolationsLabel.Text = "Pickup\r\nViolations:";
      // 
      // pickupViolationsTextBox
      // 
      this.pickupViolationsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.pickupViolationsTextBox.Location = new System.Drawing.Point(71, 205);
      this.pickupViolationsTextBox.Name = "pickupViolationsTextBox";
      this.pickupViolationsTextBox.ReadOnly = true;
      this.pickupViolationsTextBox.Size = new System.Drawing.Size(149, 20);
      this.pickupViolationsTextBox.TabIndex = 8;
      // 
      // CVRPPDTWEvaluationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.pickupViolationsTextBox);
      this.Controls.Add(this.pickupViolationsLabel);
      this.Name = "CVRPPDTWEvaluationView";
      this.Size = new System.Drawing.Size(230, 236);
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
      this.Controls.SetChildIndex(this.pickupViolationsLabel, 0);
      this.Controls.SetChildIndex(this.pickupViolationsTextBox, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label pickupViolationsLabel;
    private System.Windows.Forms.TextBox pickupViolationsTextBox;
  }
}
