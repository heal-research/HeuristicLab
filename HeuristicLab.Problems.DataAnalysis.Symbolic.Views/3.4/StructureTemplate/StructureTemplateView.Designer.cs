
using System.Drawing;
using System.Windows.Forms;

namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Views {
  partial class StructureTemplateView {
    
    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StructureTemplateView));
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.viewHost = new HeuristicLab.MainForm.WindowsForms.ViewHost();
      this.templateStructureGroupBox = new System.Windows.Forms.GroupBox();
      this.linearScalingCheckBox = new System.Windows.Forms.CheckBox();
      this.infoLabel = new System.Windows.Forms.Label();
      this.treeChart = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionTreeChart();
      this.parseButton = new System.Windows.Forms.Button();
      this.expressionInput = new System.Windows.Forms.TextBox();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.errorLabel = new System.Windows.Forms.Label();
      this.detailsGroupBox.SuspendLayout();
      this.templateStructureGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Controls.Add(this.viewHost);
      this.detailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.detailsGroupBox.Location = new System.Drawing.Point(0, 0);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(296, 400);
      this.detailsGroupBox.TabIndex = 9;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // viewHost
      // 
      this.viewHost.Caption = "View";
      this.viewHost.Content = null;
      this.viewHost.Dock = System.Windows.Forms.DockStyle.Fill;
      this.viewHost.Enabled = false;
      this.viewHost.Location = new System.Drawing.Point(3, 16);
      this.viewHost.Name = "viewHost";
      this.viewHost.ReadOnly = false;
      this.viewHost.Size = new System.Drawing.Size(290, 381);
      this.viewHost.TabIndex = 8;
      this.viewHost.ViewsLabelVisible = true;
      this.viewHost.ViewType = null;
      // 
      // templateStructureGroupBox
      // 
      this.templateStructureGroupBox.Controls.Add(this.errorLabel);
      this.templateStructureGroupBox.Controls.Add(this.linearScalingCheckBox);
      this.templateStructureGroupBox.Controls.Add(this.infoLabel);
      this.templateStructureGroupBox.Controls.Add(this.treeChart);
      this.templateStructureGroupBox.Controls.Add(this.parseButton);
      this.templateStructureGroupBox.Controls.Add(this.expressionInput);
      this.templateStructureGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.templateStructureGroupBox.Location = new System.Drawing.Point(0, 0);
      this.templateStructureGroupBox.Name = "templateStructureGroupBox";
      this.templateStructureGroupBox.Size = new System.Drawing.Size(300, 400);
      this.templateStructureGroupBox.TabIndex = 8;
      this.templateStructureGroupBox.TabStop = false;
      this.templateStructureGroupBox.Text = "Template Structure";
      // 
      // linearScalingCheckBox
      // 
      this.linearScalingCheckBox.AutoSize = true;
      this.linearScalingCheckBox.Location = new System.Drawing.Point(149, 49);
      this.linearScalingCheckBox.Name = "linearScalingCheckBox";
      this.linearScalingCheckBox.Size = new System.Drawing.Size(122, 17);
      this.linearScalingCheckBox.TabIndex = 8;
      this.linearScalingCheckBox.Text = "Apply Linear Scaling";
      this.linearScalingCheckBox.UseVisualStyleBackColor = true;
      this.linearScalingCheckBox.CheckStateChanged += new System.EventHandler(this.LinearScalingCheckBoxCheckStateChanged);
      // 
      // infoLabel
      // 
      this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.infoLabel.BackColor = System.Drawing.Color.White;
      this.infoLabel.Image = HeuristicLab.Common.Resources.VSImageLibrary.Information;
      this.infoLabel.Location = new System.Drawing.Point(275, 22);
      this.infoLabel.Name = "infoLabel";
      this.infoLabel.Size = new System.Drawing.Size(15, 15);
      this.infoLabel.TabIndex = 4;
      this.infoLabel.Text = "    ";
      this.toolTip.SetToolTip(this.infoLabel, "Double-click to open description.");
      this.infoLabel.DoubleClick += new System.EventHandler(this.HelpButtonDoubleClick);
      // 
      // treeChart
      // 
      this.treeChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.treeChart.BackgroundColor = System.Drawing.Color.White;
      this.treeChart.LineColor = System.Drawing.Color.Black;
      this.treeChart.Location = new System.Drawing.Point(3, 74);
      this.treeChart.MinimumHorizontalDistance = 30;
      this.treeChart.MinimumHorizontalPadding = 20;
      this.treeChart.MinimumVerticalDistance = 30;
      this.treeChart.MinimumVerticalPadding = 20;
      this.treeChart.Name = "treeChart";
      this.treeChart.PreferredNodeHeight = 46;
      this.treeChart.PreferredNodeWidth = 70;
      this.treeChart.Size = new System.Drawing.Size(291, 320);
      this.treeChart.SuspendRepaint = false;
      this.treeChart.TabIndex = 7;
      this.treeChart.TextFont = new System.Drawing.Font("Times New Roman", 8F);
      this.treeChart.Tree = null;
      // 
      // parseButton
      // 
      this.parseButton.Location = new System.Drawing.Point(3, 45);
      this.parseButton.Name = "parseButton";
      this.parseButton.Size = new System.Drawing.Size(140, 23);
      this.parseButton.TabIndex = 3;
      this.parseButton.Text = "Parse";
      this.parseButton.UseVisualStyleBackColor = true;
      this.parseButton.Click += new System.EventHandler(this.ParseButtonClick);
      // 
      // expressionInput
      // 
      this.expressionInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.expressionInput.Location = new System.Drawing.Point(3, 19);
      this.expressionInput.Name = "expressionInput";
      this.expressionInput.Size = new System.Drawing.Size(266, 20);
      this.expressionInput.TabIndex = 1;
      this.expressionInput.TextChanged += new System.EventHandler(this.ExpressionInputTextChanged);
      this.expressionInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ExpressionInputKeyUp);
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
      this.splitContainer.Panel2.Controls.Add(this.detailsGroupBox);
      this.splitContainer.Panel2MinSize = 5;
      this.splitContainer.Size = new System.Drawing.Size(600, 400);
      this.splitContainer.SplitterDistance = 300;
      this.splitContainer.TabIndex = 10;
      // 
      // errorLabel
      // 
      this.errorLabel.AutoSize = true;
      this.errorLabel.BackColor = System.Drawing.Color.White;
      this.errorLabel.Location = new System.Drawing.Point(3, 74);
      this.errorLabel.Name = "errorLabel";
      this.errorLabel.Size = new System.Drawing.Size(0, 13);
      this.errorLabel.TabIndex = 9;
      // 
      // StructureTemplateView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.splitContainer);
      this.Name = "StructureTemplateView";
      this.Size = new System.Drawing.Size(600, 400);
      this.detailsGroupBox.ResumeLayout(false);
      this.templateStructureGroupBox.ResumeLayout(false);
      this.templateStructureGroupBox.PerformLayout();
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }


    private System.Windows.Forms.GroupBox detailsGroupBox;
    private MainForm.WindowsForms.ViewHost viewHost;
    private System.Windows.Forms.GroupBox templateStructureGroupBox;
    private System.Windows.Forms.CheckBox linearScalingCheckBox;
    private System.Windows.Forms.Label infoLabel;
    private Encodings.SymbolicExpressionTreeEncoding.Views.SymbolicExpressionTreeChart treeChart;
    private System.Windows.Forms.Button parseButton;
    private System.Windows.Forms.TextBox expressionInput;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.ComponentModel.IContainer components;
    private ToolTip toolTip;
    private Label errorLabel;
  }
}
