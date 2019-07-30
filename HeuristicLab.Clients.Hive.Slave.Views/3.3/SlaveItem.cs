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
using System.ServiceModel;
using HeuristicLab.Clients.Hive.SlaveCore.ServiceContracts;
using HeuristicLab.Clients.Hive.SlaveCore.Views.Properties;
using HeuristicLab.Common;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive.SlaveCore.Views {

  public enum SlaveDisplayStat {
    Offline,  // not connected to Hive
    Idle,     // slave has no jobs to calculate
    Busy,     // jobs are currently running on slave
    Asleep,   // we are not accepting jobs at the moment
    NoService // the slave windows service is currently not running
  }

  public enum CoreConnection {
    Connected,
    Offline
  }

  [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
  [Item("SlaveItem", "Represents a slave which receives messages from the core")]
  public class SlaveItem : Item, ISlaveCommunicationCallbacks, IDisposable {
    private ISlaveCommunication pipeProxy;
    private DuplexChannelFactory<ISlaveCommunication> pipeFactory;
    private int lastJobsFetched = 0;

    public SlaveItem() {
    }

    private void RegisterEvents() {
      pipeFactory.Faulted += new EventHandler(pipeFactory_Faulted);
      pipeFactory.Closed += new EventHandler(pipeFactory_Closed);
      pipeFactory.Opened += new EventHandler(pipeFactory_Opened);
    }

    private void DeregisterEvents() {
      pipeFactory.Faulted -= new EventHandler(pipeFactory_Faulted);
      pipeFactory.Closed -= new EventHandler(pipeFactory_Closed);
      pipeFactory.Opened -= new EventHandler(pipeFactory_Opened);
    }

    void pipeFactory_Opened(object sender, EventArgs e) {
      OnMessageLogged("Connection to Slave core opened");
      OnCoreConnectionChanged(CoreConnection.Connected);
    }

    void pipeFactory_Closed(object sender, EventArgs e) {
      OnMessageLogged("Connection to Slave core closed");
      OnCoreConnectionChanged(CoreConnection.Offline);
    }

    void pipeFactory_Faulted(object sender, EventArgs e) {
      OnMessageLogged("Connection to Slave core faulted");
      OnCoreConnectionChanged(CoreConnection.Offline);
    }

    public void Open() {
      try {
        pipeFactory = new DuplexChannelFactory<ISlaveCommunication>(this, Settings.Default.SlaveCommunicationServiceEndpoint);
        RegisterEvents();
      }
      catch (Exception ex) {
        OnMessageLogged("Error establishing connection to Core. Are you missing a configuration file?" + Environment.NewLine + ex.ToString());
      }
    }

    public bool ReconnectToSlaveCore() {
      try {
        DeregisterEvents();
        pipeProxy = pipeFactory.CreateChannel();
        StatusCommons st = pipeProxy.Subscribe();
        if (st != null) {
          RegisterEvents();
          OnStatusChanged(st);
          return true;
        } else {
          return false;
        }
      }
      catch (Exception) {
        OnMessageLogged("Couldn't connect to Slave core. Is it possible that the core isn't running?");
        return false;
      }
    }

    public bool IsClosed() {
      if (pipeFactory == null) return true;
      return pipeFactory.State == CommunicationState.Closed || pipeFactory.State == CommunicationState.Faulted;
    }

    public void PauseAll() {
      try {
        if (pipeFactory.State != CommunicationState.Faulted && pipeFactory.State != CommunicationState.Closed)
          pipeProxy.PauseAll();
      }
      catch (Exception e) {
        OnMessageLogged("Error soft pausening core: " + e.ToString());
      }
    }

    public void StopAll() {
      try {
        if (pipeFactory.State != CommunicationState.Faulted && pipeFactory.State != CommunicationState.Closed)
          pipeProxy.StopAll();
      }
      catch (Exception e) {
        OnMessageLogged("Error hard pausening core: " + e.ToString());
      }
    }

    public void RestartCore() {
      try {
        if (pipeFactory.State != CommunicationState.Faulted && pipeFactory.State != CommunicationState.Closed)
          pipeProxy.Restart();
      }
      catch (Exception e) {
        OnMessageLogged("Error restarting core: " + e.ToString());
      }
    }

    public void Sleep() {
      try {
        if (pipeFactory.State != CommunicationState.Faulted && pipeFactory.State != CommunicationState.Closed) {
          pipeProxy.Sleep();
        }
      }
      catch (Exception e) {
        OnMessageLogged("Error sending core to sleep: " + e.ToString());
      }
    }

    public void Close() {
      if (pipeFactory.State != CommunicationState.Closed) {
        pipeProxy.Unsubscribe();
        pipeFactory.Close();
      }
    }

    public event EventHandler<EventArgs<string>> UserVisibleMessageFired;
    public void OnUserVisibleMessageFired(string msg) {
      var handler = UserVisibleMessageFired;
      if (handler != null) handler(this, new EventArgs<string>(msg));
    }

    public event EventHandler<EventArgs<SlaveDisplayStat>> SlaveDisplayStateChanged;
    public void OnSlaveDisplayStateChanged(StatusCommons status) {
      SlaveDisplayStat stat;

      if (status.Jobs.Count > 0) {
        stat = SlaveDisplayStat.Busy;
      } else {
        stat = SlaveDisplayStat.Idle;
      }
      if (status.Asleep) {
        stat = SlaveDisplayStat.Asleep;
      }
      if (status.Status == NetworkEnum.WcfConnState.Disconnected || status.Status == NetworkEnum.WcfConnState.Failed) {
        stat = SlaveDisplayStat.Offline;
      }

      var handler = SlaveDisplayStateChanged;
      if (handler != null) handler(this, new EventArgs<SlaveDisplayStat>(stat));
    }

    public void OnSlaveDisplayStateChanged(SlaveDisplayStat stat) {
      var handler = SlaveDisplayStateChanged;
      if (handler != null) handler(this, new EventArgs<SlaveDisplayStat>(stat));
    }

    public event EventHandler<EventArgs<StatusCommons>> SlaveStatusChanged;
    public void OnStatusChanged(StatusCommons status) {
      var handler = SlaveStatusChanged;
      if (handler != null) handler(this, new EventArgs<StatusCommons>(status));

      OnSlaveDisplayStateChanged(status);

      int diff = status.JobsFetched - lastJobsFetched;
      lastJobsFetched = status.JobsFetched;
      if (diff > 0) {
        if (diff == 1) {
          OnUserVisibleMessageFired("HeuristicLab Hive received 1 new task!");
        } else {
          OnUserVisibleMessageFired(string.Format("HeuristicLab Hive received {0} new jobs!", diff));
        }
      }
    }

    public event EventHandler<EventArgs<string>> SlaveMessageLogged;
    public void OnMessageLogged(string message) {
      var handler = SlaveMessageLogged;
      if (handler != null) handler(this, new EventArgs<string>(message));
    }

    public event EventHandler SlaveShutdown;
    public void OnShutdown() {
      var handler = SlaveShutdown;
      if (handler != null) handler(this, EventArgs.Empty);
      OnSlaveDisplayStateChanged(SlaveDisplayStat.NoService);
    }

    public event EventHandler<EventArgs<CoreConnection>> CoreConnectionChanged;
    public void OnCoreConnectionChanged(CoreConnection conn) {
      var handler = CoreConnectionChanged;
      if (handler != null) handler(this, new EventArgs<CoreConnection>(conn));
    }

    public void Dispose() {
      DeregisterEvents();
      Close();
    }

    public override Common.IDeepCloneable Clone(Common.Cloner cloner) {
      throw new NotImplementedException("It's not allowed to clone a SlaveItem!");
    }
  }
}
