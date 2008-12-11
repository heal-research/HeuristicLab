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

namespace HeuristicLab.Hive.Client.Console {

  delegate void UpdateTextDelegate(EventLogEntry ev);

  public partial class HiveClientConsole : Form {

    EventLog HiveClientEventLog;
    ClientConsoleCommunicatorClient cccc;
    System.Windows.Forms.Timer refreshTimer;

    public HiveClientConsole() {
      InitializeComponent();
      InitTimer();
      ConnectToClient();
      RefreshGui();
      GetEventLog();
    }

    private void InitTimer() {
      refreshTimer = new System.Windows.Forms.Timer();
      refreshTimer.Interval = 1000;
      refreshTimer.Tick += new EventHandler(refreshTimer_Tick);
      refreshTimer.Start();
    }

    void refreshTimer_Tick(object sender, EventArgs e) {
      RefreshGui();
    }
    
    private void RefreshGui() {
      StatusCommons sc = new StatusCommons();
      
      try {
        sc = cccc.GetStatusInfos();
      }
      catch (Exception ex) {
        refreshTimer.Stop();
        DialogResult res = MessageBox.Show("Connection Error, check if Hive Client is running!", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (res == DialogResult.OK)
          this.Close();
      }

      lbGuid.Text = sc.ClientGuid.ToString();
      lbCs.Text = sc.ConnectedSince.ToString();
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
      }

      UpdateGraph(zGJobs, sc.JobsDone, sc.JobsAborted);
 
      if (sc.Status == NetworkEnumWcfConnState.Connected) {
        btConnect.Enabled = false;
        btnDisconnect.Enabled = true;
        ConnectionContainer curConnection = cccc.GetCurrentConnection();
        tbIPAdress.Text = curConnection.IPAdress;
        tbPort.Text = curConnection.Port.ToString();
      } else if (sc.Status == NetworkEnumWcfConnState.Disconnected) {
        btConnect.Enabled = true;
        btnDisconnect.Enabled = false;
      } else if (sc.Status == NetworkEnumWcfConnState.Failed) {
        btConnect.Enabled = true;
        btnDisconnect.Enabled = false;
      }
    }

    private void ConnectToClient() {
      try {
        cccc = new ClientConsoleCommunicatorClient();
      }
      catch (Exception) {
        refreshTimer.Stop();
        DialogResult res = MessageBox.Show("Connection Error, check if Hive Client is running!", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        if (res == DialogResult.OK)
          this.Close();
      }
    }

    private void GetEventLog() {
      HiveClientEventLog = new EventLog("Hive Client");
      HiveClientEventLog.Source = "Hive Client";
      HiveClientEventLog.EntryWritten += new EntryWrittenEventHandler(OnEntryWritten);
      HiveClientEventLog.EnableRaisingEvents = true;

      ListViewItem curEventLogEntry;
      foreach (EventLogEntry eve in HiveClientEventLog.Entries) {
        curEventLogEntry = new ListViewItem("", 0);
        if(eve.EntryType == EventLogEntryType.Error)
          curEventLogEntry = new ListViewItem("", 1);
        curEventLogEntry.SubItems.Add(eve.InstanceId.ToString());
        curEventLogEntry.SubItems.Add(eve.Message);
        curEventLogEntry.SubItems.Add(eve.TimeGenerated.Date.ToString());
        curEventLogEntry.SubItems.Add(eve.TimeGenerated.TimeOfDay.ToString());
        lvLog.Items.Add(curEventLogEntry);
      }
    }

    private void HiveClientConsole_Load(object sender, EventArgs e) {
      //SetSize();
    }

    private void UpdateText(EventLogEntry ev) {
      if (this.lvLog.InvokeRequired) {
        this.lvLog.Invoke(new
          UpdateTextDelegate(UpdateText), new object[] { ev });
      } else {
        ListViewItem curEventLogEntry;
        curEventLogEntry = new ListViewItem("", 0);
        if (ev.EntryType == EventLogEntryType.Error)
          curEventLogEntry = new ListViewItem("", 1);
        curEventLogEntry.SubItems.Add(ev.EventID.ToString());
        curEventLogEntry.SubItems.Add(ev.Message);
        curEventLogEntry.SubItems.Add(ev.TimeGenerated.Date.ToString());
        curEventLogEntry.SubItems.Add(ev.TimeGenerated.TimeOfDay.ToString());
        lvLog.Items.Add(curEventLogEntry);
      }
    }

    public void OnEntryWritten(object source, EntryWrittenEventArgs e) {
      UpdateText(e.Entry);
    }

    private void UpdateGraph(ZedGraphControl zgc, int jobsDone, int jobsAborted) {
      zgc.GraphPane.GraphObjList.Clear();
      GraphPane myPane = zgc.GraphPane;

      // Set the titles and axis labels
      myPane.Legend.IsVisible = false;
      myPane.Title.IsVisible = false;
      myPane.Fill.Type = FillType.None;

      double sum = jobsDone + jobsAborted;
      double perDone = jobsDone / sum * 100;
      double perAborted = jobsAborted / sum * 100;

      myPane.AddPieSlice(perAborted, Color.Red, 0, "Jobs aborted");
      myPane.AddPieSlice(perDone, Color.Green, 0.1, "Jobs done");
      myPane.AxisChange();
    }

    private void HiveClientConsole_Resize(object sender, EventArgs e) {
      //SetSize();
    }

    private void lvLog_DoubleClick(object sender, EventArgs e) {
      ListViewItem lvi = lvLog.SelectedItems[0];
      HiveEventEntry hee = new HiveEventEntry(lvi.SubItems[2].Text, lvi.SubItems[3].Text, lvi.SubItems[4].Text, lvi.SubItems[1].Text);
      
      Form EventlogDetails = new EventLogEntryForm(hee);
      EventlogDetails.Show();
    }

    private void btConnect_Click(object sender, EventArgs e) {
      IPAddress ipAdress;
      int port;
      ConnectionContainer cc = new ConnectionContainer();
      //IPAddress.TryParse(tbIPAdress.Text.ToString(), ipAdress);
      if (IPAddress.TryParse(tbIPAdress.Text, out ipAdress) && int.TryParse(tbPort.Text, out port)) {
        cc.IPAdress = tbIPAdress.Text;
        cc.Port = port;
        cccc.SetConnection(cc);
      } else {
        MessageBox.Show("IP Adress and/or Port Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      
    }

    private void btnDisconnect_Click(object sender, EventArgs e) {
      cccc.Disconnect();
    }
  }
}