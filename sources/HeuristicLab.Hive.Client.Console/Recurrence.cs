using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HeuristicLab.Hive.Client.Console
{

  public enum RecurrenceMode
  {
    Daily,
    Weekly
  }

  public partial class Recurrence : Form
  {

    public OnDialogClosedDelegate dialogClosedDelegate;

    public Recurrence()
    {
      InitializeComponent();
    }

    private void btCancelRecurrence_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void btSaveRecurrence_Click(object sender, EventArgs e)
    {
      DateTime dateFrom, dateTo;
      RecurrenceMode mode = RecurrenceMode.Daily;
      int incWeek = 0;
      HashSet<DayOfWeek> days = new HashSet<DayOfWeek>();

      days = GetDays();

      //check if valid
      if (InputIsValid())
      {
        //set entity

        dateFrom = DateTime.Parse(dtpStart.Text + " " + dtpFromTime.Text);
        dateTo = DateTime.Parse(dtpEnd.Text + " " + dtpToTime.Text);

        if (int.TryParse(txtDays.Text, out incWeek))
          mode = RecurrenceMode.Weekly;
        else
          mode = RecurrenceMode.Daily;

        RecurrentEvent recurrentEvent = new RecurrentEvent()
        {
          DateFrom = dateFrom,
          DateTo = dateTo,
          AllDay = chbade.Checked,
          WeekDays = days,
          IncWeeks = incWeek,
        };

        //fire delegate and close the dialog
        dialogClosedDelegate(recurrentEvent);
        this.Close();
      }
      else
        MessageBox.Show("Incorrect date format", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private HashSet<DayOfWeek> GetDays()
    {
      HashSet<DayOfWeek> days = new HashSet<DayOfWeek>();

      if (cbMonday.Checked)
        days.Add(DayOfWeek.Monday);
      if (cbTuesday.Checked)
        days.Add(DayOfWeek.Tuesday);
      if (cbWednesday.Checked)
        days.Add(DayOfWeek.Wednesday);
      if (cbThursday.Checked)
        days.Add(DayOfWeek.Thursday);
      if (cbFriday.Checked)
        days.Add(DayOfWeek.Friday);
      if (cbSaturday.Checked)
        days.Add(DayOfWeek.Saturday);
      if (cbSunday.Checked)
        days.Add(DayOfWeek.Sunday);

      return days;
    }

    private bool InputIsValid()
    {
      DateTime dateFrom, dateTo;
      int i = 0;

      dateFrom = DateTime.Parse(dtpStart.Text + " " + dtpFromTime.Text);
      dateTo = DateTime.Parse(dtpEnd.Text + " " + dtpToTime.Text);

      if (dateFrom < dateTo && dateFrom.TimeOfDay < dateTo.TimeOfDay && int.TryParse(txtDays.Text, out i))
        return true;
      else
        return false;
    }
  }
}
