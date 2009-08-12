namespace CedmaDatabaseMerger {
  partial class MergerForm {
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
      this.setOutputButton = new System.Windows.Forms.Button();
      this.importButton = new System.Windows.Forms.Button();
      this.outputTextBox = new System.Windows.Forms.TextBox();
      this.importProgressBar = new System.Windows.Forms.ProgressBar();
      this.SuspendLayout();
      // 
      // setOutputButton
      // 
      this.setOutputButton.Location = new System.Drawing.Point(12, 12);
      this.setOutputButton.Name = "setOutputButton";
      this.setOutputButton.Size = new System.Drawing.Size(75, 23);
      this.setOutputButton.TabIndex = 0;
      this.setOutputButton.Text = "Set output...";
      this.setOutputButton.UseVisualStyleBackColor = true;
      this.setOutputButton.Click += new System.EventHandler(this.setOutputButton_Click);
      // 
      // importButton
      // 
      this.importButton.Location = new System.Drawing.Point(12, 41);
      this.importButton.Name = "importButton";
      this.importButton.Size = new System.Drawing.Size(75, 23);
      this.importButton.TabIndex = 1;
      this.importButton.Text = "Import...";
      this.importButton.UseVisualStyleBackColor = true;
      this.importButton.Click += new System.EventHandler(this.importButton_Click);
      // 
      // outputTextBox
      // 
      this.outputTextBox.Location = new System.Drawing.Point(93, 14);
      this.outputTextBox.Name = "outputTextBox";
      this.outputTextBox.Size = new System.Drawing.Size(157, 20);
      this.outputTextBox.TabIndex = 2;
      // 
      // progressBar1
      // 
      this.importProgressBar.Location = new System.Drawing.Point(94, 40);
      this.importProgressBar.Name = "progressBar1";
      this.importProgressBar.Size = new System.Drawing.Size(156, 23);
      this.importProgressBar.TabIndex = 3;
      // 
      // MergerForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(270, 82);
      this.Controls.Add(this.importProgressBar);
      this.Controls.Add(this.outputTextBox);
      this.Controls.Add(this.importButton);
      this.Controls.Add(this.setOutputButton);
      this.Name = "MergerForm";
      this.Text = "MergerForm";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button setOutputButton;
    private System.Windows.Forms.Button importButton;
    private System.Windows.Forms.TextBox outputTextBox;
    private System.Windows.Forms.ProgressBar importProgressBar;
  }
}