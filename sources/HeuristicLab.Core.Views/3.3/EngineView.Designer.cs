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
      this.operatorGraphView = new HeuristicLab.Core.Views.OperatorGraphView();
      this.scopeView = new HeuristicLab.Core.Views.ScopeView();
      this.resetButton = new System.Windows.Forms.Button();
      this.stopButton = new System.Windows.Forms.Button();
      this.startButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.tabControl = new System.Windows.Forms.TabControl();
      this.operatorGraphTabPage = new System.Windows.Forms.TabPage();
      this.globalScopeTabPage = new System.Windows.Forms.TabPage();
      this.tabControl.SuspendLayout();
      this.operatorGraphTabPage.SuspendLayout();
      this.globalScopeTabPage.SuspendLayout();
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
      // operatorGraphView
      // 
      this.operatorGraphView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.operatorGraphView.Caption = "Operator Graph";
      this.operatorGraphView.Location = new System.Drawing.Point(6, 6);
      this.operatorGraphView.Name = "operatorGraphView";
      this.operatorGraphView.Size = new System.Drawing.Size(782, 573);
      this.operatorGraphView.TabIndex = 0;
      // 
      // scopeView
      // 
      this.scopeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.scopeView.Caption = "Scope";
      this.scopeView.Location = new System.Drawing.Point(6, 6);
      this.scopeView.Name = "scopeView";
      this.scopeView.Size = new System.Drawing.Size(782, 573);
      this.scopeView.TabIndex = 0;
      // 
      // resetButton
      // 
      this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.resetButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Restart;
      this.resetButton.Location = new System.Drawing.Point(58, 617);
      this.resetButton.Name = "resetButton";
      this.resetButton.Size = new System.Drawing.Size(23, 23);
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
      this.stopButton.Location = new System.Drawing.Point(29, 617);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(23, 23);
      this.stopButton.TabIndex = 2;
      this.toolTip.SetToolTip(this.stopButton, "Stop Engine");
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // startButton
      // 
      this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.startButton.Image = HeuristicLab.Common.Resources.VS2008ImageLibrary.Play;
      this.startButton.Location = new System.Drawing.Point(0, 617);
      this.startButton.Name = "startButton";
      this.startButton.Size = new System.Drawing.Size(23, 23);
      this.startButton.TabIndex = 1;
      this.toolTip.SetToolTip(this.startButton, "Start Engine");
      this.startButton.UseVisualStyleBackColor = true;
      this.startButton.Click += new System.EventHandler(this.startButton_Click);
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.operatorGraphTabPage);
      this.tabControl.Controls.Add(this.globalScopeTabPage);
      this.tabControl.Location = new System.Drawing.Point(0, 0);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(802, 611);
      this.tabControl.TabIndex = 0;
      // 
      // operatorGraphTabPage
      // 
      this.operatorGraphTabPage.Controls.Add(this.operatorGraphView);
      this.operatorGraphTabPage.Location = new System.Drawing.Point(4, 22);
      this.operatorGraphTabPage.Name = "operatorGraphTabPage";
      this.operatorGraphTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.operatorGraphTabPage.Size = new System.Drawing.Size(794, 585);
      this.operatorGraphTabPage.TabIndex = 0;
      this.operatorGraphTabPage.Text = "Operator Graph";
      this.operatorGraphTabPage.UseVisualStyleBackColor = true;
      // 
      // globalScopeTabPage
      // 
      this.globalScopeTabPage.Controls.Add(this.scopeView);
      this.globalScopeTabPage.Location = new System.Drawing.Point(4, 22);
      this.globalScopeTabPage.Name = "globalScopeTabPage";
      this.globalScopeTabPage.Padding = new System.Windows.Forms.Padding(3);
      this.globalScopeTabPage.Size = new System.Drawing.Size(794, 585);
      this.globalScopeTabPage.TabIndex = 1;
      this.globalScopeTabPage.Text = "Global Scope";
      this.globalScopeTabPage.UseVisualStyleBackColor = true;
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
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.TextBox executionTimeTextBox;
    protected System.Windows.Forms.Label executionTimeLabel;
    protected System.Windows.Forms.Button resetButton;
    protected System.Windows.Forms.Button stopButton;
    protected System.Windows.Forms.Button startButton;
    protected HeuristicLab.Core.Views.OperatorGraphView operatorGraphView;
    protected HeuristicLab.Core.Views.ScopeView scopeView;
    protected System.Windows.Forms.ToolTip toolTip;
    protected System.Windows.Forms.TabControl tabControl;
    protected System.Windows.Forms.TabPage operatorGraphTabPage;
    protected System.Windows.Forms.TabPage globalScopeTabPage;

  }
}
