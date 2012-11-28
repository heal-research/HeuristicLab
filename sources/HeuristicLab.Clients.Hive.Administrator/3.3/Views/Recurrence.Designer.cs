#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
      this.gbAppointment = new System.Windows.Forms.GroupBox();
      this.chbade = new System.Windows.Forms.CheckBox();
      this.dtpEnd = new System.Windows.Forms.DateTimePicker();
      this.dtpToTime = new System.Windows.Forms.DateTimePicker();
      this.label5 = new System.Windows.Forms.Label();
      this.dtpStart = new System.Windows.Forms.DateTimePicker();
      this.dtpFromTime = new System.Windows.Forms.DateTimePicker();
      this.label6 = new System.Windows.Forms.Label();
      this.btSaveRecurrence = new System.Windows.Forms.Button();
      this.btCancelRecurrence = new System.Windows.Forms.Button();
      this.gbWeekly = new System.Windows.Forms.GroupBox();
      this.cbSunday = new System.Windows.Forms.CheckBox();
      this.cbSaturday = new System.Windows.Forms.CheckBox();
      this.cbFriday = new System.Windows.Forms.CheckBox();
      this.cbThursday = new System.Windows.Forms.CheckBox();
      this.cbWednesday = new System.Windows.Forms.CheckBox();
      this.cbTuesday = new System.Windows.Forms.CheckBox();
      this.cbMonday = new System.Windows.Forms.CheckBox();
      this.appointmentTypeView = new HeuristicLab.Clients.Hive.Administrator.Views.AppointmentTypeView();
      this.gbDowntimeType = new System.Windows.Forms.GroupBox();
      this.gbAppointment.SuspendLayout();
      this.gbWeekly.SuspendLayout();
      this.gbDowntimeType.SuspendLayout();
      this.SuspendLayout();
      // 
      // gbAppointment
      // 
      this.gbAppointment.Controls.Add(this.chbade);
      this.gbAppointment.Controls.Add(this.dtpEnd);
      this.gbAppointment.Controls.Add(this.dtpToTime);
      this.gbAppointment.Controls.Add(this.label5);
      this.gbAppointment.Controls.Add(this.dtpStart);
      this.gbAppointment.Controls.Add(this.dtpFromTime);
      this.gbAppointment.Controls.Add(this.label6);
      this.gbAppointment.Location = new System.Drawing.Point(2, 2);
      this.gbAppointment.Name = "gbAppointment";
      this.gbAppointment.Size = new System.Drawing.Size(386, 109);
      this.gbAppointment.TabIndex = 39;
      this.gbAppointment.TabStop = false;
      this.gbAppointment.Text = "Appointment";
      // 
      // chbade
      // 
      this.chbade.AutoSize = true;
      this.chbade.Location = new System.Drawing.Point(113, 86);
      this.chbade.Name = "chbade";
      this.chbade.Size = new System.Drawing.Size(90, 17);
      this.chbade.TabIndex = 39;
      this.chbade.Text = "All Day Event";
      this.chbade.UseVisualStyleBackColor = true;
      // 
      // dtpEnd
      // 
      this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dtpEnd.Location = new System.Drawing.Point(113, 56);
      this.dtpEnd.Name = "dtpEnd";
      this.dtpEnd.Size = new System.Drawing.Size(91, 20);
      this.dtpEnd.TabIndex = 29;
      // 
      // dtpToTime
      // 
      this.dtpToTime.CustomFormat = "HH:00";
      this.dtpToTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.dtpToTime.Location = new System.Drawing.Point(226, 56);
      this.dtpToTime.Name = "dtpToTime";
      this.dtpToTime.ShowUpDown = true;
      this.dtpToTime.Size = new System.Drawing.Size(73, 20);
      this.dtpToTime.TabIndex = 38;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(58, 60);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(55, 13);
      this.label5.TabIndex = 27;
      this.label5.Text = "End Date:";
      // 
      // dtpStart
      // 
      this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.dtpStart.Location = new System.Drawing.Point(113, 19);
      this.dtpStart.Name = "dtpStart";
      this.dtpStart.Size = new System.Drawing.Size(89, 20);
      this.dtpStart.TabIndex = 28;
      // 
      // dtpFromTime
      // 
      this.dtpFromTime.CustomFormat = "HH:00";
      this.dtpFromTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.dtpFromTime.Location = new System.Drawing.Point(226, 19);
      this.dtpFromTime.Name = "dtpFromTime";
      this.dtpFromTime.ShowUpDown = true;
      this.dtpFromTime.Size = new System.Drawing.Size(73, 20);
      this.dtpFromTime.TabIndex = 30;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(58, 23);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(58, 13);
      this.label6.TabIndex = 25;
      this.label6.Text = "Start Date:";
      // 
      // btSaveRecurrence
      // 
      this.btSaveRecurrence.Location = new System.Drawing.Point(2, 298);
      this.btSaveRecurrence.Name = "btSaveRecurrence";
      this.btSaveRecurrence.Size = new System.Drawing.Size(147, 23);
      this.btSaveRecurrence.TabIndex = 43;
      this.btSaveRecurrence.Text = "Save";
      this.btSaveRecurrence.UseVisualStyleBackColor = true;
      this.btSaveRecurrence.Click += new System.EventHandler(this.btSaveRecurrence_Click);
      // 
      // btCancelRecurrence
      // 
      this.btCancelRecurrence.Location = new System.Drawing.Point(241, 298);
      this.btCancelRecurrence.Name = "btCancelRecurrence";
      this.btCancelRecurrence.Size = new System.Drawing.Size(147, 23);
      this.btCancelRecurrence.TabIndex = 44;
      this.btCancelRecurrence.Text = "Cancel";
      this.btCancelRecurrence.UseVisualStyleBackColor = true;
      this.btCancelRecurrence.Click += new System.EventHandler(this.btCancelRecurrence_Click);
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
      this.gbWeekly.Location = new System.Drawing.Point(2, 111);
      this.gbWeekly.Name = "gbWeekly";
      this.gbWeekly.Size = new System.Drawing.Size(386, 100);
      this.gbWeekly.TabIndex = 42;
      this.gbWeekly.TabStop = false;
      this.gbWeekly.Text = "Days of week";
      // 
      // cbSunday
      // 
      this.cbSunday.AutoSize = true;
      this.cbSunday.Location = new System.Drawing.Point(199, 65);
      this.cbSunday.Name = "cbSunday";
      this.cbSunday.Size = new System.Drawing.Size(62, 17);
      this.cbSunday.TabIndex = 6;
      this.cbSunday.Text = "Sunday";
      this.cbSunday.UseVisualStyleBackColor = true;
      // 
      // cbSaturday
      // 
      this.cbSaturday.AutoSize = true;
      this.cbSaturday.Location = new System.Drawing.Point(122, 65);
      this.cbSaturday.Name = "cbSaturday";
      this.cbSaturday.Size = new System.Drawing.Size(68, 17);
      this.cbSaturday.TabIndex = 5;
      this.cbSaturday.Text = "Saturday";
      this.cbSaturday.UseVisualStyleBackColor = true;
      // 
      // cbFriday
      // 
      this.cbFriday.AutoSize = true;
      this.cbFriday.Location = new System.Drawing.Point(37, 65);
      this.cbFriday.Name = "cbFriday";
      this.cbFriday.Size = new System.Drawing.Size(54, 17);
      this.cbFriday.TabIndex = 4;
      this.cbFriday.Text = "Friday";
      this.cbFriday.UseVisualStyleBackColor = true;
      // 
      // cbThursday
      // 
      this.cbThursday.AutoSize = true;
      this.cbThursday.Location = new System.Drawing.Point(289, 40);
      this.cbThursday.Name = "cbThursday";
      this.cbThursday.Size = new System.Drawing.Size(70, 17);
      this.cbThursday.TabIndex = 3;
      this.cbThursday.Text = "Thursday";
      this.cbThursday.UseVisualStyleBackColor = true;
      // 
      // cbWednesday
      // 
      this.cbWednesday.AutoSize = true;
      this.cbWednesday.Location = new System.Drawing.Point(199, 42);
      this.cbWednesday.Name = "cbWednesday";
      this.cbWednesday.Size = new System.Drawing.Size(86, 17);
      this.cbWednesday.TabIndex = 2;
      this.cbWednesday.Text = "Wednesday ";
      this.cbWednesday.UseVisualStyleBackColor = true;
      // 
      // cbTuesday
      // 
      this.cbTuesday.AutoSize = true;
      this.cbTuesday.Location = new System.Drawing.Point(123, 42);
      this.cbTuesday.Name = "cbTuesday";
      this.cbTuesday.Size = new System.Drawing.Size(67, 17);
      this.cbTuesday.TabIndex = 1;
      this.cbTuesday.Text = "Tuesday";
      this.cbTuesday.UseVisualStyleBackColor = true;
      // 
      // cbMonday
      // 
      this.cbMonday.AutoSize = true;
      this.cbMonday.Location = new System.Drawing.Point(37, 42);
      this.cbMonday.Name = "cbMonday";
      this.cbMonday.Size = new System.Drawing.Size(64, 17);
      this.cbMonday.TabIndex = 0;
      this.cbMonday.Text = "Monday";
      this.cbMonday.UseVisualStyleBackColor = true;
      // 
      // appointmentTypeView
      // 
      this.appointmentTypeView.AppointmentType = HeuristicLab.Clients.Hive.DowntimeType.Offline;
      this.appointmentTypeView.Location = new System.Drawing.Point(10, 19);
      this.appointmentTypeView.Name = "appointmentTypeView";
      this.appointmentTypeView.Size = new System.Drawing.Size(120, 50);
      this.appointmentTypeView.TabIndex = 45;
      // 
      // gbDowntimeType
      // 
      this.gbDowntimeType.Controls.Add(this.appointmentTypeView);
      this.gbDowntimeType.Location = new System.Drawing.Point(2, 217);
      this.gbDowntimeType.Name = "gbDowntimeType";
      this.gbDowntimeType.Size = new System.Drawing.Size(386, 75);
      this.gbDowntimeType.TabIndex = 46;
      this.gbDowntimeType.TabStop = false;
      this.gbDowntimeType.Text = "Type of Downtime";
      // 
      // Recurrence
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.ClientSize = new System.Drawing.Size(393, 332);
      this.Controls.Add(this.gbDowntimeType);
      this.Controls.Add(this.gbWeekly);
      this.Controls.Add(this.btCancelRecurrence);
      this.Controls.Add(this.btSaveRecurrence);
      this.Controls.Add(this.gbAppointment);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Name = "Recurrence";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Recurrence";
      this.TopMost = true;
      this.gbAppointment.ResumeLayout(false);
      this.gbAppointment.PerformLayout();
      this.gbWeekly.ResumeLayout(false);
      this.gbWeekly.PerformLayout();
      this.gbDowntimeType.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox gbAppointment;
    private System.Windows.Forms.DateTimePicker dtpEnd;
    private System.Windows.Forms.DateTimePicker dtpStart;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Button btSaveRecurrence;
    private System.Windows.Forms.Button btCancelRecurrence;
    private System.Windows.Forms.DateTimePicker dtpFromTime;
    private System.Windows.Forms.DateTimePicker dtpToTime;
    private System.Windows.Forms.CheckBox chbade;
    private System.Windows.Forms.GroupBox gbWeekly;
    private System.Windows.Forms.CheckBox cbSunday;
    private System.Windows.Forms.CheckBox cbSaturday;
    private System.Windows.Forms.CheckBox cbFriday;
    private System.Windows.Forms.CheckBox cbThursday;
    private System.Windows.Forms.CheckBox cbWednesday;
    private System.Windows.Forms.CheckBox cbTuesday;
    private System.Windows.Forms.CheckBox cbMonday;
    private AppointmentTypeView appointmentTypeView;
    private System.Windows.Forms.GroupBox gbDowntimeType;
  }
}