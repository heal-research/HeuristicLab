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
  partial class QuadraticAssignmentProblemView {
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
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.problemTabPage = new System.Windows.Forms.TabPage();
      this.visualizationTabPage = new System.Windows.Forms.TabPage();
      this.qapView = new HeuristicLab.Problems.QuadraticAssignment.Views.QAPVisualizationControl();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.tabControl.SuspendLayout();
      this.visualizationTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Location = new System.Drawing.Point(8, 107);
      this.parameterCollectionView.Size = new System.Drawing.Size(629, 375);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(50, 29);
      this.nameTextBox.Size = new System.Drawing.Size(572, 20);
      // 
      // nameLabel
      // 
      this.nameLabel.Location = new System.Drawing.Point(3, 32);
      // 
      // infoLabel
      // 
      this.infoLabel.Location = new System.Drawing.Point(628, 32);
      // 
      // openFileDialog
      // 
      this.openFileDialog.FileName = "instance.dat";
      this.openFileDialog.Filter = "Dat files|*.dat|All files|*.*";
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.problemTabPage);
      this.tabControl.Controls.Add(this.visualizationTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 55);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(647, 437);
      this.tabControl.TabIndex = 8;
      // 
      // problemTabPage
      // 
      this.problemTabPage.Location = new System.Drawing.Point(4, 22);
      this.problemTabPage.Name = "problemTabPage";
      this.problemTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.problemTabPage.Size = new System.Drawing.Size(639, 411);
      this.problemTabPage.TabIndex = 0;
      this.problemTabPage.Text = "Problem";
      this.problemTabPage.UseVisualStyleBackColor = true;
      // 
      // visualizationTabPage
      // 
      this.visualizationTabPage.Controls.Add(this.qapView);
      this.visualizationTabPage.Location = new System.Drawing.Point(4, 22);
      this.visualizationTabPage.Name = "visualizationTabPage";
      this.visualizationTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.visualizationTabPage.Size = new System.Drawing.Size(639, 411);
      this.visualizationTabPage.TabIndex = 1;
      this.visualizationTabPage.Text = "Visualization";
      this.visualizationTabPage.UseVisualStyleBackColor = true;
      // 
      // qapView
      // 
      this.qapView.Assignment = null;
      this.qapView.Distances = null;
      this.qapView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.qapView.Location = new System.Drawing.Point(3, 3);
      this.qapView.Name = "qapView";
      this.qapView.Size = new System.Drawing.Size(633, 405);
      this.qapView.TabIndex = 0;
      this.qapView.Weights = null;
      // 
      // QuadraticAssignmentProblemView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Name = "QuadraticAssignmentProblemView";
      this.Size = new System.Drawing.Size(647, 492);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.parameterCollectionView, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.tabControl.ResumeLayout(false);
      this.visualizationTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage problemTabPage;
    private System.Windows.Forms.TabPage visualizationTabPage;
    private QAPVisualizationControl qapView;
  }
}
