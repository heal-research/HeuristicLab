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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using ZedGraph;
using HeuristicLab.Hive.Client.Console.ClientService;
using System.ServiceModel;
using System.Net;
using Calendar;

namespace HeuristicLab.Hive.Client.Console {

  #region Delegates

  delegate void UpdateTextDelegate(EventLogEntry ev);

  #endregion

  public partial class HiveClientConsole : Form {

    #region Declarations

    private const string ENDPOINTADRESS = "net.tcp://127.0.0.1:8000/ClientConsole/ClientConsoleCommunicator";
    private const string EVENTLOGNAME = "Hive Client";

    private EventLog HiveClientEventLog;
    private ClientConsoleCommunicatorClient cccc;
    private System.Windows.Forms.Timer refreshTimer;
    private ListViewColumnSorterDate lvwColumnSorter;

    private List<Appointment> onlineTimes = new List<Appointment>();

    #endregion

    #region Constructor

    public HiveClientConsole() {
      InitializeComponent();
      lvwColumnSorter = new ListViewColumnSorterDate();
      lvLog.ListViewItemSorter = lvwColumnSorter;
      lvwColumnSorter.SortColumn = 3;
      lvwColumnSorter.Order = SortOrder.Descending;
      InitTimer();
      ConnectToClient();
      RefreshGui();
      GetEventLog();
      InitCalender();
    }

    #endregion

    #region Methods

    private void InitTimer() {
      refreshTimer = new System.Windows.Forms.Timer();
      refreshTimer.Interval = 1000;
      refreshTimer.Tick += new EventHandler(refreshTimer_Tick);
      refreshTimer.Start();
    }

    private void RefreshGui() {
      try {
        cccc.GetStatusInfosAsync();
      }
      catch (Exception ex) {
        refreshTimer.Stop();
        DialogResult res = MessageBox.Show("Connection Error, check if Hive Client is running!", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (res == DialogResult.OK)
          this.Close();
      }
    }

    private void ConnectToClient() {
      try {
        cccc = new ClientConsoleCommunicatorClient(new NetTcpBinding(), new EndpointAddress(ENDPOINTADRESS));
        cccc.GetStatusInfosCompleted += new EventHandler<GetStatusInfosCompletedEventArgs>(cccc_GetStatusInfosCompleted);
        cccc.GetCurrentConnectionCompleted += new EventHandler<GetCurrentConnectionCompletedEventArgs>(cccc_GetCurrentConnectionCompleted);
      }
      catch (Exception) {
        refreshTimer.Stop();
        DialogResult res = MessageBox.Show("Connection Error, check if Hive Client is running!", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (res == DialogResult.OK)
          this.Close();
      }
    }

    private void GetEventLog() {
      HiveClientEventLog = new EventLog(EVENTLOGNAME);
      HiveClientEventLog.Source = EVENTLOGNAME;
      HiveClientEventLog.EntryWritten += new EntryWrittenEventHandler(OnEntryWritten);
      HiveClientEventLog.EnableRaisingEvents = true;

      ListViewItem curEventLogEntry;

      //databinding on listview?
      if (HiveClientEventLog != null && HiveClientEventLog.Entries != null) {
        foreach (EventLogEntry ele in HiveClientEventLog.Entries) {
          curEventLogEntry = GenerateEventEntry(ele);
          lvLog.Items.Add(curEventLogEntry);
        }
        lvJobDetail.Sort();
      }
    }

    private ListViewItem GenerateEventEntry(EventLogEntry ele) {
      ListViewItem curEventLogEntry;
      curEventLogEntry = new ListViewItem("", 0);
      if (ele.EntryType == EventLogEntryType.Error)
        curEventLogEntry = new ListViewItem("", 1);
      curEventLogEntry.SubItems.Add(ele.InstanceId.ToString());
      curEventLogEntry.SubItems.Add(ele.Message);
      curEventLogEntry.SubItems.Add(ele.TimeGenerated.ToString());
      return curEventLogEntry;
    }

    private void UpdateGraph(JobStatus[] jobs) {
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

      if (jobs.Length == 0) {
        myPane.AddPieSlice(100, Color.Green, 0.1, "");
      } else {
        for (int i = 0; i < jobs.Length; i++) {
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

    private void InitCalender() {

      dvOnline.StartDate = DateTime.Now;
      dvOnline.OnNewAppointment += new EventHandler<NewAppointmentEventArgs>(DvOnline_OnNewAppointment);
      dvOnline.OnResolveAppointments += new EventHandler<ResolveAppointmentsEventArgs>(DvOnline_OnResolveAppointments);
    }

    void DvOnline_OnResolveAppointments(object sender, ResolveAppointmentsEventArgs e) {
      List<Appointment> Apps = new List<Appointment>();

      foreach (Appointment m_App in onlineTimes)
        if ((m_App.StartDate >= e.StartDate) &&
            (m_App.StartDate <= e.EndDate))
          Apps.Add(m_App);

      e.Appointments = Apps;
    }

    void DvOnline_OnNewAppointment(object sender, NewAppointmentEventArgs e) {
      Appointment Appointment = new Appointment();

      Appointment.StartDate = e.StartDate;
      Appointment.EndDate = e.EndDate;

      onlineTimes.Add(Appointment);
    }

    #endregion

    #region Events

    private void refreshTimer_Tick(object sender, EventArgs e) {
      RefreshGui();
    }

    private void cccc_GetCurrentConnectionCompleted(object sender, GetCurrentConnectionCompletedEventArgs e) {
      if (e.Error == null) {
        ConnectionContainer curConnection = e.Result;
        tbIPAdress.Text = curConnection.IPAdress;
        tbPort.Text = curConnection.Port.ToString();
      } else {
        refreshTimer.Stop();
        DialogResult res = MessageBox.Show("Connection Error, check if Hive Client is running! - " + e.Error.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (res == DialogResult.OK)
          this.Close();
      }
    }

    private void cccc_GetStatusInfosCompleted(object sender, GetStatusInfosCompletedEventArgs e) {

      if (e.Error == null) {
        StatusCommons sc = e.Result;

        lbGuid.Text = sc.ClientGuid.ToString();
        lbConnectionStatus.Text = sc.Status.ToString();
        lbJobdone.Text = sc.JobsDone.ToString();
        lbJobsAborted.Text = sc.JobsAborted.ToString();
        lbJobsFetched.Text = sc.JobsFetched.ToString();

        this.Text = "Client Console (" + sc.Status.ToString() + ")";
        lbStatus.Text = sc.Status.ToString();

        ListViewItem curJobStatusItem;

        if (sc.Jobs != null) {
          lvJobDetail.Items.Clear();
          double progress;
          foreach (JobStatus curJob in sc.Jobs) {
            curJobStatusItem = new ListViewItem(curJob.JobId.ToString());
            curJobStatusItem.SubItems.Add(curJob.Since.ToString());
            progress = curJob.Progress * 100;
            curJobStatusItem.SubItems.Add(progress.ToString());
            lvJobDetail.Items.Add(curJobStatusItem);
          }
          lvJobDetail.Sort();
        }

        UpdateGraph(sc.Jobs);

        if (sc.Status == NetworkEnumWcfConnState.Connected) {
          btConnect.Enabled = false;
          btnDisconnect.Enabled = true;
          lbCs.Text = sc.ConnectedSince.ToString();
          cccc.GetCurrentConnectionAsync();
        } else if (sc.Status == NetworkEnumWcfConnState.Disconnected) {
          btConnect.Enabled = true;
          btnDisconnect.Enabled = false;
          lbCs.Text = String.Empty;
        } else if (sc.Status == NetworkEnumWcfConnState.Failed) {
          btConnect.Enabled = true;
          btnDisconnect.Enabled = false;
          lbCs.Text = String.Empty;
        }
      } else {
        refreshTimer.Stop();
        DialogResult res = MessageBox.Show("Connection Error, check if Hive Client is running! - " + e.Error.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (res == DialogResult.OK)
          this.Close();
      }
    }

    private void HiveClientConsole_Load(object sender, EventArgs e) {
      //nothing to do
    }

    private void UpdateText(EventLogEntry ev) {
      if (this.lvLog.InvokeRequired) {
        this.lvLog.Invoke(new
          UpdateTextDelegate(UpdateText), new object[] { ev });
      } else {
        ListViewItem curEventLogEntry = GenerateEventEntry(ev);
        lvLog.Items.Add(curEventLogEntry);
        lvJobDetail.Sort();
      }
    }

    public void OnEntryWritten(object source, EntryWrittenEventArgs e) {
      UpdateText(e.Entry);
    }

    private void HiveClientConsole_Resize(object sender, EventArgs e) {
      //nothing to do
    }

    private void lvLog_DoubleClick(object sender, EventArgs e) {
      ListViewItem lvi = lvLog.SelectedItems[0];
      HiveEventEntry hee = new HiveEventEntry(lvi.SubItems[2].Text, lvi.SubItems[3].Text, lvi.SubItems[1].Text);

      Form EventlogDetails = new EventLogEntryForm(hee);
      EventlogDetails.Show();
    }

    private void btConnect_Click(object sender, EventArgs e) {
      IPAddress ipAdress;
      int port;
      ConnectionContainer cc = new ConnectionContainer();
      if (IPAddress.TryParse(tbIPAdress.Text, out ipAdress) && int.TryParse(tbPort.Text, out port)) {
        cc.IPAdress = tbIPAdress.Text;
        cc.Port = port;
        cccc.SetConnectionAsync(cc);
      } else {
        MessageBox.Show("IP Adress and/or Port Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnDisconnect_Click(object sender, EventArgs e) {
      cccc.DisconnectAsync();
    }

    private void lvLog_ColumnClick(object sender, ColumnClickEventArgs e) {
      // Determine if clicked column is already the column that is being sorted.
      if (e.Column == lvwColumnSorter.SortColumn) {
        // Reverse the current sort direction for this column.
        if (lvwColumnSorter.Order == SortOrder.Ascending) {
          lvwColumnSorter.Order = SortOrder.Descending;
        } else {
          lvwColumnSorter.Order = SortOrder.Ascending;
        }
      } else {
        // Set the column number that is to be sorted; default to ascending.
        lvwColumnSorter.SortColumn = e.Column;
        lvwColumnSorter.Order = SortOrder.Ascending;
      }

      // Perform the sort with these new sort options.
      lvLog.Sort();
    }

    private void btn_clientShutdown_Click(object sender, EventArgs e) {
      DialogResult res = MessageBox.Show("Do you really want to shutdown the Hive Client?", "Hive Client Console", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (res == DialogResult.Yes) {
        cccc.ShutdownClient();
        this.Close();
      }
    }

    #endregion

    private void mcOnline_DateChanged(object sender, DateRangeEventArgs e) {
      dvOnline.StartDate = mcOnline.SelectionStart;
    }

    private void btCreate_Click(object sender, EventArgs e) {

      if (string.IsNullOrEmpty(txtFrom.Text)) {
        Appointment App = new Appointment();
        App.StartDate = dvOnline.SelectionStart;
        App.EndDate = dvOnline.SelectionEnd;
        App.BorderColor = Color.Red;
        App.Locked = true;
        App.Subject = "Online";
        onlineTimes.Add(App);
      } else if (string.IsNullOrEmpty(txtTimeTo.Text)) {
        Appointment App = new Appointment();
        App.StartDate = DateTime.Parse(txtFrom.Text);
        App.EndDate = DateTime.Parse(txtTo.Text);
        App.BorderColor = Color.Red;
        App.Locked = true;
        App.Subject = "Online";
        onlineTimes.Add(App);
      } else {
        DateTime from = DateTime.Parse(txtFrom.Text);
        DateTime to = DateTime.Parse(txtTo.Text);

        while (from.Date != to.Date) {
          Appointment App = new Appointment();
          App.StartDate = new DateTime(from.Year, from.Month, from.Day, int.Parse(txttimeFrom.Text), 0,0);
          App.EndDate = new DateTime(from.Year, from.Month, from.Day, int.Parse(txtTimeTo.Text), 0, 0);
          App.BorderColor = Color.Red;
          App.Locked = true;
          App.Subject = "Online";
          onlineTimes.Add(App);
          from = from.AddDays(1);
        }
      }
      
      dvOnline.Invalidate();
    }

    private void btbDelete_Click(object sender, EventArgs e) {
      if (dvOnline.SelectedAppointment != null)
        onlineTimes.Remove(dvOnline.SelectedAppointment);
      dvOnline.Invalidate();
    }

  }
}