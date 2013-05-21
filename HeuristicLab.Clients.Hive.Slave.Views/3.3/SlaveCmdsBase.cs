#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Windows.Forms;
using HeuristicLab.Common;
using HeuristicLab.Core.Views;
using HeuristicLab.MainForm;


namespace HeuristicLab.Clients.Hive.SlaveCore.Views {

  [View("HeuristicLab Slave Cmds View")]
  [Content(typeof(SlaveItem), IsDefaultView = false)]
  public partial class SlaveCmdsBase : ItemView {
    protected SlaveDisplayStat lastSlaveDisplayStat;

    public new SlaveItem Content {
      get { return (SlaveItem)base.Content; }
      set {
        if (base.Content != value) {
          base.Content = value;
        }
      }
    }

    public SlaveCmdsBase() {
      InitializeComponent();
    }

    #region Register Content Events
    protected override void DeregisterContentEvents() {

      Content.SlaveDisplayStateChanged -= new EventHandler<EventArgs<SlaveDisplayStat>>(Content_SlaveDisplayStateChanged);
      Content.CoreConnectionChanged -= new EventHandler<EventArgs<CoreConnection>>(Content_CoreConnectionChanged);

      base.DeregisterContentEvents();
    }

    protected override void RegisterContentEvents() {
      base.RegisterContentEvents();

      Content.SlaveDisplayStateChanged += new EventHandler<EventArgs<SlaveDisplayStat>>(Content_SlaveDisplayStateChanged);
      Content.CoreConnectionChanged += new EventHandler<EventArgs<CoreConnection>>(Content_CoreConnectionChanged);
    }
    #endregion

    protected override void OnContentChanged() {
      base.OnContentChanged();
      btnStart.Enabled = false;
      btnStop.Enabled = false;
    }

    protected override void SetEnabledStateOfControls() {
      base.SetEnabledStateOfControls();
    }

    #region Event Handlers
    protected virtual void Content_SlaveDisplayStateChanged(object sender, EventArgs<SlaveDisplayStat> e) {
      if (this.InvokeRequired) {
        Action<object, EventArgs<SlaveDisplayStat>> action = new Action<object, EventArgs<SlaveDisplayStat>>(Content_SlaveDisplayStateChanged);
        Invoke(action, sender, e);
      } else {
        lastSlaveDisplayStat = e.Value;
        if (e.Value == SlaveDisplayStat.Asleep || e.Value == SlaveDisplayStat.NoService) {
          btnStart.Enabled = true;
          btnStop.Enabled = false;
        }

        if (e.Value == SlaveDisplayStat.Busy || e.Value == SlaveDisplayStat.Idle || e.Value == SlaveDisplayStat.Offline) {
          btnStart.Enabled = false;
          btnStop.Enabled = true;
        }
      }
    }

    protected virtual void Content_CoreConnectionChanged(object sender, EventArgs<CoreConnection> e) {
      if (e.Value == CoreConnection.Offline) {
        btnStart.Enabled = false;
        btnStop.Enabled = false;
      }
    }
    #endregion

    protected virtual void btnStop_Click(object sender, EventArgs e) {
      if (Content != null) {
        Content.Sleep();
      }
    }

    protected virtual void btnStart_Click(object sender, EventArgs e) {
      if (Content != null) {
        if (lastSlaveDisplayStat == SlaveDisplayStat.Asleep) {
          Content.RestartCore();
        }
      }
    }
  }
}
