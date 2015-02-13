#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
  partial class UserDefinedAlgorithmView {
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
      this.globalScopeTabPage = new System.Windows.Forms.TabPage();
      this.globalScopeView = new HeuristicLab.Core.Views.ScopeView();
      this.engineTabPage.SuspendLayout();
      this.operatorGraphTabPage.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.problemTabPage.SuspendLayout();
      this.resultsTabPage.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.globalScopeTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // engineComboBox
      // 
      this.engineComboBox.Size = new System.Drawing.Size(644, 21);
      // 
      // engineViewHost
      // 
      this.engineViewHost.Size = new System.Drawing.Size(693, 402);
      // 
      // openOperatorGraphButton
      // 
      this.toolTip.SetToolTip(this.openOperatorGraphButton, "Open Operator Graph");
      this.openOperatorGraphButton.Click += new System.EventHandler(openOperatorGraphButton_Click);
      // 
      // newOperatorGraphButton
      // 
      this.toolTip.SetToolTip(this.newOperatorGraphButton, "New Operator Graph");
      this.newOperatorGraphButton.Click += new System.EventHandler(newOperatorGraphButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.globalScopeTabPage);
      this.tabControl.Controls.SetChildIndex(this.engineTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.globalScopeTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.operatorGraphTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.runsTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.resultsTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.parametersTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.problemTabPage, 0);
      // 
      // newProblemButton
      // 
      this.toolTip.SetToolTip(this.newProblemButton, "New Problem");
      // 
      // openProblemButton
      // 
      this.toolTip.SetToolTip(this.openProblemButton, "Open Problem");
      // 
      // startButton
      // 
      this.toolTip.SetToolTip(this.startButton, "Start/Resume Algorithm");
      // 
      // pauseButton
      // 
      this.toolTip.SetToolTip(this.pauseButton, "Pause Algorithm");
      // 
      // resetButton
      // 
      this.toolTip.SetToolTip(this.resetButton, "Reset Algorithm");
      // 
      // stopButton
      // 
      this.toolTip.SetToolTip(this.stopButton, "Stop Algorithm");
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // globalScopeTabPage
      // 
      this.globalScopeTabPage.Controls.Add(this.globalScopeView);
      this.globalScopeTabPage.Location = new System.Drawing.Point(4, 22);
      this.globalScopeTabPage.Name = "globalScopeTabPage";
      this.globalScopeTabPage.Size = new System.Drawing.Size(705, 441);
      this.globalScopeTabPage.TabIndex = 6;
      this.globalScopeTabPage.Text = "Global Scope";
      this.globalScopeTabPage.UseVisualStyleBackColor = true;
      // 
      // globalScopeView
      // 
      this.globalScopeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.globalScopeView.Content = null;
      this.globalScopeView.Location = new System.Drawing.Point(3, 3);
      this.globalScopeView.Name = "globalScopeView";
      this.globalScopeView.ReadOnly = false;
      this.globalScopeView.Size = new System.Drawing.Size(699, 435);
      this.globalScopeView.TabIndex = 0;
      // 
      // UserDefinedAlgorithmView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Name = "UserDefinedAlgorithmView";
      this.engineTabPage.ResumeLayout(false);
      this.engineTabPage.PerformLayout();
      this.operatorGraphTabPage.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.problemTabPage.ResumeLayout(false);
      this.resultsTabPage.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.globalScopeTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabPage globalScopeTabPage;
    private HeuristicLab.Core.Views.ScopeView globalScopeView;

  }
}
