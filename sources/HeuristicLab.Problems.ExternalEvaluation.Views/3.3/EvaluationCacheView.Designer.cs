namespace HeuristicLab.Problems.ExternalEvaluation.Views {
  partial class EvaluationCacheView {
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
      this.hits_sizeLabel = new System.Windows.Forms.Label();
      this.hits_sizeTextBox = new System.Windows.Forms.TextBox();
      this.clearButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // parameterCollectionView
      // 
      this.parameterCollectionView.Location = new System.Drawing.Point(0, 52);
      this.parameterCollectionView.Size = new System.Drawing.Size(490, 301);
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // hits_sizeLabel
      // 
      this.hits_sizeLabel.AutoSize = true;
      this.hits_sizeLabel.Location = new System.Drawing.Point(3, 29);
      this.hits_sizeLabel.Name = "hits_sizeLabel";
      this.hits_sizeLabel.Size = new System.Drawing.Size(53, 13);
      this.hits_sizeLabel.TabIndex = 4;
      this.hits_sizeLabel.Text = "Hits/Size:";
      // 
      // hits_sizeTextBox
      // 
      this.hits_sizeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.hits_sizeTextBox.Location = new System.Drawing.Point(59, 26);
      this.hits_sizeTextBox.Name = "hits_sizeTextBox";
      this.hits_sizeTextBox.ReadOnly = true;
      this.hits_sizeTextBox.Size = new System.Drawing.Size(379, 20);
      this.hits_sizeTextBox.TabIndex = 5;
      // 
      // clearButton
      // 
      this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.clearButton.Location = new System.Drawing.Point(444, 24);
      this.clearButton.Name = "clearButton";
      this.clearButton.Size = new System.Drawing.Size(43, 23);
      this.clearButton.TabIndex = 6;
      this.clearButton.Text = "Clear";
      this.toolTip.SetToolTip(this.clearButton, "Clear");
      this.clearButton.UseVisualStyleBackColor = true;
      this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
      // 
      // EvaluationCacheView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.hits_sizeTextBox);
      this.Controls.Add(this.hits_sizeLabel);
      this.Controls.Add(this.clearButton);
      this.Name = "EvaluationCacheView";
      this.Controls.SetChildIndex(this.clearButton, 0);
      this.Controls.SetChildIndex(this.hits_sizeLabel, 0);
      this.Controls.SetChildIndex(this.hits_sizeTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.parameterCollectionView, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label hits_sizeLabel;
    private System.Windows.Forms.TextBox hits_sizeTextBox;
    private System.Windows.Forms.Button clearButton;
  }
}
