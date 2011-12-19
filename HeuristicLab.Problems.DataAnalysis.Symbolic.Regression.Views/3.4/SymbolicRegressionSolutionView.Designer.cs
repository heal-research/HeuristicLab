namespace HeuristicLab.Problems.DataAnalysis.Symbolic.Regression.Views {
  partial class SymbolicRegressionSolutionView {
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
      this.btnSimplify = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.itemsGroupBox.SuspendLayout();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.btnSimplify);
      this.splitContainer.Size = new System.Drawing.Size(480, 275);
      this.splitContainer.SplitterDistance = 255;
      // 
      // itemsGroupBox
      // 
      this.itemsGroupBox.Size = new System.Drawing.Size(486, 294);
      // 
      // itemsListView
      // 
      this.itemsListView.Size = new System.Drawing.Size(249, 238);
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Size = new System.Drawing.Size(215, 246);
      // 
      // addButton
      // 
      this.toolTip.SetToolTip(this.addButton, "Add");
      // 
      // removeButton
      // 
      this.toolTip.SetToolTip(this.removeButton, "Remove");
      // 
      // viewHost
      // 
      this.viewHost.Size = new System.Drawing.Size(203, 221);
      // 
      // btnSimplify
      // 
      this.btnSimplify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSimplify.Location = new System.Drawing.Point(177, 4);
      this.btnSimplify.Name = "btnSimplify";
      this.btnSimplify.Size = new System.Drawing.Size(75, 23);
      this.btnSimplify.TabIndex = 6;
      this.btnSimplify.Text = "Simplify";
      this.btnSimplify.UseVisualStyleBackColor = true;
      this.btnSimplify.Click += new System.EventHandler(this.btn_SimplifyModel_Click);
      // 
      // SymbolicRegressionSolutionView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Name = "SymbolicRegressionSolutionView";
      this.Size = new System.Drawing.Size(486, 294);
      this.splitContainer.Panel1.ResumeLayout(false);
      this.splitContainer.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.itemsGroupBox.ResumeLayout(false);
      this.detailsGroupBox.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button btnSimplify;
  }
}
