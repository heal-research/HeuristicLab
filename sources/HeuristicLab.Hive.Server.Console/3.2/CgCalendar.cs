using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Calendar;
using HeuristicLab.Hive.Client.Console;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Contracts;
using System.Collections;

namespace HeuristicLab.Hive.Server.ServerConsole {

  //delegate to write text in the textbox from another process
  public delegate void AppendTextDelegate(String message);

  //delegate to remove text in the textbox from another process
  public delegate void RemoveTextDelegate(int newLength, int maxChars);

  //delegate fired, if a dialog is being closed
  public delegate void OnDialogClosedDelegate(RecurrentEvent e);

  public partial class CgCalendar : Form {

    public Guid ResourceId { get; set; }

    [XmlArray("Appointments")]
    [XmlArrayItem("Appointment", typeof(Appointment))]
    public List<Appointment> onlineTimes = new List<Appointment>();

    public CgCalendar(Guid resourceId) {
      ResourceId = resourceId;
      InitializeComponent();
      InitCalender();
      UpdateCalendar();
    }

    private void InitCalender() {
      dvOnline.StartDate = DateTime.Now;
      dvOnline.OnNewAppointment += new EventHandler<NewAppointmentEventArgs>(dvOnline_OnNewAppointment);
      dvOnline.OnResolveAppointments += new EventHandler<ResolveAppointmentsEventArgs>(dvOnline_OnResolveAppointments);

      //get calender from client
      
    }

    private void UpdateCalendar() {
      onlineTimes.Clear();
      ResponseList<AppointmentDto> response = ServiceLocator.GetClientManager().GetUptimeCalendarForResource(ResourceId);
      if(response.Success) {
        foreach (AppointmentDto appointmentDto in response.List) {
          onlineTimes.Add(new Appointment {
                                            AllDayEvent = appointmentDto.AllDayEvent,
                                            EndDate = appointmentDto.EndDate,
                                            StartDate = appointmentDto.StartDate,
                                            Recurring = appointmentDto.Recurring,
                                            
                                            RecurringId = appointmentDto.RecurringId,
                                            BorderColor = Color.Red,
                                            Locked = true,
                                            Subject = "Online",
                                          });
        }
        dvOnline.Invalidate();
      }   
    }

    private bool CreateAppointment() {
      DateTime from, to;

      if (!string.IsNullOrEmpty(dtpFrom.Text) && !string.IsNullOrEmpty(dtpTo.Text)) {
        if (chbade.Checked) {
          //whole day appointment, only dates are visible
          if (DateTime.TryParse(dtpFrom.Text, out from) && DateTime.TryParse(dtpTo.Text, out to) && from <= to)
            onlineTimes.Add(CreateAppointment(from, to.AddDays(1), true));
          else
            MessageBox.Show("Incorrect date format", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        } else if (!string.IsNullOrEmpty(txttimeFrom.Text) && !string.IsNullOrEmpty(txttimeTo.Text)) {
          //Timeframe appointment
          if (DateTime.TryParse(dtpFrom.Text + " " + txttimeFrom.Text, out from) && DateTime.TryParse(dtpTo.Text + " " + txttimeTo.Text, out to) && from < to) {
            if (from.Date == to.Date)
              onlineTimes.Add(CreateAppointment(from, to, false));
            else {
              //more than 1 day selected
              while (from.Date != to.Date) {
                onlineTimes.Add(CreateAppointment(from, new DateTime(from.Year, from.Month, from.Day, to.Hour, to.Minute, 0, 0), false));
                from = from.AddDays(1);
              }
              onlineTimes.Add(CreateAppointment(from, new DateTime(from.Year, from.Month, from.Day, to.Hour, to.Minute, 0, 0), false));
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

    private Appointment CreateAppointment(DateTime startDate, DateTime endDate, bool allDay) {
      Appointment App = new Appointment();
      App.StartDate = startDate;
      App.EndDate = endDate;
      App.AllDayEvent = allDay;
      App.BorderColor = Color.Red;
      App.Locked = true;
      App.Subject = "Online";
      App.Recurring = false;
      return App;
    }

    private Appointment CreateAppointment(DateTime startDate, DateTime endDate, bool allDay, bool recurring, Guid recurringId) {
      Appointment App = new Appointment();
      App.StartDate = startDate;
      App.EndDate = endDate;
      App.AllDayEvent = allDay;
      App.BorderColor = Color.Red;
      App.Locked = true;
      App.Subject = "Online";
      App.Recurring = recurring;
      App.RecurringId = recurringId;
      return App;
    }

    private void DeleteAppointment() {
      onlineTimes.Remove(dvOnline.SelectedAppointment);
    }

    private void DeleteRecurringAppointment(Guid recurringId) {
      onlineTimes.RemoveAll(a => a.RecurringId.ToString() == dvOnline.SelectedAppointment.RecurringId.ToString());
    }

    private void ChangeRecurrenceAppointment(Guid recurringId) {
      int hourfrom = int.Parse(txttimeFrom.Text.Substring(0, txttimeFrom.Text.IndexOf(':')));
      int hourTo = int.Parse(txttimeTo.Text.Substring(0, txttimeTo.Text.IndexOf(':')));
      List<Appointment> recurringAppointments = onlineTimes.Where(appointment => appointment.RecurringId == recurringId).ToList();
      recurringAppointments.ForEach(appointment => appointment.StartDate = new DateTime(appointment.StartDate.Year, appointment.StartDate.Month, appointment.StartDate.Day, hourfrom, 0, 0));
      recurringAppointments.ForEach(appointment => appointment.EndDate = new DateTime(appointment.EndDate.Year, appointment.EndDate.Month, appointment.EndDate.Day, hourTo, 0, 0));

      DeleteRecurringAppointment(recurringId);
      onlineTimes.AddRange(recurringAppointments);
    }

    public void DialogClosed(RecurrentEvent e) {
      CreateDailyRecurrenceAppointments(e.DateFrom, e.DateTo, e.AllDay, e.IncWeeks, e.WeekDays);
    }

    private void CreateDailyRecurrenceAppointments(DateTime dateFrom, DateTime dateTo, bool allDay, int incWeek, HashSet<DayOfWeek> daysOfWeek) {
      DateTime incDate = dateFrom;
      Guid guid = Guid.NewGuid();

      while (incDate.Date <= dateTo.Date) {
        if (daysOfWeek.Contains(incDate.Date.DayOfWeek))
          onlineTimes.Add(CreateAppointment(incDate, new DateTime(incDate.Year, incDate.Month, incDate.Day, dateTo.Hour, dateTo.Minute, 0), allDay, true, guid));
        incDate = incDate.AddDays(1);
      }

      dvOnline.Invalidate();
    }

    private void btbDelete_Click(object sender, EventArgs e) {
      Appointment selectedAppointment = dvOnline.SelectedAppointment;
      if (dvOnline.SelectedAppointment != null) {
        if (!selectedAppointment.Recurring)
          DeleteAppointment();
        else {
          DialogResult res = MessageBox.Show("Delete all events in this series?", "Delete recurrences", MessageBoxButtons.YesNo);
          if (res != DialogResult.Yes)
            DeleteAppointment();
          else
            DeleteRecurringAppointment(selectedAppointment.RecurringId);
        }
      }
      dvOnline.Invalidate();
    }

    private void chbade_CheckedChanged(object sender, EventArgs e) {
      txttimeFrom.Visible = !chbade.Checked;
      txttimeTo.Visible = !chbade.Checked;
    }

    private void dvOnline_OnSelectionChanged(object sender, EventArgs e) {
      //btCreate.Enabled = true;
      if (dvOnline.Selection == SelectionType.DateRange) {
        dtpFrom.Text = dvOnline.SelectionStart.ToShortDateString();
        dtpTo.Text = dvOnline.SelectionEnd.Date.ToShortDateString();
        txttimeFrom.Text = dvOnline.SelectionStart.ToShortTimeString();
        txttimeTo.Text = dvOnline.SelectionEnd.ToShortTimeString();

        btCreate.Text = "Save";
      }

      if (dvOnline.Selection == SelectionType.Appointment) {

        dtpFrom.Text = dvOnline.SelectedAppointment.StartDate.ToShortDateString();
        dtpTo.Text = dvOnline.SelectedAppointment.EndDate.ToShortDateString();
        txttimeFrom.Text = dvOnline.SelectedAppointment.StartDate.ToShortTimeString();
        txttimeTo.Text = dvOnline.SelectedAppointment.EndDate.ToShortTimeString();

        if (dvOnline.SelectedAppointment.Recurring)
          //btCreate.Enabled = false;
          //also change the caption of the save button
          btCreate.Text = "Save changes";
      }

      if (dvOnline.Selection == SelectionType.None) {
        //also change the caption of the save button
        btCreate.Text = "Save";
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
          DialogResult res = MessageBox.Show("Change all events in this series?", "Change recurrences", MessageBoxButtons.YesNo);
          if (res != DialogResult.Yes) {
            if (CreateAppointment())
              DeleteAppointment();
          } else
            ChangeRecurrenceAppointment(dvOnline.SelectedAppointment.RecurringId);
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
      List<AppointmentDto> appointments = new List<AppointmentDto>();
      foreach(Appointment app in onlineTimes) {
        AppointmentDto apdto = new AppointmentDto {
                                                    AllDayEvent = app.AllDayEvent,
                                                    EndDate = app.EndDate,
                                                    Recurring = app.Recurring,
                                                    RecurringId = app.RecurringId,
                                                    StartDate = app.StartDate
                                                  };
        appointments.Add(apdto);
      }
      ServiceLocator.GetClientManager().SetUptimeCalendarForResource(ResourceId, appointments);      
    }

    private void dvOnline_OnResolveAppointments(object sender, ResolveAppointmentsEventArgs e) {
      List<Appointment> Apps = new List<Appointment>();

      foreach (Appointment m_App in onlineTimes)
        if ((m_App.StartDate >= e.StartDate) &&
            (m_App.StartDate <= e.EndDate))
          Apps.Add(m_App);
      e.Appointments = Apps;
    }

    private void dvOnline_OnNewAppointment(object sender, NewAppointmentEventArgs e) {
      Appointment Appointment = new Appointment();

      Appointment.StartDate = e.StartDate;
      Appointment.EndDate = e.EndDate;

      onlineTimes.Add(Appointment);
    }
  }
}
    
  
    

