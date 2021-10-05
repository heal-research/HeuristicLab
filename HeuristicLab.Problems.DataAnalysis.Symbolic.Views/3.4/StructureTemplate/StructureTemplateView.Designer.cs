
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
      this.expressionInput = new System.Windows.Forms.TextBox();
      this.parseButton = new System.Windows.Forms.Button();
      this.errorLabel = new System.Windows.Forms.Label();
      this.symRegTreeChart = new HeuristicLab.Encodings.SymbolicExpressionTreeEncoding.Views.GraphicalSymbolicExpressionTreeView();
      this.templateStructureGroupBox = new System.Windows.Forms.GroupBox();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.subFunctionListView = new HeuristicLab.Problems.DataAnalysis.Symbolic.Views.SubFunctionListView();
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
      this.symRegTreeChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.symRegTreeChart.Caption = "Graphical SymbolicExpressionTree View";
      this.symRegTreeChart.Content = null;
      this.symRegTreeChart.Location = new System.Drawing.Point(6, 74);
      this.symRegTreeChart.Name = "symRegTreeChart";
      this.symRegTreeChart.ReadOnly = false;
      this.symRegTreeChart.Size = new System.Drawing.Size(288, 320);
      this.symRegTreeChart.TabIndex = 6;
      // 
      // templateStructureGroupBox
      // 
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
      this.splitContainer.Panel2.Controls.Add(this.subFunctionListView);
      this.splitContainer.Panel2MinSize = 5;
      this.splitContainer.Size = new System.Drawing.Size(600, 400);
      this.splitContainer.SplitterDistance = 300;
      this.splitContainer.TabIndex = 10;
      // 
      // subFunctionListView
      // 
      this.subFunctionListView.Caption = "View";
      this.subFunctionListView.Content = null;
      this.subFunctionListView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.subFunctionListView.Location = new System.Drawing.Point(0, 0);
      this.subFunctionListView.Name = "subFunctionListView";
      this.subFunctionListView.ReadOnly = false;
      this.subFunctionListView.Size = new System.Drawing.Size(296, 400);
      this.subFunctionListView.TabIndex = 7;
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
    private SubFunctionListView subFunctionListView;
    private System.Windows.Forms.GroupBox templateStructureGroupBox;
    private System.Windows.Forms.SplitContainer splitContainer;
  }
}
