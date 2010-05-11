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

namespace HeuristicLab.Optimization.Views {
  partial class EngineAlgorithmView {
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
      this.engineLabel = new System.Windows.Forms.Label();
      this.engineComboBox = new System.Windows.Forms.ComboBox();
      this.engineTabPage = new System.Windows.Forms.TabPage();
      this.engineViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.operatorGraphTabPage = new System.Windows.Forms.TabPage();
      this.openOperatorGraphButton = new System.Windows.Forms.Button();
      this.newOperatorGraphButton = new System.Windows.Forms.Button();
      this.operatorGraphViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.tabControl.SuspendLayout();
      this.parametersTabPage.SuspendLayout();
      this.problemTabPage.SuspendLayout();
      this.resultsTabPage.SuspendLayout();
      this.runsTabPage.SuspendLayout();
      this.problemPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.engineTabPage.SuspendLayout();
      this.operatorGraphTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.operatorGraphTabPage);
      this.tabControl.Controls.Add(this.engineTabPage);
      this.tabControl.Size = new System.Drawing.Size(713, 467);
      this.tabControl.Controls.SetChildIndex(this.engineTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.operatorGraphTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.runsTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.resultsTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.parametersTabPage, 0);
      this.tabControl.Controls.SetChildIndex(this.problemTabPage, 0);
      // 
      // parametersTabPage
      // 
      this.parametersTabPage.Size = new System.Drawing.Size(705, 441);
      // 
      // problemTabPage
      // 
      this.problemTabPage.Size = new System.Drawing.Size(705, 441);
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Size = new System.Drawing.Size(693, 429);
      // 
      // problemViewHost
      // 
      this.problemViewHost.Size = new System.Drawing.Size(693, 399);
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
      this.startButton.Location = new System.Drawing.Point(0, 525);
      this.toolTip.SetToolTip(this.startButton, "Start/Resume Algorithm");
      // 
      // pauseButton
      // 
      this.pauseButton.Location = new System.Drawing.Point(30, 525);
      this.toolTip.SetToolTip(this.pauseButton, "Pause Algorithm");
      // 
      // resetButton
      // 
      this.resetButton.Location = new System.Drawing.Point(90, 525);
      this.toolTip.SetToolTip(this.resetButton, "Reset Algorithm");
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Location = new System.Drawing.Point(487, 532);
      this.executionTimeLabel.TabIndex = 9;
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Location = new System.Drawing.Point(576, 529);
      this.executionTimeTextBox.TabIndex = 10;
      // 
      // resultsTabPage
      // 
      this.resultsTabPage.Size = new System.Drawing.Size(705, 441);
      // 
      // resultsView
      // 
      this.resultsView.Size = new System.Drawing.Size(693, 429);
      // 
      // stopButton
      // 
      this.stopButton.Location = new System.Drawing.Point(60, 525);
      this.toolTip.SetToolTip(this.stopButton, "Stop Algorithm");
      // 
      // runsTabPage
      // 
      this.runsTabPage.Size = new System.Drawing.Size(705, 441);
      // 
      // runsView
      // 
      this.runsView.Size = new System.Drawing.Size(693, 429);
      // 
      // problemPanel
      // 
      this.problemPanel.Size = new System.Drawing.Size(693, 399);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(641, 20);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Size = new System.Drawing.Size(641, 20);
      // 
      // engineLabel
      // 
      this.engineLabel.AutoSize = true;
      this.engineLabel.Location = new System.Drawing.Point(6, 9);
      this.engineLabel.Name = "engineLabel";
      this.engineLabel.Size = new System.Drawing.Size(43, 13);
      this.engineLabel.TabIndex = 0;
      this.engineLabel.Text = "&Engine:";
      // 
      // engineComboBox
      // 
      this.engineComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.engineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.engineComboBox.FormattingEnabled = true;
      this.engineComboBox.Location = new System.Drawing.Point(55, 6);
      this.engineComboBox.Name = "engineComboBox";
      this.engineComboBox.Size = new System.Drawing.Size(644, 21);
      this.engineComboBox.TabIndex = 1;
      this.engineComboBox.SelectedIndexChanged += new System.EventHandler(this.engineComboBox_SelectedIndexChanged);
      // 
      // engineTabPage
      // 
      this.engineTabPage.Controls.Add(this.engineViewHost);
      this.engineTabPage.Controls.Add(this.engineComboBox);
      this.engineTabPage.Controls.Add(this.engineLabel);
      this.engineTabPage.Location = new System.Drawing.Point(4, 22);
      this.engineTabPage.Name = "engineTabPage";
      this.engineTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.engineTabPage.Size = new System.Drawing.Size(705, 441);
      this.engineTabPage.TabIndex = 5;
      this.engineTabPage.Text = "Engine";
      this.engineTabPage.UseVisualStyleBackColor = true;
      // 
      // engineViewHost
      // 
      this.engineViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.engineViewHost.Caption = null;
      this.engineViewHost.Content = null;
      this.engineViewHost.Location = new System.Drawing.Point(6, 33);
      this.engineViewHost.Name = "engineViewHost";
      this.engineViewHost.ReadOnly = false;
      this.engineViewHost.Size = new System.Drawing.Size(693, 402);
      this.engineViewHost.TabIndex = 2;
      this.engineViewHost.ViewType = null;
      // 
      // operatorGraphTabPage
      // 
      this.operatorGraphTabPage.Controls.Add(this.openOperatorGraphButton);
      this.operatorGraphTabPage.Controls.Add(this.newOperatorGraphButton);
      this.operatorGraphTabPage.Controls.Add(this.operatorGraphViewHost);
      this.operatorGraphTabPage.Location = new System.Drawing.Point(4, 22);
      this.operatorGraphTabPage.Name = "operatorGraphTabPage";
      this.operatorGraphTabPage.Size = new System.Drawing.Size(705, 441);
      this.operatorGraphTabPage.TabIndex = 4;
      this.operatorGraphTabPage.Text = "Operator Graph";
      this.operatorGraphTabPage.UseVisualStyleBackColor = true;
      // 
      // openOperatorGraphButton
      // 
      this.openOperatorGraphButton.Enabled = false;
      this.openOperatorGraphButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Open;
      this.openOperatorGraphButton.Location = new System.Drawing.Point(33, 3);
      this.openOperatorGraphButton.Name = "openOperatorGraphButton";
      this.openOperatorGraphButton.Size = new System.Drawing.Size(24, 24);
      this.openOperatorGraphButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.openOperatorGraphButton, "Open Operator Graph");
      this.openOperatorGraphButton.UseVisualStyleBackColor = true;
      // 
      // newOperatorGraphButton
      // 
      this.newOperatorGraphButton.Enabled = false;
      this.newOperatorGraphButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.NewDocument;
      this.newOperatorGraphButton.Location = new System.Drawing.Point(3, 3);
      this.newOperatorGraphButton.Name = "newOperatorGraphButton";
      this.newOperatorGraphButton.Size = new System.Drawing.Size(24, 24);
      this.newOperatorGraphButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.newOperatorGraphButton, "New Operator Graph");
      this.newOperatorGraphButton.UseVisualStyleBackColor = true;
      // 
      // operatorGraphViewHost
      // 
      this.operatorGraphViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorGraphViewHost.Caption = null;
      this.operatorGraphViewHost.Content = null;
      this.operatorGraphViewHost.Location = new System.Drawing.Point(3, 33);
      this.operatorGraphViewHost.Name = "operatorGraphViewHost";
      this.operatorGraphViewHost.ReadOnly = true;
      this.operatorGraphViewHost.Size = new System.Drawing.Size(699, 405);
      this.operatorGraphViewHost.TabIndex = 0;
      this.operatorGraphViewHost.ViewType = null;
      // 
      // EngineAlgorithmView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Name = "EngineAlgorithmView";
      this.Size = new System.Drawing.Size(713, 549);
      this.Controls.SetChildIndex(this.stopButton, 0);
      this.Controls.SetChildIndex(this.resetButton, 0);
      this.Controls.SetChildIndex(this.pauseButton, 0);
      this.Controls.SetChildIndex(this.executionTimeLabel, 0);
      this.Controls.SetChildIndex(this.executionTimeTextBox, 0);
      this.Controls.SetChildIndex(this.startButton, 0);
      this.Controls.SetChildIndex(this.tabControl, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      this.tabControl.ResumeLayout(false);
      this.parametersTabPage.ResumeLayout(false);
      this.problemTabPage.ResumeLayout(false);
      this.resultsTabPage.ResumeLayout(false);
      this.runsTabPage.ResumeLayout(false);
      this.problemPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.engineTabPage.ResumeLayout(false);
      this.engineTabPage.PerformLayout();
      this.operatorGraphTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Label engineLabel;
    protected System.Windows.Forms.ComboBox engineComboBox;
    protected System.Windows.Forms.TabPage engineTabPage;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost engineViewHost;
    protected System.Windows.Forms.TabPage operatorGraphTabPage;
    protected HeuristicLab.MainForm.WindowsForms.ViewHost operatorGraphViewHost;
    protected System.Windows.Forms.Button openOperatorGraphButton;
    protected System.Windows.Forms.Button newOperatorGraphButton;

  }
}
