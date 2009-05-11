namespace HeuristicLab.Visualization.DataExport {
  partial class ExportDialog {
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
      this.lbExporters = new System.Windows.Forms.ListBox();
      this.btnSelectExporter = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // lbExporters
      // 
      this.lbExporters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lbExporters.FormattingEnabled = true;
      this.lbExporters.Location = new System.Drawing.Point(12, 12);
      this.lbExporters.Name = "lbExporters";
      this.lbExporters.Size = new System.Drawing.Size(204, 160);
      this.lbExporters.TabIndex = 0;
      // 
      // btnSelectExporter
      // 
      this.btnSelectExporter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSelectExporter.Location = new System.Drawing.Point(60, 184);
      this.btnSelectExporter.Name = "btnSelectExporter";
      this.btnSelectExporter.Size = new System.Drawing.Size(75, 23);
      this.btnSelectExporter.TabIndex = 1;
      this.btnSelectExporter.Text = "Select";
      this.btnSelectExporter.UseVisualStyleBackColor = true;
      this.btnSelectExporter.Click += new System.EventHandler(this.btnSelectExporter_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancel.Location = new System.Drawing.Point(141, 184);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 2;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // ExportDialog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(228, 218);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnSelectExporter);
      this.Controls.Add(this.lbExporters);
      this.Name = "ExportDialog";
      this.Text = "ExportDialog";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox lbExporters;
    private System.Windows.Forms.Button btnSelectExporter;
    private System.Windows.Forms.Button btnCancel;
  }
}