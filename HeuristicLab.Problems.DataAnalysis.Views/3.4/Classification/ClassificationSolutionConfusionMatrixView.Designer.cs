namespace HeuristicLab.Problems.DataAnalysis.Views {
  partial class ClassificationSolutionConfusionMatrixView {
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
      this.dataGridView = new System.Windows.Forms.DataGridView();
      this.cmbSamples = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridView
      // 
      this.dataGridView.AllowUserToAddRows = false;
      this.dataGridView.AllowUserToDeleteRows = false;
      this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.dataGridView.Location = new System.Drawing.Point(3, 30);
      this.dataGridView.Name = "dataGridView";
      this.dataGridView.ReadOnly = true;
      this.dataGridView.Size = new System.Drawing.Size(303, 197);
      this.dataGridView.TabIndex = 0;
      // 
      // cmbSamples
      // 
      this.cmbSamples.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbSamples.FormattingEnabled = true;
      this.cmbSamples.Location = new System.Drawing.Point(56, 3);
      this.cmbSamples.Name = "cmbSamples";
      this.cmbSamples.Size = new System.Drawing.Size(121, 21);
      this.cmbSamples.TabIndex = 1;
      this.cmbSamples.SelectedIndexChanged += new System.EventHandler(this.cmbSamples_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(47, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Samples";
      // 
      // ConfusionMatrixView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.label1);
      this.Controls.Add(this.cmbSamples);
      this.Controls.Add(this.dataGridView);
      this.Name = "ConfusionMatrixView";
      this.Size = new System.Drawing.Size(309, 230);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView;
    private System.Windows.Forms.ComboBox cmbSamples;
    private System.Windows.Forms.Label label1;

  }
}
