namespace CedmaImporter {
  partial class ImporterForm {
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
      this.problemViewPanel = new System.Windows.Forms.Panel();
      this.importButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // problemViewPanel
      // 
      this.problemViewPanel.Location = new System.Drawing.Point(12, 12);
      this.problemViewPanel.Name = "problemViewPanel";
      this.problemViewPanel.Size = new System.Drawing.Size(260, 211);
      this.problemViewPanel.TabIndex = 0;
      // 
      // button1
      // 
      this.importButton.Location = new System.Drawing.Point(12, 229);
      this.importButton.Name = "button1";
      this.importButton.Size = new System.Drawing.Size(75, 23);
      this.importButton.TabIndex = 1;
      this.importButton.Text = "button1";
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.importButton_Click);
      // 
      // ImporterForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 264);
      this.Controls.Add(this.importButton);
      this.Controls.Add(this.problemViewPanel);
      this.Name = "ImporterForm";
      this.Text = "Form1";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel problemViewPanel;
    private System.Windows.Forms.Button importButton;
  }
}

