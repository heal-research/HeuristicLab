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

namespace HeuristicLab.Problems.QuadraticAssignment.Views {
  partial class QAPVisualizationControl {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      CustomDispose(disposing);
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
      this.redrawButton = new System.Windows.Forms.Button();
      this.distancesRadioButton = new System.Windows.Forms.RadioButton();
      this.weightsRadioButton = new System.Windows.Forms.RadioButton();
      this.pictureBox = new System.Windows.Forms.PictureBox();
      this.stressLabel = new System.Windows.Forms.Label();
      this.assignmentRadioButton = new System.Windows.Forms.RadioButton();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // redrawButton
      // 
      this.redrawButton.Location = new System.Drawing.Point(3, 3);
      this.redrawButton.Name = "redrawButton";
      this.redrawButton.Size = new System.Drawing.Size(26, 23);
      this.redrawButton.TabIndex = 7;
      this.redrawButton.Text = "Redraw";
      this.redrawButton.UseVisualStyleBackColor = true;
      this.redrawButton.Click += new System.EventHandler(this.redrawButton_Click);
      // 
      // distancesRadioButton
      // 
      this.distancesRadioButton.AutoSize = true;
      this.distancesRadioButton.Checked = true;
      this.distancesRadioButton.Location = new System.Drawing.Point(35, 6);
      this.distancesRadioButton.Name = "distancesRadioButton";
      this.distancesRadioButton.Size = new System.Drawing.Size(72, 17);
      this.distancesRadioButton.TabIndex = 6;
      this.distancesRadioButton.TabStop = true;
      this.distancesRadioButton.Text = "Distances";
      this.distancesRadioButton.UseVisualStyleBackColor = true;
      this.distancesRadioButton.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
      // 
      // weightsRadioButton
      // 
      this.weightsRadioButton.AutoSize = true;
      this.weightsRadioButton.Location = new System.Drawing.Point(113, 6);
      this.weightsRadioButton.Name = "weightsRadioButton";
      this.weightsRadioButton.Size = new System.Drawing.Size(64, 17);
      this.weightsRadioButton.TabIndex = 5;
      this.weightsRadioButton.Text = "Weights";
      this.weightsRadioButton.UseVisualStyleBackColor = true;
      this.weightsRadioButton.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
      // 
      // pictureBox
      // 
      this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.pictureBox.BackColor = System.Drawing.Color.White;
      this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBox.Location = new System.Drawing.Point(0, 32);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(574, 422);
      this.pictureBox.TabIndex = 4;
      this.pictureBox.TabStop = false;
      this.pictureBox.SizeChanged += new System.EventHandler(this.pictureBox_SizeChanged);
      // 
      // stressLabel
      // 
      this.stressLabel.AutoSize = true;
      this.stressLabel.Location = new System.Drawing.Point(393, 8);
      this.stressLabel.Name = "stressLabel";
      this.stressLabel.Size = new System.Drawing.Size(10, 13);
      this.stressLabel.TabIndex = 8;
      this.stressLabel.Text = "-";
      // 
      // assignmentRadioButton
      // 
      this.assignmentRadioButton.AutoSize = true;
      this.assignmentRadioButton.Location = new System.Drawing.Point(183, 6);
      this.assignmentRadioButton.Name = "assignmentRadioButton";
      this.assignmentRadioButton.Size = new System.Drawing.Size(79, 17);
      this.assignmentRadioButton.TabIndex = 5;
      this.assignmentRadioButton.Text = "Assignment";
      this.assignmentRadioButton.UseVisualStyleBackColor = true;
      this.assignmentRadioButton.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(293, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(94, 13);
      this.label1.TabIndex = 8;
      this.label1.Text = "Normalized Stress:";
      // 
      // QAPVisualizationControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.label1);
      this.Controls.Add(this.stressLabel);
      this.Controls.Add(this.redrawButton);
      this.Controls.Add(this.distancesRadioButton);
      this.Controls.Add(this.assignmentRadioButton);
      this.Controls.Add(this.weightsRadioButton);
      this.Controls.Add(this.pictureBox);
      this.Name = "QAPVisualizationControl";
      this.Size = new System.Drawing.Size(574, 454);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button redrawButton;
    private System.Windows.Forms.RadioButton distancesRadioButton;
    private System.Windows.Forms.RadioButton weightsRadioButton;
    private System.Windows.Forms.PictureBox pictureBox;
    private System.Windows.Forms.Label stressLabel;
    private System.Windows.Forms.RadioButton assignmentRadioButton;
    private System.Windows.Forms.Label label1;

  }
}
