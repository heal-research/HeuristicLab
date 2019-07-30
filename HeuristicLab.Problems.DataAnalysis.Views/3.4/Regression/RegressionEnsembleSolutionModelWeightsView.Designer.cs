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
namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class RegressionEnsembleSolutionModelWeightsView {
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
      this.arrayView = new HeuristicLab.Data.Views.StringConvertibleArrayView();
      this.averageEstimatesCheckBox = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // arrayView
      // 
      this.arrayView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.arrayView.Caption = "StringConvertibleArray View";
      this.arrayView.Content = null;
      this.arrayView.Location = new System.Drawing.Point(3, 27);
      this.arrayView.Name = "arrayView";
      this.arrayView.ReadOnly = false;
      this.arrayView.Size = new System.Drawing.Size(711, 508);
      this.arrayView.TabIndex = 0;
      // 
      // averageEstimatesCheckBox
      // 
      this.averageEstimatesCheckBox.AutoSize = true;
      this.averageEstimatesCheckBox.Location = new System.Drawing.Point(4, 4);
      this.averageEstimatesCheckBox.Name = "averageEstimatesCheckBox";
      this.averageEstimatesCheckBox.Size = new System.Drawing.Size(150, 17);
      this.averageEstimatesCheckBox.TabIndex = 1;
      this.averageEstimatesCheckBox.Text = "Average Estimated Values";
      this.averageEstimatesCheckBox.UseVisualStyleBackColor = true;
      this.averageEstimatesCheckBox.CheckedChanged += new System.EventHandler(this.averageEstimatesCheckBox_CheckedChanged);
      // 
      // RegressionEnsembleSolutionModelWeightsView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.averageEstimatesCheckBox);
      this.Controls.Add(this.arrayView);
      this.Name = "RegressionEnsembleSolutionModelWeightsView";
      this.Size = new System.Drawing.Size(717, 538);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Data.Views.StringConvertibleArrayView arrayView;
    private System.Windows.Forms.CheckBox averageEstimatesCheckBox;
  }
}
