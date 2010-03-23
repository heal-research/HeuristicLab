#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

namespace HeuristicLab.Algorithms.SGA.Views {
  partial class SGAView {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing) {
        if (components != null) components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.engineTabPage.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.problemTabPage.SuspendLayout();
      this.resultsTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // createUserDefinedAlgorithmButton
      // 
      this.createUserDefinedAlgorithmButton.Location = new System.Drawing.Point(90, 552);
      this.toolTip.SetToolTip(this.createUserDefinedAlgorithmButton, "Create User Defined Algorithm from this Algorithm");
      // 
      // engineComboBox
      // 
      this.engineComboBox.Size = new System.Drawing.Size(675, 21);
      // 
      // engineTabPage
      // 
      this.engineTabPage.Size = new System.Drawing.Size(736, 468);
      // 
      // engineViewHost
      // 
      this.engineViewHost.Size = new System.Drawing.Size(724, 429);
      // 
      // tabControl
      // 
      this.tabControl.Size = new System.Drawing.Size(744, 494);
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Size = new System.Drawing.Size(736, 468);
      // 
      // problemTabPage
      // 
      this.problemTabPage.Size = new System.Drawing.Size(736, 468);
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Size = new System.Drawing.Size(724, 456);
      // 
      // problemViewHost
      // 
      this.problemViewHost.Size = new System.Drawing.Size(724, 426);
      // 
      // newProblemButton
      // 
      this.toolTip.SetToolTip(this.newProblemButton, "New Problem");
      // 
      // saveProblemButton
      // 
      this.toolTip.SetToolTip(this.saveProblemButton, "Save Problem");
      // 
      // openProblemButton
      // 
      this.toolTip.SetToolTip(this.openProblemButton, "Open Problem");
      // 
      // startButton
      // 
      this.startButton.Location = new System.Drawing.Point(0, 552);
      this.toolTip.SetToolTip(this.startButton, "Start Algorithm");
      // 
      // stopButton
      // 
      this.stopButton.Location = new System.Drawing.Point(30, 552);
      this.toolTip.SetToolTip(this.stopButton, "Stop Algorithm");
      // 
      // resetButton
      // 
      this.resetButton.Location = new System.Drawing.Point(60, 552);
      this.toolTip.SetToolTip(this.resetButton, "Reset Algorithm");
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Location = new System.Drawing.Point(518, 559);
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Location = new System.Drawing.Point(607, 556);
      // 
      // resultsTabPage
      // 
      this.resultsTabPage.Size = new System.Drawing.Size(736, 468);
      // 
      // resultsView
      // 
      this.resultsView.Size = new System.Drawing.Size(724, 456);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(672, 20);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Size = new System.Drawing.Size(672, 20);
      // 
      // SGAView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "SGAView";
      this.Size = new System.Drawing.Size(744, 576);
      this.engineTabPage.ResumeLayout(false);
      this.engineTabPage.PerformLayout();
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.problemTabPage.ResumeLayout(false);
      this.resultsTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion



  }
}
