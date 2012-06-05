namespace HeuristicLab.Clients.Hive.Views {
  partial class HiveInformationDialog {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HiveInformationDialog));
      this.richTextBox = new System.Windows.Forms.RichTextBox();
      this.okButton = new System.Windows.Forms.Button();
      this.panel = new System.Windows.Forms.Panel();
      this.label = new System.Windows.Forms.Label();
      this.panel.SuspendLayout();
      this.SuspendLayout();
      // 
      // richTextBox
      // 
      this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.richTextBox.BackColor = System.Drawing.Color.White;
      this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.richTextBox.Cursor = System.Windows.Forms.Cursors.Default;
      this.richTextBox.Location = new System.Drawing.Point(95, 12);
      this.richTextBox.Name = "richTextBox";
      this.richTextBox.ReadOnly = true;
      this.richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.richTextBox.Size = new System.Drawing.Size(422, 137);
      this.richTextBox.TabIndex = 1;
      this.richTextBox.Text = resources.GetString("richTextBox.Text");
      this.richTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.Location = new System.Drawing.Point(442, 170);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 0;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      this.okButton.Click += new System.EventHandler(this.okButton_Click);
      // 
      // panel
      // 
      this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel.BackColor = System.Drawing.SystemColors.Window;
      this.panel.Controls.Add(this.label);
      this.panel.Controls.Add(this.richTextBox);
      this.panel.Location = new System.Drawing.Point(0, 0);
      this.panel.Name = "panel";
      this.panel.Size = new System.Drawing.Size(529, 164);
      this.panel.TabIndex = 1;
      // 
      // label
      // 
      this.label.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.label.Location = new System.Drawing.Point(24, 58);
      this.label.Name = "label";
      this.label.Size = new System.Drawing.Size(48, 48);
      this.label.TabIndex = 0;
      // 
      // HiveInformationDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.ClientSize = new System.Drawing.Size(529, 205);
      this.Controls.Add(this.panel);
      this.Controls.Add(this.okButton);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "HiveInformationDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "HeuristicLab Hive";
      this.TopMost = true;
      this.panel.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.RichTextBox richTextBox;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Panel panel;
    private System.Windows.Forms.Label label;
  }
}