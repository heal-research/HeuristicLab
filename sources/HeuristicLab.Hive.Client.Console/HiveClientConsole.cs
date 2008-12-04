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
using HeuristicLab.Hive.Client.Communication.ClientConsole;

namespace HeuristicLab.Hive.Client.Console {

  delegate void UpdateTextDelegate(EventLogEntry ev);

  public partial class HiveClientConsole : Form {

    EventLog HiveClientEventLog;
    int selectedEventLogId;

    public HiveClientConsole() {
      InitializeComponent();
      GetEventLog();

      ClientConsoleCommunicatorClient cccc = ServiceLocator.ClientConsoleCommunicatorClient();
      StatusCommons curClientStatus = cccc.GetStatusInfos();
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
      CreateGraph(zGJobs);
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


      }
    }

    public void OnEntryWritten(object source, EntryWrittenEventArgs e) {
      UpdateText(e.Entry);
    }

    private void SetSize() {
      zGJobs.Location = new Point(10, 10);
      // Leave a small margin around the outside of the control

      zGJobs.Size = new Size(ClientRectangle.Width - 20,
                              ClientRectangle.Height - 20);
    }


    private void CreateGraph(ZedGraphControl zgc) {
      GraphPane myPane = zgc.GraphPane;

      // Set the titles and axis labels
      myPane.Legend.IsVisible = false;
      myPane.Title.IsVisible = false;

      myPane.AddPieSlice(40, Color.Red, 0, "Jobs aborted");
      myPane.AddPieSlice(60, Color.Green, 0.1, "Jobs done");

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
  }
}