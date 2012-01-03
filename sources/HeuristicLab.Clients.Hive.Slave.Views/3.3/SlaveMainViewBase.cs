#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2012 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HeuristicLab.Clients.Hive.SlaveCore.Views.Properties;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {

  [View("HeuristicLab Slave Main View Base")]
  [Content(typeof(SlaveItem), IsDefaultView = false)]
  public partial class SlaveMainViewBase : ItemView {

    public event EventHandler VisibilitySwitched;
    public void OnVisibilitySwitched() {
      var handler = VisibilitySwitched;
      if (handler != null) handler(this, new EventArgs());
    }

    public new SlaveItem Content {
      get { return (SlaveItem)base.Content; }
      set {
        if (base.Content != value) {
          base.Content = value;
        }
      }
    }

    void Content_UserVisibleMessageFired(object sender, Common.EventArgs<string> e) {
      if (Settings.Default.ShowBalloonTips) {
        notifyIcon.ShowBalloonTip(2000, "HeuristicLab Hive", e.Value, ToolTipIcon.Info);
      }
    }

    public SlaveMainViewBase() {
      InitializeComponent();
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {
      Content.CoreConnectionChanged -= new EventHandler<Common.EventArgs<CoreConnection>>(Content_CoreConnectionChanged);
      Content.SlaveDisplayStateChanged -= new EventHandler<Common.EventArgs<SlaveDisplayStat>>(Content_SlaveDisplayStateChanged);
      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();
      Content.CoreConnectionChanged += new EventHandler<Common.EventArgs<CoreConnection>>(Content_CoreConnectionChanged);
      Content.SlaveDisplayStateChanged += new EventHandler<Common.EventArgs<SlaveDisplayStat>>(Content_SlaveDisplayStateChanged);
    }

    void Content_SlaveDisplayStateChanged(object sender, Common.EventArgs<SlaveDisplayStat> e) {
      if (e.Value == SlaveDisplayStat.NoService) {
        Task.Factory.StartNew(Connector);
      }
    }

    void Content_CoreConnectionChanged(object sender, Common.EventArgs<CoreConnection> e) {
      if (e.Value == CoreConnection.Offline) {
        Task.Factory.StartNew(Connector);
      }
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();

      logView.Content = Content;
      slaveView.Content = Content;
      if (Content != null) {
        Content.UserVisibleMessageFired += new System.EventHandler<Common.EventArgs<string>>(Content_UserVisibleMessageFired);
        Task.Factory.StartNew(Connector);
      } else {
        Content.UserVisibleMessageFired -= new System.EventHandler<Common.EventArgs<string>>(Content_UserVisibleMessageFired);
      }
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    private void notifyIcon_Click(object sender, MouseEventArgs e) {
      if (e.Button == MouseButtons.Left) {
        OnVisibilitySwitched();
      }
    }

    private void closeToolStripMenuItem_Click(object sender, EventArgs e) {
      Application.Exit();
    }

    private void homepageToolStripMenuItem_Click(object sender, EventArgs e) {
      System.Diagnostics.Process.Start("http://dev.heuristiclab.com");
    }

    private void notifyIcon_BalloonTipClicked(object sender, EventArgs e) {
      OnVisibilitySwitched();
    }

    private void Connector() {
      ((SlaveItem)base.Content).Open();
      bool connected = false;
      while (!connected) {
        connected = ((SlaveItem)base.Content).ReconnectToSlaveCore();

        if (!connected) {
          Thread.Sleep(Settings.Default.ServiceReconnectTimeout);
        }
      }
      this.Invoke(new Action(SetEnabledStateOfControls));
    }

    private void btnAbout_Click(object sender, EventArgs e) {
      AboutDialog dialog = new AboutDialog();
      dialog.ShowDialog();
    }
  }
}
