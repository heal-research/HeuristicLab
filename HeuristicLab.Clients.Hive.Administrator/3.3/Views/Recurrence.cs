#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;

namespace HeuristicLab.Clients.Hive.Administrator.Views {

  public partial class Recurrence : Form {

    public ScheduleView.OnDialogClosedDelegate dialogClosedDelegate;

    public Recurrence() {
      InitializeComponent();
    }

    private void btCancelRecurrence_Click(object sender, EventArgs e) {
      this.Close();
    }

    private void btSaveRecurrence_Click(object sender, EventArgs e) {
      DateTime dateFrom, dateTo;
      HashSet<DayOfWeek> days = new HashSet<DayOfWeek>();

      days = GetDays();

      //check if valid
      if (InputIsValid()) {
        dateFrom = DateTime.Parse(dtpStart.Text);
        dateTo = DateTime.Parse(dtpEnd.Text);

        RecurrentEvent recurrentEvent = new RecurrentEvent() {
          DateFrom = dateFrom,
          DateTo = dateTo,
          AllDay = chbade.Checked,
          WeekDays = days,
          DowntimeType = appointmentTypeView.DowntimeType
        };

        //fire delegate and close the dialog
        dialogClosedDelegate(recurrentEvent);
        this.Close();
      } else {
        MessageBox.Show("Incorrect date format", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private HashSet<DayOfWeek> GetDays() {
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

    private bool InputIsValid() {
      DateTime dateFrom, dateTo;

      dateFrom = DateTime.Parse(dtpStart.Text);
      dateTo = DateTime.Parse(dtpEnd.Text);

      if (chbade.Checked && dateFrom < dateTo) {
        return true;
      }

      if (!chbade.Checked && dateFrom < dateTo && dateFrom.TimeOfDay < dateTo.TimeOfDay) {
        return true;
      }

      return false;
    }
  }
}
