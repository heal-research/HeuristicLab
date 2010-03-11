namespace HeuristicLab.PluginInfrastructure.Advanced {
  partial class LicenseConfirmationBox {
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
      this.richTextBox = new System.Windows.Forms.RichTextBox();
      this.acceptButton = new System.Windows.Forms.Button();
      this.rejectButton = new System.Windows.Forms.Button();
      this.licenseLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // richTextBox
      // 
      this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.richTextBox.Location = new System.Drawing.Point(12, 37);
      this.richTextBox.Name = "richTextBox";
      this.richTextBox.ReadOnly = true;
      this.richTextBox.Size = new System.Drawing.Size(494, 351);
      this.richTextBox.TabIndex = 0;
      this.richTextBox.Text = "";
      // 
      // acceptButton
      // 
      this.acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.acceptButton.Location = new System.Drawing.Point(298, 394);
      this.acceptButton.Name = "acceptButton";
      this.acceptButton.Size = new System.Drawing.Size(75, 23);
      this.acceptButton.TabIndex = 1;
      this.acceptButton.Text = "I Accept ";
      this.acceptButton.UseVisualStyleBackColor = true;
      this.acceptButton.Click += new System.EventHandler(this.acceptButton_Click);
      // 
      // rejectButton
      // 
      this.rejectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.rejectButton.Location = new System.Drawing.Point(379, 394);
      this.rejectButton.Name = "rejectButton";
      this.rejectButton.Size = new System.Drawing.Size(127, 23);
      this.rejectButton.TabIndex = 2;
      this.rejectButton.Text = "Cancel Installation";
      this.rejectButton.UseVisualStyleBackColor = true;
      this.rejectButton.Click += new System.EventHandler(this.rejectButton_Click);
      // 
      // licenseLabel
      // 
      this.licenseLabel.AutoSize = true;
      this.licenseLabel.Location = new System.Drawing.Point(13, 13);
      this.licenseLabel.Name = "licenseLabel";
      this.licenseLabel.Size = new System.Drawing.Size(177, 13);
      this.licenseLabel.TabIndex = 3;
      this.licenseLabel.Text = "Please confirm license agreement of";
      // 
      // LicenseConfirmationBox
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(518, 429);
      this.Controls.Add(this.licenseLabel);
      this.Controls.Add(this.rejectButton);
      this.Controls.Add(this.acceptButton);
      this.Controls.Add(this.richTextBox);
      this.Name = "LicenseConfirmationBox";
      this.Text = "License Agreement";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.RichTextBox richTextBox;
    private System.Windows.Forms.Button acceptButton;
    private System.Windows.Forms.Button rejectButton;
    private System.Windows.Forms.Label licenseLabel;
  }
}