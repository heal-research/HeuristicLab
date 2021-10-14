
using System.Drawing;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class StructureTemplateView {
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
      this.expressionInput = new System.Windows.Forms.TextBox();
      this.parseButton = new System.Windows.Forms.Button();
      this.errorLabel = new System.Windows.Forms.Label();
      this.symRegTreeChart = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.GraphicalSymbolicExpressionTreeView();
      this.templateStructureGroupBox = new System.Windows.Forms.GroupBox();
      this.treeChart = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionTreeChart();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.templateStructureGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // expressionInput
      // 
      this.expressionInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.expressionInput.Location = new System.Drawing.Point(6, 19);
      this.expressionInput.Name = "expressionInput";
      this.expressionInput.Size = new System.Drawing.Size(288, 20);
      this.expressionInput.TabIndex = 1;
      this.expressionInput.TextChanged += new System.EventHandler(this.expressionInput_TextChanged);
      // 
      // parseButton
      // 
      this.parseButton.Location = new System.Drawing.Point(6, 45);
      this.parseButton.Name = "parseButton";
      this.parseButton.Size = new System.Drawing.Size(143, 23);
      this.parseButton.TabIndex = 3;
      this.parseButton.Text = "Parse";
      this.parseButton.UseVisualStyleBackColor = true;
      this.parseButton.Click += new System.EventHandler(this.parseButton_Click);
      // 
      // errorLabel
      // 
      this.errorLabel.AutoSize = true;
      this.errorLabel.Location = new System.Drawing.Point(155, 50);
      this.errorLabel.Name = "errorLabel";
      this.errorLabel.Size = new System.Drawing.Size(54, 13);
      this.errorLabel.TabIndex = 4;
      this.errorLabel.Text = "errorLabel";
      // 
      // symRegTreeChart
      // 
      this.symRegTreeChart.AllowDrop = true;
      this.symRegTreeChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.symRegTreeChart.Caption = "Graphical SymbolicExpressionTree View";
      this.symRegTreeChart.Content = null;
      this.symRegTreeChart.Location = new System.Drawing.Point(6, 74);
      this.symRegTreeChart.Name = "symRegTreeChart";
      this.symRegTreeChart.ReadOnly = false;
      this.symRegTreeChart.Size = new System.Drawing.Size(288, 153);
      this.symRegTreeChart.TabIndex = 6;
      // 
      // templateStructureGroupBox
      // 
      this.templateStructureGroupBox.Controls.Add(this.treeChart);
      this.templateStructureGroupBox.Controls.Add(this.symRegTreeChart);
      this.templateStructureGroupBox.Controls.Add(this.parseButton);
      this.templateStructureGroupBox.Controls.Add(this.errorLabel);
      this.templateStructureGroupBox.Controls.Add(this.expressionInput);
      this.templateStructureGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.templateStructureGroupBox.Location = new System.Drawing.Point(0, 0);
      this.templateStructureGroupBox.Name = "templateStructureGroupBox";
      this.templateStructureGroupBox.Size = new System.Drawing.Size(300, 400);
      this.templateStructureGroupBox.TabIndex = 8;
      this.templateStructureGroupBox.TabStop = false;
      this.templateStructureGroupBox.Text = "Template Structure";
      // 
      // treeChart
      // 
      this.treeChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.treeChart.BackgroundColor = System.Drawing.Color.White;
      this.treeChart.LineColor = System.Drawing.Color.Black;
      this.treeChart.Location = new System.Drawing.Point(6, 233);
      this.treeChart.MinimumHorizontalDistance = 30;
      this.treeChart.MinimumHorizontalPadding = 20;
      this.treeChart.MinimumVerticalDistance = 30;
      this.treeChart.MinimumVerticalPadding = 20;
      this.treeChart.Name = "treeChart";
      this.treeChart.PreferredNodeHeight = 46;
      this.treeChart.PreferredNodeWidth = 70;
      this.treeChart.Size = new System.Drawing.Size(288, 150);
      this.treeChart.SuspendRepaint = false;
      this.treeChart.TabIndex = 7;
      this.treeChart.TextFont = new System.Drawing.Font("Times New Roman", 8F);
      this.treeChart.Tree = null;
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.templateStructureGroupBox);
      this.splitContainer.Panel1MinSize = 5;
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add(this.viewHost);
      this.splitContainer.Panel2MinSize = 5;
      this.splitContainer.Size = new System.Drawing.Size(600, 400);
      this.splitContainer.SplitterDistance = 300;
      this.splitContainer.TabIndex = 10;
      // 
      // viewHost
      // 
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(0, 0);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(296, 400);
      this.viewHost.TabIndex = 8;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // StructureTemplateView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "StructureTemplateView";
      this.Size = new System.Drawing.Size(600, 400);
      this.templateStructureGroupBox.ResumeLayout(false);
      this.templateStructureGroupBox.PerformLayout();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.TextBox expressionInput;
    private System.Windows.Forms.Button parseButton;
    private System.Windows.Forms.Label errorLabel;
    private Encodings.SymbolicExpressionTreeEncoding.Views.GraphicalSymbolicExpressionTreeView symRegTreeChart;
    private System.Windows.Forms.GroupBox templateStructureGroupBox;
    private System.Windows.Forms.SplitContainer splitContainer;
    private Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionTreeChart treeChart;
    private MainForm.WindowsForms.ViewHost viewHost;
  }
}
