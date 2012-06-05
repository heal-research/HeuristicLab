namespace HeuristicLab.Problems.ExternalEvaluation.Views {
  partial class EvaluationTCPChannelView {
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
      this.ipAddressLabel = new System.Windows.Forms.Label();
      this.portLabel = new System.Windows.Forms.Label();
      this.ipAddressTextBox = new System.Windows.Forms.TextBox();
      this.portTextBox = new System.Windows.Forms.TextBox();
      this.connectButton = new System.Windows.Forms.Button();
      this.disconnectButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
      this.SuspendLayout();
      // 
      // nameTextBox
      // 
      this.errorProvider.SetIconAlignment(this.nameTextBox, System.Windows.Forms.ErrorIconAlignment.MiddleLeft);
      this.errorProvider.SetIconPadding(this.nameTextBox, 2);
      this.nameTextBox.Location = new System.Drawing.Point(70, 0);
      this.nameTextBox.Size = new System.Drawing.Size(256, 20);
      // 
      // ipAddressLabel
      // 
      this.ipAddressLabel.AutoSize = true;
      this.ipAddressLabel.Location = new System.Drawing.Point(3, 29);
      this.ipAddressLabel.Name = "ipAddressLabel";
      this.ipAddressLabel.Size = new System.Drawing.Size(61, 13);
      this.ipAddressLabel.TabIndex = 3;
      this.ipAddressLabel.Text = "IP Address:";
      // 
      // portLabel
      // 
      this.portLabel.AutoSize = true;
      this.portLabel.Location = new System.Drawing.Point(3, 55);
      this.portLabel.Name = "portLabel";
      this.portLabel.Size = new System.Drawing.Size(29, 13);
      this.portLabel.TabIndex = 5;
      this.portLabel.Text = "Port:";
      // 
      // ipAddressTextBox
      // 
      this.ipAddressTextBox.Location = new System.Drawing.Point(70, 26);
      this.ipAddressTextBox.Name = "ipAddressTextBox";
      this.ipAddressTextBox.Size = new System.Drawing.Size(281, 20);
      this.ipAddressTextBox.TabIndex = 4;
      this.ipAddressTextBox.Text = "127.0.0.1";
      this.ipAddressTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.ipAddressTextBox_Validating);
      // 
      // portTextBox
      // 
      this.portTextBox.Location = new System.Drawing.Point(70, 52);
      this.portTextBox.Name = "portTextBox";
      this.portTextBox.Size = new System.Drawing.Size(281, 20);
      this.portTextBox.TabIndex = 6;
      this.portTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.portTextBox_Validating);
      // 
      // connectButton
      // 
      this.connectButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Play;
      this.connectButton.Location = new System.Drawing.Point(70, 78);
      this.connectButton.Name = "connectButton";
      this.connectButton.Size = new System.Drawing.Size(26, 23);
      this.connectButton.TabIndex = 7;
      this.connectButton.UseVisualStyleBackColor = true;
      this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
      // 
      // disconnectButton
      // 
      this.disconnectButton.Image = HeuristicLab.Common.Resources.VSImageLibrary.Stop;
      this.disconnectButton.Location = new System.Drawing.Point(102, 78);
      this.disconnectButton.Name = "disconnectButton";
      this.disconnectButton.Size = new System.Drawing.Size(26, 23);
      this.disconnectButton.TabIndex = 8;
      this.disconnectButton.UseVisualStyleBackColor = true;
      this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
      // 
      // EvaluationTCPChannelView
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.ipAddressLabel);
      this.Controls.Add(this.portTextBox);
      this.Controls.Add(this.portLabel);
      this.Controls.Add(this.connectButton);
      this.Controls.Add(this.ipAddressTextBox);
      this.Controls.Add(this.disconnectButton);
      this.Name = "EvaluationTCPChannelView";
      this.Size = new System.Drawing.Size(351, 105);
      this.Controls.SetChildIndex(this.disconnectButton, 0);
      this.Controls.SetChildIndex(this.infoLabel, 0);
      this.Controls.SetChildIndex(this.ipAddressTextBox, 0);
      this.Controls.SetChildIndex(this.connectButton, 0);
      this.Controls.SetChildIndex(this.portLabel, 0);
      this.Controls.SetChildIndex(this.portTextBox, 0);
      this.Controls.SetChildIndex(this.ipAddressLabel, 0);
      this.Controls.SetChildIndex(this.nameLabel, 0);
      this.Controls.SetChildIndex(this.nameTextBox, 0);
      ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label ipAddressLabel;
    private System.Windows.Forms.Label portLabel;
    private System.Windows.Forms.TextBox ipAddressTextBox;
    private System.Windows.Forms.TextBox portTextBox;
    private System.Windows.Forms.Button connectButton;
    private System.Windows.Forms.Button disconnectButton;
  }
}
