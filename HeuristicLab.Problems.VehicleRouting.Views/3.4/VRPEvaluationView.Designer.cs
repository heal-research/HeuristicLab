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
  partial class VRPEvaluationView {
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
      this.qualityLabel = new System.Windows.Forms.Label();
      this.qualityTextBox = new System.Windows.Forms.TextBox();
      this.vehiclesTextBox = new System.Windows.Forms.TextBox();
      this.distanceLabel = new System.Windows.Forms.Label();
      this.vehiclesLabel = new System.Windows.Forms.Label();
      this.penaltyLabel = new System.Windows.Forms.Label();
      this.feasibleLabel = new System.Windows.Forms.Label();
      this.distanceTextBox = new System.Windows.Forms.TextBox();
      this.penaltyTextBox = new System.Windows.Forms.TextBox();
      this.isFeasibleCcheckBox = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // qualityLabel
      // 
      this.qualityLabel.AutoSize = true;
      this.qualityLabel.Location = new System.Drawing.Point(3, 6);
      this.qualityLabel.Name = "qualityLabel";
      this.qualityLabel.Size = new System.Drawing.Size(42, 13);
      this.qualityLabel.TabIndex = 5;
      this.qualityLabel.Text = "Quality:";
      // 
      // qualityTextBox
      // 
      this.qualityTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.qualityTextBox.Location = new System.Drawing.Point(61, 3);
      this.qualityTextBox.Name = "qualityTextBox";
      this.qualityTextBox.ReadOnly = true;
      this.qualityTextBox.Size = new System.Drawing.Size(159, 20);
      this.qualityTextBox.TabIndex = 0;
      // 
      // vehiclesTextBox
      // 
      this.vehiclesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.vehiclesTextBox.Location = new System.Drawing.Point(61, 55);
      this.vehiclesTextBox.Name = "vehiclesTextBox";
      this.vehiclesTextBox.ReadOnly = true;
      this.vehiclesTextBox.Size = new System.Drawing.Size(159, 20);
      this.vehiclesTextBox.TabIndex = 2;
      // 
      // distanceLabel
      // 
      this.distanceLabel.AutoSize = true;
      this.distanceLabel.Location = new System.Drawing.Point(3, 32);
      this.distanceLabel.Name = "distanceLabel";
      this.distanceLabel.Size = new System.Drawing.Size(52, 13);
      this.distanceLabel.TabIndex = 6;
      this.distanceLabel.Text = "Distance:";
      // 
      // vehiclesLabel
      // 
      this.vehiclesLabel.AutoSize = true;
      this.vehiclesLabel.Location = new System.Drawing.Point(3, 58);
      this.vehiclesLabel.Name = "vehiclesLabel";
      this.vehiclesLabel.Size = new System.Drawing.Size(50, 13);
      this.vehiclesLabel.TabIndex = 7;
      this.vehiclesLabel.Text = "Vehicles:";
      // 
      // penaltyLabel
      // 
      this.penaltyLabel.AutoSize = true;
      this.penaltyLabel.Location = new System.Drawing.Point(3, 84);
      this.penaltyLabel.Name = "penaltyLabel";
      this.penaltyLabel.Size = new System.Drawing.Size(45, 13);
      this.penaltyLabel.TabIndex = 8;
      this.penaltyLabel.Text = "Penalty:";
      // 
      // feasibleLabel
      // 
      this.feasibleLabel.AutoSize = true;
      this.feasibleLabel.Location = new System.Drawing.Point(3, 107);
      this.feasibleLabel.Name = "feasibleLabel";
      this.feasibleLabel.Size = new System.Drawing.Size(49, 13);
      this.feasibleLabel.TabIndex = 9;
      this.feasibleLabel.Text = "Feasible:";
      // 
      // distanceTextBox
      // 
      this.distanceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.distanceTextBox.Location = new System.Drawing.Point(61, 29);
      this.distanceTextBox.Name = "distanceTextBox";
      this.distanceTextBox.ReadOnly = true;
      this.distanceTextBox.Size = new System.Drawing.Size(159, 20);
      this.distanceTextBox.TabIndex = 1;
      // 
      // penaltyTextBox
      // 
      this.penaltyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.penaltyTextBox.Location = new System.Drawing.Point(61, 81);
      this.penaltyTextBox.Name = "penaltyTextBox";
      this.penaltyTextBox.ReadOnly = true;
      this.penaltyTextBox.Size = new System.Drawing.Size(159, 20);
      this.penaltyTextBox.TabIndex = 3;
      // 
      // isFeasibleCcheckBox
      // 
      this.isFeasibleCcheckBox.AutoSize = true;
      this.isFeasibleCcheckBox.Location = new System.Drawing.Point(61, 107);
      this.isFeasibleCcheckBox.Name = "isFeasibleCcheckBox";
      this.isFeasibleCcheckBox.Size = new System.Drawing.Size(15, 14);
      this.isFeasibleCcheckBox.TabIndex = 4;
      this.isFeasibleCcheckBox.UseVisualStyleBackColor = true;
      // 
      // VRPEvaluationView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.isFeasibleCcheckBox);
      this.Controls.Add(this.penaltyTextBox);
      this.Controls.Add(this.distanceTextBox);
      this.Controls.Add(this.vehiclesTextBox);
      this.Controls.Add(this.qualityTextBox);
      this.Controls.Add(this.feasibleLabel);
      this.Controls.Add(this.penaltyLabel);
      this.Controls.Add(this.vehiclesLabel);
      this.Controls.Add(this.distanceLabel);
      this.Controls.Add(this.qualityLabel);
      this.Name = "VRPEvaluationView";
      this.Size = new System.Drawing.Size(230, 128);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label qualityLabel;
    protected System.Windows.Forms.TextBox qualityTextBox;
    protected System.Windows.Forms.TextBox vehiclesTextBox;
    protected System.Windows.Forms.Label distanceLabel;
    protected System.Windows.Forms.Label vehiclesLabel;
    protected System.Windows.Forms.Label penaltyLabel;
    protected System.Windows.Forms.Label feasibleLabel;
    protected System.Windows.Forms.TextBox distanceTextBox;
    protected System.Windows.Forms.TextBox penaltyTextBox;
    protected System.Windows.Forms.CheckBox isFeasibleCcheckBox;
  }
}
