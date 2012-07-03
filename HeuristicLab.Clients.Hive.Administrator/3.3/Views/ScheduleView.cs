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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using Calendar;
using HeuristicLab.Core;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.Administrator.Views {
  [View("Schedule View")]
  [Content(typeof(IItemList<Downtime>), IsDefaultView = true)]
  public partial class ScheduleView : ItemView {
    public new IItemList<Downtime> Content {
      get { return (IItemList<Downtime>)base.Content; }
      set { base.Content = value; }
    }

    [XmlArray("Appointments")]
    [XmlArrayItem("HiveAppointment", typeof(HiveAppointment))]
    public List<HiveAppointment> offlineTimes = new List<HiveAppointment>();

    //delegate fired, if a dialog is being closed
    public delegate void OnDialogClosedDelegate(RecurrentEvent e);

    public ScheduleView() {
      InitializeComponent();
      InitCalender();
    }

    private void InitCalender() {
      dvOnline.StartDate = DateTime.Now;
      dvOnline.OnNewAppointment += new EventHandler<NewAppointmentEventArgs>(dvOnline_OnNewAppointment);
      dvOnline.OnResolveAppointments += new EventHandler<ResolveAppointmentsEventArgs>(dvOnline_OnResolveAppointments);
    }

    private void dvOnline_OnResolveAppointments(object sender, ResolveAppointmentsEventArgs e) {
      List<HiveAppointment> apps = new List<HiveAppointment>();

      foreach (HiveAppointment app in offlineTimes) {
        if (app.StartDate >= e.StartDate && app.StartDate <= e.EndDate && !app.Deleted) {
          apps.Add(app);
        }
      }

      e.Appointments.Clear();
      foreach (HiveAppointment app in apps) {
        e.Appointments.Add(app);
      }
    }

    private void dvOnline_OnNewAppointment(object sender, NewAppointmentEventArgs e) {
      HiveAppointment Appointment = new HiveAppointment();

      Appointment.StartDate = e.StartDate;
      Appointment.EndDate = e.EndDate;
      offlineTimes.Add(Appointment);
    }

    private void UpdateCalendarFromContent() {
      offlineTimes.Clear();
      if (Content != null) {
        foreach (Downtime app in Content) {
          offlineTimes.Add(ToHiveAppointment(app));
        }
      }
      dvOnline.Invalidate();
    }

    private bool CreateAppointment() {
      DateTime from, to;

      if (!string.IsNullOrEmpty(dtpFrom.Text) && !string.IsNullOrEmpty(dtpTo.Text)) {
        if (chbade.Checked) {
          //whole day appointment, only dates are visible
          if (DateTime.TryParse(dtpFrom.Text, out from) && DateTime.TryParse(dtpTo.Text, out to) && from <= to)
            offlineTimes.Add(CreateAppointment(from, to.AddDays(1), true));
          else
            MessageBox.Show("Incorrect date format", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } else if (!string.IsNullOrEmpty(txttimeFrom.Text) && !string.IsNullOrEmpty(txttimeTo.Text)) {
          //Timeframe appointment
          if (DateTime.TryParse(dtpFrom.Text + " " + txttimeFrom.Text, out from) && DateTime.TryParse(dtpTo.Text + " " + txttimeTo.Text, out to) && from < to) {
            if (from.Date == to.Date)
              offlineTimes.Add(CreateAppointment(from, to, false));
            else {
              //more than 1 day selected
              while (from.Date != to.Date) {
                offlineTimes.Add(CreateAppointment(from, new DateTime(from.Year, from.Month, from.Day, to.Hour, to.Minute, 0, 0), false));
                from = from.AddDays(1);
              }
              offlineTimes.Add(CreateAppointment(from, new DateTime(from.Year, from.Month, from.Day, to.Hour, to.Minute, 0, 0), false));
            }
          } else
            MessageBox.Show("Incorrect date format", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        dvOnline.Invalidate();
        return true;
      } else {
        MessageBox.Show("Error in create appointment, please fill out all textboxes!", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
    }

    private HiveAppointment CreateAppointment(DateTime startDate, DateTime endDate, bool allDay) {
      HiveAppointment app = new HiveAppointment();
      app.StartDate = startDate;
      app.EndDate = endDate;
      app.AllDayEvent = allDay;
      app.BorderColor = Color.Red;
      app.Locked = true;
      app.Subject = "Offline";
      app.Recurring = false;
      return app;
    }

    private HiveAppointment CreateAppointment(DateTime startDate, DateTime endDate, bool allDay, bool recurring, Guid recurringId) {
      HiveAppointment app = new HiveAppointment();
      app.StartDate = startDate;
      app.EndDate = endDate;
      app.AllDayEvent = allDay;
      app.BorderColor = Color.Red;
      app.Locked = true;
      app.Subject = "Offline";
      app.Recurring = recurring;
      app.RecurringId = recurringId;
      return app;
    }

    private void DeleteRecurringAppointment(Guid recurringId) {
      foreach (HiveAppointment app in offlineTimes) {
        if (app.RecurringId == recurringId) {
          app.Deleted = true;
        }
      }
    }

    private void ChangeRecurrenceAppointment(Guid recurringId) {
      int hourfrom = int.Parse(txttimeFrom.Text.Substring(0, txttimeFrom.Text.IndexOf(':')));
      int hourTo = int.Parse(txttimeTo.Text.Substring(0, txttimeTo.Text.IndexOf(':')));
      List<HiveAppointment> recurringAppointments = offlineTimes.Where(appointment => ((HiveAppointment)appointment).RecurringId == recurringId).ToList();
      recurringAppointments.ForEach(appointment => appointment.StartDate = new DateTime(appointment.StartDate.Year, appointment.StartDate.Month, appointment.StartDate.Day, hourfrom, 0, 0));
      recurringAppointments.ForEach(appointment => appointment.EndDate = new DateTime(appointment.EndDate.Year, appointment.EndDate.Month, appointment.EndDate.Day, hourTo, 0, 0));

      DeleteRecurringAppointment(recurringId);
      offlineTimes.AddRange(recurringAppointments);
    }

    public void DialogClosed(RecurrentEvent e) {
      CreateDailyRecurrenceAppointments(e.DateFrom, e.DateTo, e.AllDay, e.WeekDays);
    }

    private void CreateDailyRecurrenceAppointments(DateTime dateFrom, DateTime dateTo, bool allDay, HashSet<DayOfWeek> daysOfWeek) {
      DateTime incDate = dateFrom;
      Guid guid = Guid.NewGuid();

      while (incDate.Date <= dateTo.Date) {
        if (daysOfWeek.Contains(incDate.Date.DayOfWeek))
          offlineTimes.Add(CreateAppointment(incDate, new DateTime(incDate.Year, incDate.Month, incDate.Day, dateTo.Hour, dateTo.Minute, 0), allDay, true, guid));
        incDate = incDate.AddDays(1);
      }

      dvOnline.Invalidate();
    }

    private void btbDelete_Click(object sender, EventArgs e) {
      HiveAppointment selectedAppointment = (HiveAppointment)dvOnline.SelectedAppointment;
      if (dvOnline.SelectedAppointment != null) {
        if (!selectedAppointment.Recurring)
          DeleteAppointment();
        else {
          DialogResult res = MessageBox.Show("Delete all events in this series?", "Delete recurrences", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
          if (res != DialogResult.Yes)
            DeleteAppointment();
          else
            DeleteRecurringAppointment(selectedAppointment.RecurringId);
        }
      }
      dvOnline.Invalidate();
    }

    private void DeleteAppointment() {
      try {
        HiveAppointment app = offlineTimes.First(s => s.Equals((HiveAppointment)dvOnline.SelectedAppointment));
        app.Deleted = true;
      }
      catch (InvalidOperationException) {
        // this is a ui bug where a selected all day appointment is not properly selected :-/
      }
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      base.DeregisterContentEvents();
    }
    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      UpdateCalendarFromContent();
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    public virtual void SetEnabledStateOfSchedule(bool state) {
      if (InvokeRequired) {
        Invoke(new Action(() => SetEnabledStateOfSchedule(state)));
      } else {
        if (Content == null) state = false;
        groupBox1.Enabled = state;
        btnClearCal.Enabled = state;
        btnSaveCal.Enabled = state;
      }
    }

    private void btnClearCal_Click(object sender, System.EventArgs e) {
      foreach (HiveAppointment app in offlineTimes) {
        app.Deleted = true;
      }
      dvOnline.Invalidate();
    }

    private void chbade_CheckedChanged(object sender, EventArgs e) {
      txttimeFrom.Visible = !chbade.Checked;
      txttimeTo.Visible = !chbade.Checked;
    }

    private void dvOnline_OnSelectionChanged(object sender, EventArgs e) {
      if (dvOnline.Selection == SelectionType.DateRange) {
        dtpFrom.Text = dvOnline.SelectionStart.ToShortDateString();
        dtpTo.Text = dvOnline.SelectionEnd.Date.ToShortDateString();
        txttimeFrom.Text = dvOnline.SelectionStart.ToShortTimeString();
        txttimeTo.Text = dvOnline.SelectionEnd.ToShortTimeString();
        btCreate.Text = "Create Appointment";
      }

      if (dvOnline.Selection == SelectionType.Appointment) {
        dtpFrom.Text = dvOnline.SelectedAppointment.StartDate.ToShortDateString();
        dtpTo.Text = dvOnline.SelectedAppointment.EndDate.ToShortDateString();
        txttimeFrom.Text = dvOnline.SelectedAppointment.StartDate.ToShortTimeString();
        txttimeTo.Text = dvOnline.SelectedAppointment.EndDate.ToShortTimeString();

        if (dvOnline.SelectedAppointment.Recurring)
          //also change the caption of the save button
          btCreate.Text = "Save changes";
      }
      if (dvOnline.Selection == SelectionType.None) {
        //also change the caption of the save button
        btCreate.Text = "Create Appointment";
      }
    }

    private void mcOnline_DateChanged(object sender, DateRangeEventArgs e) {
      dvOnline.StartDate = mcOnline.SelectionStart;
    }

    private void btCreate_Click(object sender, EventArgs e) {
      if (dvOnline.Selection != SelectionType.Appointment) {
        CreateAppointment();
      } else {
        //now we want to change an existing appointment
        if (!dvOnline.SelectedAppointment.Recurring) {
          if (CreateAppointment())
            DeleteAppointment();
        } else {
          //change recurring appointment
          //check, if only selected appointment has to change or whole recurrence
          DialogResult res = MessageBox.Show("Change all events in this series?", "Change recurrences", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
          if (res != DialogResult.Yes) {
            if (CreateAppointment())
              DeleteAppointment();
          } else
            ChangeRecurrenceAppointment(((HiveAppointment)dvOnline.SelectedAppointment).RecurringId);
        }
      }
      dvOnline.Invalidate();
    }

    private void btnRecurrence_Click(object sender, EventArgs e) {
      Recurrence recurrence = new Recurrence();
      recurrence.dialogClosedDelegate = new OnDialogClosedDelegate(this.DialogClosed);
      recurrence.Show();
    }

    private void btnSaveCal_Click(object sender, EventArgs e) {
      if (HiveAdminClient.Instance.DowntimeForResourceId == null || HiveAdminClient.Instance.DowntimeForResourceId == Guid.Empty) {
        MessageBox.Show("You have to save the goup before you can save the schedule. ", "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      } else {
        List<Downtime> appointments = new List<Downtime>();
        foreach (HiveAppointment app in offlineTimes) {
          if (app.Deleted && app.Id != Guid.Empty) {
            HiveAdminClient.Delete(ToDowntime(app));
          } else if (app.Changed || app.Id == null || app.Id == Guid.Empty) {
            Downtime dt = ToDowntime(app);
            appointments.Add(dt);
          }
        }
        foreach (Downtime dt in appointments) {
          dt.Store();
        }
      }
    }

    private HiveAppointment ToHiveAppointment(Downtime downtime) {
      HiveAppointment app = new HiveAppointment {
        AllDayEvent = downtime.AllDayEvent,
        EndDate = downtime.EndDate,
        StartDate = downtime.StartDate,
        Recurring = downtime.Recurring,
        RecurringId = downtime.RecurringId,
        Deleted = false,
        BorderColor = Color.Red,
        Locked = true,
        Subject = "Offline",
        Changed = downtime.Modified,
        Id = downtime.Id
      };
      return app;
    }

    private Downtime ToDowntime(HiveAppointment app) {
      Downtime downtime = new Downtime {
        AllDayEvent = app.AllDayEvent,
        EndDate = app.EndDate,
        StartDate = app.StartDate,
        Recurring = app.Recurring,
        RecurringId = app.RecurringId,
        ResourceId = HiveAdminClient.Instance.DowntimeForResourceId,
        Id = app.Id
      };
      return downtime;
    }
  }
}
