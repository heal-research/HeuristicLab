#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Problems.DataAnalysis.Views.Symbolic {
  partial class SymbolicRegressionSolutionView {
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
      this.btn_SimplifyModel = new System.Windows.Forms.Button();
      this.tabControl.SuspendLayout();
      this.dataTabPage.SuspendLayout();
      this.modelTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Size = new System.Drawing.Size(380, 233);
      // 
      // dataTabPage
      // 
      this.dataTabPage.Size = new System.Drawing.Size(372, 207);
      // 
      // modelTabPage
      // 
      this.modelTabPage.Size = new System.Drawing.Size(372, 207);
      // 
      // dataViewHost
      // 
      this.dataViewHost.Size = new System.Drawing.Size(360, 195);
      // 
      // modelViewHost
      // 
      this.modelViewHost.Size = new System.Drawing.Size(360, 195);
      // 
      // btn_SimplifyModel
      // 
      this.btn_SimplifyModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_SimplifyModel.Location = new System.Drawing.Point(3, 238);
      this.btn_SimplifyModel.Name = "btn_SimplifyModel";
      this.btn_SimplifyModel.Size = new System.Drawing.Size(376, 23);
      this.btn_SimplifyModel.TabIndex = 2;
      this.btn_SimplifyModel.Text = "Simplify Model";
      this.btn_SimplifyModel.UseVisualStyleBackColor = true;
      this.btn_SimplifyModel.Click += new System.EventHandler(this.btn_SimplifyModel_Click);
      // 
      // SymbolicRegressionSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.btn_SimplifyModel);
      this.Name = "SymbolicRegressionSolutionView";
      this.Controls.SetChildIndex(this.btn_SimplifyModel, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.tabControl.ResumeLayout(false);
      this.dataTabPage.ResumeLayout(false);
      this.modelTabPage.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btn_SimplifyModel;
  }
}
