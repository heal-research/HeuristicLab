namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class ProblemDataView {
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
      this.FeatureCorrelationButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.AllowDrop = true;
      this.parameterCollectionView.DragDrop += new System.Windows.Forms.DragEventHandler(this.parameterCollectionView_DragDrop);
      this.parameterCollectionView.DragEnter += new System.Windows.Forms.DragEventHandler(this.parameterCollectionView_DragEnterOver);
      this.parameterCollectionView.DragOver += new System.Windows.Forms.DragEventHandler(this.parameterCollectionView_DragEnterOver);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // FeatureCorrelationButton
      // 
      this.FeatureCorrelationButton.Location = new System.Drawing.Point(184, 45);
      this.FeatureCorrelationButton.Name = "FeatureCorrelationButton";
      this.FeatureCorrelationButton.Size = new System.Drawing.Size(121, 24);
      this.FeatureCorrelationButton.TabIndex = 4;
      this.FeatureCorrelationButton.Text = "Feature Correlation";
      this.FeatureCorrelationButton.UseVisualStyleBackColor = true;
      this.FeatureCorrelationButton.Click += new System.EventHandler(this.FeatureCorrelationButton_Click);
      // 
      // ProblemDataView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.FeatureCorrelationButton);
      this.Name = "ProblemDataView";
      this.Controls.SetChildIndex(this.parameterCollectionView, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.FeatureCorrelationButton, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    protected System.Windows.Forms.Button FeatureCorrelationButton;

  }
}
