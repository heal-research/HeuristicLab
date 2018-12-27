#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Optimization.Views {
  partial class ISolutionSimilarityCalculatorView {
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
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.qualityVariableNameTextBox = new System.Windows.Forms.TextBox();
      this.solutionVariableNameTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(135, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Variable Name of Solution: ";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 32);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(126, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Variable Name of Quality:";
      // 
      // qualityVariableNameTextBox
      // 
      this.qualityVariableNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.qualityVariableNameTextBox.Location = new System.Drawing.Point(144, 29);
      this.qualityVariableNameTextBox.Name = "qualityVariableNameTextBox";
      this.qualityVariableNameTextBox.Size = new System.Drawing.Size(287, 20);
      this.qualityVariableNameTextBox.TabIndex = 2;
      this.qualityVariableNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.qualityVariableNameTextBox_Validating);
      // 
      // solutionVariableNameTextBox
      // 
      this.solutionVariableNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.solutionVariableNameTextBox.Location = new System.Drawing.Point(144, 3);
      this.solutionVariableNameTextBox.Name = "solutionVariableNameTextBox";
      this.solutionVariableNameTextBox.Size = new System.Drawing.Size(287, 20);
      this.solutionVariableNameTextBox.TabIndex = 3;
      this.solutionVariableNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.solutionVariableNameTextBox_Validating);
      // 
      // ISingleObjectiveSolutionSimilarityCalculatorView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.solutionVariableNameTextBox);
      this.Controls.Add(this.qualityVariableNameTextBox);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Name = "ISingleObjectiveSolutionSimilarityCalculatorView";
      this.Size = new System.Drawing.Size(434, 53);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox qualityVariableNameTextBox;
    private System.Windows.Forms.TextBox solutionVariableNameTextBox;
  }
}
