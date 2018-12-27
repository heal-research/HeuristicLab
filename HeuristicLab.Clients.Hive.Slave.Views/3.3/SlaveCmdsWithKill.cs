#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2019 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.SlaveCore.Views.Properties;
using HeuristicLab.Common;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {

  [View("HeuristicLab Slave Cmds with Kill View")]
  [Content(typeof(SlaveItem), IsDefaultView = false)]
  public partial class SlaveCmdsWithKill : SlaveCmdsBase {
    private string serviceName = Settings.Default.ServiceName;

    private const UInt32 BCM_SETSHIELD = 0x160C;
    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
    static extern int SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, IntPtr lParam);


    public new SlaveItem Content {
      get { return (SlaveItem)base.Content; }
      set {
        if (base.Content != value) {
          base.Content = value;
        }
      }
    }

    public SlaveCmdsWithKill() {
      InitializeComponent();

      if (CheckRunAsAdmin()) {
        btnKill.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
        btnKill.Image = HeuristicLab.Common.Resources.VSImageLibrary.Stop;
      } else {
        this.btnKill.FlatStyle = FlatStyle.System;
        SendMessage(btnKill.Handle, BCM_SETSHIELD, 0, (IntPtr)1);
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

    protected override void Content_SlaveDisplayStateChanged(object sender, EventArgs<SlaveDisplayStat> e) {
      base.Content_SlaveDisplayStateChanged(sender, e);

      if (e.Value == SlaveDisplayStat.Asleep || e.Value == SlaveDisplayStat.NoService) {
        btnKill.Enabled = false;
      }

      if (e.Value == SlaveDisplayStat.Busy || e.Value == SlaveDisplayStat.Idle || e.Value == SlaveDisplayStat.Offline) {
        btnKill.Enabled = true;
      }
    }

    protected override void Content_CoreConnectionChanged(object sender, EventArgs<CoreConnection> e) {
      base.Content_CoreConnectionChanged(sender, e);
      if (e.Value == CoreConnection.Offline) {
        btnKill.Enabled = false;
      }
    }

    private bool CheckRunAsAdmin() {
      bool isRunAsAdmin = false;
      WindowsIdentity user = WindowsIdentity.GetCurrent();
      WindowsPrincipal principal = new WindowsPrincipal(user);

      try {
        isRunAsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
      }
      catch { }
      return isRunAsAdmin;
    }

    /// <summary>
    /// Shows the windows UAC dialog and restarts tray icon app
    /// </summary>
    private void ElevateApplication() {
      // launch itself as administrator
      ProcessStartInfo proc = new ProcessStartInfo(Application.ExecutablePath, Settings.Default.ShowUICmd);
      proc.UseShellExecute = true;
      proc.WorkingDirectory = Environment.CurrentDirectory;
      proc.FileName = Application.ExecutablePath;
      proc.Verb = "runas";

      try {
        Process.Start(proc);
      }
      catch {
        // user refused to allow privileges elevation       
        return;
      }
      Application.Exit();
    }

    private void StartService() {
      ServiceController service = new ServiceController(serviceName);
      try {
        if (service.Status == ServiceControllerStatus.Running) {
          service.Stop();
          service.WaitForStatus(ServiceControllerStatus.Stopped, Settings.Default.ServiceStartStopTimeout);
        }

        service.Start();
        service.WaitForStatus(ServiceControllerStatus.Running, Settings.Default.ServiceStartStopTimeout);
      }
      catch (InvalidOperationException ex) {
        MessageBox.Show("Error starting service: Hive Slave Service not found!" + Environment.NewLine + ex.ToString());
      }
      catch (Exception ex) {
        MessageBox.Show("Error starting service, exception is: " + Environment.NewLine + ex.ToString());
      }
    }

    private void StopService() {
      ServiceController service = new ServiceController(serviceName);
      try {
        if (service.Status == ServiceControllerStatus.Running) {
          service.Stop();
          service.WaitForStatus(ServiceControllerStatus.Stopped, Settings.Default.ServiceStartStopTimeout);
        }
      }
      catch (InvalidOperationException ex) {
        MessageBox.Show("Error stopping service: Hive Slave Service not found!" + Environment.NewLine + ex.ToString());
      }
      catch (Exception ex) {
        MessageBox.Show("Error stopping service, exception is: " + Environment.NewLine + ex.ToString());
      }
    }

    protected override void OnContentChanged() {
      base.OnContentChanged();
      btnKill.Enabled = false;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    private void btnKill_Click(object sender, EventArgs e) {
      if (CheckRunAsAdmin()) {
        StopService();
      } else {
        ElevateApplication();
      }
    }

    protected override void btnStart_Click(object sender, EventArgs e) {
      if (Content != null) {
        if (lastSlaveDisplayStat == SlaveDisplayStat.Asleep) {
          Content.RestartCore();
        } else if (lastSlaveDisplayStat == SlaveDisplayStat.NoService) {
          if (CheckRunAsAdmin()) {
            StartService();
          } else {
            ElevateApplication();
          }
        }
      }
    }
  }
}
