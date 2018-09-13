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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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

    public List<HiveDowntime> offlineTimes = new List<HiveDowntime>();

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
      List<HiveDowntime> apps = new List<HiveDowntime>();

      foreach (HiveDowntime app in offlineTimes) {
        if (app.StartDate >= e.StartDate && app.StartDate <= e.EndDate && !app.Deleted) {
          apps.Add(app);
        }
      }

      e.Appointments.Clear();
      foreach (HiveDowntime app in apps) {
        e.Appointments.Add(app);
      }
    }

    private void dvOnline_OnNewAppointment(object sender, NewAppointmentEventArgs e) {
      HiveDowntime downtime = new HiveDowntime();

      downtime.StartDate = e.StartDate;
      downtime.EndDate = e.EndDate;
      offlineTimes.Add(downtime);
    }

    private void UpdateCalendarFromContent() {
      offlineTimes.Clear();
      if (Content != null) {
        foreach (Downtime downtime in Content) {
          offlineTimes.Add(ToHiveDowntime(downtime));
        }
      }
      dvOnline.Invalidate();
    }

    private bool CreateDowntime(DowntimeType dtType) {
      DateTime from, to;

      if (!string.IsNullOrEmpty(dtpFrom.Text) && !string.IsNullOrEmpty(dtpTo.Text)) {
        if (DateTime.TryParse(dtpFrom.Text, out from) && DateTime.TryParse(dtpTo.Text, out to) && from <= to) {
          //whole day appointment, only dates are visible
          if (chbade.Checked) {
            offlineTimes.Add(CreateDowntime(new DateTime(from.Year, from.Month, from.Day), (new DateTime(to.Year, to.Month, to.Day)).AddDays(1), true, dtType));
          } else {
            //Timeframe appointment
            if (from.Date == to.Date)
              offlineTimes.Add(CreateDowntime(from, to, false, dtType));
            else {
              //more than 1 day selected
              while (from.Date != to.Date) {
                offlineTimes.Add(CreateDowntime(from, new DateTime(from.Year, from.Month, from.Day, to.Hour, to.Minute, 0, 0), false, dtType));
                from = from.AddDays(1);
              }
              offlineTimes.Add(CreateDowntime(from, new DateTime(from.Year, from.Month, from.Day, to.Hour, to.Minute, 0, 0), false, dtType));
            }
          }

        } else {
          MessageBox.Show("Incorrect date format", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        dvOnline.Invalidate();
        return true;
      } else {
        MessageBox.Show("Error creating downtime, please fill out all textboxes!", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return false;
      }
    }

    private HiveDowntime CreateDowntime(DateTime startDate, DateTime endDate, bool allDay, DowntimeType downtimeType) {
      HiveDowntime downtime = new HiveDowntime();
      downtime.StartDate = startDate;
      downtime.EndDate = endDate;
      downtime.AllDayEvent = allDay;
      downtime.BorderColor = Color.Red;
      downtime.Locked = true;
      downtime.Subject = downtimeType.ToString();
      downtime.Recurring = false;
      return downtime;
    }

    private HiveDowntime CreateDowntime(DateTime startDate, DateTime endDate, bool allDay, bool recurring, Guid recurringId, DowntimeType downtimeType) {
      HiveDowntime downtime = new HiveDowntime();
      downtime.StartDate = startDate;
      downtime.EndDate = endDate;
      downtime.AllDayEvent = allDay;
      downtime.BorderColor = Color.Red;
      downtime.Locked = true;
      downtime.Subject = downtimeType.ToString();
      downtime.Recurring = recurring;
      downtime.RecurringId = recurringId;
      return downtime;
    }

    private void DeleteRecurringDowntime(Guid recurringId) {
      foreach (HiveDowntime downtime in offlineTimes) {
        if (downtime.RecurringId == recurringId) {
          downtime.Deleted = true;
        }
      }
    }

    private void ChangeRecurrenceDowntime(Guid recurringId) {
      DateTime from, to;
      if (DateTime.TryParse(dtpFrom.Text, out from) && DateTime.TryParse(dtpTo.Text, out to) && from <= to) {
        List<HiveDowntime> recurringDowntimes = offlineTimes.Where(appointment => ((HiveDowntime)appointment).RecurringId == recurringId).ToList();
        recurringDowntimes.ForEach(appointment => appointment.StartDate = new DateTime(appointment.StartDate.Year, appointment.StartDate.Month, appointment.StartDate.Day, from.Hour, 0, 0));
        recurringDowntimes.ForEach(appointment => appointment.EndDate = new DateTime(appointment.EndDate.Year, appointment.EndDate.Month, appointment.EndDate.Day, to.Hour, 0, 0));
      } else {
        MessageBox.Show("Incorrect date format", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    public void DialogClosed(RecurrentEvent e) {
      CreateDailyRecurrenceDowntimes(e.DateFrom, e.DateTo, e.AllDay, e.WeekDays, e.DowntimeType);
    }

    private void CreateDailyRecurrenceDowntimes(DateTime dateFrom, DateTime dateTo, bool allDay, HashSet<DayOfWeek> daysOfWeek, DowntimeType appointmentType) {
      DateTime incDate = dateFrom;
      Guid guid = Guid.NewGuid();

      while (incDate.Date <= dateTo.Date) {
        if (daysOfWeek.Contains(incDate.Date.DayOfWeek))
          offlineTimes.Add(CreateDowntime(incDate, new DateTime(incDate.Year, incDate.Month, incDate.Day, dateTo.Hour, dateTo.Minute, 0), allDay, true, guid, appointmentType));
        incDate = incDate.AddDays(1);
      }

      dvOnline.Invalidate();
    }

    private void btbDelete_Click(object sender, EventArgs e) {
      HiveDowntime selectedDowntime = (HiveDowntime)dvOnline.SelectedAppointment;
      if (dvOnline.SelectedAppointment != null) {
        if (!selectedDowntime.Recurring)
          DeleteDowntime();
        else {
          DialogResult res = MessageBox.Show("Delete all events in this series?", "Delete recurrences", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
          if (res != DialogResult.Yes)
            DeleteDowntime();
          else
            DeleteRecurringDowntime(selectedDowntime.RecurringId);
        }
      }
      dvOnline.Invalidate();
    }

    private void DeleteDowntime() {
      try {
        HiveDowntime downtime = offlineTimes.First(s => s.Equals((HiveDowntime)dvOnline.SelectedAppointment));
        downtime.Deleted = true;
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
      bool enabled = Content != null && !Locked;
      btCreate.Enabled = enabled;
      btbDelete.Enabled = enabled;
      btnClearCal.Enabled = enabled;
      btnRecurrence.Enabled = enabled;
      btnSaveCal.Enabled = enabled;
    }

    //public virtual void SetEnabledStateOfSchedule(bool state) {
    //  if (InvokeRequired) {
    //    Invoke(new Action(() => SetEnabledStateOfSchedule(state)));
    //  } else {
    //    if (Content == null || Locked || ReadOnly) state = false;
    //    //groupBox1.Enabled = state;
    //    btnClearCal.Enabled = state;
    //    btnSaveCal.Enabled = state;
    //  }
    //}

    private void btnClearCal_Click(object sender, System.EventArgs e) {
      foreach (HiveDowntime app in offlineTimes) {
        app.Deleted = true;
      }
      dvOnline.Invalidate();
    }

    private void chbade_CheckedChanged(object sender, EventArgs e) {
      if(chbade.Checked) {
        dtpFrom.Value = new DateTime(dtpFrom.Value.Year, dtpFrom.Value.Month, dtpFrom.Value.Day);
        dtpTo.Value = new DateTime(dtpTo.Value.Year, dtpTo.Value.Month, dtpTo.Value.Day);
      }
    }

    private void dvOnline_OnSelectionChanged(object sender, EventArgs e) {
      if (dvOnline.Selection == SelectionType.DateRange) {
        dtpFrom.Value = dvOnline.SelectionStart;
        dtpTo.Value = dvOnline.SelectionEnd.Date;
        btCreate.Text = "Create Downtime";
      }

      if (dvOnline.Selection == SelectionType.Appointment) {
        dtpFrom.Value = dvOnline.SelectedAppointment.StartDate;
        dtpTo.Value = dvOnline.SelectedAppointment.EndDate;

        if (dvOnline.SelectedAppointment.Recurring)
          //also change the caption of the save button
          btCreate.Text = "Save changes";
      }
      if (dvOnline.Selection == SelectionType.None) {
        //also change the caption of the save button
        btCreate.Text = "Create Downtime";
      }
    }

    private void mcOnline_DateChanged(object sender, DateRangeEventArgs e) {
      dvOnline.StartDate = mcOnline.SelectionStart;
    }

    private void btCreate_Click(object sender, EventArgs e) {
      if (dvOnline.Selection != SelectionType.Appointment) {
        DowntimeType dtType;
        DialogResult result;
        DowntimeTypeDialog dialog = new DowntimeTypeDialog();
        result = dialog.ShowDialog(this);
        dtType = dialog.AppointmentType;
        dialog.Dispose();
        if (result == DialogResult.Cancel) return;
        CreateDowntime(dtType);
      } else {
        //now we want to change an existing appointment
        if (!dvOnline.SelectedAppointment.Recurring) {
          if (CreateDowntime(GetDowntimeTypeOfSelectedDowntime()))
            DeleteDowntime();
        } else {
          //change recurring appointment
          //check, if only selected appointment has to change or whole recurrence
          DialogResult res = MessageBox.Show("Change all events in this series?", "Change recurrences", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
          if (res != DialogResult.Yes) {
            if (CreateDowntime(GetDowntimeTypeOfSelectedDowntime()))
              DeleteDowntime();
          } else
            ChangeRecurrenceDowntime(((HiveDowntime)dvOnline.SelectedAppointment).RecurringId);
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
        MessageBox.Show("You have to save the group before you can save the schedule. ", "HeuristicLab Hive Administrator", MessageBoxButtons.OK, MessageBoxIcon.Stop);
      } else {
        List<Downtime> downtimes = new List<Downtime>();
        foreach (HiveDowntime downtime in offlineTimes) {
          if (downtime.Deleted && downtime.Id != Guid.Empty) {
            HiveAdminClient.Delete(ToDowntime(downtime));
          } else if (downtime.Changed || downtime.Id == null || downtime.Id == Guid.Empty) {
            Downtime dt = ToDowntime(downtime);
            downtimes.Add(dt);
          }
        }
        foreach (Downtime dt in downtimes) {
          dt.Store();
        }
      }
    }

    private HiveDowntime ToHiveDowntime(Downtime downtime) {
      HiveDowntime hiveDowntime = new HiveDowntime {
        AllDayEvent = downtime.AllDayEvent,
        EndDate = downtime.EndDate,
        StartDate = downtime.StartDate,
        Recurring = downtime.Recurring,
        RecurringId = downtime.RecurringId,
        Deleted = false,
        BorderColor = Color.Red,
        Locked = true,
        Subject = downtime.DowntimeType.ToString(),
        Changed = downtime.Modified,
        Id = downtime.Id
      };
      return hiveDowntime;
    }

    private Downtime ToDowntime(HiveDowntime hiveDowntime) {
      Downtime downtime = new Downtime {
        AllDayEvent = hiveDowntime.AllDayEvent,
        EndDate = hiveDowntime.EndDate,
        StartDate = hiveDowntime.StartDate,
        Recurring = hiveDowntime.Recurring,
        RecurringId = hiveDowntime.RecurringId,
        ResourceId = HiveAdminClient.Instance.DowntimeForResourceId,
        Id = hiveDowntime.Id,
        DowntimeType = (DowntimeType)Enum.Parse(typeof(DowntimeType), hiveDowntime.Subject)
      };
      return downtime;
    }

    private DowntimeType GetDowntimeTypeOfSelectedDowntime() {
      return (DowntimeType)Enum.Parse(typeof(DowntimeType), ((HiveDowntime)dvOnline.SelectedAppointment).Subject);
    }
  }
}
