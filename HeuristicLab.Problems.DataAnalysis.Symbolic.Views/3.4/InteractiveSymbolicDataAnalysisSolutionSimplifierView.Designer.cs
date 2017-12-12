#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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


namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class InteractiveSymbolicDataAnalysisSolutionSimplifierView {
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InteractiveSymbolicDataAnalysisSolutionSimplifierView));
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.grpSimplify = new System.Windows.Forms.GroupBox();
      this.treeStatusValue = new System.Windows.Forms.Label();
      this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
      this.btnSimplify = new System.Windows.Forms.Button();
      this.btnOptimizeConstants = new System.Windows.Forms.Button();
      this.grpViewHost = new System.Windows.Forms.GroupBox();
      this.treeChart = new HeuristicLab.Problems.DataAnalysis.Symbolic.Views.InteractiveSymbolicExpressionTreeChart();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.grpSimplify.SuspendLayout();
      this.flowLayoutPanel.SuspendLayout();
      this.grpViewHost.SuspendLayout();
      this.SuspendLayout();
      // 
      // viewHost
      // 
      this.viewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(6, 16);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(336, 383);
      this.viewHost.TabIndex = 0;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.grpSimplify);
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.grpViewHost);
      this.splitContainer.Size = new System.Drawing.Size(565, 405);
      this.splitContainer.SplitterDistance = 213;
      this.splitContainer.TabIndex = 1;
      // 
      // grpSimplify
      // 
      this.grpSimplify.AutoSize = true;
      this.grpSimplify.Controls.Add(this.treeStatusValue);
      this.grpSimplify.Controls.Add(this.flowLayoutPanel);
      this.grpSimplify.Controls.Add(this.treeChart);
      this.grpSimplify.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grpSimplify.Location = new System.Drawing.Point(0, 0);
      this.grpSimplify.Name = "grpSimplify";
      this.grpSimplify.Size = new System.Drawing.Size(213, 405);
      this.grpSimplify.TabIndex = 1;
      this.grpSimplify.TabStop = false;
      this.grpSimplify.Text = "Simplify";
      // 
      // treeStatusValue
      // 
      this.treeStatusValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.treeStatusValue.AutoSize = true;
      this.treeStatusValue.BackColor = System.Drawing.Color.White;
      this.treeStatusValue.ForeColor = System.Drawing.Color.Red;
      this.treeStatusValue.Location = new System.Drawing.Point(6, 351);
      this.treeStatusValue.Name = "treeStatusValue";
      this.treeStatusValue.Size = new System.Drawing.Size(63, 13);
      this.treeStatusValue.TabIndex = 3;
      this.treeStatusValue.Text = "Invalid Tree";
      this.treeStatusValue.Visible = false;
      // 
      // flowLayoutPanel
      // 
      this.flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.flowLayoutPanel.Controls.Add(this.btnSimplify);
      this.flowLayoutPanel.Controls.Add(this.btnOptimizeConstants);
      this.flowLayoutPanel.Location = new System.Drawing.Point(6, 370);
      this.flowLayoutPanel.Name = "flowLayoutPanel";
      this.flowLayoutPanel.Size = new System.Drawing.Size(204, 29);
      this.flowLayoutPanel.TabIndex = 2;
      this.flowLayoutPanel.WrapContents = false;
      // 
      // btnSimplify
      // 
      this.btnSimplify.AutoSize = true;
      this.btnSimplify.Image = HeuristicLab.Common.Resources.VSImageLibrary.FormulaEvaluator;
      this.btnSimplify.Location = new System.Drawing.Point(3, 3);
      this.btnSimplify.Name = "btnSimplify";
      this.btnSimplify.Size = new System.Drawing.Size(80, 24);
      this.btnSimplify.TabIndex = 1;
      this.btnSimplify.Text = "Simplify";
      this.btnSimplify.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.btnSimplify.UseVisualStyleBackColor = true;
      this.btnSimplify.Click += new System.EventHandler(this.btnSimplify_Click);
      // 
      // btnOptimizeConstants
      // 
      this.btnOptimizeConstants.AutoSize = true;
      this.btnOptimizeConstants.Enabled = false;
      this.btnOptimizeConstants.Image = HeuristicLab.Common.Resources.VSImageLibrary.Performance;
      this.btnOptimizeConstants.Location = new System.Drawing.Point(89, 3);
      this.btnOptimizeConstants.Name = "btnOptimizeConstants";
      this.btnOptimizeConstants.Size = new System.Drawing.Size(80, 24);
      this.btnOptimizeConstants.TabIndex = 2;
      this.btnOptimizeConstants.Text = "Optimize";
      this.btnOptimizeConstants.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.btnOptimizeConstants.UseVisualStyleBackColor = true;
      this.btnOptimizeConstants.Click += new System.EventHandler(this.btnOptimizeConstants_Click);
      // 
      // grpViewHost
      // 
      this.grpViewHost.Controls.Add(this.viewHost);
      this.grpViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grpViewHost.Location = new System.Drawing.Point(0, 0);
      this.grpViewHost.Name = "grpViewHost";
      this.grpViewHost.Size = new System.Drawing.Size(348, 405);
      this.grpViewHost.TabIndex = 1;
      this.grpViewHost.TabStop = false;
      this.grpViewHost.Text = "Details";
      // 
      // treeChart
      // 
      this.treeChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.treeChart.BackgroundColor = System.Drawing.Color.White;
      this.treeChart.LineColor = System.Drawing.Color.Black;
      this.treeChart.Location = new System.Drawing.Point(6, 16);
      this.treeChart.ModifyTree = null;
      this.treeChart.Name = "treeChart";
      this.treeChart.Size = new System.Drawing.Size(201, 348);
      this.treeChart.SuspendRepaint = false;
      this.treeChart.TabIndex = 0;
      this.treeChart.TextFont = new System.Drawing.Font("Times New Roman", 8F);
      this.treeChart.Tree = null;
      this.treeChart.SymbolicExpressionTreeNodeDoubleClicked += new System.Windows.Forms.MouseEventHandler(this.treeChart_SymbolicExpressionTreeNodeDoubleClicked);
      // 
      // InteractiveSymbolicDataAnalysisSolutionSimplifierView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.DoubleBuffered = true;
      this.Name = "InteractiveSymbolicDataAnalysisSolutionSimplifierView";
      this.Size = new System.Drawing.Size(565, 405);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.grpSimplify.ResumeLayout(false);
      this.grpSimplify.PerformLayout();
      this.flowLayoutPanel.ResumeLayout(false);
      this.flowLayoutPanel.PerformLayout();
      this.grpViewHost.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private HeuristicLab.Problems.DataAnalysis.Symbolic.Views.InteractiveSymbolicExpressionTreeChart treeChart;
    private System.Windows.Forms.SplitContainer splitContainer;
    private HeuristicLab.MainForm.WindowsForms.ViewHost viewHost;
    private System.Windows.Forms.GroupBox grpSimplify;
    private System.Windows.Forms.GroupBox grpViewHost;
    private System.Windows.Forms.Button btnSimplify;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
    protected System.Windows.Forms.Button btnOptimizeConstants;
    private System.Windows.Forms.Label treeStatusValue;
  }
}
