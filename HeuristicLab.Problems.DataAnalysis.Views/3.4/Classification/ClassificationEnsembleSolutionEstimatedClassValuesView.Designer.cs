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
  partial class ClassificationEnsembleSolutionEstimatedClassValuesView {
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
      this.SamplesComboBox = new System.Windows.Forms.ComboBox();
      this.SuspendLayout();
      // 
      // matrixView
      // 
      this.matrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.matrixView.Dock = System.Windows.Forms.DockStyle.None;
      this.matrixView.Location = new System.Drawing.Point(3, 31);
      this.matrixView.Size = new System.Drawing.Size(304, 251);
      // 
      // SamplesComboBox
      // 
      this.SamplesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.SamplesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.SamplesComboBox.FormattingEnabled = true;
      this.SamplesComboBox.Location = new System.Drawing.Point(4, 4);
      this.SamplesComboBox.Name = "SamplesComboBox";
      this.SamplesComboBox.Size = new System.Drawing.Size(303, 21);
      this.SamplesComboBox.TabIndex = 2;
      this.SamplesComboBox.SelectedIndexChanged += new System.EventHandler(this.SamplesComboBox_SelectedIndexChanged);
      // 
      // ClassificationEnsembleSolutionEstimatedClassValuesView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.SamplesComboBox);
      this.Name = "ClassificationEnsembleSolutionEstimatedClassValuesView";
      this.Controls.SetChildIndex(this.matrixView, 0);
      this.Controls.SetChildIndex(this.SamplesComboBox, 0);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox SamplesComboBox;

  }
}
