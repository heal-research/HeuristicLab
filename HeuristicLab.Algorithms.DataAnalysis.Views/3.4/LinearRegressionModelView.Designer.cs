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

namespace HeuristicLab.Algorithms.DataAnalysis.Views {
  partial class LinearRegressionModelView {
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
      this.parametersView = new HeuristicLab.Data.Views.StringConvertibleArrayView();
      this.covarianceMatrixView = new HeuristicLab.Data.Views.StringConvertibleMatrixView();
      this.parameterVectorLabel = new System.Windows.Forms.Label();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.covMatLabel = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // parametersView
      // 
      this.parametersView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.parametersView.Caption = "StringConvertibleArray View";
      this.parametersView.Content = null;
      this.parametersView.Location = new System.Drawing.Point(6, 19);
      this.parametersView.Name = "parametersView";
      this.parametersView.ReadOnly = true;
      this.parametersView.Size = new System.Drawing.Size(136, 267);
      this.parametersView.TabIndex = 0;
      // 
      // covarianceMatrixView
      // 
      this.covarianceMatrixView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.covarianceMatrixView.Caption = "StringConvertibleMatrix View";
      this.covarianceMatrixView.Content = null;
      this.covarianceMatrixView.Location = new System.Drawing.Point(3, 19);
      this.covarianceMatrixView.Name = "covarianceMatrixView";
      this.covarianceMatrixView.ReadOnly = true;
      this.covarianceMatrixView.ShowRowsAndColumnsTextBox = true;
      this.covarianceMatrixView.ShowStatisticalInformation = true;
      this.covarianceMatrixView.Size = new System.Drawing.Size(194, 267);
      this.covarianceMatrixView.TabIndex = 1;
      // 
      // parameterVectorLabel
      // 
      this.parameterVectorLabel.AutoSize = true;
      this.parameterVectorLabel.Location = new System.Drawing.Point(3, 0);
      this.parameterVectorLabel.Margin = new System.Windows.Forms.Padding(3);
      this.parameterVectorLabel.Name = "parameterVectorLabel";
      this.parameterVectorLabel.Size = new System.Drawing.Size(63, 13);
      this.parameterVectorLabel.TabIndex = 2;
      this.parameterVectorLabel.Text = "Parameters:";
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer1";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.parametersView);
      this.splitContainer.Panel1.Controls.Add(this.parameterVectorLabel);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.covMatLabel);
      this.splitContainer.Panel2.Controls.Add(this.covarianceMatrixView);
      this.splitContainer.Size = new System.Drawing.Size(349, 289);
      this.splitContainer.SplitterDistance = 145;
      this.splitContainer.TabIndex = 4;
      // 
      // covMatLabel
      // 
      this.covMatLabel.AutoSize = true;
      this.covMatLabel.Location = new System.Drawing.Point(3, 0);
      this.covMatLabel.Margin = new System.Windows.Forms.Padding(3);
      this.covMatLabel.Name = "covMatLabel";
      this.covMatLabel.Size = new System.Drawing.Size(94, 13);
      this.covMatLabel.TabIndex = 3;
      this.covMatLabel.Text = "Covariance matrix:";
      // 
      // LinearRegressionModelView
      // 
      this.AllowDrop = true;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.splitContainer);
      this.Name = "LinearRegressionModelView";
      this.Size = new System.Drawing.Size(349, 289);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      this.splitContainer.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }




    #endregion

    private Data.Views.StringConvertibleArrayView parametersView;
    private Data.Views.StringConvertibleMatrixView covarianceMatrixView;
    private System.Windows.Forms.Label parameterVectorLabel;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.Label covMatLabel;
  }
}
