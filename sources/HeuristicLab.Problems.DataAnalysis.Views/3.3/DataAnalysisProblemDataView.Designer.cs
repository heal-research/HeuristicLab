namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class DataAnalysisProblemDataView {
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
      this.variableCollectionView = new HeuristicLab.Core.Views.VariableCollectionView();
      this.importButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Size = new System.Drawing.Size(513, 20);
      // 
      // descriptionTextBox
      // 
      this.descriptionTextBox.Size = new System.Drawing.Size(513, 20);
      // 
      // variableCollectionView
      // 
      this.variableCollectionView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.variableCollectionView.Caption = "VariableCollection";
      this.variableCollectionView.Content = null;
      this.variableCollectionView.Location = new System.Drawing.Point(0, 52);
      this.variableCollectionView.Name = "variableCollectionView";
      this.variableCollectionView.ReadOnly = false;
      this.variableCollectionView.Size = new System.Drawing.Size(588, 366);
      this.variableCollectionView.TabIndex = 0;
      // 
      // importButton
      // 
      this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.importButton.Location = new System.Drawing.Point(6, 424);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(579, 23);
      this.importButton.TabIndex = 4;
      this.importButton.Text = "Import from CSV file";
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.importButton_Click);
      // 
      // DataAnalysisProblemDataView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.variableCollectionView);
      this.Controls.Add(this.importButton);
      this.Name = "DataAnalysisProblemDataView";
      this.Size = new System.Drawing.Size(588, 450);
      this.Controls.SetChildIndex(this.importButton, 0);
      this.Controls.SetChildIndex(this.variableCollectionView, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.descriptionLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      this.Controls.SetChildIndex(this.descriptionTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private HeuristicLab.Core.Views.VariableCollectionView variableCollectionView;
    private System.Windows.Forms.Button importButton;
  }
}
