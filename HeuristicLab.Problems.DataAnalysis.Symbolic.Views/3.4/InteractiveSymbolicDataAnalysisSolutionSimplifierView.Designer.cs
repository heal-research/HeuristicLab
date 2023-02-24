#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      this.tabControl = new System.Windows.Forms.TabControl();
      this.tabPageTree = new System.Windows.Forms.TabPage();
      this.panelTree = new System.Windows.Forms.Panel();
      this.treeStatusValue = new System.Windows.Forms.Label();
      this.treeChart = new HeuristicLab.Problems.DataAnalysis.Symbolic.Views.InteractiveSymbolicExpressionTreeChart();
      this.tabPageParameter = new System.Windows.Forms.TabPage();
      this.parametersViewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
      this.btnSimplify = new System.Windows.Forms.Button();
      this.btnOptimizeParameters = new System.Windows.Forms.Button();
      this.grpViewHost = new System.Windows.Forms.GroupBox();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.grpSimplify.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.tabPageTree.SuspendLayout();
      this.panelTree.SuspendLayout();
      this.tabPageParameter.SuspendLayout();
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
      this.viewHost.Size = new System.Drawing.Size(372, 466);
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
      this.splitContainer.Size = new System.Drawing.Size(649, 488);
      this.splitContainer.SplitterDistance = 261;
      this.splitContainer.TabIndex = 1;
      // 
      // grpSimplify
      // 
      this.grpSimplify.AutoSize = true;
      this.grpSimplify.Controls.Add(this.tabControl);
      this.grpSimplify.Controls.Add(this.flowLayoutPanel);
      this.grpSimplify.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grpSimplify.Location = new System.Drawing.Point(0, 0);
      this.grpSimplify.Name = "grpSimplify";
      this.grpSimplify.Size = new System.Drawing.Size(261, 488);
      this.grpSimplify.TabIndex = 1;
      this.grpSimplify.TabStop = false;
      this.grpSimplify.Text = "Simplify";
      // 
      // tabControl
      // 
      this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControl.Controls.Add(this.tabPageTree);
      this.tabControl.Controls.Add(this.tabPageParameter);
      this.tabControl.Location = new System.Drawing.Point(6, 16);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(249, 434);
      this.tabControl.TabIndex = 4;
      // 
      // tabPageTree
      // 
      this.tabPageTree.Controls.Add(this.panelTree);
      this.tabPageTree.Location = new System.Drawing.Point(4, 22);
      this.tabPageTree.Name = "tabPageTree";
      this.tabPageTree.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageTree.Size = new System.Drawing.Size(241, 408);
      this.tabPageTree.TabIndex = 0;
      this.tabPageTree.Text = "Tree";
      this.tabPageTree.UseVisualStyleBackColor = true;
      // 
      // panelTree
      // 
      this.panelTree.Controls.Add(this.treeStatusValue);
      this.panelTree.Controls.Add(this.treeChart);
      this.panelTree.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panelTree.Location = new System.Drawing.Point(3, 3);
      this.panelTree.Name = "panelTree";
      this.panelTree.Size = new System.Drawing.Size(235, 402);
      this.panelTree.TabIndex = 4;
      // 
      // treeStatusValue
      // 
      this.treeStatusValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.treeStatusValue.AutoSize = true;
      this.treeStatusValue.BackColor = System.Drawing.Color.White;
      this.treeStatusValue.ForeColor = System.Drawing.Color.Red;
      this.treeStatusValue.Location = new System.Drawing.Point(0, 386);
      this.treeStatusValue.Name = "treeStatusValue";
      this.treeStatusValue.Size = new System.Drawing.Size(63, 13);
      this.treeStatusValue.TabIndex = 3;
      this.treeStatusValue.Text = "Invalid Tree";
      this.treeStatusValue.Visible = false;
      // 
      // treeChart
      // 
      this.treeChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.treeChart.BackgroundColor = System.Drawing.Color.White;
      this.treeChart.LineColor = System.Drawing.Color.Black;
      this.treeChart.Location = new System.Drawing.Point(3, 3);
      this.treeChart.MinimumHorizontalDistance = 30;
      this.treeChart.MinimumHorizontalPadding = 20;
      this.treeChart.MinimumVerticalDistance = 30;
      this.treeChart.MinimumVerticalPadding = 20;
      this.treeChart.ModifyTree = null;
      this.treeChart.Name = "treeChart";
      this.treeChart.PreferredNodeHeight = 46;
      this.treeChart.PreferredNodeWidth = 70;
      this.treeChart.Size = new System.Drawing.Size(229, 396);
      this.treeChart.SuspendRepaint = false;
      this.treeChart.TabIndex = 0;
      this.treeChart.TextFont = new System.Drawing.Font("Times New Roman", 8F);
      this.treeChart.Tree = null;
      this.treeChart.SymbolicExpressionTreeNodeDoubleClicked += new System.Windows.Forms.MouseEventHandler(this.treeChart_SymbolicExpressionTreeNodeDoubleClicked);
      // 
      // tabPageParameter
      // 
      this.tabPageParameter.Controls.Add(this.parametersViewHost);
      this.tabPageParameter.Location = new System.Drawing.Point(4, 22);
      this.tabPageParameter.Name = "tabPageParameter";
      this.tabPageParameter.Padding = new System.Windows.Forms.Padding(3);
      this.tabPageParameter.Size = new System.Drawing.Size(241, 408);
      this.tabPageParameter.TabIndex = 1;
      this.tabPageParameter.Text = "Parameters";
      this.tabPageParameter.UseVisualStyleBackColor = true;
      // 
      // parametersViewHost
      // 
      this.parametersViewHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.parametersViewHost.Caption = "View";
      this.parametersViewHost.Content = null;
      this.parametersViewHost.Enabled = false;
      this.parametersViewHost.Location = new System.Drawing.Point(6, 6);
      this.parametersViewHost.Name = "parametersViewHost";
      this.parametersViewHost.ReadOnly = false;
      this.parametersViewHost.Size = new System.Drawing.Size(229, 396);
      this.parametersViewHost.TabIndex = 0;
      this.parametersViewHost.ViewsLabelVisible = true;
      this.parametersViewHost.ViewType = null;
      // 
      // flowLayoutPanel
      // 
      this.flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.flowLayoutPanel.Controls.Add(this.btnSimplify);
      this.flowLayoutPanel.Controls.Add(this.btnOptimizeParameters);
      this.flowLayoutPanel.Location = new System.Drawing.Point(6, 453);
      this.flowLayoutPanel.Name = "flowLayoutPanel";
      this.flowLayoutPanel.Size = new System.Drawing.Size(249, 29);
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
      this.toolTip.SetToolTip(this.btnSimplify, "Simplifies the model structure based on mathematical simplification rules.");
      this.btnSimplify.UseVisualStyleBackColor = true;
      this.btnSimplify.Click += new System.EventHandler(this.btnSimplify_Click);
      // 
      // btnOptimizeParameters
      // 
      this.btnOptimizeParameters.AutoSize = true;
      this.btnOptimizeParameters.Enabled = false;
      this.btnOptimizeParameters.Image = HeuristicLab.Common.Resources.VSImageLibrary.Performance;
      this.btnOptimizeParameters.Location = new System.Drawing.Point(89, 3);
      this.btnOptimizeParameters.Name = "btnOptimizeParameters";
      this.btnOptimizeParameters.Size = new System.Drawing.Size(80, 24);
      this.btnOptimizeParameters.TabIndex = 2;
      this.btnOptimizeParameters.Text = "Optimize";
      this.btnOptimizeParameters.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
      this.toolTip.SetToolTip(this.btnOptimizeParameters, "Optimizes the parameters of the model. \r\nIf the algorithm converges, opt" +
        "imization is stopped.");
      this.btnOptimizeParameters.UseVisualStyleBackColor = true;
      this.btnOptimizeParameters.Click += new System.EventHandler(this.btnOptimizeParameters_Click);
      // 
      // grpViewHost
      // 
      this.grpViewHost.Controls.Add(this.viewHost);
      this.grpViewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grpViewHost.Location = new System.Drawing.Point(0, 0);
      this.grpViewHost.Name = "grpViewHost";
      this.grpViewHost.Size = new System.Drawing.Size(384, 488);
      this.grpViewHost.TabIndex = 1;
      this.grpViewHost.TabStop = false;
      this.grpViewHost.Text = "Details";
      // 
      // InteractiveSymbolicDataAnalysisSolutionSimplifierView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.DoubleBuffered = true;
      this.Name = "InteractiveSymbolicDataAnalysisSolutionSimplifierView";
      this.Size = new System.Drawing.Size(649, 488);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.grpSimplify.ResumeLayout(false);
      this.tabControl.ResumeLayout(false);
      this.tabPageTree.ResumeLayout(false);
      this.panelTree.ResumeLayout(false);
      this.panelTree.PerformLayout();
      this.tabPageParameter.ResumeLayout(false);
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
    protected System.Windows.Forms.Button btnOptimizeParameters;
    private System.Windows.Forms.Label treeStatusValue;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage tabPageTree;
    private System.Windows.Forms.Panel panelTree;
    private System.Windows.Forms.TabPage tabPageParameter;
    private MainForm.WindowsForms.ViewHost parametersViewHost;
  }
}
