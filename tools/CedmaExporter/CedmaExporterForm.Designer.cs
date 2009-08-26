namespace CedmaExporter {
  partial class CedmaExporterForm {
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
      this.exportButton = new System.Windows.Forms.Button();
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.cancelButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // exportButton
      // 
      this.exportButton.Location = new System.Drawing.Point(12, 12);
      this.exportButton.Name = "exportButton";
      this.exportButton.Size = new System.Drawing.Size(75, 23);
      this.exportButton.TabIndex = 0;
      this.exportButton.Text = "Export...";
      this.exportButton.UseVisualStyleBackColor = true;
      this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
      // 
      // progressBar
      // 
      this.progressBar.Location = new System.Drawing.Point(12, 41);
      this.progressBar.Name = "progressBar";
      this.progressBar.Size = new System.Drawing.Size(260, 23);
      this.progressBar.TabIndex = 1;
      // 
      // cancelButton
      // 
      this.cancelButton.Enabled = false;
      this.cancelButton.Location = new System.Drawing.Point(94, 11);
      this.cancelButton.Name = "cancelButton";
      this.cancelButton.Size = new System.Drawing.Size(75, 23);
      this.cancelButton.TabIndex = 2;
      this.cancelButton.Text = "Cancel";
      this.cancelButton.UseVisualStyleBackColor = true;
      this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
      // 
      // CedmaExporterForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 81);
      this.Controls.Add(this.cancelButton);
      this.Controls.Add(this.progressBar);
      this.Controls.Add(this.exportButton);
      this.Name = "CedmaExporterForm";
      this.Text = "CEDMA Exporter";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button exportButton;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.Button cancelButton;
  }
}

