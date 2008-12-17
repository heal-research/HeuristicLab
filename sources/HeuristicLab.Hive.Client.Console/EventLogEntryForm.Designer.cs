namespace HeuristicLab.Hive.Client.Console {
  partial class EventLogEntryForm {
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
      this.lbDateLabel = new System.Windows.Forms.Label();
      this.lbDate = new System.Windows.Forms.Label();
      this.lbId = new System.Windows.Forms.Label();
      this.lbIdLabel = new System.Windows.Forms.Label();
      this.txtMessage = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // lbDateLabel
      // 
      this.lbDateLabel.AutoSize = true;
      this.lbDateLabel.Location = new System.Drawing.Point(13, 27);
      this.lbDateLabel.Name = "lbDateLabel";
      this.lbDateLabel.Size = new System.Drawing.Size(80, 13);
      this.lbDateLabel.TabIndex = 1;
      this.lbDateLabel.Text = "Date and Time:";
      // 
      // lbDate
      // 
      this.lbDate.AutoSize = true;
      this.lbDate.Location = new System.Drawing.Point(89, 27);
      this.lbDate.Name = "lbDate";
      this.lbDate.Size = new System.Drawing.Size(23, 13);
      this.lbDate.TabIndex = 3;
      this.lbDate.Text = "null";
      // 
      // lbId
      // 
      this.lbId.AutoSize = true;
      this.lbId.Location = new System.Drawing.Point(27, 9);
      this.lbId.Name = "lbId";
      this.lbId.Size = new System.Drawing.Size(23, 13);
      this.lbId.TabIndex = 6;
      this.lbId.Text = "null";
      // 
      // lbIdLabel
      // 
      this.lbIdLabel.AutoSize = true;
      this.lbIdLabel.Location = new System.Drawing.Point(13, 9);
      this.lbIdLabel.Name = "lbIdLabel";
      this.lbIdLabel.Size = new System.Drawing.Size(19, 13);
      this.lbIdLabel.TabIndex = 5;
      this.lbIdLabel.Text = "Id:";
      // 
      // txtMessage
      // 
      this.txtMessage.BackColor = System.Drawing.SystemColors.InactiveBorder;
      this.txtMessage.Location = new System.Drawing.Point(16, 55);
      this.txtMessage.Multiline = true;
      this.txtMessage.Name = "txtMessage";
      this.txtMessage.ReadOnly = true;
      this.txtMessage.Size = new System.Drawing.Size(250, 179);
      this.txtMessage.TabIndex = 7;
      // 
      // EventLogEntryForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(278, 246);
      this.Controls.Add(this.txtMessage);
      this.Controls.Add(this.lbId);
      this.Controls.Add(this.lbIdLabel);
      this.Controls.Add(this.lbDate);
      this.Controls.Add(this.lbDateLabel);
      this.Name = "EventLogEntryForm";
      this.Text = "Detail";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lbDateLabel;
    private System.Windows.Forms.Label lbDate;
    private System.Windows.Forms.Label lbId;
    private System.Windows.Forms.Label lbIdLabel;
    private System.Windows.Forms.TextBox txtMessage;
  }
}