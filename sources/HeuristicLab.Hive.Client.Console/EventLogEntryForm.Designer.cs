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
      this.lbMessage = new System.Windows.Forms.ListBox();
      this.lbDateLabel = new System.Windows.Forms.Label();
      this.lbTimeLabel = new System.Windows.Forms.Label();
      this.lbDate = new System.Windows.Forms.Label();
      this.lbTime = new System.Windows.Forms.Label();
      this.lbId = new System.Windows.Forms.Label();
      this.lbIdLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lbMessage
      // 
      this.lbMessage.BackColor = System.Drawing.SystemColors.InactiveBorder;
      this.lbMessage.FormattingEnabled = true;
      this.lbMessage.Location = new System.Drawing.Point(12, 97);
      this.lbMessage.MultiColumn = true;
      this.lbMessage.Name = "lbMessage";
      this.lbMessage.Size = new System.Drawing.Size(254, 160);
      this.lbMessage.TabIndex = 0;
      // 
      // lbDateLabel
      // 
      this.lbDateLabel.AutoSize = true;
      this.lbDateLabel.Location = new System.Drawing.Point(13, 13);
      this.lbDateLabel.Name = "lbDateLabel";
      this.lbDateLabel.Size = new System.Drawing.Size(33, 13);
      this.lbDateLabel.TabIndex = 1;
      this.lbDateLabel.Text = "Date:";
      // 
      // lbTimeLabel
      // 
      this.lbTimeLabel.AutoSize = true;
      this.lbTimeLabel.Location = new System.Drawing.Point(13, 37);
      this.lbTimeLabel.Name = "lbTimeLabel";
      this.lbTimeLabel.Size = new System.Drawing.Size(33, 13);
      this.lbTimeLabel.TabIndex = 2;
      this.lbTimeLabel.Text = "Time:";
      // 
      // lbDate
      // 
      this.lbDate.AutoSize = true;
      this.lbDate.Location = new System.Drawing.Point(52, 13);
      this.lbDate.Name = "lbDate";
      this.lbDate.Size = new System.Drawing.Size(33, 13);
      this.lbDate.TabIndex = 3;
      this.lbDate.Text = "Date:";
      // 
      // lbTime
      // 
      this.lbTime.AutoSize = true;
      this.lbTime.Location = new System.Drawing.Point(52, 37);
      this.lbTime.Name = "lbTime";
      this.lbTime.Size = new System.Drawing.Size(33, 13);
      this.lbTime.TabIndex = 4;
      this.lbTime.Text = "Time:";
      // 
      // lbId
      // 
      this.lbId.AutoSize = true;
      this.lbId.Location = new System.Drawing.Point(52, 60);
      this.lbId.Name = "lbId";
      this.lbId.Size = new System.Drawing.Size(19, 13);
      this.lbId.TabIndex = 6;
      this.lbId.Text = "Id:";
      // 
      // lbIdLabel
      // 
      this.lbIdLabel.AutoSize = true;
      this.lbIdLabel.Location = new System.Drawing.Point(13, 60);
      this.lbIdLabel.Name = "lbIdLabel";
      this.lbIdLabel.Size = new System.Drawing.Size(19, 13);
      this.lbIdLabel.TabIndex = 5;
      this.lbIdLabel.Text = "Id:";
      // 
      // EventLogEntryForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(278, 269);
      this.Controls.Add(this.lbId);
      this.Controls.Add(this.lbIdLabel);
      this.Controls.Add(this.lbTime);
      this.Controls.Add(this.lbDate);
      this.Controls.Add(this.lbTimeLabel);
      this.Controls.Add(this.lbDateLabel);
      this.Controls.Add(this.lbMessage);
      this.Name = "EventLogEntryForm";
      this.Text = "Eventlog Entry Details";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox lbMessage;
    private System.Windows.Forms.Label lbDateLabel;
    private System.Windows.Forms.Label lbTimeLabel;
    private System.Windows.Forms.Label lbDate;
    private System.Windows.Forms.Label lbTime;
    private System.Windows.Forms.Label lbId;
    private System.Windows.Forms.Label lbIdLabel;
  }
}