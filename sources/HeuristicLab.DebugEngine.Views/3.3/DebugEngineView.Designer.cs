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

namespace HeuristicLab.DebugEngine {
  partial class DebugEngineView {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugEngineView));
      this.operationContentView = new HeuristicLab.DebugEngine.OperationContentView();
      this.executionStackView = new HeuristicLab.DebugEngine.ExecutionStackView();
      this.splitContainer2 = new System.Windows.Forms.SplitContainer();
      this.refreshButton = new System.Windows.Forms.Button();
      this.skipStackOpsCheckBox = new System.Windows.Forms.CheckBox();
      this.stepButton = new System.Windows.Forms.Button();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.splitContainer3 = new System.Windows.Forms.SplitContainer();
      this.operatorTraceView = new HeuristicLab.DebugEngine.OperatorTraceView();
      this.splitContainer2.Panel1.SuspendLayout();
      this.splitContainer2.Panel2.SuspendLayout();
      this.splitContainer2.SuspendLayout();
      this.splitContainer3.Panel1.SuspendLayout();
      this.splitContainer3.Panel2.SuspendLayout();
      this.splitContainer3.SuspendLayout();
      this.SuspendLayout();
      // 
      // operationContentView
      // 
      this.operationContentView.Caption = "Operation Content View";
      this.operationContentView.Content = null;
      this.operationContentView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operationContentView.Location = new System.Drawing.Point(0, 0);
      this.operationContentView.Name = "operationContentView";
      this.operationContentView.ReadOnly = false;
      this.operationContentView.Size = new System.Drawing.Size(484, 538);
      this.operationContentView.TabIndex = 0;
      // 
      // executionStackView
      // 
      this.executionStackView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.executionStackView.Caption = "Execution Stack View";
      this.executionStackView.Content = null;
      this.executionStackView.Location = new System.Drawing.Point(0, 0);
      this.executionStackView.Name = "executionStackView";
      this.executionStackView.ReadOnly = false;
      this.executionStackView.Size = new System.Drawing.Size(197, 505);
      this.executionStackView.TabIndex = 0;
      // 
      // splitContainer2
      // 
      this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer2.Location = new System.Drawing.Point(0, 0);
      this.splitContainer2.Name = "splitContainer2";
      // 
      // splitContainer2.Panel1
      // 
      this.splitContainer2.Panel1.Controls.Add(this.executionStackView);
      this.splitContainer2.Panel1.Controls.Add(this.refreshButton);
      this.splitContainer2.Panel1.Controls.Add(this.skipStackOpsCheckBox);
      this.splitContainer2.Panel1.Controls.Add(this.stepButton);
      // 
      // splitContainer2.Panel2
      // 
      this.splitContainer2.Panel2.Controls.Add(this.operationContentView);
      this.splitContainer2.Size = new System.Drawing.Size(687, 538);
      this.splitContainer2.SplitterDistance = 199;
      this.splitContainer2.TabIndex = 0;
      // 
      // refreshButton
      // 
      this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.refreshButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Refresh;
      this.refreshButton.Location = new System.Drawing.Point(3, 511);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(24, 24);
      this.refreshButton.TabIndex = 5;
      this.toolTip.SetToolTip(this.refreshButton, "Refresh View");
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
      // 
      // skipStackOpsCheckBox
      // 
      this.skipStackOpsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.skipStackOpsCheckBox.AutoSize = true;
      this.skipStackOpsCheckBox.Checked = true;
      this.skipStackOpsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.skipStackOpsCheckBox.Location = new System.Drawing.Point(63, 516);
      this.skipStackOpsCheckBox.Name = "skipStackOpsCheckBox";
      this.skipStackOpsCheckBox.Size = new System.Drawing.Size(78, 17);
      this.skipStackOpsCheckBox.TabIndex = 6;
      this.skipStackOpsCheckBox.Text = "Skip Stack";
      this.toolTip.SetToolTip(this.skipStackOpsCheckBox, "Automatically step over steps that manipulate only the execution stack");
      this.skipStackOpsCheckBox.UseVisualStyleBackColor = true;
      // 
      // stepButton
      // 
      this.stepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.stepButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.MoveNext;
      this.stepButton.Location = new System.Drawing.Point(33, 511);
      this.stepButton.Name = "stepButton";
      this.stepButton.Size = new System.Drawing.Size(24, 24);
      this.stepButton.TabIndex = 4;
      this.toolTip.SetToolTip(this.stepButton, "Step");
      this.stepButton.UseVisualStyleBackColor = true;
      this.stepButton.Click += new System.EventHandler(this.stepButton_Click);
      // 
      // splitContainer3
      // 
      this.splitContainer3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer3.Location = new System.Drawing.Point(0, 0);
      this.splitContainer3.Name = "splitContainer3";
      // 
      // splitContainer3.Panel1
      // 
      this.splitContainer3.Panel1.Controls.Add(this.splitContainer2);
      // 
      // splitContainer3.Panel2
      // 
      this.splitContainer3.Panel2.Controls.Add(this.operatorTraceView);
      this.splitContainer3.Size = new System.Drawing.Size(872, 538);
      this.splitContainer3.SplitterDistance = 687;
      this.splitContainer3.TabIndex = 7;
      // 
      // operatorTraceView
      // 
      this.operatorTraceView.Caption = "Operator Trace View";
      this.operatorTraceView.Content = null;
      this.operatorTraceView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.operatorTraceView.Location = new System.Drawing.Point(0, 0);
      this.operatorTraceView.Name = "operatorTraceView";
      this.operatorTraceView.ReadOnly = false;
      this.operatorTraceView.Size = new System.Drawing.Size(181, 538);
      this.operatorTraceView.TabIndex = 0;
      // 
      // DebugEngineView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.splitContainer3);
      this.Name = "DebugEngineView";
      this.Size = new System.Drawing.Size(872, 538);
      this.splitContainer2.Panel1.ResumeLayout(false);
      this.splitContainer2.Panel1.PerformLayout();
      this.splitContainer2.Panel2.ResumeLayout(false);
      this.splitContainer2.ResumeLayout(false);
      this.splitContainer3.Panel1.ResumeLayout(false);
      this.splitContainer3.Panel2.ResumeLayout(false);
      this.splitContainer3.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.Button stepButton;
    private System.Windows.Forms.ToolTip toolTip;
    private HeuristicLab.DebugEngine.ExecutionStackView executionStackView;
    private HeuristicLab.DebugEngine.OperationContentView operationContentView;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.CheckBox skipStackOpsCheckBox;
    private System.Windows.Forms.SplitContainer splitContainer3;
    private OperatorTraceView operatorTraceView;

  }
}
