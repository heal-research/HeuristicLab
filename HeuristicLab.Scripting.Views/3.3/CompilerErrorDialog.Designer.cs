namespace HeuristicLab.Scripting.Views {
  partial class CompilerErrorDialog {
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
      this.iconLabel = new System.Windows.Forms.Label();
      this.detailsGroupBox = new System.Windows.Forms.GroupBox();
      this.codeValueLinkLabel = new System.Windows.Forms.LinkLabel();
      this.messageLabel = new System.Windows.Forms.Label();
      this.codeLabel = new System.Windows.Forms.Label();
      this.columnValueLabel = new System.Windows.Forms.Label();
      this.columnLabel = new System.Windows.Forms.Label();
      this.lineValueLabel = new System.Windows.Forms.Label();
      this.lineLabel = new System.Windows.Forms.Label();
      this.messageValueTextBox = new System.Windows.Forms.TextBox();
      this.infoTextBox = new System.Windows.Forms.TextBox();
      this.okButton = new System.Windows.Forms.Button();
      this.detailsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // iconLabel
      // 
      this.iconLabel.Location = new System.Drawing.Point(12, 9);
      this.iconLabel.Name = "iconLabel";
      this.iconLabel.Size = new System.Drawing.Size(30, 30);
      this.iconLabel.TabIndex = 1;
      // 
      // detailsGroupBox
      // 
      this.detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.detailsGroupBox.Controls.Add(this.codeValueLinkLabel);
      this.detailsGroupBox.Controls.Add(this.messageLabel);
      this.detailsGroupBox.Controls.Add(this.codeLabel);
      this.detailsGroupBox.Controls.Add(this.columnValueLabel);
      this.detailsGroupBox.Controls.Add(this.columnLabel);
      this.detailsGroupBox.Controls.Add(this.lineValueLabel);
      this.detailsGroupBox.Controls.Add(this.lineLabel);
      this.detailsGroupBox.Controls.Add(this.messageValueTextBox);
      this.detailsGroupBox.Location = new System.Drawing.Point(12, 42);
      this.detailsGroupBox.Name = "detailsGroupBox";
      this.detailsGroupBox.Size = new System.Drawing.Size(560, 212);
      this.detailsGroupBox.TabIndex = 3;
      this.detailsGroupBox.TabStop = false;
      this.detailsGroupBox.Text = "Details";
      // 
      // codeValueLinkLabel
      // 
      this.codeValueLinkLabel.AutoSize = true;
      this.codeValueLinkLabel.Location = new System.Drawing.Point(65, 65);
      this.codeValueLinkLabel.Name = "codeValueLinkLabel";
      this.codeValueLinkLabel.Size = new System.Drawing.Size(21, 13);
      this.codeValueLinkLabel.TabIndex = 5;
      this.codeValueLinkLabel.TabStop = true;
      this.codeValueLinkLabel.Text = "{0}";
      this.codeValueLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.codeLinkLabel_LinkClicked);
      // 
      // messageLabel
      // 
      this.messageLabel.AutoSize = true;
      this.messageLabel.Location = new System.Drawing.Point(6, 88);
      this.messageLabel.Name = "messageLabel";
      this.messageLabel.Size = new System.Drawing.Size(53, 13);
      this.messageLabel.TabIndex = 6;
      this.messageLabel.Text = "Message:";
      // 
      // codeLabel
      // 
      this.codeLabel.AutoSize = true;
      this.codeLabel.Location = new System.Drawing.Point(6, 65);
      this.codeLabel.Name = "codeLabel";
      this.codeLabel.Size = new System.Drawing.Size(35, 13);
      this.codeLabel.TabIndex = 4;
      this.codeLabel.Text = "Code:";
      // 
      // columnValueLabel
      // 
      this.columnValueLabel.AutoSize = true;
      this.columnValueLabel.Location = new System.Drawing.Point(65, 42);
      this.columnValueLabel.Name = "columnValueLabel";
      this.columnValueLabel.Size = new System.Drawing.Size(21, 13);
      this.columnValueLabel.TabIndex = 3;
      this.columnValueLabel.Text = "{0}";
      // 
      // columnLabel
      // 
      this.columnLabel.AutoSize = true;
      this.columnLabel.Location = new System.Drawing.Point(6, 42);
      this.columnLabel.Name = "columnLabel";
      this.columnLabel.Size = new System.Drawing.Size(45, 13);
      this.columnLabel.TabIndex = 2;
      this.columnLabel.Text = "Column:";
      // 
      // lineValueLabel
      // 
      this.lineValueLabel.AutoSize = true;
      this.lineValueLabel.Location = new System.Drawing.Point(65, 19);
      this.lineValueLabel.Name = "lineValueLabel";
      this.lineValueLabel.Size = new System.Drawing.Size(21, 13);
      this.lineValueLabel.TabIndex = 1;
      this.lineValueLabel.Text = "{0}";
      // 
      // lineLabel
      // 
      this.lineLabel.AutoSize = true;
      this.lineLabel.Location = new System.Drawing.Point(6, 19);
      this.lineLabel.Name = "lineLabel";
      this.lineLabel.Size = new System.Drawing.Size(30, 13);
      this.lineLabel.TabIndex = 0;
      this.lineLabel.Text = "Line:";
      // 
      // messageValueTextBox
      // 
      this.messageValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.messageValueTextBox.BackColor = System.Drawing.SystemColors.Control;
      this.messageValueTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.messageValueTextBox.Location = new System.Drawing.Point(68, 88);
      this.messageValueTextBox.Multiline = true;
      this.messageValueTextBox.Name = "messageValueTextBox";
      this.messageValueTextBox.ReadOnly = true;
      this.messageValueTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.messageValueTextBox.Size = new System.Drawing.Size(486, 118);
      this.messageValueTextBox.TabIndex = 7;
      this.messageValueTextBox.Text = "{0}";
      // 
      // infoTextBox
      // 
      this.infoTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.infoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.infoTextBox.Cursor = System.Windows.Forms.Cursors.Default;
      this.infoTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.infoTextBox.Location = new System.Drawing.Point(48, 17);
      this.infoTextBox.Name = "infoTextBox";
      this.infoTextBox.ReadOnly = true;
      this.infoTextBox.Size = new System.Drawing.Size(524, 13);
      this.infoTextBox.TabIndex = 2;
      this.infoTextBox.Text = "{0} was generated during compilation.";
      // 
      // okButton
      // 
      this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.okButton.Location = new System.Drawing.Point(497, 260);
      this.okButton.Name = "okButton";
      this.okButton.Size = new System.Drawing.Size(75, 23);
      this.okButton.TabIndex = 0;
      this.okButton.Text = "&OK";
      this.okButton.UseVisualStyleBackColor = true;
      // 
      // CompilerErrorDialog
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.CancelButton = this.okButton;
      this.ClientSize = new System.Drawing.Size(584, 295);
      this.Controls.Add(this.okButton);
      this.Controls.Add(this.infoTextBox);
      this.Controls.Add(this.detailsGroupBox);
      this.Controls.Add(this.iconLabel);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "CompilerErrorDialog";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "{0}";
      this.detailsGroupBox.ResumeLayout(false);
      this.detailsGroupBox.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label iconLabel;
    private System.Windows.Forms.GroupBox detailsGroupBox;
    private System.Windows.Forms.TextBox messageValueTextBox;
    private System.Windows.Forms.Label lineLabel;
    private System.Windows.Forms.Label columnLabel;
    private System.Windows.Forms.Label lineValueLabel;
    private System.Windows.Forms.Label codeLabel;
    private System.Windows.Forms.Label columnValueLabel;
    private System.Windows.Forms.LinkLabel codeValueLinkLabel;
    private System.Windows.Forms.Label messageLabel;
    private System.Windows.Forms.TextBox infoTextBox;
    private System.Windows.Forms.Button okButton;
  }
}