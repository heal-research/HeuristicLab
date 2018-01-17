#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2018 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  partial class ScheduleView {
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
      this.components = new System.ComponentModel.Container();
      Calendar.DrawTool drawTool1 = new Calendar.DrawTool();
      this.dvOnline = new Calendar.DayView();
      this.txttimeTo = new System.Windows.Forms.DateTimePicker();
      this.txttimeFrom = new System.Windows.Forms.DateTimePicker();
      this.dtpTo = new System.Windows.Forms.DateTimePicker();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.dtpFrom = new System.Windows.Forms.DateTimePicker();
      this.chbade = new System.Windows.Forms.CheckBox();
      this.btnRecurrence = new System.Windows.Forms.Button();
      this.btbDelete = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.btCreate = new System.Windows.Forms.Button();
      this.btnSaveCal = new System.Windows.Forms.Button();
      this.btnClearCal = new System.Windows.Forms.Button();
      this.mcOnline = new System.Windows.Forms.MonthCalendar();
      this.toolTip = new System.Windows.Forms.ToolTip(this.components);
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // dvOnline
      // 
      drawTool1.DayView = this.dvOnline;
      this.dvOnline.ActiveTool = drawTool1;
      this.dvOnline.AmPmDisplay = false;
      this.dvOnline.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
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
      this.dvOnline.Location = new System.Drawing.Point(3, 183);
      this.dvOnline.MinHalfHourApp = false;
      this.dvOnline.Name = "dvOnline";
      this.dvOnline.SelectionEnd = new System.DateTime(((long)(0)));
      this.dvOnline.SelectionStart = new System.DateTime(((long)(0)));
      this.dvOnline.Size = new System.Drawing.Size(836, 354);
      this.dvOnline.StartDate = new System.DateTime(((long)(0)));
      this.dvOnline.TabIndex = 54;
      this.dvOnline.OnSelectionChanged += new System.EventHandler<System.EventArgs>(this.dvOnline_OnSelectionChanged);
      // 
      // txttimeTo
      // 
      this.txttimeTo.CustomFormat = "HH:mm";
      this.txttimeTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.txttimeTo.Location = new System.Drawing.Point(164, 45);
      this.txttimeTo.Name = "txttimeTo";
      this.txttimeTo.ShowUpDown = true;
      this.txttimeTo.Size = new System.Drawing.Size(73, 20);
      this.txttimeTo.TabIndex = 40;
      // 
      // txttimeFrom
      // 
      this.txttimeFrom.CustomFormat = "HH:mm";
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
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
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
      this.groupBox1.Location = new System.Drawing.Point(375, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(259, 168);
      this.groupBox1.TabIndex = 56;
      this.groupBox1.TabStop = false;
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
      this.btnRecurrence.Image = HeuristicLab.Common.Resources.VSImageLibrary.Timer;
      this.btnRecurrence.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnRecurrence.Location = new System.Drawing.Point(135, 129);
      this.btnRecurrence.Name = "btnRecurrence";
      this.btnRecurrence.Size = new System.Drawing.Size(113, 26);
      this.btnRecurrence.TabIndex = 30;
      this.btnRecurrence.Text = "Recurrence";
      this.toolTip.SetToolTip(this.btnRecurrence, "Create recurring appointments");
      this.btnRecurrence.UseVisualStyleBackColor = true;
      this.btnRecurrence.Click += new System.EventHandler(this.btnRecurrence_Click);
      // 
      // btbDelete
      // 
      this.btbDelete.Image = HeuristicLab.Common.Resources.VSImageLibrary.Delete;
      this.btbDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btbDelete.Location = new System.Drawing.Point(8, 129);
      this.btbDelete.Name = "btbDelete";
      this.btbDelete.Size = new System.Drawing.Size(114, 26);
      this.btbDelete.TabIndex = 25;
      this.btbDelete.Text = "Delete";
      this.toolTip.SetToolTip(this.btbDelete, "Delete selected appointment");
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
      this.btCreate.Image = HeuristicLab.Common.Resources.VSImageLibrary.Add;
      this.btCreate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btCreate.Location = new System.Drawing.Point(6, 93);
      this.btCreate.Name = "btCreate";
      this.btCreate.Size = new System.Drawing.Size(242, 26);
      this.btCreate.TabIndex = 20;
      this.btCreate.Text = "Create Downtime";
      this.toolTip.SetToolTip(this.btCreate, "Create a new downtime in the calender");
      this.btCreate.UseVisualStyleBackColor = true;
      this.btCreate.Click += new System.EventHandler(this.btCreate_Click);
      // 
      // btnSaveCal
      // 
      this.btnSaveCal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSaveCal.Image = HeuristicLab.Common.Resources.VSImageLibrary.PublishToWeb;
      this.btnSaveCal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnSaveCal.Location = new System.Drawing.Point(640, 41);
      this.btnSaveCal.Name = "btnSaveCal";
      this.btnSaveCal.Size = new System.Drawing.Size(199, 26);
      this.btnSaveCal.TabIndex = 57;
      this.btnSaveCal.Text = "Save Calendar on Server";
      this.toolTip.SetToolTip(this.btnSaveCal, "Store the calender on the server");
      this.btnSaveCal.UseVisualStyleBackColor = true;
      this.btnSaveCal.Click += new System.EventHandler(this.btnSaveCal_Click);
      // 
      // btnClearCal
      // 
      this.btnClearCal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClearCal.Image = HeuristicLab.Common.Resources.VSImageLibrary.Document;
      this.btnClearCal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnClearCal.Location = new System.Drawing.Point(640, 9);
      this.btnClearCal.Name = "btnClearCal";
      this.btnClearCal.Size = new System.Drawing.Size(199, 26);
      this.btnClearCal.TabIndex = 58;
      this.btnClearCal.Text = "Clear Calendar";
      this.toolTip.SetToolTip(this.btnClearCal, "Remove all appointments from calender");
      this.btnClearCal.UseVisualStyleBackColor = true;
      this.btnClearCal.Click += new System.EventHandler(this.btnClearCal_Click);
      // 
      // mcOnline
      // 
      this.mcOnline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      this.mcOnline.CalendarDimensions = new System.Drawing.Size(2, 1);
      this.mcOnline.Location = new System.Drawing.Point(3, 9);
      this.mcOnline.Name = "mcOnline";
      this.mcOnline.TabIndex = 55;
      this.mcOnline.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.mcOnline_DateChanged);
      // 
      // ScheduleView
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.Controls.Add(this.dvOnline);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.btnSaveCal);
      this.Controls.Add(this.btnClearCal);
      this.Controls.Add(this.mcOnline);
      this.Name = "ScheduleView";
      this.Size = new System.Drawing.Size(842, 540);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DateTimePicker txttimeTo;
    private Calendar.DayView dvOnline;
    private System.Windows.Forms.DateTimePicker txttimeFrom;
    private System.Windows.Forms.DateTimePicker dtpTo;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.DateTimePicker dtpFrom;
    private System.Windows.Forms.CheckBox chbade;
    private System.Windows.Forms.Button btnRecurrence;
    private System.Windows.Forms.Button btbDelete;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btCreate;
    private System.Windows.Forms.Button btnSaveCal;
    private System.Windows.Forms.Button btnClearCal;
    private System.Windows.Forms.MonthCalendar mcOnline;
    private System.Windows.Forms.ToolTip toolTip;
  }
}
