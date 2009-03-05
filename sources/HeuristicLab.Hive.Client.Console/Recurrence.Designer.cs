namespace HeuristicLab.Hive.Client.Console {
  partial class Recurrence {
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
      this.cmbEnd = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.gbAppointment = new System.Windows.Forms.GroupBox();
      this.cmbDuration = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.gbRecurrence = new System.Windows.Forms.GroupBox();
      this.rbtWeekly = new System.Windows.Forms.RadioButton();
      this.rbtDaily = new System.Windows.Forms.RadioButton();
      this.txtDays = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.gbDaily = new System.Windows.Forms.GroupBox();
      this.gbWeekly = new System.Windows.Forms.GroupBox();
      this.cbSunday = new System.Windows.Forms.CheckBox();
      this.cbSaturday = new System.Windows.Forms.CheckBox();
      this.cbFriday = new System.Windows.Forms.CheckBox();
      this.cbThursday = new System.Windows.Forms.CheckBox();
      this.cbWednesday = new System.Windows.Forms.CheckBox();
      this.cbTuesday = new System.Windows.Forms.CheckBox();
      this.cbMonday = new System.Windows.Forms.CheckBox();
      this.label7 = new System.Windows.Forms.Label();
      this.gbDuration = new System.Windows.Forms.GroupBox();
      this.dtpEnd = new System.Windows.Forms.DateTimePicker();
      this.dtpStart = new System.Windows.Forms.DateTimePicker();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.cmbEndTime = new System.Windows.Forms.ComboBox();
      this.cmbBeginTime = new System.Windows.Forms.ComboBox();
      this.gbAppointment.SuspendLayout();
      this.gbRecurrence.SuspendLayout();
      this.gbDaily.SuspendLayout();
      this.gbWeekly.SuspendLayout();
      this.gbDuration.SuspendLayout();
      this.SuspendLayout();
      // 
      // cmbEnd
      // 
      this.cmbEnd.AutoSize = true;
      this.cmbEnd.Location = new System.Drawing.Point(129, 22);
      this.cmbEnd.Name = "cmbEnd";
      this.cmbEnd.Size = new System.Drawing.Size(29, 13);
      this.cmbEnd.TabIndex = 34;
      this.cmbEnd.Text = "End:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(7, 22);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(37, 13);
      this.label1.TabIndex = 32;
      this.label1.Text = "Begin:";
      // 
      // gbAppointment
      // 
      this.gbAppointment.Controls.Add(this.cmbEndTime);
      this.gbAppointment.Controls.Add(this.cmbBeginTime);
      this.gbAppointment.Controls.Add(this.cmbDuration);
      this.gbAppointment.Controls.Add(this.label3);
      this.gbAppointment.Controls.Add(this.label1);
      this.gbAppointment.Controls.Add(this.cmbEnd);
      this.gbAppointment.Location = new System.Drawing.Point(2, 2);
      this.gbAppointment.Name = "gbAppointment";
      this.gbAppointment.Size = new System.Drawing.Size(386, 50);
      this.gbAppointment.TabIndex = 39;
      this.gbAppointment.TabStop = false;
      this.gbAppointment.Text = "Appointment";
      // 
      // cmbDuration
      // 
      this.cmbDuration.FormattingEnabled = true;
      this.cmbDuration.Location = new System.Drawing.Point(303, 18);
      this.cmbDuration.Name = "cmbDuration";
      this.cmbDuration.Size = new System.Drawing.Size(77, 21);
      this.cmbDuration.TabIndex = 37;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(255, 21);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(50, 13);
      this.label3.TabIndex = 36;
      this.label3.Text = "Duration:";
      // 
      // gbRecurrence
      // 
      this.gbRecurrence.Controls.Add(this.rbtWeekly);
      this.gbRecurrence.Controls.Add(this.rbtDaily);
      this.gbRecurrence.Location = new System.Drawing.Point(2, 59);
      this.gbRecurrence.Name = "gbRecurrence";
      this.gbRecurrence.Size = new System.Drawing.Size(123, 94);
      this.gbRecurrence.TabIndex = 40;
      this.gbRecurrence.TabStop = false;
      this.gbRecurrence.Text = "Recurrence Pattern";
      // 
      // rbtWeekly
      // 
      this.rbtWeekly.AutoSize = true;
      this.rbtWeekly.Location = new System.Drawing.Point(11, 58);
      this.rbtWeekly.Name = "rbtWeekly";
      this.rbtWeekly.Size = new System.Drawing.Size(61, 17);
      this.rbtWeekly.TabIndex = 1;
      this.rbtWeekly.Text = "Weekly";
      this.rbtWeekly.UseVisualStyleBackColor = true;
      this.rbtWeekly.CheckedChanged += new System.EventHandler(this.rbtDaily_CheckedChanged);
      // 
      // rbtDaily
      // 
      this.rbtDaily.AutoSize = true;
      this.rbtDaily.Checked = true;
      this.rbtDaily.Location = new System.Drawing.Point(11, 34);
      this.rbtDaily.Name = "rbtDaily";
      this.rbtDaily.Size = new System.Drawing.Size(48, 17);
      this.rbtDaily.TabIndex = 0;
      this.rbtDaily.TabStop = true;
      this.rbtDaily.Text = "Daily";
      this.rbtDaily.UseVisualStyleBackColor = true;
      this.rbtDaily.CheckedChanged += new System.EventHandler(this.rbtDaily_CheckedChanged);
      // 
      // txtDays
      // 
      this.txtDays.Location = new System.Drawing.Point(50, 17);
      this.txtDays.Name = "txtDays";
      this.txtDays.Size = new System.Drawing.Size(30, 20);
      this.txtDays.TabIndex = 24;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(10, 20);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(34, 13);
      this.label4.TabIndex = 23;
      this.label4.Text = "Every";
      // 
      // gbDaily
      // 
      this.gbDaily.Controls.Add(this.label7);
      this.gbDaily.Controls.Add(this.txtDays);
      this.gbDaily.Controls.Add(this.label4);
      this.gbDaily.Location = new System.Drawing.Point(123, 59);
      this.gbDaily.Name = "gbDaily";
      this.gbDaily.Size = new System.Drawing.Size(265, 94);
      this.gbDaily.TabIndex = 41;
      this.gbDaily.TabStop = false;
      // 
      // gbWeekly
      // 
      this.gbWeekly.Controls.Add(this.cbSunday);
      this.gbWeekly.Controls.Add(this.cbSaturday);
      this.gbWeekly.Controls.Add(this.cbFriday);
      this.gbWeekly.Controls.Add(this.cbThursday);
      this.gbWeekly.Controls.Add(this.cbWednesday);
      this.gbWeekly.Controls.Add(this.cbTuesday);
      this.gbWeekly.Controls.Add(this.cbMonday);
      this.gbWeekly.Location = new System.Drawing.Point(123, 59);
      this.gbWeekly.Name = "gbWeekly";
      this.gbWeekly.Size = new System.Drawing.Size(265, 94);
      this.gbWeekly.TabIndex = 42;
      this.gbWeekly.TabStop = false;
      this.gbWeekly.Visible = false;
      // 
      // cbSunday
      // 
      this.cbSunday.AutoSize = true;
      this.cbSunday.Location = new System.Drawing.Point(11, 58);
      this.cbSunday.Name = "cbSunday";
      this.cbSunday.Size = new System.Drawing.Size(62, 17);
      this.cbSunday.TabIndex = 6;
      this.cbSunday.Text = "Sunday";
      this.cbSunday.UseVisualStyleBackColor = true;
      // 
      // cbSaturday
      // 
      this.cbSaturday.AutoSize = true;
      this.cbSaturday.Location = new System.Drawing.Point(173, 37);
      this.cbSaturday.Name = "cbSaturday";
      this.cbSaturday.Size = new System.Drawing.Size(68, 17);
      this.cbSaturday.TabIndex = 5;
      this.cbSaturday.Text = "Saturday";
      this.cbSaturday.UseVisualStyleBackColor = true;
      // 
      // cbFriday
      // 
      this.cbFriday.AutoSize = true;
      this.cbFriday.Location = new System.Drawing.Point(97, 37);
      this.cbFriday.Name = "cbFriday";
      this.cbFriday.Size = new System.Drawing.Size(54, 17);
      this.cbFriday.TabIndex = 4;
      this.cbFriday.Text = "Friday";
      this.cbFriday.UseVisualStyleBackColor = true;
      // 
      // cbThursday
      // 
      this.cbThursday.AutoSize = true;
      this.cbThursday.Location = new System.Drawing.Point(11, 37);
      this.cbThursday.Name = "cbThursday";
      this.cbThursday.Size = new System.Drawing.Size(70, 17);
      this.cbThursday.TabIndex = 3;
      this.cbThursday.Text = "Thursday";
      this.cbThursday.UseVisualStyleBackColor = true;
      // 
      // cbWednesday
      // 
      this.cbWednesday.AutoSize = true;
      this.cbWednesday.Location = new System.Drawing.Point(173, 16);
      this.cbWednesday.Name = "cbWednesday";
      this.cbWednesday.Size = new System.Drawing.Size(86, 17);
      this.cbWednesday.TabIndex = 2;
      this.cbWednesday.Text = "Wednesday ";
      this.cbWednesday.UseVisualStyleBackColor = true;
      // 
      // cbTuesday
      // 
      this.cbTuesday.AutoSize = true;
      this.cbTuesday.Location = new System.Drawing.Point(97, 16);
      this.cbTuesday.Name = "cbTuesday";
      this.cbTuesday.Size = new System.Drawing.Size(67, 17);
      this.cbTuesday.TabIndex = 1;
      this.cbTuesday.Text = "Tuesday";
      this.cbTuesday.UseVisualStyleBackColor = true;
      // 
      // cbMonday
      // 
      this.cbMonday.AutoSize = true;
      this.cbMonday.Location = new System.Drawing.Point(11, 16);
      this.cbMonday.Name = "cbMonday";
      this.cbMonday.Size = new System.Drawing.Size(64, 17);
      this.cbMonday.TabIndex = 0;
      this.cbMonday.Text = "Monday";
      this.cbMonday.UseVisualStyleBackColor = true;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(86, 20);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(37, 13);
      this.label7.TabIndex = 25;
      this.label7.Text = "Day(s)";
      // 
      // gbDuration
      // 
      this.gbDuration.Controls.Add(this.dtpEnd);
      this.gbDuration.Controls.Add(this.dtpStart);
      this.gbDuration.Controls.Add(this.label5);
      this.gbDuration.Controls.Add(this.label6);
      this.gbDuration.Location = new System.Drawing.Point(2, 160);
      this.gbDuration.Name = "gbDuration";
      this.gbDuration.Size = new System.Drawing.Size(386, 50);
      this.gbDuration.TabIndex = 42;
      this.gbDuration.TabStop = false;
      this.gbDuration.Text = "Duration";
      // 
      // dtpEnd
      // 
      this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dtpEnd.Location = new System.Drawing.Point(250, 18);
      this.dtpEnd.Name = "dtpEnd";
      this.dtpEnd.Size = new System.Drawing.Size(91, 20);
      this.dtpEnd.TabIndex = 29;
      // 
      // dtpStart
      // 
      this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.dtpStart.Location = new System.Drawing.Point(69, 18);
      this.dtpStart.Name = "dtpStart";
      this.dtpStart.Size = new System.Drawing.Size(89, 20);
      this.dtpStart.TabIndex = 28;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(189, 22);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(55, 13);
      this.label5.TabIndex = 27;
      this.label5.Text = "End Date:";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(14, 22);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(58, 13);
      this.label6.TabIndex = 25;
      this.label6.Text = "Start Date:";
      // 
      // cmbEndTime
      // 
      this.cmbEndTime.FormattingEnabled = true;
      this.cmbEndTime.Location = new System.Drawing.Point(155, 18);
      this.cmbEndTime.Name = "cmbEndTime";
      this.cmbEndTime.Size = new System.Drawing.Size(77, 21);
      this.cmbEndTime.TabIndex = 39;
      // 
      // cmbBeginTime
      // 
      this.cmbBeginTime.FormattingEnabled = true;
      this.cmbBeginTime.Location = new System.Drawing.Point(46, 18);
      this.cmbBeginTime.Name = "cmbBeginTime";
      this.cmbBeginTime.Size = new System.Drawing.Size(77, 21);
      this.cmbBeginTime.TabIndex = 38;
      // 
      // Recurrence
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(390, 214);
      this.Controls.Add(this.gbWeekly);
      this.Controls.Add(this.gbDuration);
      this.Controls.Add(this.gbDaily);
      this.Controls.Add(this.gbRecurrence);
      this.Controls.Add(this.gbAppointment);
      this.Name = "Recurrence";
      this.Text = "Recurrence";
      this.gbAppointment.ResumeLayout(false);
      this.gbAppointment.PerformLayout();
      this.gbRecurrence.ResumeLayout(false);
      this.gbRecurrence.PerformLayout();
      this.gbDaily.ResumeLayout(false);
      this.gbDaily.PerformLayout();
      this.gbWeekly.ResumeLayout(false);
      this.gbWeekly.PerformLayout();
      this.gbDuration.ResumeLayout(false);
      this.gbDuration.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label cmbEnd;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox gbAppointment;
    private System.Windows.Forms.ComboBox cmbDuration;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox gbRecurrence;
    private System.Windows.Forms.RadioButton rbtWeekly;
    private System.Windows.Forms.RadioButton rbtDaily;
    private System.Windows.Forms.TextBox txtDays;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.GroupBox gbDaily;
    private System.Windows.Forms.GroupBox gbDuration;
    private System.Windows.Forms.DateTimePicker dtpEnd;
    private System.Windows.Forms.DateTimePicker dtpStart;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.GroupBox gbWeekly;
    private System.Windows.Forms.CheckBox cbSunday;
    private System.Windows.Forms.CheckBox cbSaturday;
    private System.Windows.Forms.CheckBox cbFriday;
    private System.Windows.Forms.CheckBox cbThursday;
    private System.Windows.Forms.CheckBox cbWednesday;
    private System.Windows.Forms.CheckBox cbTuesday;
    private System.Windows.Forms.CheckBox cbMonday;
    private System.Windows.Forms.ComboBox cmbEndTime;
    private System.Windows.Forms.ComboBox cmbBeginTime;
  }
}