#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Windows.Forms;
using Calendar;
using HeuristicLab.Hive.Client.Console.ClientService;
using ZedGraph;
using HeuristicLab.Hive.Contracts;
using System.Xml.Serialization;
using System.IO;

namespace HeuristicLab.Hive.Client.Console
{

    #region Delegates

    public delegate void AppendTextDelegate(String message);
    public delegate void RemoveTextDelegate(int newLength, int maxChars);
    public delegate void OnDialogClosedDelegate(RecurrentEvent e);

    #endregion

    public partial class HiveClientConsole : Form
    {

        #region Declarations

        private const string ENDPOINTADRESS = "net.tcp://127.0.0.1:8000/ClientConsole/ClientConsoleCommunicator";
        //private const string EVENTLOGNAME = "Hive Client";

        //private EventLog HiveClientEventLog;
        private LogFileReader logFileReader;
        private ClientConsoleCommunicatorClient cccc;
        private System.Windows.Forms.Timer refreshTimer;
        //private ListViewColumnSorterDate lvwColumnSorter;

        [XmlArray("Appointments")]
        [XmlArrayItem("Appointment", typeof(Appointment))]
        public List<Appointment> onlineTimes = new List<Appointment>();

        public OnDialogClosedDelegate dialogClosedDelegate;

        #endregion

        #region Constructor

        public HiveClientConsole()
        {
            InitializeComponent();
            InitTimer();
            ConnectToClient();
            RefreshGui();
            InitCalender();
            InitLogFileReader();
        }

        private void InitTestCalenderEntries()
        {
            DateTime date = DateTime.Now;
            while (date.Year == 2009)
            {

                onlineTimes.Add(CreateAppointment(date.AddHours(1), date.AddHours(3), false));
                date = date.AddDays(1);
            }
        }

        #endregion

        #region Methods

        private void InitLogFileReader()
        {
            logFileReader = new LogFileReader(Environment.CurrentDirectory + @"/Hive.log");
            logFileReader.MoreData += new LogFileReader.MoreDataHandler(logFileReader_MoreData);
            logFileReader.Start();
        }

        private void logFileReader_MoreData(object sender, string newData)
        {
            int maxChars = txtLog.MaxLength;
            if (newData.Length > maxChars)
            {
                newData = newData.Remove(0, newData.Length - maxChars);
            }
            int newLength = this.txtLog.Text.Length + newData.Length;
            if (newLength > maxChars)
            {
                RemoveText(newLength, maxChars);
            }
            AppendText(newData);
        }

        private void RemoveText(int newLength, int maxChars)
        {
            if (this.txtLog.InvokeRequired)
            {
                this.txtLog.Invoke(new
                  RemoveTextDelegate(RemoveText), new object[] { newLength, maxChars });
            }
            else
            {
                this.txtLog.Text = this.txtLog.Text.Remove(0, newLength - (int)maxChars);
            }
        }

        private void InitTimer()
        {
            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = 1000;
            refreshTimer.Tick += new EventHandler(refreshTimer_Tick);
            refreshTimer.Start();
        }

        private void RefreshGui()
        {
            try
            {
                cccc.GetStatusInfosAsync();
            }
            catch (Exception ex)
            {
                refreshTimer.Stop();
                DialogResult res = MessageBox.Show("Connection Error, check if Hive Client is running!", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (res == DialogResult.OK)
                    this.Close();
            }
        }

        private void ConnectToClient()
        {
            try
            {
                //changed by MB, 16.04.09
                //cccc = new ClientConsoleCommunicatorClient(new NetTcpBinding(), new EndpointAddress(ENDPOINTADRESS));
                cccc = new ClientConsoleCommunicatorClient(WcfSettings.GetBinding(), new EndpointAddress(ENDPOINTADRESS));
                cccc.GetStatusInfosCompleted += new EventHandler<GetStatusInfosCompletedEventArgs>(cccc_GetStatusInfosCompleted);
                cccc.GetCurrentConnectionCompleted += new EventHandler<GetCurrentConnectionCompletedEventArgs>(cccc_GetCurrentConnectionCompleted);
                cccc.GetUptimeCalendarCompleted += new EventHandler<GetUptimeCalendarCompletedEventArgs>(cccc_GetUptimeCalendarCompleted);
                cccc.SetUptimeCalendarCompleted += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(cccc_SetUptimeCalendarCompleted);
            }
            catch (Exception)
            {
                refreshTimer.Stop();
                DialogResult res = MessageBox.Show("Connection Error, check if Hive Client is running!", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (res == DialogResult.OK)
                    this.Close();
            }
        }

        void cccc_SetUptimeCalendarCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                MessageBox.Show("Calendar successfully saved!", "Calender", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error saving calender \n" + e.Error.ToString(), "Calender", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void cccc_GetUptimeCalendarCompleted(object sender, GetUptimeCalendarCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result != null)
                {
                    onlineTimes = e.Result.ToList<Appointment>();
                    onlineTimes.ForEach(a => a.BorderColor = Color.Red);
                }
                else
                {
                    onlineTimes = new List<Appointment>();
                }
            }
            //InitTestCalenderEntries();
        }

        private void UpdateGraph(JobStatus[] jobs)
        {
            ZedGraphControl zgc = new ZedGraphControl();
            GraphPane myPane = zgc.GraphPane;
            myPane.GraphObjList.Clear();

            myPane.Title.IsVisible = false;  // no title
            myPane.Border.IsVisible = false; // no border
            myPane.Chart.Border.IsVisible = false; // no border around the chart
            myPane.XAxis.IsVisible = false;  // no x-axis
            myPane.YAxis.IsVisible = false;  // no y-axis
            myPane.Legend.IsVisible = false; // no legend

            myPane.Fill.Color = this.BackColor;

            myPane.Chart.Fill.Type = FillType.None;
            myPane.Fill.Type = FillType.Solid;

            double allProgress = 0;
            double done = 0;

            if (jobs.Length == 0)
            {
                myPane.AddPieSlice(100, Color.Green, 0.1, "");
            }
            else
            {
                for (int i = 0; i < jobs.Length; i++)
                {
                    allProgress += jobs[i].Progress;
                }

                done = allProgress / jobs.Length;

                myPane.AddPieSlice(done, Color.Green, 0, "");
                myPane.AddPieSlice(1 - done, Color.Red, 0, "");
            }
            //Hides the slice labels
            PieItem.Default.LabelType = PieLabelType.None;

            myPane.AxisChange();

            pbGraph.Image = zgc.GetImage();
        }

        private void InitCalender()
        {
            dvOnline.StartDate = DateTime.Now;
            dvOnline.OnNewAppointment += new EventHandler<NewAppointmentEventArgs>(DvOnline_OnNewAppointment);
            dvOnline.OnResolveAppointments += new EventHandler<ResolveAppointmentsEventArgs>(DvOnline_OnResolveAppointments);

            //get calender from client
            cccc.GetUptimeCalendarAsync();
        }

        private Appointment CreateAppointment(DateTime startDate, DateTime endDate, bool allDay)
        {
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

        private Appointment CreateAppointment(DateTime startDate, DateTime endDate, bool allDay, bool recurring, Guid recurringId)
        {
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

        private void DeleteAppointment()
        {
            onlineTimes.Remove(dvOnline.SelectedAppointment);
        }

        private void DeleteRecurringAppointment(Guid recurringId)
        {
            onlineTimes.RemoveAll(a => a.RecurringId.ToString() == dvOnline.SelectedAppointment.RecurringId.ToString());
        }

        #endregion

        #region Events

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshGui();
        }

        private void cccc_GetCurrentConnectionCompleted(object sender, GetCurrentConnectionCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ConnectionContainer curConnection = e.Result;
                tbIPAdress.Text = curConnection.IPAdress;
                tbPort.Text = curConnection.Port.ToString();
            }
            else
            {
                refreshTimer.Stop();
                DialogResult res = MessageBox.Show("Connection Error, check if Hive Client is running! - " + e.Error.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (res == DialogResult.OK)
                    this.Close();
            }
        }

        private void cccc_GetStatusInfosCompleted(object sender, GetStatusInfosCompletedEventArgs e)
        {

            if (e.Error == null)
            {
                StatusCommons sc = e.Result;

                lbGuid.Text = sc.ClientGuid.ToString();
                lbConnectionStatus.Text = sc.Status.ToString();
                lbJobdone.Text = sc.JobsDone.ToString();
                lbJobsAborted.Text = sc.JobsAborted.ToString();
                lbJobsFetched.Text = sc.JobsFetched.ToString();

                this.Text = "Client Console (" + sc.Status.ToString() + ")";

                ListViewItem curJobStatusItem;

                if (sc.Jobs != null)
                {
                    lvJobDetail.Items.Clear();
                    double progress;
                    foreach (JobStatus curJob in sc.Jobs)
                    {
                        curJobStatusItem = new ListViewItem(curJob.JobId.ToString());
                        curJobStatusItem.SubItems.Add(curJob.Since.ToString());
                        progress = curJob.Progress * 100;
                        curJobStatusItem.SubItems.Add(progress.ToString());
                        lvJobDetail.Items.Add(curJobStatusItem);
                    }
                    lvJobDetail.Sort();
                }

                UpdateGraph(sc.Jobs);

                if (sc.Status == NetworkEnumWcfConnState.Connected || sc.Status == NetworkEnumWcfConnState.Loggedin)
                {
                    btConnect.Enabled = false;
                    btnDisconnect.Enabled = true;
                    lbCs.Text = sc.ConnectedSince.ToString();
                    cccc.GetCurrentConnectionAsync();
                }
                else if (sc.Status == NetworkEnumWcfConnState.Disconnected)
                {
                    btConnect.Enabled = true;
                    btnDisconnect.Enabled = false;
                    lbCs.Text = String.Empty;
                }
                else if (sc.Status == NetworkEnumWcfConnState.Failed)
                {
                    btConnect.Enabled = true;
                    btnDisconnect.Enabled = false;
                    lbCs.Text = String.Empty;
                }

                cccc.GetCurrentConnection();
            }
            else
            {
                refreshTimer.Stop();
                DialogResult res = MessageBox.Show("Connection Error, check if Hive Client is running! - " + e.Error.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (res == DialogResult.OK)
                    this.Close();
            }
        }

        private void HiveClientConsole_Load(object sender, EventArgs e)
        {
            //nothing to do
        }

        private void AppendText(string message)
        {
            if (this.txtLog.InvokeRequired)
            {
                this.txtLog.Invoke(new
                  AppendTextDelegate(AppendText), new object[] { message });
            }
            else
            {
                this.txtLog.AppendText(message);
            }
        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            IPAddress ipAdress;
            int port;
            ConnectionContainer cc = new ConnectionContainer();
            if (IPAddress.TryParse(tbIPAdress.Text, out ipAdress) && int.TryParse(tbPort.Text, out port))
            {
                cc.IPAdress = tbIPAdress.Text;
                cc.Port = port;
                cccc.SetConnectionAsync(cc);
            }
            else
            {
                MessageBox.Show("IP Adress and/or Port Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            cccc.DisconnectAsync();
        }

        private void btn_clientShutdown_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Do you really want to shutdown the Hive Client?", "Hive Client Console", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                logFileReader.Stop();
                cccc.ShutdownClient();
                this.Close();
            }
        }

        private void btbDelete_Click(object sender, EventArgs e)
        {
            Appointment selectedAppointment = dvOnline.SelectedAppointment;
            if (dvOnline.SelectedAppointment != null)
            {
                if (!selectedAppointment.Recurring)
                    DeleteAppointment();
                else
                {
                    DialogResult res = MessageBox.Show("Delete all events in this series?", "Delete recurrences", MessageBoxButtons.YesNo);
                    if (res != DialogResult.Yes)
                        DeleteAppointment();
                    else
                        DeleteRecurringAppointment(selectedAppointment.RecurringId);
                }
            }
            dvOnline.Invalidate();
        }

        private void chbade_CheckedChanged(object sender, EventArgs e)
        {
            txttimeFrom.Visible = !chbade.Checked;
            txttimeTo.Visible = !chbade.Checked;
        }

        private void dvOnline_OnSelectionChanged(object sender, EventArgs e)
        {
            //btCreate.Enabled = true;
            if (dvOnline.Selection == SelectionType.DateRange)
            {
                dtpFrom.Text = dvOnline.SelectionStart.ToShortDateString();
                dtpTo.Text = dvOnline.SelectionEnd.Date.ToShortDateString();
                txttimeFrom.Text = dvOnline.SelectionStart.ToShortTimeString();
                txttimeTo.Text = dvOnline.SelectionEnd.ToShortTimeString();

                btCreate.Text = "Save";
            }

            if (dvOnline.Selection == SelectionType.Appointment)
            {

                dtpFrom.Text = dvOnline.SelectedAppointment.StartDate.ToShortDateString();
                dtpTo.Text = dvOnline.SelectedAppointment.EndDate.ToShortDateString();
                txttimeFrom.Text = dvOnline.SelectedAppointment.StartDate.ToShortTimeString();
                txttimeTo.Text = dvOnline.SelectedAppointment.EndDate.ToShortTimeString();

                if (dvOnline.SelectedAppointment.Recurring)
                    //btCreate.Enabled = false;
                    //also change the caption of the save button
                    btCreate.Text = "Save changes";
            }

            if (dvOnline.Selection == SelectionType.None)
            {
                //also change the caption of the save button
                btCreate.Text = "Save";
            }

        }

        private void Connection_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                btConnect_Click(null, null);
        }

        private void mcOnline_DateChanged(object sender, DateRangeEventArgs e)
        {
            dvOnline.StartDate = mcOnline.SelectionStart;
        }

        private bool CreateAppointment()
        {
            DateTime from, to;

            if (!string.IsNullOrEmpty(dtpFrom.Text) && !string.IsNullOrEmpty(dtpTo.Text))
            {
                if (chbade.Checked)
                {
                    //whole day appointment, only dates are visible
                    if (DateTime.TryParse(dtpFrom.Text, out from) && DateTime.TryParse(dtpTo.Text, out to) && from <= to)
                        onlineTimes.Add(CreateAppointment(from, to.AddDays(1), true));
                    else
                        MessageBox.Show("Incorrect date format", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (!string.IsNullOrEmpty(txttimeFrom.Text) && !string.IsNullOrEmpty(txttimeTo.Text))
                {
                    //Timeframe appointment
                    if (DateTime.TryParse(dtpFrom.Text + " " + txttimeFrom.Text, out from) && DateTime.TryParse(dtpTo.Text + " " + txttimeTo.Text, out to) && from < to)
                    {
                        if (from.Date == to.Date)
                            onlineTimes.Add(CreateAppointment(from, to, false));
                        else
                        {
                            //more than 1 day selected
                            while (from.Date != to.Date)
                            {
                                onlineTimes.Add(CreateAppointment(from, new DateTime(from.Year, from.Month, from.Day, to.Hour, to.Minute, 0, 0), false));
                                from = from.AddDays(1);
                            }
                            onlineTimes.Add(CreateAppointment(from, new DateTime(from.Year, from.Month, from.Day, to.Hour, to.Minute, 0, 0), false));
                        }
                    }
                    else
                        MessageBox.Show("Incorrect date format", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                dvOnline.Invalidate();
                return true;
            }
            else
            {
                MessageBox.Show("Error in create appointment, please fill out all textboxes!", "Schedule Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            if (dvOnline.Selection != SelectionType.Appointment)
            {
                CreateAppointment();
            }
            else
            {
                //now we want to change an existing appointment
                if (!dvOnline.SelectedAppointment.Recurring)
                {
                    if (CreateAppointment())
                        DeleteAppointment();
                }
                else
                {
                    //change recurring appointment
                    //check, if only selected appointment has to change or whole recurrence
                    DialogResult res = MessageBox.Show("Change all events in this series?", "Change recurrences", MessageBoxButtons.YesNo);
                    if (res != DialogResult.Yes)
                    {
                        if (CreateAppointment())
                            DeleteAppointment();
                    }
                    else
                        ChangeRecurrenceAppointment(dvOnline.SelectedAppointment.RecurringId);
                }
            }
            dvOnline.Invalidate();
        }

        private void ChangeRecurrenceAppointment(Guid recurringId)
        {
            int hourfrom = int.Parse(txttimeFrom.Text.Substring(0, txttimeFrom.Text.IndexOf(':')));
            int hourTo = int.Parse(txttimeTo.Text.Substring(0, txttimeTo.Text.IndexOf(':')));
            List<Appointment> recurringAppointments = onlineTimes.Where(appointment => appointment.RecurringId == recurringId).ToList();
            recurringAppointments.ForEach(appointment => appointment.StartDate = new DateTime(appointment.StartDate.Year, appointment.StartDate.Month, appointment.StartDate.Day, hourfrom, 0, 0));
            recurringAppointments.ForEach(appointment => appointment.EndDate = new DateTime(appointment.EndDate.Year, appointment.EndDate.Month, appointment.EndDate.Day, hourTo, 0, 0));

            DeleteRecurringAppointment(recurringId);
            onlineTimes.AddRange(recurringAppointments);
        }

        private void DvOnline_OnResolveAppointments(object sender, ResolveAppointmentsEventArgs e)
        {
            List<Appointment> Apps = new List<Appointment>();

            foreach (Appointment m_App in onlineTimes)
                if ((m_App.StartDate >= e.StartDate) &&
                    (m_App.StartDate <= e.EndDate))
                    Apps.Add(m_App);
            e.Appointments = Apps;
        }

        private void DvOnline_OnNewAppointment(object sender, NewAppointmentEventArgs e)
        {
            Appointment Appointment = new Appointment();

            Appointment.StartDate = e.StartDate;
            Appointment.EndDate = e.EndDate;

            onlineTimes.Add(Appointment);
        }

        private void btnRecurrence_Click(object sender, EventArgs e)
        {
            Recurrence recurrence = new Recurrence();
            recurrence.dialogClosedDelegate = new OnDialogClosedDelegate(this.DialogClosed);
            recurrence.Show();
        }

        public void DialogClosed(RecurrentEvent e)
        {
            CreateDailyRecurrenceAppointments(e.DateFrom, e.DateTo, e.AllDay, e.IncWeeks, e.WeekDays);
        }

        private void CreateDailyRecurrenceAppointments(DateTime dateFrom, DateTime dateTo, bool allDay, int incWeek, HashSet<DayOfWeek> daysOfWeek)
        {
            DateTime incDate = dateFrom;
            Guid guid = Guid.NewGuid();

            while (incDate.Date <= dateTo.Date)
            {
                if (daysOfWeek.Contains(incDate.Date.DayOfWeek))
                    onlineTimes.Add(CreateAppointment(incDate, new DateTime(incDate.Year, incDate.Month, incDate.Day, dateTo.Hour, dateTo.Minute, 0), allDay, true, guid));
                incDate = incDate.AddDays(1);
            }

            dvOnline.Invalidate();
        }

        private void btnSaveCal_Click(object sender, EventArgs e)
        {
            cccc.SetUptimeCalendarAsync(onlineTimes.ToArray());
        }

        #endregion

    }
}