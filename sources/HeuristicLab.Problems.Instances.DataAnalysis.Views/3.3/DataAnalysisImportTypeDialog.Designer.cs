namespace HeuristicLab.Problems.Instances.DataAnalysis.Views {
  partial class DataAnalysisImportTypeDialog {
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
      this.ShuffelDataCheckbox = new System.Windows.Forms.CheckBox();
      this.OkButton = new System.Windows.Forms.Button();
      this.CancelButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // ShuffelDataCheckbox
      // 
      this.ShuffelDataCheckbox.AutoSize = true;
      this.ShuffelDataCheckbox.Location = new System.Drawing.Point(61, 12);
      this.ShuffelDataCheckbox.Name = "ShuffelDataCheckbox";
      this.ShuffelDataCheckbox.Size = new System.Drawing.Size(91, 17);
      this.ShuffelDataCheckbox.TabIndex = 1;
      this.ShuffelDataCheckbox.Text = "Shuffel Data?";
      this.ShuffelDataCheckbox.UseVisualStyleBackColor = true;
      // 
      // OkButton
      // 
      this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.OkButton.Location = new System.Drawing.Point(12, 35);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(75, 23);
      this.OkButton.TabIndex = 2;
      this.OkButton.Text = "&Ok";
      this.OkButton.UseVisualStyleBackColor = true;
      // 
      // CancelButton
      // 
      this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.CancelButton.Location = new System.Drawing.Point(115, 35);
      this.CancelButton.Name = "CancelButton";
      this.CancelButton.Size = new System.Drawing.Size(75, 23);
      this.CancelButton.TabIndex = 3;
      this.CancelButton.Text = "&Cancel";
      this.CancelButton.UseVisualStyleBackColor = true;
      // 
      // DataAnalysisImportTypeDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(202, 69);
      this.Controls.Add(this.CancelButton);
      this.Controls.Add(this.OkButton);
      this.Controls.Add(this.ShuffelDataCheckbox);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Name = "DataAnalysisImportTypeDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "DataAnalysisImportTypeDialog";
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    #endregion

    protected System.Windows.Forms.CheckBox ShuffelDataCheckbox;
    protected System.Windows.Forms.Button OkButton;
    protected System.Windows.Forms.Button CancelButton;

  }
}