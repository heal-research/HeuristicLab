#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class RegressionSolutionView {
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
      this.btnImpactCalculation = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.itemsGroupBox.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnImpactCalculation
      // 
      this.btnImpactCalculation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
      this.btnImpactCalculation.Image = HeuristicLab.Common.Resources.VSImageLibrary.Zoom;
      this.btnImpactCalculation.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnImpactCalculation.Name = "btnImpactCalculation";
      this.btnImpactCalculation.TabIndex = 6;
      this.btnImpactCalculation.Size = new System.Drawing.Size(110, 24);
      this.btnImpactCalculation.Text = "Variable Impacts";
      this.btnImpactCalculation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.btnImpactCalculation.UseVisualStyleBackColor = true;
      this.btnImpactCalculation.Click += new System.EventHandler(this.btnImpactCalculation_Click);
      this.toolTip.SetToolTip(this.btnImpactCalculation, "Simplify solution");
      // 
      // flowLayoutPanel
      // 
      this.flowLayoutPanel.Controls.Add(this.btnImpactCalculation);
      // 
      // splitContainer
      // 
      // 
      // itemsGroupBox
      // 
      this.itemsGroupBox.Text = "Regression Solution";
      // 
      // addButton
      // 
      this.toolTip.SetToolTip(this.addButton, "Add");
      // 
      // removeButton
      // 
      this.toolTip.SetToolTip(this.removeButton, "Remove");
      // 
      // RegressionSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Name = "RegressionSolutionView";
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    protected System.Windows.Forms.Button btnImpactCalculation;
  }
}
