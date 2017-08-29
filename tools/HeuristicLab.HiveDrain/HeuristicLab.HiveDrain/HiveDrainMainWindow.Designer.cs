namespace HeuristicLab.HiveDrain {
  partial class HiveDrainMainWindow {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HiveDrainMainWindow));
      this.label1 = new System.Windows.Forms.Label();
      this.patterTextBox = new System.Windows.Forms.TextBox();
      this.downloadButton = new System.Windows.Forms.Button();
      this.logView = new HeuristicLab.Core.Views.LogView();
      this.oneFileCheckBox = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(134, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Pattern to match job name:";
      // 
      // patterTextBox
      // 
      this.patterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.patterTextBox.Location = new System.Drawing.Point(152, 12);
      this.patterTextBox.Name = "patterTextBox";
      this.patterTextBox.Size = new System.Drawing.Size(678, 20);
      this.patterTextBox.TabIndex = 1;
      // 
      // downloadButton
      // 
      this.downloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.downloadButton.Location = new System.Drawing.Point(698, 38);
      this.downloadButton.Name = "downloadButton";
      this.downloadButton.Size = new System.Drawing.Size(132, 23);
      this.downloadButton.TabIndex = 3;
      this.downloadButton.Text = "Download";
      this.downloadButton.UseVisualStyleBackColor = true;
      this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
      // 
      // logView
      // 
      this.logView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.logView.Caption = "Log View";
      this.logView.Content = null;
      this.logView.Location = new System.Drawing.Point(15, 67);
      this.logView.Name = "logView";
      this.logView.ReadOnly = false;
      this.logView.Size = new System.Drawing.Size(815, 368);
      this.logView.TabIndex = 4;
      // 
      // oneFileCheckBox
      // 
      this.oneFileCheckBox.AutoSize = true;
      this.oneFileCheckBox.Checked = true;
      this.oneFileCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.oneFileCheckBox.Location = new System.Drawing.Point(15, 44);
      this.oneFileCheckBox.Name = "oneFileCheckBox";
      this.oneFileCheckBox.Size = new System.Drawing.Size(102, 17);
      this.oneFileCheckBox.TabIndex = 5;
      this.oneFileCheckBox.Text = "Save as one file";
      this.oneFileCheckBox.UseVisualStyleBackColor = true;
      // 
      // HiveDrainMainWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(842, 447);
      this.Controls.Add(this.oneFileCheckBox);
      this.Controls.Add(this.logView);
      this.Controls.Add(this.downloadButton);
      this.Controls.Add(this.patterTextBox);
      this.Controls.Add(this.label1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "HiveDrainMainWindow";
      this.Text = "HiveDrainMainWindow";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HiveDrainMainWindow_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox patterTextBox;
    private System.Windows.Forms.Button downloadButton;
    private HeuristicLab.Core.Views.LogView logView;
    private System.Windows.Forms.CheckBox oneFileCheckBox;
  }
}