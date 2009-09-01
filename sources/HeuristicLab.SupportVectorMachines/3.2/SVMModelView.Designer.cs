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
  partial class SVMModelView {
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
      this.numberSupportVectorsLabel = new System.Windows.Forms.Label();
      this.numberOfSupportVectors = new System.Windows.Forms.TextBox();
      this.rhoLabel = new System.Windows.Forms.Label();
      this.rho = new System.Windows.Forms.TextBox();
      this.svmTypeLabel = new System.Windows.Forms.Label();
      this.kernelTypeLabel = new System.Windows.Forms.Label();
      this.gammaLabel = new System.Windows.Forms.Label();
      this.svmType = new System.Windows.Forms.TextBox();
      this.kernelType = new System.Windows.Forms.TextBox();
      this.gamma = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // numberSupportVectorsLabel
      // 
      this.numberSupportVectorsLabel.AutoSize = true;
      this.numberSupportVectorsLabel.Location = new System.Drawing.Point(3, 7);
      this.numberSupportVectorsLabel.Name = "numberSupportVectorsLabel";
      this.numberSupportVectorsLabel.Size = new System.Drawing.Size(135, 13);
      this.numberSupportVectorsLabel.TabIndex = 0;
      this.numberSupportVectorsLabel.Text = "Number of support vectors:";
      // 
      // numberOfSupportVectors
      // 
      this.numberOfSupportVectors.Location = new System.Drawing.Point(144, 4);
      this.numberOfSupportVectors.Name = "numberOfSupportVectors";
      this.numberOfSupportVectors.ReadOnly = true;
      this.numberOfSupportVectors.Size = new System.Drawing.Size(100, 20);
      this.numberOfSupportVectors.TabIndex = 1;
      // 
      // rhoLabel
      // 
      this.rhoLabel.AutoSize = true;
      this.rhoLabel.Location = new System.Drawing.Point(3, 33);
      this.rhoLabel.Name = "rhoLabel";
      this.rhoLabel.Size = new System.Drawing.Size(30, 13);
      this.rhoLabel.TabIndex = 2;
      this.rhoLabel.Text = "Rho:";
      // 
      // rho
      // 
      this.rho.Location = new System.Drawing.Point(144, 30);
      this.rho.Name = "rho";
      this.rho.ReadOnly = true;
      this.rho.Size = new System.Drawing.Size(100, 20);
      this.rho.TabIndex = 3;
      // 
      // svmTypeLabel
      // 
      this.svmTypeLabel.AutoSize = true;
      this.svmTypeLabel.Location = new System.Drawing.Point(3, 59);
      this.svmTypeLabel.Name = "svmTypeLabel";
      this.svmTypeLabel.Size = new System.Drawing.Size(60, 13);
      this.svmTypeLabel.TabIndex = 4;
      this.svmTypeLabel.Text = "SVM Type:";
      // 
      // kernelTypeLabel
      // 
      this.kernelTypeLabel.AutoSize = true;
      this.kernelTypeLabel.Location = new System.Drawing.Point(3, 85);
      this.kernelTypeLabel.Name = "kernelTypeLabel";
      this.kernelTypeLabel.Size = new System.Drawing.Size(67, 13);
      this.kernelTypeLabel.TabIndex = 7;
      this.kernelTypeLabel.Text = "Kernel Type:";
      // 
      // gammaLabel
      // 
      this.gammaLabel.AutoSize = true;
      this.gammaLabel.Location = new System.Drawing.Point(3, 111);
      this.gammaLabel.Name = "gammaLabel";
      this.gammaLabel.Size = new System.Drawing.Size(46, 13);
      this.gammaLabel.TabIndex = 8;
      this.gammaLabel.Text = "Gamma:";
      // 
      // svmType
      // 
      this.svmType.Location = new System.Drawing.Point(144, 56);
      this.svmType.Name = "svmType";
      this.svmType.ReadOnly = true;
      this.svmType.Size = new System.Drawing.Size(100, 20);
      this.svmType.TabIndex = 10;
      // 
      // kernelType
      // 
      this.kernelType.Location = new System.Drawing.Point(144, 82);
      this.kernelType.Name = "kernelType";
      this.kernelType.ReadOnly = true;
      this.kernelType.Size = new System.Drawing.Size(100, 20);
      this.kernelType.TabIndex = 11;
      // 
      // gamma
      // 
      this.gamma.Location = new System.Drawing.Point(144, 108);
      this.gamma.Name = "gamma";
      this.gamma.ReadOnly = true;
      this.gamma.Size = new System.Drawing.Size(100, 20);
      this.gamma.TabIndex = 12;
      // 
      // SVMModelView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.gamma);
      this.Controls.Add(this.kernelType);
      this.Controls.Add(this.svmType);
      this.Controls.Add(this.gammaLabel);
      this.Controls.Add(this.kernelTypeLabel);
      this.Controls.Add(this.svmTypeLabel);
      this.Controls.Add(this.rho);
      this.Controls.Add(this.rhoLabel);
      this.Controls.Add(this.numberOfSupportVectors);
      this.Controls.Add(this.numberSupportVectorsLabel);
      this.Name = "SVMModelView";
      this.Size = new System.Drawing.Size(253, 135);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label numberSupportVectorsLabel;
    private System.Windows.Forms.TextBox numberOfSupportVectors;
    private System.Windows.Forms.Label rhoLabel;
    private System.Windows.Forms.TextBox rho;
    private System.Windows.Forms.Label svmTypeLabel;
    private System.Windows.Forms.Label kernelTypeLabel;
    private System.Windows.Forms.Label gammaLabel;
    private System.Windows.Forms.TextBox svmType;
    private System.Windows.Forms.TextBox kernelType;
    private System.Windows.Forms.TextBox gamma;
  }
}
