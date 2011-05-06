namespace HeuristicLab.Problems.ExternalEvaluation {
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.sizeLabel = new System.Windows.Forms.Label();
      this.sizeTextBox = new System.Windows.Forms.TextBox();
      this.hitsTextBox = new System.Windows.Forms.TextBox();
      this.hitsLabel = new System.Windows.Forms.Label();
      this.clearButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      // 
      // sizeLabel
      // 
      this.sizeLabel.AutoSize = true;
      this.sizeLabel.Location = new System.Drawing.Point(6, 30);
      this.sizeLabel.Name = "sizeLabel";
      this.sizeLabel.Size = new System.Drawing.Size(30, 13);
      this.sizeLabel.TabIndex = 3;
      this.sizeLabel.Text = "Size:";
      // 
      // sizeTextBox
      // 
      this.sizeTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.sizeTextBox.Location = new System.Drawing.Point(58, 27);
      this.sizeTextBox.Name = "sizeTextBox";
      this.sizeTextBox.ReadOnly = true;
      this.sizeTextBox.Size = new System.Drawing.Size(290, 20);
      this.sizeTextBox.TabIndex = 4;
      // 
      // hitsTextBox
      // 
      this.hitsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.hitsTextBox.Location = new System.Drawing.Point(58, 54);
      this.hitsTextBox.Name = "hitsTextBox";
      this.hitsTextBox.ReadOnly = true;
      this.hitsTextBox.Size = new System.Drawing.Size(290, 20);
      this.hitsTextBox.TabIndex = 5;
      // 
      // hitsLabel
      // 
      this.hitsLabel.AutoSize = true;
      this.hitsLabel.Location = new System.Drawing.Point(6, 57);
      this.hitsLabel.Name = "hitsLabel";
      this.hitsLabel.Size = new System.Drawing.Size(28, 13);
      this.hitsLabel.TabIndex = 6;
      this.hitsLabel.Text = "Hits:";
      // 
      // clearButton
      // 
      this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.clearButton.Location = new System.Drawing.Point(273, 80);
      this.clearButton.Name = "clearButton";
      this.clearButton.Size = new System.Drawing.Size(75, 23);
      this.clearButton.TabIndex = 7;
      this.clearButton.Text = "Clear";
      this.clearButton.UseVisualStyleBackColor = true;
      this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
      // 
      // EvaluationCacheView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.Controls.Add(this.hitsTextBox);
      this.Controls.Add(this.sizeTextBox);
      this.Controls.Add(this.sizeLabel);
      this.Controls.Add(this.clearButton);
      this.Controls.Add(this.hitsLabel);
      this.Name = "EvaluationCacheView";
      this.Size = new System.Drawing.Size(351, 107);
      this.Controls.SetChildIndex(this.hitsLabel, 0);
      this.Controls.SetChildIndex(this.clearButton, 0);
      this.Controls.SetChildIndex(this.sizeLabel, 0);
      this.Controls.SetChildIndex(this.sizeTextBox, 0);
      this.Controls.SetChildIndex(this.hitsTextBox, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label sizeLabel;
    private System.Windows.Forms.TextBox sizeTextBox;
    private System.Windows.Forms.TextBox hitsTextBox;
    private System.Windows.Forms.Label hitsLabel;
    private System.Windows.Forms.Button clearButton;
  }
}
