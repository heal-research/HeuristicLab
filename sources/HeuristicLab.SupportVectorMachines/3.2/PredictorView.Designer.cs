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

namespace HeuristicLab.SupportVectorMachines {
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
      this.maxTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.maxTimeOffsetLabel = new System.Windows.Forms.Label();
      this.minTimeOffsetTextBox = new System.Windows.Forms.TextBox();
      this.minTimeOffsetLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lowerPredictionLimit
      // 
      this.lowerPredictionLimit.AutoSize = true;
      this.lowerPredictionLimit.Location = new System.Drawing.Point(3, 189);
      this.lowerPredictionLimit.Name = "lowerPredictionLimit";
      this.lowerPredictionLimit.Size = new System.Drawing.Size(113, 13);
      this.lowerPredictionLimit.TabIndex = 13;
      this.lowerPredictionLimit.Text = "Lower Prediction Limit:";
      // 
      // lowerLimitTextbox
      // 
      this.lowerLimitTextbox.Location = new System.Drawing.Point(144, 186);
      this.lowerLimitTextbox.Name = "lowerLimitTextbox";
      this.lowerLimitTextbox.ReadOnly = true;
      this.lowerLimitTextbox.Size = new System.Drawing.Size(100, 20);
      this.lowerLimitTextbox.TabIndex = 14;
      // 
      // upperLimitTextbox
      // 
      this.upperLimitTextbox.Location = new System.Drawing.Point(144, 212);
      this.upperLimitTextbox.Name = "upperLimitTextbox";
      this.upperLimitTextbox.ReadOnly = true;
      this.upperLimitTextbox.Size = new System.Drawing.Size(100, 20);
      this.upperLimitTextbox.TabIndex = 16;
      // 
      // upperPredictionLimit
      // 
      this.upperPredictionLimit.AutoSize = true;
      this.upperPredictionLimit.Location = new System.Drawing.Point(3, 215);
      this.upperPredictionLimit.Name = "upperPredictionLimit";
      this.upperPredictionLimit.Size = new System.Drawing.Size(113, 13);
      this.upperPredictionLimit.TabIndex = 15;
      this.upperPredictionLimit.Text = "Upper Prediction Limit:";
      // 
      // maxTimeOffsetTextBox
      // 
      this.maxTimeOffsetTextBox.Location = new System.Drawing.Point(144, 134);
      this.maxTimeOffsetTextBox.Name = "maxTimeOffsetTextBox";
      this.maxTimeOffsetTextBox.ReadOnly = true;
      this.maxTimeOffsetTextBox.Size = new System.Drawing.Size(100, 20);
      this.maxTimeOffsetTextBox.TabIndex = 18;
      // 
      // maxTimeOffsetLabel
      // 
      this.maxTimeOffsetLabel.AutoSize = true;
      this.maxTimeOffsetLabel.Location = new System.Drawing.Point(3, 137);
      this.maxTimeOffsetLabel.Name = "maxTimeOffsetLabel";
      this.maxTimeOffsetLabel.Size = new System.Drawing.Size(81, 13);
      this.maxTimeOffsetLabel.TabIndex = 17;
      this.maxTimeOffsetLabel.Text = "Max time offset:";
      // 
      // minTimeOffsetTextBox
      // 
      this.minTimeOffsetTextBox.Location = new System.Drawing.Point(144, 160);
      this.minTimeOffsetTextBox.Name = "minTimeOffsetTextBox";
      this.minTimeOffsetTextBox.ReadOnly = true;
      this.minTimeOffsetTextBox.Size = new System.Drawing.Size(100, 20);
      this.minTimeOffsetTextBox.TabIndex = 20;
      // 
      // minTimeOffsetLabel
      // 
      this.minTimeOffsetLabel.AutoSize = true;
      this.minTimeOffsetLabel.Location = new System.Drawing.Point(3, 163);
      this.minTimeOffsetLabel.Name = "minTimeOffsetLabel";
      this.minTimeOffsetLabel.Size = new System.Drawing.Size(78, 13);
      this.minTimeOffsetLabel.TabIndex = 19;
      this.minTimeOffsetLabel.Text = "Min time offset:";
      // 
      // PredictorView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.minTimeOffsetTextBox);
      this.Controls.Add(this.minTimeOffsetLabel);
      this.Controls.Add(this.maxTimeOffsetTextBox);
      this.Controls.Add(this.maxTimeOffsetLabel);
      this.Controls.Add(this.upperLimitTextbox);
      this.Controls.Add(this.upperPredictionLimit);
      this.Controls.Add(this.lowerLimitTextbox);
      this.Controls.Add(this.lowerPredictionLimit);
      this.Name = "PredictorView";
      this.Size = new System.Drawing.Size(252, 240);
      this.Controls.SetChildIndex(this.lowerPredictionLimit, 0);
      this.Controls.SetChildIndex(this.lowerLimitTextbox, 0);
      this.Controls.SetChildIndex(this.upperPredictionLimit, 0);
      this.Controls.SetChildIndex(this.upperLimitTextbox, 0);
      this.Controls.SetChildIndex(this.maxTimeOffsetLabel, 0);
      this.Controls.SetChildIndex(this.maxTimeOffsetTextBox, 0);
      this.Controls.SetChildIndex(this.minTimeOffsetLabel, 0);
      this.Controls.SetChildIndex(this.minTimeOffsetTextBox, 0);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lowerPredictionLimit;
    private System.Windows.Forms.TextBox lowerLimitTextbox;
    private System.Windows.Forms.TextBox upperLimitTextbox;
    private System.Windows.Forms.Label upperPredictionLimit;
    private System.Windows.Forms.TextBox maxTimeOffsetTextBox;
    private System.Windows.Forms.Label maxTimeOffsetLabel;
    private System.Windows.Forms.TextBox minTimeOffsetTextBox;
    private System.Windows.Forms.Label minTimeOffsetLabel;

  }
}
