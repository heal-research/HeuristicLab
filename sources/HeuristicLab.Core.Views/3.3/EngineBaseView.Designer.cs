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
  partial class EngineBaseView {
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
      this.executionTimeTextBox = new System.Windows.Forms.TextBox();
      this.executionTimeLabel = new System.Windows.Forms.Label();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.operatorGraphGroupBox = new System.Windows.Forms.GroupBox();
      this.operatorGraphView = new HeuristicLab.Core.Views.OperatorGraphView();
      this.globalScopeGroupBox = new System.Windows.Forms.GroupBox();
      this.scopeView = new HeuristicLab.Core.Views.ScopeView();
      this.resetButton = new System.Windows.Forms.Button();
      this.abortButton = new System.Windows.Forms.Button();
      this.executeButton = new System.Windows.Forms.Button();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.operatorGraphGroupBox.SuspendLayout();
      this.globalScopeGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // executionTimeTextBox
      // 
      this.executionTimeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeTextBox.Location = new System.Drawing.Point(499, 460);
      this.executionTimeTextBox.Name = "executionTimeTextBox";
      this.executionTimeTextBox.ReadOnly = true;
      this.executionTimeTextBox.Size = new System.Drawing.Size(141, 20);
      this.executionTimeTextBox.TabIndex = 5;
      // 
      // executionTimeLabel
      // 
      this.executionTimeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.executionTimeLabel.AutoSize = true;
      this.executionTimeLabel.Location = new System.Drawing.Point(496, 444);
      this.executionTimeLabel.Name = "executionTimeLabel";
      this.executionTimeLabel.Size = new System.Drawing.Size(83, 13);
      this.executionTimeLabel.TabIndex = 4;
      this.executionTimeLabel.Text = "&Execution Time:";
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.Location = new System.Drawing.Point(0, 0);
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.operatorGraphGroupBox);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.globalScopeGroupBox);
      this.splitContainer1.Size = new System.Drawing.Size(640, 441);
      this.splitContainer1.SplitterDistance = 320;
      this.splitContainer1.TabIndex = 0;
      // 
      // operatorGraphGroupBox
      // 
      this.operatorGraphGroupBox.Controls.Add(this.operatorGraphView);
      this.operatorGraphGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorGraphGroupBox.Location = new System.Drawing.Point(0, 0);
      this.operatorGraphGroupBox.Name = "operatorGraphGroupBox";
      this.operatorGraphGroupBox.Size = new System.Drawing.Size(320, 441);
      this.operatorGraphGroupBox.TabIndex = 0;
      this.operatorGraphGroupBox.TabStop = false;
      this.operatorGraphGroupBox.Text = "Operator &Graph";
      // 
      // operatorGraphView
      // 
      this.operatorGraphView.Caption = "Operator Graph";
      this.operatorGraphView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorGraphView.Location = new System.Drawing.Point(3, 16);
      this.operatorGraphView.Name = "operatorGraphView";
      this.operatorGraphView.OperatorGraph = null;
      this.operatorGraphView.Size = new System.Drawing.Size(314, 422);
      this.operatorGraphView.TabIndex = 0;
      // 
      // globalScopeGroupBox
      // 
      this.globalScopeGroupBox.Controls.Add(this.scopeView);
      this.globalScopeGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.globalScopeGroupBox.Location = new System.Drawing.Point(0, 0);
      this.globalScopeGroupBox.Name = "globalScopeGroupBox";
      this.globalScopeGroupBox.Size = new System.Drawing.Size(316, 441);
      this.globalScopeGroupBox.TabIndex = 0;
      this.globalScopeGroupBox.TabStop = false;
      this.globalScopeGroupBox.Text = "Global &Scope";
      // 
      // scopeView
      // 
      this.scopeView.Caption = "Scope";
      this.scopeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.scopeView.Location = new System.Drawing.Point(3, 16);
      this.scopeView.Name = "scopeView";
      this.scopeView.Scope = null;
      this.scopeView.Size = new System.Drawing.Size(310, 422);
      this.scopeView.TabIndex = 0;
      // 
      // resetButton
      // 
      this.resetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.resetButton.Location = new System.Drawing.Point(196, 457);
      this.resetButton.Name = "resetButton";
      this.resetButton.Size = new System.Drawing.Size(92, 23);
      this.resetButton.TabIndex = 3;
      this.resetButton.Text = "&Reset";
      this.resetButton.UseVisualStyleBackColor = true;
      this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
      // 
      // abortButton
      // 
      this.abortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.abortButton.Enabled = false;
      this.abortButton.Location = new System.Drawing.Point(98, 457);
      this.abortButton.Name = "abortButton";
      this.abortButton.Size = new System.Drawing.Size(92, 23);
      this.abortButton.TabIndex = 2;
      this.abortButton.Text = "&Abort";
      this.abortButton.UseVisualStyleBackColor = true;
      this.abortButton.Click += new System.EventHandler(this.abortButton_Click);
      // 
      // executeButton
      // 
      this.executeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.executeButton.Location = new System.Drawing.Point(0, 457);
      this.executeButton.Name = "executeButton";
      this.executeButton.Size = new System.Drawing.Size(92, 23);
      this.executeButton.TabIndex = 1;
      this.executeButton.Text = "&Execute";
      this.executeButton.UseVisualStyleBackColor = true;
      this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
      // 
      // EngineBaseEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.executionTimeTextBox);
      this.Controls.Add(this.executionTimeLabel);
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.resetButton);
      this.Controls.Add(this.abortButton);
      this.Controls.Add(this.executeButton);
      this.Name = "EngineBaseEditor";
      this.Size = new System.Drawing.Size(640, 480);
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.ResumeLayout(false);
      this.operatorGraphGroupBox.ResumeLayout(false);
      this.globalScopeGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.TextBox executionTimeTextBox;
    protected System.Windows.Forms.Label executionTimeLabel;
    protected System.Windows.Forms.SplitContainer splitContainer1;
    protected System.Windows.Forms.GroupBox operatorGraphGroupBox;
    protected System.Windows.Forms.GroupBox globalScopeGroupBox;
    protected System.Windows.Forms.Button resetButton;
    protected System.Windows.Forms.Button abortButton;
    protected System.Windows.Forms.Button executeButton;
    protected HeuristicLab.Core.Views.OperatorGraphView operatorGraphView;
    protected HeuristicLab.Core.Views.ScopeView scopeView;

  }
}
