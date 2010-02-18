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

namespace HeuristicLab.Core.Views {
  partial class EngineView {
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
        if (typeSelectorDialog != null) typeSelectorDialog.Dispose();
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
      this.components = new System.ComponentModel.Container();
      this.executionTimeTextBox = new System.Windows.Forms.TextBox();
      this.executionTimeLabel = new System.Windows.Forms.Label();
      this.scopeView = new HeuristicLab.Core.Views.ScopeView();
      this.resetButton = new System.Windows.Forms.Button();
      this.stopButton = new System.Windows.Forms.Button();
      this.startButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.newProblemButton = new System.Windows.Forms.Button();
      this.openProblemButton = new System.Windows.Forms.Button();
      this.saveProblemButton = new System.Windows.Forms.Button();
      this.newOperatorGraphButton = new System.Windows.Forms.Button();
      this.openOperatorGraphButton = new System.Windows.Forms.Button();
      this.saveOperatorGraphButton = new System.Windows.Forms.Button();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.operatorGraphTabPage = new System.Windows.Forms.TabPage();
      this.operatorGraphViewHost = new HeuristicLab.Core.Views.ViewHost();
      this.globalScopeTabPage = new System.Windows.Forms.TabPage();
      this.problemTabPage = new System.Windows.Forms.TabPage();
      this.problemViewHost = new HeuristicLab.Core.Views.ViewHost();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
      this.tabControl.SuspendLayout();
      this.operatorGraphTabPage.SuspendLayout();
      this.globalScopeTabPage.SuspendLayout();
      this.problemTabPage.SuspendLayout();
      this.SuspendLayout();
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeTextBox.Location = new System.Drawing.Point(661, 620);
      this.executionTimeTextBox.Name = "executionTimeTextBox";
      this.executionTimeTextBox.ReadOnly = true;
      this.executionTimeTextBox.Size = new System.Drawing.Size(141, 20);
      this.executionTimeTextBox.TabIndex = 5;
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeLabel.AutoSize = true;
      this.executionTimeLabel.Location = new System.Drawing.Point(572, 623);
      this.executionTimeLabel.Name = "executionTimeLabel";
      this.executionTimeLabel.Size = new System.Drawing.Size(83, 13);
      this.executionTimeLabel.TabIndex = 4;
      this.executionTimeLabel.Text = "&Execution Time:";
      // 
      // scopeView
      // 
      this.scopeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.scopeView.Caption = "Scope";
      this.scopeView.Content = null;
      this.scopeView.Location = new System.Drawing.Point(6, 6);
      this.scopeView.Name = "scopeView";
      this.scopeView.Size = new System.Drawing.Size(782, 572);
      this.scopeView.TabIndex = 0;
      // 
      // resetButton
      // 
      this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.resetButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Restart;
      this.resetButton.Location = new System.Drawing.Point(60, 616);
      this.resetButton.Name = "resetButton";
      this.resetButton.Size = new System.Drawing.Size(24, 24);
      this.resetButton.TabIndex = 3;
      this.toolTip.SetToolTip(this.resetButton, "Reset Engine");
      this.resetButton.UseVisualStyleBackColor = true;
      this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
      // 
      // stopButton
      // 
      this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stopButton.Enabled = false;
      this.stopButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Stop;
      this.stopButton.Location = new System.Drawing.Point(30, 616);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(24, 24);
      this.stopButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.stopButton, "Stop Engine");
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.startButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Play;
      this.startButton.Location = new System.Drawing.Point(0, 616);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(24, 24);
      this.startButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.startButton, "Start Engine");
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // newProblemButton
      // 
      this.newProblemButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.NewDocument;
      this.newProblemButton.Location = new System.Drawing.Point(6, 6);
      this.newProblemButton.Name = "newProblemButton";
      this.newProblemButton.Size = new System.Drawing.Size(24, 24);
      this.newProblemButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.newProblemButton, "Create New Problem");
      this.newProblemButton.UseVisualStyleBackColor = true;
      this.newProblemButton.Click += new System.EventHandler(this.newProblemButton_Click);
      // 
      // openProblemButton
      // 
      this.openProblemButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Open;
      this.openProblemButton.Location = new System.Drawing.Point(36, 6);
      this.openProblemButton.Name = "openProblemButton";
      this.openProblemButton.Size = new System.Drawing.Size(24, 24);
      this.openProblemButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.openProblemButton, "Open Problem");
      this.openProblemButton.UseVisualStyleBackColor = true;
      this.openProblemButton.Click += new System.EventHandler(this.openProblemButton_Click);
      // 
      // saveProblemButton
      // 
      this.saveProblemButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Save;
      this.saveProblemButton.Location = new System.Drawing.Point(66, 6);
      this.saveProblemButton.Name = "saveProblemButton";
      this.saveProblemButton.Size = new System.Drawing.Size(24, 24);
      this.saveProblemButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.saveProblemButton, "Save Problem");
      this.saveProblemButton.UseVisualStyleBackColor = true;
      this.saveProblemButton.Click += new System.EventHandler(this.saveProblemButton_Click);
      // 
      // newOperatorGraphButton
      // 
      this.newOperatorGraphButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.NewDocument;
      this.newOperatorGraphButton.Location = new System.Drawing.Point(6, 6);
      this.newOperatorGraphButton.Name = "newOperatorGraphButton";
      this.newOperatorGraphButton.Size = new System.Drawing.Size(24, 24);
      this.newOperatorGraphButton.TabIndex = 0;
      this.toolTip.SetToolTip(this.newOperatorGraphButton, "Create New Operator Graph");
      this.newOperatorGraphButton.UseVisualStyleBackColor = true;
      this.newOperatorGraphButton.Click += new System.EventHandler(this.newOperatorGraphButton_Click);
      // 
      // openOperatorGraphButton
      // 
      this.openOperatorGraphButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Open;
      this.openOperatorGraphButton.Location = new System.Drawing.Point(36, 6);
      this.openOperatorGraphButton.Name = "openOperatorGraphButton";
      this.openOperatorGraphButton.Size = new System.Drawing.Size(24, 24);
      this.openOperatorGraphButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.openOperatorGraphButton, "Open Operator Graph");
      this.openOperatorGraphButton.UseVisualStyleBackColor = true;
      this.openOperatorGraphButton.Click += new System.EventHandler(this.openOperatorGraphButton_Click);
      // 
      // saveOperatorGraphButton
      // 
      this.saveOperatorGraphButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Save;
      this.saveOperatorGraphButton.Location = new System.Drawing.Point(66, 6);
      this.saveOperatorGraphButton.Name = "saveOperatorGraphButton";
      this.saveOperatorGraphButton.Size = new System.Drawing.Size(24, 24);
      this.saveOperatorGraphButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.saveOperatorGraphButton, "Save Operator Graph");
      this.saveOperatorGraphButton.UseVisualStyleBackColor = true;
      this.saveOperatorGraphButton.Click += new System.EventHandler(this.saveOperatorGraphButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.operatorGraphTabPage);
      this.tabControl.Controls.Add(this.globalScopeTabPage);
      this.tabControl.Controls.Add(this.problemTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(802, 610);
      this.tabControl.TabIndex = 3;
      // 
      // operatorGraphTabPage
      // 
      this.operatorGraphTabPage.Controls.Add(this.operatorGraphViewHost);
      this.operatorGraphTabPage.Controls.Add(this.saveOperatorGraphButton);
      this.operatorGraphTabPage.Controls.Add(this.openOperatorGraphButton);
      this.operatorGraphTabPage.Controls.Add(this.newOperatorGraphButton);
      this.operatorGraphTabPage.Location = new System.Drawing.Point(4, 22);
      this.operatorGraphTabPage.Name = "operatorGraphTabPage";
      this.operatorGraphTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.operatorGraphTabPage.Size = new System.Drawing.Size(794, 584);
      this.operatorGraphTabPage.TabIndex = 0;
      this.operatorGraphTabPage.Text = "Operator Graph";
      this.operatorGraphTabPage.UseVisualStyleBackColor = true;
      // 
      // operatorGraphViewHost
      // 
      this.operatorGraphViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorGraphViewHost.Content = null;
      this.operatorGraphViewHost.Location = new System.Drawing.Point(6, 36);
      this.operatorGraphViewHost.Name = "operatorGraphViewHost";
      this.operatorGraphViewHost.Size = new System.Drawing.Size(782, 542);
      this.operatorGraphViewHost.TabIndex = 3;
      this.operatorGraphViewHost.ViewType = null;
      // 
      // globalScopeTabPage
      // 
      this.globalScopeTabPage.Controls.Add(this.scopeView);
      this.globalScopeTabPage.Location = new System.Drawing.Point(4, 22);
      this.globalScopeTabPage.Name = "globalScopeTabPage";
      this.globalScopeTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.globalScopeTabPage.Size = new System.Drawing.Size(794, 584);
      this.globalScopeTabPage.TabIndex = 1;
      this.globalScopeTabPage.Text = "Global Scope";
      this.globalScopeTabPage.UseVisualStyleBackColor = true;
      // 
      // problemTabPage
      // 
      this.problemTabPage.Controls.Add(this.saveProblemButton);
      this.problemTabPage.Controls.Add(this.openProblemButton);
      this.problemTabPage.Controls.Add(this.newProblemButton);
      this.problemTabPage.Controls.Add(this.problemViewHost);
      this.problemTabPage.Location = new System.Drawing.Point(4, 22);
      this.problemTabPage.Name = "problemTabPage";
      this.problemTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.problemTabPage.Size = new System.Drawing.Size(794, 584);
      this.problemTabPage.TabIndex = 2;
      this.problemTabPage.Text = "Problem";
      this.problemTabPage.UseVisualStyleBackColor = true;
      // 
      // problemViewHost
      // 
      this.problemViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.problemViewHost.Content = null;
      this.problemViewHost.Location = new System.Drawing.Point(6, 36);
      this.problemViewHost.Name = "problemViewHost";
      this.problemViewHost.Size = new System.Drawing.Size(782, 542);
      this.problemViewHost.TabIndex = 3;
      this.problemViewHost.ViewType = null;
      // 
      // openFileDialog
      // 
      this.openFileDialog.DefaultExt = "hl";
      this.openFileDialog.FileName = "Item";
      this.openFileDialog.Filter = "HeuristicLab Files|*.hl|All Files|*.*";
      this.openFileDialog.Title = "Open File";
      // 
      // saveFileDialog
      // 
      this.saveFileDialog.DefaultExt = "hl";
      this.saveFileDialog.FileName = "Item";
      this.saveFileDialog.Filter = "Uncompressed HeuristicLab Files|*.hl|HeuristicLab Files|*.hl|All Files|*.*";
      this.saveFileDialog.FilterIndex = 2;
      this.saveFileDialog.Title = "Save File";
      // 
      // EngineView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.executionTimeTextBox);
      this.Controls.Add(this.stopButton);
      this.Controls.Add(this.executionTimeLabel);
      this.Controls.Add(this.resetButton);
      this.Controls.Add(this.startButton);
      this.Name = "EngineView";
      this.Size = new System.Drawing.Size(802, 640);
      this.tabControl.ResumeLayout(false);
      this.operatorGraphTabPage.ResumeLayout(false);
      this.globalScopeTabPage.ResumeLayout(false);
      this.problemTabPage.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.TextBox executionTimeTextBox;
    protected System.Windows.Forms.Label executionTimeLabel;
    protected System.Windows.Forms.Button resetButton;
    protected System.Windows.Forms.Button stopButton;
    protected System.Windows.Forms.Button startButton;
    protected HeuristicLab.Core.Views.ScopeView scopeView;
    protected System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.TabControl tabControl;
    protected System.Windows.Forms.TabPage globalScopeTabPage;
    protected System.Windows.Forms.TabPage problemTabPage;
    protected ViewHost problemViewHost;
    protected System.Windows.Forms.Button newProblemButton;
    protected System.Windows.Forms.Button saveProblemButton;
    protected System.Windows.Forms.Button openProblemButton;
    protected System.Windows.Forms.OpenFileDialog openFileDialog;
    protected System.Windows.Forms.SaveFileDialog saveFileDialog;
    protected System.Windows.Forms.TabPage operatorGraphTabPage;
    protected System.Windows.Forms.Button saveOperatorGraphButton;
    protected System.Windows.Forms.Button openOperatorGraphButton;
    protected System.Windows.Forms.Button newOperatorGraphButton;
    protected ViewHost operatorGraphViewHost;

  }
}
