#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Diagnostics;
using System.Management;
using System.Security.Principal;
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.SlaveCore.Views;
using HeuristicLab.Clients.Hive.SlaveCore.Views.Properties;

namespace HeuristicLab.Clients.Hive.SlaveCore.SlaveTrayIcon {
  static class Program {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args) {
      if (args.Length < 1 || (args.Length > 0 && args[0] != Settings.Default.ShowUICmd)) {
        KillOtherInstances();
      }

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      MainWindow mw = new MainWindow();
      if (args.Length < 1 || (args.Length > 0 && args[0] != Settings.Default.ShowUICmd)) {
        mw.Hide();
      } else if (args.Length > 0 && args[0] == Settings.Default.ShowUICmd) {
        mw.Show();
      }

      mw.Content = new SlaveItem();
      Application.Run();
    }

    /// <summary>
    /// Gets the owner of a process
    /// (based on http://www.codeproject.com/KB/cs/processownersid.aspx)
    /// </summary>    
    private static string GetProcessOwner(int pid) {
      string ownerSID = String.Empty;
      string processName = String.Empty;
      try {
        ObjectQuery sq = new ObjectQuery
            ("Select * from Win32_Process Where ProcessID = '" + pid + "'");
        ManagementObjectSearcher searcher = new ManagementObjectSearcher(sq);
        if (searcher.Get().Count == 0)
          return ownerSID;
        foreach (ManagementObject oReturn in searcher.Get()) {
          string[] sid = new String[1];
          oReturn.InvokeMethod("GetOwnerSid", (object[])sid);
          ownerSID = sid[0];
          return ownerSID;
        }
      }
      catch {
        return ownerSID;
      }
      return ownerSID;
    }

    /// <summary>
    /// Kill all other slave tray icons, we only want 1 running at a time
    /// (and if a newer version is installed the older one should be killed)
    /// </summary>
    private static void KillOtherInstances() {
      String currentUserSID = WindowsIdentity.GetCurrent().User.Value;
      Process curProc = Process.GetCurrentProcess();

      try {
        Process[] procs = Process.GetProcessesByName(curProc.ProcessName);
        foreach (Process p in procs) {
          if (p.Id != curProc.Id) {
            String procUserSID = GetProcessOwner(p.Id);
            if (procUserSID == currentUserSID) {
              p.Kill();
            }
          }
        }
      }
      catch (InvalidOperationException) { }
      catch (Exception) {
        MessageBox.Show("There is another instance of the Hive Slave tray icon running which can't be closed.", "HeuristicLab Hive Slave");
      }
    }
  }
}
