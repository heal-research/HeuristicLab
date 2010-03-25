namespace HeuristicLab.Hive.Server.ServerConsole {
  partial class CgCalendar {
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
      Calendar.DrawTool drawTool1 = new Calendar.DrawTool();
      this.dvOnline = new Calendar.DayView();
      this.btnSaveCal = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.txttimeTo = new System.Windows.Forms.DateTimePicker();
      this.txttimeFrom = new System.Windows.Forms.DateTimePicker();
      this.dtpTo = new System.Windows.Forms.DateTimePicker();
      this.dtpFrom = new System.Windows.Forms.DateTimePicker();
      this.chbade = new System.Windows.Forms.CheckBox();
      this.btnRecurrence = new System.Windows.Forms.Button();
      this.btbDelete = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.btCreate = new System.Windows.Forms.Button();
      this.mcOnline = new System.Windows.Forms.MonthCalendar();
      this.btnClearCal = new System.Windows.Forms.Button();
      this.cbx_forcePush = new System.Windows.Forms.CheckBox();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // dvOnline
      // 
      drawTool1.DayView = this.dvOnline;
      this.dvOnline.ActiveTool = drawTool1;
      this.dvOnline.AmPmDisplay = false;
      this.dvOnline.AppointmentDuration = Calendar.AppointmentSlotDuration.SixtyMinutes;
      this.dvOnline.AppointmentHeightMode = Calendar.AppHeightDrawMode.TrueHeightAll;
      this.dvOnline.DayHeadersHeight = 15;
      this.dvOnline.DaysToShow = 7;
      this.dvOnline.DrawAllAppBorder = false;
      this.dvOnline.EnableDurationDisplay = false;
      this.dvOnline.EnableRoundedCorners = false;
      this.dvOnline.EnableShadows = false;
      this.dvOnline.EnableTimeIndicator = false;
      this.dvOnline.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
      this.dvOnline.Location = new System.Drawing.Point(18, 185);
      this.dvOnline.MinHalfHourApp = false;
      this.dvOnline.Name = "dvOnline";
      this.dvOnline.SelectionEnd = new System.DateTime(((long)(0)));
      this.dvOnline.SelectionStart = new System.DateTime(((long)(0)));
      this.dvOnline.Size = new System.Drawing.Size(823, 354);
      this.dvOnline.StartDate = new System.DateTime(((long)(0)));
      this.dvOnline.TabIndex = 42;
      this.dvOnline.OnSelectionChanged += new System.EventHandler<System.EventArgs>(this.dvOnline_OnSelectionChanged);
      // 
      // btnSaveCal
      // 
      this.btnSaveCal.Location = new System.Drawing.Point(642, 49);
      this.btnSaveCal.Name = "btnSaveCal";
      this.btnSaveCal.Size = new System.Drawing.Size(199, 23);
      this.btnSaveCal.TabIndex = 45;
      this.btnSaveCal.Text = "Save Calendar";
      this.btnSaveCal.UseVisualStyleBackColor = true;
      this.btnSaveCal.Click += new System.EventHandler(this.btnSaveCal_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.txttimeTo);
      this.groupBox1.Controls.Add(this.txttimeFrom);
      this.groupBox1.Controls.Add(this.dtpTo);
      this.groupBox1.Controls.Add(this.dtpFrom);
      this.groupBox1.Controls.Add(this.chbade);
      this.groupBox1.Controls.Add(this.btnRecurrence);
      this.groupBox1.Controls.Add(this.btbDelete);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.btCreate);
      this.groupBox1.Location = new System.Drawing.Point(382, 13);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(254, 161);
      this.groupBox1.TabIndex = 44;
      this.groupBox1.TabStop = false;
      // 
      // txttimeTo
      // 
      this.txttimeTo.CustomFormat = "HH:00";
      this.txttimeTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.txttimeTo.Location = new System.Drawing.Point(164, 45);
      this.txttimeTo.Name = "txttimeTo";
      this.txttimeTo.ShowUpDown = true;
      this.txttimeTo.Size = new System.Drawing.Size(73, 20);
      this.txttimeTo.TabIndex = 40;
      // 
      // txttimeFrom
      // 
      this.txttimeFrom.CustomFormat = "HH:00";
      this.txttimeFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.txttimeFrom.Location = new System.Drawing.Point(164, 12);
      this.txttimeFrom.Name = "txttimeFrom";
      this.txttimeFrom.ShowUpDown = true;
      this.txttimeFrom.Size = new System.Drawing.Size(73, 20);
      this.txttimeFrom.TabIndex = 39;
      // 
      // dtpTo
      // 
      this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.dtpTo.Location = new System.Drawing.Point(72, 45);
      this.dtpTo.Name = "dtpTo";
      this.dtpTo.Size = new System.Drawing.Size(89, 20);
      this.dtpTo.TabIndex = 33;
      // 
      // dtpFrom
      // 
      this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.dtpFrom.Location = new System.Drawing.Point(72, 12);
      this.dtpFrom.Name = "dtpFrom";
      this.dtpFrom.Size = new System.Drawing.Size(89, 20);
      this.dtpFrom.TabIndex = 32;
      // 
      // chbade
      // 
      this.chbade.AutoSize = true;
      this.chbade.Location = new System.Drawing.Point(135, 70);
      this.chbade.Name = "chbade";
      this.chbade.Size = new System.Drawing.Size(90, 17);
      this.chbade.TabIndex = 31;
      this.chbade.Text = "All Day Event";
      this.chbade.UseVisualStyleBackColor = true;
      this.chbade.CheckedChanged += new System.EventHandler(this.chbade_CheckedChanged);
      // 
      // btnRecurrence
      // 
      this.btnRecurrence.Location = new System.Drawing.Point(135, 129);
      this.btnRecurrence.Name = "btnRecurrence";
      this.btnRecurrence.Size = new System.Drawing.Size(113, 26);
      this.btnRecurrence.TabIndex = 30;
      this.btnRecurrence.Text = "Recurrence";
      this.btnRecurrence.UseVisualStyleBackColor = true;
      this.btnRecurrence.Click += new System.EventHandler(this.btnRecurrence_Click);
      // 
      // btbDelete
      // 
      this.btbDelete.Location = new System.Drawing.Point(8, 129);
      this.btbDelete.Name = "btbDelete";
      this.btbDelete.Size = new System.Drawing.Size(114, 26);
      this.btbDelete.TabIndex = 25;
      this.btbDelete.Text = "Delete";
      this.btbDelete.UseVisualStyleBackColor = true;
      this.btbDelete.Click += new System.EventHandler(this.btbDelete_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(16, 46);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(55, 13);
      this.label2.TabIndex = 23;
      this.label2.Text = "End Time:";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(16, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(58, 13);
      this.label1.TabIndex = 21;
      this.label1.Text = "Start Time:";
      // 
      // btCreate
      // 
      this.btCreate.Location = new System.Drawing.Point(6, 93);
      this.btCreate.Name = "btCreate";
      this.btCreate.Size = new System.Drawing.Size(242, 26);
      this.btCreate.TabIndex = 20;
      this.btCreate.Text = "Save Appointment";
      this.btCreate.UseVisualStyleBackColor = true;
      this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
      // 
      // mcOnline
      // 
      this.mcOnline.CalendarDimensions = new System.Drawing.Size(2, 1);
      this.mcOnline.Location = new System.Drawing.Point(18, 18);
      this.mcOnline.Name = "mcOnline";
      this.mcOnline.TabIndex = 43;
      this.mcOnline.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.mcOnline_DateChanged);
      // 
      // btnClearCal
      // 
      this.btnClearCal.Location = new System.Drawing.Point(642, 18);
      this.btnClearCal.Name = "btnClearCal";
      this.btnClearCal.Size = new System.Drawing.Size(199, 23);
      this.btnClearCal.TabIndex = 46;
      this.btnClearCal.Text = "Clear Calendar";
      this.btnClearCal.UseVisualStyleBackColor = true;
      this.btnClearCal.Click += new System.EventHandler(this.btnClearCal_Click);
      // 
      // cbx_forcePush
      // 
      this.cbx_forcePush.AutoSize = true;
      this.cbx_forcePush.Location = new System.Drawing.Point(642, 78);
      this.cbx_forcePush.Name = "cbx_forcePush";
      this.cbx_forcePush.Size = new System.Drawing.Size(80, 17);
      this.cbx_forcePush.TabIndex = 47;
      this.cbx_forcePush.Text = "Force Push";
      this.cbx_forcePush.UseVisualStyleBackColor = true;
      // 
      // CgCalendar
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(859, 558);
      this.Controls.Add(this.cbx_forcePush);
      this.Controls.Add(this.btnClearCal);
      this.Controls.Add(this.btnSaveCal);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.mcOnline);
      this.Controls.Add(this.dvOnline);
      this.Name = "CgCalendar";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnSaveCal;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.DateTimePicker txttimeTo;
    private System.Windows.Forms.DateTimePicker txttimeFrom;
    private System.Windows.Forms.DateTimePicker dtpTo;
    private System.Windows.Forms.DateTimePicker dtpFrom;
    private System.Windows.Forms.CheckBox chbade;
    private System.Windows.Forms.Button btnRecurrence;
    private System.Windows.Forms.Button btbDelete;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btCreate;
    private System.Windows.Forms.MonthCalendar mcOnline;
    private Calendar.DayView dvOnline;
    private System.Windows.Forms.Button btnClearCal;
    private System.Windows.Forms.CheckBox cbx_forcePush;
  }
}